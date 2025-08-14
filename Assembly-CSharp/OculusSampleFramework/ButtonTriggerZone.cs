using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D08 RID: 3336
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06005288 RID: 21128 RVA: 0x0019A3CE File Offset: 0x001985CE
		// (set) Token: 0x06005289 RID: 21129 RVA: 0x0019A3D6 File Offset: 0x001985D6
		public Collider Collider { get; private set; }

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x0600528A RID: 21130 RVA: 0x0019A3DF File Offset: 0x001985DF
		// (set) Token: 0x0600528B RID: 21131 RVA: 0x0019A3E7 File Offset: 0x001985E7
		public Interactable ParentInteractable { get; private set; }

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x0600528C RID: 21132 RVA: 0x0019A3F0 File Offset: 0x001985F0
		public InteractableCollisionDepth CollisionDepth
		{
			get
			{
				if (this.ParentInteractable.ProximityCollider == this)
				{
					return InteractableCollisionDepth.Proximity;
				}
				if (this.ParentInteractable.ContactCollider == this)
				{
					return InteractableCollisionDepth.Contact;
				}
				if (this.ParentInteractable.ActionCollider != this)
				{
					return InteractableCollisionDepth.None;
				}
				return InteractableCollisionDepth.Action;
			}
		}

		// Token: 0x0600528D RID: 21133 RVA: 0x0019A430 File Offset: 0x00198630
		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		// Token: 0x04005BF7 RID: 23543
		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
