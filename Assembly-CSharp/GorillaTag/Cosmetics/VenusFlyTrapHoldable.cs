using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F69 RID: 3945
	[RequireComponent(typeof(TransferrableObject))]
	public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x060061A8 RID: 25000 RVA: 0x001F0ABC File Offset: 0x001EECBC
		// (set) Token: 0x060061A9 RID: 25001 RVA: 0x001F0AC4 File Offset: 0x001EECC4
		public bool TickRunning { get; set; }

		// Token: 0x060061AA RID: 25002 RVA: 0x001F0ACD File Offset: 0x001EECCD
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x001F0ADC File Offset: 0x001EECDC
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent += this.TriggerEntered;
			this.state = VenusFlyTrapHoldable.VenusState.Open;
			this.localRotA = this.lipA.transform.localRotation;
			this.localRotB = this.lipB.transform.localRotation;
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnTriggerEvent;
			}
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001F0BF4 File Offset: 0x001EEDF4
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent -= this.TriggerEntered;
			if (this._events != null)
			{
				this._events.Activate -= this.OnTriggerEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001F0C60 File Offset: 0x001EEE60
		public void Tick()
		{
			if (this.transferrableObject.InHand() && this.audioSource && !this.audioSource.isPlaying && this.flyLoopingAudio != null)
			{
				this.audioSource.clip = this.flyLoopingAudio;
				this.audioSource.GTPlay();
			}
			if (!this.transferrableObject.InHand() && this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed && Time.time - this.closedStartedTime >= this.closedDuration)
			{
				this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
				if (this.audioSource && this.openingAudio != null)
				{
					this.audioSource.GTPlayOneShot(this.openingAudio, 1f);
				}
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
			{
				this.SmoothRotation(true);
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Opening)
			{
				this.SmoothRotation(false);
			}
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x001F0D70 File Offset: 0x001EEF70
		private void SmoothRotation(bool isClosing)
		{
			if (isClosing)
			{
				Quaternion quaternion = Quaternion.Euler(this.targetRotationB);
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, quaternion, Time.deltaTime * this.speed);
				Quaternion quaternion2 = Quaternion.Euler(this.targetRotationA);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, quaternion2, Time.deltaTime * this.speed);
				if (Quaternion.Angle(this.lipB.transform.localRotation, quaternion) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, quaternion2) < 1f)
				{
					this.lipB.transform.localRotation = quaternion;
					this.lipA.transform.localRotation = quaternion2;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
					return;
				}
			}
			else
			{
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, Time.deltaTime * this.speed / 2f);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, Time.deltaTime * this.speed / 2f);
				if (Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) < 1f)
				{
					this.lipB.transform.localRotation = this.localRotB;
					this.lipA.transform.localRotation = this.localRotA;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
				}
			}
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x001F0F5A File Offset: 0x001EF15A
		private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
		{
			this.state = newState;
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed)
			{
				this.closedStartedTime = Time.time;
			}
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001F0F78 File Offset: 0x001EF178
		private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
		{
			if (this.state != VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(this.layers))
			{
				return;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(Array.Empty<object>());
			}
			this.OnTriggerLocal();
			GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
			if (componentInChildren == null)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060061B1 RID: 25009 RVA: 0x001F1013 File Offset: 0x001EF213
		private void OnTriggerEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnTriggerEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.OnTriggerLocal();
		}

		// Token: 0x060061B2 RID: 25010 RVA: 0x001F103F File Offset: 0x001EF23F
		private void OnTriggerLocal()
		{
			this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
			if (this.audioSource && this.closingAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.closingAudio, 1f);
			}
		}

		// Token: 0x04006DE1 RID: 28129
		[SerializeField]
		private GameObject lipA;

		// Token: 0x04006DE2 RID: 28130
		[SerializeField]
		private GameObject lipB;

		// Token: 0x04006DE3 RID: 28131
		[SerializeField]
		private Vector3 targetRotationA;

		// Token: 0x04006DE4 RID: 28132
		[SerializeField]
		private Vector3 targetRotationB;

		// Token: 0x04006DE5 RID: 28133
		[SerializeField]
		private float closedDuration = 3f;

		// Token: 0x04006DE6 RID: 28134
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04006DE7 RID: 28135
		[SerializeField]
		private UnityLayer layers;

		// Token: 0x04006DE8 RID: 28136
		[SerializeField]
		private TriggerEventNotifier triggerEventNotifier;

		// Token: 0x04006DE9 RID: 28137
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04006DEA RID: 28138
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04006DEB RID: 28139
		[SerializeField]
		private GameObject bug;

		// Token: 0x04006DEC RID: 28140
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006DED RID: 28141
		[SerializeField]
		private AudioClip closingAudio;

		// Token: 0x04006DEE RID: 28142
		[SerializeField]
		private AudioClip openingAudio;

		// Token: 0x04006DEF RID: 28143
		[SerializeField]
		private AudioClip flyLoopingAudio;

		// Token: 0x04006DF0 RID: 28144
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04006DF1 RID: 28145
		private float closedStartedTime;

		// Token: 0x04006DF2 RID: 28146
		private VenusFlyTrapHoldable.VenusState state;

		// Token: 0x04006DF3 RID: 28147
		private Quaternion localRotA;

		// Token: 0x04006DF4 RID: 28148
		private Quaternion localRotB;

		// Token: 0x04006DF5 RID: 28149
		private RubberDuckEvents _events;

		// Token: 0x04006DF6 RID: 28150
		private TransferrableObject transferrableObject;

		// Token: 0x02000F6A RID: 3946
		private enum VenusState
		{
			// Token: 0x04006DF9 RID: 28153
			Closed,
			// Token: 0x04006DFA RID: 28154
			Open,
			// Token: 0x04006DFB RID: 28155
			Closing,
			// Token: 0x04006DFC RID: 28156
			Opening
		}
	}
}
