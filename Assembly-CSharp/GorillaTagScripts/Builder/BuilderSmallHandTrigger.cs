using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA0 RID: 3232
	public class BuilderSmallHandTrigger : MonoBehaviour
	{
		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x0600503E RID: 20542 RVA: 0x00190104 File Offset: 0x0018E304
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x0600503F RID: 20543 RVA: 0x00190114 File Offset: 0x0018E314
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (this.onlySmallHands && !this.ignoreScale && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.requireMinimumVelocity)
			{
				float num = this.minimumVelocityMagnitude * GorillaTagger.Instance.offlineVRRig.scaleFactor;
				if ((componentInParent.isLeftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.1f, false).sqrMagnitude < num * num)
				{
					return;
				}
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			this.lastTriggeredFrame = Time.frameCount;
			UnityEvent triggeredEvent = this.TriggeredEvent;
			if (triggeredEvent != null)
			{
				triggeredEvent.Invoke();
			}
			if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
			{
				this.timeline.Play();
			}
			if (this.animation != null && this.animation.clip != null)
			{
				this.animation.Play();
			}
		}

		// Token: 0x04005972 RID: 22898
		[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
		public PlayableDirector timeline;

		// Token: 0x04005973 RID: 22899
		[Tooltip("Optional animation to play")]
		public Animation animation;

		// Token: 0x04005974 RID: 22900
		private int lastTriggeredFrame = -1;

		// Token: 0x04005975 RID: 22901
		public bool onlySmallHands;

		// Token: 0x04005976 RID: 22902
		[SerializeField]
		protected bool requireMinimumVelocity;

		// Token: 0x04005977 RID: 22903
		[SerializeField]
		protected float minimumVelocityMagnitude = 0.1f;

		// Token: 0x04005978 RID: 22904
		private bool hasCheckedZone;

		// Token: 0x04005979 RID: 22905
		private bool ignoreScale;

		// Token: 0x0400597A RID: 22906
		internal UnityEvent TriggeredEvent = new UnityEvent();

		// Token: 0x0400597B RID: 22907
		[SerializeField]
		private BuilderPiece myPiece;
	}
}
