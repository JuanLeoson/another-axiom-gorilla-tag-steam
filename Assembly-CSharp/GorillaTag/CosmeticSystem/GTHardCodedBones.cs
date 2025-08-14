using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000EDB RID: 3803
	public static class GTHardCodedBones
	{
		// Token: 0x06005E7D RID: 24189 RVA: 0x001DC535 File Offset: 0x001DA735
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void HandleRuntimeInitialize_OnBeforeSceneLoad()
		{
			VRRigCache.OnPostInitialize += GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
		}

		// Token: 0x06005E7E RID: 24190 RVA: 0x001DC548 File Offset: 0x001DA748
		private static void HandleVRRigCache_OnPostInitialize()
		{
			VRRigCache.OnPostInitialize -= GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
			GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig();
			VRRigCache.OnPostSpawnRig += GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig;
		}

		// Token: 0x06005E7F RID: 24191 RVA: 0x001DC574 File Offset: 0x001DA774
		private static void HandleVRRigCache_OnPostSpawnRig()
		{
			if (!VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			for (int i = 0; i < allRigs.Length; i++)
			{
				Transform[] array;
				string str;
				if (!GTHardCodedBones.TryGetBoneXforms(allRigs[i], out array, out str))
				{
					Debug.LogError("GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig: Error getting bone Transforms: " + str, allRigs[i]);
					return;
				}
			}
		}

		// Token: 0x06005E80 RID: 24192 RVA: 0x0005570E File Offset: 0x0005390E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(GTHardCodedBones.EBone bone)
		{
			return (int)bone;
		}

		// Token: 0x06005E81 RID: 24193 RVA: 0x001DC5CC File Offset: 0x001DA7CC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(string name)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06005E82 RID: 24194 RVA: 0x001DC600 File Offset: 0x001DA800
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneIndexByName(string name, out int out_index)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					out_index = i;
					return true;
				}
			}
			out_index = 0;
			return false;
		}

		// Token: 0x06005E83 RID: 24195 RVA: 0x001DC637 File Offset: 0x001DA837
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBone(string name)
		{
			return (GTHardCodedBones.EBone)GTHardCodedBones.GetBoneIndex(name);
		}

		// Token: 0x06005E84 RID: 24196 RVA: 0x001DC640 File Offset: 0x001DA840
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneByName(string name, out GTHardCodedBones.EBone out_eBone)
		{
			int num;
			if (GTHardCodedBones.TryGetBoneIndexByName(name, out num))
			{
				out_eBone = (GTHardCodedBones.EBone)num;
				return true;
			}
			out_eBone = GTHardCodedBones.EBone.None;
			return false;
		}

		// Token: 0x06005E85 RID: 24197 RVA: 0x001DC660 File Offset: 0x001DA860
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(int boneIndex)
		{
			return GTHardCodedBones.kBoneNames[boneIndex];
		}

		// Token: 0x06005E86 RID: 24198 RVA: 0x001DC669 File Offset: 0x001DA869
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(int boneIndex, out string out_name)
		{
			if (boneIndex >= 0 && boneIndex < GTHardCodedBones.kBoneNames.Length)
			{
				out_name = GTHardCodedBones.kBoneNames[boneIndex];
				return true;
			}
			out_name = "None";
			return false;
		}

		// Token: 0x06005E87 RID: 24199 RVA: 0x001DC68C File Offset: 0x001DA88C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(GTHardCodedBones.EBone bone)
		{
			return GTHardCodedBones.GetBoneName((int)bone);
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x001DC694 File Offset: 0x001DA894
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(GTHardCodedBones.EBone bone, out string out_name)
		{
			return GTHardCodedBones.TryGetBoneName((int)bone, out out_name);
		}

		// Token: 0x06005E89 RID: 24201 RVA: 0x001DC6A0 File Offset: 0x001DA8A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(string name)
		{
			if (name == "None")
			{
				return 0L;
			}
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return 1L << i - 1;
				}
			}
			return 0L;
		}

		// Token: 0x06005E8A RID: 24202 RVA: 0x001DC6EA File Offset: 0x001DA8EA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(GTHardCodedBones.EBone bone)
		{
			if (bone == GTHardCodedBones.EBone.None)
			{
				return 0L;
			}
			return 1L << bone - GTHardCodedBones.EBone.rig;
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x001DC6FB File Offset: 0x001DA8FB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static EHandedness GetHandednessFromBone(GTHardCodedBones.EBone bone)
		{
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1728432283058160L) != 0L)
			{
				return EHandedness.Left;
			}
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1769114204897280L) == 0L)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x001DC728 File Offset: 0x001DA928
		public static bool TryGetBoneXforms(VRRig vrRig, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
			{
				return false;
			}
			VRRigAnchorOverrides componentInChildren = vrRig.GetComponentInChildren<VRRigAnchorOverrides>(true);
			BodyDockPositions componentInChildren2 = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outBoneXforms[46] = componentInChildren2.leftBackTransform;
			outBoneXforms[47] = componentInChildren2.rightBackTransform;
			outBoneXforms[42] = componentInChildren2.chestTransform;
			outBoneXforms[43] = componentInChildren.CurrentBadgeTransform;
			outBoneXforms[44] = componentInChildren.nameTransform;
			outBoneXforms[52] = componentInChildren.huntComputer;
			outBoneXforms[50] = componentInChildren.friendshipBraceletLeftAnchor;
			outBoneXforms[51] = componentInChildren.friendshipBraceletRightAnchor;
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			return true;
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x001DC7F4 File Offset: 0x001DA9F4
		public static bool TryGetSlotAnchorXforms(VRRig vrRig, out Transform[] outSlotXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outSlotXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_slotXforms.TryGetValue(instanceID, out outSlotXforms))
			{
				return true;
			}
			Transform[] array;
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out array, out outErrorMsg))
			{
				return false;
			}
			outSlotXforms = new Transform[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				outSlotXforms[i] = array[i];
			}
			BodyDockPositions componentInChildren = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outSlotXforms[7] = componentInChildren.leftArmTransform;
			outSlotXforms[25] = componentInChildren.rightArmTransform;
			outSlotXforms[8] = componentInChildren.leftHandTransform;
			outSlotXforms[26] = componentInChildren.rightHandTransform;
			GTHardCodedBones._gInstIds_To_slotXforms[instanceID] = outSlotXforms;
			return true;
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x001DC8AC File Offset: 0x001DAAAC
		public static bool TryGetBoneXforms(SkinnedMeshRenderer skinnedMeshRenderer, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (skinnedMeshRenderer == null)
			{
				outErrorMsg = "The SkinnedMeshRenderer was null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = skinnedMeshRenderer.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			GTHardCodedBones._gMissingBonesReport.Clear();
			Transform[] bones = skinnedMeshRenderer.bones;
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i] == null)
				{
					Debug.LogError(string.Format("this should never happen -- skinned mesh bone index {0} is null in component: ", i) + "\"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else if (bones[i].parent == null)
				{
					Debug.LogError(string.Format("unexpected and unhandled scenario -- skinned mesh bone at index {0} has no parent in ", i) + "component: \"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else
				{
					bones[i] = (bones[i].name.EndsWith("_new") ? bones[i].parent : bones[i]);
				}
			}
			outBoneXforms = new Transform[GTHardCodedBones.kBoneNames.Length];
			for (int j = 1; j < GTHardCodedBones.kBoneNames.Length; j++)
			{
				string text = GTHardCodedBones.kBoneNames[j];
				if (!(text == "None") && !text.EndsWith("_end") && !text.Contains("Anchor") && j != 1)
				{
					bool flag = false;
					foreach (Transform transform in bones)
					{
						if (!(transform == null) && !(transform.name != text))
						{
							outBoneXforms[j] = transform;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						GTHardCodedBones._gMissingBonesReport.Add(j);
					}
				}
			}
			for (int l = 1; l < GTHardCodedBones.kBoneNames.Length; l++)
			{
				string text2 = GTHardCodedBones.kBoneNames[l];
				if (text2.EndsWith("_end"))
				{
					string text3 = text2;
					int boneIndex = GTHardCodedBones.GetBoneIndex(text3.Substring(0, text3.Length - 4));
					if (boneIndex < 0)
					{
						GTHardCodedBones._gMissingBonesReport.Add(l);
					}
					else
					{
						Transform transform2 = outBoneXforms[boneIndex];
						if (transform2 == null)
						{
							GTHardCodedBones._gMissingBonesReport.Add(l);
						}
						else
						{
							Transform transform3 = transform2.Find(text2);
							if (transform3 == null)
							{
								GTHardCodedBones._gMissingBonesReport.Add(l);
							}
							else
							{
								outBoneXforms[l] = transform3;
							}
						}
					}
				}
			}
			Transform transform4 = outBoneXforms[2];
			if (transform4 != null && transform4.parent != null)
			{
				outBoneXforms[1] = transform4.parent;
			}
			else
			{
				GTHardCodedBones._gMissingBonesReport.Add(1);
			}
			for (int m = 1; m < GTHardCodedBones.kBoneNames.Length; m++)
			{
				string text4 = GTHardCodedBones.kBoneNames[m];
				if (text4.Contains("Anchor"))
				{
					Transform transform5;
					if (transform4.TryFindByPath("/**/" + text4, out transform5, false))
					{
						outBoneXforms[m] = transform5;
					}
					else
					{
						GameObject gameObject = new GameObject(text4);
						gameObject.transform.SetParent(transform4, false);
						outBoneXforms[m] = gameObject.transform;
					}
				}
			}
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			if (GTHardCodedBones._gMissingBonesReport.Count == 0)
			{
				return true;
			}
			string text5 = "The SkinnedMeshRenderer on \"" + skinnedMeshRenderer.name + "\" did not have these expected bones: ";
			foreach (int num in GTHardCodedBones._gMissingBonesReport)
			{
				text5 = text5 + "\n- " + GTHardCodedBones.kBoneNames[num];
			}
			outErrorMsg = text5;
			return true;
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x001DCC4C File Offset: 0x001DAE4C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, string boneName, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(boneName)];
			return boneXform != null;
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x001DCC60 File Offset: 0x001DAE60
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(eBone)];
			return boneXform != null;
		}

		// Token: 0x06005E91 RID: 24209 RVA: 0x001DCC74 File Offset: 0x001DAE74
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetFirstBoneInParents(Transform transform, out GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			while (transform != null)
			{
				string name = transform.name;
				if (name == "DropZoneAnchor" && transform.parent != null)
				{
					string name2 = transform.parent.name;
					if (name2 == "Slingshot Chest Snap")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftArm")
					{
						eBone = GTHardCodedBones.EBone.forearm_L;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemRightShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
						boneXform = transform;
						return true;
					}
				}
				else
				{
					if (name == "TransferrableItemLeftHand")
					{
						eBone = GTHardCodedBones.EBone.hand_L;
						boneXform = transform;
						return true;
					}
					if (name == "TransferrableItemRightHand")
					{
						eBone = GTHardCodedBones.EBone.hand_R;
						boneXform = transform;
						return true;
					}
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(transform.name);
				if (bone != GTHardCodedBones.EBone.None)
				{
					eBone = bone;
					boneXform = transform;
					return true;
				}
				transform = transform.parent;
			}
			eBone = GTHardCodedBones.EBone.None;
			boneXform = null;
			return false;
		}

		// Token: 0x06005E92 RID: 24210 RVA: 0x001DCD64 File Offset: 0x001DAF64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBoneEnumOfCosmeticPosStateFlag(TransferrableObject.PositionState positionState)
		{
			if (positionState <= TransferrableObject.PositionState.OnChest)
			{
				switch (positionState)
				{
				case TransferrableObject.PositionState.None:
					break;
				case TransferrableObject.PositionState.OnLeftArm:
					return GTHardCodedBones.EBone.forearm_L;
				case TransferrableObject.PositionState.OnRightArm:
					return GTHardCodedBones.EBone.forearm_R;
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
					goto IL_5F;
				case TransferrableObject.PositionState.InLeftHand:
					return GTHardCodedBones.EBone.hand_L;
				case TransferrableObject.PositionState.InRightHand:
					return GTHardCodedBones.EBone.hand_R;
				default:
					if (positionState != TransferrableObject.PositionState.OnChest)
					{
						goto IL_5F;
					}
					return GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
				}
			}
			else
			{
				if (positionState == TransferrableObject.PositionState.OnLeftShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
				}
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
				}
				if (positionState != TransferrableObject.PositionState.Dropped)
				{
					goto IL_5F;
				}
			}
			return GTHardCodedBones.EBone.None;
			IL_5F:
			throw new ArgumentOutOfRangeException(positionState.ToString());
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x001DCDE4 File Offset: 0x001DAFE4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticBodyDockDropPosFlags(BodyDockPositions.DropPositions enumFlags)
		{
			BodyDockPositions.DropPositions[] values = EnumData<BodyDockPositions.DropPositions>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (BodyDockPositions.DropPositions dropPositions in values)
			{
				if (dropPositions != BodyDockPositions.DropPositions.All && dropPositions != BodyDockPositions.DropPositions.None && dropPositions != BodyDockPositions.DropPositions.MaxDropPostions && (enumFlags & dropPositions) != BodyDockPositions.DropPositions.None)
				{
					list.Add(GTHardCodedBones._k_bodyDockDropPosition_to_eBone[dropPositions]);
				}
			}
			return list;
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x001DCE3C File Offset: 0x001DB03C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticTransferrablePosStateFlags(TransferrableObject.PositionState enumFlags)
		{
			TransferrableObject.PositionState[] values = EnumData<TransferrableObject.PositionState>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (TransferrableObject.PositionState positionState in values)
			{
				if (positionState != TransferrableObject.PositionState.None && positionState != TransferrableObject.PositionState.Dropped && (enumFlags & positionState) != TransferrableObject.PositionState.None)
				{
					list.Add(GTHardCodedBones._k_transferrablePosState_to_eBone[positionState]);
				}
			}
			return list;
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x001DCE90 File Offset: 0x001DB090
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetTransferrablePosStateFromBoneEnum(GTHardCodedBones.EBone eBone, out TransferrableObject.PositionState outPosState)
		{
			return GTHardCodedBones._k_eBone_to_transferrablePosState.TryGetValue(eBone, out outPosState);
		}

		// Token: 0x06005E96 RID: 24214 RVA: 0x001DCEA0 File Offset: 0x001DB0A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Transform GetBoneXformOfCosmeticPosStateFlag(TransferrableObject.PositionState anchorPosState, Transform[] bones)
		{
			if (bones.Length != 53)
			{
				throw new Exception(string.Format("{0}: Supplied bones array length is {1} but requires ", "GTHardCodedBones", bones.Length) + string.Format("{0}.", 53));
			}
			int boneIndex = GTHardCodedBones.GetBoneIndex(GTHardCodedBones.GetBoneEnumOfCosmeticPosStateFlag(anchorPosState));
			if (boneIndex != -1)
			{
				return bones[boneIndex];
			}
			return null;
		}

		// Token: 0x0400688D RID: 26765
		public const int kBoneCount = 53;

		// Token: 0x0400688E RID: 26766
		public static readonly string[] kBoneNames = new string[]
		{
			"None",
			"rig",
			"body",
			"head",
			"head_end",
			"shoulder.L",
			"upper_arm.L",
			"forearm.L",
			"hand.L",
			"palm.01.L",
			"palm.02.L",
			"thumb.01.L",
			"thumb.02.L",
			"thumb.03.L",
			"thumb.03.L_end",
			"f_index.01.L",
			"f_index.02.L",
			"f_index.03.L",
			"f_index.03.L_end",
			"f_middle.01.L",
			"f_middle.02.L",
			"f_middle.03.L",
			"f_middle.03.L_end",
			"shoulder.R",
			"upper_arm.R",
			"forearm.R",
			"hand.R",
			"palm.01.R",
			"palm.02.R",
			"thumb.01.R",
			"thumb.02.R",
			"thumb.03.R",
			"thumb.03.R_end",
			"f_index.01.R",
			"f_index.02.R",
			"f_index.03.R",
			"f_index.03.R_end",
			"f_middle.01.R",
			"f_middle.02.R",
			"f_middle.03.R",
			"f_middle.03.R_end",
			"body_AnchorTop_Neck",
			"body_AnchorFront_StowSlot",
			"body_AnchorFrontLeft_Badge",
			"body_AnchorFrontRight_NameTag",
			"body_AnchorBack",
			"body_AnchorBackLeft_StowSlot",
			"body_AnchorBackRight_StowSlot",
			"body_AnchorBottom",
			"body_AnchorBackBottom_Tail",
			"hand_L_AnchorBack",
			"hand_R_AnchorBack",
			"hand_L_AnchorFront_GameModeItemSlot"
		};

		// Token: 0x0400688F RID: 26767
		private const long kLeftSideMask = 1728432283058160L;

		// Token: 0x04006890 RID: 26768
		private const long kRightSideMask = 1769114204897280L;

		// Token: 0x04006891 RID: 26769
		private static readonly Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> _k_bodyDockDropPosition_to_eBone = new Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone>
		{
			{
				BodyDockPositions.DropPositions.None,
				GTHardCodedBones.EBone.None
			},
			{
				BodyDockPositions.DropPositions.LeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				BodyDockPositions.DropPositions.RightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				BodyDockPositions.DropPositions.Chest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				BodyDockPositions.DropPositions.LeftBack,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				BodyDockPositions.DropPositions.RightBack,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			}
		};

		// Token: 0x04006892 RID: 26770
		private static readonly Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> _k_transferrablePosState_to_eBone = new Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone>
		{
			{
				TransferrableObject.PositionState.None,
				GTHardCodedBones.EBone.None
			},
			{
				TransferrableObject.PositionState.OnLeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				TransferrableObject.PositionState.OnRightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				TransferrableObject.PositionState.InLeftHand,
				GTHardCodedBones.EBone.hand_L
			},
			{
				TransferrableObject.PositionState.InRightHand,
				GTHardCodedBones.EBone.hand_R
			},
			{
				TransferrableObject.PositionState.OnChest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				TransferrableObject.PositionState.OnLeftShoulder,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				TransferrableObject.PositionState.OnRightShoulder,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			},
			{
				TransferrableObject.PositionState.Dropped,
				GTHardCodedBones.EBone.None
			}
		};

		// Token: 0x04006893 RID: 26771
		private static readonly Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> _k_eBone_to_transferrablePosState = new Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState>
		{
			{
				GTHardCodedBones.EBone.None,
				TransferrableObject.PositionState.None
			},
			{
				GTHardCodedBones.EBone.forearm_L,
				TransferrableObject.PositionState.OnLeftArm
			},
			{
				GTHardCodedBones.EBone.forearm_R,
				TransferrableObject.PositionState.OnRightArm
			},
			{
				GTHardCodedBones.EBone.hand_L,
				TransferrableObject.PositionState.InLeftHand
			},
			{
				GTHardCodedBones.EBone.hand_R,
				TransferrableObject.PositionState.InRightHand
			},
			{
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot,
				TransferrableObject.PositionState.OnChest
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot,
				TransferrableObject.PositionState.OnLeftShoulder
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot,
				TransferrableObject.PositionState.OnRightShoulder
			}
		};

		// Token: 0x04006894 RID: 26772
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly List<int> _gMissingBonesReport = new List<int>(53);

		// Token: 0x04006895 RID: 26773
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_boneXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x04006896 RID: 26774
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_slotXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x02000EDC RID: 3804
		public enum EBone
		{
			// Token: 0x04006898 RID: 26776
			None,
			// Token: 0x04006899 RID: 26777
			rig,
			// Token: 0x0400689A RID: 26778
			body,
			// Token: 0x0400689B RID: 26779
			head,
			// Token: 0x0400689C RID: 26780
			head_end,
			// Token: 0x0400689D RID: 26781
			shoulder_L,
			// Token: 0x0400689E RID: 26782
			upper_arm_L,
			// Token: 0x0400689F RID: 26783
			forearm_L,
			// Token: 0x040068A0 RID: 26784
			hand_L,
			// Token: 0x040068A1 RID: 26785
			palm_01_L,
			// Token: 0x040068A2 RID: 26786
			palm_02_L,
			// Token: 0x040068A3 RID: 26787
			thumb_01_L,
			// Token: 0x040068A4 RID: 26788
			thumb_02_L,
			// Token: 0x040068A5 RID: 26789
			thumb_03_L,
			// Token: 0x040068A6 RID: 26790
			thumb_03_L_end,
			// Token: 0x040068A7 RID: 26791
			f_index_01_L,
			// Token: 0x040068A8 RID: 26792
			f_index_02_L,
			// Token: 0x040068A9 RID: 26793
			f_index_03_L,
			// Token: 0x040068AA RID: 26794
			f_index_03_L_end,
			// Token: 0x040068AB RID: 26795
			f_middle_01_L,
			// Token: 0x040068AC RID: 26796
			f_middle_02_L,
			// Token: 0x040068AD RID: 26797
			f_middle_03_L,
			// Token: 0x040068AE RID: 26798
			f_middle_03_L_end,
			// Token: 0x040068AF RID: 26799
			shoulder_R,
			// Token: 0x040068B0 RID: 26800
			upper_arm_R,
			// Token: 0x040068B1 RID: 26801
			forearm_R,
			// Token: 0x040068B2 RID: 26802
			hand_R,
			// Token: 0x040068B3 RID: 26803
			palm_01_R,
			// Token: 0x040068B4 RID: 26804
			palm_02_R,
			// Token: 0x040068B5 RID: 26805
			thumb_01_R,
			// Token: 0x040068B6 RID: 26806
			thumb_02_R,
			// Token: 0x040068B7 RID: 26807
			thumb_03_R,
			// Token: 0x040068B8 RID: 26808
			thumb_03_R_end,
			// Token: 0x040068B9 RID: 26809
			f_index_01_R,
			// Token: 0x040068BA RID: 26810
			f_index_02_R,
			// Token: 0x040068BB RID: 26811
			f_index_03_R,
			// Token: 0x040068BC RID: 26812
			f_index_03_R_end,
			// Token: 0x040068BD RID: 26813
			f_middle_01_R,
			// Token: 0x040068BE RID: 26814
			f_middle_02_R,
			// Token: 0x040068BF RID: 26815
			f_middle_03_R,
			// Token: 0x040068C0 RID: 26816
			f_middle_03_R_end,
			// Token: 0x040068C1 RID: 26817
			body_AnchorTop_Neck,
			// Token: 0x040068C2 RID: 26818
			body_AnchorFront_StowSlot,
			// Token: 0x040068C3 RID: 26819
			body_AnchorFrontLeft_Badge,
			// Token: 0x040068C4 RID: 26820
			body_AnchorFrontRight_NameTag,
			// Token: 0x040068C5 RID: 26821
			body_AnchorBack,
			// Token: 0x040068C6 RID: 26822
			body_AnchorBackLeft_StowSlot,
			// Token: 0x040068C7 RID: 26823
			body_AnchorBackRight_StowSlot,
			// Token: 0x040068C8 RID: 26824
			body_AnchorBottom,
			// Token: 0x040068C9 RID: 26825
			body_AnchorBackBottom_Tail,
			// Token: 0x040068CA RID: 26826
			hand_L_AnchorBack,
			// Token: 0x040068CB RID: 26827
			hand_R_AnchorBack,
			// Token: 0x040068CC RID: 26828
			hand_L_AnchorFront_GameModeItemSlot
		}

		// Token: 0x02000EDD RID: 3805
		public enum EStowSlots
		{
			// Token: 0x040068CE RID: 26830
			None,
			// Token: 0x040068CF RID: 26831
			forearm_L = 7,
			// Token: 0x040068D0 RID: 26832
			forearm_R = 25,
			// Token: 0x040068D1 RID: 26833
			body_AnchorFront_Chest = 42,
			// Token: 0x040068D2 RID: 26834
			body_AnchorBackLeft = 46,
			// Token: 0x040068D3 RID: 26835
			body_AnchorBackRight
		}

		// Token: 0x02000EDE RID: 3806
		public enum EHandAndStowSlots
		{
			// Token: 0x040068D5 RID: 26837
			None,
			// Token: 0x040068D6 RID: 26838
			forearm_L = 7,
			// Token: 0x040068D7 RID: 26839
			hand_L,
			// Token: 0x040068D8 RID: 26840
			forearm_R = 25,
			// Token: 0x040068D9 RID: 26841
			hand_R,
			// Token: 0x040068DA RID: 26842
			body_AnchorFront_Chest = 42,
			// Token: 0x040068DB RID: 26843
			body_AnchorBackLeft = 46,
			// Token: 0x040068DC RID: 26844
			body_AnchorBackRight
		}

		// Token: 0x02000EDF RID: 3807
		public enum ECosmeticSlots
		{
			// Token: 0x040068DE RID: 26846
			Hat = 4,
			// Token: 0x040068DF RID: 26847
			Badge = 43,
			// Token: 0x040068E0 RID: 26848
			Face = 3,
			// Token: 0x040068E1 RID: 26849
			ArmLeft = 6,
			// Token: 0x040068E2 RID: 26850
			ArmRight = 24,
			// Token: 0x040068E3 RID: 26851
			BackLeft = 46,
			// Token: 0x040068E4 RID: 26852
			BackRight,
			// Token: 0x040068E5 RID: 26853
			HandLeft = 8,
			// Token: 0x040068E6 RID: 26854
			HandRight = 26,
			// Token: 0x040068E7 RID: 26855
			Chest = 42,
			// Token: 0x040068E8 RID: 26856
			Fur = 1,
			// Token: 0x040068E9 RID: 26857
			Shirt,
			// Token: 0x040068EA RID: 26858
			Pants = 48,
			// Token: 0x040068EB RID: 26859
			Back = 45,
			// Token: 0x040068EC RID: 26860
			Arms = 2,
			// Token: 0x040068ED RID: 26861
			TagEffect = 0
		}

		// Token: 0x02000EE0 RID: 3808
		[Serializable]
		public struct SturdyEBone : ISerializationCallbackReceiver
		{
			// Token: 0x17000930 RID: 2352
			// (get) Token: 0x06005E98 RID: 24216 RVA: 0x001DD1FC File Offset: 0x001DB3FC
			// (set) Token: 0x06005E99 RID: 24217 RVA: 0x001DD204 File Offset: 0x001DB404
			public GTHardCodedBones.EBone Bone
			{
				get
				{
					return this._bone;
				}
				set
				{
					this._bone = value;
					this._boneName = GTHardCodedBones.GetBoneName(this._bone);
				}
			}

			// Token: 0x06005E9A RID: 24218 RVA: 0x001DD21E File Offset: 0x001DB41E
			public SturdyEBone(GTHardCodedBones.EBone bone)
			{
				this._bone = bone;
				this._boneName = null;
			}

			// Token: 0x06005E9B RID: 24219 RVA: 0x001DD22E File Offset: 0x001DB42E
			public SturdyEBone(string boneName)
			{
				this._bone = GTHardCodedBones.GetBone(boneName);
				this._boneName = null;
			}

			// Token: 0x06005E9C RID: 24220 RVA: 0x001DD243 File Offset: 0x001DB443
			public static implicit operator GTHardCodedBones.EBone(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return sturdyBone.Bone;
			}

			// Token: 0x06005E9D RID: 24221 RVA: 0x001DD24C File Offset: 0x001DB44C
			public static implicit operator GTHardCodedBones.SturdyEBone(GTHardCodedBones.EBone bone)
			{
				return new GTHardCodedBones.SturdyEBone(bone);
			}

			// Token: 0x06005E9E RID: 24222 RVA: 0x001DD243 File Offset: 0x001DB443
			public static explicit operator int(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return (int)sturdyBone.Bone;
			}

			// Token: 0x06005E9F RID: 24223 RVA: 0x001DD254 File Offset: 0x001DB454
			public override string ToString()
			{
				return this._boneName;
			}

			// Token: 0x06005EA0 RID: 24224 RVA: 0x000023F5 File Offset: 0x000005F5
			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
			}

			// Token: 0x06005EA1 RID: 24225 RVA: 0x001DD25C File Offset: 0x001DB45C
			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				if (string.IsNullOrEmpty(this._boneName))
				{
					this._bone = GTHardCodedBones.EBone.None;
					this._boneName = "None";
					return;
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(this._boneName);
				if (bone != GTHardCodedBones.EBone.None)
				{
					this._bone = bone;
				}
			}

			// Token: 0x040068EE RID: 26862
			[SerializeField]
			private GTHardCodedBones.EBone _bone;

			// Token: 0x040068EF RID: 26863
			[SerializeField]
			private string _boneName;
		}
	}
}
