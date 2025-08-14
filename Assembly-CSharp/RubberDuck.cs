using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004AE RID: 1198
public class RubberDuck : TransferrableObject
{
	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001D96 RID: 7574 RVA: 0x0009EBAF File Offset: 0x0009CDAF
	// (set) Token: 0x06001D97 RID: 7575 RVA: 0x0009EBC1 File Offset: 0x0009CDC1
	public bool fxActive
	{
		get
		{
			return this.hasParticleFX && this._fxActive;
		}
		set
		{
			if (!this.hasParticleFX)
			{
				return;
			}
			this.pFXEmissionModule.enabled = value;
			this._fxActive = value;
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001D98 RID: 7576 RVA: 0x0009EBDF File Offset: 0x0009CDDF
	public int SqueezeSound
	{
		get
		{
			if (this.squeezeSoundBank.Length > 1)
			{
				return this.squeezeSoundBank[Random.Range(0, this.squeezeSoundBank.Length)];
			}
			if (this.squeezeSoundBank.Length == 1)
			{
				return this.squeezeSoundBank[0];
			}
			return this.squeezeSound;
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001D99 RID: 7577 RVA: 0x0009EC1C File Offset: 0x0009CE1C
	public int SqueezeReleaseSound
	{
		get
		{
			if (this.squeezeReleaseSoundBank.Length > 1)
			{
				return this.squeezeReleaseSoundBank[Random.Range(0, this.squeezeReleaseSoundBank.Length)];
			}
			if (this.squeezeReleaseSoundBank.Length == 1)
			{
				return this.squeezeReleaseSoundBank[0];
			}
			return this.squeezeReleaseSound;
		}
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x0009EC5C File Offset: 0x0009CE5C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		if (this.skinRenderer == null)
		{
			this.skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}
		this.hasSkinRenderer = (this.skinRenderer != null);
		this.myThreshold = 0.7f;
		this.hysterisis = 0.3f;
		this.hasParticleFX = (this.particleFX != null);
		if (this.hasParticleFX)
		{
			this.pFXEmissionModule = this.particleFX.emission;
			this.pFXEmissionModule.rateOverTime = this.particleFXEmissionIdle;
		}
		this.fxActive = false;
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x0009ECFC File Offset: 0x0009CEFC
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnSqueezeActivate;
			this._events.Deactivate += this.OnSqueezeDeactivate;
		}
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x0009EDEC File Offset: 0x0009CFEC
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Activate -= this.OnSqueezeActivate;
			this._events.Deactivate -= this.OnSqueezeDeactivate;
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x0009EE63 File Offset: 0x0009D063
	private void OnSqueezeActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeActivateLocal();
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x0009EE8A File Offset: 0x0009D08A
	private void SqueezeActivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionSqueeze);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x0009EEC7 File Offset: 0x0009D0C7
	private void OnSqueezeDeactivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnSqueezeDeactivate");
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeDeactivateLocal();
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x0009EEFA File Offset: 0x0009D0FA
	private void SqueezeDeactivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionIdle);
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x0009EF08 File Offset: 0x0009D108
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		float num = 0f;
		if (base.InHand())
		{
			this.tempHandPos = ((base.myOnlineRig != null) ? base.myOnlineRig.ReturnHandPosition() : base.myRig.ReturnHandPosition());
			if (this.currentState == TransferrableObject.PositionState.InLeftHand)
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10000) / 1000f);
			}
			else
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10) / 1f);
			}
		}
		if (this.hasSkinRenderer)
		{
			this.skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(this.skinRenderer.GetBlendShapeWeight(0), num * 11.1f, this.blendShapeMaxWeight));
		}
		if (this.fxActive)
		{
			this.squeezeTimeElapsed += Time.deltaTime;
			this.pFXEmissionModule.rateOverTime = Mathf.Lerp(this.particleFXEmissionIdle, this.particleFXEmissionSqueeze, this.particleFXEmissionCooldownCurve.Evaluate(this.squeezeTimeElapsed));
			if (this.squeezeTimeElapsed > this.particleFXEmissionSqueeze)
			{
				this.fxActive = false;
			}
		}
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x0009F024 File Offset: 0x0009D224
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			RigContainer localRig = VRRigCache.Instance.localRig;
			int num = this.SqueezeSound;
			localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			if (localRig.netView)
			{
				localRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					num,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.squeezeStrength, Time.deltaTime);
		}
		if (this._raiseActivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent activate = events.Activate;
				if (activate == null)
				{
					return;
				}
				activate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeActivateLocal();
			}
		}
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x0009F100 File Offset: 0x0009D300
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			int num = this.SqueezeReleaseSound;
			Debug.Log("Squeezy Deactivate: " + num.ToString());
			VRRigCache.Instance.localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			RigContainer rigContainer;
			if (GorillaGameManager.instance && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
			{
				rigContainer.Rig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					num,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.releaseStrength, Time.deltaTime);
		}
		if (this._raiseDeactivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent deactivate = events.Deactivate;
				if (deactivate == null)
				{
					return;
				}
				deactivate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeDeactivateLocal();
			}
		}
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x0009F20C File Offset: 0x0009D40C
	public void PlayParticleFX(float rate)
	{
		if (!this.hasParticleFX)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		if (!this.fxActive)
		{
			this.fxActive = true;
		}
		this.squeezeTimeElapsed = 0f;
		this.pFXEmissionModule.rateOverTime = rate;
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x0009F260 File Offset: 0x0009D460
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x0009F26B File Offset: 0x0009D46B
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x0400262A RID: 9770
	[DebugOption]
	public bool disableActivation;

	// Token: 0x0400262B RID: 9771
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x0400262C RID: 9772
	private SkinnedMeshRenderer skinRenderer;

	// Token: 0x0400262D RID: 9773
	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	// Token: 0x0400262E RID: 9774
	private int tempHandPos;

	// Token: 0x0400262F RID: 9775
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeSound = 75;

	// Token: 0x04002630 RID: 9776
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeReleaseSound = 76;

	// Token: 0x04002631 RID: 9777
	[GorillaSoundLookup]
	public int[] squeezeSoundBank;

	// Token: 0x04002632 RID: 9778
	[GorillaSoundLookup]
	public int[] squeezeReleaseSoundBank;

	// Token: 0x04002633 RID: 9779
	public float squeezeStrength = 0.05f;

	// Token: 0x04002634 RID: 9780
	public float releaseStrength = 0.03f;

	// Token: 0x04002635 RID: 9781
	public ParticleSystem particleFX;

	// Token: 0x04002636 RID: 9782
	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	// Token: 0x04002637 RID: 9783
	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	// Token: 0x04002638 RID: 9784
	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	// Token: 0x04002639 RID: 9785
	private bool hasSkinRenderer;

	// Token: 0x0400263A RID: 9786
	private ParticleSystem.EmissionModule pFXEmissionModule;

	// Token: 0x0400263B RID: 9787
	private bool hasParticleFX;

	// Token: 0x0400263C RID: 9788
	private float squeezeTimeElapsed;

	// Token: 0x0400263D RID: 9789
	[SerializeField]
	private RubberDuckEvents _events;

	// Token: 0x0400263E RID: 9790
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x0400263F RID: 9791
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04002640 RID: 9792
	[SerializeField]
	private SoundEffects _sfxActivate;

	// Token: 0x04002641 RID: 9793
	[SerializeField]
	private bool _fxActive;
}
