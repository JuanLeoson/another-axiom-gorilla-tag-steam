using System;
using UnityEngine;

// Token: 0x0200095B RID: 2395
[Obsolete("Use ControllerInputPoller instead", false)]
public class ControllerBehaviour : MonoBehaviour, IBuildValidation
{
	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x06003ABE RID: 15038 RVA: 0x0013039E File Offset: 0x0012E59E
	// (set) Token: 0x06003ABF RID: 15039 RVA: 0x001303A5 File Offset: 0x0012E5A5
	[OnEnterPlay_SetNull]
	public static ControllerBehaviour Instance { get; private set; }

	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x06003AC0 RID: 15040 RVA: 0x001303AD File Offset: 0x0012E5AD
	private ControllerInputPoller Poller
	{
		get
		{
			if (this.poller != null)
			{
				return this.poller;
			}
			if (ControllerInputPoller.instance != null)
			{
				this.poller = ControllerInputPoller.instance;
				return this.poller;
			}
			return null;
		}
	}

	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x06003AC1 RID: 15041 RVA: 0x001303E8 File Offset: 0x0012E5E8
	public bool ButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton);
		}
	}

	// Token: 0x170005A6 RID: 1446
	// (get) Token: 0x06003AC2 RID: 15042 RVA: 0x00130439 File Offset: 0x0012E639
	public bool LeftButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x170005A7 RID: 1447
	// (get) Token: 0x06003AC3 RID: 15043 RVA: 0x00130472 File Offset: 0x0012E672
	public bool RightButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x06003AC4 RID: 15044 RVA: 0x001304AC File Offset: 0x0012E6AC
	public bool IsLeftStick
	{
		get
		{
			return !(this.Poller == null) && (Mathf.Min(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) < -this.uxSettings.StickSensitvity || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x06003AC5 RID: 15045 RVA: 0x0013050C File Offset: 0x0012E70C
	public bool IsRightStick
	{
		get
		{
			return !(this.Poller == null) && (Mathf.Max(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) > this.uxSettings.StickSensitvity || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x06003AC6 RID: 15046 RVA: 0x00130568 File Offset: 0x0012E768
	public bool IsUpStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x06003AC7 RID: 15047 RVA: 0x001305B8 File Offset: 0x0012E7B8
	public bool IsDownStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x06003AC8 RID: 15048 RVA: 0x00130608 File Offset: 0x0012E808
	public float StickXValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.x), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.x));
			}
			return 0f;
		}
	}

	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x06003AC9 RID: 15049 RVA: 0x00130658 File Offset: 0x0012E858
	public float StickYValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.y), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.y));
			}
			return 0f;
		}
	}

	// Token: 0x170005AE RID: 1454
	// (get) Token: 0x06003ACA RID: 15050 RVA: 0x001306A8 File Offset: 0x0012E8A8
	public bool TriggerDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerTriggerButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x06003ACB RID: 15051 RVA: 0x001306D4 File Offset: 0x0012E8D4
	// (remove) Token: 0x06003ACC RID: 15052 RVA: 0x0013070C File Offset: 0x0012E90C
	public event ControllerBehaviour.OnActionEvent OnAction;

	// Token: 0x06003ACD RID: 15053 RVA: 0x00130741 File Offset: 0x0012E941
	private void Awake()
	{
		if (ControllerBehaviour.Instance != null)
		{
			Debug.LogError("[CONTROLLER_BEHAVIOUR] Trying to create new singleton but one already exists", base.gameObject);
			Object.DestroyImmediate(this);
			return;
		}
		ControllerBehaviour.Instance = this;
	}

	// Token: 0x06003ACE RID: 15054 RVA: 0x00130770 File Offset: 0x0012E970
	private void Update()
	{
		bool flag = (this.IsLeftStick && this.wasLeftStick) || (this.IsRightStick && this.wasRightStick) || (this.IsUpStick && this.wasUpStick) || (this.IsDownStick && this.wasDownStick);
		if (Time.time - this.actionTime < this.actionDelay / this.repeatAction)
		{
			return;
		}
		if (this.wasHeld && flag)
		{
			this.repeatAction += this.actionRepeatDelayReduction;
		}
		else
		{
			this.repeatAction = 1f;
		}
		if (this.IsLeftStick || this.IsRightStick || this.IsUpStick || this.IsDownStick || this.ButtonDown)
		{
			this.actionTime = Time.time;
		}
		if (this.OnAction != null)
		{
			this.OnAction();
		}
		this.wasHeld = flag;
		this.wasDownStick = this.IsDownStick;
		this.wasUpStick = this.IsUpStick;
		this.wasLeftStick = this.IsLeftStick;
		this.wasRightStick = this.IsRightStick;
	}

	// Token: 0x06003ACF RID: 15055 RVA: 0x00130885 File Offset: 0x0012EA85
	public bool BuildValidationCheck()
	{
		if (this.uxSettings == null)
		{
			Debug.LogError("ControllerBehaviour must set UXSettings");
			return false;
		}
		return true;
	}

	// Token: 0x06003AD0 RID: 15056 RVA: 0x001308A2 File Offset: 0x0012EAA2
	public static ControllerBehaviour CreateNewControllerBehaviour(GameObject gameObject, UXSettings settings)
	{
		ControllerBehaviour controllerBehaviour = gameObject.AddComponent<ControllerBehaviour>();
		controllerBehaviour.uxSettings = settings;
		return controllerBehaviour;
	}

	// Token: 0x0400482B RID: 18475
	private float actionTime;

	// Token: 0x0400482C RID: 18476
	private float repeatAction = 1f;

	// Token: 0x0400482D RID: 18477
	[SerializeField]
	private UXSettings uxSettings;

	// Token: 0x0400482E RID: 18478
	[SerializeField]
	private float actionDelay = 0.5f;

	// Token: 0x0400482F RID: 18479
	[SerializeField]
	private float actionRepeatDelayReduction = 0.5f;

	// Token: 0x04004830 RID: 18480
	[Tooltip("Should the triggers modify the x axis like the sticks do?")]
	[SerializeField]
	private bool useTriggersAsSticks;

	// Token: 0x04004831 RID: 18481
	private ControllerInputPoller poller;

	// Token: 0x04004832 RID: 18482
	private bool wasLeftStick;

	// Token: 0x04004833 RID: 18483
	private bool wasRightStick;

	// Token: 0x04004834 RID: 18484
	private bool wasUpStick;

	// Token: 0x04004835 RID: 18485
	private bool wasDownStick;

	// Token: 0x04004836 RID: 18486
	private bool wasHeld;

	// Token: 0x0200095C RID: 2396
	// (Invoke) Token: 0x06003AD3 RID: 15059
	public delegate void OnActionEvent();
}
