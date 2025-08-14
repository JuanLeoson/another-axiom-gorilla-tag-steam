using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x000FBF98 File Offset: 0x000FA198
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x000FBFD8 File Offset: 0x000FA1D8
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x000FBFF0 File Offset: 0x000FA1F0
	public void OnTriggerEnter(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x000FC070 File Offset: 0x000FA270
	public void OnTriggerExit(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04003BF2 RID: 15346
	public float maxScale;

	// Token: 0x04003BF3 RID: 15347
	public float minScale;

	// Token: 0x04003BF4 RID: 15348
	public bool isAssurance;

	// Token: 0x04003BF5 RID: 15349
	public bool affectLayerA = true;

	// Token: 0x04003BF6 RID: 15350
	public bool affectLayerB = true;

	// Token: 0x04003BF7 RID: 15351
	public bool affectLayerC = true;

	// Token: 0x04003BF8 RID: 15352
	public bool affectLayerD = true;

	// Token: 0x04003BF9 RID: 15353
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04003BFA RID: 15354
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04003BFB RID: 15355
	[SerializeField]
	private bool triggerWithBodyCollider;
}
