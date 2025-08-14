using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005F9 RID: 1529
[Serializable]
public class GRAbilityDie
{
	// Token: 0x0600258D RID: 9613 RVA: 0x000C98A4 File Offset: 0x000C7AA4
	public void Setup(GameAgent agent, Animation anim, Transform root)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		if (this.disableAllCollidersWhenDead)
		{
			agent.GetComponentsInChildren<Collider>(this.disableCollidersWhenDead);
		}
		if (this.disableAllRenderersWhenDead)
		{
			agent.GetComponentsInChildren<Renderer>(this.hideWhenDead);
		}
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.staggerMovement.Setup(root);
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x000C9908 File Offset: 0x000C7B08
	public void Start()
	{
		this.startTime = Time.timeAsDouble;
		if (this.animData.Count > 0)
		{
			int index = Random.Range(0, this.animData.Count);
			this.delayDeath = this.animData[index].duration;
			this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.delayDeath);
			this.PlayAnim(this.animData[index].animName, 0.1f, this.animData[index].speed);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.isDead = false;
		if (this.doKnockback)
		{
			this.staggerMovement.Start();
		}
		this.soundDeath.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundOnHide.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundDeath.Play(null);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
	}

	// Token: 0x0600258F RID: 9615 RVA: 0x000C9A03 File Offset: 0x000C7C03
	public void Stop()
	{
		this.staggerMovement.Stop();
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		GRAbilityDie.Hide(this.hideWhenDead, false);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000C9A41 File Offset: 0x000C7C41
	public void SetStaggerVelocity(Vector3 vel)
	{
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.delayDeath);
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000C9A58 File Offset: 0x000C7C58
	private void Die()
	{
		this.soundOnHide.Play(null);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
			this.fxDeath.SetActive(true);
		}
		GRAbilityDie.Hide(this.hideWhenDead, true);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		GameEntity entity = this.agent.entity;
		GameEntity gameEntity;
		if (this.lootTable != null && entity.IsAuthority() && this.lootTable.TryForRandomItem(out gameEntity))
		{
			Transform transform = this.lootSpawnMarker;
			if (transform == null)
			{
				transform = this.agent.transform;
			}
			entity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), transform.position, transform.rotation, 0L);
		}
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000C9B28 File Offset: 0x000C7D28
	public void DestroySelf()
	{
		GameEntity entity = this.agent.entity;
		if (entity.IsAuthority())
		{
			entity.manager.RequestDestroyItem(entity.id);
		}
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000C9B5A File Offset: 0x000C7D5A
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsDone()
	{
		return false;
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x000C9B94 File Offset: 0x000C7D94
	public void Update(float dt)
	{
		if (this.startTime >= 0.0)
		{
			if (this.doKnockback)
			{
				this.staggerMovement.Update(dt);
			}
			double num = Time.timeAsDouble - this.startTime;
			if (!this.isDead && num > (double)this.delayDeath)
			{
				this.isDead = true;
				this.Die();
				return;
			}
			if (this.isDead && num > (double)(this.delayDeath + this.destroyDelay))
			{
				this.DestroySelf();
				this.startTime = -1.0;
			}
		}
	}

	// Token: 0x06002596 RID: 9622 RVA: 0x000C9C24 File Offset: 0x000C7E24
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000C9C68 File Offset: 0x000C7E68
	public static void Disable(List<Collider> colliders, bool disable)
	{
		if (colliders == null)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].enabled = !disable;
			}
		}
	}

	// Token: 0x04002F97 RID: 12183
	public float delayDeath;

	// Token: 0x04002F98 RID: 12184
	public List<Renderer> hideWhenDead;

	// Token: 0x04002F99 RID: 12185
	public List<Collider> disableCollidersWhenDead;

	// Token: 0x04002F9A RID: 12186
	public bool disableAllCollidersWhenDead;

	// Token: 0x04002F9B RID: 12187
	public bool disableAllRenderersWhenDead;

	// Token: 0x04002F9C RID: 12188
	public GameObject fxDeath;

	// Token: 0x04002F9D RID: 12189
	public AbilitySound soundDeath;

	// Token: 0x04002F9E RID: 12190
	public AbilitySound soundOnHide;

	// Token: 0x04002F9F RID: 12191
	public float destroyDelay = 3f;

	// Token: 0x04002FA0 RID: 12192
	public bool doKnockback = true;

	// Token: 0x04002FA1 RID: 12193
	public GRBreakableItemSpawnConfig lootTable;

	// Token: 0x04002FA2 RID: 12194
	public Transform lootSpawnMarker;

	// Token: 0x04002FA3 RID: 12195
	private GameAgent agent;

	// Token: 0x04002FA4 RID: 12196
	private Animation anim;

	// Token: 0x04002FA5 RID: 12197
	public List<AnimationData> animData;

	// Token: 0x04002FA6 RID: 12198
	private Transform root;

	// Token: 0x04002FA7 RID: 12199
	private double startTime;

	// Token: 0x04002FA8 RID: 12200
	private bool isDead;

	// Token: 0x04002FA9 RID: 12201
	public GRAbilityInterpolatedMovement staggerMovement;
}
