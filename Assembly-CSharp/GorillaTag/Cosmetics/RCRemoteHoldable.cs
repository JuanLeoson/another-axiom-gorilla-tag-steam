using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F17 RID: 3863
	public class RCRemoteHoldable : TransferrableObject, ISnapTurnOverride
	{
		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06005F9E RID: 24478 RVA: 0x001E5788 File Offset: 0x001E3988
		public XRNode XRNode
		{
			get
			{
				return this.xrNode;
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06005F9F RID: 24479 RVA: 0x001E5790 File Offset: 0x001E3990
		public RCVehicle Vehicle
		{
			get
			{
				return this.targetVehicle;
			}
		}

		// Token: 0x06005FA0 RID: 24480 RVA: 0x001E5798 File Offset: 0x001E3998
		public bool TurnOverrideActive()
		{
			return base.gameObject.activeSelf && this.currentlyHeld && this.xrNode == XRNode.RightHand;
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x001E57BC File Offset: 0x001E39BC
		protected override void Awake()
		{
			base.Awake();
			this.initialJoystickRotation = this.joystickTransform.localRotation;
			this.initialTriggerRotation = this.triggerTransform.localRotation;
			if (this.buttonTransform != null)
			{
				this.initialButtonRotation = this.buttonTransform.localRotation;
				this.initialButtonPosition = this.buttonTransform.localPosition;
			}
		}

		// Token: 0x06005FA2 RID: 24482 RVA: 0x001E5824 File Offset: 0x001E3A24
		internal override void OnEnable()
		{
			base.OnEnable();
			if (!this._TryFindRemoteVehicle())
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this._events.IsNotNull() || base.gameObject.TryGetComponent<RubberDuckEvents>(out this._events))
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
				this._events.Activate += this.OnStartConnectionEvent;
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x06005FA3 RID: 24483 RVA: 0x001E5914 File Offset: 0x001E3B14
		internal override void OnDisable()
		{
			base.OnDisable();
			GorillaSnapTurn gorillaSnapTurn = (GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null;
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
			if (this.networkSync != null && this.networkSync.photonView.IsMine)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this._events.IsNotNull())
			{
				this._events.Activate -= this.OnStartConnectionEvent;
			}
		}

		// Token: 0x06005FA4 RID: 24484 RVA: 0x001E59B8 File Offset: 0x001E3BB8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			GorillaSnapTurn gorillaSnapTurn = (GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null;
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
		}

		// Token: 0x06005FA5 RID: 24485 RVA: 0x001E59F8 File Offset: 0x001E3BF8
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (PhotonNetwork.InRoom && this.networkSync != null && this.networkSync.photonView.Owner == null)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this.networkSync == null && PhotonNetwork.InRoom)
			{
				object[] data = new object[]
				{
					this.myIndex
				};
				GameObject gameObject = PhotonNetwork.Instantiate(this.networkSyncPrefabName, Vector3.zero, Quaternion.identity, 0, data);
				this.networkSync = ((gameObject != null) ? gameObject.GetComponent<RCCosmeticNetworkSync>() : null);
			}
			this.currentlyHeld = true;
			bool flag = grabbingHand == EquipmentInteractor.instance.rightHand;
			this.xrNode = (flag ? XRNode.RightHand : XRNode.LeftHand);
			GorillaSnapTurn component = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			if (flag)
			{
				component.SetTurningOverride(this);
			}
			else
			{
				component.UnsetTurningOverride(this);
			}
			if (this.targetVehicle != null)
			{
				this.targetVehicle.StartConnection(this, this.networkSync);
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(this.emptyArgs);
			}
		}

		// Token: 0x06005FA6 RID: 24486 RVA: 0x001E5B48 File Offset: 0x001E3D48
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.currentlyHeld = false;
			this.currentInput = default(RCRemoteHoldable.RCInput);
			if (this.targetVehicle != null)
			{
				this.targetVehicle.EndConnection();
			}
			this.joystickTransform.localRotation = this.initialJoystickRotation;
			this.triggerTransform.localRotation = this.initialTriggerRotation;
			GorillaTagger.Instance.GetComponent<GorillaSnapTurn>().UnsetTurningOverride(this);
			return true;
		}

		// Token: 0x06005FA7 RID: 24487 RVA: 0x001E5BC0 File Offset: 0x001E3DC0
		private void Update()
		{
			if (this.currentlyHeld)
			{
				this.currentInput.joystick = ControllerInputPoller.Primary2DAxis(this.xrNode);
				this.currentInput.trigger = ControllerInputPoller.TriggerFloat(this.xrNode);
				this.currentInput.buttons = (ControllerInputPoller.PrimaryButtonPress(this.xrNode) ? 1 : 0);
				if (this.targetVehicle != null)
				{
					this.targetVehicle.ApplyRemoteControlInput(this.currentInput);
				}
				this.joystickTransform.localRotation = this.initialJoystickRotation * Quaternion.Euler(this.joystickLeanDegrees * this.currentInput.joystick.y, 0f, -this.joystickLeanDegrees * this.currentInput.joystick.x);
				this.triggerTransform.localRotation = this.initialTriggerRotation * Quaternion.Euler(this.triggerPullDegrees * this.currentInput.trigger, 0f, 0f);
				if (this.buttonTransform != null)
				{
					this.buttonTransform.localPosition = this.initialButtonPosition + this.initialButtonRotation * new Vector3(0f, 0f, -this.buttonPressDepth * (float)((this.currentInput.buttons > 0) ? 1 : 0));
				}
			}
		}

		// Token: 0x06005FA8 RID: 24488 RVA: 0x001E5D1F File Offset: 0x001E3F1F
		public void OnStartConnectionEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x06005FA9 RID: 24489 RVA: 0x001E5D46 File Offset: 0x001E3F46
		public void WakeUpRemoteVehicle()
		{
			if (this.networkSync != null && this.targetVehicle.IsNotNull() && !this.targetVehicle.HasLocalAuthority)
			{
				this.targetVehicle.WakeUpRemote(this.networkSync);
			}
		}

		// Token: 0x06005FAA RID: 24490 RVA: 0x001E5D84 File Offset: 0x001E3F84
		private bool _TryFindRemoteVehicle()
		{
			if (this.targetVehicle != null)
			{
				return true;
			}
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent.IsNull())
			{
				Debug.LogError("RCRemoteHoldable: unable to find parent vrrig");
				return false;
			}
			CosmeticItemInstance cosmeticItemInstance = componentInParent.cosmeticsObjectRegistry.Cosmetic(base.name);
			int instanceID = base.gameObject.GetInstanceID();
			return this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.objects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.leftObjects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.rightObjects);
		}

		// Token: 0x06005FAB RID: 24491 RVA: 0x001E5E10 File Offset: 0x001E4010
		private bool _TryFindRemoteVehicle_InCosmeticInstanceArray(int thisGobjInstId, List<GameObject> gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.GetInstanceID() != thisGobjInstId)
				{
					this.targetVehicle = gameObject.GetComponentInChildren<RCVehicle>(true);
					if (this.targetVehicle != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04006AD9 RID: 27353
		[SerializeField]
		private Transform joystickTransform;

		// Token: 0x04006ADA RID: 27354
		[SerializeField]
		private Transform triggerTransform;

		// Token: 0x04006ADB RID: 27355
		[SerializeField]
		private Transform buttonTransform;

		// Token: 0x04006ADC RID: 27356
		private RCVehicle targetVehicle;

		// Token: 0x04006ADD RID: 27357
		private float joystickLeanDegrees = 30f;

		// Token: 0x04006ADE RID: 27358
		private float triggerPullDegrees = 40f;

		// Token: 0x04006ADF RID: 27359
		private float buttonPressDepth = 0.005f;

		// Token: 0x04006AE0 RID: 27360
		private Quaternion initialJoystickRotation;

		// Token: 0x04006AE1 RID: 27361
		private Quaternion initialTriggerRotation;

		// Token: 0x04006AE2 RID: 27362
		private Quaternion initialButtonRotation;

		// Token: 0x04006AE3 RID: 27363
		private Vector3 initialButtonPosition;

		// Token: 0x04006AE4 RID: 27364
		private bool currentlyHeld;

		// Token: 0x04006AE5 RID: 27365
		private XRNode xrNode;

		// Token: 0x04006AE6 RID: 27366
		private RCRemoteHoldable.RCInput currentInput;

		// Token: 0x04006AE7 RID: 27367
		[HideInInspector]
		public RCCosmeticNetworkSync networkSync;

		// Token: 0x04006AE8 RID: 27368
		private string networkSyncPrefabName = "RCCosmeticNetworkSync";

		// Token: 0x04006AE9 RID: 27369
		private RubberDuckEvents _events;

		// Token: 0x04006AEA RID: 27370
		private object[] emptyArgs = new object[0];

		// Token: 0x02000F18 RID: 3864
		public struct RCInput
		{
			// Token: 0x04006AEB RID: 27371
			public Vector2 joystick;

			// Token: 0x04006AEC RID: 27372
			public float trigger;

			// Token: 0x04006AED RID: 27373
			public byte buttons;
		}
	}
}
