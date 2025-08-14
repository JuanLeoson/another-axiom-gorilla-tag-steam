using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000BF7 RID: 3063
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x06004A74 RID: 19060 RVA: 0x00169CC9 File Offset: 0x00167EC9
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x06004A75 RID: 19061 RVA: 0x00169CDC File Offset: 0x00167EDC
		private void OnTriggerEnter(Collider other)
		{
			if (this.attachPoint.childCount == 0)
			{
				this.UpdateHookState(false);
			}
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || componentInParent.InHand())
			{
				return;
			}
			if (this.IsHooked())
			{
				return;
			}
			this.UpdateHookState(true);
			componentInParent.SnapItem(true, this.attachPoint.position);
		}

		// Token: 0x06004A76 RID: 19062 RVA: 0x00169D38 File Offset: 0x00167F38
		private void OnTriggerExit(Collider other)
		{
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || !componentInParent.InHand())
			{
				return;
			}
			this.UpdateHookState(false);
			componentInParent.SnapItem(false, Vector3.zero);
		}

		// Token: 0x06004A77 RID: 19063 RVA: 0x00169D71 File Offset: 0x00167F71
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x06004A78 RID: 19064 RVA: 0x00169D7A File Offset: 0x00167F7A
		internal void SetIsHook(bool isHooked)
		{
			this.isHooked = isHooked;
			UnityAction unityAction = this.onHookedChanged;
			if (unityAction == null)
			{
				return;
			}
			unityAction();
		}

		// Token: 0x06004A79 RID: 19065 RVA: 0x00169D93 File Offset: 0x00167F93
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04005368 RID: 21352
		public Transform attachPoint;

		// Token: 0x04005369 RID: 21353
		public UnityAction onHookedChanged;

		// Token: 0x0400536A RID: 21354
		private bool isHooked;

		// Token: 0x0400536B RID: 21355
		private bool wasHooked;

		// Token: 0x0400536C RID: 21356
		public bool inForest;
	}
}
