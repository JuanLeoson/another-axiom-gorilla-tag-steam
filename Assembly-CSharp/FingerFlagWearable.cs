using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000E2 RID: 226
public class FingerFlagWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x0600059E RID: 1438 RVA: 0x00020AF9 File Offset: 0x0001ECF9
	// (set) Token: 0x0600059F RID: 1439 RVA: 0x00020B01 File Offset: 0x0001ED01
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060005A0 RID: 1440 RVA: 0x00020B0A File Offset: 0x0001ED0A
	// (set) Token: 0x060005A1 RID: 1441 RVA: 0x00020B12 File Offset: 0x0001ED12
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060005A2 RID: 1442 RVA: 0x00020B1B File Offset: 0x0001ED1B
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = base.GetComponentInParent<VRRig>(true);
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00020B44 File Offset: 0x0001ED44
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00020B7C File Offset: 0x0001ED7C
	private void UpdateLocal()
	{
		int node = this.attachedToLeftHand ? 4 : 5;
		bool flag = ControllerInputPoller.GripFloat((XRNode)node) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)node);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)node);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00020BFC File Offset: 0x0001EDFC
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = (this.extended && this.retractExtendTime <= 0f);
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00020C8A File Offset: 0x0001EE8A
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00020CC3 File Offset: 0x0001EEC3
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00020CE0 File Offset: 0x0001EEE0
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00020D00 File Offset: 0x0001EF00
	private void UpdateAnimation()
	{
		float num = this.extended ? this.extendSpeed : (-this.retractSpeed);
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00020D58 File Offset: 0x0001EF58
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.GTPlay();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x040006B2 RID: 1714
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x040006B3 RID: 1715
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x040006B4 RID: 1716
	public Transform thumbRingBone;

	// Token: 0x040006B5 RID: 1717
	public Transform[] clothBones;

	// Token: 0x040006B6 RID: 1718
	public Transform[] clothRigidbodies;

	// Token: 0x040006B7 RID: 1719
	[Header("Animation")]
	public Animator animator;

	// Token: 0x040006B8 RID: 1720
	public float extendSpeed = 1.5f;

	// Token: 0x040006B9 RID: 1721
	public float retractSpeed = 2.25f;

	// Token: 0x040006BA RID: 1722
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x040006BB RID: 1723
	public AudioClip extendAudioClip;

	// Token: 0x040006BC RID: 1724
	public AudioClip retractAudioClip;

	// Token: 0x040006BD RID: 1725
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x040006BE RID: 1726
	public float extendVibrationStrength = 0.2f;

	// Token: 0x040006BF RID: 1727
	public float retractVibrationDuration = 0.05f;

	// Token: 0x040006C0 RID: 1728
	public float retractVibrationStrength = 0.2f;

	// Token: 0x040006C1 RID: 1729
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x040006C2 RID: 1730
	private bool networkedExtended;

	// Token: 0x040006C3 RID: 1731
	private bool extended;

	// Token: 0x040006C4 RID: 1732
	private bool fullyRetracted;

	// Token: 0x040006C5 RID: 1733
	private float retractExtendTime;

	// Token: 0x040006C6 RID: 1734
	private InputDevice inputDevice;

	// Token: 0x040006C7 RID: 1735
	private VRRig myRig;

	// Token: 0x040006C8 RID: 1736
	private int stateBitIndex;
}
