using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInventory : NetworkBehaviour {

	public List<GameObject> startingItems;
	public GameObject inventoryGridModel;

	private GameObject inventoryGrid;
	private List<GameObject> inventory = new List<GameObject>(inventorySize);
	private const int inventorySize = 8;

	public void Init()
	{
		VR_Player player = GetComponent<VR_Player>();

		if(startingItems.Count <= inventorySize)
		{
			for(var i = 0; i < startingItems.Count; i++)
			{
				player.CmdSpawn(startingItems[i].name, GetComponent<NetworkIdentity>().netId);
			}
		}
	}

	private Vector3[] slotPosition = new Vector3[] {new Vector3(-.2f, .2f ,0), new Vector3(0, .2f, 0), new Vector3(.2f, .2f, 0), new Vector3(-.2f, 0, 0), new Vector3(.2f, 0, 0), new Vector3(.2f, -.2f, 0), new Vector3(0, -.2f, 0), new Vector3(-.2f, -.2f, 0)};

	public GameObject GeneratePreview(PlayerHand hand)
	{
		GameObject gridPreview = Instantiate(inventoryGridModel, hand.transform.position, hand.transform.rotation);
		gridPreview.transform.LookAt(Camera.main.transform);
		gridPreview.transform.Rotate(0,180,0);
		for(var i = 0; i < slotPosition.Length; i++)
		{
			if(i < inventory.Count)
			{
				Item item = inventory[i].GetComponent<Item>();
				InventoryModel model = Instantiate(item.inventoryModel, gridPreview.transform.position, gridPreview.transform.rotation);
				model.GetComponent<InventoryModel>().objRef = inventory[i].GetComponent<Item>();
				model.transform.SetParent(gridPreview.transform);
				model.transform.localPosition = slotPosition[i];
			}
			else
			{
				//handle empty slots
			}
		}
		return gridPreview;
	}

	public void RemoveItem(GameObject item)
	{
		inventory.Remove(item);
	}

	public void AddItem(GameObject item)
	{
		if(inventory.Count >= inventorySize)
		{
			Debug.LogError("Inventory Full");
		}
		else if(item.GetComponent<Item>() == null)
		{
			Debug.LogError("Item Type Unacceptable");
		}
		else
		{
			inventory.Add(item);
		}
	}
}
