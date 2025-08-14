using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D18 RID: 3352
	public class FingerTipPokeTool : InteractableTool
	{
		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x060052E2 RID: 21218 RVA: 0x00144C03 File Offset: 0x00142E03
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Poke;
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x060052E3 RID: 21219 RVA: 0x00002076 File Offset: 0x00000276
		public override ToolInputState ToolInputState
		{
			get
			{
				return ToolInputState.Inactive;
			}
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x060052E4 RID: 21220 RVA: 0x00002076 File Offset: 0x00000276
		public override bool IsFarFieldTool
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x060052E5 RID: 21221 RVA: 0x0019B2CC File Offset: 0x001994CC
		// (set) Token: 0x060052E6 RID: 21222 RVA: 0x0019B2DE File Offset: 0x001994DE
		public override bool EnableState
		{
			get
			{
				return this._fingerTipPokeToolView.gameObject.activeSelf;
			}
			set
			{
				this._fingerTipPokeToolView.gameObject.SetActive(value);
			}
		}

		// Token: 0x060052E7 RID: 21223 RVA: 0x0019B2F4 File Offset: 0x001994F4
		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._fingerTipPokeToolView.InteractableTool = this;
			this._velocityFrames = new Vector3[10];
			Array.Clear(this._velocityFrames, 0, 10);
			base.StartCoroutine(this.AttachTriggerLogic());
		}

		// Token: 0x060052E8 RID: 21224 RVA: 0x0019B340 File Offset: 0x00199540
		private IEnumerator AttachTriggerLogic()
		{
			while (!HandsManager.Instance || !HandsManager.Instance.IsInitialized())
			{
				yield return null;
			}
			OVRSkeleton skeleton = base.IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton;
			OVRSkeleton.BoneId boneId;
			switch (this._fingerToFollow)
			{
			case OVRPlugin.HandFinger.Thumb:
				boneId = OVRSkeleton.BoneId.Hand_Thumb3;
				break;
			case OVRPlugin.HandFinger.Index:
				boneId = OVRSkeleton.BoneId.Hand_Index3;
				break;
			case OVRPlugin.HandFinger.Middle:
				boneId = OVRSkeleton.BoneId.Hand_Middle3;
				break;
			case OVRPlugin.HandFinger.Ring:
				boneId = OVRSkeleton.BoneId.Hand_Ring3;
				break;
			default:
				boneId = OVRSkeleton.BoneId.Hand_Pinky3;
				break;
			}
			List<BoneCapsuleTriggerLogic> list = new List<BoneCapsuleTriggerLogic>();
			List<OVRBoneCapsule> capsulesPerBone = HandsManager.GetCapsulesPerBone(skeleton, boneId);
			foreach (OVRBoneCapsule ovrboneCapsule in capsulesPerBone)
			{
				BoneCapsuleTriggerLogic boneCapsuleTriggerLogic = ovrboneCapsule.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
				ovrboneCapsule.CapsuleCollider.isTrigger = true;
				boneCapsuleTriggerLogic.ToolTags = this.ToolTags;
				list.Add(boneCapsuleTriggerLogic);
			}
			this._boneCapsuleTriggerLogic = list.ToArray();
			if (capsulesPerBone.Count > 0)
			{
				this._capsuleToTrack = capsulesPerBone[0];
			}
			this._isInitialized = true;
			yield break;
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x0019B350 File Offset: 0x00199550
		private void Update()
		{
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._isInitialized || this._capsuleToTrack == null)
			{
				return;
			}
			float handScale = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand).HandScale;
			Transform transform = this._capsuleToTrack.CapsuleCollider.transform;
			Vector3 right = transform.right;
			Vector3 vector = transform.position + this._capsuleToTrack.CapsuleCollider.height * 0.5f * right;
			Vector3 b = handScale * this._fingerTipPokeToolView.SphereRadius * right;
			Vector3 position = vector + b;
			base.transform.position = position;
			base.transform.rotation = transform.rotation;
			base.InteractionPosition = vector;
			this.UpdateAverageVelocity();
			this.CheckAndUpdateScale();
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x0019B438 File Offset: 0x00199638
		private void UpdateAverageVelocity()
		{
			Vector3 position = this._position;
			Vector3 position2 = base.transform.position;
			Vector3 vector = (position2 - position) / Time.deltaTime;
			this._position = position2;
			this._velocityFrames[this._currVelocityFrame] = vector;
			this._currVelocityFrame = (this._currVelocityFrame + 1) % 10;
			base.Velocity = Vector3.zero;
			if (!this._sampledMaxFramesAlready && this._currVelocityFrame == 9)
			{
				this._sampledMaxFramesAlready = true;
			}
			int num = this._sampledMaxFramesAlready ? 10 : (this._currVelocityFrame + 1);
			for (int i = 0; i < num; i++)
			{
				base.Velocity += this._velocityFrames[i];
			}
			base.Velocity /= (float)num;
		}

		// Token: 0x060052EB RID: 21227 RVA: 0x0019B510 File Offset: 0x00199710
		private void CheckAndUpdateScale()
		{
			float num = base.IsRightHandedTool ? HandsManager.Instance.RightHand.HandScale : HandsManager.Instance.LeftHand.HandScale;
			if (Mathf.Abs(num - this._lastScale) > Mathf.Epsilon)
			{
				base.transform.localScale = new Vector3(num, num, num);
				this._lastScale = num;
			}
		}

		// Token: 0x060052EC RID: 21228 RVA: 0x0019B574 File Offset: 0x00199774
		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			this._currentIntersectingObjects.Clear();
			BoneCapsuleTriggerLogic[] boneCapsuleTriggerLogic = this._boneCapsuleTriggerLogic;
			for (int i = 0; i < boneCapsuleTriggerLogic.Length; i++)
			{
				foreach (ColliderZone colliderZone in boneCapsuleTriggerLogic[i].CollidersTouchingUs)
				{
					this._currentIntersectingObjects.Add(new InteractableCollisionInfo(colliderZone, colliderZone.CollisionDepth, this));
				}
			}
			return this._currentIntersectingObjects;
		}

		// Token: 0x060052ED RID: 21229 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
		}

		// Token: 0x060052EE RID: 21230 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void DeFocus()
		{
		}

		// Token: 0x04005C42 RID: 23618
		private const int NUM_VELOCITY_FRAMES = 10;

		// Token: 0x04005C43 RID: 23619
		[SerializeField]
		private FingerTipPokeToolView _fingerTipPokeToolView;

		// Token: 0x04005C44 RID: 23620
		[SerializeField]
		private OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

		// Token: 0x04005C45 RID: 23621
		private Vector3[] _velocityFrames;

		// Token: 0x04005C46 RID: 23622
		private int _currVelocityFrame;

		// Token: 0x04005C47 RID: 23623
		private bool _sampledMaxFramesAlready;

		// Token: 0x04005C48 RID: 23624
		private Vector3 _position;

		// Token: 0x04005C49 RID: 23625
		private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

		// Token: 0x04005C4A RID: 23626
		private float _lastScale = 1f;

		// Token: 0x04005C4B RID: 23627
		private bool _isInitialized;

		// Token: 0x04005C4C RID: 23628
		private OVRBoneCapsule _capsuleToTrack;
	}
}
