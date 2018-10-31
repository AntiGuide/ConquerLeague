using System;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

/// <summary>
/// ONLY INITIALIZE ONE TIME!
/// This class handles the complete client side communication.
/// </summary>
public class CommunicationNet : MonoBehaviour {
    /// <summary>Gets set on start and allows static access to the one instance of CommunicationNet</summary>
    public static CommunicationNet FakeStatic;

    /// <summary> Reference to the networking component of the enemy </summary>
    [SerializeField] private PlayerNet enemyPlayerNet;

    /// <summary> Reference to the networking component of the local player </summary>
    [SerializeField] private PlayerNet friendlyPlayerNet;

    /// <summary> The spawn point of the player on the left side </summary>
    [SerializeField] private Transform startPointLeft;

    /// <summary> The spawn point of the player on the right side </summary>
    [SerializeField] private Transform startPointRight;

    /// <summary> The config to connect to the server </summary>
    private NetClient client;

    /// <summary> The open connection to the server </summary>
    private NetConnection connection;

    /// <summary> The queue that is sent out as fast as possible </summary>
    private List<byte[]> sendQueue = new List<byte[]>();

    /// <summary> The delivery method of each item in the queue </summary>
    private List<NetDeliveryMethod> sendMethodQueue = new List<NetDeliveryMethod>();

    /// <summary> Marks if this player is on the left or the right side</summary>
    private bool isLeft;

    /// <summary> The different message types that can arrive </summary>
    private enum GameMessageType : byte {
        PLAYER_MOVEMENT = 0,
        SESSION_INITIALITZE = 1 // Don't change (has to be the same between client and server)
    }

    /// <summary>
    /// Takes an input as a byte array and processes the data back to the original form
    /// </summary>
    /// <param name="input">The byte[] recieved from the server/peer</param>
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

    /// <summary>
    /// Takes an input as a byte array and processes the data to determine on which side the player starts
    /// </summary>
    /// <param name="input">The byte[] recieved from the server/peer</param>
    public void RecieveSessionInitialize(byte[] input) {
        // 0 = GameMessageType
        // 1 = Left or Right
        isLeft = BitConverter.ToBoolean(input, 1);

        // Vehicle type (To be included)
        // Weapon type (To be included)
        var startFriendly = isLeft ? startPointLeft : startPointRight;
        var startEnemy = isLeft ? startPointRight : startPointLeft;

        friendlyPlayerNet.SetNewMovementPack(startFriendly.position, startFriendly.rotation, Vector3.zero);
        enemyPlayerNet.SetNewMovementPack(startEnemy.position, startEnemy.rotation, Vector3.zero);
    }

    /// <summary>
    /// Sends out the relevant data for this player
    /// </summary>
    /// <param name="transform">The players transform</param>
    /// <param name="rigidbody">The players rigidbody</param>
    /// <param name="hp">The players HP</param>
    public void SendPlayerMovement(Transform transform, Rigidbody rigidbody, byte hp = 1) {
        var send = new byte[5][];
        send[0] = new byte[] { (byte)GameMessageType.PLAYER_MOVEMENT };
        send[1] = ToByteArray(transform.position);
        send[2] = ToByteArray(transform.rotation);
        send[3] = ToByteArray(rigidbody.velocity);
        send[4] = new byte[] { hp };
        Send(MergeArrays(send));
    }

