using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200011C RID: 284
public class GreyZoneSummoner : MonoBehaviour
{
	// Token: 0x170000AE RID: 174
	// (get) Token: 0x0600074A RID: 1866 RVA: 0x00029C8C File Offset: 0x00027E8C
	public Vector3 SummoningFocusPoint
	{
		get
		{
			return this.summoningFocusPoint.position;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600074B RID: 1867 RVA: 0x00029C99 File Offset: 0x00027E99
	public float SummonerMaxDistance
	{
		get
		{
			return this.areaTriggerCollider.radius + 1f;
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00029CAC File Offset: 0x00027EAC
	private void OnEnable()
	{
		this.greyZoneManager = GreyZoneManager.Instance;
		if (this.greyZoneManager == null)
		{
			return;
		}
		this.greyZoneManager.RegisterSummoner(this);
		this.areaTriggerNotifier.TriggerEnterEvent += this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent += this.ColliderExitedArea;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00029D10 File Offset: 0x00027F10
	private void OnDisable()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.DeregisterSummoner(this);
		}
		this.areaTriggerNotifier.TriggerEnterEvent -= this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent -= this.ColliderExitedArea;
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00029D68 File Offset: 0x00027F68
	public void UpdateProgressFeedback(bool greyZoneAvailable)
	{
		if (this.greyZoneManager == null)
		{
			return;
		}
		if (greyZoneAvailable && !this.candlesParent.gameObject.activeSelf)
		{
			this.candlesParent.gameObject.SetActive(true);
		}
		this.candlesTimeline.time = (double)Mathf.Clamp01(this.greyZoneManager.SummoningProgress) * this.candlesTimeline.duration;
		this.candlesTimeline.Evaluate();
		if (!this.greyZoneManager.GreyZoneActive)
		{
			float value = (float)this.summoningTones.Count * this.greyZoneManager.SummoningProgress;
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				float num = Mathf.InverseLerp((float)i, (float)i + 1f + this.summoningTonesFadeOverlap, value);
				this.summoningTones[i].volume = num * this.summoningTonesMaxVolume;
			}
		}
		this.greyZoneActivationButton.isOn = this.greyZoneManager.GreyZoneActive;
		this.greyZoneActivationButton.UpdateColor();
		for (int j = 0; j < this.greyZoneGravityFactorButtons.Count; j++)
		{
			this.greyZoneGravityFactorButtons[j].isOn = (this.greyZoneManager.GravityFactorSelection == j);
			this.greyZoneGravityFactorButtons[j].UpdateColor();
		}
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00029EB1 File Offset: 0x000280B1
	public void OnGreyZoneActivated()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeOutSummoningTones());
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00029EC6 File Offset: 0x000280C6
	private IEnumerator FadeOutSummoningTones()
	{
		float fadeStartTime = Time.time;
		float fadeRate = 1f / this.summoningTonesFadeTime;
		while (Time.time < fadeStartTime + this.summoningTonesFadeTime)
		{
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				this.summoningTones[i].volume = Mathf.MoveTowards(this.summoningTones[i].volume, 0f, this.summoningTonesMaxVolume * fadeRate * Time.deltaTime);
			}
			yield return null;
		}
		for (int j = 0; j < this.summoningTones.Count; j++)
		{
			this.summoningTones[j].volume = 0f;
		}
		yield break;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00029ED8 File Offset: 0x000280D8
	public void ColliderEnteredArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntity component = other.GetComponent<ZoneEntity>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigEnteredSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00029F24 File Offset: 0x00028124
	public void ColliderExitedArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntity component = other.GetComponent<ZoneEntity>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigExitedSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x040008DA RID: 2266
	[SerializeField]
	private Transform summoningFocusPoint;

	// Token: 0x040008DB RID: 2267
	[SerializeField]
	private Transform candlesParent;

	// Token: 0x040008DC RID: 2268
	[SerializeField]
	private PlayableDirector candlesTimeline;

	// Token: 0x040008DD RID: 2269
	[SerializeField]
	private TriggerEventNotifier areaTriggerNotifier;

	// Token: 0x040008DE RID: 2270
	[SerializeField]
	private SphereCollider areaTriggerCollider;

	// Token: 0x040008DF RID: 2271
	[SerializeField]
	private GorillaPressableButton greyZoneActivationButton;

	// Token: 0x040008E0 RID: 2272
	[SerializeField]
	private List<AudioSource> summoningTones = new List<AudioSource>();

	// Token: 0x040008E1 RID: 2273
	[SerializeField]
	private float summoningTonesMaxVolume = 1f;

	// Token: 0x040008E2 RID: 2274
	[SerializeField]
	private float summoningTonesFadeOverlap = 0.5f;

	// Token: 0x040008E3 RID: 2275
	[SerializeField]
	private float summoningTonesFadeTime = 4f;

	// Token: 0x040008E4 RID: 2276
	[SerializeField]
	private List<GorillaPressableButton> greyZoneGravityFactorButtons = new List<GorillaPressableButton>();

	// Token: 0x040008E5 RID: 2277
	private GreyZoneManager greyZoneManager;
}
