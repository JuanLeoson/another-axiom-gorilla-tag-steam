using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D22 RID: 3362
	public class RayTool : InteractableTool
	{
		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06005323 RID: 21283 RVA: 0x0001D558 File Offset: 0x0001B758
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Ray;
			}
		}

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06005324 RID: 21284 RVA: 0x0019BCCD File Offset: 0x00199ECD
		public override ToolInputState ToolInputState
		{
			get
			{
				if (this._pinchStateModule.PinchDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDown;
				}
				if (this._pinchStateModule.PinchSteadyOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDownStay;
				}
				if (this._pinchStateModule.PinchUpAndDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputUp;
				}
				return ToolInputState.Inactive;
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x06005325 RID: 21285 RVA: 0x0001D558 File Offset: 0x0001B758
		public override bool IsFarFieldTool
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x06005326 RID: 21286 RVA: 0x0019BCFD File Offset: 0x00199EFD
		// (set) Token: 0x06005327 RID: 21287 RVA: 0x0019BD0A File Offset: 0x00199F0A
		public override bool EnableState
		{
			get
			{
				return this._rayToolView.EnableState;
			}
			set
			{
				this._rayToolView.EnableState = value;
			}
		}

		// Token: 0x06005328 RID: 21288 RVA: 0x0019BD18 File Offset: 0x00199F18
		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._rayToolView.InteractableTool = this;
			this._coneAngleReleaseDegrees = this._coneAngleDegrees * 1.2f;
			this._initialized = true;
		}

		// Token: 0x06005329 RID: 21289 RVA: 0x0019BD4A File Offset: 0x00199F4A
		private void OnDestroy()
		{
			if (InteractableToolsInputRouter.Instance != null)
			{
				InteractableToolsInputRouter.Instance.UnregisterInteractableTool(this);
			}
		}

		// Token: 0x0600532A RID: 21290 RVA: 0x0019BD64 File Offset: 0x00199F64
		private void Update()
		{
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._initialized)
			{
				return;
			}
			OVRHand ovrhand = base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand;
			Transform pointerPose = ovrhand.PointerPose;
			base.transform.position = pointerPose.position;
			base.transform.rotation = pointerPose.rotation;
			Vector3 interactionPosition = base.InteractionPosition;
			Vector3 position = base.transform.position;
			base.Velocity = (position - interactionPosition) / Time.deltaTime;
			base.InteractionPosition = position;
			this._pinchStateModule.UpdateState(ovrhand, this._focusedInteractable);
			this._rayToolView.ToolActivateState = (this._pinchStateModule.PinchSteadyOnFocusedObject || this._pinchStateModule.PinchDownOnFocusedObject);
		}

		// Token: 0x0600532B RID: 21291 RVA: 0x0019BE43 File Offset: 0x0019A043
		private Vector3 GetRayCastOrigin()
		{
			return base.transform.position + 0.8f * base.transform.forward;
		}

		// Token: 0x0600532C RID: 21292 RVA: 0x0019BE6C File Offset: 0x0019A06C
		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			if (!this._initialized)
			{
				return this._currentIntersectingObjects;
			}
			if (this._currInteractableCastedAgainst != null && this.HasRayReleasedInteractable(this._currInteractableCastedAgainst))
			{
				this._currInteractableCastedAgainst = null;
			}
			if (this._currInteractableCastedAgainst == null)
			{
				this._currentIntersectingObjects.Clear();
				this._currInteractableCastedAgainst = this.FindTargetInteractable();
				if (this._currInteractableCastedAgainst != null)
				{
					int num = Physics.OverlapSphereNonAlloc(this._currInteractableCastedAgainst.transform.position, 0.01f, this._collidersOverlapped);
					for (int i = 0; i < num; i++)
					{
						ColliderZone component = this._collidersOverlapped[i].GetComponent<ColliderZone>();
						if (component != null)
						{
							Interactable parentInteractable = component.ParentInteractable;
							if (!(parentInteractable == null) && !(parentInteractable != this._currInteractableCastedAgainst))
							{
								InteractableCollisionInfo item = new InteractableCollisionInfo(component, component.CollisionDepth, this);
								this._currentIntersectingObjects.Add(item);
							}
						}
					}
					if (this._currentIntersectingObjects.Count == 0)
					{
						this._currInteractableCastedAgainst = null;
					}
				}
			}
			return this._currentIntersectingObjects;
		}

		// Token: 0x0600532D RID: 21293 RVA: 0x0019BF78 File Offset: 0x0019A178
		private bool HasRayReleasedInteractable(Interactable focusedInteractable)
		{
			Vector3 position = base.transform.position;
			Vector3 forward = base.transform.forward;
			float num = Mathf.Cos(this._coneAngleReleaseDegrees * 0.017453292f);
			Vector3 lhs = focusedInteractable.transform.position - position;
			lhs.Normalize();
			return Vector3.Dot(lhs, forward) < num;
		}

		// Token: 0x0600532E RID: 21294 RVA: 0x0019BFD4 File Offset: 0x0019A1D4
		private Interactable FindTargetInteractable()
		{
			Vector3 rayCastOrigin = this.GetRayCastOrigin();
			Vector3 forward = base.transform.forward;
			Interactable interactable = this.FindPrimaryRaycastHit(rayCastOrigin, forward);
			if (interactable == null)
			{
				interactable = this.FindInteractableViaConeTest(rayCastOrigin, forward);
			}
			return interactable;
		}

		// Token: 0x0600532F RID: 21295 RVA: 0x0019C014 File Offset: 0x0019A214
		private Interactable FindPrimaryRaycastHit(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			int num = Physics.RaycastNonAlloc(new Ray(rayOrigin, rayDirection), this._primaryHits, float.PositiveInfinity);
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this._primaryHits[i];
				ColliderZone component = raycastHit.transform.GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						float magnitude = (parentInteractable.transform.position - rayOrigin).magnitude;
						if (interactable == null || magnitude < num2)
						{
							interactable = parentInteractable;
							num2 = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		// Token: 0x06005330 RID: 21296 RVA: 0x0019C0C4 File Offset: 0x0019A2C4
		private Interactable FindInteractableViaConeTest(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			float num = 0f;
			float num2 = Mathf.Cos(this._coneAngleDegrees * 0.017453292f);
			float num3 = Mathf.Tan(0.017453292f * this._coneAngleDegrees * 0.5f) * this._farFieldMaxDistance;
			int num4 = Physics.OverlapBoxNonAlloc(rayOrigin + rayDirection * this._farFieldMaxDistance * 0.5f, new Vector3(num3, num3, this._farFieldMaxDistance * 0.5f), this._secondaryOverlapResults, base.transform.rotation);
			for (int i = 0; i < num4; i++)
			{
				ColliderZone component = this._secondaryOverlapResults[i].GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						Vector3 vector = parentInteractable.transform.position - rayOrigin;
						float magnitude = vector.magnitude;
						vector /= magnitude;
						if (Vector3.Dot(vector, rayDirection) >= num2 && (interactable == null || magnitude < num))
						{
							interactable = parentInteractable;
							num = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		// Token: 0x06005331 RID: 21297 RVA: 0x0019C1E7 File Offset: 0x0019A3E7
		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
			this._rayToolView.SetFocusedInteractable(focusedInteractable);
			this._focusedInteractable = focusedInteractable;
		}

		// Token: 0x06005332 RID: 21298 RVA: 0x0019C1FC File Offset: 0x0019A3FC
		public override void DeFocus()
		{
			this._rayToolView.SetFocusedInteractable(null);
			this._focusedInteractable = null;
		}

		// Token: 0x04005C72 RID: 23666
		private const float MINIMUM_RAY_CAST_DISTANCE = 0.8f;

		// Token: 0x04005C73 RID: 23667
		private const float COLLIDER_RADIUS = 0.01f;

		// Token: 0x04005C74 RID: 23668
		private const int NUM_MAX_PRIMARY_HITS = 10;

		// Token: 0x04005C75 RID: 23669
		private const int NUM_MAX_SECONDARY_HITS = 25;

		// Token: 0x04005C76 RID: 23670
		private const int NUM_COLLIDERS_TO_TEST = 20;

		// Token: 0x04005C77 RID: 23671
		[SerializeField]
		private RayToolView _rayToolView;

		// Token: 0x04005C78 RID: 23672
		[Range(0f, 45f)]
		[SerializeField]
		private float _coneAngleDegrees = 20f;

		// Token: 0x04005C79 RID: 23673
		[SerializeField]
		private float _farFieldMaxDistance = 5f;

		// Token: 0x04005C7A RID: 23674
		private PinchStateModule _pinchStateModule = new PinchStateModule();

		// Token: 0x04005C7B RID: 23675
		private Interactable _focusedInteractable;

		// Token: 0x04005C7C RID: 23676
		private Collider[] _collidersOverlapped = new Collider[20];

		// Token: 0x04005C7D RID: 23677
		private Interactable _currInteractableCastedAgainst;

		// Token: 0x04005C7E RID: 23678
		private float _coneAngleReleaseDegrees;

		// Token: 0x04005C7F RID: 23679
		private RaycastHit[] _primaryHits = new RaycastHit[10];

		// Token: 0x04005C80 RID: 23680
		private Collider[] _secondaryOverlapResults = new Collider[25];

		// Token: 0x04005C81 RID: 23681
		private bool _initialized;
	}
}
