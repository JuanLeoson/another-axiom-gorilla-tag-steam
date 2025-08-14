using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class ObjectHierarchyFlattener : MonoBehaviour
{
	// Token: 0x06000C72 RID: 3186 RVA: 0x0004346C File Offset: 0x0004166C
	private void ResetTransform()
	{
		base.transform.SetParent(this.originalParentTransform);
		base.transform.localPosition = this.originalLocalPosition;
		base.transform.localRotation = this.originalLocalRotation;
		base.transform.localScale = this.originalScale;
		if (this.crumb != null)
		{
			Object.Destroy(this.crumb);
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x000434D8 File Offset: 0x000416D8
	public void InvokeLateUpdate()
	{
		if (!this.originalParentGO.activeInHierarchy)
		{
			ObjectHierarchyFlattenerManager.UnregisterOHF(this);
			base.Invoke("ResetTransform", 0f);
			return;
		}
		if (!this.trackTransformOfParent)
		{
			return;
		}
		if (this.maintainRelativeScale)
		{
			base.transform.localScale = Vector3.Scale(this.originalParentTransform.lossyScale, this.originalScale);
		}
		base.transform.rotation = this.originalParentTransform.rotation * this.originalLocalRotation;
		base.transform.position = this.originalParentTransform.position + base.transform.rotation * this.calcOffset * (this.originalParentTransform.lossyScale.x / this.originalParentScale) * this.originalParentScale;
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x000435B4 File Offset: 0x000417B4
	private void OnEnable()
	{
		ObjectHierarchyFlattenerManager.RegisterOHF(this);
		this.originalParentTransform = base.transform.parent;
		this.originalParentGO = this.originalParentTransform.gameObject;
		this.originalLocalPosition = base.transform.localPosition;
		this.originalLocalRotation = base.transform.localRotation;
		this.originalParentScale = base.transform.parent.lossyScale.x;
		this.originalScale = base.transform.localScale;
		this.calcOffset = Vector3.Scale(this.originalLocalPosition, this.originalScale);
		if (this.originalParentGO.GetComponent<FlattenerCrumb>() == null)
		{
			this.crumb = this.originalParentGO.AddComponent<FlattenerCrumb>();
		}
		base.transform.SetParent(null);
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0004367E File Offset: 0x0004187E
	private void OnDisable()
	{
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		base.Invoke("ResetTransform", 0f);
	}

	// Token: 0x04000F70 RID: 3952
	private GameObject originalParentGO;

	// Token: 0x04000F71 RID: 3953
	private Transform originalParentTransform;

	// Token: 0x04000F72 RID: 3954
	private Vector3 originalLocalPosition;

	// Token: 0x04000F73 RID: 3955
	private Vector3 calcOffset;

	// Token: 0x04000F74 RID: 3956
	private Quaternion originalLocalRotation;

	// Token: 0x04000F75 RID: 3957
	private Vector3 originalScale;

	// Token: 0x04000F76 RID: 3958
	private float originalParentScale;

	// Token: 0x04000F77 RID: 3959
	public bool trackTransformOfParent;

	// Token: 0x04000F78 RID: 3960
	public bool maintainRelativeScale;

	// Token: 0x04000F79 RID: 3961
	private FlattenerCrumb crumb;
}
