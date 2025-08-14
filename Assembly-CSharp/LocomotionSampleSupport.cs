using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200035F RID: 863
public class LocomotionSampleSupport : MonoBehaviour
{
	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001464 RID: 5220 RVA: 0x0006DD0C File Offset: 0x0006BF0C
	private LocomotionTeleport TeleportController
	{
		get
		{
			return this.lc.GetComponent<LocomotionTeleport>();
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x0006DD1C File Offset: 0x0006BF1C
	public void Start()
	{
		this.lc = Object.FindObjectOfType<LocomotionController>();
		DebugUIBuilder.instance.AddButton("Node Teleport w/ A", new DebugUIBuilder.OnClick(this.SetupNodeTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("Dual-stick teleport", new DebugUIBuilder.OnClick(this.SetupTwoStickTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("L Strafe R Teleport", new DebugUIBuilder.OnClick(this.SetupLeftStrafeRightTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("Walk Only", new DebugUIBuilder.OnClick(this.SetupWalkOnly), -1, 0, false);
		if (Object.FindObjectOfType<EventSystem>() == null)
		{
			Debug.LogError("Need EventSystem");
		}
		this.SetupTwoStickTeleport();
		Physics.IgnoreLayerCollision(0, 4);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0006DDD4 File Offset: 0x0006BFD4
	public void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0006DE2C File Offset: 0x0006C02C
	[Conditional("DEBUG_LOCOMOTION_PANEL")]
	private static void Log(string msg)
	{
		Debug.Log(msg);
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0006DE34 File Offset: 0x0006C034
	public static TActivate ActivateCategory<TCategory, TActivate>(GameObject target) where TCategory : MonoBehaviour where TActivate : MonoBehaviour
	{
		TCategory[] components = target.GetComponents<TCategory>();
		string[] array = new string[7];
		array[0] = "Activate ";
		int num = 1;
		Type typeFromHandle = typeof(TActivate);
		array[num] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
		array[2] = " derived from ";
		int num2 = 3;
		Type typeFromHandle2 = typeof(TCategory);
		array[num2] = ((typeFromHandle2 != null) ? typeFromHandle2.ToString() : null);
		array[4] = "[";
		array[5] = components.Length.ToString();
		array[6] = "]";
		LocomotionSampleSupport.Log(string.Concat(array));
		TActivate result = default(TActivate);
		foreach (TCategory monoBehaviour in components)
		{
			bool flag = monoBehaviour.GetType() == typeof(TActivate);
			string[] array2 = new string[5];
			int num3 = 0;
			Type type = monoBehaviour.GetType();
			array2[num3] = ((type != null) ? type.ToString() : null);
			array2[1] = " is ";
			int num4 = 2;
			Type typeFromHandle3 = typeof(TActivate);
			array2[num4] = ((typeFromHandle3 != null) ? typeFromHandle3.ToString() : null);
			array2[3] = " = ";
			array2[4] = flag.ToString();
			LocomotionSampleSupport.Log(string.Concat(array2));
			if (flag)
			{
				result = (TActivate)((object)monoBehaviour);
			}
			if (monoBehaviour.enabled != flag)
			{
				monoBehaviour.enabled = flag;
			}
		}
		return result;
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0006DF74 File Offset: 0x0006C174
	protected void ActivateHandlers<TInput, TAim, TTarget, TOrientation, TTransition>() where TInput : TeleportInputHandler where TAim : TeleportAimHandler where TTarget : TeleportTargetHandler where TOrientation : TeleportOrientationHandler where TTransition : TeleportTransition
	{
		this.ActivateInput<TInput>();
		this.ActivateAim<TAim>();
		this.ActivateTarget<TTarget>();
		this.ActivateOrientation<TOrientation>();
		this.ActivateTransition<TTransition>();
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0006DF94 File Offset: 0x0006C194
	protected void ActivateInput<TActivate>() where TActivate : TeleportInputHandler
	{
		this.ActivateCategory<TeleportInputHandler, TActivate>();
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0006DF9D File Offset: 0x0006C19D
	protected void ActivateAim<TActivate>() where TActivate : TeleportAimHandler
	{
		this.ActivateCategory<TeleportAimHandler, TActivate>();
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0006DFA6 File Offset: 0x0006C1A6
	protected void ActivateTarget<TActivate>() where TActivate : TeleportTargetHandler
	{
		this.ActivateCategory<TeleportTargetHandler, TActivate>();
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0006DFAF File Offset: 0x0006C1AF
	protected void ActivateOrientation<TActivate>() where TActivate : TeleportOrientationHandler
	{
		this.ActivateCategory<TeleportOrientationHandler, TActivate>();
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0006DFB8 File Offset: 0x0006C1B8
	protected void ActivateTransition<TActivate>() where TActivate : TeleportTransition
	{
		this.ActivateCategory<TeleportTransition, TActivate>();
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0006DFC1 File Offset: 0x0006C1C1
	protected TActivate ActivateCategory<TCategory, TActivate>() where TCategory : MonoBehaviour where TActivate : MonoBehaviour
	{
		return LocomotionSampleSupport.ActivateCategory<TCategory, TActivate>(this.lc.gameObject);
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0006DFD3 File Offset: 0x0006C1D3
	protected void UpdateToggle(Toggle toggle, bool enabled)
	{
		if (enabled != toggle.isOn)
		{
			toggle.isOn = enabled;
		}
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x0006DFE5 File Offset: 0x0006C1E5
	private void SetupNonCap()
	{
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport;
		component.AimButton = OVRInput.RawButton.A;
		component.TeleportButton = OVRInput.RawButton.A;
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x0006E008 File Offset: 0x0006C208
	private void SetupTeleportDefaults()
	{
		this.TeleportController.enabled = true;
		this.lc.PlayerController.RotationEitherThumbstick = false;
		this.TeleportController.EnableMovement(false, false, false, false);
		this.TeleportController.EnableRotation(false, false, false, false);
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.CapacitiveButtonForAimAndTeleport;
		component.AimButton = OVRInput.RawButton.A;
		component.TeleportButton = OVRInput.RawButton.A;
		component.CapacitiveAimAndTeleportButton = TeleportInputHandlerTouch.AimCapTouchButtons.A;
		component.FastTeleport = false;
		TeleportInputHandlerHMD component2 = this.TeleportController.GetComponent<TeleportInputHandlerHMD>();
		component2.AimButton = OVRInput.RawButton.A;
		component2.TeleportButton = OVRInput.RawButton.A;
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.LTouch;
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x0006E0A6 File Offset: 0x0006C2A6
	protected GameObject AddInstance(GameObject template, string label)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(template);
		gameObject.transform.SetParent(base.transform, false);
		gameObject.name = label;
		return gameObject;
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x0006E0C8 File Offset: 0x0006C2C8
	private void SetupNodeTeleport()
	{
		this.SetupTeleportDefaults();
		this.SetupNonCap();
		this.lc.PlayerController.RotationEitherThumbstick = true;
		this.TeleportController.EnableRotation(true, false, false, true);
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerLaser, TeleportTargetHandlerNode, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportInputHandlerTouch>().AimingController = OVRInput.Controller.RTouch;
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x0006E118 File Offset: 0x0006C318
	private void SetupTwoStickTeleport()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.EnableRotation(true, false, false, true);
		this.TeleportController.EnableMovement(false, false, false, false);
		this.lc.PlayerController.RotationEitherThumbstick = true;
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
		component.AimingController = OVRInput.Controller.Touch;
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.Touch;
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x0006E189 File Offset: 0x0006C389
	private void SetupWalkOnly()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.enabled = false;
		this.lc.PlayerController.EnableLinearMovement = true;
		this.lc.PlayerController.RotationEitherThumbstick = false;
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x0006E1C0 File Offset: 0x0006C3C0
	private void SetupLeftStrafeRightTeleport()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.EnableRotation(true, false, false, true);
		this.TeleportController.EnableMovement(true, false, false, false);
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
		component.AimingController = OVRInput.Controller.RTouch;
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.RTouch;
	}

	// Token: 0x04001BF3 RID: 7155
	private LocomotionController lc;

	// Token: 0x04001BF4 RID: 7156
	private bool inMenu;
}
