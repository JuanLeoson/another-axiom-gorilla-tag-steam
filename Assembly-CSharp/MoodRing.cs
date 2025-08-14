using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200049A RID: 1178
public class MoodRing : MonoBehaviour, ISpawnable
{
	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001D25 RID: 7461 RVA: 0x0009C663 File Offset: 0x0009A863
	// (set) Token: 0x06001D26 RID: 7462 RVA: 0x0009C66B File Offset: 0x0009A86B
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001D27 RID: 7463 RVA: 0x0009C674 File Offset: 0x0009A874
	// (set) Token: 0x06001D28 RID: 7464 RVA: 0x0009C67C File Offset: 0x0009A87C
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001D29 RID: 7465 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x0009C685 File Offset: 0x0009A885
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x0009C690 File Offset: 0x0009A890
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			if (!this.isCycling)
			{
				this.animRedValue = this.myRig.playerColor.r;
				this.animGreenValue = this.myRig.playerColor.g;
				this.animBlueValue = this.myRig.playerColor.b;
			}
			this.isCycling = true;
			this.RainbowCycle(ref this.animRedValue, ref this.animGreenValue, ref this.animBlueValue);
			this.myRig.InitializeNoobMaterialLocal(this.animRedValue, this.animGreenValue, this.animBlueValue);
			return;
		}
		if (this.isCycling)
		{
			this.isCycling = false;
			if (this.myRig.isOfflineVRRig)
			{
				this.animRedValue = Mathf.Round(this.animRedValue * 9f) / 9f;
				this.animGreenValue = Mathf.Round(this.animGreenValue * 9f) / 9f;
				this.animBlueValue = Mathf.Round(this.animBlueValue * 9f) / 9f;
				GorillaTagger.Instance.UpdateColor(this.animRedValue, this.animGreenValue, this.animBlueValue);
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						this.animRedValue,
						this.animGreenValue,
						this.animBlueValue
					});
				}
				PlayerPrefs.SetFloat("redValue", this.animRedValue);
				PlayerPrefs.SetFloat("greenValue", this.animGreenValue);
				PlayerPrefs.SetFloat("blueValue", this.animBlueValue);
				PlayerPrefs.Save();
			}
		}
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x0009C874 File Offset: 0x0009AA74
	private void RainbowCycle(ref float r, ref float g, ref float b)
	{
		float num = this.furCycleSpeed * Time.deltaTime;
		if (r == 1f)
		{
			if (b > 0f)
			{
				b = Mathf.Clamp01(b - num);
				return;
			}
			if (g < 1f)
			{
				g = Mathf.Clamp01(g + num);
				return;
			}
			r = Mathf.Clamp01(r - num);
			return;
		}
		else if (g == 1f)
		{
			if (r > 0f)
			{
				r = Mathf.Clamp01(r - num);
				return;
			}
			if (b < 1f)
			{
				b = Mathf.Clamp01(b + num);
				return;
			}
			g = Mathf.Clamp01(g - num);
			return;
		}
		else
		{
			if (b != 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			if (g > 0f)
			{
				g = Mathf.Clamp01(g - num);
				return;
			}
			if (r < 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			b = Mathf.Clamp01(b - num);
			return;
		}
	}

	// Token: 0x0400258D RID: 9613
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x0400258E RID: 9614
	private VRRig myRig;

	// Token: 0x0400258F RID: 9615
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002590 RID: 9616
	[SerializeField]
	private float furCycleSpeed;

	// Token: 0x04002591 RID: 9617
	private float nextFurCycleTimestamp;

	// Token: 0x04002592 RID: 9618
	private float animRedValue;

	// Token: 0x04002593 RID: 9619
	private float animGreenValue;

	// Token: 0x04002594 RID: 9620
	private float animBlueValue;

	// Token: 0x04002595 RID: 9621
	private bool isCycling;
}
