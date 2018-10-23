using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingTest : MonoBehaviour {

    NetClient client;

    // Use this for initialization
    void Start () {
        var config = new NetPeerConfiguration("ConquerLeague");
        client = new NetClient(config);
        client.Start();
        client.Connect(host: "127.0.0.1", port: 47410);
    }
	
	// Update is called once per frame
	void Update () {
        // ReadMessages(client);
        client.SendMessage(client.CreateMessage("Test"), NetDeliveryMethod.ReliableOrdered);
    }

    private void OnApplicationQuit() {
        client.Disconnect("NormalExit");
    }

    void ReadMessages(NetClient client) {
        NetIncomingMessage message;
        while ((message = client.ReadMessage()) != null) {
            switch (message.MessageType) {
                case NetIncomingMessageType.Data:
                    // handle custom messages
                    // var data = message.Read();
                    break;

                case NetIncomingMessageType.StatusChanged:
                    // handle connection status messages
                    switch (message.SenderConnection.Status) {
                        /* .. */
                    }
                    break;
                case NetIncomingMessageType.DebugMessage:
                    // handle debug messages
                    // (only received when compiled in DEBUG mode)
                    Console.WriteLine(message.ReadString());
                    break;
                default:
                    Console.WriteLine("unhandled message with type: " + message.MessageType);
                    break;
            }
        }
    }
}
