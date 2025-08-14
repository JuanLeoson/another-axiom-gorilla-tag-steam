using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000D3F RID: 3391
	[RequireComponent(typeof(OVRGrabber))]
	public class Hand : MonoBehaviour
	{
		// Token: 0x060053F7 RID: 21495 RVA: 0x0019F0BA File Offset: 0x0019D2BA
		private void Awake()
		{
			this.m_grabber = base.GetComponent<OVRGrabber>();
		}

		// Token: 0x060053F8 RID: 21496 RVA: 0x0019F0C8 File Offset: 0x0019D2C8
		private void Start()
		{
			this.m_showAfterInputFocusAcquired = new List<Renderer>();
			this.m_colliders = (from childCollider in base.GetComponentsInChildren<Collider>()
			where !childCollider.isTrigger
			select childCollider).ToArray<Collider>();
			this.CollisionEnable(false);
			this.m_animLayerIndexPoint = this.m_animator.GetLayerIndex("Point Layer");
			this.m_animLayerIndexThumb = this.m_animator.GetLayerIndex("Thumb Layer");
			this.m_animParamIndexFlex = Animator.StringToHash("Flex");
			this.m_animParamIndexPose = Animator.StringToHash("Pose");
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x060053F9 RID: 21497 RVA: 0x0019F18A File Offset: 0x0019D38A
		private void OnDestroy()
		{
			OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
			OVRManager.InputFocusLost -= this.OnInputFocusLost;
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x0019F1B0 File Offset: 0x0019D3B0
		private void Update()
		{
			this.UpdateCapTouchStates();
			this.m_pointBlend = this.InputValueRateChange(this.m_isPointing, this.m_pointBlend);
			this.m_thumbsUpBlend = this.InputValueRateChange(this.m_isGivingThumbsUp, this.m_thumbsUpBlend);
			float num = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			bool enabled = this.m_grabber.grabbedObject == null && num >= 0.9f;
			this.CollisionEnable(enabled);
			this.UpdateAnimStates();
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x0019F22F File Offset: 0x0019D42F
		private void UpdateCapTouchStates()
		{
			this.m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, this.m_controller);
			this.m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this.m_controller);
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x0019F25C File Offset: 0x0019D45C
		private void LateUpdate()
		{
			if (this.m_collisionEnabled && this.m_collisionScaleCurrent + Mathf.Epsilon < 1f)
			{
				this.m_collisionScaleCurrent = Mathf.Min(1f, this.m_collisionScaleCurrent + Time.deltaTime * 1f);
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					this.m_colliders[i].transform.localScale = new Vector3(this.m_collisionScaleCurrent, this.m_collisionScaleCurrent, this.m_collisionScaleCurrent);
				}
			}
		}

		// Token: 0x060053FD RID: 21501 RVA: 0x0019F2E4 File Offset: 0x0019D4E4
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.m_showAfterInputFocusAcquired.Clear();
				Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled)
					{
						componentsInChildren[i].enabled = false;
						this.m_showAfterInputFocusAcquired.Add(componentsInChildren[i]);
					}
				}
				this.CollisionEnable(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x0019F350 File Offset: 0x0019D550
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				for (int i = 0; i < this.m_showAfterInputFocusAcquired.Count; i++)
				{
					if (this.m_showAfterInputFocusAcquired[i])
					{
						this.m_showAfterInputFocusAcquired[i].enabled = true;
					}
				}
				this.m_showAfterInputFocusAcquired.Clear();
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x0019F3B4 File Offset: 0x0019D5B4
		private float InputValueRateChange(bool isDown, float value)
		{
			float num = Time.deltaTime * 20f;
			float num2 = isDown ? 1f : -1f;
			return Mathf.Clamp01(value + num * num2);
		}

		// Token: 0x06005400 RID: 21504 RVA: 0x0019F3E8 File Offset: 0x0019D5E8
		private void UpdateAnimStates()
		{
			bool flag = this.m_grabber.grabbedObject != null;
			HandPose handPose = this.m_defaultGrabPose;
			if (flag)
			{
				HandPose component = this.m_grabber.grabbedObject.GetComponent<HandPose>();
				if (component != null)
				{
					handPose = component;
				}
			}
			HandPoseId poseId = handPose.PoseId;
			this.m_animator.SetInteger(this.m_animParamIndexPose, (int)poseId);
			float value = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			this.m_animator.SetFloat(this.m_animParamIndexFlex, value);
			float weight = (!flag || handPose.AllowPointing) ? this.m_pointBlend : 0f;
			this.m_animator.SetLayerWeight(this.m_animLayerIndexPoint, weight);
			float weight2 = (!flag || handPose.AllowThumbsUp) ? this.m_thumbsUpBlend : 0f;
			this.m_animator.SetLayerWeight(this.m_animLayerIndexThumb, weight2);
			float value2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller);
			this.m_animator.SetFloat("Pinch", value2);
		}

		// Token: 0x06005401 RID: 21505 RVA: 0x0019F4E4 File Offset: 0x0019D6E4
		private void CollisionEnable(bool enabled)
		{
			if (this.m_collisionEnabled == enabled)
			{
				return;
			}
			this.m_collisionEnabled = enabled;
			if (enabled)
			{
				this.m_collisionScaleCurrent = 0.01f;
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					Collider collider = this.m_colliders[i];
					collider.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					collider.enabled = true;
				}
				return;
			}
			this.m_collisionScaleCurrent = 1f;
			for (int j = 0; j < this.m_colliders.Length; j++)
			{
				Collider collider2 = this.m_colliders[j];
				collider2.enabled = false;
				collider2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
		}

		// Token: 0x04005D67 RID: 23911
		public const string ANIM_LAYER_NAME_POINT = "Point Layer";

		// Token: 0x04005D68 RID: 23912
		public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		// Token: 0x04005D69 RID: 23913
		public const string ANIM_PARAM_NAME_FLEX = "Flex";

		// Token: 0x04005D6A RID: 23914
		public const string ANIM_PARAM_NAME_POSE = "Pose";

		// Token: 0x04005D6B RID: 23915
		public const float THRESH_COLLISION_FLEX = 0.9f;

		// Token: 0x04005D6C RID: 23916
		public const float INPUT_RATE_CHANGE = 20f;

		// Token: 0x04005D6D RID: 23917
		public const float COLLIDER_SCALE_MIN = 0.01f;

		// Token: 0x04005D6E RID: 23918
		public const float COLLIDER_SCALE_MAX = 1f;

		// Token: 0x04005D6F RID: 23919
		public const float COLLIDER_SCALE_PER_SECOND = 1f;

		// Token: 0x04005D70 RID: 23920
		public const float TRIGGER_DEBOUNCE_TIME = 0.05f;

		// Token: 0x04005D71 RID: 23921
		public const float THUMB_DEBOUNCE_TIME = 0.15f;

		// Token: 0x04005D72 RID: 23922
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04005D73 RID: 23923
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04005D74 RID: 23924
		[SerializeField]
		private HandPose m_defaultGrabPose;

		// Token: 0x04005D75 RID: 23925
		private Collider[] m_colliders;

		// Token: 0x04005D76 RID: 23926
		private bool m_collisionEnabled = true;

		// Token: 0x04005D77 RID: 23927
		private OVRGrabber m_grabber;

		// Token: 0x04005D78 RID: 23928
		private List<Renderer> m_showAfterInputFocusAcquired;

		// Token: 0x04005D79 RID: 23929
		private int m_animLayerIndexThumb = -1;

		// Token: 0x04005D7A RID: 23930
		private int m_animLayerIndexPoint = -1;

		// Token: 0x04005D7B RID: 23931
		private int m_animParamIndexFlex = -1;

		// Token: 0x04005D7C RID: 23932
		private int m_animParamIndexPose = -1;

		// Token: 0x04005D7D RID: 23933
		private bool m_isPointing;

		// Token: 0x04005D7E RID: 23934
		private bool m_isGivingThumbsUp;

		// Token: 0x04005D7F RID: 23935
		private float m_pointBlend;

		// Token: 0x04005D80 RID: 23936
		private float m_thumbsUpBlend;

		// Token: 0x04005D81 RID: 23937
		private bool m_restoreOnInputAcquired;

		// Token: 0x04005D82 RID: 23938
		private float m_collisionScaleCurrent;
	}
}
