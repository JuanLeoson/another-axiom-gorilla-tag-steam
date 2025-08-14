using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F37 RID: 3895
	public class CosmeticSwapper : MonoBehaviour
	{
		// Token: 0x06006091 RID: 24721 RVA: 0x001EB2D0 File Offset: 0x001E94D0
		private void Awake()
		{
			this.controller = CosmeticsController.instance;
		}

		// Token: 0x06006092 RID: 24722 RVA: 0x001EB2DF File Offset: 0x001E94DF
		public void SwapInCosmetic(VRRig vrRig)
		{
			this.TriggerSwap(vrRig);
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x001EB2E8 File Offset: 0x001E94E8
		public CosmeticSwapper.SwapMode GetCurrentMode()
		{
			return this.swapMode;
		}

		// Token: 0x06006094 RID: 24724 RVA: 0x001EB2F0 File Offset: 0x001E94F0
		public bool ShouldHoldFinalStep()
		{
			return this.holdFinalStep;
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x001EB2F8 File Offset: 0x001E94F8
		public int GetCurrentStepIndex(VRRig rig)
		{
			if (rig == null)
			{
				return 0;
			}
			return rig.CosmeticStepIndex;
		}

		// Token: 0x06006096 RID: 24726 RVA: 0x001EB30B File Offset: 0x001E950B
		public int GetNumberOfSteps()
		{
			return this.cosmeticIDs.Count;
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x001EB318 File Offset: 0x001E9518
		private void TriggerSwap(VRRig rig)
		{
			if (GorillaGameManager.instance != null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()))
			{
				return;
			}
			if (rig == null || this.controller == null || this.cosmeticIDs.Count == 0)
			{
				return;
			}
			if (rig != GorillaTagger.Instance.offlineVRRig)
			{
				return;
			}
			rig.SetCosmeticSwapper(this, this.stepTimeout);
			if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
			{
				foreach (string nameOrId in this.cosmeticIDs)
				{
					CosmeticSwapper.CosmeticState? cosmeticState = this.SwapInCosmeticWithReturn(nameOrId, rig);
					if (cosmeticState != null)
					{
						rig.AddNewSwappedCosmetic(cosmeticState.Value);
					}
				}
				return;
			}
			int cosmeticStepIndex = rig.CosmeticStepIndex;
			if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
			{
				return;
			}
			string nameOrId2 = this.cosmeticIDs[cosmeticStepIndex];
			CosmeticSwapper.CosmeticState? cosmeticState2 = this.SwapInCosmeticWithReturn(nameOrId2, rig);
			if (cosmeticState2 != null)
			{
				rig.AddNewSwappedCosmetic(cosmeticState2.Value);
				if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
				{
					if (this.holdFinalStep)
					{
						rig.MarkFinalCosmeticStep();
					}
					if (this.OnSwappingSequenceCompleted != null)
					{
						this.OnSwappingSequenceCompleted.Invoke(rig);
						return;
					}
				}
				else
				{
					rig.UnmarkFinalCosmeticStep();
				}
			}
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x001EB478 File Offset: 0x001E9678
		private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			if (this.controller == null)
			{
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(nameOrId);
			if (cosmeticItem.isNullItem)
			{
				Debug.LogWarning("Cosmetic not found: " + nameOrId);
				return null;
			}
			bool isLeftHand;
			CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(cosmeticItem, out isLeftHand);
			if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
			{
				Debug.LogWarning("Could not determine slot for: " + cosmeticItem.displayName);
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem2 = this.controller.currentWornSet.items[(int)cosmeticSlot];
			this.controller.TemporaryUnlock(rig, nameOrId);
			this.controller.RemoveCosmeticItemFromSet(this.controller.currentWornSet, cosmeticItem2.displayName, false);
			this.controller.ApplyCosmeticItemToSet(this.controller.currentWornSet, cosmeticItem, isLeftHand, false);
			this.controller.UpdateWornCosmetics(true);
			this.ApplyToRig(rig);
			return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState
			{
				cosmeticId = nameOrId,
				replacedItem = cosmeticItem2,
				slot = cosmeticSlot,
				isLeftHand = isLeftHand
			});
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x001EB594 File Offset: 0x001E9794
		public void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state, VRRig rig)
		{
			if (this.controller == null)
			{
				return;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			this.controller.RemoveCosmeticItemFromSet(this.controller.currentWornSet, cosmeticItem.displayName, false);
			this.controller.ClearTemporaryUnlocks(rig, cosmeticItem.displayName);
			if (!state.replacedItem.isNullItem)
			{
				this.controller.TemporaryUnlock(rig, state.replacedItem.itemName);
				this.controller.ApplyCosmeticItemToSet(this.controller.currentWornSet, state.replacedItem, state.isLeftHand, false);
			}
			this.controller.UpdateWornCosmetics(true);
			this.ApplyToRig(rig);
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x001EB650 File Offset: 0x001E9850
		private void ApplyToRig(VRRig vrRig)
		{
			if (vrRig == null)
			{
				Debug.LogWarning("VRRig is null");
				return;
			}
			BodyDockPositions component = vrRig.GetComponent<BodyDockPositions>();
			CosmeticItemRegistry cosmeticsObjectRegistry = vrRig.cosmeticsObjectRegistry;
			this.controller.currentWornSet.ActivateCosmetics(this.controller.tryOnSet, vrRig, component, cosmeticsObjectRegistry);
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x001EB6A0 File Offset: 0x001E98A0
		private CosmeticsController.CosmeticItem FindItem(string nameOrId)
		{
			CosmeticsController.CosmeticItem result;
			if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, out result))
			{
				return result;
			}
			string itemID;
			if (this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, out itemID))
			{
				return this.controller.GetItemFromDict(itemID);
			}
			return this.controller.nullItem;
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001EB6F4 File Offset: 0x001E98F4
		private CosmeticsController.CosmeticSlots GetCosmeticSlot(CosmeticsController.CosmeticItem item, out bool isLeftHand)
		{
			isLeftHand = false;
			if (!item.isHoldable)
			{
				return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
			}
			CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
			CosmeticsController.CosmeticItem cosmeticItem = currentWornSet.items[7];
			CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
			if (cosmeticItem.isNullItem || (!cosmeticItem2.isNullItem && item.itemName == cosmeticItem.itemName))
			{
				isLeftHand = true;
			}
			if (!isLeftHand)
			{
				return CosmeticsController.CosmeticSlots.HandRight;
			}
			return CosmeticsController.CosmeticSlots.HandLeft;
		}

		// Token: 0x04006C2B RID: 27691
		[SerializeField]
		private List<string> cosmeticIDs = new List<string>();

		// Token: 0x04006C2C RID: 27692
		[SerializeField]
		private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;

		// Token: 0x04006C2D RID: 27693
		[SerializeField]
		private float stepTimeout = 10f;

		// Token: 0x04006C2E RID: 27694
		[Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
		[SerializeField]
		private bool holdFinalStep = true;

		// Token: 0x04006C2F RID: 27695
		[SerializeField]
		private UnityEvent<VRRig> OnSwappingSequenceCompleted;

		// Token: 0x04006C30 RID: 27696
		[SerializeField]
		private List<GameModeType> gameModeExclusion = new List<GameModeType>();

		// Token: 0x04006C31 RID: 27697
		private CosmeticsController controller;

		// Token: 0x02000F38 RID: 3896
		public enum SwapMode
		{
			// Token: 0x04006C33 RID: 27699
			AllAtOnce,
			// Token: 0x04006C34 RID: 27700
			StepByStep
		}

		// Token: 0x02000F39 RID: 3897
		public struct CosmeticState
		{
			// Token: 0x04006C35 RID: 27701
			public string cosmeticId;

			// Token: 0x04006C36 RID: 27702
			public CosmeticsController.CosmeticItem replacedItem;

			// Token: 0x04006C37 RID: 27703
			public CosmeticsController.CosmeticSlots slot;

			// Token: 0x04006C38 RID: 27704
			public bool isLeftHand;
		}
	}
}