    /// <summary>
    /// Decodes a byte array into the original Vector3
    /// </summary>
    /// <param name="input">The byte[] recieved from the server/peer</param>
    /// <param name="startIndex">The index at which the function should start to read</param>
    /// <returns>The Vector3 that was encoded earlier</returns>
    private Vector3 ByteArrayToVector3(byte[] input, int startIndex = 0) {
        if (startIndex + (sizeof(float) * 2) > input.Length) {
            Debug.Log("Array to small");
        }

        var x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        var y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        var z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Decodes a byte array into the original Quaternion
    /// </summary>
    /// <param name="input">The byte[] recieved from the server/peer</param>
    /// <param name="startIndex">The index at which the function should start to read</param>
    /// <returns>The Quaternion that was encoded earlier</returns>
    private Quaternion ByteArrayToQuaternion(byte[] input, int startIndex = 0) {
        if (startIndex + (sizeof(float) * 3) > input.Length) {
            Debug.Log("Array to small");
        }
        
        var x = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 0));
        var y = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 1));
        var z = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 2));
        var w = BitConverter.ToSingle(input, startIndex + (sizeof(float) * 3));
        return new Quaternion(x, y, z, w);
    }

    /// <summary>
    /// Encodes a Quaternion into a byte array
    /// </summary>
    /// <param name="qua">The Quaternion to encode</param>
    /// <returns>The encoded Quaternion as a byte array</returns>
    private byte[] ToByteArray(Quaternion qua) {
        var tmpByteArrays = new byte[4][];
        tmpByteArrays[0] = BitConverter.GetBytes(qua.x);
        tmpByteArrays[1] = BitConverter.GetBytes(qua.y);
        tmpByteArrays[2] = BitConverter.GetBytes(qua.z);
        tmpByteArrays[3] = BitConverter.GetBytes(qua.w);
        return MergeArrays(tmpByteArrays);
    }

    /// <summary>
    /// Encodes a Vector3 into a byte array
    /// </summary>
    /// <param name="vec">The Vector3 to encode</param>
    /// <returns>The encoded Vector3 as a byte array</returns>
    private byte[] ToByteArray(Vector3 vec) {
        var tmpByteArrays = new byte[3][];
        tmpByteArrays[0] = BitConverter.GetBytes(vec.x);
        tmpByteArrays[1] = BitConverter.GetBytes(vec.y);
        tmpByteArrays[2] = BitConverter.GetBytes(vec.z);
        return MergeArrays(tmpByteArrays);
    }

    /// <summary>
    /// Merges multiple byte arrays into one
    /// </summary>
    /// <param name="input">Byte arrays to be merged</param>
    /// <returns>Merged array</returns>
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

    /// <summary>
    /// Get sum of elements in all arrays
    /// </summary>
    /// <param name="input">Arrays to be counted</param>
    /// <returns>Length of arrays combined</returns>
    private int GetLength(byte[][] input) {
        var output = 0;
        foreach (var t in input) {
            output += t.Length;
        }

        return output;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        if (FakeStatic != null) {
            Application.Quit();
        }

        FakeStatic = this;
        var config = new NetPeerConfiguration("ConquerLeague");
        client = new NetClient(config);
        client.Start();
        ////connection = client.Connect(host: "192.168.0.100", port: 47410);
        connection = client.Connect(host: "192.168.1.2", port: 47410);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        StartCoroutine(SendFromQueue());
        StartCoroutine(ReadMessages(client));
    }

    /// <summary>
    /// Add a byte array to send to the queue
    /// </summary>
    /// <param name="data">The byte array to send</param>
    /// <param name="netDeliveryMethod">The wanted delivery method</param>
    void Send(byte[] data, NetDeliveryMethod netDeliveryMethod = NetDeliveryMethod.ReliableSequenced) {
        sendQueue.Add(data);
        sendMethodQueue.Add(netDeliveryMethod);
    }

    /// <summary>
    /// Send byte arrays from the queue
    /// </summary>
    /// <returns>IEnumerator for coroutine</returns>
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

    /// <summary>
    /// Gets you a part of the byte array 
    /// </summary>
    /// <param name="data">Original byte arrray</param>
    /// <param name="indexFrom">Index to start reading (inclusive)</param>
    /// <param name="indexTo">Index to stop reading (exclusive)</param>
    /// <returns>The requested part of the byte array</returns>
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

    /// <summary>
    /// Reads incoming messages
    /// </summary>
    /// <param name="client">The socket to read on</param>
    /// <returns>IEnumerator for coroutine</returns>
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
