using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHead : NetworkBehaviour {
	[HideInInspector]
	public GameObject hmd;

	private bool parentIsLocalPlayer;

	void Start()
	{
		StartCoroutine(WaitForInit());
	}

	IEnumerator WaitForInit()
	{
		while(transform.parent == null)
			yield return null;

		if(transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
			Init();
	}

	void Init()
	{
		GetComponentInChildren<MeshRenderer>().enabled = false;
		GameManager.Instance.player.head = this;
		hmd = Camera.main.gameObject;
		parentIsLocalPlayer = true;
	}

	void PositionModel()
	{
		this.transform.position = hmd.transform.position;
		this.transform.rotation = hmd.transform.rotation;
	}

	void FixedUpdate () 
	{
		if(parentIsLocalPlayer)
		{
			if(hmd != null)
				PositionModel();
		}	
	}
}
