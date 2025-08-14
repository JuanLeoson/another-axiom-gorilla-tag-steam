using System;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class SPPquad : MonoBehaviour
{
	// Token: 0x0600151D RID: 5405 RVA: 0x000730BC File Offset: 0x000712BC
	private void Start()
	{
		this.passthroughLayer = base.GetComponent<OVRPassthroughLayer>();
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00073147 File Offset: 0x00071347
	public void Grab(OVRInput.Controller grabHand)
	{
		this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
		this.controllerHand = grabHand;
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00073166 File Offset: 0x00071366
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
	}

	// Token: 0x04001CB7 RID: 7351
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x04001CB8 RID: 7352
	public MeshFilter projectionObject;

	// Token: 0x04001CB9 RID: 7353
	private OVRInput.Controller controllerHand;
}
