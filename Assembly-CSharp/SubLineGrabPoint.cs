using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000459 RID: 1113
[Serializable]
public class SubLineGrabPoint : SubGrabPoint
{
	// Token: 0x06001B58 RID: 7000 RVA: 0x000929B0 File Offset: 0x00090BB0
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		float distAlongLine = advancedItemState.distAlongLine;
		Vector3 pos = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), distAlongLine);
		Quaternion q = Quaternion.Slerp(this.startPointRelativeTransformToGrabPointOrigin.rotation, this.endPointRelativeTransformToGrabPointOrigin.rotation, distAlongLine);
		return Matrix4x4.TRS(pos, q, Vector3.one);
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x00092A08 File Offset: 0x00090C08
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		if (this.startPoint == null || this.endPoint == null)
		{
			return;
		}
		this.startPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.startPoint.position);
		this.endPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.endPoint.position);
		this.endPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.endPoint.localToWorldMatrix;
		this.startPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.startPoint.localToWorldMatrix;
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x00092AA1 File Offset: 0x00090CA1
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			distAlongLine = SubLineGrabPoint.<GetPreData>g__FindNearestFractionOnLine|8_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00092AD8 File Offset: 0x00090CD8
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		float t = SubLineGrabPoint.<EvaluateScore>g__FindNearestFractionOnLine|9_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position);
		Vector3 a = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), t);
		Vector3 b = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(a - b);
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x00092B48 File Offset: 0x00090D48
	[CompilerGenerated]
	internal static float <GetPreData>g__FindNearestFractionOnLine|8_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x00092B84 File Offset: 0x00090D84
	[CompilerGenerated]
	internal static float <EvaluateScore>g__FindNearestFractionOnLine|9_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x040023DC RID: 9180
	public Transform startPoint;

	// Token: 0x040023DD RID: 9181
	public Transform endPoint;

	// Token: 0x040023DE RID: 9182
	public Vector3 startPointRelativeToGrabPointOrigin;

	// Token: 0x040023DF RID: 9183
	public Vector3 endPointRelativeToGrabPointOrigin;

	// Token: 0x040023E0 RID: 9184
	public Matrix4x4 startPointRelativeTransformToGrabPointOrigin;

	// Token: 0x040023E1 RID: 9185
	public Matrix4x4 endPointRelativeTransformToGrabPointOrigin;
}
