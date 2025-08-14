using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F6C RID: 3948
	public class SimpleTransformAnimatorCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060061B6 RID: 25014 RVA: 0x001F1103 File Offset: 0x001EF303
		private void DebugToggle()
		{
			this.Toggle();
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x060061B7 RID: 25015 RVA: 0x001F110B File Offset: 0x001EF30B
		// (set) Token: 0x060061B8 RID: 25016 RVA: 0x001F1113 File Offset: 0x001EF313
		public bool TickRunning { get; set; }

		// Token: 0x060061B9 RID: 25017 RVA: 0x001F111C File Offset: 0x001EF31C
		private void OnEnable()
		{
			this.currentBlendValue = this.targetBlendValue;
			this.UpdateTransform();
		}

		// Token: 0x060061BA RID: 25018 RVA: 0x001F1130 File Offset: 0x001EF330
		private void OnDisable()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
			}
		}

		// Token: 0x060061BB RID: 25019 RVA: 0x001F1148 File Offset: 0x001EF348
		private void CheckAnimationNeeded()
		{
			bool flag = !Mathf.Approximately(this.currentBlendValue, this.targetBlendValue);
			if (flag && !this.TickRunning)
			{
				TickSystem<object>.AddCallbackTarget(this);
				this.TickRunning = true;
				this.isAnimating = true;
				return;
			}
			if (!flag && this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
				this.isAnimating = false;
			}
		}

		// Token: 0x060061BC RID: 25020 RVA: 0x001F11AC File Offset: 0x001EF3AC
		public void Tick()
		{
			float num = 1f / this.transitionTime;
			this.currentBlendValue = Mathf.MoveTowards(this.currentBlendValue, this.targetBlendValue, Time.deltaTime * num);
			this.UpdateTransform();
			this.CheckAnimationNeeded();
		}

		// Token: 0x060061BD RID: 25021 RVA: 0x001F11F0 File Offset: 0x001EF3F0
		private void UpdateTransform()
		{
			Vector3 position = this.targetTransform.position;
			Quaternion rotation = this.targetTransform.rotation;
			if (this.mode == SimpleTransformAnimatorCosmetic.LerpMode.Position || this.mode == SimpleTransformAnimatorCosmetic.LerpMode.PositionAndRotation)
			{
				position = Vector3.Lerp(this.poseA.position, this.poseB.position, this.currentBlendValue);
			}
			if (this.mode == SimpleTransformAnimatorCosmetic.LerpMode.Rotation || this.mode == SimpleTransformAnimatorCosmetic.LerpMode.PositionAndRotation)
			{
				rotation = Quaternion.Slerp(this.poseA.rotation, this.poseB.rotation, this.currentBlendValue);
			}
			this.targetTransform.SetPositionAndRotation(position, rotation);
		}

		// Token: 0x060061BE RID: 25022 RVA: 0x001F1289 File Offset: 0x001EF489
		public void Toggle()
		{
			this.targetBlendValue = ((this.targetBlendValue < 0.5f) ? 1f : 0f);
			this.CheckAnimationNeeded();
		}

		// Token: 0x060061BF RID: 25023 RVA: 0x001F12B0 File Offset: 0x001EF4B0
		public void TogglePoseA()
		{
			this.targetBlendValue = 0f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x060061C0 RID: 25024 RVA: 0x001F12C3 File Offset: 0x001EF4C3
		public void TogglePoseB()
		{
			this.targetBlendValue = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x04006DFF RID: 28159
		[SerializeField]
		[Tooltip("The object that will animate (blend) between the poses.")]
		private Transform targetTransform;

		// Token: 0x04006E00 RID: 28160
		[SerializeField]
		[Tooltip("Start pose (blend value 0).")]
		private Transform poseA;

		// Token: 0x04006E01 RID: 28161
		[SerializeField]
		[Tooltip("End pose (blend value 1).")]
		private Transform poseB;

		// Token: 0x04006E02 RID: 28162
		[SerializeField]
		[Tooltip("Total time (in seconds) to animate fully between poses.")]
		private float transitionTime = 1f;

		// Token: 0x04006E03 RID: 28163
		[SerializeField]
		[Tooltip("Controls what aspect of the transform is affected by the blend.")]
		private SimpleTransformAnimatorCosmetic.LerpMode mode = SimpleTransformAnimatorCosmetic.LerpMode.PositionAndRotation;

		// Token: 0x04006E04 RID: 28164
		private float currentBlendValue;

		// Token: 0x04006E05 RID: 28165
		private float targetBlendValue;

		// Token: 0x04006E06 RID: 28166
		private bool isAnimating;

		// Token: 0x02000F6D RID: 3949
		public enum LerpMode
		{
			// Token: 0x04006E09 RID: 28169
			Position,
			// Token: 0x04006E0A RID: 28170
			Rotation,
			// Token: 0x04006E0B RID: 28171
			PositionAndRotation
		}
	}
}
