using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000231 RID: 561
public static class MatCombinerUtils
{
	// Token: 0x06000D30 RID: 3376 RVA: 0x0004668C File Offset: 0x0004488C
	public static void ApplyExtraFingerprintRules(ref UberShaderMatUsedProps matUsedProps)
	{
		matUsedProps.fingerprint._BaseColor = new int4(100, 100, 100, 100);
		matUsedProps.fingerprint._BaseMap_AtlasSlice = -999;
		if (matUsedProps._EmissionToggle != 0)
		{
			int4 emissionColor = matUsedProps.fingerprint._EmissionColor;
			if ((emissionColor.x != 0 || emissionColor.y != 0 || emissionColor.z != 0) && matUsedProps.fingerprint._EmissionColor.w != 0)
			{
				goto IL_AB;
			}
		}
		matUsedProps._EmissionToggle = 0;
		matUsedProps._EmissionColor = 0;
		matUsedProps._EmissionUVScrollSpeed = 0;
		matUsedProps.fingerprint._EmissionColor = int4.zero;
		matUsedProps.fingerprint._EmissionUVScrollSpeed = int4.zero;
		matUsedProps.fingerprint._EmissionMap = string.Empty;
		IL_AB:
		MaterialFingerprint fingerprint = matUsedProps.fingerprint;
		if (fingerprint._WaterEffect <= 0 || fingerprint._HeightBasedWaterEffect != 0)
		{
			matUsedProps._WaterEffect = 999;
			matUsedProps.fingerprint._WaterEffect = 999;
			matUsedProps._HeightBasedWaterEffect = 999;
			matUsedProps.fingerprint._HeightBasedWaterEffect = 999;
		}
		if (matUsedProps._UseSpecular == 0 || matUsedProps.fingerprint._Smoothness == 0)
		{
			matUsedProps._UseSpecular = 0;
			matUsedProps.fingerprint._UseSpecular = 0;
			matUsedProps._Smoothness = 0;
			matUsedProps.fingerprint._Smoothness = 0;
		}
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x000467D0 File Offset: 0x000449D0
	public static Material AverageMaterials(List<Material> oldMats)
	{
		Material material = new Material(oldMats[0]);
		Shader shader = material.shader;
		int propertyCount = shader.GetPropertyCount();
		for (int i = 0; i < propertyCount; i++)
		{
			string propertyName = shader.GetPropertyName(i);
			if (!propertyName.EndsWith("_AtlasSlice"))
			{
				int propertyNameId = shader.GetPropertyNameId(i);
				if (propertyName == "_HalfLambertToggle")
				{
					material.SetFloat(propertyNameId, 0f);
					material.DisableKeyword("_HALF_LAMBERT_TERM");
				}
				else if (propertyName == "_UseSpecular")
				{
					material.SetFloat(propertyNameId, 0f);
					material.DisableKeyword("_GT_RIM_LIGHT");
				}
				else
				{
					ShaderPropertyType propertyType = shader.GetPropertyType(i);
					switch (propertyType)
					{
					case ShaderPropertyType.Color:
					{
						Color color = Color.black;
						foreach (Material material2 in oldMats)
						{
							color += material2.GetColor(propertyNameId);
						}
						color /= (float)oldMats.Count;
						material.SetColor(propertyNameId, color);
						break;
					}
					case ShaderPropertyType.Vector:
					{
						Vector4 vector = Vector4.zero;
						foreach (Material material3 in oldMats)
						{
							vector += material3.GetVector(propertyNameId);
						}
						vector /= (float)oldMats.Count;
						material.SetVector(propertyNameId, vector);
						break;
					}
					case ShaderPropertyType.Float:
					case ShaderPropertyType.Range:
					{
						double num = 0.0;
						foreach (Material material4 in oldMats)
						{
							num += (double)material4.GetFloat(propertyNameId);
						}
						num /= (double)oldMats.Count;
						material.SetFloat(propertyNameId, (float)num);
						break;
					}
					case ShaderPropertyType.Texture:
						break;
					case ShaderPropertyType.Int:
					{
						double num2 = 0.0;
						foreach (Material material5 in oldMats)
						{
							num2 += (double)material5.GetInteger(propertyNameId);
						}
						num2 /= (double)oldMats.Count;
						material.SetInteger(propertyNameId, (int)num2);
						break;
					}
					default:
						Debug.LogError("ERROR!!! MaterialCombiner: Unknown property type: " + propertyType.ToString());
						break;
					}
				}
			}
		}
		material.SetColor(ShaderProps._BaseColor, Color.white);
		return material;
	}

	// Token: 0x0400101B RID: 4123
	private const string _k_logPre = "MaterialCombiner: ";
}
