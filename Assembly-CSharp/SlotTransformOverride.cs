using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x0200045B RID: 1115
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001B64 RID: 7012 RVA: 0x00092CD3 File Offset: 0x00090ED3
	// (set) Token: 0x06001B65 RID: 7013 RVA: 0x00092CE0 File Offset: 0x00090EE0
	private XformOffset _EdXformOffsetRepresenationOf_overrideTransformMatrix
	{
		get
		{
			return new XformOffset(this.overrideTransformMatrix);
		}
		set
		{
			this.overrideTransformMatrix = Matrix4x4.TRS(value.pos, value.rot, value.scale);
		}
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x00092D00 File Offset: 0x00090F00
	public void Initialize(Component component, Transform anchor)
	{
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x00092DAC File Offset: 0x00090FAC
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x00092DC0 File Offset: 0x00090FC0
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint item = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(item);
	}

	// Token: 0x040023E5 RID: 9189
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	public Transform overrideTransform;

	// Token: 0x040023E6 RID: 9190
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	[Delayed]
	public string overrideTransform_path;

	// Token: 0x040023E7 RID: 9191
	public TransferrableObject.PositionState positionState;

	// Token: 0x040023E8 RID: 9192
	public bool useAdvancedGrab;

	// Token: 0x040023E9 RID: 9193
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x040023EA RID: 9194
	public Transform advancedGrabPointAnchor;

	// Token: 0x040023EB RID: 9195
	public Transform advancedGrabPointOrigin;

	// Token: 0x040023EC RID: 9196
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x040023ED RID: 9197
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x040023EE RID: 9198
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
