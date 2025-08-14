using System;
using UnityEngine;

// Token: 0x020003AA RID: 938
[ExecuteInEditMode]
public class SimpleResizable : MonoBehaviour
{
	// Token: 0x17000267 RID: 615
	// (get) Token: 0x060015BD RID: 5565 RVA: 0x000769D1 File Offset: 0x00074BD1
	public Vector3 PivotPosition
	{
		get
		{
			return this._pivotTransform.position;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x060015BE RID: 5566 RVA: 0x000769DE File Offset: 0x00074BDE
	// (set) Token: 0x060015BF RID: 5567 RVA: 0x000769E6 File Offset: 0x00074BE6
	public Vector3 DefaultSize { get; private set; }

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x060015C0 RID: 5568 RVA: 0x000769EF File Offset: 0x00074BEF
	// (set) Token: 0x060015C1 RID: 5569 RVA: 0x000769F7 File Offset: 0x00074BF7
	public Mesh OriginalMesh { get; private set; }

	// Token: 0x060015C2 RID: 5570 RVA: 0x00076A00 File Offset: 0x00074C00
	public void SetNewSize(Vector3 newSize)
	{
		this._newSize = newSize;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x00076A0C File Offset: 0x00074C0C
	private void Awake()
	{
		this._meshFilter = base.GetComponent<MeshFilter>();
		this.OriginalMesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.DefaultSize = this.OriginalMesh.bounds.size;
		this._newSize = this.DefaultSize;
		this._oldSize = this._newSize;
		if (!this._pivotTransform)
		{
			this._pivotTransform = base.transform.Find("Pivot");
		}
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x00076A8C File Offset: 0x00074C8C
	private void OnEnable()
	{
		this.DefaultSize = this.OriginalMesh.bounds.size;
		if (this._newSize == Vector3.zero)
		{
			this._newSize = this.DefaultSize;
		}
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x00076AD0 File Offset: 0x00074CD0
	private void Update()
	{
		if (Application.isPlaying && !this._updateInPlayMode)
		{
			return;
		}
		if (this._newSize != this._oldSize)
		{
			this._oldSize = this._newSize;
			Mesh sharedMesh = SimpleResizer.ProcessVertices(this, this._newSize, true);
			this._meshFilter.sharedMesh = sharedMesh;
			this._meshFilter.sharedMesh.RecalculateBounds();
		}
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x00076B38 File Offset: 0x00074D38
	private void OnDrawGizmos()
	{
		if (!this._pivotTransform)
		{
			return;
		}
		Gizmos.color = Color.red;
		float d = 0.1f;
		Vector3 from = this._pivotTransform.position + Vector3.left * d * 0.5f;
		Vector3 from2 = this._pivotTransform.position + Vector3.down * d * 0.5f;
		Vector3 from3 = this._pivotTransform.position + Vector3.back * d * 0.5f;
		Gizmos.DrawRay(from, Vector3.right * d);
		Gizmos.DrawRay(from2, Vector3.up * d);
		Gizmos.DrawRay(from3, Vector3.forward * d);
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x00076C08 File Offset: 0x00074E08
	private void OnDrawGizmosSelected()
	{
		if (this._meshFilter.sharedMesh == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 center = this._meshFilter.sharedMesh.bounds.center;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		switch (this.ScalingX)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x * this.PaddingX * 2f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingX, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingXMax, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		switch (this.ScalingY)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y * this.PaddingY * 2f, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingY, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingYMax, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		switch (this.ScalingZ)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y, this._newSize.z * this.PaddingZ * 2f));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZ), new Vector3(this._newSize.x, this._newSize.y, 0f));
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZMax), new Vector3(this._newSize.x, this._newSize.y, 0f));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 1f, 1f);
		Gizmos.DrawWireCube(center, this._newSize);
	}

	// Token: 0x04001D7B RID: 7547
	[Space(15f)]
	public SimpleResizable.Method ScalingX;

	// Token: 0x04001D7C RID: 7548
	[Range(0f, 0.5f)]
	public float PaddingX;

	// Token: 0x04001D7D RID: 7549
	[Range(-0.5f, 0f)]
	public float PaddingXMax;

	// Token: 0x04001D7E RID: 7550
	[Space(15f)]
	public SimpleResizable.Method ScalingY;

	// Token: 0x04001D7F RID: 7551
	[Range(0f, 0.5f)]
	public float PaddingY;

	// Token: 0x04001D80 RID: 7552
	[Range(-0.5f, 0f)]
	public float PaddingYMax;

	// Token: 0x04001D81 RID: 7553
	[Space(15f)]
	public SimpleResizable.Method ScalingZ;

	// Token: 0x04001D82 RID: 7554
	[Range(0f, 0.5f)]
	public float PaddingZ;

	// Token: 0x04001D83 RID: 7555
	[Range(-0.5f, 0f)]
	public float PaddingZMax;

	// Token: 0x04001D86 RID: 7558
	private Vector3 _oldSize;

	// Token: 0x04001D87 RID: 7559
	private MeshFilter _meshFilter;

	// Token: 0x04001D88 RID: 7560
	[SerializeField]
	private Vector3 _newSize;

	// Token: 0x04001D89 RID: 7561
	[SerializeField]
	private bool _updateInPlayMode;

	// Token: 0x04001D8A RID: 7562
	[SerializeField]
	private Transform _pivotTransform;

	// Token: 0x020003AB RID: 939
	public enum Method
	{
		// Token: 0x04001D8C RID: 7564
		Adapt,
		// Token: 0x04001D8D RID: 7565
		AdaptWithAsymmetricalPadding,
		// Token: 0x04001D8E RID: 7566
		Scale,
		// Token: 0x04001D8F RID: 7567
		None
	}
}
