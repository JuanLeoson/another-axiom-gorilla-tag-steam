using System;
using UnityEngine;

// Token: 0x02000362 RID: 866
public class AugmentedObject : MonoBehaviour
{
	// Token: 0x06001483 RID: 5251 RVA: 0x0006EC84 File Offset: 0x0006CE84
	private void Start()
	{
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0006ECEC File Offset: 0x0006CEEC
	private void Update()
	{
		if (this.controllerHand != OVRInput.Controller.None && OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
		{
			this.ToggleShadowType();
		}
		if (this.shadow)
		{
			if (this.groundShadow)
			{
				this.shadow.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				return;
			}
			this.shadow.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0006ED7A File Offset: 0x0006CF7A
	public void Grab(OVRInput.Controller grabHand)
	{
		this.controllerHand = grabHand;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x0006ED83 File Offset: 0x0006CF83
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x0006ED8C File Offset: 0x0006CF8C
	private void ToggleShadowType()
	{
		this.groundShadow = !this.groundShadow;
	}

	// Token: 0x04001C0E RID: 7182
	public OVRInput.Controller controllerHand;

	// Token: 0x04001C0F RID: 7183
	public Transform shadow;

	// Token: 0x04001C10 RID: 7184
	private bool groundShadow;
}
