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
	public InventoryModel touchedInventoryItem;
	[HideInInspector]
	public SteamVR_Controller.Device device;
	[HideInInspector]
	public SteamVR_TrackedObject trackedObj;

	private Rigidbody equippedItemRb;
	private SphereCollider handCollider;
	private GameObject inventoryPreview;

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
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) && inventoryPreview == null)
		{	
			//Place equipped item in inventory
			if(equippedItem != null)
			{
				UnequipItem();
			}
			//Generate an inventory preview
			inventoryPreview = GameManager.Instance.player.playerInventory.GeneratePreview(this);
			GameManager.Instance.player.leftHand.handCollider.radius = 0.01f;
			GameManager.Instance.player.rightHand.handCollider.radius = 0.01f;
		}
		// If touchpad is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && inventoryPreview != null)
		{	
			//Equip selected inventory item
			if(touchedInventoryItem != null)
			{
				EquipItem(touchedInventoryItem.objRef);
				touchedInventoryItem.objRef.OnTouchpadUp();
			}
			//Destroy inventory preview
			Destroy(inventoryPreview);
			GameManager.Instance.player.leftHand.handCollider.radius = 0.1f;
			GameManager.Instance.player.rightHand.handCollider.radius = 0.1f;
		}
		// If trigger is pressed down
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			//Activate equipped item on trigger method
			if(equippedItem != null)
			{
				equippedItem.OnTriggerDown();
			}
			//Or equip touched item
			else if(touchedItem != null)
			{
				EquipItem(touchedItem);
				touchedItem.EquipInit();
			}
		}
		// If trigger is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			//Activate equipped item trigger release method
			if(equippedItem != null)
				equippedItem.OnTriggerUp();
		}
		// While trigger is pressed
		if(device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
		{
			//Continuously activate equipped item trigger method
			if(equippedItem != null)
			equippedItem.OnTrigger();
		}
		// If grip button is pressed down
		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			//Activate equipped item on grip method
			if(equippedItem != null)
				equippedItem.OnGripDown();
			//Or equip touched item
			else if(touchedItem != null)
			{
				EquipItem(touchedItem);
				touchedItem.EquipInit();
			}
		}
		// If grip button is released
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
			//Activate equipped item grip release method
			if(equippedItem != null)
				equippedItem.OnGripUp();
		}
		// While grip button is pressed
		if(device.GetPress(SteamVR_Controller.ButtonMask.Grip))
		{	
			//Continuously activate equipped item grip method
			if(equippedItem != null)
				equippedItem.OnGrip();
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

	// void OnTriggerEnter(Collider col)
	// {
	// 	if(col.GetComponent<InventoryModel>() != null)
	// 	{
	// 		device.TriggerHapticPulse(100);
	// 	}
		 
	// }
}
