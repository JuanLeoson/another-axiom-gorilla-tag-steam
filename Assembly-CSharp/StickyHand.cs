using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class StickyHand : MonoBehaviour, ISpawnable
{
	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000B30 RID: 2864 RVA: 0x0003B9BB File Offset: 0x00039BBB
	// (set) Token: 0x06000B31 RID: 2865 RVA: 0x0003B9C3 File Offset: 0x00039BC3
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000B32 RID: 2866 RVA: 0x0003B9CC File Offset: 0x00039BCC
	// (set) Token: 0x06000B33 RID: 2867 RVA: 0x0003B9D4 File Offset: 0x00039BD4
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000B34 RID: 2868 RVA: 0x0003B9E0 File Offset: 0x00039BE0
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.isLocal = rig.isLocal;
		this.flatHand.enabled = false;
		this.defaultLocalPosition = this.stringParent.transform.InverseTransformPoint(this.rb.transform.position);
		int num = (this.CosmeticSelectedSide == ECosmeticSelectSide.Left) ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003BA58 File Offset: 0x00039C58
	private void Update()
	{
		if (this.isLocal)
		{
			if (this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringDetachLength))
			{
				this.Unstick();
			}
			else if (!this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringTeleportLength))
			{
				this.rb.transform.position = this.stringParent.transform.TransformPoint(this.defaultLocalPosition);
			}
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.rb.isKinematic);
			return;
		}
		if (GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex) != this.rb.isKinematic)
		{
			if (this.rb.isKinematic)
			{
				this.Unstick();
				return;
			}
			this.Stick();
		}
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003BB86 File Offset: 0x00039D86
	private void Stick()
	{
		this.thwackSound.Play();
		this.flatHand.enabled = true;
		this.regularHand.enabled = false;
		this.rb.isKinematic = true;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0003BBB7 File Offset: 0x00039DB7
	private void Unstick()
	{
		this.schlupSound.Play();
		this.rb.isKinematic = false;
		this.flatHand.enabled = false;
		this.regularHand.enabled = true;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003BBE8 File Offset: 0x00039DE8
	private void OnCollisionStay(Collision collision)
	{
		if (!this.isLocal || this.rb.isKinematic)
		{
			return;
		}
		if ((this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringMaxAttachLength))
		{
			return;
		}
		this.Stick();
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		this.rb.transform.rotation = Quaternion.LookRotation(normal, this.rb.transform.up);
		Vector3 vector = this.rb.transform.position - point;
		vector -= Vector3.Dot(vector, normal) * normal;
		this.rb.transform.position = point + vector + this.surfaceOffsetDistance * normal;
	}

	// Token: 0x04000DC1 RID: 3521
	[SerializeField]
	private MeshRenderer flatHand;

	// Token: 0x04000DC2 RID: 3522
	[SerializeField]
	private MeshRenderer regularHand;

	// Token: 0x04000DC3 RID: 3523
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000DC4 RID: 3524
	[SerializeField]
	private GameObject stringParent;

	// Token: 0x04000DC5 RID: 3525
	[SerializeField]
	private float surfaceOffsetDistance;

	// Token: 0x04000DC6 RID: 3526
	[SerializeField]
	private float stringMaxAttachLength;

	// Token: 0x04000DC7 RID: 3527
	[SerializeField]
	private float stringDetachLength;

	// Token: 0x04000DC8 RID: 3528
	[SerializeField]
	private float stringTeleportLength;

	// Token: 0x04000DC9 RID: 3529
	[SerializeField]
	private SoundBankPlayer thwackSound;

	// Token: 0x04000DCA RID: 3530
	[SerializeField]
	private SoundBankPlayer schlupSound;

	// Token: 0x04000DCB RID: 3531
	private VRRig myRig;

	// Token: 0x04000DCC RID: 3532
	private bool isLocal;

	// Token: 0x04000DCD RID: 3533
	private int stateBitIndex;

	// Token: 0x04000DCE RID: 3534
	private Vector3 defaultLocalPosition;
}
