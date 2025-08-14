using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class Bubbler : TransferrableObject
{
	// Token: 0x06001D85 RID: 7557 RVA: 0x0009E1DC File Offset: 0x0009C3DC
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasParticleSystem = (this.bubbleParticleSystem != null);
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x0009E280 File Offset: 0x0009C480
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = (this.bubblerAudio != null && this.bubblerAudio.clip != null);
		this.hasPopBubbleAudio = (this.popBubbleAudio != null && this.popBubbleAudio.clip != null);
		this.hasFan = (this.fan != null);
		this.hasActiveOnlyComponent = (this.gameObjectActiveOnlyWhileTriggerDown != null);
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x0009E310 File Offset: 0x0009C510
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x0009E364 File Offset: 0x0009C564
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x0009E3D4 File Offset: 0x0009C5D4
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x0009E3E2 File Offset: 0x0009C5E2
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && GTPlayer.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x0009E408 File Offset: 0x0009C608
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool forLeftController = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool enabled = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enabled;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTStop();
			}
			if (this.hasActiveOnlyComponent)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(false);
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTPlay();
			}
			if (this.hasActiveOnlyComponent && !this.gameObjectActiveOnlyWhileTriggerDown.activeSelf)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(true);
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(forLeftController, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(forLeftController, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				if (!this.fanYaxisinstead)
				{
					float z = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, 0f, z);
				}
				else
				{
					float y = this.fan.transform.localEulerAngles.y + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, y, 0f);
				}
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = (particles <= 0);
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint key in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(key, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							GTAudioSourceExtensions.GTPlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(key);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, out this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x0009E814 File Offset: 0x0009CA14
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x0002472C File Offset: 0x0002292C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x0009E823 File Offset: 0x0009CA23
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x0009E82E File Offset: 0x0009CA2E
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002606 RID: 9734
	[SerializeField]
	private bool _worksInWater = true;

	// Token: 0x04002607 RID: 9735
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04002608 RID: 9736
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04002609 RID: 9737
	public AudioSource bubblerAudio;

	// Token: 0x0400260A RID: 9738
	public AudioSource popBubbleAudio;

	// Token: 0x0400260B RID: 9739
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x0400260C RID: 9740
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x0400260D RID: 9741
	private Vector3 outPosition;

	// Token: 0x0400260E RID: 9742
	private bool allBubblesPopped;

	// Token: 0x0400260F RID: 9743
	public bool disableActivation;

	// Token: 0x04002610 RID: 9744
	public bool disableDeactivation;

	// Token: 0x04002611 RID: 9745
	public float rotationSpeed = 5f;

	// Token: 0x04002612 RID: 9746
	public GameObject fan;

	// Token: 0x04002613 RID: 9747
	public bool fanYaxisinstead;

	// Token: 0x04002614 RID: 9748
	public float ongoingStrength = 0.005f;

	// Token: 0x04002615 RID: 9749
	public float triggerStrength = 0.2f;

	// Token: 0x04002616 RID: 9750
	private float initialTriggerPull;

	// Token: 0x04002617 RID: 9751
	private float initialTriggerDuration;

	// Token: 0x04002618 RID: 9752
	private bool hasBubblerAudio;

	// Token: 0x04002619 RID: 9753
	private bool hasPopBubbleAudio;

	// Token: 0x0400261A RID: 9754
	public GameObject gameObjectActiveOnlyWhileTriggerDown;

	// Token: 0x0400261B RID: 9755
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x0400261C RID: 9756
	private bool hasParticleSystem;

	// Token: 0x0400261D RID: 9757
	private bool hasFan;

	// Token: 0x0400261E RID: 9758
	private bool hasActiveOnlyComponent;

	// Token: 0x020004AB RID: 1195
	private enum BubblerState
	{
		// Token: 0x04002620 RID: 9760
		None = 1,
		// Token: 0x04002621 RID: 9761
		Bubbling
	}
}
