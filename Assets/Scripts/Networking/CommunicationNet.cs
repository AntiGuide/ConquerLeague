using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary> Reference to the left base </summary>
    [SerializeField] private Base leftBase;

    /// <summary> Reference to the right base </summary>
    [SerializeField] private Base rightBase;

    /// <summary> The spawn point of the player on the left side </summary>
    [SerializeField] private Transform startPointLeft;

    /// <summary> The spawn point of the player on the right side </summary>
    [SerializeField] private Transform startPointRight;

    /// <summary> The port the server listens on</summary>
    [SerializeField] private int portNumber;

    /// <summary> Reference to the GoalManager displaying the score</summary>
    [SerializeField] private GoalManager goalManager;

    /// <summary> The config to connect to the server </summary>
    public NetClient client;

    /// <summary> The open connection to the server </summary>
    private NetConnection connection;

    /// <summary> The queue that is sent out as fast as possible. Also contains the delivery method for each item in the queue </summary>
    private List<SendData> sendQueue = new List<SendData>();

    /// <summary> Marks if this player is on the left or the right side</summary>
    private bool isLeft;

    /// <summary> A running number for minion identification. Reset after byte.Max. If minion ID not deinitialized when reused bad things happen.</summary>
    private byte aktMinionID = 0;

    /// <summary> Reference to all minions</summary>
    private GameObject[] minions = new GameObject[byte.MaxValue];

    /// <summary> Temporary variable for incoming messages as member to take stress of GC</summary>
    private NetIncomingMessage message;

    /// <summary> Temporary variable for outgoing messages as member to take stress of GC</summary>
    private NetOutgoingMessage outMessage;

    public GameObject[] Minions {
        get {
            return minions;
        }

        set {
            minions = value;
        }
    }

    private struct SendData{
        public byte[] Data;
        public NetDeliveryMethod DeliveryMethod;
    }

    /// <summary> The different message types that can arrive </summary>
    private enum GameMessageType : byte {
        PLAYER_MOVEMENT = 0,
        SESSION_INITIALITZE = 1, // Don't change (has to be the same between client and server)
        MINION_INITIALITZE = 2,
        MINION_MOVE = 3,
        MINION_DEINITIALIZE = 4,
        NEW_SCORE = 5,
        PLAYER_DEATH = 6,
        PLAYER_DAMAGE_DEALT = 7,
        TOWER_DAMAGE = 8,
        TOWER_CONQUERED = 9,
        MINION_HP = 10
    }

    /// <summary>
    /// Get a usable minion ID. Reset after byte.Max. If minion ID not deinitialized when reused bad things happen.
    /// </summary>
    /// <returns>Usable minion ID</returns>
    public byte RequestMinionID() {
        var ret = aktMinionID;
        aktMinionID = aktMinionID == byte.MaxValue ? byte.MinValue : (byte)(aktMinionID + 1);
        return ret;
    }

    /// <summary>
    /// Triggered when an incoming message signals that the enemy died
    /// </summary>
    public void RecievePlayerDeath() {
        enemyPlayerNet?.OnNetDeath(); // 0 = GameMessageType
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

        // 42 - ?
        var shooting = BitConverter.ToBoolean(input, 42);

        enemyPlayerNet?.SetNewMovementPack(position, quaternion, velocity, hp, shooting);
    }

    /// <summary>
    /// Handles incoming data for the minion movement event
    /// </summary>
    /// <param name="input">The incoming data</param>
    public void RecieveMinionMovement(byte[] input) {
        // 0 = GameMessageType
        // 2 - 13 = position
        var position = ByteArrayToVector3(input, 2);

        // 14 - 29 = rotation
        var quaternion = ByteArrayToQuaternion(input, 14);

        // 30 - 41
        var velocity = ByteArrayToVector3(input, 30);

        minions[input[1]].GetComponent<MinionNet>().SetNewMovementPack(position, quaternion, velocity);
    }

    public void RecieveMinionHP(byte[] input) {
        // 0 = GameMessageType
        // 1 = id
        // 2 = hp
        if (minions[input[1]] == null || minions[input[1]]?.GetComponent<MinionNet>() == null) {
            return;
        }
        minions[input[1]]?.GetComponent<MinionNet>()?.SetNewHP(input[2]);
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
        GameManager.LeftTeam = isLeft ? TeamHandler.TeamState.FRIENDLY : TeamHandler.TeamState.ENEMY;
        GameManager.RightTeam = isLeft ? TeamHandler.TeamState.ENEMY : TeamHandler.TeamState.FRIENDLY;
        leftBase.TeamHandler.TeamID = isLeft ? TeamHandler.TeamState.FRIENDLY : TeamHandler.TeamState.ENEMY;
        rightBase.TeamHandler.TeamID = isLeft ? TeamHandler.TeamState.ENEMY : TeamHandler.TeamState.FRIENDLY;

        // Set aside
        enemyPlayerNet?.SetNewMovementPack(startEnemy.position * 5, startEnemy.rotation, Vector3.zero);

        friendlyPlayerNet?.SetNewMovementPack(startFriendly.position, startFriendly.rotation, Vector3.zero);
        friendlyPlayerNet.StartPoint = startFriendly;

        enemyPlayerNet?.SetNewMovementPack(startEnemy.position, startEnemy.rotation, Vector3.zero);
        enemyPlayerNet.StartPoint = startEnemy;
    }

    /// <summary>
    /// Handles incoming data for the player damage event
    /// </summary>
    /// <param name="input">The incoming data</param>
    public void RecievePlayerDamage(byte[] input) {
        // 0 = GameMessageType
        // 1 = Damage
        friendlyPlayerNet?.DamageTaken(input[1]);
    }

    /// <summary>
    /// Handles incoming data for the tower damage event
    /// </summary>
    /// <param name="input">The incoming data</param>
    public void RecieveTowerDamage(byte[] input) {
        // 0 = GameMessageType
        // 1 = TowerID
        // 2 = Damage
        GameManager.towers[input[1]].DamageTaken(input[2]);
    }

    public void RecieveTowerConquered(byte[] input) {
        // 0 = GameMessageType
        // 1 = TowerID
        GameManager.towers[input[1]].TurretConquered();
    }

    /// <summary>
    /// Sends a message to inform other player about this players death
    /// </summary>
    public void SendPlayerDeath() {
        Send(new byte[] { (byte)GameMessageType.PLAYER_DEATH }, NetDeliveryMethod.ReliableUnordered);
    }

    /// <summary>
    /// Sends damage dealt to other player
    /// </summary>
    /// <param name="damage">The damage that was dealt</param>
    public void SendPlayerDamage(byte damage) {
        var send = new byte[2][];
        send[0] = new byte[] { (byte)GameMessageType.PLAYER_DAMAGE_DEALT };
        send[1] = new byte[] { damage };
        Send(MergeArrays(send), NetDeliveryMethod.ReliableUnordered);
    }

    /// <summary>
    /// Sends damage dealt to tower
    /// </summary>
    /// <param name="damage">The damage that was dealt</param>
    public void SendTowerDamage(byte id, byte damage) {
        var send = new byte[3][];
        send[0] = new byte[] { (byte)GameMessageType.TOWER_DAMAGE }; // 0 = GameMessageType
        send[1] = new byte[] { id }; // 1 = TowerID
        send[2] = new byte[] { damage }; // 2 = Damage
        Send(MergeArrays(send), NetDeliveryMethod.ReliableUnordered);
    }

    public void SendTowerConquered(byte id) {
        var send = new byte[2][];
        send[0] = new byte[] { (byte)GameMessageType.TOWER_CONQUERED }; // 0 = GameMessageType
        send[1] = new byte[] { id }; // 1 = TowerID
        Send(MergeArrays(send), NetDeliveryMethod.ReliableUnordered);
    }

    /// <summary>
    /// Sends out the relevant data for this player
    /// </summary>
    /// <param name="transform">The players transform</param>
    /// <param name="rigidbody">The players rigidbody</param>
    /// <param name="hp">The players HP</param>
    public void SendPlayerMovement(Transform transform, Rigidbody rigidbody, byte hp, bool shooting) {
        var send = new byte[6][];
        send[0] = new byte[] { (byte)GameMessageType.PLAYER_MOVEMENT };
        send[1] = ToByteArray(transform.position);
        send[2] = ToByteArray(transform.rotation);
        send[3] = ToByteArray(rigidbody.velocity);
        send[4] = new byte[] { hp };
        send[5] = BitConverter.GetBytes(shooting);
        Send(MergeArrays(send));
    }

    /// <summary>
    /// Send minion information about movement (position, rotation, velocity) and HP
    /// </summary>
    /// <param name="transform">The minions transform</param>
    /// <param name="rigidbody">The minions rigidbody</param>
    /// <param name="id">Identifies the minion</param>
    /// <param name="hp">The minions hitpoints</param>
    public void SendMinionMovement(Transform transform, Rigidbody rigidbody, byte id) {
        var send = new byte[4][];
        send[0] = new byte[] { (byte)GameMessageType.MINION_MOVE, id };
        send[1] = ToByteArray(transform.position);
        send[2] = ToByteArray(transform.rotation);
        send[3] = ToByteArray(rigidbody.velocity);
        Send(MergeArrays(send));
    }

    public void SendMinionHP(byte id, byte hp) {
        Send(new byte[] { (byte)GameMessageType.MINION_HP, id, hp });
    }

    /// <summary>
    /// Initialize/Spawn a minion
    /// </summary>
    /// <param name="id">The assigned ID for this minion</param>
    public void SendMinionInitialization(byte id) {
        var send = new byte[2];
        send[0] = (byte)GameMessageType.MINION_INITIALITZE; // 0 = GameMessageType
        send[1] = id; // 1 = ID
        Send(send, NetDeliveryMethod.ReliableOrdered);
    }

    /// <summary>
    /// Deinitialize/Despawn a minion
    /// </summary>
    /// <param name="id"></param>
    public void SendMinionDeinitialization(byte id) {
        var send = new byte[2];
        send[0] = (byte)GameMessageType.MINION_DEINITIALIZE; // 0 = GameMessageType
        send[1] = id; // 1 = ID
        Send(send, NetDeliveryMethod.ReliableOrdered);
    }

    /// <summary>
    /// Send a new score
    /// </summary>
    /// <param name="leftSide">The new score of the left side</param>
    /// <param name="rightSide">The new score of the right side</param>
    public void SendNewScore(uint leftSide, uint rightSide) {
        var send = new byte[3][];
        send[0] = new byte[] { (byte)GameMessageType.NEW_SCORE };
        send[1] = BitConverter.GetBytes(leftSide);
        send[2] = BitConverter.GetBytes(rightSide);
        Send(MergeArrays(send), NetDeliveryMethod.ReliableOrdered);
    }

    /// <summary>
    /// Handle new score event
    /// </summary>
    /// <param name="input"></param>
    public void RecieveNewScore(byte[] input) {
        goalManager.LeftGoals = BitConverter.ToUInt32(input, 1);
        goalManager.RightGoals = BitConverter.ToUInt32(input, 1 + sizeof(uint));
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
    private byte[] MergeArrays(params byte[][] input) {
        byte[] rv = new byte[input.Sum(a => a.Length)];
        int offset = 0;
        foreach (byte[] array in input) {
            System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }

        return rv;
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
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse); // Enable DiscoveryResponse messages
        config.Port = 47410;
        client = new NetClient(config);
        client.Start();
        client.DiscoverLocalPeers(portNumber); // Emit a discovery signal
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        SendFromQueue();
        ReadMessages(client);
    }

    /// <summary>
    /// Add a byte array to send to the queue
    /// </summary>
    /// <param name="data">The byte array to send</param>
    /// <param name="netDeliveryMethod">The wanted delivery method</param>
    void Send(byte[] data, NetDeliveryMethod netDeliveryMethod = NetDeliveryMethod.UnreliableSequenced) {
        sendQueue.Add(new SendData() { Data = data, DeliveryMethod = netDeliveryMethod });
    }

    /// <summary>
    /// Send byte arrays from the queue
    /// </summary>
    /// <returns>IEnumerator for coroutine</returns>
    void SendFromQueue() {
        while (connection != null && sendQueue.Count > 0) {
            outMessage = client.CreateMessage();
            outMessage.Write(sendQueue[0].Data.Length);
            outMessage.Write(sendQueue[0].Data);
            var ret = client.SendMessage(outMessage, connection, sendQueue[0].DeliveryMethod);
            sendQueue.RemoveAt(0);
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
    private void ReadMessages(NetClient client) {
        while ((message = client.ReadMessage()) != null) {
            switch (message.MessageType) {
                case NetIncomingMessageType.DiscoveryResponse:
                    Debug.Log("Found server at " + message.SenderEndPoint + " name: " + message.ReadString());
                    
                    // DO NOT REMOVE (NEEDED TO CORRECTLY GENERATE ANDROID_MANIFEST)
                    Debug.Log("Reachability: " + Application.internetReachability.ToString() + Network.connectionTesterIP);
                    // DO NOT REMOVE (NEEDED TO CORRECTLY GENERATE ANDROID_MANIFEST)

                    connection = client.Connect(message.SenderEndPoint);
                    break;
                case NetIncomingMessageType.Data:
                    var data = message.ReadBytes(message.ReadInt32());
                    HandleNewDataPackage(data);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch (message.SenderConnection.Status) {
                        case NetConnectionStatus.Connected:
                            connection = message.SenderConnection;
                            NetConnection net = client.Connections[0];
                            break;
                        default:
                            Debug.Log("Unhandled status change with type: " + message.SenderConnection.Status.ToString());
                            break;
                    }

                    break;
                default:
                    break;
            }

            //debugText.text += "Message came in " + message?.SenderEndPoint?.ToString();
            client.Recycle(message);
        }
    }

    /// <summary>
    /// Handles new data recieved in ReadMessages()
    /// </summary>
    /// <param name="data">The data that has been recieved</param>
    private void HandleNewDataPackage(byte[] data) {
        switch (data[0]) {
            case (byte)GameMessageType.PLAYER_MOVEMENT:
                RecievePlayerMovement(data);
                break;
            case (byte)GameMessageType.SESSION_INITIALITZE:
                RecieveSessionInitialize(data);
                StartCoroutine(GameManager.StartGame());
                break;
            case (byte)GameMessageType.MINION_INITIALITZE:
                if (isLeft) {
                    minions[data[1]] = rightBase.RecieveMinionInitialize(data);
                } else {
                    minions[data[1]] = leftBase.RecieveMinionInitialize(data);
                }

                break;
            case (byte)GameMessageType.MINION_DEINITIALIZE:
                Destroy(minions[data[1]]);
                minions[data[1]] = null;
                break;
            case (byte)GameMessageType.MINION_MOVE:
                RecieveMinionMovement(data);
                break;
            case (byte)GameMessageType.NEW_SCORE:
                RecieveNewScore(data);
                break;
            case (byte)GameMessageType.PLAYER_DEATH:
                RecievePlayerDeath();
                break;
            case (byte)GameMessageType.PLAYER_DAMAGE_DEALT:
                RecievePlayerDamage(data);
                break;
            case (byte)GameMessageType.TOWER_DAMAGE:
                RecieveTowerDamage(data);
                break;
            case (byte)GameMessageType.TOWER_CONQUERED:
                RecieveTowerConquered(data);
                break;
            case (byte)GameMessageType.MINION_HP:
                RecieveMinionHP(data);
                break;
            default:
                Debug.Log("Unknown Packet recieved. Maybe the App is not updated?");
                break;
        }
    }

    private void OnApplicationQuit() {
        client?.Disconnect("OnApplicationQuit");
    }

    private void OnApplicationPause(bool pause) {
        client?.Disconnect("OnApplicationPause");
    }
}
