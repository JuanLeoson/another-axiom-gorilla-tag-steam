using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F20 RID: 3872
	public class DreidelHoldable : TransferrableObject
	{
		// Token: 0x06005FF2 RID: 24562 RVA: 0x001E7508 File Offset: 0x001E5708
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnDreidelSpin;
			}
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x001E75D8 File Offset: 0x001E57D8
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnDreidelSpin;
				Object.Destroy(this._events);
				this._events = null;
			}
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x001E7630 File Offset: 0x001E5830
		private void OnDreidelSpin(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "OnDreidelSpin");
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			Vector3 surfacePoint = (Vector3)args[0];
			Vector3 surfaceNormal = (Vector3)args[1];
			float num = (float)args[2];
			double num2 = (double)args[6];
			float num3 = 10000f;
			if (surfacePoint.IsValid(num3))
			{
				float num4 = 10000f;
				if (surfaceNormal.IsValid(num4) && float.IsFinite(num) && double.IsFinite(num2))
				{
					bool counterClockwise = (bool)args[3];
					Dreidel.Side side = (Dreidel.Side)args[4];
					Dreidel.Variation variation = (Dreidel.Variation)args[5];
					this.StartSpinLocal(surfacePoint, surfaceNormal, num, counterClockwise, side, variation, num2);
					return;
				}
			}
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x001E76EF File Offset: 0x001E58EF
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TryCheckForSurfaces();
			}
		}

		// Token: 0x06005FF6 RID: 24566 RVA: 0x001E7713 File Offset: 0x001E5913
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TrySetIdle();
			}
			return true;
		}

		// Token: 0x06005FF7 RID: 24567 RVA: 0x001E773C File Offset: 0x001E593C
		public override void OnActivate()
		{
			base.OnActivate();
			Vector3 vector;
			Vector3 vector2;
			float num;
			Dreidel.Side side;
			Dreidel.Variation variation;
			double num2;
			if (this.dreidelAnimation != null && this.dreidelAnimation.TryGetSpinStartData(out vector, out vector2, out num, out side, out variation, out num2))
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						vector,
						vector2,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(vector, vector2, num, flag, side, variation, num2);
			}
		}

		// Token: 0x06005FF8 RID: 24568 RVA: 0x001E781C File Offset: 0x001E5A1C
		private void StartSpinLocal(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.SetSpinStartData(surfacePoint, surfaceNormal, duration, counterClockwise, side, variation, startTime);
				this.dreidelAnimation.Spin();
			}
		}

		// Token: 0x06005FF9 RID: 24569 RVA: 0x001E7850 File Offset: 0x001E5A50
		public void DebugSpinDreidel()
		{
			Transform transform = GTPlayer.Instance.headCollider.transform;
			Vector3 origin = transform.position + transform.forward * 0.5f;
			float maxDistance = 2f;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, maxDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 point = raycastHit.point;
				Vector3 normal = raycastHit.normal;
				float num = Random.Range(7f, 10f);
				Dreidel.Side side = (Dreidel.Side)Random.Range(0, 4);
				Dreidel.Variation variation = (Dreidel.Variation)Random.Range(0, 5);
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				double num2 = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						point,
						normal,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(point, normal, num, flag, side, variation, num2);
			}
		}

		// Token: 0x04006B38 RID: 27448
		[SerializeField]
		private Dreidel dreidelAnimation;

		// Token: 0x04006B39 RID: 27449
		private RubberDuckEvents _events;
	}
}
