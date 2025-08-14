using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public class GRBarrierSpectral : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x060025ED RID: 9709 RVA: 0x000CAFE4 File Offset: 0x000C91E4
	public void Awake()
	{
		this.hitFx.SetActive(false);
		this.destroyedFx.SetActive(false);
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000CB000 File Offset: 0x000C9200
	public void OnEntityInit()
	{
		this.entity.SetState((long)this.health);
		Vector3 localScale = BitPackUtils.UnpackWorldPosFromNetwork(this.entity.createData);
		base.transform.localScale = localScale;
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000CB03C File Offset: 0x000C923C
	public void OnEntityStateChange(long prevState, long newState)
	{
		int nextHealth = (int)newState;
		this.ChangeHealth(nextHealth);
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000CB054 File Offset: 0x000C9254
	public void OnImpact(GameHitType hitType)
	{
		if (hitType == GameHitType.Flash)
		{
			int nextHealth = Mathf.Max(this.health - 1, 0);
			this.ChangeHealth(nextHealth);
			if (this.entity.IsAuthority())
			{
				this.entity.RequestState(this.entity.id, (long)this.health);
			}
		}
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000CB0A8 File Offset: 0x000C92A8
	private void ChangeHealth(int nextHealth)
	{
		if (this.health != nextHealth)
		{
			this.health = nextHealth;
			if (this.health == 0)
			{
				this.collider.enabled = false;
				this.visualMesh.enabled = false;
				this.audioSource.PlayOneShot(this.onDestroyedClip, this.onDestroyedVolume);
				this.destroyedFx.SetActive(false);
				this.destroyedFx.SetActive(true);
			}
			else
			{
				this.audioSource.PlayOneShot(this.onDamageClip, this.onDamageVolume);
				this.hitFx.SetActive(false);
				this.hitFx.SetActive(true);
			}
			this.RefreshVisuals();
		}
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000CB150 File Offset: 0x000C9350
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			this.OnImpact(hitTypeId);
		}
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000CB18C File Offset: 0x000C938C
	public void RefreshVisuals()
	{
		if (this.lastVisualUpdateHealth != this.health)
		{
			this.lastVisualUpdateHealth = this.health;
			Color color = this.visualMesh.material.GetColor("_BaseColor");
			color.a = (float)this.health / (float)this.maxHealth;
			this.visualMesh.material.SetColor("_BaseColor", color);
		}
	}

	// Token: 0x04003015 RID: 12309
	public GameEntity entity;

	// Token: 0x04003016 RID: 12310
	public MeshRenderer visualMesh;

	// Token: 0x04003017 RID: 12311
	public Collider collider;

	// Token: 0x04003018 RID: 12312
	public AudioSource audioSource;

	// Token: 0x04003019 RID: 12313
	public AudioClip onDamageClip;

	// Token: 0x0400301A RID: 12314
	public float onDamageVolume;

	// Token: 0x0400301B RID: 12315
	public AudioClip onDestroyedClip;

	// Token: 0x0400301C RID: 12316
	public float onDestroyedVolume;

	// Token: 0x0400301D RID: 12317
	[SerializeField]
	private GameObject hitFx;

	// Token: 0x0400301E RID: 12318
	[SerializeField]
	private GameObject destroyedFx;

	// Token: 0x0400301F RID: 12319
	public int maxHealth = 3;

	// Token: 0x04003020 RID: 12320
	[ReadOnly]
	public int health = 3;

	// Token: 0x04003021 RID: 12321
	private int lastVisualUpdateHealth = -1;
}
