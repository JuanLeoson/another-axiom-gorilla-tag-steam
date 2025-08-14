using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200029F RID: 671
public class MaterialCycler : MonoBehaviour
{
	// Token: 0x06000F78 RID: 3960 RVA: 0x0005B437 File Offset: 0x00059637
	private void Awake()
	{
		this.materialCyclerNetworked = base.GetComponent<MaterialCyclerNetworked>();
		this.SetMaterials();
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x0005B44B File Offset: 0x0005964B
	private void OnEnable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize += this.MaterialCyclerNetworked_OnSynchronize;
		}
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x0005B472 File Offset: 0x00059672
	private void OnDisable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize -= this.MaterialCyclerNetworked_OnSynchronize;
		}
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0005B49C File Offset: 0x0005969C
	private void MaterialCyclerNetworked_OnSynchronize(int idx, int3 rgb)
	{
		if (idx < 0 || idx >= this.materials.Length)
		{
			return;
		}
		this.index = idx;
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material = this.materials[this.index].Materials[i];
			this.renderers[i].material.SetColor(this.setColorTarget, new Color((float)rgb.x / 9f, (float)rgb.y / 9f, (float)rgb.z / 9f));
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0005B590 File Offset: 0x00059790
	private void SetMaterials()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (this.materials[this.index].Materials.Length > i)
			{
				this.renderers[i].material = this.materials[this.index].Materials[i];
			}
			else
			{
				this.renderers[i].material = null;
			}
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0005B64F File Offset: 0x0005984F
	public void NextMaterial()
	{
		this.index = (this.index + 1) % this.materials.Length;
		this.SetMaterials();
		this.SetDirty();
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0005B674 File Offset: 0x00059874
	private void SetDirty()
	{
		if (this.materialCyclerNetworked == null)
		{
			return;
		}
		this.synchTime = Time.time + this.materialCyclerNetworked.SyncTimeOut;
		if (this.crDirty == null)
		{
			this.crDirty = base.StartCoroutine(this.timeOutDirty());
		}
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0005B6C1 File Offset: 0x000598C1
	private IEnumerator timeOutDirty()
	{
		while (this.synchTime > Time.time)
		{
			yield return null;
		}
		this.synchronize();
		this.crDirty = null;
		yield break;
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0005B6D0 File Offset: 0x000598D0
	private void synchronize()
	{
		this.materialCyclerNetworked.Synchronize(this.index, this.renderers[0].material.color);
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0005B6F8 File Offset: 0x000598F8
	public void SetColor(Vector3 rgb)
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material.SetColor(this.setColorTarget, new Color(rgb.x, rgb.y, rgb.z));
		}
		this.SetDirty();
	}

	// Token: 0x0400182A RID: 6186
	[SerializeField]
	private MaterialCycler.MaterialPack[] materials;

	// Token: 0x0400182B RID: 6187
	[SerializeField]
	private Renderer[] renderers;

	// Token: 0x0400182C RID: 6188
	private int index;

	// Token: 0x0400182D RID: 6189
	[SerializeField]
	private string setColorTarget = "_BaseColor";

	// Token: 0x0400182E RID: 6190
	[SerializeField]
	private UnityEvent<Vector3> reset;

	// Token: 0x0400182F RID: 6191
	private Coroutine crDirty;

	// Token: 0x04001830 RID: 6192
	private float synchTime;

	// Token: 0x04001831 RID: 6193
	private MaterialCyclerNetworked materialCyclerNetworked;

	// Token: 0x020002A0 RID: 672
	[Serializable]
	private class MaterialPack
	{
		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0005B760 File Offset: 0x00059960
		public Material[] Materials
		{
			get
			{
				return this.materials;
			}
		}

		// Token: 0x04001832 RID: 6194
		[SerializeField]
		private Material[] materials;
	}
}
