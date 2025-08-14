using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A3D RID: 2621
public class RigOwnedPhysicsBody : MonoBehaviour
{
	// Token: 0x0600403A RID: 16442 RVA: 0x00145CA0 File Offset: 0x00143EA0
	private void Awake()
	{
		this.hasTransformView = (this.transformView != null);
		this.hasRigidbodyView = (this.rigidbodyView != null);
		if (!this.hasTransformView && !this.hasRigidbodyView && this.otherComponents.Length == 0)
		{
			GTDev.LogError<string>("RigOwnedPhysicsBody has nothing to do! No TransformView, RigidbodyView, or otherComponents", null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.transform.parent = null;
				return;
			}
			if (this.hasRigidbodyView)
			{
				this.rigidbodyView.transform.parent = null;
			}
		}
	}

	// Token: 0x0600403B RID: 16443 RVA: 0x00145D30 File Offset: 0x00143F30
	private void OnEnable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnNetDisconnect;
		if (!this.hasRig)
		{
			this.rig = base.GetComponentInParent<VRRig>();
			this.hasRig = (this.rig != null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(true);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(true);
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnNetConnect();
			return;
		}
		this.OnNetDisconnect();
	}

	// Token: 0x0600403C RID: 16444 RVA: 0x00145E08 File Offset: 0x00144008
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnNetDisconnect;
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(false);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(false);
			}
		}
		this.OnNetDisconnect();
	}

	// Token: 0x0600403D RID: 16445 RVA: 0x00145E9C File Offset: 0x0014409C
	private void OnNetConnect()
	{
		if (this.hasTransformView)
		{
			this.transformView.enabled = this.hasRig;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = this.hasRig;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.hasRig;
		}
		if (!this.hasRig)
		{
			return;
		}
		PhotonView getView = this.rig.netView.GetView;
		List<Component> observedComponents = getView.ObservedComponents;
		if (this.hasTransformView)
		{
			this.transformView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.transformView))
			{
				observedComponents.Add(this.transformView);
			}
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.rigidbodyView))
			{
				observedComponents.Add(this.rigidbodyView);
			}
		}
		foreach (MonoBehaviourPun item in this.otherComponents)
		{
			if (!observedComponents.Contains(item))
			{
				observedComponents.Add(item);
			}
		}
	}

	// Token: 0x0600403E RID: 16446 RVA: 0x00145FB4 File Offset: 0x001441B4
	private void OnNetDisconnect()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.hasTransformView)
		{
			this.transformView.enabled = false;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = false;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (!this.hasRig || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		List<Component> observedComponents = this.rig.netView.GetView.ObservedComponents;
		if (this.hasTransformView)
		{
			observedComponents.Remove(this.transformView);
		}
		if (this.hasRigidbodyView)
		{
			observedComponents.Remove(this.rigidbodyView);
		}
		foreach (MonoBehaviourPun item in this.otherComponents)
		{
			observedComponents.Remove(item);
		}
	}

	// Token: 0x04004BDF RID: 19423
	private VRRig rig;

	// Token: 0x04004BE0 RID: 19424
	public RigOwnedTransformView transformView;

	// Token: 0x04004BE1 RID: 19425
	private bool hasTransformView;

	// Token: 0x04004BE2 RID: 19426
	public RigOwnedRigidbodyView rigidbodyView;

	// Token: 0x04004BE3 RID: 19427
	private bool hasRigidbodyView;

	// Token: 0x04004BE4 RID: 19428
	public MonoBehaviourPun[] otherComponents;

	// Token: 0x04004BE5 RID: 19429
	private bool hasRig;

	// Token: 0x04004BE6 RID: 19430
	[Tooltip("To make a rigidbody unaffected by the movement of the holdable part, put this script on the holdable, make the RigOwnedRigidbodyView a child of it, and check this box")]
	[SerializeField]
	private bool detachTransform;
}
