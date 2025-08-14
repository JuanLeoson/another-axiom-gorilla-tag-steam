using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D17 RID: 3351
	public class InteractableToolsInputRouter : MonoBehaviour
	{
		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x060052DA RID: 21210 RVA: 0x0019AF38 File Offset: 0x00199138
		public static InteractableToolsInputRouter Instance
		{
			get
			{
				if (InteractableToolsInputRouter._instance == null)
				{
					InteractableToolsInputRouter[] array = Object.FindObjectsOfType<InteractableToolsInputRouter>();
					if (array.Length != 0)
					{
						InteractableToolsInputRouter._instance = array[0];
						for (int i = 1; i < array.Length; i++)
						{
							Object.Destroy(array[i].gameObject);
						}
					}
				}
				return InteractableToolsInputRouter._instance;
			}
		}

		// Token: 0x060052DB RID: 21211 RVA: 0x0019AF84 File Offset: 0x00199184
		public void RegisterInteractableTool(InteractableTool interactableTool)
		{
			if (interactableTool.IsRightHandedTool)
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._rightHandFarTools.Add(interactableTool);
					return;
				}
				this._rightHandNearTools.Add(interactableTool);
				return;
			}
			else
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._leftHandFarTools.Add(interactableTool);
					return;
				}
				this._leftHandNearTools.Add(interactableTool);
				return;
			}
		}

		// Token: 0x060052DC RID: 21212 RVA: 0x0019AFE0 File Offset: 0x001991E0
		public void UnregisterInteractableTool(InteractableTool interactableTool)
		{
			if (interactableTool.IsRightHandedTool)
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._rightHandFarTools.Remove(interactableTool);
					return;
				}
				this._rightHandNearTools.Remove(interactableTool);
				return;
			}
			else
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._leftHandFarTools.Remove(interactableTool);
					return;
				}
				this._leftHandNearTools.Remove(interactableTool);
				return;
			}
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x0019B03C File Offset: 0x0019923C
		private void Update()
		{
			if (!HandsManager.Instance.IsInitialized())
			{
				return;
			}
			bool flag = HandsManager.Instance.LeftHand.IsTracked && HandsManager.Instance.LeftHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool flag2 = HandsManager.Instance.RightHand.IsTracked && HandsManager.Instance.RightHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool isPointerPoseValid = HandsManager.Instance.LeftHand.IsPointerPoseValid;
			bool isPointerPoseValid2 = HandsManager.Instance.RightHand.IsPointerPoseValid;
			bool flag3 = this.UpdateToolsAndEnableState(this._leftHandNearTools, flag);
			this.UpdateToolsAndEnableState(this._leftHandFarTools, !flag3 && flag && isPointerPoseValid);
			bool flag4 = this.UpdateToolsAndEnableState(this._rightHandNearTools, flag2);
			this.UpdateToolsAndEnableState(this._rightHandFarTools, !flag4 && flag2 && isPointerPoseValid2);
		}

		// Token: 0x060052DE RID: 21214 RVA: 0x0019B116 File Offset: 0x00199316
		private bool UpdateToolsAndEnableState(HashSet<InteractableTool> tools, bool toolsAreEnabledThisFrame)
		{
			bool result = this.UpdateTools(tools, !toolsAreEnabledThisFrame);
			this.ToggleToolsEnableState(tools, toolsAreEnabledThisFrame);
			return result;
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x0019B12C File Offset: 0x0019932C
		private bool UpdateTools(HashSet<InteractableTool> tools, bool resetCollisionData = false)
		{
			bool flag = false;
			foreach (InteractableTool interactableTool in tools)
			{
				List<InteractableCollisionInfo> nextIntersectingObjects = interactableTool.GetNextIntersectingObjects();
				if (nextIntersectingObjects.Count > 0 && !resetCollisionData)
				{
					if (!flag)
					{
						flag = (nextIntersectingObjects.Count > 0);
					}
					interactableTool.UpdateCurrentCollisionsBasedOnDepth();
					if (interactableTool.IsFarFieldTool)
					{
						KeyValuePair<Interactable, InteractableCollisionInfo> firstCurrentCollisionInfo = interactableTool.GetFirstCurrentCollisionInfo();
						if (interactableTool.ToolInputState == ToolInputState.PrimaryInputUp)
						{
							firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ActionCollider;
							firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Action;
						}
						else
						{
							firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ContactCollider;
							firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Contact;
						}
						interactableTool.FocusOnInteractable(firstCurrentCollisionInfo.Key, firstCurrentCollisionInfo.Value.InteractableCollider);
					}
				}
				else
				{
					interactableTool.DeFocus();
					interactableTool.ClearAllCurrentCollisionInfos();
				}
				interactableTool.UpdateLatestCollisionData();
			}
			return flag;
		}

		// Token: 0x060052E0 RID: 21216 RVA: 0x0019B240 File Offset: 0x00199440
		private void ToggleToolsEnableState(HashSet<InteractableTool> tools, bool enableState)
		{
			foreach (InteractableTool interactableTool in tools)
			{
				if (interactableTool.EnableState != enableState)
				{
					interactableTool.EnableState = enableState;
				}
			}
		}

		// Token: 0x04005C3B RID: 23611
		private static InteractableToolsInputRouter _instance;

		// Token: 0x04005C3C RID: 23612
		private bool _leftPinch;

		// Token: 0x04005C3D RID: 23613
		private bool _rightPinch;

		// Token: 0x04005C3E RID: 23614
		private HashSet<InteractableTool> _leftHandNearTools = new HashSet<InteractableTool>();

		// Token: 0x04005C3F RID: 23615
		private HashSet<InteractableTool> _leftHandFarTools = new HashSet<InteractableTool>();

		// Token: 0x04005C40 RID: 23616
		private HashSet<InteractableTool> _rightHandNearTools = new HashSet<InteractableTool>();

		// Token: 0x04005C41 RID: 23617
		private HashSet<InteractableTool> _rightHandFarTools = new HashSet<InteractableTool>();
	}
}
