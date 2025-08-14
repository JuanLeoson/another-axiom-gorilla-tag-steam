using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000100 RID: 256
public class FingerTorch : MonoBehaviour, ISpawnable
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000653 RID: 1619 RVA: 0x000248D3 File Offset: 0x00022AD3
	// (set) Token: 0x06000654 RID: 1620 RVA: 0x000248DB File Offset: 0x00022ADB
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000655 RID: 1621 RVA: 0x000248E4 File Offset: 0x00022AE4
	// (set) Token: 0x06000656 RID: 1622 RVA: 0x000248EC File Offset: 0x00022AEC
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000657 RID: 1623 RVA: 0x000248F5 File Offset: 0x00022AF5
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00024918 File Offset: 0x00022B18
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x000023F5 File Offset: 0x000005F5
	protected void OnDisable()
	{
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00024950 File Offset: 0x00022B50
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

	// Token: 0x0600065C RID: 1628 RVA: 0x000249D0 File Offset: 0x00022BD0
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00024A04 File Offset: 0x00022C04
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00024A3D File Offset: 0x00022C3D
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00024A5A File Offset: 0x00022C5A
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

	// Token: 0x06000660 RID: 1632 RVA: 0x00024A78 File Offset: 0x00022C78
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

	// Token: 0x0400079D RID: 1949
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x0400079E RID: 1950
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x0400079F RID: 1951
	public Transform thumbRingBone;

	// Token: 0x040007A0 RID: 1952
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x040007A1 RID: 1953
	public AudioClip extendAudioClip;

	// Token: 0x040007A2 RID: 1954
	public AudioClip retractAudioClip;

	// Token: 0x040007A3 RID: 1955
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x040007A4 RID: 1956
	public float extendVibrationStrength = 0.2f;

	// Token: 0x040007A5 RID: 1957
	public float retractVibrationDuration = 0.05f;

	// Token: 0x040007A6 RID: 1958
	public float retractVibrationStrength = 0.2f;

	// Token: 0x040007A7 RID: 1959
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x040007A8 RID: 1960
	private bool networkedExtended;

	// Token: 0x040007A9 RID: 1961
	private bool extended;

	// Token: 0x040007AA RID: 1962
	private InputDevice inputDevice;

	// Token: 0x040007AB RID: 1963
	private VRRig myRig;

	// Token: 0x040007AC RID: 1964
	private int stateBitIndex;
}
