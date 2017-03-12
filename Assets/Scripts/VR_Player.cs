using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerState
{
	InRoom,
	InArena,
	InMenu,
	None
}

public class VR_Player : NetworkBehaviour {
	[HideInInspector]
	public PlayerState playerState;

	//Camera rig reference
	[HideInInspector]
	public GameObject cameraRig;
	
	[HideInInspector]
	public PlayerHand leftHand;
	[HideInInspector]
	public PlayerHand rightHand;
	[HideInInspector]
	public PlayerHead head;

	[HideInInspector]
	public PlayerInventory playerInventory;

	void Awake()
	{
		playerInventory = GetComponent<PlayerInventory>();

	}

	void Start () 
	{
		if(isLocalPlayer)
		{
			Init();
			CmdSpawn("LeftHandGeneric", this.netId);
			CmdSpawn("RightHandGeneric", this.netId);
			CmdSpawn("HeadGeneric", this.netId);
		}
	}
	
	void Init() 
	{
		GameManager.Instance.player = this;
		//Get in-game references
		if(GameObject.FindGameObjectWithTag("cameraRig") != null)
		{
			cameraRig = GameObject.FindGameObjectWithTag("cameraRig");
			cameraRig.transform.position = this.transform.position;
			cameraRig.transform.rotation = this.transform.rotation;
		} 
		else
		{
			Debug.LogError("camera rig not found");
		}

		playerInventory.Init();


	}

	[Command]
	void CmdPlayerScale(Vector3 scale)
	{
		RpcPlayerScale(scale);
	}

	[ClientRpc]
	void RpcPlayerScale(Vector3 scale)
	{
		this.transform.localScale = scale;
	}

	[Command]
	public void CmdSpawn(string prefabName, NetworkInstanceId playerId)
	{
		//Load prefab
		GameObject prefab = Resources.Load(prefabName) as GameObject;
		//Create obj on server
		GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		//Get player obj for auth
		GameObject player = ClientScene.FindLocalObject(playerId);
		//Set parent on server
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(0,0,0);
		//Spawn obj on network with client authority
		NetworkServer.SpawnWithClientAuthority(go, player);
		//Get obj Net ID to find with ClientRpc call
		NetworkInstanceId objId = go.GetComponent<NetworkIdentity>().netId;
		//Set object parent
		if(go.GetComponent<NetworkTransformCustom>() != null)
			go.GetComponent<NetworkTransformCustom>().CmdSetParent(playerId);
		//Rpc call to set parent and ref
		RpcAssign(objId, playerId);
	}

	[ClientRpc]
	void RpcAssign(NetworkInstanceId objId, NetworkInstanceId playerId)
	{
		GameObject go = ClientScene.FindLocalObject(objId);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(0,0,0);
		go.transform.localScale = new Vector3 (1,1,1);
	}

	// void OnPlayerConnected(NetworkPlayer player)
	// {
	// 	NetworkInstanceId parentId = this.transform.parent.GetComponent<NetworkIdentity>().netId;
	// 	NetworkIdentity[] childIds = GetComponentsInChildren<NetworkIdentity>();
	// 	foreach(NetworkIdentity ni in childIds)
	// 		RpcSetParent(parentId, ni.netId);
	// }

	// [ClientRpc]
	// void RpcSetParent(NetworkInstanceId parentId, NetworkInstanceId childId)
	// {
	// 	GameObject parent = ClientScene.FindLocalObject(parentId);
	// 	GameObject child = ClientScene.FindLocalObject(childId);
	// 	child.transform.SetParent(parent.transform);
	// }
}
