using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EF5 RID: 3829
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x06005EF9 RID: 24313 RVA: 0x001DEF2C File Offset: 0x001DD12C
		protected void Awake()
		{
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null)
			{
				return;
			}
			this.meshes = new Mesh[componentsInChildren.Length];
			this.xforms = new Transform[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.meshes[i] = componentsInChildren[i].mesh;
				this.xforms[i] = componentsInChildren[i].transform;
			}
		}

		// Token: 0x06005EFA RID: 24314 RVA: 0x001DEF90 File Offset: 0x001DD190
		protected void OnEnable()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Transform transform = main.transform;
			Vector3 position = transform.position;
			Vector3 a = Vector3.Normalize(transform.forward);
			float nearClipPlane = main.nearClipPlane;
			float d = (main.farClipPlane - nearClipPlane) / 2f + nearClipPlane;
			Vector3 position2 = position + a * d;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				Vector3 center = this.xforms[i].InverseTransformPoint(position2);
				this.meshes[i].bounds = new Bounds(center, Vector3.one);
			}
		}

		// Token: 0x04006961 RID: 26977
		private Mesh[] meshes;

		// Token: 0x04006962 RID: 26978
		private Transform[] xforms;
	}
}
