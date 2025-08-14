using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD6 RID: 3542
	public class PoseableMannequin : MonoBehaviour
	{
		// Token: 0x060057E8 RID: 22504 RVA: 0x001B4B95 File Offset: 0x001B2D95
		public void Start()
		{
			this.skinnedMeshRenderer.gameObject.SetActive(false);
			this.staticGorillaMesh.gameObject.SetActive(true);
		}

		// Token: 0x060057E9 RID: 22505 RVA: 0x0015D6C8 File Offset: 0x0015B8C8
		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		// Token: 0x060057EA RID: 22506 RVA: 0x0015D6C8 File Offset: 0x0015B8C8
		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		// Token: 0x060057EB RID: 22507 RVA: 0x001B4BB9 File Offset: 0x001B2DB9
		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		// Token: 0x060057EC RID: 22508 RVA: 0x000023F5 File Offset: 0x000005F5
		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		// Token: 0x060057ED RID: 22509 RVA: 0x001B4BCD File Offset: 0x001B2DCD
		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x060057EE RID: 22510 RVA: 0x001B4C07 File Offset: 0x001B2E07
		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x060057EF RID: 22511 RVA: 0x001B4C1C File Offset: 0x001B2E1C
		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		// Token: 0x060057F0 RID: 22512 RVA: 0x001B4C6C File Offset: 0x001B2E6C
		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		// Token: 0x060057F1 RID: 22513 RVA: 0x001B4CE0 File Offset: 0x001B2EE0
		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x060057F2 RID: 22514 RVA: 0x000023F5 File Offset: 0x000005F5
		public void CreasteTestClip()
		{
		}

		// Token: 0x060057F3 RID: 22515 RVA: 0x001B4D1C File Offset: 0x001B2F1C
		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		// Token: 0x060057F4 RID: 22516 RVA: 0x001B4D2B File Offset: 0x001B2F2B
		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		// Token: 0x060057F5 RID: 22517 RVA: 0x000023F5 File Offset: 0x000005F5
		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		// Token: 0x060057F6 RID: 22518 RVA: 0x001B4D34 File Offset: 0x001B2F34
		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] keys2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] keys3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] keys4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve curve = new AnimationCurve(keys);
			AnimationCurve curve2 = new AnimationCurve(keys2);
			AnimationCurve curve3 = new AnimationCurve(keys3);
			AnimationCurve curve4 = new AnimationCurve(keys4);
			string relativePath = "";
			string b = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == b)
				{
					relativePath = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.x", curve);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.y", curve2);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.z", curve3);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.w", curve4);
		}

		// Token: 0x060057F7 RID: 22519 RVA: 0x000023F5 File Offset: 0x000005F5
		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		// Token: 0x060057F8 RID: 22520 RVA: 0x000023F5 File Offset: 0x000005F5
		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		// Token: 0x060057F9 RID: 22521 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnValidate()
		{
		}

		// Token: 0x040061A9 RID: 25001
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x040061AA RID: 25002
		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		// Token: 0x040061AB RID: 25003
		public GTPosRotConstraints[] cosmeticConstraints;

		// Token: 0x040061AC RID: 25004
		public Mesh BakedColliderMesh;

		// Token: 0x040061AD RID: 25005
		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		// Token: 0x040061AE RID: 25006
		[SerializeField]
		protected string prefabFolderPath;

		// Token: 0x040061AF RID: 25007
		[SerializeField]
		protected string prefabAssetName;

		// Token: 0x040061B0 RID: 25008
		public MeshFilter staticGorillaMesh;

		// Token: 0x040061B1 RID: 25009
		public MeshCollider staticGorillaMeshCollider;

		// Token: 0x040061B2 RID: 25010
		public MeshRenderer staticGorillaMeshRenderer;
	}
}
