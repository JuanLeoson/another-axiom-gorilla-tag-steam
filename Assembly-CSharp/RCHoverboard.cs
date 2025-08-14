using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag.Cosmetics;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000112 RID: 274
public class RCHoverboard : RCVehicle
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x00027813 File Offset: 0x00025A13
	// (set) Token: 0x060006F7 RID: 1783 RVA: 0x0002781B File Offset: 0x00025A1B
	private float _MaxForwardSpeed
	{
		get
		{
			return this.m_maxForwardSpeed;
		}
		set
		{
			this.m_maxForwardSpeed = value;
			this._forwardAccel = value / math.max(0.01f, this.m_forwardAccelTime);
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060006F8 RID: 1784 RVA: 0x0002783C File Offset: 0x00025A3C
	// (set) Token: 0x060006F9 RID: 1785 RVA: 0x00027844 File Offset: 0x00025A44
	private float _MaxTurnRate
	{
		get
		{
			return this.m_maxTurnRate;
		}
		set
		{
			this.m_maxTurnRate = value;
			this._turnAccel = value / math.max(1E-06f, this.m_turnAccelTime);
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060006FA RID: 1786 RVA: 0x00027865 File Offset: 0x00025A65
	// (set) Token: 0x060006FB RID: 1787 RVA: 0x0002786D File Offset: 0x00025A6D
	private float _MaxTiltAngle
	{
		get
		{
			return this.m_maxTiltAngle;
		}
		set
		{
			this.m_maxTiltAngle = value;
			this._tiltAccel = value / math.max(1E-06f, this.m_tiltTime);
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00027890 File Offset: 0x00025A90
	protected override void Awake()
	{
		base.Awake();
		this._hasAudioSource = (this.m_audioSource != null);
		this._hasHoverSound = (this.m_hoverSound != null);
		this._MaxForwardSpeed = this.m_maxForwardSpeed;
		this._MaxTurnRate = this.m_maxTurnRate;
		this._MaxTiltAngle = this.m_maxTiltAngle;
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x000278EC File Offset: 0x00025AEC
	protected override void AuthorityBeginDocked()
	{
		base.AuthorityBeginDocked();
		this._currentTurnRate = 0f;
		this._currentTiltAngle = 0f;
		float3 to = this._ProjectOnPlane(base.transform.forward, math.up());
		this._currentTurnAngle = this._SignedAngle(new float3(0f, 0f, 1f), to, new float3(0f, 1f, 0f));
		this._motorLevel = 0f;
		if (this._hasAudioSource)
		{
			this.m_audioSource.Stop();
			this.m_audioSource.volume = 0f;
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00027994 File Offset: 0x00025B94
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized)
		{
			float x = math.length(this.activeInput.joystick);
			this._motorLevel = math.saturate(x);
			if (this.hasNetworkSync)
			{
				this.networkSync.syncedState.dataA = (byte)((uint)(this._motorLevel * 255f));
				return;
			}
		}
		else
		{
			this._motorLevel = 0f;
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00027A08 File Offset: 0x00025C08
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized && this.hasNetworkSync)
		{
			this._motorLevel = (float)this.networkSync.syncedState.dataA / 255f;
			return;
		}
		this._motorLevel = 0f;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00027A58 File Offset: 0x00025C58
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		switch (this.localState)
		{
		case RCVehicle.State.Disabled:
		case RCVehicle.State.DockedLeft:
		case RCVehicle.State.DockedRight:
		case RCVehicle.State.Crashed:
			break;
		case RCVehicle.State.Mobilized:
			if (this._hasAudioSource && this._hasHoverSound)
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.m_audioSource.volume = 0f;
					this.m_audioSource.clip = this.m_hoverSound;
					this.m_audioSource.loop = true;
					this.m_audioSource.GTPlay();
					return;
				}
				float target = math.lerp(this.m_hoverSoundVolumeMinMax.x, this.m_hoverSoundVolumeMinMax.y, this._motorLevel);
				float maxDelta = this.m_hoverSoundVolumeMinMax.y / this.m_hoverSoundVolumeRampTime * dt;
				this.m_audioSource.volume = this._MoveTowards(this.m_audioSource.volume, target, maxDelta);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00027B3C File Offset: 0x00025D3C
	protected void FixedUpdate()
	{
		if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = this.m_inputThrustForward.Get(this.activeInput) - this.m_inputThrustBack.Get(this.activeInput);
		float num2 = this.m_inputTurn.Get(this.activeInput);
		float num3 = this.m_inputJump.Get(this.activeInput);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, 134218241, QueryTriggerInteraction.Ignore);
		bool flag2 = flag && raycastHit.distance <= this.m_hoverHeight + 0.1f;
		if (num3 > 0.001f && flag2 && !this._hasJumped)
		{
			this.rb.AddForce(Vector3.up * this.m_jumpForce, ForceMode.Impulse);
			this._hasJumped = true;
		}
		else if (num3 <= 0.001f)
		{
			this._hasJumped = false;
		}
		float target = num2 * this._MaxTurnRate;
		this._currentTurnRate = this._MoveTowards(this._currentTurnRate, target, this._turnAccel * fixedDeltaTime);
		this._currentTurnAngle += this._currentTurnRate * fixedDeltaTime;
		float target2 = math.lerp(-this.m_maxTiltAngle, this.m_maxTiltAngle, math.unlerp(-1f, 1f, num));
		this._currentTiltAngle = this._MoveTowards(this._currentTiltAngle, target2, this._tiltAccel * fixedDeltaTime);
		base.transform.rotation = quaternion.EulerXYZ(math.radians(new float3(this._currentTiltAngle, this._currentTurnAngle, 0f)));
		float3 @float = base.transform.forward;
		float num4 = math.dot(@float, this.rb.velocity);
		float num5 = num * this.m_maxForwardSpeed;
		float rhs = (math.abs(num5) > 0.001f && ((num5 > 0f && num4 < num5) || (num5 < 0f && num4 > num5))) ? math.sign(num5) : 0f;
		this.rb.AddForce(@float * this._forwardAccel * rhs, ForceMode.Acceleration);
		if (flag)
		{
			float num6 = math.saturate(this.m_hoverHeight - raycastHit.distance);
			float num7 = math.dot(this.rb.velocity, Vector3.up);
			float rhs2 = num6 * this.m_hoverForce - num7 * this.m_hoverDamp;
			this.rb.AddForce(math.up() * rhs2, ForceMode.Force);
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00027DE8 File Offset: 0x00025FE8
	protected void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.collider.gameObject;
		bool flag = gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
		bool flag2 = gameObject.IsOnLayer(UnityLayer.GorillaHand);
		if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
		{
			Vector3 vector = Vector3.zero;
			if (flag2)
			{
				GorillaHandClimber component = gameObject.GetComponent<GorillaHandClimber>();
				if (component != null)
				{
					vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
				}
			}
			else if (collision.rigidbody != null)
			{
				vector = collision.rigidbody.velocity;
			}
			if (flag || vector.sqrMagnitude > 0.01f)
			{
				if (base.HasLocalAuthority)
				{
					this.AuthorityApplyImpact(vector, flag);
					return;
				}
				if (this.networkSync != null)
				{
					this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[]
					{
						vector,
						flag
					});
				}
			}
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00027EE9 File Offset: 0x000260E9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _MoveTowards(float current, float target, float maxDelta)
	{
		if (math.abs(target - current) > maxDelta)
		{
			return current + math.sign(target - current) * maxDelta;
		}
		return target;
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00027F04 File Offset: 0x00026104
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _SignedAngle(float3 from, float3 to, float3 axis)
	{
		float3 x = math.normalize(from);
		float3 y = math.normalize(to);
		float x2 = math.acos(math.dot(x, y));
		float num = math.sign(math.dot(math.cross(x, y), axis));
		return math.degrees(x2) * num;
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00027F45 File Offset: 0x00026145
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float3 _ProjectOnPlane(float3 vector, float3 planeNormal)
	{
		return vector - math.dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x04000857 RID: 2135
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputTurn = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickX, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.1f, 0f, 0f, 1.25f, 0f, 0f),
		new Keyframe(0.9f, 1f, 1.25f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	}));

	// Token: 0x04000858 RID: 2136
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustForward = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.Trigger, AnimationCurves.EaseInCirc);

	// Token: 0x04000859 RID: 2137
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustBack = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickBack, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.9f, 0f, 0f, 9.9999f, 0.5825f, 0.3767f),
		new Keyframe(1f, 1f, 9.9999f, 1f, 0f, 0f)
	}));

	// Token: 0x0400085A RID: 2138
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputJump = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.PrimaryFaceButton, AnimationCurves.Linear);

	// Token: 0x0400085B RID: 2139
	[Tooltip("Desired hover height above ground from this transform's position.")]
	[SerializeField]
	private float m_hoverHeight = 0.2f;

	// Token: 0x0400085C RID: 2140
	[Tooltip("Upward force to maintain hover when below hoverHeight.")]
	[SerializeField]
	private float m_hoverForce = 200f;

	// Token: 0x0400085D RID: 2141
	[Tooltip("Damping factor to smooth out vertical movement.")]
	[SerializeField]
	private float m_hoverDamp = 5f;

	// Token: 0x0400085E RID: 2142
	[Tooltip("Upward impulse force for jump.")]
	[SerializeField]
	private float m_jumpForce = 3.5f;

	// Token: 0x0400085F RID: 2143
	private bool _hasJumped;

	// Token: 0x04000860 RID: 2144
	[SerializeField]
	[HideInInspector]
	private float m_maxForwardSpeed = 6f;

	// Token: 0x04000861 RID: 2145
	[SerializeField]
	[Tooltip("Time (seconds) to reach max forward speed from zero.")]
	private float m_forwardAccelTime = 2f;

	// Token: 0x04000862 RID: 2146
	[SerializeField]
	[HideInInspector]
	private float m_maxTurnRate = 720f;

	// Token: 0x04000863 RID: 2147
	[Tooltip("Time (seconds) to reach max turning rate.")]
	[SerializeField]
	private float m_turnAccelTime = 0.75f;

	// Token: 0x04000864 RID: 2148
	[SerializeField]
	[HideInInspector]
	private float m_maxTiltAngle = 30f;

	// Token: 0x04000865 RID: 2149
	[Tooltip("Time (seconds) to reach max tilt angle.")]
	[SerializeField]
	private float m_tiltTime = 0.1f;

	// Token: 0x04000866 RID: 2150
	[Tooltip("Audio source for any motor or hover sound.")]
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000867 RID: 2151
	[Tooltip("Looping motor/hover sound clip.")]
	[SerializeField]
	private AudioClip m_hoverSound;

	// Token: 0x04000868 RID: 2152
	[Tooltip("Volume range for the hover sound (x = min, y = max).")]
	[SerializeField]
	private float2 m_hoverSoundVolumeMinMax = new float2(0.1f, 0.5f);

	// Token: 0x04000869 RID: 2153
	[Tooltip("Time it takes for the volume to reach max value.")]
	[SerializeField]
	private float m_hoverSoundVolumeRampTime = 1f;

	// Token: 0x0400086A RID: 2154
	private bool _hasAudioSource;

	// Token: 0x0400086B RID: 2155
	private bool _hasHoverSound;

	// Token: 0x0400086C RID: 2156
	private float _forwardAccel;

	// Token: 0x0400086D RID: 2157
	private float _turnAccel;

	// Token: 0x0400086E RID: 2158
	private float _tiltAccel;

	// Token: 0x0400086F RID: 2159
	private float _currentTurnRate;

	// Token: 0x04000870 RID: 2160
	private float _currentTurnAngle;

	// Token: 0x04000871 RID: 2161
	private float _currentTiltAngle;

	// Token: 0x04000872 RID: 2162
	private float _motorLevel;

	// Token: 0x02000113 RID: 275
	private enum _EInputSource
	{
		// Token: 0x04000874 RID: 2164
		None,
		// Token: 0x04000875 RID: 2165
		StickX,
		// Token: 0x04000876 RID: 2166
		StickForward,
		// Token: 0x04000877 RID: 2167
		StickBack,
		// Token: 0x04000878 RID: 2168
		Trigger,
		// Token: 0x04000879 RID: 2169
		PrimaryFaceButton
	}

	// Token: 0x02000114 RID: 276
	[Serializable]
	private struct _SingleInputOption
	{
		// Token: 0x06000707 RID: 1799 RVA: 0x00028173 File Offset: 0x00026373
		public _SingleInputOption(RCHoverboard._EInputSource source, AnimationCurve remapCurve)
		{
			this.source = new GTOption<StringEnum<RCHoverboard._EInputSource>>(source);
			this.remapCurve = new GTOption<AnimationCurve>(remapCurve);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00028194 File Offset: 0x00026394
		public float Get(RCRemoteHoldable.RCInput input)
		{
			float num;
			switch (this.source.ResolvedValue.Value)
			{
			case RCHoverboard._EInputSource.None:
				num = 0f;
				break;
			case RCHoverboard._EInputSource.StickX:
				num = input.joystick.x;
				break;
			case RCHoverboard._EInputSource.StickForward:
				num = math.saturate(input.joystick.y);
				break;
			case RCHoverboard._EInputSource.StickBack:
				num = math.saturate(-input.joystick.y);
				break;
			case RCHoverboard._EInputSource.Trigger:
				num = input.trigger;
				break;
			case RCHoverboard._EInputSource.PrimaryFaceButton:
				num = (float)input.buttons;
				break;
			default:
				num = 0f;
				break;
			}
			float x = num;
			return this.remapCurve.ResolvedValue.Evaluate(math.abs(x)) * math.sign(x);
		}

		// Token: 0x0400087A RID: 2170
		public GTOption<StringEnum<RCHoverboard._EInputSource>> source;

		// Token: 0x0400087B RID: 2171
		public GTOption<AnimationCurve> remapCurve;
	}
}
