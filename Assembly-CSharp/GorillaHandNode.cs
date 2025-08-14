using System;
using UnityEngine;

// Token: 0x020006CE RID: 1742
public class GorillaHandNode : MonoBehaviour
{
	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002B68 RID: 11112 RVA: 0x000E5C30 File Offset: 0x000E3E30
	public bool isGripping
	{
		get
		{
			return this.PollGrip();
		}
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x06002B69 RID: 11113 RVA: 0x000E5C38 File Offset: 0x000E3E38
	public bool isLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x06002B6A RID: 11114 RVA: 0x000E5C40 File Offset: 0x000E3E40
	public bool isRightHand
	{
		get
		{
			return this._isRightHand;
		}
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000E5C48 File Offset: 0x000E3E48
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000E5C50 File Offset: 0x000E3E50
	private bool PollGrip()
	{
		if (this.rig == null)
		{
			return false;
		}
		bool flag = this.PollThumb() >= 0.25f;
		bool flag2 = this.PollIndex() >= 0.25f;
		bool flag3 = this.PollMiddle() >= 0.25f;
		return flag && flag2 && flag3;
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000E5CA4 File Offset: 0x000E3EA4
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		if (this.rigidbody == null)
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.rig)
		{
			this.vrIndex = (this._isLeftHand ? this.rig.leftIndex : this.rig.rightIndex);
			this.vrThumb = (this._isLeftHand ? this.rig.leftThumb : this.rig.rightThumb);
			this.vrMiddle = (this._isLeftHand ? this.rig.leftMiddle : this.rig.rightMiddle);
		}
		this._isLeftHand = base.name.Contains("left", StringComparison.OrdinalIgnoreCase);
		this._isRightHand = base.name.Contains("right", StringComparison.OrdinalIgnoreCase);
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(this._isLeftHand ? UnityTag.GorillaHandLeft : UnityTag.GorillaHandRight);
		base.gameObject.SetLayer(UnityLayer.GorillaHand);
		this.rigidbody.includeLayers = num;
		this.rigidbody.excludeLayers = ~num;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnTriggerStay(Collider other)
	{
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x000E5E63 File Offset: 0x000E4063
	private float PollIndex()
	{
		return Mathf.Clamp01(this.vrIndex.calcT / 0.88f);
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000E5E7B File Offset: 0x000E407B
	private float PollMiddle()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000E5E7B File Offset: 0x000E407B
	private float PollThumb()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x040036BA RID: 14010
	public VRRig rig;

	// Token: 0x040036BB RID: 14011
	public Collider collider;

	// Token: 0x040036BC RID: 14012
	public Rigidbody rigidbody;

	// Token: 0x040036BD RID: 14013
	[Space]
	[NonSerialized]
	public VRMapIndex vrIndex;

	// Token: 0x040036BE RID: 14014
	[NonSerialized]
	public VRMapThumb vrThumb;

	// Token: 0x040036BF RID: 14015
	[NonSerialized]
	public VRMapMiddle vrMiddle;

	// Token: 0x040036C0 RID: 14016
	[Space]
	public GorillaHandSocket attachedToSocket;

	// Token: 0x040036C1 RID: 14017
	[Space]
	[SerializeField]
	private bool _isLeftHand;

	// Token: 0x040036C2 RID: 14018
	[SerializeField]
	private bool _isRightHand;

	// Token: 0x040036C3 RID: 14019
	public bool ignoreSockets;
}
