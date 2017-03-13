using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHand : NetworkBehaviour {

	[HideInInspector]
	public Item touchedItem;
	[HideInInspector]
	public Item equippedItem;
	[HideInInspector]
	public SteamVR_Controller.Device device;
	[HideInInspector]
	public SteamVR_TrackedObject trackedObj;

	private Rigidbody equippedItemRb;
	private SphereCollider handCollider;

	private GameObject viveController;

	private bool parentIsLocalPlayer;

	// //Get references
	void Awake()
	{
		handCollider = GetComponent<SphereCollider>();
	}
	
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
		
		if(this.gameObject.CompareTag("LeftHand"))
		{
			viveController = GameManager.Instance.player.cameraRig.GetComponent<SteamVR_ControllerManager>().left;
			GameManager.Instance.player.leftHand = this;
		}
		else if(this.gameObject.CompareTag("RightHand"))
		{
			viveController = GameManager.Instance.player.cameraRig.GetComponent<SteamVR_ControllerManager>().right;
			GameManager.Instance.player.rightHand = this;
		}
		parentIsLocalPlayer = true;
	}

	void FixedUpdate () 
	{
		if(parentIsLocalPlayer)
		{
			// Retrieve Controller inputs
			if(device == null && viveController != null)
			{
				device = viveController.GetComponent<ViveController>().device;
				trackedObj = viveController.GetComponent<ViveController>().trackedObj;
			}

			if(viveController != null)
				PositionObjects();

			if(device != null)
			{	
				HandInteractions();
			}
		}
	}
	
	void PositionObjects()
	{
		//Position hand to vive controller
		this.transform.position = viveController.transform.position;
		this.transform.rotation = viveController.transform.rotation;

		// Position equipped item to vive controller
		if( equippedItem != null)
		{
			if(equippedItemRb != null)
			{
				equippedItemRb.MovePosition(viveController.transform.position + equippedItem.positionOffset);
				equippedItemRb.MoveRotation(viveController.transform.rotation * Quaternion.Euler(equippedItem.rotationOffset));
			}
			else
			{
				equippedItem.transform.position = viveController.transform.position + equippedItem.positionOffset;
				equippedItem.transform.rotation = (viveController.transform.rotation * Quaternion.Euler(equippedItem.rotationOffset));
			}
		}
	}

	void HandInteractions()
	{	
		//If touchpad is pressed down
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{

		}
		// If touchpad is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
		{	

		}
		// If trigger is pressed down
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
		{

		}
		// If trigger is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
		{

		}
		// While trigger is pressed
		if(device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
		{

		}
		// If grip button is pressed down
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{

		}
		// If grip button is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{

		}
		// While grip button is pressed
		if(device.GetPress(SteamVR_Controller.ButtonMask.Grip))
		{	

		}
	}

	// // Equip item and deactivate hand model
	public void EquipItem(Item item)
	{
		equippedItem = item;
		item.currentHand = this;
		equippedItemRb = item.GetComponent<Rigidbody>();
		CmdToggleHand(false);
	}
	// Unequip item and activate hand model
	public void UnequipItem()
	{
		equippedItem.OnTouchpadDown();
		equippedItem.currentHand = null;
		equippedItem = null;
		equippedItemRb = null;
		CmdToggleHand(true);
	}

	[Command]
	void CmdToggleHand(bool active)
	{
		MeshRenderer handModel = GetComponentInChildren<MeshRenderer>();
		handModel.enabled = (active);
		handCollider.enabled=(active);
		RpcToggleHand(active);
	}

	[ClientRpc]
	void RpcToggleHand(bool active)
	{
		MeshRenderer handModel = GetComponentInChildren<MeshRenderer>();
		handModel.enabled = (active);
		handCollider.enabled=(active);
	}

}
