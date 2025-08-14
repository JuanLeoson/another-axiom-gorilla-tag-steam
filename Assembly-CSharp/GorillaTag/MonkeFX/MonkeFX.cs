using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02000EAD RID: 3757
	public class MonkeFX : ITickSystemPost
	{
		// Token: 0x06005DE3 RID: 24035 RVA: 0x001DA0AC File Offset: 0x001D82AC
		private static void InitBonesArray()
		{
			MonkeFX._rigs = VRRigCache.Instance.GetAllRigs();
			MonkeFX._bones = new Transform[MonkeFX._rigs.Length * MonkeFX._boneNames.Length];
			for (int i = 0; i < MonkeFX._rigs.Length; i++)
			{
				if (MonkeFX._rigs[i] == null)
				{
					MonkeFX._errorLog_nullVRRigFromVRRigCache.AddOccurrence(i.ToString());
				}
				else
				{
					int num = i * MonkeFX._boneNames.Length;
					if (MonkeFX._rigs[i].mainSkin == null)
					{
						MonkeFX._errorLog_nullMainSkin.AddOccurrence(MonkeFX._rigs[i].transform.GetPath());
						Debug.LogError("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene path: \n- \"" + MonkeFX._rigs[i].transform.GetPath() + "\"");
					}
					else
					{
						for (int j = 0; j < MonkeFX._rigs[i].mainSkin.bones.Length; j++)
						{
							Transform transform = MonkeFX._rigs[i].mainSkin.bones[j];
							if (transform == null)
							{
								MonkeFX._errorLog_nullBone.AddOccurrence(j.ToString());
							}
							else
							{
								for (int k = 0; k < MonkeFX._boneNames.Length; k++)
								{
									if (MonkeFX._boneNames[k] == transform.name)
									{
										MonkeFX._bones[num + k] = transform;
									}
								}
							}
						}
					}
				}
			}
			MonkeFX._errorLog_nullVRRigFromVRRigCache.LogOccurrences(VRRigCache.Instance, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 106);
			MonkeFX._errorLog_nullMainSkin.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 107);
			MonkeFX._errorLog_nullBone.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 108);
		}

		// Token: 0x06005DE4 RID: 24036 RVA: 0x000023F5 File Offset: 0x000005F5
		private static void UpdateBones()
		{
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x000023F5 File Offset: 0x000005F5
		private static void UpdateBone()
		{
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x001DA254 File Offset: 0x001D8454
		public static void Register(MonkeFXSettingsSO settingsSO)
		{
			MonkeFX.EnsureInstance();
			if (settingsSO == null || !MonkeFX.instance._settingsSOs.Add(settingsSO))
			{
				return;
			}
			int num = MonkeFX.instance._srcMeshId_to_sourceMesh.Count;
			for (int i = 0; i < settingsSO.sourceMeshes.Length; i++)
			{
				Mesh obj = settingsSO.sourceMeshes[i].obj;
				if (!(obj == null) && MonkeFX.instance._srcMeshInst_to_meshId.TryAdd(obj.GetInstanceID(), num))
				{
					MonkeFX.instance._srcMeshId_to_sourceMesh.Add(obj);
					num++;
				}
			}
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001DA2EC File Offset: 0x001D84EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetScaleToFitInBounds(Mesh mesh)
		{
			Bounds bounds = mesh.bounds;
			float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
			if (num <= 0f)
			{
				return 0f;
			}
			return 1f / num;
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x001DA344 File Offset: 0x001D8544
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Pack0To1Floats(float x, float y)
		{
			return Mathf.Clamp01(x) * 65536f + Mathf.Clamp01(y);
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06005DE9 RID: 24041 RVA: 0x001DA359 File Offset: 0x001D8559
		// (set) Token: 0x06005DEA RID: 24042 RVA: 0x001DA360 File Offset: 0x001D8560
		public static MonkeFX instance { get; private set; }

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06005DEB RID: 24043 RVA: 0x001DA368 File Offset: 0x001D8568
		// (set) Token: 0x06005DEC RID: 24044 RVA: 0x001DA36F File Offset: 0x001D856F
		public static bool hasInstance { get; private set; }

		// Token: 0x06005DED RID: 24045 RVA: 0x001DA377 File Offset: 0x001D8577
		private static void EnsureInstance()
		{
			if (MonkeFX.hasInstance)
			{
				return;
			}
			MonkeFX.instance = new MonkeFX();
			MonkeFX.hasInstance = true;
		}

		// Token: 0x06005DEE RID: 24046 RVA: 0x001DA391 File Offset: 0x001D8591
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnAfterFirstSceneLoaded()
		{
			MonkeFX.EnsureInstance();
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x06005DEF RID: 24047 RVA: 0x001DA3A2 File Offset: 0x001D85A2
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			MonkeFX.UpdateBones();
		}

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06005DF0 RID: 24048 RVA: 0x001DA3B1 File Offset: 0x001D85B1
		// (set) Token: 0x06005DF1 RID: 24049 RVA: 0x001DA3B9 File Offset: 0x001D85B9
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005DF2 RID: 24050 RVA: 0x001DA3C2 File Offset: 0x001D85C2
		private static void PauseTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.RemovePostTickCallback(MonkeFX.instance);
		}

		// Token: 0x06005DF3 RID: 24051 RVA: 0x001DA3DF File Offset: 0x001D85DF
		private static void ResumeTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x040067DD RID: 26589
		private static readonly string[] _boneNames = new string[]
		{
			"body",
			"hand.L",
			"hand.R"
		};

		// Token: 0x040067DE RID: 26590
		private static VRRig[] _rigs;

		// Token: 0x040067DF RID: 26591
		private static Transform[] _bones;

		// Token: 0x040067E0 RID: 26592
		private static int _rigsHash;

		// Token: 0x040067E1 RID: 26593
		private static readonly GTLogErrorLimiter _errorLog_nullVRRigFromVRRigCache = new GTLogErrorLimiter("(This should never happen) Skipping null `VRRig` obtained from `VRRigCache`!", 10, "\n- ");

		// Token: 0x040067E2 RID: 26594
		private static GTLogErrorLimiter _errorLog_nullMainSkin = new GTLogErrorLimiter("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene paths: \n", 10, "\n- ");

		// Token: 0x040067E3 RID: 26595
		private static readonly GTLogErrorLimiter _errorLog_nullBone = new GTLogErrorLimiter("(This should never happen) Skipping null bone obtained from `VRRig.mainSkin.bones`! Index(es): ", 10, "\n- ");

		// Token: 0x040067E4 RID: 26596
		private readonly HashSet<MonkeFXSettingsSO> _settingsSOs = new HashSet<MonkeFXSettingsSO>(8);

		// Token: 0x040067E5 RID: 26597
		private readonly Dictionary<int, int> _srcMeshInst_to_meshId = new Dictionary<int, int>(8);

		// Token: 0x040067E6 RID: 26598
		private readonly List<Mesh> _srcMeshId_to_sourceMesh = new List<Mesh>(8);

		// Token: 0x040067E7 RID: 26599
		private readonly List<MonkeFX.ElementsRange> _srcMeshId_to_elemRange = new List<MonkeFX.ElementsRange>(8);

		// Token: 0x040067E8 RID: 26600
		private readonly Dictionary<int, List<MonkeFXSettingsSO>> _meshId_to_settingsUsers = new Dictionary<int, List<MonkeFXSettingsSO>>();

		// Token: 0x040067E9 RID: 26601
		private const float _k16BitFactor = 65536f;

		// Token: 0x02000EAE RID: 3758
		private struct ElementsRange
		{
			// Token: 0x040067ED RID: 26605
			public int min;

			// Token: 0x040067EE RID: 26606
			public int max;
		}
	}
}
