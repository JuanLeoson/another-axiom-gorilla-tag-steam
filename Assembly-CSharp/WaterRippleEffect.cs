using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000261 RID: 609
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x06000E20 RID: 3616 RVA: 0x00056A1D File Offset: 0x00054C1D
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00056A48 File Offset: 0x00054C48
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x00056A64 File Offset: 0x00054C64
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x00056AFC File Offset: 0x00054CFC
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x040016D2 RID: 5842
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x040016D3 RID: 5843
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x040016D4 RID: 5844
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x040016D5 RID: 5845
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x040016D6 RID: 5846
	private int ripplePlaybackSpeedHash;

	// Token: 0x040016D7 RID: 5847
	private float rippleStartTime = -1f;

	// Token: 0x040016D8 RID: 5848
	private Animator animator;

	// Token: 0x040016D9 RID: 5849
	private SpriteRenderer renderer;

	// Token: 0x040016DA RID: 5850
	private WaterVolume waterVolume;
}
