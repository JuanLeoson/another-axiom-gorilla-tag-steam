using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000188 RID: 392
public class DeployableObject : TransferrableObject
{
	// Token: 0x060009F3 RID: 2547 RVA: 0x000369AF File Offset: 0x00034BAF
	protected override void Awake()
	{
		this._deploySignal.OnSignal += this.DeployRPC;
		base.Awake();
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x000369D0 File Offset: 0x00034BD0
	internal override void OnEnable()
	{
		this._deploySignal.Enable();
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		for (int i = 0; i < this._rigAwareObjects.Length; i++)
		{
			IRigAware rigAware = this._rigAwareObjects[i] as IRigAware;
			if (rigAware != null)
			{
				rigAware.SetRig(componentInParent);
			}
		}
		this.m_VRRig = componentInParent;
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00036A44 File Offset: 0x00034C44
	internal override void OnDisable()
	{
		this.m_VRRig = null;
		this._deploySignal.Disable();
		if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
		base.OnDisable();
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00036A71 File Offset: 0x00034C71
	protected override void OnDestroy()
	{
		this._deploySignal.Dispose();
		base.OnDestroy();
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00036A84 File Offset: 0x00034C84
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this._objectToDeploy.activeSelf)
			{
				this.DeployChild();
				return;
			}
		}
		else if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00036AD8 File Offset: 0x00034CD8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRig.LocalRig != this.ownerRig)
		{
			return false;
		}
		bool isRightHand = releasingHand == EquipmentInteractor.instance.rightHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isRightHand);
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(Vector3.zero);
		Quaternion rotation = transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		this.DeployLocal(vector, rotation, averageVelocity, false);
		this._deploySignal.Raise(ReceiverGroup.Others, BitPackUtils.PackWorldPosForNetwork(vector), BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f));
		return true;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00036B7B File Offset: 0x00034D7B
	protected virtual void DeployLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this.DisableWhileDeployed(true);
		this._child.Deploy(this, launchPos, launchRot, releaseVel, isRemote);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00036B98 File Offset: 0x00034D98
	private void DeployRPC(long packedPos, int packedRot, long packedVel, PhotonSignalInfo info)
	{
		if (info.sender != base.OwningPlayer())
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "DeployRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork(packedRot);
		Vector3 inVel = BitPackUtils.UnpackWorldPosFromNetwork(packedVel) / 100f;
		float num = 10000f;
		if (!vector.IsValid(num) || !launchRot.IsValid() || !this.m_VRRig.IsPositionInRange(vector, this._maxDeployDistance))
		{
			return;
		}
		this.DeployLocal(vector, launchRot, this.m_VRRig.ClampVelocityRelativeToPlayerSafe(inVel, this._maxThrowVelocity), true);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00036C30 File Offset: 0x00034E30
	private void DisableWhileDeployed(bool active)
	{
		if (this._disabledWhileDeployed.IsNullOrEmpty<GameObject>())
		{
			return;
		}
		for (int i = 0; i < this._disabledWhileDeployed.Length; i++)
		{
			this._disabledWhileDeployed[i].SetActive(!active);
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00036C6F File Offset: 0x00034E6F
	public void DeployChild()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this._objectToDeploy.SetActive(true);
		this.DisableWhileDeployed(true);
		UnityEvent onDeploy = this._onDeploy;
		if (onDeploy == null)
		{
			return;
		}
		onDeploy.Invoke();
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00036CA2 File Offset: 0x00034EA2
	public void ReturnChild()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this._objectToDeploy.SetActive(false);
		this.DisableWhileDeployed(false);
		UnityEvent onReturn = this._onReturn;
		if (onReturn == null)
		{
			return;
		}
		onReturn.Invoke();
	}

	// Token: 0x04000C00 RID: 3072
	[SerializeField]
	private GameObject _objectToDeploy;

	// Token: 0x04000C01 RID: 3073
	[SerializeField]
	private DeployedChild _child;

	// Token: 0x04000C02 RID: 3074
	[SerializeField]
	private GameObject[] _disabledWhileDeployed = new GameObject[0];

	// Token: 0x04000C03 RID: 3075
	[SerializeField]
	private SoundBankPlayer deploySound;

	// Token: 0x04000C04 RID: 3076
	[SerializeField]
	private PhotonSignal<long, int, long> _deploySignal = "_deploySignal";

	// Token: 0x04000C05 RID: 3077
	[SerializeField]
	private float _maxDeployDistance = 4f;

	// Token: 0x04000C06 RID: 3078
	[SerializeField]
	private float _maxThrowVelocity = 50f;

	// Token: 0x04000C07 RID: 3079
	[SerializeField]
	private UnityEvent _onDeploy;

	// Token: 0x04000C08 RID: 3080
	[SerializeField]
	private UnityEvent _onReturn;

	// Token: 0x04000C09 RID: 3081
	[SerializeField]
	private Component[] _rigAwareObjects = new Component[0];

	// Token: 0x04000C0A RID: 3082
	private VRRig m_VRRig;
}
