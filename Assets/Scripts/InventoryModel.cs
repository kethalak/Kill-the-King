using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour {

	[HideInInspector]
	public Item objRef;

	private float selectedScale = 1.25f;

	void OnTriggerEnter(Collider col)
	{
		if(col.GetComponent<PlayerHand>() != null)
		{
			if(col.GetComponent<PlayerHand>().touchedInventoryItem == null)
			{
				PlayerHand hand = col.GetComponent<PlayerHand>();
				hand.touchedInventoryItem = this;
				transform.localScale *= selectedScale;
				if(hand.device != null)
					hand.device.TriggerHapticPulse(2000);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.GetComponent<PlayerHand>() != null)
		{
			if(col.GetComponent<PlayerHand>().touchedInventoryItem == this)
			{
				col.GetComponent<PlayerHand>().touchedInventoryItem = null;
				transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}
}
