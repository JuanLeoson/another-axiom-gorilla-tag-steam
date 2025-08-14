using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
public class GameEntity : MonoBehaviour
{
	// Token: 0x1700037E RID: 894
	// (get) Token: 0x0600234A RID: 9034 RVA: 0x000BDA28 File Offset: 0x000BBC28
	// (set) Token: 0x0600234B RID: 9035 RVA: 0x000BDA30 File Offset: 0x000BBC30
	[DebugReadout]
	public GameEntityId id { get; internal set; }

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x0600234C RID: 9036 RVA: 0x000BDA39 File Offset: 0x000BBC39
	// (set) Token: 0x0600234D RID: 9037 RVA: 0x000BDA41 File Offset: 0x000BBC41
	[DebugReadout]
	public int typeId { get; private set; }

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x0600234E RID: 9038 RVA: 0x000BDA4A File Offset: 0x000BBC4A
	// (set) Token: 0x0600234F RID: 9039 RVA: 0x000BDA52 File Offset: 0x000BBC52
	[DebugReadout]
	public long createData { get; private set; }

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06002350 RID: 9040 RVA: 0x000BDA5B File Offset: 0x000BBC5B
	// (set) Token: 0x06002351 RID: 9041 RVA: 0x000BDA63 File Offset: 0x000BBC63
	[DebugReadout]
	public int heldByActorNumber { get; internal set; }

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06002352 RID: 9042 RVA: 0x000BDA6C File Offset: 0x000BBC6C
	// (set) Token: 0x06002353 RID: 9043 RVA: 0x000BDA74 File Offset: 0x000BBC74
	[DebugReadout]
	public int heldByHandIndex { get; internal set; }

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06002354 RID: 9044 RVA: 0x000BDA7D File Offset: 0x000BBC7D
	// (set) Token: 0x06002355 RID: 9045 RVA: 0x000BDA85 File Offset: 0x000BBC85
	[DebugReadout]
	public int lastHeldByActorNumber { get; internal set; }

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06002356 RID: 9046 RVA: 0x000BDA8E File Offset: 0x000BBC8E
	// (set) Token: 0x06002357 RID: 9047 RVA: 0x000BDA96 File Offset: 0x000BBC96
	[DebugReadout]
	public int onlyGrabActorNumber { get; internal set; }

	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06002358 RID: 9048 RVA: 0x000BDAA0 File Offset: 0x000BBCA0
	// (remove) Token: 0x06002359 RID: 9049 RVA: 0x000BDAD8 File Offset: 0x000BBCD8
	public event GameEntity.StateChangedEvent OnStateChanged;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x0600235A RID: 9050 RVA: 0x000BDB10 File Offset: 0x000BBD10
	// (remove) Token: 0x0600235B RID: 9051 RVA: 0x000BDB48 File Offset: 0x000BBD48
	public event GameEntity.EntityDestroyedEvent onEntityDestroyed;

	// Token: 0x0600235C RID: 9052 RVA: 0x000BDB80 File Offset: 0x000BBD80
	private void Awake()
	{
		this.id = GameEntityId.Invalid;
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.heldByActorNumber = -1;
		this.heldByHandIndex = -1;
		this.onlyGrabActorNumber = -1;
		this.entityComponents = new List<IGameEntityComponent>(1);
		base.GetComponentsInChildren<IGameEntityComponent>(this.entityComponents);
		this.entitySerialize = new List<IGameEntitySerialize>(1);
		base.GetComponentsInChildren<IGameEntitySerialize>(this.entitySerialize);
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000BDBEC File Offset: 0x000BBDEC
	public void Init(GameEntityManager manager, int typeId, long createData)
	{
		this.manager = manager;
		this.typeId = typeId;
		this.createData = createData;
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityInit();
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000BDC38 File Offset: 0x000BBE38
	public void OnDestroy()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityDestroy();
		}
		GameEntity.EntityDestroyedEvent entityDestroyedEvent = this.onEntityDestroyed;
		if (entityDestroyedEvent == null)
		{
			return;
		}
		entityDestroyedEvent(this);
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000BDC85 File Offset: 0x000BBE85
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000BDCA6 File Offset: 0x000BBEA6
	public void PlayCatchFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.PlayOneShot(this.catchSound);
		}
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000BDCD8 File Offset: 0x000BBED8
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.PlayOneShot(this.throwSound);
		}
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000BDD0A File Offset: 0x000BBF0A
	private bool IsGamePlayer(Collider collider)
	{
		return GamePlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x000BDD19 File Offset: 0x000BBF19
	public long GetState()
	{
		return this.state;
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000BDD21 File Offset: 0x000BBF21
	public void RequestState(GameEntityId id, long newState)
	{
		this.manager.RequestState(id, newState);
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000BDD30 File Offset: 0x000BBF30
	public bool IsAuthority()
	{
		return this.manager.IsAuthority();
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000BDD40 File Offset: 0x000BBF40
	public void SetState(long newState)
	{
		if (this.state != newState)
		{
			long prevState = this.state;
			this.state = newState;
			GameEntity.StateChangedEvent onStateChanged = this.OnStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged(prevState, newState);
			}
			for (int i = 0; i < this.entityComponents.Count; i++)
			{
				this.entityComponents[i].OnEntityStateChange(prevState, newState);
			}
		}
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000BDDA0 File Offset: 0x000BBFA0
	public int GetNetId(GameEntityId gameEntityId)
	{
		return this.manager.GetNetIdFromEntityId(gameEntityId);
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000BDDAE File Offset: 0x000BBFAE
	public int GetNetId()
	{
		return this.manager.GetNetIdFromEntityId(this.id);
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000BDDC4 File Offset: 0x000BBFC4
	public static GameEntity Get(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameEntity component = transform.GetComponent<GameEntity>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x000BDE08 File Offset: 0x000BC008
	public bool IsHeldByLocalPlayer()
	{
		return this.heldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x04002CEF RID: 11503
	public const int Invalid = -1;

	// Token: 0x04002CF3 RID: 11507
	public bool pickupable = true;

	// Token: 0x04002CF4 RID: 11508
	public AudioSource audioSource;

	// Token: 0x04002CF5 RID: 11509
	public AudioClip catchSound;

	// Token: 0x04002CF6 RID: 11510
	public float catchSoundVolume;

	// Token: 0x04002CF7 RID: 11511
	public AudioClip throwSound;

	// Token: 0x04002CF8 RID: 11512
	public float throwSoundVolume;

	// Token: 0x04002CF9 RID: 11513
	private Rigidbody rigidBody;

	// Token: 0x04002CFE RID: 11518
	[NonSerialized]
	public GameEntityManager manager;

	// Token: 0x04002CFF RID: 11519
	public Action OnGrabbed;

	// Token: 0x04002D00 RID: 11520
	public Action OnReleased;

	// Token: 0x04002D03 RID: 11523
	private long state;

	// Token: 0x04002D04 RID: 11524
	private List<IGameEntityComponent> entityComponents;

	// Token: 0x04002D05 RID: 11525
	public List<IGameEntitySerialize> entitySerialize;

	// Token: 0x020005A5 RID: 1445
	// (Invoke) Token: 0x0600236D RID: 9069
	public delegate void StateChangedEvent(long prevState, long nextState);

	// Token: 0x020005A6 RID: 1446
	// (Invoke) Token: 0x06002371 RID: 9073
	public delegate void EntityDestroyedEvent(GameEntity entity);
}
