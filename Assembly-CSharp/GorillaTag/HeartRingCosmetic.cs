using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E48 RID: 3656
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x06005BD3 RID: 23507 RVA: 0x001CF0C7 File Offset: 0x001CD2C7
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				base.enabled = false;
			};
		}

		// Token: 0x06005BD4 RID: 23508 RVA: 0x001CF0DC File Offset: 0x001CD2DC
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x06005BD5 RID: 23509 RVA: 0x001CF1E4 File Offset: 0x001CD3E4
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = (this.isHauntedVoiceChanger && flag);
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x0400658A RID: 25994
		public GameObject effects;

		// Token: 0x0400658B RID: 25995
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x0400658C RID: 25996
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x0400658D RID: 25997
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x0400658E RID: 25998
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x0400658F RID: 25999
		private VRRig ownerRig;

		// Token: 0x04006590 RID: 26000
		private Transform ownerHead;

		// Token: 0x04006591 RID: 26001
		private ParticleSystem particleSystem;

		// Token: 0x04006592 RID: 26002
		private AudioSource audioSource;

		// Token: 0x04006593 RID: 26003
		private float maxEmissionRate;

		// Token: 0x04006594 RID: 26004
		private float maxVolume;

		// Token: 0x04006595 RID: 26005
		private const float emissionFadeTime = 0.1f;

		// Token: 0x04006596 RID: 26006
		private const float volumeFadeTime = 2f;
	}
}
