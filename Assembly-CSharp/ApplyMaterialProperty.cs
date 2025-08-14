using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200016F RID: 367
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x0600099D RID: 2461 RVA: 0x00034AA1 File Offset: 0x00032CA1
	private void Start()
	{
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x00034AB4 File Offset: 0x00032CB4
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00034AF6 File Offset: 0x00032CF6
	public void SetColor(string propertyName, Color color)
	{
		this.SetColor(Shader.PropertyToID(propertyName), color);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00034B05 File Offset: 0x00032D05
	public void SetColor(int propertyId, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00034B1C File Offset: 0x00032D1C
	public void SetFloat(string propertyName, float value)
	{
		this.SetFloat(Shader.PropertyToID(propertyName), value);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00034B2B File Offset: 0x00032D2B
	public void SetFloat(int propertyId, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00034B44 File Offset: 0x00032D44
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(int id, string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i].id == id)
			{
				return this.customData[i];
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData = new ApplyMaterialProperty.CustomMaterialData(id, propertyName);
		this.customData.Add(customMaterialData);
		return customMaterialData;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00034BA0 File Offset: 0x00032DA0
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = this.targetMaterial = this._instance.Material;
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00034D6C File Offset: 0x00032F6C
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x04000B61 RID: 2913
	public ApplyMaterialProperty.ApplyMode mode;

	// Token: 0x04000B62 RID: 2914
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x04000B63 RID: 2915
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x04000B64 RID: 2916
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04000B65 RID: 2917
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x04000B66 RID: 2918
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x04000B67 RID: 2919
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x02000170 RID: 368
	public enum ApplyMode
	{
		// Token: 0x04000B69 RID: 2921
		MaterialInstance,
		// Token: 0x04000B6A RID: 2922
		MaterialPropertyBlock
	}

	// Token: 0x02000171 RID: 369
	public enum SuportedTypes
	{
		// Token: 0x04000B6C RID: 2924
		Color,
		// Token: 0x04000B6D RID: 2925
		Float,
		// Token: 0x04000B6E RID: 2926
		Vector2,
		// Token: 0x04000B6F RID: 2927
		Vector3,
		// Token: 0x04000B70 RID: 2928
		Vector4,
		// Token: 0x04000B71 RID: 2929
		Texture2D
	}

	// Token: 0x02000172 RID: 370
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x060009A7 RID: 2471 RVA: 0x00034F2C File Offset: 0x0003312C
		public CustomMaterialData(string propertyName)
		{
			this.name = propertyName;
			this.id = Shader.PropertyToID(propertyName);
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00034F9C File Offset: 0x0003319C
		public CustomMaterialData(int propertyId, string propertyName)
		{
			this.name = propertyName;
			this.id = propertyId;
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00035008 File Offset: 0x00033208
		public override int GetHashCode()
		{
			return new ValueTuple<int, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.id, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x04000B72 RID: 2930
		public string name;

		// Token: 0x04000B73 RID: 2931
		public int id;

		// Token: 0x04000B74 RID: 2932
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x04000B75 RID: 2933
		public Color color;

		// Token: 0x04000B76 RID: 2934
		public float @float;

		// Token: 0x04000B77 RID: 2935
		public Vector2 vector2;

		// Token: 0x04000B78 RID: 2936
		public Vector3 vector3;

		// Token: 0x04000B79 RID: 2937
		public Vector4 vector4;

		// Token: 0x04000B7A RID: 2938
		public Texture2D texture2D;
	}
}
