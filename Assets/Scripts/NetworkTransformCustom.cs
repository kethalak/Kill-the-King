using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// [NetworkSettings (channel = 0, sendInterval = 0.04f)]
public class NetworkTransformCustom : NetworkBehaviour {

	[Header("Transform Syncing:")]
	public bool syncPosition = true;
	public float positionThreshold = 0.01f;
	[RangeAttribute(1f, 30f)]
	public float positionInterpolation = 20;
	public bool syncRotation = true;
	public float rotationThreshold = 0.01f;
	[RangeAttribute(1f, 30f)] 
	public float rotationInterpolation= 20;
	public bool syncScale = true;
	public float scaleThreshold = 0.01f;
	
	[SyncVar(hook="SetActive")]
	private bool isActive = true;

	private Rigidbody2D rb;

	[SyncVar] Vector3 position;
	[SyncVar] Vector3 scale;
	[SyncVar] Quaternion rotation = Quaternion.Euler(Vector3.zero);
	[SyncVar] NetworkInstanceId parentId;

	void Awake()
	{

	}

	private void Start()
	{
		if(!isLocalPlayer && transform.parent != null)
		{
			transform.SetParent(ClientScene.FindLocalObject(parentId).transform);

		}

		SetActive(isActive);
	}

	void Update()
	{
		SyncTransform();
	}
	
	void SyncTransform()
	{
		if(GetComponent<NetworkIdentity>().hasAuthority)
		{
			if(Vector3.Distance(position, transform.position) >= positionThreshold)
				CmdSetPosition(transform.position);
			if(Quaternion.Angle(rotation, transform.rotation) >= rotationThreshold)
				CmdSetRotation(transform.rotation);
			if(scale != transform.localScale)
				CmdSetScale(transform.localScale);
		}
		else
		{
			if(syncPosition)
			transform.position = Vector3.Lerp(transform.position, position, positionInterpolation * Time.deltaTime);
			if(syncRotation)
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationInterpolation * Time.deltaTime);
			if(syncScale)
			transform.localScale = scale;
		}
	}


	[Command]
	void CmdSetPosition(Vector3 newPosition)
	{
		position = newPosition;
	}

	[Command]
	void CmdSetRotation(Quaternion newRotation)
	{
		rotation = newRotation;
	}

	[Command]
	void CmdSetScale(Vector3 newScale)
	{
		scale = newScale;
	}

	[Command]
	public void CmdSetTransform(Vector3 pos, Quaternion rot)
	{
		RpcSetTransform(pos, rot);
	}

	[ClientRpc]
	void RpcSetTransform(Vector3 pos, Quaternion rot)
	{
		position = pos;
		rotation = rot;
		transform.position = pos;
		transform.rotation = rot;
	}
	
	[Command]
	public void CmdSetActive(bool active)
	{
		isActive = active;
	}

	[Command]
	public void CmdSetParentNull()
	{
		RpcSetParentNull();
	}

	[Command]
	public void CmdSetParent(NetworkInstanceId id)
	{
		parentId = id;
		RpcSetParent(id);
	}

	[ClientRpc]
	void RpcSetParentNull()
	{
		transform.SetParent(null);
	}

	[ClientRpc]
	void RpcSetParent (NetworkInstanceId id)
	{
		transform.SetParent(ClientScene.FindLocalObject(id).transform);
	}

	[ClientCallback]
	void SetActive(bool active)
	{
		this.gameObject.SetActive(active);		
	}
}

