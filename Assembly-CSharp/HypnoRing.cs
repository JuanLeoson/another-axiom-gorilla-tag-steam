using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class HypnoRing : MonoBehaviour, ISpawnable
{
	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001D1D RID: 7453 RVA: 0x0009C515 File Offset: 0x0009A715
	// (set) Token: 0x06001D1E RID: 7454 RVA: 0x0009C51D File Offset: 0x0009A71D
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001D1F RID: 7455 RVA: 0x0009C526 File Offset: 0x0009A726
	// (set) Token: 0x06001D20 RID: 7456 RVA: 0x0009C52E File Offset: 0x0009A72E
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001D21 RID: 7457 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x0009C537 File Offset: 0x0009A737
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x0009C540 File Offset: 0x0009A740
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			base.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * this.rotationSpeed, Vector3.up);
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.maxVolume, Time.deltaTime / this.fadeInDuration);
			this.audioSource.volume = this.currentVolume;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
				return;
			}
		}
		else
		{
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, 0f, Time.deltaTime / this.fadeOutDuration);
			if (this.audioSource.isPlaying)
			{
				if (this.currentVolume == 0f)
				{
					this.audioSource.GTStop();
					return;
				}
				this.audioSource.volume = this.currentVolume;
			}
		}
	}

	// Token: 0x04002583 RID: 9603
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002584 RID: 9604
	private VRRig myRig;

	// Token: 0x04002585 RID: 9605
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002586 RID: 9606
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002587 RID: 9607
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04002588 RID: 9608
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04002589 RID: 9609
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x0400258C RID: 9612
	private float currentVolume;
}
