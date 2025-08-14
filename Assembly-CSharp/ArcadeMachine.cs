using System;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002A5 RID: 677
[NetworkBehaviourWeaved(128)]
public class ArcadeMachine : NetworkComponent
{
	// Token: 0x06000FA7 RID: 4007 RVA: 0x0005BC2C File Offset: 0x00059E2C
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x0005BC40 File Offset: 0x00059E40
	protected override void Start()
	{
		base.Start();
		if (this.arcadeGame != null && this.arcadeGame.Scale.x > 0f && this.arcadeGame.Scale.y > 0f)
		{
			this.arcadeGameInstance = UnityEngine.Object.Instantiate<ArcadeGame>(this.arcadeGame, this.screen.transform);
			this.arcadeGameInstance.transform.localScale = new Vector3(1f / this.arcadeGameInstance.Scale.x, 1f / this.arcadeGameInstance.Scale.y, 1f);
			this.screen.forceRenderingOff = true;
			this.arcadeGameInstance.SetMachine(this);
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0005BD10 File Offset: 0x00059F10
	public void PlaySound(int soundId, int priority)
	{
		if (!this.audioSource.isPlaying || this.audioSourcePriority >= priority)
		{
			this.audioSource.GTStop();
			this.audioSourcePriority = priority;
			this.audioSource.clip = this.arcadeGameInstance.audioClips[soundId];
			this.audioSource.GTPlay();
			if (this.networkSynchronized && base.IsMine)
			{
				base.GetView.RPC("ArcadeGameInstance_OnPlaySound_RPC", RpcTarget.Others, new object[]
				{
					soundId
				});
			}
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0005BD98 File Offset: 0x00059F98
	public bool IsPlayerLocallyControlled(int player)
	{
		return this.sticks[player].heldByLocalPlayer;
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0005BDA8 File Offset: 0x00059FA8
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		for (int i = 0; i < this.sticks.Length; i++)
		{
			this.sticks[i].Init(this, i);
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0005BDE3 File Offset: 0x00059FE3
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0005BDF4 File Offset: 0x00059FF4
	[PunRPC]
	private void ArcadeGameInstance_OnPlaySound_RPC(int id, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || id > this.arcadeGameInstance.audioClips.Length || id < 0 || !this.soundCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.clip = this.arcadeGameInstance.audioClips[id];
		this.audioSource.GTPlay();
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0005BE63 File Offset: 0x0005A063
	public void OnJoystickStateChange(int player, ArcadeButtons buttons)
	{
		if (this.arcadeGameInstance != null)
		{
			this.arcadeGameInstance.OnInputStateChange(player, buttons);
		}
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0005BE7C File Offset: 0x0005A07C
	public bool IsControllerInUse(int player)
	{
		if (base.IsMine)
		{
			return this.playersPerJoystick[player] != null && Time.time < this.playerIdleTimeouts[player];
		}
		return (this.buttonsStateValue & 1 << player * 8) != 0;
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0005BEB4 File Offset: 0x0005A0B4
	[Networked]
	[Capacity(128)]
	[NetworkedWeaved(0, 128)]
	public unsafe NetworkArray<byte> Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArcadeMachine.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<byte>((byte*)(this.Ptr + 0), 128, ReaderWriter@System_Byte.GetInstance());
		}
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0005BEF4 File Offset: 0x0005A0F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0005BF01 File Offset: 0x0005A101
	public void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.ReadPlayerDataPUN(player, stream, info);
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0005BF11 File Offset: 0x0005A111
	public void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.WritePlayerDataPUN(player, stream, info);
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0005BF48 File Offset: 0x0005A148
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.Data, this._Data, "Data");
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0005BF6A File Offset: 0x0005A16A
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.Data, ref this._Data);
	}

	// Token: 0x04001845 RID: 6213
	[SerializeField]
	private ArcadeGame arcadeGame;

	// Token: 0x04001846 RID: 6214
	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	// Token: 0x04001847 RID: 6215
	[SerializeField]
	private Renderer screen;

	// Token: 0x04001848 RID: 6216
	[SerializeField]
	private bool networkSynchronized = true;

	// Token: 0x04001849 RID: 6217
	[SerializeField]
	private CallLimiter soundCallLimit;

	// Token: 0x0400184A RID: 6218
	private int buttonsStateValue;

	// Token: 0x0400184B RID: 6219
	private AudioSource audioSource;

	// Token: 0x0400184C RID: 6220
	private int audioSourcePriority;

	// Token: 0x0400184D RID: 6221
	private ArcadeGame arcadeGameInstance;

	// Token: 0x0400184E RID: 6222
	private Player[] playersPerJoystick = new Player[4];

	// Token: 0x0400184F RID: 6223
	private float[] playerIdleTimeouts = new float[4];

	// Token: 0x04001850 RID: 6224
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 128)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private byte[] _Data;
}
