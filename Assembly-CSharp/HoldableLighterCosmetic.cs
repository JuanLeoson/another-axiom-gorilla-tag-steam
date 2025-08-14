using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A8 RID: 168
public class HoldableLighterCosmetic : MonoBehaviour
{
	// Token: 0x0600042E RID: 1070 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnEnable()
	{
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00018979 File Offset: 0x00016B79
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00018993 File Offset: 0x00016B93
	private bool IsMyItem()
	{
		return this.rig != null && this.rig.isOfflineVRRig;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x000189B0 File Offset: 0x00016BB0
	private void DebugPull()
	{
		this.TriggerPulled();
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x000189B8 File Offset: 0x00016BB8
	private void DebugRelease()
	{
		this.TriggerReleased();
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x000189C0 File Offset: 0x00016BC0
	public void TriggerPulled()
	{
		this.triggerHeld = true;
		if (this.OwnerID == 0)
		{
			this.TrySetID();
		}
		double time = PhotonNetwork.Time;
		switch (this.GetResultAtTime(time, this.OwnerID))
		{
		case HoldableLighterCosmetic.LighterResult.Flicker:
		{
			UnityEvent onFlicker = this.OnFlicker;
			if (onFlicker != null)
			{
				onFlicker.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Light:
		{
			UnityEvent onLight = this.OnLight;
			if (onLight != null)
			{
				onLight.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Explode:
		{
			UnityEvent onExplode = this.OnExplode;
			if (onExplode != null)
			{
				onExplode.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.75f, 0.5f);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00018AC8 File Offset: 0x00016CC8
	private HoldableLighterCosmetic.LighterResult GetResultAtTime(double photonTime, int seed)
	{
		int num = (int)Math.Floor(photonTime);
		float num2 = (float)new Random(seed ^ num).NextDouble();
		if (num2 < this.explodeWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Explode;
		}
		if (num2 < this.explodeWeight + this.lightWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Light;
		}
		return HoldableLighterCosmetic.LighterResult.Flicker;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00018B0A File Offset: 0x00016D0A
	public void TriggerReleased()
	{
		this.triggerHeld = false;
		UnityEvent onTriggerRelease = this.OnTriggerRelease;
		if (onTriggerRelease == null)
		{
			return;
		}
		onTriggerRelease.Invoke();
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00018B24 File Offset: 0x00016D24
	private void TrySetID()
	{
		if (this.parentTransferable.IsLocalObject())
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
		{
			string userId = this.parentTransferable.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x040004B9 RID: 1209
	private int OwnerID;

	// Token: 0x040004BA RID: 1210
	[Header("Weights (0 to 1 total)")]
	[Range(0f, 1f)]
	public float flickerWeight = 0.5f;

	// Token: 0x040004BB RID: 1211
	[Range(0f, 1f)]
	public float lightWeight = 0.3f;

	// Token: 0x040004BC RID: 1212
	[Range(0f, 1f)]
	public float explodeWeight = 0.2f;

	// Token: 0x040004BD RID: 1213
	[Header("Unity Events")]
	public UnityEvent OnFlicker;

	// Token: 0x040004BE RID: 1214
	public UnityEvent OnLight;

	// Token: 0x040004BF RID: 1215
	public UnityEvent OnExplode;

	// Token: 0x040004C0 RID: 1216
	public UnityEvent OnTriggerRelease;

	// Token: 0x040004C1 RID: 1217
	private HoldableLighterCosmetic.LighterResult[] resultTimeline;

	// Token: 0x040004C2 RID: 1218
	private bool triggerHeld;

	// Token: 0x040004C3 RID: 1219
	private float lastCheckTime;

	// Token: 0x040004C4 RID: 1220
	private VRRig rig;

	// Token: 0x040004C5 RID: 1221
	private TransferrableObject parentTransferable;

	// Token: 0x020000A9 RID: 169
	public enum LighterResult
	{
		// Token: 0x040004C7 RID: 1223
		Flicker,
		// Token: 0x040004C8 RID: 1224
		Light,
		// Token: 0x040004C9 RID: 1225
		Explode
	}
}
