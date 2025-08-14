using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public abstract class CosmeticCritter : MonoBehaviour
{
	// Token: 0x17000368 RID: 872
	// (get) Token: 0x060021FC RID: 8700 RVA: 0x000B8913 File Offset: 0x000B6B13
	// (set) Token: 0x060021FD RID: 8701 RVA: 0x000B891B File Offset: 0x000B6B1B
	public int Seed { get; protected set; }

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x060021FE RID: 8702 RVA: 0x000B8924 File Offset: 0x000B6B24
	// (set) Token: 0x060021FF RID: 8703 RVA: 0x000B892C File Offset: 0x000B6B2C
	public CosmeticCritterSpawner Spawner { get; protected set; }

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002200 RID: 8704 RVA: 0x000B8935 File Offset: 0x000B6B35
	// (set) Token: 0x06002201 RID: 8705 RVA: 0x000B893D File Offset: 0x000B6B3D
	public Type CachedType { get; private set; }

	// Token: 0x06002202 RID: 8706 RVA: 0x000B8946 File Offset: 0x000B6B46
	public int GetGlobalMaxCritters()
	{
		return this.globalMaxCritters;
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000B894E File Offset: 0x000B6B4E
	public void SetSeedSpawnerTypeAndTime(int seed, CosmeticCritterSpawner spawner, Type type, double time)
	{
		this.Seed = seed;
		this.Spawner = spawner;
		this.CachedType = type;
		this.startTime = time;
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnSpawn()
	{
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnDespawn()
	{
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void SetRandomVariables()
	{
	}

	// Token: 0x06002207 RID: 8711
	public abstract void Tick();

	// Token: 0x06002208 RID: 8712 RVA: 0x000B896D File Offset: 0x000B6B6D
	protected double GetAliveTime()
	{
		if (!PhotonNetwork.InRoom)
		{
			return Time.timeAsDouble - this.startTime;
		}
		return PhotonNetwork.Time - this.startTime;
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000B898F File Offset: 0x000B6B8F
	public virtual bool Expired()
	{
		return this.GetAliveTime() > (double)this.lifetime || this.GetAliveTime() < 0.0;
	}

	// Token: 0x04002B8D RID: 11149
	[Tooltip("After this many seconds the critter will forcibly despawn.")]
	[SerializeField]
	protected float lifetime;

	// Token: 0x04002B8E RID: 11150
	[Tooltip("The maximum number of this kind of critter that can be in the room at any given time.")]
	[SerializeField]
	private int globalMaxCritters;

	// Token: 0x04002B92 RID: 11154
	protected double startTime;
}
