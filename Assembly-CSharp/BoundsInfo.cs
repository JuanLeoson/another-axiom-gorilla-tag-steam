using System;
using MathGeoLib;
using UnityEngine;

// Token: 0x02000B5B RID: 2907
[Serializable]
public struct BoundsInfo
{
	// Token: 0x1700068E RID: 1678
	// (get) Token: 0x0600458F RID: 17807 RVA: 0x0015B623 File Offset: 0x00159823
	public Vector3 sizeComputed
	{
		get
		{
			return Vector3.Scale(this.size, this.scale) * this.inflate;
		}
	}

	// Token: 0x1700068F RID: 1679
	// (get) Token: 0x06004590 RID: 17808 RVA: 0x0015B641 File Offset: 0x00159841
	public Vector3 sizeComputedAA
	{
		get
		{
			return Vector3.Scale(this.sizeAA, this.scaleAA) * this.inflateAA;
		}
	}

	// Token: 0x06004591 RID: 17809 RVA: 0x0015B660 File Offset: 0x00159860
	public static BoundsInfo ComputeBounds(Vector3[] vertices)
	{
		if (vertices.Length == 0)
		{
			return default(BoundsInfo);
		}
		OrientedBoundingBox orientedBoundingBox = OrientedBoundingBox.BruteEnclosing(vertices);
		Vector4 column = orientedBoundingBox.Axis1;
		Vector4 column2 = orientedBoundingBox.Axis2;
		Vector4 column3 = orientedBoundingBox.Axis3;
		Vector4 column4 = new Vector4(0f, 0f, 0f, 1f);
		BoundsInfo result = default(BoundsInfo);
		result.center = orientedBoundingBox.Center;
		result.size = orientedBoundingBox.Extent * 2f;
		result.rotation = new Matrix4x4(column, column2, column3, column4).rotation;
		result.scale = Vector3.one;
		result.inflate = 1f;
		Bounds bounds = GeometryUtility.CalculateBounds(vertices, Matrix4x4.identity);
		result.centerAA = bounds.center;
		result.sizeAA = bounds.size;
		result.scaleAA = Vector3.one;
		result.inflateAA = 1f;
		return result;
	}

	// Token: 0x06004592 RID: 17810 RVA: 0x0015B764 File Offset: 0x00159964
	public static BoxCollider CreateBoxCollider(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int hashCode3 = bounds.rotation.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2, hashCode3);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.center;
		transform.rotation = bounds.rotation;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputed;
		return boxCollider;
	}

	// Token: 0x06004593 RID: 17811 RVA: 0x0015B810 File Offset: 0x00159A10
	public static BoxCollider CreateBoxColliderAA(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.centerAA;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputedAA;
		return boxCollider;
	}

	// Token: 0x0400508C RID: 20620
	public Vector3 center;

	// Token: 0x0400508D RID: 20621
	public Vector3 size;

	// Token: 0x0400508E RID: 20622
	public Quaternion rotation;

	// Token: 0x0400508F RID: 20623
	public Vector3 scale;

	// Token: 0x04005090 RID: 20624
	public float inflate;

	// Token: 0x04005091 RID: 20625
	[Space]
	public Vector3 centerAA;

	// Token: 0x04005092 RID: 20626
	public Vector3 sizeAA;

	// Token: 0x04005093 RID: 20627
	public Vector3 scaleAA;

	// Token: 0x04005094 RID: 20628
	public float inflateAA;
}
