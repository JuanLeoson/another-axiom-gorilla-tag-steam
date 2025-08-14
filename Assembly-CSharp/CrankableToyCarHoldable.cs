using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class CrankableToyCarHoldable : TransferrableObject
{
	// Token: 0x060009DF RID: 2527 RVA: 0x000360F6 File Offset: 0x000342F6
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00036118 File Offset: 0x00034318
	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += this.OnDeployRPC;
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00036203 File Offset: 0x00034403
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00036224 File Offset: 0x00034424
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00036278 File Offset: 0x00034478
	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float value = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? pitchOverride = new float?(value);
				soundBankPlayer.Play(null, pitchOverride);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0003638C File Offset: 0x0003458C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool isRightHand = releasingHand == EquipmentInteractor.instance.rightHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isRightHand);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000364BB File Offset: 0x000346BB
	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x000364E0 File Offset: 0x000346E0
	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 launchPos = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 releaseVel = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float lifetime = (float)args[3];
		float num = 10000f;
		if (launchPos.IsValid(num) && launchRot.IsValid())
		{
			float num2 = 10000f;
			if (releaseVel.IsValid(num2))
			{
				this.DeployCarLocal(launchPos, launchRot, releaseVel, lifetime, true);
				return;
			}
		}
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00036595 File Offset: 0x00034795
	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x000365BD File Offset: 0x000347BD
	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	// Token: 0x04000BDC RID: 3036
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	// Token: 0x04000BDD RID: 3037
	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	// Token: 0x04000BDE RID: 3038
	[SerializeField]
	private GameObject deployablePart;

	// Token: 0x04000BDF RID: 3039
	[SerializeField]
	private GameObject disabledWhileDeployed;

	// Token: 0x04000BE0 RID: 3040
	[SerializeField]
	private float crankAnglePerClick;

	// Token: 0x04000BE1 RID: 3041
	[SerializeField]
	private float maxCrankStrength;

	// Token: 0x04000BE2 RID: 3042
	[SerializeField]
	private float minClickPitch;

	// Token: 0x04000BE3 RID: 3043
	[SerializeField]
	private float maxClickPitch;

	// Token: 0x04000BE4 RID: 3044
	[SerializeField]
	private float minLifetime;

	// Token: 0x04000BE5 RID: 3045
	[SerializeField]
	private float maxLifetime;

	// Token: 0x04000BE6 RID: 3046
	[SerializeField]
	private SoundBankPlayer clickSound;

	// Token: 0x04000BE7 RID: 3047
	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	// Token: 0x04000BE8 RID: 3048
	[SerializeField]
	private float crankHapticStrength = 0.1f;

	// Token: 0x04000BE9 RID: 3049
	[SerializeField]
	private float crankHapticDuration = 0.05f;

	// Token: 0x04000BEA RID: 3050
	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	// Token: 0x04000BEB RID: 3051
	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	// Token: 0x04000BEC RID: 3052
	private float currentCrankStrength;

	// Token: 0x04000BED RID: 3053
	private float currentCrankClickAmount;

	// Token: 0x04000BEE RID: 3054
	private RubberDuckEvents _events;
}
