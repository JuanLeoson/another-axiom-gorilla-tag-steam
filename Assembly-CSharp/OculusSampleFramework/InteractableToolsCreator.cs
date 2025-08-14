using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D15 RID: 3349
	public class InteractableToolsCreator : MonoBehaviour
	{
		// Token: 0x060052CF RID: 21199 RVA: 0x0019AC84 File Offset: 0x00198E84
		private void Awake()
		{
			if (this.LeftHandTools != null && this.LeftHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.LeftHandTools, false));
			}
			if (this.RightHandTools != null && this.RightHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.RightHandTools, true));
			}
		}

		// Token: 0x060052D0 RID: 21200 RVA: 0x0019ACDB File Offset: 0x00198EDB
		private IEnumerator AttachToolsToHands(Transform[] toolObjects, bool isRightHand)
		{
			HandsManager handsManagerObj = null;
			while ((handsManagerObj = HandsManager.Instance) == null || !handsManagerObj.IsInitialized())
			{
				yield return null;
			}
			HashSet<Transform> hashSet = new HashSet<Transform>();
			foreach (Transform transform in toolObjects)
			{
				hashSet.Add(transform.transform);
			}
			foreach (Transform toolObject in hashSet)
			{
				OVRSkeleton handSkeletonToAttachTo = isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton;
				while (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
				{
					yield return null;
				}
				this.AttachToolToHandTransform(toolObject, isRightHand);
				handSkeletonToAttachTo = null;
				toolObject = null;
			}
			HashSet<Transform>.Enumerator enumerator = default(HashSet<Transform>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060052D1 RID: 21201 RVA: 0x0019ACF8 File Offset: 0x00198EF8
		private void AttachToolToHandTransform(Transform tool, bool isRightHanded)
		{
			Transform transform = Object.Instantiate<Transform>(tool).transform;
			transform.localPosition = Vector3.zero;
			InteractableTool component = transform.GetComponent<InteractableTool>();
			component.IsRightHandedTool = isRightHanded;
			component.Initialize();
		}

		// Token: 0x04005C30 RID: 23600
		[SerializeField]
		private Transform[] LeftHandTools;

		// Token: 0x04005C31 RID: 23601
		[SerializeField]
		private Transform[] RightHandTools;
	}
}
