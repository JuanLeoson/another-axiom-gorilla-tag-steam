using System;
using UnityEngine;

// Token: 0x02000B26 RID: 2854
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class LightningStrike : MonoBehaviour
{
	// Token: 0x060044B0 RID: 17584 RVA: 0x00156F38 File Offset: 0x00155138
	private void Initialize()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.psMain = this.ps.main;
		this.psMain.playOnAwake = true;
		this.psMain.stopAction = ParticleSystemStopAction.Disable;
		this.psShape = this.ps.shape;
		this.psTrails = this.ps.trails;
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.playOnAwake = true;
	}

	// Token: 0x060044B1 RID: 17585 RVA: 0x00156FB4 File Offset: 0x001551B4
	public void Play(Vector3 p1, Vector3 p2, float beamWidthMultiplier, float audioVolume, float duration, Gradient colorOverLifetime)
	{
		if (this.ps == null)
		{
			this.Initialize();
		}
		base.transform.position = p1;
		base.transform.rotation = Quaternion.LookRotation(p1 - p2);
		this.psShape.radius = Vector3.Distance(p1, p2) * 0.5f;
		this.psShape.position = new Vector3(0f, 0f, -this.psShape.radius);
		this.psShape.randomPositionAmount = Mathf.Clamp(this.psShape.radius / 50f, 0f, 1f);
		this.psTrails.widthOverTrail = new ParticleSystem.MinMaxCurve(beamWidthMultiplier * 0.1f, beamWidthMultiplier);
		this.psTrails.colorOverLifetime = colorOverLifetime;
		this.psMain.duration = duration;
		this.audioSource.volume = Mathf.Clamp(this.psShape.radius / 5f, 0f, 1f) * audioVolume;
		base.gameObject.SetActive(true);
	}

	// Token: 0x04004ECF RID: 20175
	public static SRand rand = new SRand("LightningStrike");

	// Token: 0x04004ED0 RID: 20176
	private ParticleSystem ps;

	// Token: 0x04004ED1 RID: 20177
	private ParticleSystem.MainModule psMain;

	// Token: 0x04004ED2 RID: 20178
	private ParticleSystem.ShapeModule psShape;

	// Token: 0x04004ED3 RID: 20179
	private ParticleSystem.TrailModule psTrails;

	// Token: 0x04004ED4 RID: 20180
	private AudioSource audioSource;
}
