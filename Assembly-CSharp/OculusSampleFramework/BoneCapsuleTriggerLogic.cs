using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D05 RID: 3333
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		// Token: 0x06005275 RID: 21109 RVA: 0x00199D5F File Offset: 0x00197F5F
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		// Token: 0x06005276 RID: 21110 RVA: 0x00199D6C File Offset: 0x00197F6C
		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		// Token: 0x06005277 RID: 21111 RVA: 0x00199D74 File Offset: 0x00197F74
		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		// Token: 0x06005278 RID: 21112 RVA: 0x00199DB4 File Offset: 0x00197FB4
		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		// Token: 0x06005279 RID: 21113 RVA: 0x00199DF4 File Offset: 0x00197FF4
		private void CleanUpDeadColliders()
		{
			this._elementsToCleanUp.Clear();
			foreach (ColliderZone colliderZone in this.CollidersTouchingUs)
			{
				if (!colliderZone.Collider.gameObject.activeInHierarchy)
				{
					this._elementsToCleanUp.Add(colliderZone);
				}
			}
			foreach (ColliderZone item in this._elementsToCleanUp)
			{
				this.CollidersTouchingUs.Remove(item);
			}
		}

		// Token: 0x04005BE3 RID: 23523
		public InteractableToolTags ToolTags;

		// Token: 0x04005BE4 RID: 23524
		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		// Token: 0x04005BE5 RID: 23525
		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
