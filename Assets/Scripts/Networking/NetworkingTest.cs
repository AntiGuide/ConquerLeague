using System;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingTest : MonoBehaviour {
    [SerializeField]
    private Text debugTest;
    private NetClient client;
    private int i = 0;

    // Use this for initialization
    void Start() {
        var config = new NetPeerConfiguration("ConquerLeague");
        client = new NetClient(config);
        client.Start();
        client.Connect(host: "192.168.0.100", port: 47410);
    }

    // Update is called once per frame
    void Update() {
        client.SendMessage(client.CreateMessage("Test " + ++i), NetDeliveryMethod.ReliableOrdered);
        ReadMessages(client);
    }

    private void OnApplicationQuit() {
        client.Disconnect("NormalExit");
    }

    void ReadMessages(NetClient client) {
        NetIncomingMessage message;
        while ((message = client.ReadMessage()) != null) {
            switch (message.MessageType) {
                case NetIncomingMessageType.Data:
                    debugTest.text = message.ReadString();
                    break;

                ////case NetIncomingMessageType.StatusChanged:
                ////    switch (message.SenderConnection.Status) {
                ////    }
                ////    break;
                case NetIncomingMessageType.DebugMessage:
                    Console.WriteLine(message.ReadString());
                    debugTest.text = message.ReadString();
                    break;
                default:
                    Console.WriteLine("unhandled message with type: " + message.MessageType);
                    break;
            }
        }
    }
}
