using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class SecondLookSkeletonEnabler : Tappable
{
	// Token: 0x0600055C RID: 1372 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001F724 File Offset: 0x0001D924
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, info);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.GTPlay();
			this.particles.Play();
		}
	}

	// Token: 0x0400066A RID: 1642
	public bool isTapped;

	// Token: 0x0400066B RID: 1643
	public AudioSource playOnDisappear;

	// Token: 0x0400066C RID: 1644
	public ParticleSystem particles;

	// Token: 0x0400066D RID: 1645
	public GameObject spookyText;

	// Token: 0x0400066E RID: 1646
	private SecondLookSkeleton skele;
}
