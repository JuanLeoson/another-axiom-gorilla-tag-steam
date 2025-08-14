using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000D0F RID: 3343
	public abstract class Interactable : MonoBehaviour
	{
		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x060052B7 RID: 21175 RVA: 0x0019AA6B File Offset: 0x00198C6B
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x060052B8 RID: 21176 RVA: 0x0019AA73 File Offset: 0x00198C73
		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x060052B9 RID: 21177 RVA: 0x0019AA7B File Offset: 0x00198C7B
		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x060052BA RID: 21178 RVA: 0x000E3FB3 File Offset: 0x000E21B3
		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x060052BB RID: 21179 RVA: 0x0019AA84 File Offset: 0x00198C84
		// (remove) Token: 0x060052BC RID: 21180 RVA: 0x0019AABC File Offset: 0x00198CBC
		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		// Token: 0x060052BD RID: 21181 RVA: 0x0019AAF1 File Offset: 0x00198CF1
		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x060052BE RID: 21182 RVA: 0x0019AB08 File Offset: 0x00198D08
		// (remove) Token: 0x060052BF RID: 21183 RVA: 0x0019AB40 File Offset: 0x00198D40
		public event Action<ColliderZoneArgs> ContactZoneEvent;

		// Token: 0x060052C0 RID: 21184 RVA: 0x0019AB75 File Offset: 0x00198D75
		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		// Token: 0x1400009E RID: 158
		// (add) Token: 0x060052C1 RID: 21185 RVA: 0x0019AB8C File Offset: 0x00198D8C
		// (remove) Token: 0x060052C2 RID: 21186 RVA: 0x0019ABC4 File Offset: 0x00198DC4
		public event Action<ColliderZoneArgs> ActionZoneEvent;

		// Token: 0x060052C3 RID: 21187 RVA: 0x0019ABF9 File Offset: 0x00198DF9
		protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
		{
			if (this.ActionZoneEvent != null)
			{
				this.ActionZoneEvent(args);
			}
		}

		// Token: 0x060052C4 RID: 21188
		public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

		// Token: 0x060052C5 RID: 21189 RVA: 0x0019AC0F File Offset: 0x00198E0F
		protected virtual void Awake()
		{
			InteractableRegistry.RegisterInteractable(this);
		}

		// Token: 0x060052C6 RID: 21190 RVA: 0x0019AC17 File Offset: 0x00198E17
		protected virtual void OnDestroy()
		{
			InteractableRegistry.UnregisterInteractable(this);
		}

		// Token: 0x04005C19 RID: 23577
		protected ColliderZone _proximityZoneCollider;

		// Token: 0x04005C1A RID: 23578
		protected ColliderZone _contactZoneCollider;

		// Token: 0x04005C1B RID: 23579
		protected ColliderZone _actionZoneCollider;

		// Token: 0x04005C1F RID: 23583
		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		// Token: 0x02000D10 RID: 3344
		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
