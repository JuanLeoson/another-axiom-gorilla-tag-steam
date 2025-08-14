using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000912 RID: 2322
public class HandRayController : MonoBehaviour
{
	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x0600394F RID: 14671 RVA: 0x001293AE File Offset: 0x001275AE
	public static HandRayController Instance
	{
		get
		{
			if (HandRayController.instance == null)
			{
				HandRayController.instance = Object.FindObjectOfType<HandRayController>();
				if (HandRayController.instance == null)
				{
					Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Not found in scene", Array.Empty<object>());
				}
			}
			return HandRayController.instance;
		}
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x001293E8 File Offset: 0x001275E8
	private void Awake()
	{
		if (HandRayController.instance != null)
		{
			Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Duplicate instance of HandRayController", Array.Empty<object>());
			Object.DestroyImmediate(this);
			return;
		}
		HandRayController.instance = this;
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x00129414 File Offset: 0x00127614
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x00129474 File Offset: 0x00127674
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x0012947C File Offset: 0x0012767C
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x001294BC File Offset: 0x001276BC
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.HideHands();
		}
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x001294FC File Offset: 0x001276FC
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x06003956 RID: 14678 RVA: 0x0012951B File Offset: 0x0012771B
	private void PostUpdate()
	{
		if (!this._hasInitialised)
		{
			return;
		}
		if (this.ActiveHand == HandRayController.HandSide.Left)
		{
			if (ControllerBehaviour.Instance.RightButtonDown)
			{
				this.ToggleHands();
			}
			return;
		}
		if (ControllerBehaviour.Instance.LeftButtonDown)
		{
			this.ToggleHands();
		}
	}

	// Token: 0x06003957 RID: 14679 RVA: 0x00129554 File Offset: 0x00127754
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x06003958 RID: 14680 RVA: 0x001295B0 File Offset: 0x001277B0
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x0012960C File Offset: 0x0012780C
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x0600395A RID: 14682 RVA: 0x00129640 File Offset: 0x00127840
	private void ToggleHands()
	{
		if (!this._hasInitialised)
		{
			this.InitialiseHands();
			return;
		}
		HandRayController.HandSide handSide = (this.ActiveHand == HandRayController.HandSide.Left) ? HandRayController.HandSide.Right : HandRayController.HandSide.Left;
		Debug.LogFormat(string.Concat(new string[]
		{
			"[KID::UI::HAND_RAY_CONTROLLER] Setting ActiveHand FROM: [",
			this.ActiveHand.ToString(),
			"] TO: [",
			handSide.ToString(),
			"]"
		}), Array.Empty<object>());
		this.ActiveHand = handSide;
		this.ToggleRightHandRay(handSide == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(handSide == HandRayController.HandSide.Left);
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x001296D5 File Offset: 0x001278D5
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x04004673 RID: 18035
	private static HandRayController instance;

	// Token: 0x04004674 RID: 18036
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x04004675 RID: 18037
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x04004676 RID: 18038
	private bool _hasInitialised;

	// Token: 0x04004677 RID: 18039
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x04004678 RID: 18040
	private XRRayInteractor _activeHandRay;

	// Token: 0x04004679 RID: 18041
	private int _activationCounter;

	// Token: 0x02000913 RID: 2323
	private enum HandSide
	{
		// Token: 0x0400467B RID: 18043
		Left,
		// Token: 0x0400467C RID: 18044
		Right
	}
}
