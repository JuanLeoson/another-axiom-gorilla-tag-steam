using System;
using TMPro;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class UiDeviceInspector : MonoBehaviour
{
	// Token: 0x0600162B RID: 5675 RVA: 0x00078AD7 File Offset: 0x00076CD7
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00078AEC File Offset: 0x00076CEC
	private void Update()
	{
		string sourceText = UiDeviceInspector.ToDeviceModel() + " [" + UiDeviceInspector.ToHandednessString(this.m_handedness) + "]";
		this.m_title.SetText(sourceText, true);
		string text = OVRInput.IsControllerConnected(this.m_controller) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>";
		string text2 = (OVRInput.GetControllerOrientationTracked(this.m_controller) && OVRInput.GetControllerPositionTracked(this.m_controller)) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>";
		this.m_status.SetText(string.Concat(new string[]
		{
			"Connected [",
			text,
			"] Tracked [",
			text2,
			"]"
		}), true);
		this.m_thumbRestTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, this.m_controller));
		this.m_indexTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
		this.m_gripTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
		this.m_thumbRestForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller));
		this.m_stylusTipForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryStylusForce, this.m_controller));
		this.m_indexCurl1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerCurl, this.m_controller));
		this.m_indexSlider1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerSlide, this.m_controller));
		this.m_ax.SetValue(OVRInput.Get(OVRInput.Button.One, this.m_controller));
		this.m_axTouch.SetValue(OVRInput.Get(OVRInput.Touch.One, this.m_controller));
		this.m_by.SetValue(OVRInput.Get(OVRInput.Button.Two, this.m_controller));
		this.m_byTouch.SetValue(OVRInput.Get(OVRInput.Touch.Two, this.m_controller));
		this.m_indexTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, this.m_controller));
		this.m_thumbstick.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, this.m_controller), OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller));
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x00078CEB File Offset: 0x00076EEB
	private static string ToDeviceModel()
	{
		return "Touch";
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00078CF2 File Offset: 0x00076EF2
	private static string ToHandednessString(OVRInput.Handedness handedness)
	{
		if (handedness == OVRInput.Handedness.LeftHanded)
		{
			return "L";
		}
		if (handedness != OVRInput.Handedness.RightHanded)
		{
			return "-";
		}
		return "R";
	}

	// Token: 0x04001DE9 RID: 7657
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04001DEA RID: 7658
	[Header("Left Column Components")]
	[SerializeField]
	private TextMeshProUGUI m_title;

	// Token: 0x04001DEB RID: 7659
	[SerializeField]
	private TextMeshProUGUI m_status;

	// Token: 0x04001DEC RID: 7660
	[SerializeField]
	private UiBoolInspector m_thumbRestTouch;

	// Token: 0x04001DED RID: 7661
	[SerializeField]
	private UiAxis1dInspector m_thumbRestForce;

	// Token: 0x04001DEE RID: 7662
	[SerializeField]
	private UiAxis1dInspector m_indexTrigger;

	// Token: 0x04001DEF RID: 7663
	[SerializeField]
	private UiAxis1dInspector m_gripTrigger;

	// Token: 0x04001DF0 RID: 7664
	[SerializeField]
	private UiAxis1dInspector m_stylusTipForce;

	// Token: 0x04001DF1 RID: 7665
	[SerializeField]
	private UiAxis1dInspector m_indexCurl1d;

	// Token: 0x04001DF2 RID: 7666
	[SerializeField]
	private UiAxis1dInspector m_indexSlider1d;

	// Token: 0x04001DF3 RID: 7667
	[Header("Right Column Components")]
	[SerializeField]
	private UiBoolInspector m_ax;

	// Token: 0x04001DF4 RID: 7668
	[SerializeField]
	private UiBoolInspector m_axTouch;

	// Token: 0x04001DF5 RID: 7669
	[SerializeField]
	private UiBoolInspector m_by;

	// Token: 0x04001DF6 RID: 7670
	[SerializeField]
	private UiBoolInspector m_byTouch;

	// Token: 0x04001DF7 RID: 7671
	[SerializeField]
	private UiBoolInspector m_indexTouch;

	// Token: 0x04001DF8 RID: 7672
	[SerializeField]
	private UiAxis2dInspector m_thumbstick;

	// Token: 0x04001DF9 RID: 7673
	private OVRInput.Controller m_controller;
}
