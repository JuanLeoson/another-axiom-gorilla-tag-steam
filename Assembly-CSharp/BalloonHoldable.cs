using System;
using GorillaExtensions;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class BalloonHoldable : TransferrableObject, IFXContext
{
	// Token: 0x0600168D RID: 5773 RVA: 0x0007ACCC File Offset: 0x00078ECC
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.balloonDynamics = base.GetComponent<ITetheredObjectBehavior>();
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<Renderer>();
		}
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x0007AD25 File Offset: 0x00078F25
	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x0007AD38 File Offset: 0x00078F38
	internal override void OnEnable()
	{
		base.OnEnable();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = true;
		this.lineRenderer.enabled = false;
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.worldShareableInstance != null)
			{
				return;
			}
			base.SpawnTransferableObjectViews();
		}
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
		}
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x0007ADAA File Offset: 0x00078FAA
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.worldShareableInstance != null)
		{
			return;
		}
		base.SpawnTransferableObjectViews();
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x0007ADC7 File Offset: 0x00078FC7
	private bool ShouldSimulate()
	{
		return !base.Attached() && this.balloonState == BalloonHoldable.BalloonStates.Normal;
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x0007ADDC File Offset: 0x00078FDC
	internal override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x0007ADF9 File Offset: 0x00078FF9
	public override void PreDisable()
	{
		this.originalOwner = null;
		base.PreDisable();
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x0007AE08 File Offset: 0x00079008
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.balloonState = BalloonHoldable.BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x0007AE28 File Offset: 0x00079028
	protected override void OnWorldShareableItemSpawn()
	{
		WorldShareableItem worldShareableInstance = this.worldShareableInstance;
		if (worldShareableInstance != null)
		{
			worldShareableInstance.rpcCallBack = new Action(this.PopBalloonRemote);
			worldShareableInstance.onOwnerChangeCb = new WorldShareableItem.OnOwnerChangeDelegate(this.OnOwnerChangeCb);
			worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		this.originalOwner = worldShareableInstance.target.owner;
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x0007AE88 File Offset: 0x00079088
	public override void ResetToHome()
	{
		if (base.IsLocalObject() && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		this.PopBalloon();
		this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
		base.ResetToHome();
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x0007AF06 File Offset: 0x00079106
	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		this.PlayPopBalloonFX();
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x0007AF14 File Offset: 0x00079114
	protected override void OnItemDestroyedOrDisabled()
	{
		this.PlayPopBalloonFX();
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
		}
		base.transform.parent = base.DefaultAnchor();
		base.OnItemDestroyedOrDisabled();
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x0007AF48 File Offset: 0x00079148
	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this, default(PhotonMessageInfoWrapped));
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x0007AF68 File Offset: 0x00079168
	private void EnableDynamics(bool enable, bool collider, bool forceKinematicOn = false)
	{
		bool kinematic = false;
		if (forceKinematicOn)
		{
			kinematic = true;
		}
		else if (NetworkSystem.Instance.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				kinematic = true;
			}
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(enable, collider, kinematic);
		}
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x0007AFD4 File Offset: 0x000791D4
	private void PopBalloon()
	{
		this.PlayPopBalloonFX();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = false;
		this.lineRenderer.enabled = false;
		if (this.gripInteractor != null)
		{
			this.gripInteractor.gameObject.SetActive(false);
		}
		if ((object.Equals(this.originalOwner, PhotonNetwork.LocalPlayer) || !NetworkSystem.Instance.InRoom) && NetworkSystem.Instance.InRoom && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
			this.EnableDynamics(false, false, false);
		}
		if (this.IsMyItem())
		{
			if (base.InLeftHand())
			{
				EquipmentInteractor.instance.ReleaseLeftHand();
			}
			if (base.InRightHand())
			{
				EquipmentInteractor.instance.ReleaseRightHand();
			}
		}
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0007B0CD File Offset: 0x000792CD
	public void PopBalloonRemote()
	{
		if (this.ShouldSimulate())
		{
			this.balloonState = BalloonHoldable.BalloonStates.Pop;
		}
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnOwnerChangeCb(NetPlayer newOwner, NetPlayer prevOwner)
	{
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0007B0E0 File Offset: 0x000792E0
	public override void OnOwnershipTransferred(NetPlayer newOwner, NetPlayer prevOwner)
	{
		base.OnOwnershipTransferred(newOwner, prevOwner);
		if (object.Equals(prevOwner, NetworkSystem.Instance.LocalPlayer) && newOwner == null)
		{
			return;
		}
		if (!object.Equals(newOwner, NetworkSystem.Instance.LocalPlayer))
		{
			this.EnableDynamics(false, true, true);
			return;
		}
		if (this.ShouldSimulate() && this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(true, true, false);
		}
		if (!this.rb)
		{
			return;
		}
		if (!this.rb.isKinematic)
		{
			this.rb.AddForceAtPosition(this.forceAppliedAsRemote, this.collisionPtAsRemote, ForceMode.VelocityChange);
		}
		this.forceAppliedAsRemote = Vector3.zero;
		this.collisionPtAsRemote = Vector3.zero;
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x0007B190 File Offset: 0x00079390
	private void OwnerPopBalloon()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
		}
		this.balloonState = BalloonHoldable.BalloonStates.Pop;
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x0007B1D8 File Offset: 0x000793D8
	private void RunLocalPopSM()
	{
		switch (this.balloonState)
		{
		case BalloonHoldable.BalloonStates.Normal:
			break;
		case BalloonHoldable.BalloonStates.Pop:
			this.timer = Time.time;
			this.PopBalloon();
			this.balloonState = BalloonHoldable.BalloonStates.WaitForOwnershipTransfer;
			this.lastOwnershipRequest = Time.time;
			return;
		case BalloonHoldable.BalloonStates.Waiting:
			if (Time.time - this.timer >= this.poppedTimerLength)
			{
				this.timer = Time.time;
				this.mesh.enabled = true;
				this.balloonInflatSource.GTPlay();
				this.balloonState = BalloonHoldable.BalloonStates.Refilling;
				return;
			}
			base.transform.localScale = new Vector3(this.beginScale, this.beginScale, this.beginScale);
			return;
		case BalloonHoldable.BalloonStates.WaitForOwnershipTransfer:
			if (!NetworkSystem.Instance.InRoom)
			{
				this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
				base.ReDock();
				return;
			}
			if (this.worldShareableInstance != null)
			{
				WorldShareableItem worldShareableInstance = this.worldShareableInstance;
				NetPlayer owner = worldShareableInstance.Owner;
				if (worldShareableInstance != null && owner == this.originalOwner)
				{
					this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
					base.ReDock();
				}
				if (base.IsLocalObject() && this.lastOwnershipRequest + 5f < Time.time)
				{
					this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
					this.lastOwnershipRequest = Time.time;
					return;
				}
			}
			break;
		case BalloonHoldable.BalloonStates.WaitForReDock:
			if (base.Attached())
			{
				this.fullyInflatedScale = base.transform.localScale;
				base.ReDock();
				this.balloonState = BalloonHoldable.BalloonStates.Waiting;
				return;
			}
			break;
		case BalloonHoldable.BalloonStates.Refilling:
		{
			float num = Time.time - this.timer;
			if (num >= this.scaleTimerLength)
			{
				base.transform.localScale = this.fullyInflatedScale;
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				if (this.gripInteractor != null)
				{
					this.gripInteractor.gameObject.SetActive(true);
				}
			}
			num = Mathf.Clamp01(num / this.scaleTimerLength);
			float d = Mathf.Lerp(this.beginScale, 1f, num);
			base.transform.localScale = this.fullyInflatedScale * d;
			return;
		}
		case BalloonHoldable.BalloonStates.Returning:
			if (this.balloonDynamics.ReturnStep())
			{
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				base.ReDock();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x0007B3F0 File Offset: 0x000795F0
	protected override void OnStateChanged()
	{
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
			return;
		}
		if (base.OnShoulder())
		{
			if (this.balloonDynamics != null && this.balloonDynamics.IsEnabled())
			{
				this.EnableDynamics(false, false, false);
			}
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x0007B450 File Offset: 0x00079650
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (Time.frameCount == this.enabledOnFrame)
		{
			this.OnStateChanged();
		}
		if (base.InHand() && this.detatchOnGrab)
		{
			float d = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
			base.transform.localScale = Vector3.one * d;
		}
		if (base.Dropped() && this.balloonState == BalloonHoldable.BalloonStates.Normal && this.maxDistanceFromOwner > 0f && (!NetworkSystem.Instance.InRoom || this.originalOwner.IsLocal) && (VRRig.LocalRig.transform.position - base.transform.position).IsLongerThan(this.maxDistanceFromOwner * base.transform.localScale.x))
		{
			this.OwnerPopBalloon();
		}
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		if (this.balloonState != BalloonHoldable.BalloonStates.Normal)
		{
			this.RunLocalPopSM();
		}
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x0007B579 File Offset: 0x00079779
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x0007B584 File Offset: 0x00079784
	private void Grab()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		if (this.detatchOnGrab)
		{
			float num = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
			base.transform.localScale = Vector3.one * num;
			this.EnableDynamics(true, true, false);
			this.balloonDynamics.EnableDistanceConstraints(true, num);
			this.lineRenderer.enabled = true;
			return;
		}
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x0007B618 File Offset: 0x00079818
	private void Release()
	{
		if (this.disableRelease)
		{
			this.balloonState = BalloonHoldable.BalloonStates.Returning;
			return;
		}
		if (this.balloonDynamics == null)
		{
			return;
		}
		float num = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
		base.transform.localScale = Vector3.one * num;
		this.EnableDynamics(true, true, false);
		this.balloonDynamics.EnableDistanceConstraints(false, num);
		this.lineRenderer.enabled = false;
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x0007B6A4 File Offset: 0x000798A4
	public void OnTriggerEnter(Collider other)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag = false;
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.TriggerEnter(other, ref zero, ref zero2, ref flag);
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (flag)
		{
			RequestableOwnershipGuard component = PhotonView.Get(this.worldShareableInstance.gameObject).GetComponent<RequestableOwnershipGuard>();
			if (!component.isTrulyMine)
			{
				if (zero.magnitude > this.forceAppliedAsRemote.magnitude)
				{
					this.forceAppliedAsRemote = zero;
					this.collisionPtAsRemote = zero2;
				}
				component.RequestOwnershipImmediately(delegate
				{
				});
			}
		}
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x0007B768 File Offset: 0x00079968
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.ShouldSimulate() || this.disableCollisionHandling)
		{
			return;
		}
		this.balloonBopSource.GTPlay();
		if (!collision.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OwnerPopBalloon();
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (PhotonView.Get(this.worldShareableInstance.gameObject).IsMine)
		{
			this.OwnerPopBalloon();
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x060016A8 RID: 5800 RVA: 0x0007B7E0 File Offset: 0x000799E0
	FXSystemSettings IFXContext.settings
	{
		get
		{
			return this.ownerRig.fxSettings;
		}
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x0007B7F0 File Offset: 0x000799F0
	void IFXContext.OnPlayFX()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.balloonPopFXColor);
		}
	}

	// Token: 0x04001E5A RID: 7770
	private ITetheredObjectBehavior balloonDynamics;

	// Token: 0x04001E5B RID: 7771
	[SerializeField]
	private Renderer mesh;

	// Token: 0x04001E5C RID: 7772
	private LineRenderer lineRenderer;

	// Token: 0x04001E5D RID: 7773
	private Rigidbody rb;

	// Token: 0x04001E5E RID: 7774
	private NetPlayer originalOwner;

	// Token: 0x04001E5F RID: 7775
	public GameObject balloonPopFXPrefab;

	// Token: 0x04001E60 RID: 7776
	public Color balloonPopFXColor;

	// Token: 0x04001E61 RID: 7777
	private float timer;

	// Token: 0x04001E62 RID: 7778
	public float scaleTimerLength = 2f;

	// Token: 0x04001E63 RID: 7779
	public float poppedTimerLength = 2.5f;

	// Token: 0x04001E64 RID: 7780
	public float beginScale = 0.1f;

	// Token: 0x04001E65 RID: 7781
	public float bopSpeed = 1f;

	// Token: 0x04001E66 RID: 7782
	private Vector3 fullyInflatedScale;

	// Token: 0x04001E67 RID: 7783
	public AudioSource balloonBopSource;

	// Token: 0x04001E68 RID: 7784
	public AudioSource balloonInflatSource;

	// Token: 0x04001E69 RID: 7785
	private Vector3 forceAppliedAsRemote;

	// Token: 0x04001E6A RID: 7786
	private Vector3 collisionPtAsRemote;

	// Token: 0x04001E6B RID: 7787
	private WaterVolume waterVolume;

	// Token: 0x04001E6C RID: 7788
	[DebugReadout]
	private BalloonHoldable.BalloonStates balloonState;

	// Token: 0x04001E6D RID: 7789
	private float returnTimer;

	// Token: 0x04001E6E RID: 7790
	[SerializeField]
	private float maxDistanceFromOwner;

	// Token: 0x04001E6F RID: 7791
	public float lastOwnershipRequest;

	// Token: 0x04001E70 RID: 7792
	[SerializeField]
	private bool disableCollisionHandling;

	// Token: 0x04001E71 RID: 7793
	[SerializeField]
	private bool disableRelease;

	// Token: 0x020003CB RID: 971
	private enum BalloonStates
	{
		// Token: 0x04001E73 RID: 7795
		Normal,
		// Token: 0x04001E74 RID: 7796
		Pop,
		// Token: 0x04001E75 RID: 7797
		Waiting,
		// Token: 0x04001E76 RID: 7798
		WaitForOwnershipTransfer,
		// Token: 0x04001E77 RID: 7799
		WaitForReDock,
		// Token: 0x04001E78 RID: 7800
		Refilling,
		// Token: 0x04001E79 RID: 7801
		Returning
	}
}
