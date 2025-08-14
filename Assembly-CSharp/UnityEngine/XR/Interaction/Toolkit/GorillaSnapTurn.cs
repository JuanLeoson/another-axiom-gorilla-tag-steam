using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Valve.VR;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x02000CD6 RID: 3286
	public class GorillaSnapTurn : LocomotionProvider
	{
		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06005197 RID: 20887 RVA: 0x001969B3 File Offset: 0x00194BB3
		// (set) Token: 0x06005198 RID: 20888 RVA: 0x001969BB File Offset: 0x00194BBB
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06005199 RID: 20889 RVA: 0x001969C4 File Offset: 0x00194BC4
		// (set) Token: 0x0600519A RID: 20890 RVA: 0x001969CC File Offset: 0x00194BCC
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x0600519B RID: 20891 RVA: 0x001969D5 File Offset: 0x00194BD5
		// (set) Token: 0x0600519C RID: 20892 RVA: 0x001969DD File Offset: 0x00194BDD
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x0600519D RID: 20893 RVA: 0x001969E6 File Offset: 0x00194BE6
		// (set) Token: 0x0600519E RID: 20894 RVA: 0x001969EE File Offset: 0x00194BEE
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x0600519F RID: 20895 RVA: 0x001969F7 File Offset: 0x00194BF7
		// (set) Token: 0x060051A0 RID: 20896 RVA: 0x001969FF File Offset: 0x00194BFF
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x060051A1 RID: 20897 RVA: 0x00196A08 File Offset: 0x00194C08
		// (set) Token: 0x060051A2 RID: 20898 RVA: 0x00196A10 File Offset: 0x00194C10
		public string turnType
		{
			get
			{
				return this.m_TurnType;
			}
			private set
			{
				this.m_TurnType = value;
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x060051A3 RID: 20899 RVA: 0x00196A19 File Offset: 0x00194C19
		// (set) Token: 0x060051A4 RID: 20900 RVA: 0x00196A21 File Offset: 0x00194C21
		public int turnFactor
		{
			get
			{
				return this.m_TurnFactor;
			}
			private set
			{
				this.m_TurnFactor = value;
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x060051A5 RID: 20901 RVA: 0x00196A2A File Offset: 0x00194C2A
		public static GorillaSnapTurn CachedSnapTurnRef
		{
			get
			{
				if (GorillaSnapTurn._cachedReference == null)
				{
					Debug.LogError("[SNAP_TURN] Tried accessing static cached reference, but was still null. Trying to find component in scene");
					GorillaSnapTurn._cachedReference = Object.FindObjectOfType<GorillaSnapTurn>();
				}
				return GorillaSnapTurn._cachedReference;
			}
		}

		// Token: 0x060051A6 RID: 20902 RVA: 0x00196A52 File Offset: 0x00194C52
		protected override void Awake()
		{
			base.Awake();
			if (GorillaSnapTurn._cachedReference != null)
			{
				Debug.LogError("[SNAP_TURN] A [GorillaSnapTurn] component already exists in the scene");
				return;
			}
			GorillaSnapTurn._cachedReference = this;
		}

		// Token: 0x060051A7 RID: 20903 RVA: 0x00196A78 File Offset: 0x00194C78
		private void Update()
		{
			this.ValidateTurningOverriders();
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				InputFeatureUsage<Vector2>[] vec2UsageList = GorillaSnapTurn.m_Vec2UsageList;
				GorillaSnapTurn.InputAxes turnUsage = this.m_TurnUsage;
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (xrcontroller != null && xrcontroller.enableInputActions)
					{
						InputDevice inputDevice = xrcontroller.inputDevice;
						Vector2 vector = (xrcontroller.controllerNode == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
						if (vector.x > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (vector.x < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Math.Abs(this.m_CurrentTurnAmount) > 0f && base.BeginLocomotion())
			{
				if (base.system.xrRig != null)
				{
					GTPlayer.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.EndLocomotion();
			}
		}

		// Token: 0x060051A8 RID: 20904 RVA: 0x00196BA8 File Offset: 0x00194DA8
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x060051A9 RID: 20905 RVA: 0x00196C25 File Offset: 0x00194E25
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x060051AA RID: 20906 RVA: 0x00196C40 File Offset: 0x00194E40
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (!base.CanBeginLocomotion())
			{
				return;
			}
			if (this.turningOverriders.Count > 0)
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x060051AB RID: 20907 RVA: 0x00196C9C File Offset: 0x00194E9C
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			this.turnType = turnMode;
			this.turnFactor = turnSpeedFactor;
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x060051AC RID: 20908 RVA: 0x00196D2F File Offset: 0x00194F2F
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x060051AD RID: 20909 RVA: 0x00196D4E File Offset: 0x00194F4E
		public void SetTurningOverride(ISnapTurnOverride caller)
		{
			if (!this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Add(caller);
			}
		}

		// Token: 0x060051AE RID: 20910 RVA: 0x00196D6B File Offset: 0x00194F6B
		public void UnsetTurningOverride(ISnapTurnOverride caller)
		{
			if (this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Remove(caller);
			}
		}

		// Token: 0x060051AF RID: 20911 RVA: 0x00196D88 File Offset: 0x00194F88
		public void ValidateTurningOverriders()
		{
			foreach (ISnapTurnOverride snapTurnOverride in this.turningOverriders)
			{
				if (snapTurnOverride == null || !snapTurnOverride.TurnOverrideActive())
				{
					this.turningOverriders.Remove(snapTurnOverride);
				}
			}
		}

		// Token: 0x060051B0 RID: 20912 RVA: 0x00196DEC File Offset: 0x00194FEC
		public static void DisableSnapTurn()
		{
			Debug.Log("[SNAP_TURN] Disabling Snap Turn");
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			GorillaSnapTurn._cachedTurnFactor = PlayerPrefs.GetInt("turnFactor");
			GorillaSnapTurn._cachedTurnType = PlayerPrefs.GetString("stickTurning");
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode("NONE", 0);
		}

		// Token: 0x060051B1 RID: 20913 RVA: 0x00196E3F File Offset: 0x0019503F
		public static void UpdateAndSaveTurnType(string mode)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetString("stickTurning", mode);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(mode, GorillaSnapTurn.CachedSnapTurnRef.turnFactor);
		}

		// Token: 0x060051B2 RID: 20914 RVA: 0x00196E7E File Offset: 0x0019507E
		public static void UpdateAndSaveTurnFactor(int factor)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetInt("turnFactor", factor);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(GorillaSnapTurn.CachedSnapTurnRef.turnType, factor);
		}

		// Token: 0x060051B3 RID: 20915 RVA: 0x00196EC0 File Offset: 0x001950C0
		public static void LoadSettingsFromPlayerPrefs()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			string defaultValue = (Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP";
			string @string = PlayerPrefs.GetString("stickTurning", defaultValue);
			int @int = PlayerPrefs.GetInt("turnFactor", 4);
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(@string, @int);
		}

		// Token: 0x060051B4 RID: 20916 RVA: 0x00196F18 File Offset: 0x00195118
		public static void LoadSettingsFromCache()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(GorillaSnapTurn._cachedTurnType))
			{
				GorillaSnapTurn._cachedTurnType = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			}
			string cachedTurnType = GorillaSnapTurn._cachedTurnType;
			int cachedTurnFactor = GorillaSnapTurn._cachedTurnFactor;
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(cachedTurnType, cachedTurnFactor);
		}

		// Token: 0x04005B19 RID: 23321
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x04005B1A RID: 23322
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x04005B1B RID: 23323
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x04005B1C RID: 23324
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x04005B1D RID: 23325
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x04005B1E RID: 23326
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x04005B1F RID: 23327
		private float m_CurrentTurnAmount;

		// Token: 0x04005B20 RID: 23328
		private float m_TimeStarted;

		// Token: 0x04005B21 RID: 23329
		private bool m_AxisReset;

		// Token: 0x04005B22 RID: 23330
		public float turnSpeed = 1f;

		// Token: 0x04005B23 RID: 23331
		private HashSet<ISnapTurnOverride> turningOverriders = new HashSet<ISnapTurnOverride>();

		// Token: 0x04005B24 RID: 23332
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x04005B25 RID: 23333
		private static int _cachedTurnFactor;

		// Token: 0x04005B26 RID: 23334
		private static string _cachedTurnType;

		// Token: 0x04005B27 RID: 23335
		private string m_TurnType = "";

		// Token: 0x04005B28 RID: 23336
		private int m_TurnFactor = 1;

		// Token: 0x04005B29 RID: 23337
		[OnEnterPlay_SetNull]
		private static GorillaSnapTurn _cachedReference;

		// Token: 0x02000CD7 RID: 3287
		public enum InputAxes
		{
			// Token: 0x04005B2B RID: 23339
			Primary2DAxis,
			// Token: 0x04005B2C RID: 23340
			Secondary2DAxis
		}
	}
}
