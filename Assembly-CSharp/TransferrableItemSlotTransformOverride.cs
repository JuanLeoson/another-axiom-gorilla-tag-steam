using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200045C RID: 1116
public class TransferrableItemSlotTransformOverride : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001B6A RID: 7018 RVA: 0x00092DFF File Offset: 0x00090FFF
	// (set) Token: 0x06001B6B RID: 7019 RVA: 0x00092E07 File Offset: 0x00091007
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001B6C RID: 7020 RVA: 0x00092E10 File Offset: 0x00091010
	// (set) Token: 0x06001B6D RID: 7021 RVA: 0x00092E18 File Offset: 0x00091018
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001B6E RID: 7022 RVA: 0x00092E24 File Offset: 0x00091024
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.defaultPosition = new SlotTransformOverride
		{
			positionState = TransferrableObject.PositionState.None
		};
		this.lastPosition = TransferrableObject.PositionState.None;
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			slotTransformOverride.Initialize(this, this.anchor);
		}
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x00092E94 File Offset: 0x00091094
	public void AddGripPosition(TransferrableObject.PositionState state, TransferrableObjectGripPosition togp)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if (slotTransformOverride.positionState == state)
			{
				slotTransformOverride.AddSubGrabPoint(togp);
				return;
			}
		}
		SlotTransformOverride slotTransformOverride2 = new SlotTransformOverride
		{
			positionState = state
		};
		this.transformOverridesDeprecated.Add(slotTransformOverride2);
		slotTransformOverride2.AddSubGrabPoint(togp);
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x00092F14 File Offset: 0x00091114
	public void SliceUpdate()
	{
		if (this.followingTransferrableObject == null)
		{
			return;
		}
		if (this.followingTransferrableObject.currentState != this.lastPosition)
		{
			SlotTransformOverride slotTransformOverride = this.transformOverridesDeprecated.Find((SlotTransformOverride x) => (x.positionState & this.followingTransferrableObject.currentState) > TransferrableObject.PositionState.None);
			if (slotTransformOverride != null && slotTransformOverride.positionState == TransferrableObject.PositionState.None)
			{
				slotTransformOverride = this.defaultPosition;
			}
		}
		this.lastPosition = this.followingTransferrableObject.currentState;
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x00092F7E File Offset: 0x0009117E
	private void Awake()
	{
		this.GenerateTransformFromPositionState();
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x00092F88 File Offset: 0x00091188
	public void GenerateTransformFromPositionState()
	{
		this.transformFromPosition = new Dictionary<TransferrableObject.PositionState, Transform>();
		if (this.transformOverridesDeprecated.Count > 0)
		{
			this.transformFromPosition[TransferrableObject.PositionState.None] = this.transformOverridesDeprecated[0].overrideTransform;
		}
		foreach (TransferrableObject.PositionState positionState in Enum.GetValues(typeof(TransferrableObject.PositionState)).Cast<TransferrableObject.PositionState>())
		{
			if (positionState == TransferrableObject.PositionState.None)
			{
				this.transformFromPosition[positionState] = null;
			}
			else
			{
				Transform value = null;
				foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
				{
					if ((slotTransformOverride.positionState & positionState) != TransferrableObject.PositionState.None)
					{
						value = slotTransformOverride.overrideTransform;
						break;
					}
				}
				this.transformFromPosition[positionState] = value;
			}
		}
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00093084 File Offset: 0x00091284
	[CanBeNull]
	public Transform GetTransformFromPositionState(TransferrableObject.PositionState currentState)
	{
		if (this.transformFromPosition == null)
		{
			this.GenerateTransformFromPositionState();
		}
		return this.transformFromPosition[currentState];
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x000930A0 File Offset: 0x000912A0
	public bool GetTransformFromPositionState(TransferrableObject.PositionState currentState, AdvancedItemState advancedItemState, Transform targetDockXf, out Matrix4x4 matrix4X4)
	{
		if (currentState != TransferrableObject.PositionState.None)
		{
			foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
			{
				if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None)
				{
					if (!slotTransformOverride.useAdvancedGrab)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					if (advancedItemState.index >= slotTransformOverride.multiPoints.Count)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[advancedItemState.index];
					matrix4X4 = subGrabPoint.GetTransformFromPositionState(advancedItemState, slotTransformOverride, targetDockXf);
					return true;
				}
			}
			matrix4X4 = Matrix4x4.identity;
			return false;
		}
		if (this.transformOverridesDeprecated.Count > 0)
		{
			matrix4X4 = this.transformOverridesDeprecated[0].overrideTransformMatrix;
			return true;
		}
		matrix4X4 = Matrix4x4.identity;
		return false;
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000931A4 File Offset: 0x000913A4
	public AdvancedItemState GetAdvancedItemStateFromHand(TransferrableObject.PositionState currentState, Transform handTransform, Transform targetDock)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None && slotTransformOverride.multiPoints.Count != 0)
			{
				SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[0];
				float num = float.PositiveInfinity;
				int index = -1;
				for (int i = 0; i < slotTransformOverride.multiPoints.Count; i++)
				{
					SubGrabPoint subGrabPoint2 = slotTransformOverride.multiPoints[i];
					if (!(subGrabPoint2.gripPoint == null))
					{
						float num2 = subGrabPoint2.EvaluateScore(base.transform, handTransform, targetDock);
						if (num2 < num)
						{
							subGrabPoint = subGrabPoint2;
							num = num2;
							index = i;
						}
					}
				}
				AdvancedItemState advancedItemStateFromHand = subGrabPoint.GetAdvancedItemStateFromHand(base.transform, handTransform, targetDock, slotTransformOverride);
				advancedItemStateFromHand.index = index;
				return advancedItemStateFromHand;
			}
		}
		return new AdvancedItemState();
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000932A4 File Offset: 0x000914A4
	public void Edit()
	{
		if (TransferrableItemSlotTransformOverride.OnBringUpWindow != null)
		{
			TransferrableItemSlotTransformOverride.OnBringUpWindow(base.GetComponent<TransferrableObject>());
		}
	}

	// Token: 0x040023EF RID: 9199
	[FormerlySerializedAs("transformOverridesList")]
	public List<SlotTransformOverride> transformOverridesDeprecated;

	// Token: 0x040023F0 RID: 9200
	[SerializeReference]
	public List<SlotTransformOverride> transformOverrides;

	// Token: 0x040023F1 RID: 9201
	private TransferrableObject.PositionState lastPosition;

	// Token: 0x040023F2 RID: 9202
	[Tooltip("(2024-08-20 MattO) For cosmetics this is almost always assigned to the TransferrableObject component in the same prefab and almost always belonging to the same gameobject as this Component.")]
	public TransferrableObject followingTransferrableObject;

	// Token: 0x040023F3 RID: 9203
	[Tooltip("(2024-08-20 MattO) This is filled in automatically by the cosmetic spawner.")]
	public SlotTransformOverride defaultPosition;

	// Token: 0x040023F4 RID: 9204
	[Obsolete("(2024-08-2024) This used to be assigned to `defaultPosition.overrideTransform` before, but was there ever an instance where it wasn't null? Keeping it serialized just in case there is a reason for it.")]
	public Transform defaultTransform;

	// Token: 0x040023F5 RID: 9205
	public Transform anchor;

	// Token: 0x040023F8 RID: 9208
	public Dictionary<TransferrableObject.PositionState, Transform> transformFromPosition;

	// Token: 0x040023F9 RID: 9209
	public static Action<TransferrableObject> OnBringUpWindow;
}
