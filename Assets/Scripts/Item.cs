using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ItemState
{
	OnSpawn,
	OnGround,
	InInventory,
	Equipped,
	None
}
public abstract class Item : NetworkBehaviour {

	protected ItemState itemState = ItemState.None;

	public GameObject itemModel;
	public InventoryModel inventoryModel;
	public Vector3 positionOffset;
	public Vector3 rotationOffset;
	[HideInInspector]
	public PlayerHand currentHand;
	protected GameObject spawnFXPrefab;
	protected GameObject spawnFX;
	protected NetworkIdentity networkIdentity;
	protected NetworkTransformCustom networkTransform;
	protected VR_Player player;
	protected Rigidbody rb;

	protected virtual void Awake()
	{	
		//Get references
		networkTransform = GetComponent<NetworkTransformCustom>();
		networkIdentity = GetComponent<NetworkIdentity>();
		spawnFXPrefab = Resources.Load("ItemSpawnFX") as GameObject;
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.isKinematic = true;
	}

	public virtual void Start()
	{
		StartCoroutine(WaitForInit());
	}

	protected IEnumerator WaitForInit()
	{
		while(this.transform.parent == null)
			yield return null;
		if(this.transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
			Init();
	}

	public virtual void Init()
	{
		player = GameManager.Instance.player;
		InventoryInit();
	}

	// public virtual void SpawnInit()
	// {
	// 	ChangeState(ItemState.OnSpawn);
	// 	spawnFX = Instantiate(spawnFXPrefab, transform.position, Quaternion.identity);
	// 	transform.position += new Vector3(0, .5f, 0);
	// 	spawnFX.transform.SetParent(this.transform);
	// }
	
	public virtual void InventoryInit()
	{
		networkTransform.CmdSetActive(false);
		ChangeState(ItemState.InInventory);
		player.playerInventory.AddItem(this.gameObject);
		networkTransform.CmdSetParent(player.GetComponent<NetworkIdentity>().netId);
	}

	public virtual void EquipInit()
	{
		if(itemState == ItemState.InInventory)
		{
			player.playerInventory.RemoveItem(this.gameObject);
			networkTransform.CmdSetTransform(currentHand.transform.position, currentHand.transform.rotation);
			networkTransform.CmdSetParentNull();
			networkTransform.CmdSetActive(true);
		}

		if(itemState == ItemState.OnGround)
		{
			CmdFromGround(currentHand.transform.position, currentHand.transform.parent.GetComponent<NetworkIdentity>().netId);
		}

		ChangeState(ItemState.Equipped);
	}

	public virtual void OnTouchpadDown()
	{
		InventoryInit();
	}

	public virtual void OnTouchpadUp()
	{
		EquipInit();
	}

	public virtual void OnGripDown()
	{
		rb.isKinematic = false;
		rb.useGravity = false;
	}

	public virtual void OnGripUp()
	{
		Vector3 itemVel;
		Vector3 itemAngVel;

		var origin = currentHand.trackedObj.origin ? currentHand.trackedObj.origin : currentHand.trackedObj.transform.parent;
		if (origin != null)
		{
			itemVel = origin.TransformVector(currentHand.device.velocity);
			itemAngVel = origin.TransformVector(currentHand.device.angularVelocity);
		}
		else
		{
			itemVel = currentHand.device.velocity;
			itemAngVel = currentHand.device.angularVelocity * 2;
		}
		currentHand.UnequipItem();
		CmdThrow(itemVel, itemAngVel);
	}

	
	[Command]
	public virtual void CmdThrow(Vector3 vel, Vector3 angVel)
	{
		GetComponent<NetworkTransformCustom>().syncPosition = false;
		GetComponent<NetworkTransformCustom>().syncRotation = false;
		GetComponent<NetworkTransformCustom>().syncScale = false;
		RpcThrow(vel, angVel);
	}

	[ClientRpc]
	public virtual void RpcThrow(Vector3 vel, Vector3 angVel)
	{
		ChangeState(ItemState.OnGround);
		rb.isKinematic = false;
		rb.useGravity = true;
		rb.velocity = vel;
		rb.angularVelocity = angVel;
	}
	
	[Command]
	public virtual void CmdFromGround(Vector3 newPos, NetworkInstanceId player)
	{
		networkIdentity.RemoveClientAuthority(networkIdentity.clientAuthorityOwner);
		networkIdentity.AssignClientAuthority(ClientScene.FindLocalObject(player).GetComponent<NetworkIdentity>().connectionToClient);
		RpcFromGround(newPos);
	}

	[ClientRpc]
	public virtual void RpcFromGround(Vector3 newPos)
	{
		networkTransform.CmdSetActive(false);
		transform.position = newPos;
		rb.useGravity = false;
		rb.isKinematic = true;
		GetComponent<NetworkTransformCustom>().syncPosition = true;
		GetComponent<NetworkTransformCustom>().syncRotation = true;
		GetComponent<NetworkTransformCustom>().syncScale = true;
		networkTransform.CmdSetActive(true);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.GetComponent<PlayerHand>() != null)
		{
			if(col.GetComponent<PlayerHand>() != null)
			{
				col.GetComponent<PlayerHand>().touchedItem = this;
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.GetComponent<PlayerHand>() != null)
		{
			if(col.GetComponent<PlayerHand>().touchedItem == this)
			{
				col.GetComponent<PlayerHand>().touchedItem = null;
			}
		}
	}

	public virtual void ChangeState(ItemState state)
	{
		if(itemState != state)
		{
			switch(itemState)
			{
				case ItemState.OnSpawn:
				if(spawnFX != null)
				Destroy(spawnFX);
				break;
			}
			itemState = state;
		}
	}
	public virtual void OnTriggerDown()
	{

	}

	public virtual void OnTriggerUp()
	{

	}

	public virtual void OnTrigger()
	{

	}
	
	public virtual void OnGrip()
	{

	}
}
