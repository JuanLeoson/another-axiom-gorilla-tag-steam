using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000458 RID: 1112
[Serializable]
public class SubGrabPoint
{
	// Token: 0x06001B4E RID: 6990 RVA: 0x00092625 File Offset: 0x00090825
	public virtual Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPointLocalToAdvOriginLocal;
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x0009262D File Offset: 0x0009082D
	public virtual Quaternion GetRotationRelativeToObjectAnchor(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripRotation_ParentAnchorLocal;
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x00092635 File Offset: 0x00090835
	public virtual Vector3 GetGrabPositionRelativeToGrabPointOrigin(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPoint_AdvOriginLocal;
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x00092640 File Offset: 0x00090840
	public virtual void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		if (this.gripPoint == null)
		{
			return;
		}
		this.gripPoint_AdvOriginLocal = advancedGrabPointOrigin.InverseTransformPoint(this.gripPoint.position);
		this.gripRotation_AdvOriginLocal = Quaternion.Inverse(advancedGrabPointOrigin.rotation) * this.gripPoint.rotation;
		this.advAnchor_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * grabPointAnchor.rotation;
		this.gripRotation_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * this.gripPoint.rotation;
		this.gripPointLocalToAdvOriginLocal = advancedGrabPointOrigin.worldToLocalMatrix * this.gripPoint.localToWorldMatrix;
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x000926ED File Offset: 0x000908ED
	public Vector3 GetPositionOnObject(Transform transferableObject, SlotTransformOverride slotTransformOverride)
	{
		return transferableObject.TransformPoint(this.gripPoint_AdvOriginLocal);
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x000926FC File Offset: 0x000908FC
	public virtual Matrix4x4 GetTransformFromPositionState(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride, Transform targetDockXf)
	{
		Quaternion q = advancedItemState.deltaRotation;
		if (!q.IsValid())
		{
			q = Quaternion.identity;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		Matrix4x4 matrix4x2 = this.GetTransformation_GripPointLocalToAdvOriginLocal(advancedItemState.preData, slotTransformOverride) * matrix4x.inverse;
		Matrix4x4 rhs = slotTransformOverride.AdvAnchorLocalToAdvOriginLocal * matrix4x2.inverse;
		return slotTransformOverride.AdvOriginLocalToParentAnchorLocal * rhs;
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x0009276C File Offset: 0x0009096C
	public AdvancedItemState GetAdvancedItemStateFromHand(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		AdvancedItemState.PreData preData = this.GetPreData(objectTransform, handTransform, targetDock, slotTransformOverride);
		Matrix4x4 matrix4x = targetDock.localToWorldMatrix * slotTransformOverride.AdvOriginLocalToParentAnchorLocal * slotTransformOverride.AdvAnchorLocalToAdvOriginLocal;
		Matrix4x4 rhs = objectTransform.localToWorldMatrix * this.GetTransformation_GripPointLocalToAdvOriginLocal(preData, slotTransformOverride);
		Quaternion quaternion = (matrix4x.inverse * rhs).rotation;
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 vector3 = quaternion * Vector3.forward;
		bool reverseGrip = false;
		Vector2 up = Vector2.up;
		float angle = 0f;
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
			quaternion = Quaternion.identity;
			break;
		case LimitAxis.YAxis:
			if (this.allowReverseGrip)
			{
				if (Vector3.Dot(vector, Vector3.up) < 0f)
				{
					Debug.Log("Using Reverse Grip");
					reverseGrip = true;
					vector = Vector3.down;
				}
				else
				{
					vector = Vector3.up;
				}
			}
			else
			{
				vector = Vector3.up;
			}
			vector2 = Vector3.Cross(vector, vector3);
			vector3 = Vector3.Cross(vector2, vector);
			up = new Vector2(vector3.z, vector3.x);
			quaternion = Quaternion.LookRotation(vector3, vector);
			break;
		case LimitAxis.XAxis:
			vector2 = Vector3.right;
			vector3 = Vector3.Cross(vector2, vector);
			vector = Vector3.Cross(vector3, vector2);
			break;
		case LimitAxis.ZAxis:
			vector3 = Vector3.forward;
			vector2 = Vector3.Cross(vector, vector3);
			vector = Vector3.Cross(vector3, vector2);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return new AdvancedItemState
		{
			preData = preData,
			limitAxis = this.limitAxis,
			angle = angle,
			reverseGrip = reverseGrip,
			angleVectorWhereUpIsStandard = up,
			deltaRotation = quaternion
		};
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x00092930 File Offset: 0x00090B30
	public virtual AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			pointType = AdvancedItemState.PointType.Standard
		};
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x00092940 File Offset: 0x00090B40
	public virtual float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 b = objectTransform.InverseTransformPoint(handTransform.position);
		float num = Vector3.SqrMagnitude(this.gripPoint_AdvOriginLocal - b);
		float f;
		Vector3 vector;
		(Quaternion.Inverse(objectTransform.rotation * this.gripRotation_AdvOriginLocal) * targetDock.rotation * this.advAnchor_ParentAnchorLocal).ToAngleAxis(out f, out vector);
		return num + Mathf.Abs(f) * 0.0001f;
	}

	// Token: 0x040023D3 RID: 9171
	[FormerlySerializedAs("transform")]
	public Transform gripPoint;

	// Token: 0x040023D4 RID: 9172
	public LimitAxis limitAxis;

	// Token: 0x040023D5 RID: 9173
	public bool allowReverseGrip;

	// Token: 0x040023D6 RID: 9174
	private Vector3 gripPoint_AdvOriginLocal;

	// Token: 0x040023D7 RID: 9175
	private Vector3 gripPointOffset_AdvOriginLocal;

	// Token: 0x040023D8 RID: 9176
	public Quaternion gripRotation_AdvOriginLocal;

	// Token: 0x040023D9 RID: 9177
	public Quaternion advAnchor_ParentAnchorLocal;

	// Token: 0x040023DA RID: 9178
	public Quaternion gripRotation_ParentAnchorLocal;

	// Token: 0x040023DB RID: 9179
	public Matrix4x4 gripPointLocalToAdvOriginLocal;
}
