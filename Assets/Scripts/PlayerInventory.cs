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


	public GameObject GeneratePreview(PlayerHand hand)
	{
		return gameObject;
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
