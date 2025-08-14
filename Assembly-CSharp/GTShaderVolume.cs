using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200027E RID: 638
[ExecuteAlways]
public class GTShaderVolume : MonoBehaviour
{
	// Token: 0x06000E8D RID: 3725 RVA: 0x00058404 File Offset: 0x00056604
	private void OnEnable()
	{
		if (GTShaderVolume.gVolumes.Count > 16)
		{
			return;
		}
		if (!GTShaderVolume.gVolumes.Contains(this))
		{
			GTShaderVolume.gVolumes.Add(this);
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0005842D File Offset: 0x0005662D
	private void OnDisable()
	{
		GTShaderVolume.gVolumes.Remove(this);
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x0005843C File Offset: 0x0005663C
	public static void SyncVolumeData()
	{
		m4x4 m4x = default(m4x4);
		int count = GTShaderVolume.gVolumes.Count;
		for (int i = 0; i < 16; i++)
		{
			if (i >= count)
			{
				MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
			}
			else
			{
				GTShaderVolume gtshaderVolume = GTShaderVolume.gVolumes[i];
				if (!gtshaderVolume)
				{
					MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
				}
				else
				{
					Transform transform = gtshaderVolume.transform;
					Vector4 vector = transform.position;
					Vector4 vector2 = transform.rotation.ToVector();
					Vector4 vector3 = transform.localScale;
					m4x.SetRow0(ref vector);
					m4x.SetRow1(ref vector2);
					m4x.SetRow2(ref vector3);
					m4x.Push(ref GTShaderVolume.ShaderData[i]);
				}
			}
		}
		Shader.SetGlobalInteger(GTShaderVolume._GT_ShaderVolumesActive, count);
		Shader.SetGlobalMatrixArray(GTShaderVolume._GT_ShaderVolumes, GTShaderVolume.ShaderData);
	}

	// Token: 0x04001777 RID: 6007
	public const int MAX_VOLUMES = 16;

	// Token: 0x04001778 RID: 6008
	private static Matrix4x4[] ShaderData = new Matrix4x4[16];

	// Token: 0x04001779 RID: 6009
	[Space]
	private static List<GTShaderVolume> gVolumes = new List<GTShaderVolume>(16);

	// Token: 0x0400177A RID: 6010
	private static ShaderHashId _GT_ShaderVolumes = "_GT_ShaderVolumes";

	// Token: 0x0400177B RID: 6011
	private static ShaderHashId _GT_ShaderVolumesActive = "_GT_ShaderVolumesActive";
}
