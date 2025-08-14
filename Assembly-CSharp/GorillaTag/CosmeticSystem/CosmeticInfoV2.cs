using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED4 RID: 3796
	[Serializable]
	public struct CosmeticInfoV2 : ISerializationCallbackReceiver
	{
		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06005E72 RID: 24178 RVA: 0x001DC300 File Offset: 0x001DA500
		public bool hasHoldableParts
		{
			get
			{
				CosmeticPart[] array = this.holdableParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06005E73 RID: 24179 RVA: 0x001DC320 File Offset: 0x001DA520
		public bool hasWardrobeParts
		{
			get
			{
				CosmeticPart[] array = this.wardrobeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06005E74 RID: 24180 RVA: 0x001DC340 File Offset: 0x001DA540
		public bool hasStoreParts
		{
			get
			{
				CosmeticPart[] array = this.storeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06005E75 RID: 24181 RVA: 0x001DC360 File Offset: 0x001DA560
		public bool hasFunctionalParts
		{
			get
			{
				CosmeticPart[] array = this.functionalParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06005E76 RID: 24182 RVA: 0x001DC380 File Offset: 0x001DA580
		public bool hasFirstPersonViewParts
		{
			get
			{
				CosmeticPart[] array = this.firstPersonViewParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x001DC3A0 File Offset: 0x001DA5A0
		public CosmeticInfoV2(string displayName)
		{
			this.enabled = true;
			this.season = null;
			this.displayName = displayName;
			this.playFabID = "";
			this.category = CosmeticsController.CosmeticCategory.None;
			this.icon = null;
			this.isHoldable = false;
			this.isThrowable = false;
			this.usesBothHandSlots = false;
			this.hideWardrobeMannequin = false;
			this.holdableParts = new CosmeticPart[0];
			this.functionalParts = new CosmeticPart[0];
			this.wardrobeParts = new CosmeticPart[0];
			this.storeParts = new CosmeticPart[0];
			this.firstPersonViewParts = new CosmeticPart[0];
			this.setCosmetics = new CosmeticSO[0];
			this.anchorAntiIntersectOffsets = default(CosmeticAnchorAntiIntersectOffsets);
			this.debugCosmeticSOName = "__UNINITIALIZED__";
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x000023F5 File Offset: 0x000005F5
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x001DC45C File Offset: 0x001DA65C
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
			this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
			this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
			this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
			this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
			if (this.setCosmetics == null)
			{
				this.setCosmetics = Array.Empty<CosmeticSO>();
			}
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x001DC4C0 File Offset: 0x001DA6C0
		private void _OnAfterDeserialize_InitializePartsArray(ref CosmeticPart[] parts, ECosmeticPartType partType)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i].partType = partType;
				ref CosmeticAttachInfo[] ptr = ref parts[i].attachAnchors;
				if (ptr == null)
				{
					ptr = Array.Empty<CosmeticAttachInfo>();
				}
			}
		}

		// Token: 0x04006861 RID: 26721
		public bool enabled;

		// Token: 0x04006862 RID: 26722
		[Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
		[Delayed]
		public SeasonSO season;

		// Token: 0x04006863 RID: 26723
		[Tooltip("Name that is displayed in the store during purchasing.")]
		[Delayed]
		public string displayName;

		// Token: 0x04006864 RID: 26724
		[Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
		[Delayed]
		public string playFabID;

		// Token: 0x04006865 RID: 26725
		public Sprite icon;

		// Token: 0x04006866 RID: 26726
		[Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
		public StringEnum<CosmeticsController.CosmeticCategory> category;

		// Token: 0x04006867 RID: 26727
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
		[HideInInspector]
		public bool isHoldable;

		// Token: 0x04006868 RID: 26728
		public bool isThrowable;

		// Token: 0x04006869 RID: 26729
		public bool usesBothHandSlots;

		// Token: 0x0400686A RID: 26730
		public bool hideWardrobeMannequin;

		// Token: 0x0400686B RID: 26731
		public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";

		// Token: 0x0400686C RID: 26732
		public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components which are parented to slots in \"Gorilla Player Networked.prefab\". Which slots can be used by the \n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x0400686D RID: 26733
		[Space]
		[Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components which are parented to slots in \"Gorilla Player Networked.prefab\". Which slots can be used by the \n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] holdableParts;

		// Token: 0x0400686E RID: 26734
		public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";

		// Token: 0x0400686F RID: 26735
		public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nAny parts attached to the head (like hats) are hidden from local camera view. To make it visible from first person: create a first person version of the prefab and assign it to the \"First Person\" array below.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x04006870 RID: 26736
		[Space]
		[Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nAny parts attached to the head (like hats) are hidden from local camera view. To make it visible from first person: create a first person version of the prefab and assign it to the \"First Person\" array below.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] functionalParts;

		// Token: 0x04006871 RID: 26737
		public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x04006872 RID: 26738
		public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x04006873 RID: 26739
		[Space]
		[Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] wardrobeParts;

		// Token: 0x04006874 RID: 26740
		[Space]
		[Tooltip("TODO")]
		public CosmeticPart[] storeParts;

		// Token: 0x04006875 RID: 26741
		public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x04006876 RID: 26742
		public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x04006877 RID: 26743
		[Space]
		[Tooltip("\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] firstPersonViewParts;

		// Token: 0x04006878 RID: 26744
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets;

		// Token: 0x04006879 RID: 26745
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticSO[] setCosmetics;

		// Token: 0x0400687A RID: 26746
		[NonSerialized]
		public string debugCosmeticSOName;
	}
}
