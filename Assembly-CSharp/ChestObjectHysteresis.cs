using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000430 RID: 1072
public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001A09 RID: 6665 RVA: 0x0008BBE5 File Offset: 0x00089DE5
	// (set) Token: 0x06001A0A RID: 6666 RVA: 0x0008BBED File Offset: 0x00089DED
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001A0B RID: 6667 RVA: 0x0008BBF6 File Offset: 0x00089DF6
	// (set) Token: 0x06001A0C RID: 6668 RVA: 0x0008BBFE File Offset: 0x00089DFE
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001A0D RID: 6669 RVA: 0x0008BC08 File Offset: 0x00089E08
	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!this.angleFollower && (string.IsNullOrEmpty(this.angleFollower_path) || base.transform.TryFindByPath(this.angleFollower_path, out this.angleFollower, false)))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"",
				this.angleFollower_path,
				"\". For component at: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x0008BC96 File Offset: 0x00089E96
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x0008BCBA File Offset: 0x00089EBA
	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x0008BCC2 File Offset: 0x00089EC2
	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x0008BCCC File Offset: 0x00089ECC
	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x0400224D RID: 8781
	public float angleHysteresis;

	// Token: 0x0400224E RID: 8782
	public float angleBetween;

	// Token: 0x0400224F RID: 8783
	public Transform angleFollower;

	// Token: 0x04002250 RID: 8784
	[Delayed]
	public string angleFollower_path;

	// Token: 0x04002251 RID: 8785
	private Quaternion lastAngleQuat;

	// Token: 0x04002252 RID: 8786
	private Quaternion currentAngleQuat;
}
