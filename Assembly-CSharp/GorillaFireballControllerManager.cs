using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020007F7 RID: 2039
public class GorillaFireballControllerManager : MonoBehaviour
{
	// Token: 0x06003309 RID: 13065 RVA: 0x001096E0 File Offset: 0x001078E0
	private void Update()
	{
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			List<InputDevice> list = new List<InputDevice>();
			List<InputDevice> list2 = new List<InputDevice>();
			InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, list);
			InputDevices.GetDevicesAtXRNode(XRNode.RightHand, list2);
			if (list.Count == 1)
			{
				this.leftHand = list[0];
			}
			if (list2.Count == 1)
			{
				this.rightHand = list2[0];
			}
		}
		float axis = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		if (this.leftHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(true);
		}
		else if (this.leftHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(true);
		}
		this.leftHandLastState = axis;
		axis = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		if (this.rightHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(false);
		}
		else if (this.rightHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(false);
		}
		this.rightHandLastState = axis;
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x001097E4 File Offset: 0x001079E4
	public void TryThrowFireball(bool isLeftHand)
	{
		if (isLeftHand && GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
			return;
		}
		if (!isLeftHand && GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
		}
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00109864 File Offset: 0x00107A64
	public void CreateFireball(bool isLeftHand)
	{
		object[] array = new object[1];
		Vector3 position;
		if (isLeftHand)
		{
			array[0] = true;
			position = GorillaPlaySpace.Instance.myVRRig.leftHandTransform.position;
		}
		else
		{
			array[0] = false;
			position = GorillaPlaySpace.Instance.myVRRig.rightHandTransform.position;
		}
		PhotonNetwork.Instantiate("GorillaPrefabs/GorillaFireball", position, Quaternion.identity, 0, array);
	}

	// Token: 0x04003FE9 RID: 16361
	public InputDevice leftHand;

	// Token: 0x04003FEA RID: 16362
	public InputDevice rightHand;

	// Token: 0x04003FEB RID: 16363
	public bool hasInitialized;

	// Token: 0x04003FEC RID: 16364
	public float leftHandLastState;

	// Token: 0x04003FED RID: 16365
	public float rightHandLastState;

	// Token: 0x04003FEE RID: 16366
	public float throwingThreshold = 0.9f;
}
