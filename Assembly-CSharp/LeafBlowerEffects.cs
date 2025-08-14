using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class LeafBlowerEffects : MonoBehaviour, ISpawnable
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x000396E9 File Offset: 0x000378E9
	// (set) Token: 0x06000AAA RID: 2730 RVA: 0x000396F1 File Offset: 0x000378F1
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000AAB RID: 2731 RVA: 0x000396FA File Offset: 0x000378FA
	// (set) Token: 0x06000AAC RID: 2732 RVA: 0x00039702 File Offset: 0x00037902
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000AAD RID: 2733 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0003970C File Offset: 0x0003790C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.headToleranceAngleCos = Mathf.Cos(0.017453292f * this.headToleranceAngle);
		this.squareHitAngleCos = Mathf.Cos(0.017453292f * this.squareHitAngle);
		this.fan = rig.cosmeticReferences.Get(this.fanRef).GetComponent<CosmeticFan>();
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00039763 File Offset: 0x00037963
	public void StartFan()
	{
		this.fan.Run();
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00039770 File Offset: 0x00037970
	public void StopFan()
	{
		this.fan.Stop();
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0003977D File Offset: 0x0003797D
	public void UpdateEffects()
	{
		this.ProjectParticles();
		this.BlowFaces();
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0003978C File Offset: 0x0003798C
	public void ProjectParticles()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.gunBarrel.transform.position, this.gunBarrel.transform.forward, out raycastHit, this.projectionRange, this.raycastLayers))
		{
			SpawnOnEnter component = raycastHit.collider.GetComponent<SpawnOnEnter>();
			if (component != null)
			{
				component.OnTriggerEnter(raycastHit.collider);
			}
			if (Vector3.Dot(raycastHit.normal, this.gunBarrel.transform.forward) < -this.squareHitAngleCos)
			{
				this.squareHitParticleSystem.transform.position = raycastHit.point;
				this.squareHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Play(true);
					return;
				}
			}
			else
			{
				this.angledHitParticleSystem.transform.position = raycastHit.point;
				this.angledHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Play(true);
					return;
				}
			}
		}
		else
		{
			this.StopEffects();
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0003993E File Offset: 0x00037B3E
	public void StopEffects()
	{
		this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0003995C File Offset: 0x00037B5C
	public void BlowFaces()
	{
		Vector3 position = this.gunBarrel.transform.position;
		Vector3 forward = this.gunBarrel.transform.forward;
		if (NetworkSystem.Instance.InRoom)
		{
			using (List<VRRig>.Enumerator enumerator = GorillaParent.instance.vrrigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig rig = enumerator.Current;
					this.TryBlowFace(rig, position, forward);
				}
				return;
			}
		}
		this.TryBlowFace(VRRig.LocalRig, position, forward);
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x000399F4 File Offset: 0x00037BF4
	private void TryBlowFace(VRRig rig, Vector3 origin, Vector3 directionNormalized)
	{
		Transform rigTarget = rig.head.rigTarget;
		Vector3 vector = rigTarget.position - origin;
		float num = Vector3.Dot(vector, directionNormalized);
		if (num < 0f || num > this.projectionRange)
		{
			return;
		}
		if ((vector - num * directionNormalized).IsLongerThan(this.projectionWidth))
		{
			return;
		}
		if (Vector3.Dot(-rigTarget.forward, vector.normalized) < this.headToleranceAngleCos)
		{
			return;
		}
		rig.GetComponent<GorillaMouthFlap>().EnableLeafBlower();
	}

	// Token: 0x04000D0F RID: 3343
	[SerializeField]
	private GameObject gunBarrel;

	// Token: 0x04000D10 RID: 3344
	[SerializeField]
	private float projectionRange;

	// Token: 0x04000D11 RID: 3345
	[SerializeField]
	private float projectionWidth;

	// Token: 0x04000D12 RID: 3346
	[SerializeField]
	private float headToleranceAngle;

	// Token: 0x04000D13 RID: 3347
	[SerializeField]
	private LayerMask raycastLayers;

	// Token: 0x04000D14 RID: 3348
	[SerializeField]
	private ParticleSystem angledHitParticleSystem;

	// Token: 0x04000D15 RID: 3349
	[SerializeField]
	private ParticleSystem squareHitParticleSystem;

	// Token: 0x04000D16 RID: 3350
	[SerializeField]
	private float squareHitAngle;

	// Token: 0x04000D17 RID: 3351
	[SerializeField]
	private CosmeticRefID fanRef;

	// Token: 0x04000D18 RID: 3352
	private float headToleranceAngleCos;

	// Token: 0x04000D19 RID: 3353
	private float squareHitAngleCos;

	// Token: 0x04000D1A RID: 3354
	private CosmeticFan fan;
}
