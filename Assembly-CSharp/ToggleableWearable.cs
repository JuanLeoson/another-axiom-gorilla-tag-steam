using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000256 RID: 598
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x06000DF6 RID: 3574 RVA: 0x00055230 File Offset: 0x00053430
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = (this.ownerRig != null);
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = (this.audioSource != null);
		this.assignedSlotBitIndex = (int)this.assignedSlot;
		if (this.oneShot)
		{
			this.toggleCooldownRange.x = this.toggleCooldownRange.x + this.animationTransitionDuration;
			this.toggleCooldownRange.y = this.toggleCooldownRange.y + this.animationTransitionDuration;
		}
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00055338 File Offset: 0x00053538
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.lossyScale.x, this.colliders, this.layerMask) > 0 && this.toggleCooldownTimer < 0f)
			{
				XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					this.LocalToggle(componentInParent.controllerNode == XRNode.LeftHand, true, true);
				}
				this.toggleCooldownTimer = Random.Range(this.toggleCooldownRange.x, this.toggleCooldownRange.y);
				this.toggleTimer = 0f;
			}
			if (this.resetTimer > 0f)
			{
				this.toggleTimer += Time.deltaTime;
				if (this.toggleTimer > this.resetTimer && this.startOn != this.isOn)
				{
					this.LocalToggle(false, true, false);
					this.toggleTimer = 0f;
				}
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag, true);
			}
		}
		if (this.oneShot)
		{
			if (this.isOn)
			{
				this.progress = Mathf.MoveTowards(this.progress, 1f, Time.deltaTime / this.animationTransitionDuration);
				if (this.progress == 1f)
				{
					if (this.ownerIsLocal)
					{
						this.LocalToggle(false, false, false);
					}
					else
					{
						this.SharedSetState(false, false);
					}
					this.progress = 0f;
				}
			}
		}
		else
		{
			this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		}
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00055548 File Offset: 0x00053748
	private void LocalToggle(bool isLeftHand, bool playAudio, bool playHaptics)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0, playAudio);
		if (playHaptics && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x000555DC File Offset: 0x000537DC
	private void SharedSetState(bool state, bool playAudio)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (!playAudio || !this.hasAudioSource)
		{
			return;
		}
		AudioClip audioClip = this.isOn ? this.toggleOnSound : this.toggleOffSound;
		if (audioClip == null)
		{
			return;
		}
		if (this.oneShot)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.GTPlay();
			return;
		}
		this.audioSource.GTPlayOneShot(audioClip, 1f);
	}

	// Token: 0x040015C5 RID: 5573
	public Renderer[] renderers;

	// Token: 0x040015C6 RID: 5574
	public Animator[] animators;

	// Token: 0x040015C7 RID: 5575
	public float animationTransitionDuration = 1f;

	// Token: 0x040015C8 RID: 5576
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x040015C9 RID: 5577
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x040015CA RID: 5578
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x040015CB RID: 5579
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x040015CC RID: 5580
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x040015CD RID: 5581
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x040015CE RID: 5582
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x040015CF RID: 5583
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x040015D0 RID: 5584
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x040015D1 RID: 5585
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x040015D2 RID: 5586
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x040015D3 RID: 5587
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x040015D4 RID: 5588
	private VRRig ownerRig;

	// Token: 0x040015D5 RID: 5589
	private bool ownerIsLocal;

	// Token: 0x040015D6 RID: 5590
	private bool isOn;

	// Token: 0x040015D7 RID: 5591
	[SerializeField]
	private Vector2 toggleCooldownRange = new Vector2(0.2f, 0.2f);

	// Token: 0x040015D8 RID: 5592
	private bool hasAudioSource;

	// Token: 0x040015D9 RID: 5593
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x040015DA RID: 5594
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x040015DB RID: 5595
	private float toggleCooldownTimer;

	// Token: 0x040015DC RID: 5596
	private int assignedSlotBitIndex;

	// Token: 0x040015DD RID: 5597
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x040015DE RID: 5598
	private float progress;

	// Token: 0x040015DF RID: 5599
	[SerializeField]
	private bool oneShot;

	// Token: 0x040015E0 RID: 5600
	[SerializeField]
	[Tooltip("Seconds before reverting to its default state, as defined by 'Start On.' A value of 0 or less means never.")]
	private float resetTimer;

	// Token: 0x040015E1 RID: 5601
	private float toggleTimer;
}
