using System;
using System.Collections.Generic;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02000EA5 RID: 3749
	public class GorillaMaterialReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x06005DCC RID: 24012 RVA: 0x001D943C File Offset: 0x001D763C
		public void PopulateRuntimeLookupArrays()
		{
			this._momentEnumCount = ((GorillaMaterialReaction.EMomentInState[])Enum.GetValues(typeof(GorillaMaterialReaction.EMomentInState))).Length;
			this._matCount = this._ownerVRRig.materialsToChangeTo.Length;
			this._mat_x_moment_x_activeBool_to_gObjs = new GameObject[this._momentEnumCount * this._matCount * 2][];
			for (int i = 0; i < this._matCount; i++)
			{
				for (int j = 0; j < this._momentEnumCount; j++)
				{
					GorillaMaterialReaction.EMomentInState emomentInState = (GorillaMaterialReaction.EMomentInState)j;
					List<GameObject> list = new List<GameObject>();
					List<GameObject> list2 = new List<GameObject>();
					foreach (GorillaMaterialReaction.ReactionEntry reactionEntry in this._statusEffectReactions)
					{
						int[] statusMaterialIndexes = reactionEntry.statusMaterialIndexes;
						for (int l = 0; l < statusMaterialIndexes.Length; l++)
						{
							if (statusMaterialIndexes[l] == i)
							{
								foreach (GorillaMaterialReaction.GameObjectStates gameObjectStates2 in reactionEntry.gameObjectStates)
								{
									switch (emomentInState)
									{
									case GorillaMaterialReaction.EMomentInState.OnEnter:
										if (gameObjectStates2.onEnter.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									case GorillaMaterialReaction.EMomentInState.OnStay:
										if (gameObjectStates2.onStay.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									case GorillaMaterialReaction.EMomentInState.OnExit:
										if (gameObjectStates2.onExit.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									default:
										Debug.LogError(string.Format("Unhandled enum value for {0}: {1}", "EMomentInState", emomentInState));
										break;
									}
								}
							}
						}
					}
					int num = i * this._momentEnumCount * 2 + j * 2;
					this._mat_x_moment_x_activeBool_to_gObjs[num] = list2.ToArray();
					this._mat_x_moment_x_activeBool_to_gObjs[num + 1] = list.ToArray();
				}
			}
		}

		// Token: 0x06005DCD RID: 24013 RVA: 0x001D966B File Offset: 0x001D786B
		protected void Awake()
		{
			this.RemoveAndReportNulls();
			this.PopulateRuntimeLookupArrays();
		}

		// Token: 0x06005DCE RID: 24014 RVA: 0x001D967C File Offset: 0x001D787C
		protected void OnEnable()
		{
			if (this._ownerVRRig == null)
			{
				this._ownerVRRig = base.GetComponentInParent<VRRig>(true);
			}
			if (this._ownerVRRig == null)
			{
				Debug.LogError("GorillaMaterialReaction: Disabling because could not find VRRig! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			this._reactionsRemaining = 0;
			for (int i = 0; i < this._statusEffectReactions.Length; i++)
			{
				this._reactionsRemaining += this._statusEffectReactions[i].gameObjectStates.Length;
			}
			this._currentMatIndexStartTime = 0.0;
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06005DCF RID: 24015 RVA: 0x0001D44F File Offset: 0x0001B64F
		protected void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06005DD0 RID: 24016 RVA: 0x001D9724 File Offset: 0x001D7924
		// (set) Token: 0x06005DD1 RID: 24017 RVA: 0x001D972C File Offset: 0x001D792C
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005DD2 RID: 24018 RVA: 0x001D9738 File Offset: 0x001D7938
		void ITickSystemPost.PostTick()
		{
			if (!GorillaComputer.hasInstance || this._ownerVRRig == null)
			{
				return;
			}
			GorillaComputer instance = GorillaComputer.instance;
			int num = (GorillaGameManager.instance == null) ? 0 : GorillaGameManager.instance.MyMatIndex(this._ownerVRRig.creator);
			if (this._previousMatIndex == num && this._reactionsRemaining <= 0)
			{
				return;
			}
			double num2 = (double)instance.startupMillis / 1000.0 + Time.realtimeSinceStartupAsDouble;
			bool flag = false;
			if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnExit && this._previousMatIndex != num)
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnEnter;
				flag = true;
				this._currentMatIndexStartTime = num2;
				this._currentMomentDuration = -1.0;
				GorillaGameManager instance2 = GorillaGameManager.instance;
				if (instance2 != null)
				{
					GorillaTagManager gorillaTagManager = instance2 as GorillaTagManager;
					if (gorillaTagManager != null)
					{
						this._currentMomentDuration = (double)gorillaTagManager.tagCoolDown;
					}
				}
			}
			else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnEnter && this._previousMatIndex == num && (this._currentMomentDuration < 0.0 || this._currentMomentDuration < num2 - this._currentMatIndexStartTime))
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnStay;
				flag = true;
				this._currentMomentDuration = -1.0;
			}
			else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnStay && this._previousMatIndex != num)
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnExit;
				flag = true;
				this._currentMomentDuration = -1.0;
			}
			this._previousMatIndex = num;
			if (!flag)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				GameObject[] array = this._mat_x_moment_x_activeBool_to_gObjs[(int)(num * this._momentEnumCount * 2 + this._currentMomentInState * GorillaMaterialReaction.EMomentInState.OnExit + i)];
				for (int j = 0; j < array.Length; j++)
				{
					array[j].SetActive(i == 1);
				}
			}
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x001D98F0 File Offset: 0x001D7AF0
		private void RemoveAndReportNulls()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (this._statusEffectReactions == null)
			{
				Debug.Log(string.Format("{0}: The array `{1}` is null. ", "GorillaMaterialReaction", this._statusEffectReactions) + "(this should never happen)");
				this._statusEffectReactions = Array.Empty<GorillaMaterialReaction.ReactionEntry>();
			}
			for (int i = 0; i < this._statusEffectReactions.Length; i++)
			{
				GorillaMaterialReaction.GameObjectStates[] gameObjectStates = this._statusEffectReactions[i].gameObjectStates;
				if (gameObjectStates == null)
				{
					this._statusEffectReactions[i].gameObjectStates = Array.Empty<GorillaMaterialReaction.GameObjectStates>();
				}
				else
				{
					int num = 0;
					int[] array = new int[gameObjectStates.Length];
					for (int j = 0; j < gameObjectStates.Length; j++)
					{
						if (gameObjectStates[j].gameObject == null)
						{
							array[num] = j;
							num++;
						}
						else
						{
							array[num] = -1;
						}
					}
					if (num == 0)
					{
						return;
					}
					stringBuilder.Clear();
					stringBuilder.Append("GorillaMaterialReaction");
					stringBuilder.Append(": Removed null references in array `");
					stringBuilder.Append("_statusEffectReactions");
					stringBuilder.Append("[").Append(i).Append("].").Append("gameObjectStates");
					stringBuilder.Append("' at indexes: ");
					stringBuilder.AppendJoin<int>(", ", array);
					stringBuilder.Append(".");
					Debug.LogError(stringBuilder.ToString(), this);
					GorillaMaterialReaction.GameObjectStates[] array2 = new GorillaMaterialReaction.GameObjectStates[gameObjectStates.Length - num];
					int num2 = 0;
					for (int k = 0; k < gameObjectStates.Length; k++)
					{
						if (!(gameObjectStates[k].gameObject == null))
						{
							array2[num2++] = gameObjectStates[k];
						}
					}
					this._statusEffectReactions[i].gameObjectStates = array2;
				}
			}
		}

		// Token: 0x040067A5 RID: 26533
		[SerializeField]
		private GorillaMaterialReaction.ReactionEntry[] _statusEffectReactions;

		// Token: 0x040067A6 RID: 26534
		private int _previousMatIndex;

		// Token: 0x040067A7 RID: 26535
		private GorillaMaterialReaction.EMomentInState _currentMomentInState;

		// Token: 0x040067A8 RID: 26536
		private double _currentMatIndexStartTime;

		// Token: 0x040067A9 RID: 26537
		private double _currentMomentDuration;

		// Token: 0x040067AA RID: 26538
		private int _reactionsRemaining;

		// Token: 0x040067AB RID: 26539
		private int _momentEnumCount;

		// Token: 0x040067AC RID: 26540
		private int _matCount;

		// Token: 0x040067AD RID: 26541
		private GameObject[][] _mat_x_moment_x_activeBool_to_gObjs;

		// Token: 0x040067AE RID: 26542
		private VRRig _ownerVRRig;

		// Token: 0x02000EA6 RID: 3750
		[Serializable]
		public struct ReactionEntry
		{
			// Token: 0x040067B0 RID: 26544
			[Tooltip("If any of these statuses are true then this reaction will be executed.")]
			public int[] statusMaterialIndexes;

			// Token: 0x040067B1 RID: 26545
			public GorillaMaterialReaction.GameObjectStates[] gameObjectStates;
		}

		// Token: 0x02000EA7 RID: 3751
		[Serializable]
		public struct GameObjectStates
		{
			// Token: 0x040067B2 RID: 26546
			public GameObject gameObject;

			// Token: 0x040067B3 RID: 26547
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onEnter;

			// Token: 0x040067B4 RID: 26548
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onStay;

			// Token: 0x040067B5 RID: 26549
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onExit;
		}

		// Token: 0x02000EA8 RID: 3752
		[Serializable]
		public struct MomentInStateActiveOption
		{
			// Token: 0x040067B6 RID: 26550
			public bool change;

			// Token: 0x040067B7 RID: 26551
			public bool activeState;
		}

		// Token: 0x02000EA9 RID: 3753
		public enum EMomentInState
		{
			// Token: 0x040067B9 RID: 26553
			OnEnter,
			// Token: 0x040067BA RID: 26554
			OnStay,
			// Token: 0x040067BB RID: 26555
			OnExit
		}

		// Token: 0x02000EAA RID: 3754
		public class MomentInStateAttribute : Attribute
		{
		}
	}
}
