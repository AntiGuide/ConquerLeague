using System;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

public class CommunicationNet : MonoBehaviour {
    public static CommunicationNet FakeStatic;

    [SerializeField]
    private PlayerNet enemyPlayerNet;

    [SerializeField]
    private PlayerNet friendlyPlayerNet;

    [SerializeField]
    private NetDeliveryMethod deliveryMethod;

    [SerializeField]
    private Transform StartPointLeft;

    [SerializeField]
    private Transform StartPointRight;

    private NetClient client;
    private NetConnection connection;
    private List<byte[]> sendQueue = new List<byte[]>();
    private List<NetDeliveryMethod> sendMethodQueue = new List<NetDeliveryMethod>();
    private bool isLeft;

    public bool ConnectedToServer { get; set; }

    private enum GameMessageType : byte {
        PLAYER_MOVEMENT = 0,
        SESSION_INITIALITZE = 1
    }

    public void RecievePlayerMovement(byte[] input) {
        // 0 = GameMessageType
        // 1 - 12 = position
        var position = ByteArrayToVector3(input, 1);
        // 13 - 28 = rotation
        var quaternion = ByteArrayToQuaternion(input, 13);
        // 29 - 40
        var velocity = ByteArrayToVector3(input, 29);
        // 41 = hp
        var hp = input[41];

        enemyPlayerNet.SetNewMovementPack(position, quaternion, velocity, hp);
    }

    public void RecieveSessionInitialize(byte[] input) {
        // 0 = GameMessageType
        // 1 = Left or Right
        isLeft = BitConverter.ToBoolean(input, 1);
        // Vehicle type (To be included)
        // Weapon type (To be included)
        var startFriendly = isLeft ? StartPointLeft : StartPointRight;
        var startEnemy = isLeft ? StartPointRight : StartPointLeft;

        friendlyPlayerNet.SetNewMovementPack(startFriendly.position, startFriendly.rotation, Vector3.zero);
        enemyPlayerNet.SetNewMovementPack(startEnemy.position, startEnemy.rotation, Vector3.zero);
    }

    private Vector3 ByteArrayToVector3(byte[] input, int startIndex) {
        var x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        var y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        var z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        return new Vector3(x,y,z);
    }

    private Quaternion ByteArrayToQuaternion(byte[] input, int startIndex) {
        if (startIndex + (sizeof(float) * 3) > input.Length) {
            Debug.Log("Array to small");
        }
        
        var x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        var y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        var z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        var w = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 3));
        return new Quaternion(x, y, z, w);
    }

    public void SendPlayerMovement(Transform transform, Rigidbody rigidbody, byte hp = 1) {
        var send = new byte[5][];
        send[0] = new byte[] { (byte)GameMessageType.PLAYER_MOVEMENT };
        send[1] = Vector3ToByteArray(transform.position);
        send[2] = QuaternionToByteArray(transform.rotation);
        send[3] = Vector3ToByteArray(rigidbody.velocity);
        send[4] = new byte[] { hp };
        Send(MergeArrays(send));
    }

    private byte[] QuaternionToByteArray(Quaternion qua) {
        var tmpByteArrays = new byte[4][];
        tmpByteArrays[0] = BitConverter.GetBytes(qua.x);
        tmpByteArrays[1] = BitConverter.GetBytes(qua.y);
        tmpByteArrays[2] = BitConverter.GetBytes(qua.z);
        tmpByteArrays[3] = BitConverter.GetBytes(qua.w);
        return MergeArrays(tmpByteArrays);
    }

    private byte[] Vector3ToByteArray(Vector3 vec) {
        var tmpByteArrays = new byte[3][];
        tmpByteArrays[0] = BitConverter.GetBytes(vec.x);
        tmpByteArrays[1] = BitConverter.GetBytes(vec.y);
        tmpByteArrays[2] = BitConverter.GetBytes(vec.z);
        return MergeArrays(tmpByteArrays);
    }

    private byte[] MergeArrays(byte[][] input) {
        var output = new byte[GetLength(input)];
        var x = 0;
        var y = 0;

        for (var i = 0; i < output.Length; i++) {
            while (y >= input[x].Length) {
                x++;
                y = 0;
            }

            output[i] = input[x][y];
            y++;
        }

        return output;
    }

    private int GetLength(byte[][] input) {
        var output = 0;
        foreach (var t in input) {
            output += t.Length;
        }

        return output;
    }

    // Use this for initialization
    void Start() {
        if (FakeStatic != null) {
            Application.Quit();
        }

        FakeStatic = this;
        var config = new NetPeerConfiguration("ConquerLeague");
        client = new NetClient(config);
        client.Start();
        //connection = client.Connect(host: "192.168.0.100", port: 47410);
        connection = client.Connect(host: "192.168.1.2", port: 47410);
    }

    // Update is called once per frame
    void Update() {
        StartCoroutine(SendFromQueue());
        StartCoroutine(ReadMessages(client));
    }

    void Send(byte[] data, NetDeliveryMethod netDeliveryMethod = NetDeliveryMethod.ReliableSequenced) {
        sendQueue.Add(data);
        sendMethodQueue.Add(netDeliveryMethod);
    }

    IEnumerator SendFromQueue() {
        while (sendQueue.Count > 0) {
            var msg = client.CreateMessage();
            msg.Write(sendQueue[0].Length);
            msg.Write(sendQueue[0]);
            var ret = client.SendMessage(msg, connection, sendMethodQueue[0]);
            while (ret == NetSendResult.Queued) {
                yield return new WaitForSeconds(0.01f);
            }
            sendQueue.RemoveAt(0);
            sendMethodQueue.RemoveAt(0);
            yield return null;
        }
    }

    private byte[] GetPartFromByteArray(byte[] data, int indexFrom, int indexTo) {
        if (indexTo - indexFrom <= 0) {
            Debug.LogError("indexTo can not be smaller or equals than indexFrom");
            Application.Quit();
        }

        var newByteArray = new byte[indexTo - indexFrom];
        for (var i = indexFrom; i < indexTo; i++) {
            newByteArray[i - indexFrom] = data[i];
        }

        return newByteArray;
    }

    IEnumerator ReadMessages(NetClient client) {
        NetIncomingMessage message;
        while ((message = client.ReadMessage()) != null) {
            switch (message.MessageType) {
                case NetIncomingMessageType.Data:
                    var dataLength = message.ReadInt32();
                    var data = message.ReadBytes(dataLength);
                    if (data.Length == 0) {
                        break;
                    }

                    switch (data[0]) {
                        case (byte)GameMessageType.PLAYER_MOVEMENT:
                            if (data.Length < 42) {
                                break;
                            }

                            RecievePlayerMovement(data);
                            break;
                        case (byte)GameMessageType.SESSION_INITIALITZE:
                            //if (data.Length < 42) {
                            //    break;
                            //}

                            RecieveSessionInitialize(data);
                            break;
                        default:
                            Debug.Log("Unknown Packet recieved. Maybe the App is not updated?");
                            break;
                    }
                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch (message.SenderConnection.Status) {
                        case NetConnectionStatus.Connected:
                            connection = message.SenderConnection;
                            NetConnection net = client.Connections[0];
                            ConnectedToServer = true;
                            break;
                        default:
                            Debug.Log("boop");
                            break;
                    }

                    break;
                default:
                    Console.WriteLine("unhandled message with type: " + message.MessageType);
                    break;
            }

            yield return null;
        }
    }
}
