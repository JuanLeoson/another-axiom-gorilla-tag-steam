using System;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class StylusTip : MonoBehaviour
{
	// Token: 0x0600161C RID: 5660 RVA: 0x00078560 File Offset: 0x00076760
	private void Awake()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
		this.m_breadCrumbContainer = new GameObject(string.Format("BreadCrumbContainer ({0})", this.m_handedness));
		this.m_breadCrumbs = new GameObject[60];
		for (int i = 0; i < this.m_breadCrumbs.Length; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_breadCrumbPf, this.m_breadCrumbContainer.transform);
			gameObject.name = string.Format("BreadCrumb ({0})", i);
			gameObject.SetActive(false);
			this.m_breadCrumbs[i] = gameObject;
		}
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x00078600 File Offset: 0x00076800
	private void Update()
	{
		Pose pose = new Pose(OVRInput.GetLocalControllerPosition(this.m_controller), OVRInput.GetLocalControllerRotation(this.m_controller));
		Pose transformedBy = pose.GetTransformedBy(this.m_trackingSpace);
		Pose transformedBy2 = StylusTip.GetT_Device_StylusTip(this.m_controller).GetTransformedBy(transformedBy);
		base.transform.SetPositionAndRotation(transformedBy2.position, transformedBy2.rotation);
		float num = OVRInput.Get(OVRInput.Axis1D.PrimaryStylusForce, this.m_controller);
		bool flag = num > 0f;
		GameObject gameObject = this.m_breadCrumbs[this.m_breadCrumbIndexCurr];
		gameObject.transform.position = base.transform.position;
		float num2 = Mathf.Lerp(0.005f, 0.02f, num);
		gameObject.transform.localScale = new Vector3(num2, num2, num2);
		gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, num);
		gameObject.SetActive(flag);
		float num3 = 0f;
		float num4 = float.PositiveInfinity;
		if (this.m_breadCrumbIndexPrev >= 0)
		{
			num4 = (base.transform.position - this.m_breadCrumbs[this.m_breadCrumbIndexPrev].transform.position).magnitude;
			num3 = (num2 + this.m_breadCrumbs[this.m_breadCrumbIndexPrev].transform.localScale.x) * 0.5f;
		}
		if (flag && num4 >= num3)
		{
			this.m_breadCrumbIndexPrev = this.m_breadCrumbIndexCurr;
			this.m_breadCrumbIndexCurr = (this.m_breadCrumbIndexCurr + 1) % this.m_breadCrumbs.Length;
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00078790 File Offset: 0x00076990
	private static Pose GetT_Device_StylusTip(OVRInput.Controller controller)
	{
		Pose identity = Pose.identity;
		if (controller == OVRInput.Controller.LTouch || controller == OVRInput.Controller.RTouch)
		{
			identity = new Pose(new Vector3(0.0094f, -0.07145f, -0.07565f), Quaternion.Euler(35.305f, 50.988f, 37.901f));
		}
		if (controller == OVRInput.Controller.LTouch)
		{
			identity.position.x = identity.position.x * -1f;
			identity.rotation.y = identity.rotation.y * -1f;
			identity.rotation.z = identity.rotation.z * -1f;
		}
		return identity;
	}

	// Token: 0x04001DD2 RID: 7634
	private const int MaxBreadCrumbs = 60;

	// Token: 0x04001DD3 RID: 7635
	private const float BreadCrumbMinSize = 0.005f;

	// Token: 0x04001DD4 RID: 7636
	private const float BreadCrumbMaxSize = 0.02f;

	// Token: 0x04001DD5 RID: 7637
	[Header("External")]
	[SerializeField]
	private Transform m_trackingSpace;

	// Token: 0x04001DD6 RID: 7638
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04001DD7 RID: 7639
	[SerializeField]
	private GameObject m_breadCrumbPf;

	// Token: 0x04001DD8 RID: 7640
	private GameObject m_breadCrumbContainer;

	// Token: 0x04001DD9 RID: 7641
	private GameObject[] m_breadCrumbs;

	// Token: 0x04001DDA RID: 7642
	private int m_breadCrumbIndexPrev = -1;

	// Token: 0x04001DDB RID: 7643
	private int m_breadCrumbIndexCurr;

	// Token: 0x04001DDC RID: 7644
	private OVRInput.Controller m_controller;
}
