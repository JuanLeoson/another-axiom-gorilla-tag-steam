using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x02000E68 RID: 3688
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06005C4F RID: 23631 RVA: 0x001D07F5 File Offset: 0x001CE9F5
		private void Awake()
		{
			StaticLodManager.gorillaInteractableLayer = UnityLayer.GorillaInteractable;
		}

		// Token: 0x06005C50 RID: 23632 RVA: 0x001D07FE File Offset: 0x001CE9FE
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.mainCamera = Camera.main;
			this.hasMainCamera = (this.mainCamera != null);
		}

		// Token: 0x06005C51 RID: 23633 RVA: 0x00010F78 File Offset: 0x0000F178
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06005C52 RID: 23634 RVA: 0x001D0824 File Offset: 0x001CEA24
		public static int Register(StaticLodGroup lodGroup)
		{
			StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			Text[] array = lodGroup.GetComponentsInChildren<Text>(true);
			List<Text> list = new List<Text>(array.Length);
			foreach (Text text in array)
			{
				StaticLodGroupExcluder componentInParent2 = text.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent2 != null) || !(componentInParent2 != componentInParent))
				{
					list.Add(text);
				}
			}
			array = list.ToArray();
			Image[] array3 = lodGroup.GetComponentsInChildren<Image>(true);
			List<Image> list2 = new List<Image>(array3.Length);
			foreach (Image image in array3)
			{
				StaticLodGroupExcluder componentInParent3 = image.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent3 != null) || !(componentInParent3 != componentInParent))
				{
					list2.Add(image);
				}
			}
			array3 = list2.ToArray();
			TextMeshPro[] array5 = lodGroup.GetComponentsInChildren<TextMeshPro>(true);
			List<TextMeshPro> list3 = new List<TextMeshPro>(array5.Length);
			foreach (TextMeshPro textMeshPro in array5)
			{
				StaticLodGroupExcluder componentInParent4 = textMeshPro.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent4 != null) || !(componentInParent4 != componentInParent))
				{
					list3.Add(textMeshPro);
				}
			}
			array5 = list3.ToArray();
			Collider[] componentsInChildren = lodGroup.GetComponentsInChildren<Collider>(true);
			List<Collider> list4 = new List<Collider>(componentsInChildren.Length);
			foreach (Collider collider in componentsInChildren)
			{
				if (collider.gameObject.IsOnLayer(StaticLodManager.gorillaInteractableLayer))
				{
					StaticLodGroupExcluder componentInParent5 = collider.GetComponentInParent<StaticLodGroupExcluder>();
					if (!(componentInParent5 != null) || !(componentInParent5 != componentInParent))
					{
						list4.Add(collider);
					}
				}
			}
			Bounds bounds;
			if (array.Length != 0)
			{
				bounds = new Bounds(array[0].transform.position, Vector3.one * 0.01f);
			}
			else if (array5.Length != 0)
			{
				bounds = new Bounds(array5[0].transform.position, Vector3.one * 0.01f);
			}
			else if (list4.Count > 0)
			{
				bounds = new Bounds(list4[0].bounds.center, list4[0].bounds.size);
			}
			else if (array3.Length != 0)
			{
				bounds = new Bounds(array3[0].transform.position, Vector3.one * 0.01f);
			}
			else
			{
				bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
			}
			foreach (Text text2 in array)
			{
				bounds.Encapsulate(text2.transform.position);
			}
			foreach (TextMeshPro textMeshPro2 in array5)
			{
				bounds.Encapsulate(textMeshPro2.transform.position);
			}
			foreach (Collider collider2 in list4)
			{
				bounds.Encapsulate(collider2.bounds);
			}
			foreach (Image image2 in list2)
			{
				bounds.Encapsulate(image2.transform.position);
			}
			StaticLodManager.GroupInfo groupInfo = new StaticLodManager.GroupInfo
			{
				isLoaded = true,
				componentEnabled = lodGroup.isActiveAndEnabled,
				center = bounds.center,
				radiusSq = bounds.extents.sqrMagnitude,
				uiEnabled = true,
				uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
				uiTexts = array,
				uiTMPs = array5,
				uiImages = array3,
				collidersEnabled = true,
				collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
				interactableColliders = list4.ToArray()
			};
			int count;
			if (StaticLodManager.freeSlots.TryPop(out count))
			{
				StaticLodManager.groupMonoBehaviours[count] = lodGroup;
				StaticLodManager.groupInfos[count] = groupInfo;
			}
			else
			{
				count = StaticLodManager.groupMonoBehaviours.Count;
				StaticLodManager.groupMonoBehaviours.Add(lodGroup);
				StaticLodManager.groupInfos.Add(groupInfo);
			}
			return count;
		}

		// Token: 0x06005C53 RID: 23635 RVA: 0x001D0C88 File Offset: 0x001CEE88
		public static void Unregister(int lodGroupIndex)
		{
			StaticLodManager.groupMonoBehaviours[lodGroupIndex] = null;
			StaticLodManager.groupInfos[lodGroupIndex] = default(StaticLodManager.GroupInfo);
			StaticLodManager.freeSlots.Push(lodGroupIndex);
		}

		// Token: 0x06005C54 RID: 23636 RVA: 0x001D0CC0 File Offset: 0x001CEEC0
		public static void SetEnabled(int index, bool enable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			StaticLodManager.GroupInfo value = StaticLodManager.groupInfos[index];
			value.componentEnabled = enable;
			StaticLodManager.groupInfos[index] = value;
		}

		// Token: 0x06005C55 RID: 23637 RVA: 0x001D0CF8 File Offset: 0x001CEEF8
		public void SliceUpdate()
		{
			if (!this.hasMainCamera)
			{
				return;
			}
			Vector3 position = this.mainCamera.transform.position;
			for (int i = 0; i < StaticLodManager.groupInfos.Count; i++)
			{
				StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[i];
				if (groupInfo.isLoaded && groupInfo.componentEnabled)
				{
					float num = Mathf.Max(0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
					float num2 = groupInfo.uiEnabled ? 0.010000001f : 0f;
					bool flag = num < groupInfo.uiEnableDistanceSq + num2;
					if (flag != groupInfo.uiEnabled)
					{
						for (int j = 0; j < groupInfo.uiTexts.Length; j++)
						{
							Text text = groupInfo.uiTexts[j];
							if (!(text == null))
							{
								text.enabled = flag;
							}
						}
						for (int k = 0; k < groupInfo.uiTMPs.Length; k++)
						{
							TextMeshPro textMeshPro = groupInfo.uiTMPs[k];
							if (!(textMeshPro == null))
							{
								textMeshPro.enabled = flag;
							}
						}
						for (int l = 0; l < groupInfo.uiImages.Length; l++)
						{
							Image image = groupInfo.uiImages[l];
							if (!(image == null))
							{
								image.enabled = flag;
							}
						}
					}
					groupInfo.uiEnabled = flag;
					num2 = (groupInfo.collidersEnabled ? 0.010000001f : 0f);
					bool flag2 = num < groupInfo.collisionEnableDistanceSq + num2;
					if (flag2 != groupInfo.collidersEnabled)
					{
						for (int m = 0; m < groupInfo.interactableColliders.Length; m++)
						{
							if (!(groupInfo.interactableColliders[m] == null))
							{
								groupInfo.interactableColliders[m].enabled = flag2;
							}
						}
					}
					groupInfo.collidersEnabled = flag2;
					StaticLodManager.groupInfos[i] = groupInfo;
				}
			}
		}

		// Token: 0x040065EB RID: 26091
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

		// Token: 0x040065EC RID: 26092
		[DebugReadout]
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32);

		// Token: 0x040065ED RID: 26093
		[OnEnterPlay_Clear]
		private static readonly Stack<int> freeSlots = new Stack<int>();

		// Token: 0x040065EE RID: 26094
		private static UnityLayer gorillaInteractableLayer;

		// Token: 0x040065EF RID: 26095
		private Camera mainCamera;

		// Token: 0x040065F0 RID: 26096
		private bool hasMainCamera;

		// Token: 0x02000E69 RID: 3689
		private struct GroupInfo
		{
			// Token: 0x040065F1 RID: 26097
			public bool isLoaded;

			// Token: 0x040065F2 RID: 26098
			public bool componentEnabled;

			// Token: 0x040065F3 RID: 26099
			public Vector3 center;

			// Token: 0x040065F4 RID: 26100
			public float radiusSq;

			// Token: 0x040065F5 RID: 26101
			public bool uiEnabled;

			// Token: 0x040065F6 RID: 26102
			public float uiEnableDistanceSq;

			// Token: 0x040065F7 RID: 26103
			public Text[] uiTexts;

			// Token: 0x040065F8 RID: 26104
			public TextMeshPro[] uiTMPs;

			// Token: 0x040065F9 RID: 26105
			public Image[] uiImages;

			// Token: 0x040065FA RID: 26106
			public bool collidersEnabled;

			// Token: 0x040065FB RID: 26107
			public float collisionEnableDistanceSq;

			// Token: 0x040065FC RID: 26108
			public Collider[] interactableColliders;
		}
	}
}
