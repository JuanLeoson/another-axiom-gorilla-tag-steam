using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F8 RID: 1272
public class GameBallPlayerLocal : MonoBehaviour
{
	// Token: 0x06001EF4 RID: 7924 RVA: 0x000A3998 File Offset: 0x000A1B98
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000A39E7 File Offset: 0x000A1BE7
	private void OnApplicationQuit()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000A39F3 File Offset: 0x000A1BF3
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000A39E7 File Offset: 0x000A1BE7
	private void OnDestroy()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x000A3A04 File Offset: 0x000A1C04
	public void OnUpdateInteract()
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000A3A50 File Offset: 0x000A1C50
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion data = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out data.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out data.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out data.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out data.angVelocity);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000A3ADC File Offset: 0x000A1CDC
	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000A3B20 File Offset: 0x000A1D20
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000A3B5D File Offset: 0x000A1D5D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000A3B6C File Offset: 0x000A1D6C
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000A3B94 File Offset: 0x000A1D94
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000A3BDC File Offset: 0x000A1DDC
	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
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
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 position2 = gameBall.transform.position;
				Vector3 vector = gameBall.transform.position - position;
				if (vector.sqrMagnitude > num2 * num2)
				{
					position2 = position + vector.normalized * num2;
				}
				object obj2 = obj;
				Vector3 localPosition = obj2.InverseTransformPoint(position2);
				Quaternion localRotation = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, localPosition, localRotation);
			}
		}
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000A3D68 File Offset: 0x000A1F68
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion rotation;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation2 = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation2 * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation2 * -(Quaternion.Inverse(rotation) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float d = Vector3.Dot(vector3, vector);
				vector = vector3 * d;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x000A3F20 File Offset: 0x000A2120
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x000A3F28 File Offset: 0x000A2128
	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x000A38E3 File Offset: 0x000A1AE3
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x000A38E9 File Offset: 0x000A1AE9
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x000A3F5B File Offset: 0x000A215B
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x000A3F77 File Offset: 0x000A2177
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x04002792 RID: 10130
	public GameBallPlayer gamePlayer;

	// Token: 0x04002793 RID: 10131
	private const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04002794 RID: 10132
	private GameBallPlayerLocal.HandData[] hands;

	// Token: 0x04002795 RID: 10133
	private GameBallPlayerLocal.InputData[] inputData;

	// Token: 0x04002796 RID: 10134
	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	// Token: 0x020004F9 RID: 1273
	private enum HandGrabState
	{
		// Token: 0x04002798 RID: 10136
		Empty,
		// Token: 0x04002799 RID: 10137
		Holding
	}

	// Token: 0x020004FA RID: 1274
	private struct HandData
	{
		// Token: 0x0400279A RID: 10138
		public GameBallPlayerLocal.HandGrabState grabState;

		// Token: 0x0400279B RID: 10139
		public bool gripWasHeld;

		// Token: 0x0400279C RID: 10140
		public double gripPressedTime;

		// Token: 0x0400279D RID: 10141
		public GameBallId grabbedGameBallId;
	}

	// Token: 0x020004FB RID: 1275
	public struct InputDataMotion
	{
		// Token: 0x0400279E RID: 10142
		public double time;

		// Token: 0x0400279F RID: 10143
		public Vector3 position;

		// Token: 0x040027A0 RID: 10144
		public Quaternion rotation;

		// Token: 0x040027A1 RID: 10145
		public Vector3 velocity;

		// Token: 0x040027A2 RID: 10146
		public Vector3 angVelocity;
	}

	// Token: 0x020004FC RID: 1276
	public class InputData
	{
		// Token: 0x06001F08 RID: 7944 RVA: 0x000A3F99 File Offset: 0x000A2199
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x000A3FB4 File Offset: 0x000A21B4
		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x000A3FE4 File Offset: 0x000A21E4
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x06001F0B RID: 7947 RVA: 0x000A4060 File Offset: 0x000A2260
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 a = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x040027A3 RID: 10147
		public int maxInputs;

		// Token: 0x040027A4 RID: 10148
		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
