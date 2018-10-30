using System;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

public class CommunicationNet : MonoBehaviour {
    public static CommunicationNet FakeStatic;
    [SerializeField]
    private PlayerNet playerNet;
    [SerializeField]
    private NetDeliveryMethod deliveryMethod;
    private NetClient client;
    private NetConnection connection;
    private List<byte[]> sendQueue = new List<byte[]>();
    private List<NetDeliveryMethod> sendMethodQueue = new List<NetDeliveryMethod>();

    public bool ConnectedToServer { get; set; }

    private enum GameMessageType : byte {
        PLAYER_MOVEMENT = 0
    }

    public void RecievePlayerMovement(byte[] input) {
        Vector3 position;
        Quaternion quaternion;
        Vector3 velocity;
        byte hp;

        // 0 = GameMessageType
        // 1 - 12 = position
        position = ByteArrayToVector3(input, 1);
        // 13 - 28 = rotation
        quaternion = ByteArrayToQuaternion(input, 13);
        // 29 - 40
        velocity = ByteArrayToVector3(input, 29);
        // 41 = hp
        hp = input[41];

        playerNet.SetNewMovementPack(position, quaternion, velocity, hp);
    }

    private Vector3 ByteArrayToVector3(byte[] input, int startIndex) {
        float x, y, z;
        x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        return new Vector3(x,y,z);
    }

    private Quaternion ByteArrayToQuaternion(byte[] input, int startIndex) {
        //Debug.Log(startIndex + " + " + (sizeof(float) * 3) + " = " + (startIndex + (sizeof(float) * 3)));
        if (startIndex + (sizeof(float) * 3) > input.Length) {
            Debug.Log("Array to small");
        }

        float x, y, z, w;
        x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        w = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 3));
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
        int x = 0;
        int y = 0;

        for (int i = 0; i < output.Length; i++) {
            while (y >= input[x].Length) {
                x++;
                y = 0;
            }
            output[i] = input[x][y];
            //if (output.Length >= 30) {
            //    Debug.Log(output[i]);
            //}

            y++;
        }

        return output;
    }

    private int GetLength(byte[][] input) {
        var output = 0;
        for (int i = 0; i < input.Length; i++) {
            output += input[i].Length;
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
        connection = client.Connect(host: "192.168.0.104", port: 47410);
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
        for (int i = indexFrom; i < indexTo; i++) {
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
                    ////message.ReadBytes(message.ReadInt32());
                    if (data.Length < 42) {
                        continue;
                    }
                    //Debug.Log("Packet size: " + data.Length + " Byte(s)");
                    //data = GetPartFromByteArray(data, 1, data.Length);
                    if (data[0] == (byte)GameMessageType.PLAYER_MOVEMENT) {
                        RecievePlayerMovement(data);
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
                ////case NetIncomingMessageType.DebugMessage:
                ////    Debug.Log(message.ReadString());
                ////    var debugText = message.ReadString();
                ////    break;
                default:
                    Console.WriteLine("unhandled message with type: " + message.MessageType);
                    break;
            }

            yield return null;
        }
    }
}
