using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200060D RID: 1549
public class GRBreakable : MonoBehaviour, IGameHittable
{
	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002600 RID: 9728 RVA: 0x000CB56A File Offset: 0x000C976A
	public bool BrokenLocal
	{
		get
		{
			return this.brokenLocal;
		}
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000CB572 File Offset: 0x000C9772
	private void OnEnable()
	{
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000CB58B File Offset: 0x000C978B
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000CB5B4 File Offset: 0x000C97B4
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		GRBreakable.BreakableState breakableState = (GRBreakable.BreakableState)nextState;
		if (breakableState == GRBreakable.BreakableState.Broken)
		{
			this.BreakLocal();
			return;
		}
		if (breakableState == GRBreakable.BreakableState.Unbroken)
		{
			this.RestoreLocal();
		}
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000CB5D8 File Offset: 0x000C97D8
	public void BreakLocal()
	{
		if (!this.brokenLocal)
		{
			this.brokenLocal = true;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = false;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(true);
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.breakSound, this.breakSoundVolume);
			}
			GameEntity gameEntity;
			if (this.gameEntity.IsAuthority() && this.holdsRandomItem && this.itemSpawnProbability.TryForRandomItem(out gameEntity))
			{
				this.gameEntity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), this.itemSpawnLocation.position, this.itemSpawnLocation.rotation, 0L);
			}
		}
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000CB6F0 File Offset: 0x000C98F0
	public void RestoreLocal()
	{
		if (this.brokenLocal)
		{
			this.brokenLocal = false;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = true;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(true);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000CB780 File Offset: 0x000C9980
	public bool IsHitValid(GameHitData hit)
	{
		return !this.brokenLocal && hit.hitTypeId == 0;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000CB798 File Offset: 0x000C9998
	public void OnHit(GameHitData hit)
	{
		if (hit.hitTypeId == 0 && (int)this.gameEntity.GetState() != 1)
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
			GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(hit.hitByEntityId);
			if (gameEntity != null && gameEntity.IsHeldByLocalPlayer())
			{
				PlayerGameEvents.MiscEvent("GRSmashBreakable", 1);
			}
		}
	}

	// Token: 0x0400302F RID: 12335
	public GameEntity gameEntity;

	// Token: 0x04003030 RID: 12336
	public List<Transform> enableWhenBroken;

	// Token: 0x04003031 RID: 12337
	public List<Transform> disableWhenBroken;

	// Token: 0x04003032 RID: 12338
	public Collider breakableCollider;

	// Token: 0x04003033 RID: 12339
	public bool holdsRandomItem = true;

	// Token: 0x04003034 RID: 12340
	public Transform itemSpawnLocation;

	// Token: 0x04003035 RID: 12341
	public GRBreakableItemSpawnConfig itemSpawnProbability;

	// Token: 0x04003036 RID: 12342
	public AudioSource audioSource;

	// Token: 0x04003037 RID: 12343
	public AudioClip breakSound;

	// Token: 0x04003038 RID: 12344
	public float breakSoundVolume;

	// Token: 0x04003039 RID: 12345
	private bool brokenLocal;

	// Token: 0x0200060E RID: 1550
	public enum BreakableState
	{
		// Token: 0x0400303B RID: 12347
		Unbroken,
		// Token: 0x0400303C RID: 12348
		Broken
	}
}
