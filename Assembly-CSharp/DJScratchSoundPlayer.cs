using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class DJScratchSoundPlayer : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000A0C RID: 2572 RVA: 0x00036F7D File Offset: 0x0003517D
	// (set) Token: 0x06000A0D RID: 2573 RVA: 0x00036F85 File Offset: 0x00035185
	public bool IsSpawned { get; set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000A0E RID: 2574 RVA: 0x00036F8E File Offset: 0x0003518E
	// (set) Token: 0x06000A0F RID: 2575 RVA: 0x00036F96 File Offset: 0x00035196
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000A10 RID: 2576 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00036FA0 File Offset: 0x000351A0
	private void OnEnable()
	{
		if (this._events.IsNull())
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
		}
		this._events.Activate += this.OnPlayEvent;
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00037032 File Offset: 0x00035232
	private void OnDisable()
	{
		if (this._events.IsNotNull())
		{
			this._events.Activate -= this.OnPlayEvent;
			this._events.Dispose();
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0003706E File Offset: 0x0003526E
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!rig.isLocal)
		{
			this.scratchTableLeft.enabled = false;
			this.scratchTableRight.enabled = false;
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00037097 File Offset: 0x00035297
	public void Play(ScratchSoundType type, bool isLeft)
	{
		if (this.myRig.isLocal)
		{
			this.PlayLocal(type, isLeft);
			this._events.Activate.RaiseOthers(new object[]
			{
				(int)(type + (isLeft ? 100 : 0))
			});
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x000370D8 File Offset: 0x000352D8
	public void OnPlayEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length != 1)
		{
			Debug.LogError(string.Format("Invalid DJ Scratch Event - expected 1 arg, got {0}", args.Length));
			return;
		}
		int num = (int)args[0];
		bool flag = num >= 100;
		if (flag)
		{
			num -= 100;
		}
		ScratchSoundType scratchSoundType = (ScratchSoundType)num;
		if (scratchSoundType < ScratchSoundType.Pause || scratchSoundType > ScratchSoundType.Back)
		{
			return;
		}
		this.PlayLocal(scratchSoundType, flag);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00037150 File Offset: 0x00035350
	public void PlayLocal(ScratchSoundType type, bool isLeft)
	{
		switch (type)
		{
		case ScratchSoundType.Pause:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			this.scratchPause.Play();
			return;
		case ScratchSoundType.Resume:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).ResumeTrack();
			this.scratchResume.Play();
			return;
		case ScratchSoundType.Forward:
			this.scratchForward.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		case ScratchSoundType.Back:
			this.scratchBack.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		default:
			return;
		}
	}

	// Token: 0x04000C20 RID: 3104
	[SerializeField]
	private SoundBankPlayer scratchForward;

	// Token: 0x04000C21 RID: 3105
	[SerializeField]
	private SoundBankPlayer scratchBack;

	// Token: 0x04000C22 RID: 3106
	[SerializeField]
	private SoundBankPlayer scratchPause;

	// Token: 0x04000C23 RID: 3107
	[SerializeField]
	private SoundBankPlayer scratchResume;

	// Token: 0x04000C24 RID: 3108
	[SerializeField]
	private DJScratchtable scratchTableLeft;

	// Token: 0x04000C25 RID: 3109
	[SerializeField]
	private DJScratchtable scratchTableRight;

	// Token: 0x04000C26 RID: 3110
	private RubberDuckEvents _events;

	// Token: 0x04000C27 RID: 3111
	private VRRig myRig;
}
