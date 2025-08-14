using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001C6 RID: 454
public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000B49 RID: 2889 RVA: 0x0003BF18 File Offset: 0x0003A118
	private void OnEnable()
	{
		if (this.myRig.IsNull())
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this._events == null && this.myRig != null && this.myRig.Creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(this.myRig.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnActivate;
		}
		bool flag = !PhotonNetwork.InRoom && this.myRig != null && this.myRig.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && this.myRig != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == this.myRig;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0003C040 File Offset: 0x0003A240
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
		if (this._events != null)
		{
			this._events.Activate -= this.OnActivate;
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0003C0B9 File Offset: 0x0003A2B9
	private void OnActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnJumpActivate");
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0003C0F4 File Offset: 0x0003A2F4
	public void Tick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = (instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false));
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.RigidbodyVelocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.RigidbodyVelocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0003C200 File Offset: 0x0003A400
	// (set) Token: 0x06000B4E RID: 2894 RVA: 0x0003C208 File Offset: 0x0003A408
	public bool TickRunning { get; set; }

	// Token: 0x04000DDE RID: 3550
	[SerializeField]
	private float minJumpStrength = 1f;

	// Token: 0x04000DDF RID: 3551
	[SerializeField]
	private float minJumpVertical = 1f;

	// Token: 0x04000DE0 RID: 3552
	[SerializeField]
	private float cooldownTime = 1f;

	// Token: 0x04000DE1 RID: 3553
	[SerializeField]
	private UnityEvent onJumping;

	// Token: 0x04000DE2 RID: 3554
	private RubberDuckEvents _events;

	// Token: 0x04000DE3 RID: 3555
	private bool playerOnGround;

	// Token: 0x04000DE4 RID: 3556
	private float minJumpTime = 0.05f;

	// Token: 0x04000DE5 RID: 3557
	private bool waitingForGrounding;

	// Token: 0x04000DE6 RID: 3558
	private float jumpStartTime;

	// Token: 0x04000DE7 RID: 3559
	private float lastActivationTime;

	// Token: 0x04000DE8 RID: 3560
	private VRRig myRig;
}
