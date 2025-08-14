using System;
using UnityEngine;

// Token: 0x02000898 RID: 2200
[Serializable]
public class XformNode
{
	// Token: 0x17000547 RID: 1351
	// (get) Token: 0x06003770 RID: 14192 RVA: 0x0011FC80 File Offset: 0x0011DE80
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 result = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref result);
			return result;
		}
	}

	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06003771 RID: 14193 RVA: 0x0011FCBE File Offset: 0x0011DEBE
	// (set) Token: 0x06003772 RID: 14194 RVA: 0x0011FCCB File Offset: 0x0011DECB
	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x0011FCD9 File Offset: 0x0011DED9
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x04004407 RID: 17415
	public Vector4 localPosition;

	// Token: 0x04004408 RID: 17416
	public Transform parent;
}
