using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F09 RID: 3849
	public class TalkingSkullHelper : MonoBehaviour, IGorillaSliceableSimple, ITickSystemTick
	{
		// Token: 0x06005F47 RID: 24391 RVA: 0x001E1004 File Offset: 0x001DF204
		public void SetMouseMatchMode(bool value)
		{
			if (!this.mouseRotation && value)
			{
				this.mouseRotation = true;
				this.cachedParentRotation = this.GetParentHand().localRotation;
				this.cachedTargetLocalRotation = this.targetLocalRotation;
				Debug.Log("MOUSE");
				return;
			}
			if (this.mouseRotation && !value)
			{
				this.mouseRotation = false;
				this.cachedTargetLocalRotation = this.GetParentHand().localRotation;
				Debug.Log("NONE");
			}
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x001E107C File Offset: 0x001DF27C
		public void SetJoystickMatchMode(bool value)
		{
			if (!this.joystickRotation && value)
			{
				this.joystickRotation = true;
				this.cachedParentRotation = this.GetParentHand().localRotation;
				this.cachedTargetLocalRotation = this.targetLocalRotation;
				Debug.Log("JOYSTICK");
				return;
			}
			if (this.joystickRotation && !value)
			{
				this.joystickRotation = false;
				this.cachedTargetLocalRotation = this.GetParentHand().localRotation;
				Debug.Log("NONE");
			}
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x001E10F4 File Offset: 0x001DF2F4
		private Transform GetParentHand()
		{
			TransferrableObject.PositionState currentState = this.transferrableObject.currentState;
			if (currentState != TransferrableObject.PositionState.OnLeftArm && currentState != TransferrableObject.PositionState.OnLeftShoulder)
			{
				return this.rightHandTransform;
			}
			return this.leftHandTransform;
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x001E1124 File Offset: 0x001DF324
		public void Awake()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this.SetEyeColor(this.EyeColorOff);
			this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
			this.animator = base.GetComponent<Animator>();
			this.idleAnimationTrigger = Animator.StringToHash(this.idleAnimationTriggerName);
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x001E1174 File Offset: 0x001DF374
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this._speakerLoudness = base.GetComponentInParent<GorillaSpeakerLoudness>();
			this.leftHandTransform = this.transferrableObject.ownerRig.leftHand.rigTarget;
			this.rightHandTransform = this.transferrableObject.ownerRig.rightHand.rigTarget;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x001E11D0 File Offset: 0x001DF3D0
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x001E11DF File Offset: 0x001DF3DF
		public void ToggleIsPlaced(bool isPlaced)
		{
			this._isPlaced = isPlaced;
			if (!this._isPlaced)
			{
				this.CanTalk(false);
				this.targetLocalRotation = Quaternion.identity;
				this.cachedParentRotation = Quaternion.identity;
			}
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x001E1210 File Offset: 0x001DF410
		public void CanTalk(bool toggle)
		{
			if (this._isActive == toggle)
			{
				return;
			}
			this._isActive = toggle;
			if (toggle)
			{
				this._activator.StartLocalBroadcast();
				this.SetEyeColor(this.EyeColorOn);
				return;
			}
			this._activator.StopLocalBroadcast();
			this.SetEyeColor(this.EyeColorOff);
		}

		// Token: 0x06005F4F RID: 24399 RVA: 0x001E1260 File Offset: 0x001DF460
		public void SliceUpdate()
		{
			this._deltaTime = Time.time - this._timeLastUpdated;
			this._timeLastUpdated = Time.time;
			if (!this._isPlaced || !this._speakerLoudness.IsSpeaking)
			{
				this._attack = 0f;
				if (this._animation != null && !this._animation.isPlaying)
				{
					this._skullTransform.localPosition = this.SkullOffsetPosition;
				}
				return;
			}
			float num = 0f;
			if (this._speakerLoudness != null)
			{
				num = this._speakerLoudness.LoudnessNormalized;
			}
			if (num >= this.LoudnessThreshold)
			{
				this._attack += this._deltaTime;
				if (this._attack >= this.LoudnessAttack && this._animation != null && !this._animation.isPlaying)
				{
					this._animation.Play();
				}
				if (this.rotatingMouthTransform != null)
				{
					this.rotatingMouthTransform.localRotation = Quaternion.Slerp(this.initialMouthRotation, this.fullVolumeMouthRotation, Mathf.Min(num, 1f) * (Mathf.Sin(Time.time * 3.1415927f * this.mouthRotationYapSpeed) * 0.5f + 0.5f));
					return;
				}
			}
			else
			{
				this._attack = 0f;
				if (this.rotatingMouthTransform != null)
				{
					this.rotatingMouthTransform.localRotation = this.initialMouthRotation;
				}
			}
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x001E13CF File Offset: 0x001DF5CF
		private void SetEyeColor(Color eyeColor)
		{
			this._materialPropertyBlock.SetColor("_BaseColor", eyeColor);
			this._skinnedMeshRenderer.SetPropertyBlock(this._materialPropertyBlock, 0);
		}

		// Token: 0x06005F51 RID: 24401 RVA: 0x001E13F4 File Offset: 0x001DF5F4
		private void ResetToFirstFrame()
		{
			this._animation.Rewind();
			this._animation.Play();
			this._animation.Sample();
			this._animation.Stop();
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06005F52 RID: 24402 RVA: 0x001E1423 File Offset: 0x001DF623
		// (set) Token: 0x06005F53 RID: 24403 RVA: 0x001E142B File Offset: 0x001DF62B
		public bool TickRunning { get; set; }

		// Token: 0x06005F54 RID: 24404 RVA: 0x001E1434 File Offset: 0x001DF634
		public void Tick()
		{
			if (this.hasBlendShape)
			{
				if (!this._isPlaced)
				{
					if (this.currentBlendShapeWeight > 0f)
					{
						this.currentBlendShapeWeight -= Time.deltaTime * this.AntennaExtendSpeed;
						this._skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Clamp(this.currentBlendShapeWeight, 0f, 100f));
					}
				}
				else if (this.currentBlendShapeWeight < 100f)
				{
					this.currentBlendShapeWeight += Time.deltaTime * this.AntennaExtendSpeed;
					this._skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Clamp(this.currentBlendShapeWeight, 0f, 100f));
				}
			}
			if (this._isPlaced && this.dockHandRotationMatchTransform != null)
			{
				Quaternion localRotation = this.GetParentHand().localRotation;
				Quaternion rhs = localRotation * Quaternion.Inverse(this.cachedParentRotation);
				if (this.mouseRotation)
				{
					this.targetLocalRotation *= rhs;
					this.cachedParentRotation = localRotation;
				}
				else if (this.joystickRotation)
				{
					float num;
					Vector3 axis;
					rhs.ToAngleAxis(out num, out axis);
					this.targetLocalRotation *= Quaternion.AngleAxis(num * Time.deltaTime, axis);
				}
			}
			float t = 1f - Mathf.Exp(-30f * Time.deltaTime);
			this.dockHandRotationMatchTransform.localRotation = Quaternion.Slerp(this.dockHandRotationMatchTransform.localRotation, this.targetLocalRotation, t);
			if (this._isPlaced && this._animation == null && this.animator != null)
			{
				float num2 = 1f - Mathf.Exp(-this.idleAnimationChancePerSecond * Time.deltaTime);
				if (Random.value < num2)
				{
					this.animator.SetTrigger(this.idleAnimationTrigger);
				}
			}
		}

		// Token: 0x040069D5 RID: 27093
		public TalkingCosmeticType TalkingCosmeticType;

		// Token: 0x040069D6 RID: 27094
		[Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
		public float LoudnessThreshold = 0.1f;

		// Token: 0x040069D7 RID: 27095
		[Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
		public float LoudnessAttack = 0.15f;

		// Token: 0x040069D8 RID: 27096
		[FormerlySerializedAs("HasAntenna")]
		public bool hasBlendShape = true;

		// Token: 0x040069D9 RID: 27097
		[Tooltip("How fast the antenna should extend (with the range of the blend shape from 0-100).")]
		public float AntennaExtendSpeed = 100f;

		// Token: 0x040069DA RID: 27098
		public Color EyeColorOff = Color.black;

		// Token: 0x040069DB RID: 27099
		public Color EyeColorOn = Color.white;

		// Token: 0x040069DC RID: 27100
		private float _attack;

		// Token: 0x040069DD RID: 27101
		private float _deltaTime;

		// Token: 0x040069DE RID: 27102
		private float _timeLastUpdated;

		// Token: 0x040069DF RID: 27103
		private bool _isPlaced;

		// Token: 0x040069E0 RID: 27104
		private bool _isActive;

		// Token: 0x040069E1 RID: 27105
		private float currentBlendShapeWeight;

		// Token: 0x040069E2 RID: 27106
		[SerializeField]
		private Animation _animation;

		// Token: 0x040069E3 RID: 27107
		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		// Token: 0x040069E4 RID: 27108
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040069E5 RID: 27109
		[SerializeField]
		private LoudSpeakerActivator _activator;

		// Token: 0x040069E6 RID: 27110
		[SerializeField]
		private GorillaSpeakerLoudness _speakerLoudness;

		// Token: 0x040069E7 RID: 27111
		[SerializeField]
		private Transform _skullTransform;

		// Token: 0x040069E8 RID: 27112
		public Vector3 SkullOffsetPosition = new Vector3(0f, -0.15f, 0f);

		// Token: 0x040069E9 RID: 27113
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x040069EA RID: 27114
		private Quaternion targetLocalRotation = Quaternion.identity;

		// Token: 0x040069EB RID: 27115
		private Quaternion cachedTargetLocalRotation = Quaternion.identity;

		// Token: 0x040069EC RID: 27116
		public Transform dockHandRotationMatchTransform;

		// Token: 0x040069ED RID: 27117
		public float rotationMatchDegreesPerSecond = 90f;

		// Token: 0x040069EE RID: 27118
		public Quaternion rotationMatchOffset = Quaternion.Euler(-50f, -90f, 0f);

		// Token: 0x040069EF RID: 27119
		private bool mouseRotation;

		// Token: 0x040069F0 RID: 27120
		private bool joystickRotation;

		// Token: 0x040069F1 RID: 27121
		private Quaternion cachedParentRotation;

		// Token: 0x040069F2 RID: 27122
		public Transform rotatingMouthTransform;

		// Token: 0x040069F3 RID: 27123
		public Quaternion initialMouthRotation;

		// Token: 0x040069F4 RID: 27124
		public Quaternion fullVolumeMouthRotation;

		// Token: 0x040069F5 RID: 27125
		public float mouthRotationYapSpeed = 2f;

		// Token: 0x040069F6 RID: 27126
		private TransferrableObject transferrableObject;

		// Token: 0x040069F7 RID: 27127
		private Transform leftHandTransform;

		// Token: 0x040069F8 RID: 27128
		private Transform rightHandTransform;

		// Token: 0x040069F9 RID: 27129
		private Animator animator;

		// Token: 0x040069FA RID: 27130
		public string idleAnimationTriggerName;

		// Token: 0x040069FB RID: 27131
		private int idleAnimationTrigger;

		// Token: 0x040069FC RID: 27132
		[Range(0f, 1f)]
		public float idleAnimationChancePerSecond = 0.33f;

		// Token: 0x040069FD RID: 27133
		private float skullVolume;
	}
}
