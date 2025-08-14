using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D06 RID: 3334
	public class ButtonController : Interactable
	{
		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x0600527B RID: 21115 RVA: 0x00199ED2 File Offset: 0x001980D2
		public override int ValidToolTagsMask
		{
			get
			{
				return this._toolTagsMask;
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x0600527C RID: 21116 RVA: 0x00199EDA File Offset: 0x001980DA
		public Vector3 LocalButtonDirection
		{
			get
			{
				return this._localButtonDirection;
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x0600527D RID: 21117 RVA: 0x00199EE2 File Offset: 0x001980E2
		// (set) Token: 0x0600527E RID: 21118 RVA: 0x00199EEA File Offset: 0x001980EA
		public InteractableState CurrentButtonState { get; private set; }

		// Token: 0x0600527F RID: 21119 RVA: 0x00199EF4 File Offset: 0x001980F4
		protected override void Awake()
		{
			base.Awake();
			foreach (InteractableToolTags interactableToolTags in this._allValidToolsTags)
			{
				this._toolTagsMask |= (int)interactableToolTags;
			}
			this._proximityZoneCollider = this._proximityZone.GetComponent<ColliderZone>();
			this._contactZoneCollider = this._contactZone.GetComponent<ColliderZone>();
			this._actionZoneCollider = this._actionZone.GetComponent<ColliderZone>();
		}

		// Token: 0x06005280 RID: 21120 RVA: 0x00199F64 File Offset: 0x00198164
		private void FireInteractionEventsOnDepth(InteractableCollisionDepth oldDepth, InteractableTool collidingTool, InteractionType interactionType)
		{
			switch (oldDepth)
			{
			case InteractableCollisionDepth.Proximity:
				this.OnProximityZoneEvent(new ColliderZoneArgs(base.ProximityCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Contact:
				this.OnContactZoneEvent(new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Action:
				this.OnActionZoneEvent(new ColliderZoneArgs(base.ActionCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			default:
				return;
			}
		}

		// Token: 0x06005281 RID: 21121 RVA: 0x00199FD4 File Offset: 0x001981D4
		public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
		{
			bool isFarFieldTool = interactableTool.IsFarFieldTool;
			if (!isFarFieldTool && !this._allowMultipleNearFieldInteraction && this._toolToState.Keys.Count > 0 && !this._toolToState.ContainsKey(interactableTool))
			{
				return;
			}
			InteractableState currentButtonState = this.CurrentButtonState;
			Vector3 vector = base.transform.TransformDirection(this._localButtonDirection);
			bool validContact = this.IsValidContact(interactableTool, vector) || interactableTool.IsFarFieldTool;
			bool toolIsInProximity = newCollisionDepth >= InteractableCollisionDepth.Proximity;
			bool flag = newCollisionDepth == InteractableCollisionDepth.Contact;
			bool flag2 = newCollisionDepth == InteractableCollisionDepth.Action;
			bool flag3 = oldCollisionDepth != newCollisionDepth;
			if (flag3)
			{
				this.FireInteractionEventsOnDepth(oldCollisionDepth, interactableTool, InteractionType.Exit);
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Enter);
			}
			else
			{
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Stay);
			}
			InteractableState interactableState = currentButtonState;
			if (interactableTool.IsFarFieldTool)
			{
				interactableState = (flag ? InteractableState.ContactState : (flag2 ? InteractableState.ActionState : InteractableState.Default));
			}
			else
			{
				Plane plane = new Plane(-vector, this._buttonPlaneCenter.position);
				bool onPositiveSideOfInteractable = !this._makeSureToolIsOnPositiveSide || plane.GetSide(interactableTool.InteractionPosition);
				interactableState = this.GetUpcomingStateNearField(currentButtonState, newCollisionDepth, flag2, flag, toolIsInProximity, validContact, onPositiveSideOfInteractable);
			}
			if (interactableState != InteractableState.Default)
			{
				this._toolToState[interactableTool] = interactableState;
			}
			else
			{
				this._toolToState.Remove(interactableTool);
			}
			if (isFarFieldTool || this._allowMultipleNearFieldInteraction)
			{
				foreach (InteractableState interactableState2 in this._toolToState.Values)
				{
					if (interactableState < interactableState2)
					{
						interactableState = interactableState2;
					}
				}
			}
			if (currentButtonState != interactableState)
			{
				this.CurrentButtonState = interactableState;
				InteractionType interactionType = (!flag3) ? InteractionType.Stay : ((newCollisionDepth == InteractableCollisionDepth.None) ? InteractionType.Exit : InteractionType.Enter);
				ColliderZone collider;
				switch (this.CurrentButtonState)
				{
				case InteractableState.ProximityState:
					collider = base.ProximityCollider;
					break;
				case InteractableState.ContactState:
					collider = base.ContactCollider;
					break;
				case InteractableState.ActionState:
					collider = base.ActionCollider;
					break;
				default:
					collider = null;
					break;
				}
				Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
				if (interactableStateChanged == null)
				{
					return;
				}
				interactableStateChanged.Invoke(new InteractableStateArgs(this, interactableTool, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(collider, (float)Time.frameCount, interactableTool, interactionType)));
			}
		}

		// Token: 0x06005282 RID: 21122 RVA: 0x0019A1FC File Offset: 0x001983FC
		private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
		{
			InteractableState result = oldState;
			switch (oldState)
			{
			case InteractableState.Default:
				if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					result = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				else if (toolIsInProximity)
				{
					result = InteractableState.ProximityState;
				}
				break;
			case InteractableState.ProximityState:
				if (newCollisionDepth < InteractableCollisionDepth.Proximity)
				{
					result = InteractableState.Default;
				}
				else if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					result = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				break;
			case InteractableState.ContactState:
				if (newCollisionDepth < InteractableCollisionDepth.Contact)
				{
					result = (toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default);
				}
				else if (toolIsInActionZone && validContact && onPositiveSideOfInteractable)
				{
					result = InteractableState.ActionState;
				}
				break;
			case InteractableState.ActionState:
				if (!toolIsInActionZone)
				{
					if (toolIsInContactZone)
					{
						result = InteractableState.ContactState;
					}
					else if (toolIsInProximity)
					{
						result = InteractableState.ProximityState;
					}
					else
					{
						result = InteractableState.Default;
					}
				}
				break;
			}
			return result;
		}

		// Token: 0x06005283 RID: 21123 RVA: 0x0019A294 File Offset: 0x00198494
		public void ForceResetButton()
		{
			InteractableState currentButtonState = this.CurrentButtonState;
			this.CurrentButtonState = InteractableState.Default;
			Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
			if (interactableStateChanged == null)
			{
				return;
			}
			interactableStateChanged.Invoke(new InteractableStateArgs(this, null, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, null, InteractionType.Exit)));
		}

		// Token: 0x06005284 RID: 21124 RVA: 0x0019A2E0 File Offset: 0x001984E0
		private bool IsValidContact(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			if (this._contactTests == null || collidingTool.IsFarFieldTool)
			{
				return true;
			}
			ButtonController.ContactTest[] contactTests = this._contactTests;
			for (int i = 0; i < contactTests.Length; i++)
			{
				if (contactTests[i] == ButtonController.ContactTest.BackwardsPress)
				{
					if (!this.PassEntryTest(collidingTool, buttonDirection))
					{
						return false;
					}
				}
				else if (!this.PassPerpTest(collidingTool, buttonDirection))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005285 RID: 21125 RVA: 0x0019A334 File Offset: 0x00198534
		private bool PassEntryTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			return Vector3.Dot(collidingTool.Velocity.normalized, buttonDirection) >= 0.8f;
		}

		// Token: 0x06005286 RID: 21126 RVA: 0x0019A360 File Offset: 0x00198560
		private bool PassPerpTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			Vector3 vector = collidingTool.ToolTransform.right;
			if (collidingTool.IsRightHandedTool)
			{
				vector = -vector;
			}
			return Vector3.Dot(vector, buttonDirection) >= 0.5f;
		}

		// Token: 0x04005BE6 RID: 23526
		private const float ENTRY_DOT_THRESHOLD = 0.8f;

		// Token: 0x04005BE7 RID: 23527
		private const float PERP_DOT_THRESHOLD = 0.5f;

		// Token: 0x04005BE8 RID: 23528
		[SerializeField]
		private GameObject _proximityZone;

		// Token: 0x04005BE9 RID: 23529
		[SerializeField]
		private GameObject _contactZone;

		// Token: 0x04005BEA RID: 23530
		[SerializeField]
		private GameObject _actionZone;

		// Token: 0x04005BEB RID: 23531
		[SerializeField]
		private ButtonController.ContactTest[] _contactTests;

		// Token: 0x04005BEC RID: 23532
		[SerializeField]
		private Transform _buttonPlaneCenter;

		// Token: 0x04005BED RID: 23533
		[SerializeField]
		private bool _makeSureToolIsOnPositiveSide = true;

		// Token: 0x04005BEE RID: 23534
		[SerializeField]
		private Vector3 _localButtonDirection = Vector3.down;

		// Token: 0x04005BEF RID: 23535
		[SerializeField]
		private InteractableToolTags[] _allValidToolsTags = new InteractableToolTags[]
		{
			InteractableToolTags.All
		};

		// Token: 0x04005BF0 RID: 23536
		private int _toolTagsMask;

		// Token: 0x04005BF1 RID: 23537
		[SerializeField]
		private bool _allowMultipleNearFieldInteraction;

		// Token: 0x04005BF3 RID: 23539
		private Dictionary<InteractableTool, InteractableState> _toolToState = new Dictionary<InteractableTool, InteractableState>();

		// Token: 0x02000D07 RID: 3335
		public enum ContactTest
		{
			// Token: 0x04005BF5 RID: 23541
			PerpenTest,
			// Token: 0x04005BF6 RID: 23542
			BackwardsPress
		}
	}
}
