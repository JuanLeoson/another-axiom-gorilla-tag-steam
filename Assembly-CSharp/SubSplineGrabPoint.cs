using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200045A RID: 1114
[Serializable]
public class SubSplineGrabPoint : SubLineGrabPoint
{
	// Token: 0x06001B5F RID: 7007 RVA: 0x00092BBD File Offset: 0x00090DBD
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return CatmullRomSpline.Evaluate(this.controlPointsTransformsRelativeToGrabOrigin, advancedItemState.distAlongLine);
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x00092BD0 File Offset: 0x00090DD0
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		this.controlPointsRelativeToGrabOrigin = new List<Vector3>();
		foreach (Transform transform in this.spline.controlPointTransforms)
		{
			this.controlPointsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.InverseTransformPoint(transform.position));
			this.controlPointsTransformsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.worldToLocalMatrix * transform.localToWorldMatrix);
		}
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x00092C44 File Offset: 0x00090E44
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		Vector3 worldPoint = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector;
		return new AdvancedItemState.PreData
		{
			distAlongLine = CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, worldPoint, out vector),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00092C80 File Offset: 0x00090E80
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 a;
		CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out a);
		return Vector3.SqrMagnitude(a - vector);
	}

	// Token: 0x040023E2 RID: 9186
	public CatmullRomSpline spline;

	// Token: 0x040023E3 RID: 9187
	public List<Vector3> controlPointsRelativeToGrabOrigin = new List<Vector3>();

	// Token: 0x040023E4 RID: 9188
	public List<Matrix4x4> controlPointsTransformsRelativeToGrabOrigin = new List<Matrix4x4>();
}
