using System;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class GrabObject : MonoBehaviour
{
	// Token: 0x060014B1 RID: 5297 RVA: 0x0006F813 File Offset: 0x0006DA13
	public void Grab(OVRInput.Controller grabHand)
	{
		this.grabbedRotation = base.transform.rotation;
		GrabObject.GrabbedObject grabbedObjectDelegate = this.GrabbedObjectDelegate;
		if (grabbedObjectDelegate == null)
		{
			return;
		}
		grabbedObjectDelegate(grabHand);
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x0006F837 File Offset: 0x0006DA37
	public void Release()
	{
		GrabObject.ReleasedObject releasedObjectDelegate = this.ReleasedObjectDelegate;
		if (releasedObjectDelegate == null)
		{
			return;
		}
		releasedObjectDelegate();
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x0006F849 File Offset: 0x0006DA49
	public void CursorPos(Vector3 cursorPos)
	{
		GrabObject.SetCursorPosition cursorPositionDelegate = this.CursorPositionDelegate;
		if (cursorPositionDelegate == null)
		{
			return;
		}
		cursorPositionDelegate(cursorPos);
	}

	// Token: 0x04001C39 RID: 7225
	[TextArea]
	public string ObjectName;

	// Token: 0x04001C3A RID: 7226
	[TextArea]
	public string ObjectInstructions;

	// Token: 0x04001C3B RID: 7227
	public GrabObject.ManipulationType objectManipulationType;

	// Token: 0x04001C3C RID: 7228
	public bool showLaserWhileGrabbed;

	// Token: 0x04001C3D RID: 7229
	[HideInInspector]
	public Quaternion grabbedRotation = Quaternion.identity;

	// Token: 0x04001C3E RID: 7230
	public GrabObject.GrabbedObject GrabbedObjectDelegate;

	// Token: 0x04001C3F RID: 7231
	public GrabObject.ReleasedObject ReleasedObjectDelegate;

	// Token: 0x04001C40 RID: 7232
	public GrabObject.SetCursorPosition CursorPositionDelegate;

	// Token: 0x0200036B RID: 875
	public enum ManipulationType
	{
		// Token: 0x04001C42 RID: 7234
		Default,
		// Token: 0x04001C43 RID: 7235
		ForcedHand,
		// Token: 0x04001C44 RID: 7236
		DollyHand,
		// Token: 0x04001C45 RID: 7237
		DollyAttached,
		// Token: 0x04001C46 RID: 7238
		HorizontalScaled,
		// Token: 0x04001C47 RID: 7239
		VerticalScaled,
		// Token: 0x04001C48 RID: 7240
		Menu
	}

	// Token: 0x0200036C RID: 876
	// (Invoke) Token: 0x060014B6 RID: 5302
	public delegate void GrabbedObject(OVRInput.Controller grabHand);

	// Token: 0x0200036D RID: 877
	// (Invoke) Token: 0x060014BA RID: 5306
	public delegate void ReleasedObject();

	// Token: 0x0200036E RID: 878
	// (Invoke) Token: 0x060014BE RID: 5310
	public delegate void SetCursorPosition(Vector3 cursorPosition);
}
