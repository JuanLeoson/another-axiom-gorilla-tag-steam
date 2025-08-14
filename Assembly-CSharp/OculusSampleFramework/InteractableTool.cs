using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D1E RID: 3358
	public abstract class InteractableTool : MonoBehaviour
	{
		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06005302 RID: 21250 RVA: 0x0005860D File Offset: 0x0005680D
		public Transform ToolTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06005303 RID: 21251 RVA: 0x0019B822 File Offset: 0x00199A22
		// (set) Token: 0x06005304 RID: 21252 RVA: 0x0019B82A File Offset: 0x00199A2A
		public bool IsRightHandedTool { get; set; }

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06005305 RID: 21253
		public abstract InteractableToolTags ToolTags { get; }

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06005306 RID: 21254
		public abstract ToolInputState ToolInputState { get; }

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06005307 RID: 21255
		public abstract bool IsFarFieldTool { get; }

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x06005308 RID: 21256 RVA: 0x0019B833 File Offset: 0x00199A33
		// (set) Token: 0x06005309 RID: 21257 RVA: 0x0019B83B File Offset: 0x00199A3B
		public Vector3 Velocity { get; protected set; }

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x0600530A RID: 21258 RVA: 0x0019B844 File Offset: 0x00199A44
		// (set) Token: 0x0600530B RID: 21259 RVA: 0x0019B84C File Offset: 0x00199A4C
		public Vector3 InteractionPosition { get; protected set; }

		// Token: 0x0600530C RID: 21260 RVA: 0x0019B855 File Offset: 0x00199A55
		public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
		{
			return this._currentIntersectingObjects;
		}

		// Token: 0x0600530D RID: 21261
		public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

		// Token: 0x0600530E RID: 21262
		public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

		// Token: 0x0600530F RID: 21263
		public abstract void DeFocus();

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06005310 RID: 21264
		// (set) Token: 0x06005311 RID: 21265
		public abstract bool EnableState { get; set; }

		// Token: 0x06005312 RID: 21266
		public abstract void Initialize();

		// Token: 0x06005313 RID: 21267 RVA: 0x0019B85D File Offset: 0x00199A5D
		public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
		{
			return this._currInteractableToCollisionInfos.First<KeyValuePair<Interactable, InteractableCollisionInfo>>();
		}

		// Token: 0x06005314 RID: 21268 RVA: 0x0019B86A File Offset: 0x00199A6A
		public void ClearAllCurrentCollisionInfos()
		{
			this._currInteractableToCollisionInfos.Clear();
		}

		// Token: 0x06005315 RID: 21269 RVA: 0x0019B878 File Offset: 0x00199A78
		public virtual void UpdateCurrentCollisionsBasedOnDepth()
		{
			this._currInteractableToCollisionInfos.Clear();
			foreach (InteractableCollisionInfo interactableCollisionInfo in this._currentIntersectingObjects)
			{
				Interactable parentInteractable = interactableCollisionInfo.InteractableCollider.ParentInteractable;
				InteractableCollisionDepth collisionDepth = interactableCollisionInfo.CollisionDepth;
				InteractableCollisionInfo interactableCollisionInfo2 = null;
				if (!this._currInteractableToCollisionInfos.TryGetValue(parentInteractable, out interactableCollisionInfo2))
				{
					this._currInteractableToCollisionInfos[parentInteractable] = interactableCollisionInfo;
				}
				else if (interactableCollisionInfo2.CollisionDepth < collisionDepth)
				{
					interactableCollisionInfo2.InteractableCollider = interactableCollisionInfo.InteractableCollider;
					interactableCollisionInfo2.CollisionDepth = collisionDepth;
				}
			}
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x0019B924 File Offset: 0x00199B24
		public virtual void UpdateLatestCollisionData()
		{
			this._addedInteractables.Clear();
			this._removedInteractables.Clear();
			this._remainingInteractables.Clear();
			foreach (Interactable interactable in this._currInteractableToCollisionInfos.Keys)
			{
				if (!this._prevInteractableToCollisionInfos.ContainsKey(interactable))
				{
					this._addedInteractables.Add(interactable);
				}
				else
				{
					this._remainingInteractables.Add(interactable);
				}
			}
			foreach (Interactable interactable2 in this._prevInteractableToCollisionInfos.Keys)
			{
				if (!this._currInteractableToCollisionInfos.ContainsKey(interactable2))
				{
					this._removedInteractables.Add(interactable2);
				}
			}
			foreach (Interactable interactable3 in this._removedInteractables)
			{
				interactable3.UpdateCollisionDepth(this, this._prevInteractableToCollisionInfos[interactable3].CollisionDepth, InteractableCollisionDepth.None);
			}
			foreach (Interactable interactable4 in this._addedInteractables)
			{
				InteractableCollisionDepth collisionDepth = this._currInteractableToCollisionInfos[interactable4].CollisionDepth;
				interactable4.UpdateCollisionDepth(this, InteractableCollisionDepth.None, collisionDepth);
			}
			foreach (Interactable interactable5 in this._remainingInteractables)
			{
				InteractableCollisionDepth collisionDepth2 = this._currInteractableToCollisionInfos[interactable5].CollisionDepth;
				InteractableCollisionDepth collisionDepth3 = this._prevInteractableToCollisionInfos[interactable5].CollisionDepth;
				interactable5.UpdateCollisionDepth(this, collisionDepth3, collisionDepth2);
			}
			this._prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(this._currInteractableToCollisionInfos);
		}

		// Token: 0x04005C64 RID: 23652
		protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

		// Token: 0x04005C65 RID: 23653
		private List<Interactable> _addedInteractables = new List<Interactable>();

		// Token: 0x04005C66 RID: 23654
		private List<Interactable> _removedInteractables = new List<Interactable>();

		// Token: 0x04005C67 RID: 23655
		private List<Interactable> _remainingInteractables = new List<Interactable>();

		// Token: 0x04005C68 RID: 23656
		private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

		// Token: 0x04005C69 RID: 23657
		private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();
	}
}
