using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCustom : NetworkManager {
	public GameObject playerVR;
	// public GameObject playerPC;

	public GameObject[] spawnPoints;

	public class NetworkMessage : MessageBase {
        public string platform;
    }

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		// NetworkMessage msg = extraMessageReader.ReadMessage<NetworkMessage>();
		GameObject spawn = spawnPoints[NetworkServer.connections.Count - 1];
		// if(msg.platform == "VR")
		// {
			GameObject player = Instantiate(playerVR, spawn.transform.position, spawn.transform.rotation);
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		// }
		// else if (msg.platform == "PC")
		// {
		// 	GameObject player = Instantiate(playerPC, spawn.transform.position, spawn.transform.rotation);
		// 	NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	// 	}	
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		NetworkMessage netMsg = new NetworkMessage();
		netMsg.platform = PlayerPrefsManager.GetPlatform();
		ClientScene.AddPlayer(conn, 0, netMsg);
	}
}
