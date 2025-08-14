using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000897 RID: 2199
[ExecuteAlways]
public class Xform : MonoBehaviour
{
	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x0600376A RID: 14186 RVA: 0x0011F9F3 File Offset: 0x0011DBF3
	public float3 localExtents
	{
		get
		{
			return this.localScale * 0.5f;
		}
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0011FA05 File Offset: 0x0011DC05
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x0011FA28 File Offset: 0x0011DC28
	public Matrix4x4 TRS()
	{
		if (this.parent.AsNull<Transform>() == null)
		{
			return this.LocalTRS();
		}
		return this.parent.localToWorldMatrix * this.LocalTRS();
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0011FA5C File Offset: 0x0011DC5C
	private unsafe void Update()
	{
		Matrix4x4 matrix = this.TRS();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(matrix))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.PlaneWithNormal(Xform.AXIS_XR_RT * 0.5f, Xform.AXIS_XR_RT, Xform.F2_ONE, Xform.CR);
				commandBuilder.PlaneWithNormal(Xform.AXIS_YG_UP * 0.5f, Xform.AXIS_YG_UP, Xform.F2_ONE, Xform.CG);
				commandBuilder.PlaneWithNormal(Xform.AXIS_ZB_FW * 0.5f, Xform.AXIS_ZB_FW, Xform.F2_ONE, Xform.CB);
				commandBuilder.WireBox(float3.zero, quaternion.identity, 1f, this.displayColor);
			}
		}
	}

	// Token: 0x040043FA RID: 17402
	public Transform parent;

	// Token: 0x040043FB RID: 17403
	[Space]
	public Color displayColor = SRand.New().NextColor();

	// Token: 0x040043FC RID: 17404
	[Space]
	public float3 localPosition = float3.zero;

	// Token: 0x040043FD RID: 17405
	public float3 localScale = Vector3.one;

	// Token: 0x040043FE RID: 17406
	public Quaternion localRotation = quaternion.identity;

	// Token: 0x040043FF RID: 17407
	private static readonly float3 F3_ONE = 1f;

	// Token: 0x04004400 RID: 17408
	private static readonly float2 F2_ONE = 1f;

	// Token: 0x04004401 RID: 17409
	private static readonly float3 AXIS_ZB_FW = new float3(0f, 0f, 1f);

	// Token: 0x04004402 RID: 17410
	private static readonly float3 AXIS_YG_UP = new float3(0f, 1f, 0f);

	// Token: 0x04004403 RID: 17411
	private static readonly float3 AXIS_XR_RT = new float3(1f, 0f, 0f);

	// Token: 0x04004404 RID: 17412
	private static readonly Color CR = new Color(1f, 0f, 0f, 0.24f);

	// Token: 0x04004405 RID: 17413
	private static readonly Color CG = new Color(0f, 1f, 0f, 0.24f);

	// Token: 0x04004406 RID: 17414
	private static readonly Color CB = new Color(0f, 0f, 1f, 0.24f);
}
