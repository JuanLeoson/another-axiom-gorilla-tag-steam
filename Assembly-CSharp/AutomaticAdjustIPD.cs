using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F0 RID: 1264
public class AutomaticAdjustIPD : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001EAB RID: 7851 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000A1F44 File Offset: 0x000A0144
	public void SliceUpdate()
	{
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			for (int i = 0; i < this.adjustXScaleObjects.Length; i++)
			{
				Transform transform = this.adjustXScaleObjects[i];
				if (!transform)
				{
					return;
				}
				transform.localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x04002751 RID: 10065
	public InputDevice headset;

	// Token: 0x04002752 RID: 10066
	public float currentIPD;

	// Token: 0x04002753 RID: 10067
	public Vector3 leftEyePosition;

	// Token: 0x04002754 RID: 10068
	public Vector3 rightEyePosition;

	// Token: 0x04002755 RID: 10069
	public bool testOverride;

	// Token: 0x04002756 RID: 10070
	public Transform[] adjustXScaleObjects;

	// Token: 0x04002757 RID: 10071
	public float sizeAt58mm = 1f;

	// Token: 0x04002758 RID: 10072
	public float sizeAt63mm = 1.12f;

	// Token: 0x04002759 RID: 10073
	public float lastIPD;
}
