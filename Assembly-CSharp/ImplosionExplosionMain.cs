using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class ImplosionExplosionMain : MonoBehaviour
{
	// Token: 0x06000057 RID: 87 RVA: 0x000033C8 File Offset: 0x000015C8
	public void Start()
	{
		this.m_aaInstancedDiamondMatrix = new Matrix4x4[(this.NumDiamonds + ImplosionExplosionMain.kNumInstancedBushesPerDrawCall - 1) / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][];
		for (int i = 0; i < this.m_aaInstancedDiamondMatrix.Length; i++)
		{
			this.m_aaInstancedDiamondMatrix[i] = new Matrix4x4[ImplosionExplosionMain.kNumInstancedBushesPerDrawCall];
		}
		for (int j = 0; j < this.NumDiamonds; j++)
		{
			float d = Random.Range(0.1f, 0.4f);
			Vector3 pos = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(0.5f, 7f), Random.Range(-3.5f, 3.5f));
			Quaternion q = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			this.m_aaInstancedDiamondMatrix[j / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][j % ImplosionExplosionMain.kNumInstancedBushesPerDrawCall].SetTRS(pos, q, d * Vector3.one);
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000034D8 File Offset: 0x000016D8
	public void Update()
	{
		Mesh sharedMesh = this.Diamond.GetComponent<MeshFilter>().sharedMesh;
		Material sharedMaterial = this.Diamond.GetComponent<MeshRenderer>().sharedMaterial;
		if (this.m_diamondMaterialProps == null)
		{
			this.m_diamondMaterialProps = new MaterialPropertyBlock();
		}
		if (this.ReactorField.UpdateShaderConstants(this.m_diamondMaterialProps, 1f, 1f))
		{
			foreach (Matrix4x4[] array in this.m_aaInstancedDiamondMatrix)
			{
				Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_diamondMaterialProps);
			}
		}
	}

	// Token: 0x04000047 RID: 71
	public BoingReactorField ReactorField;

	// Token: 0x04000048 RID: 72
	public GameObject Diamond;

	// Token: 0x04000049 RID: 73
	public int NumDiamonds;

	// Token: 0x0400004A RID: 74
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x0400004B RID: 75
	private Matrix4x4[][] m_aaInstancedDiamondMatrix;

	// Token: 0x0400004C RID: 76
	private MaterialPropertyBlock m_diamondMaterialProps;
}
