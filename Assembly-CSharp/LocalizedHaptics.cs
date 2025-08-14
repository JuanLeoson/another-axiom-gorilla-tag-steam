using System;
using UnityEngine;

// Token: 0x020003B9 RID: 953
public class LocalizedHaptics : MonoBehaviour
{
	// Token: 0x06001615 RID: 5653 RVA: 0x000783FC File Offset: 0x000765FC
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x00078414 File Offset: 0x00076614
	private void Update()
	{
		float amplitude = (OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Thumb, 0f, amplitude, this.m_controller);
		float amplitude2 = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Index, 0f, amplitude2, this.m_controller);
		float amplitude3 = (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Hand, 0f, amplitude3, this.m_controller);
	}

	// Token: 0x04001DCD RID: 7629
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04001DCE RID: 7630
	private OVRInput.Controller m_controller;
}
