using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005C2 RID: 1474
public class GamePlayerLocal : MonoBehaviour
{
	// Token: 0x0600243B RID: 9275 RVA: 0x000C1D24 File Offset: 0x000BFF24
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000C1D74 File Offset: 0x000BFF74
	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(this.currGameEntityManager, j);
		}
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000C1DBC File Offset: 0x000BFFBC
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion data = default(GamePlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out data.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out data.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out data.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out data.angVelocity);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000C1E48 File Offset: 0x000C0048
	private void UpdateHand(GameEntityManager gameEntityManager, int handIndex)
	{
		if (!this.gamePlayer.GetGameEntityId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(gameEntityManager, handIndex);
			return;
		}
		this.UpdateHandHolding(gameEntityManager, handIndex);
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x000C1E7C File Offset: 0x000C007C
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		if (handIndex == 1)
		{
			EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
		}
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000C1EFC File Offset: 0x000C00FC
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000C1F35 File Offset: 0x000C0135
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000C1F44 File Offset: 0x000C0144
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameEntityId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x000C1F8C File Offset: 0x000C018C
	private void UpdateHandEmpty(GameEntityManager gameEntityManager, int handIndex)
	{
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		if (gameEntityManager == null)
		{
			return;
		}
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Transform handTransform = this.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Quaternion rotation = handTransform.rotation;
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position);
			if (gameEntityId.IsValid())
			{
				bool isLeftHand = GamePlayerLocal.IsLeftHand(handIndex);
				Transform handTransform2 = this.GetHandTransform(handIndex);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 position2 = gameEntity.transform.position;
				Quaternion rotation2 = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				if (component != null)
				{
					GameGrab gameGrab;
					component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab);
					position2 = gameGrab.position;
					rotation2 = gameGrab.rotation;
				}
				Vector3 localPosition = handTransform2.InverseTransformPoint(position2);
				Quaternion localRotation = Quaternion.Inverse(handTransform2.rotation) * rotation2;
				handTransform2.InverseTransformPoint(gameEntity.transform.position);
				gameEntityManager.RequestGrabEntity(gameEntityId, isLeftHand, localPosition, localRotation);
			}
		}
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x000C20F0 File Offset: 0x000C02F0
	private void UpdateHandHolding(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			return;
		}
		XRNode xrnode = this.GetXRNode(handIndex);
		if (this.gamePlayer.IsGrabbingDisabled() || ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion rotation;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation2 = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation2 * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation2 * -(Quaternion.Inverse(rotation) * vector);
			GameEntityId gameEntityId = this.gamePlayer.GetGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			gameEntityManager.RequestThrowEntity(gameEntityId, GamePlayerLocal.IsLeftHand(handIndex), GTPlayer.Instance.HeadCenterPosition, vector2, vector);
		}
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000A3F20 File Offset: 0x000A2120
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000C2239 File Offset: 0x000C0439
	private Transform GetHandTransform(int handIndex)
	{
		return GamePlayer.GetHandTransform(GorillaTagger.Instance.offlineVRRig, handIndex);
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x000C224C File Offset: 0x000C044C
	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x000C22C3 File Offset: 0x000C04C3
	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x000A38E3 File Offset: 0x000A1AE3
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000A38E9 File Offset: 0x000A1AE9
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x000A3F5B File Offset: 0x000A215B
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000A3F77 File Offset: 0x000A2177
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x04002DA4 RID: 11684
	public GamePlayer gamePlayer;

	// Token: 0x04002DA5 RID: 11685
	private GamePlayerLocal.HandData[] hands;

	// Token: 0x04002DA6 RID: 11686
	public const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04002DA7 RID: 11687
	private GamePlayerLocal.InputData[] inputData;

	// Token: 0x04002DA8 RID: 11688
	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	// Token: 0x04002DA9 RID: 11689
	[NonSerialized]
	public GameEntityManager currGameEntityManager;

	// Token: 0x020005C3 RID: 1475
	private enum HandGrabState
	{
		// Token: 0x04002DAB RID: 11691
		Empty,
		// Token: 0x04002DAC RID: 11692
		Holding
	}

	// Token: 0x020005C4 RID: 1476
	private struct HandData
	{
		// Token: 0x04002DAD RID: 11693
		public GamePlayerLocal.HandGrabState grabState;

		// Token: 0x04002DAE RID: 11694
		public bool gripWasHeld;

		// Token: 0x04002DAF RID: 11695
		public double gripPressedTime;

		// Token: 0x04002DB0 RID: 11696
		public GameEntityId grabbedGameBallId;
	}

	// Token: 0x020005C5 RID: 1477
	public struct InputDataMotion
	{
		// Token: 0x04002DB1 RID: 11697
		public double time;

		// Token: 0x04002DB2 RID: 11698
		public Vector3 position;

		// Token: 0x04002DB3 RID: 11699
		public Quaternion rotation;

		// Token: 0x04002DB4 RID: 11700
		public Vector3 velocity;

		// Token: 0x04002DB5 RID: 11701
		public Vector3 angVelocity;
	}

	// Token: 0x020005C6 RID: 1478
	public class InputData
	{
		// Token: 0x0600244E RID: 9294 RVA: 0x000C22DC File Offset: 0x000C04DC
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x000C22F7 File Offset: 0x000C04F7
		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06002450 RID: 9296 RVA: 0x000C2324 File Offset: 0x000C0524
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x000C23A0 File Offset: 0x000C05A0
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 a = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					a += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return a / (float)num3;
		}

		// Token: 0x04002DB6 RID: 11702
		public int maxInputs;

		// Token: 0x04002DB7 RID: 11703
		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
