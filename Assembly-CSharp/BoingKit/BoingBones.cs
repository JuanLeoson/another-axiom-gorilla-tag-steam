using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FC1 RID: 4033
	public class BoingBones : BoingReactor
	{
		// Token: 0x060064DD RID: 25821 RVA: 0x001FFD9F File Offset: 0x001FDF9F
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060064DE RID: 25822 RVA: 0x001FFDA7 File Offset: 0x001FDFA7
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060064DF RID: 25823 RVA: 0x001FFDAF File Offset: 0x001FDFAF
		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		// Token: 0x060064E0 RID: 25824 RVA: 0x001FFDCB File Offset: 0x001FDFCB
		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		// Token: 0x060064E1 RID: 25825 RVA: 0x001FFDD9 File Offset: 0x001FDFD9
		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		// Token: 0x060064E2 RID: 25826 RVA: 0x001FFDED File Offset: 0x001FDFED
		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		// Token: 0x060064E3 RID: 25827 RVA: 0x001FFDFC File Offset: 0x001FDFFC
		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = (chain != null) ? chain.Root : null;
				int num3 = (transform != null) ? Codec.HashTransformHierarchy(transform) : -1;
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = chain.Exclusion.ToArray<Transform>();
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!chain.Exclusion.Contains(rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!chain.Exclusion.Contains(child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float lengthFromRoot = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, lengthFromRoot));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num6 = 0;
								while (bone3.ChildIndices[num6] >= 0)
								{
									num6++;
								}
								if (num6 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num6] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num7 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num8 = 0; num8 < list.Count; num8++)
							{
								BoingBones.Bone bone4 = list[num8];
								float t = Mathf.Clamp01(bone4.LengthFromRoot * num7);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		// Token: 0x060064E4 RID: 25828 RVA: 0x00200218 File Offset: 0x001FE418
		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float t = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		// Token: 0x060064E5 RID: 25829 RVA: 0x002002A0 File Offset: 0x001FE4A0
		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x002002D0 File Offset: 0x001FE4D0
		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Transform.position);
				bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
				bone.CachedPositionWs = bone.Transform.position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Transform.rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.Transform.localScale;
			}
			this.CachedTransformValid = true;
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x060064E7 RID: 25831 RVA: 0x00200394 File Offset: 0x001FE594
		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		// Token: 0x060064E8 RID: 25832 RVA: 0x0020039C File Offset: 0x001FE59C
		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float d = (this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime;
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 b = chain.Gravity * d;
					float num = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Transform.position);
								bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num2 = Vector3.Distance(bone.Transform.position, bone2.Transform.position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num2;
							num = Mathf.Max(num, bone.LengthFromRoot);
						}
					}
					float num3 = MathUtil.InvSafe(num);
					foreach (BoingBones.Bone bone3 in array)
					{
						float t = bone3.LengthFromRoot * num3;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Transform.position, bone3.Transform.position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Transform.position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float t2 = bone4.LengthFromRoot * num3;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t2, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t2, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t2, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t2, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t2, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t2, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + b;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Transform.rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector = bone4.Transform.position;
						Quaternion rotation = bone4.Transform.rotation;
						Vector3 localScale = bone4.Transform.localScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Transform.position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 a = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion a2 = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Transform.position;
							Quaternion rotation2 = bone4.Transform.rotation;
							Vector3 b2 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion b3 = bone6.RotationInverseWs * rotation2;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 point = Vector3.Lerp(a, b2, poseStiffness);
							Quaternion rhs = Quaternion.Slerp(a2, b3, poseStiffness);
							vector = value + bone6.SpringRotationWs * point;
							rotation = bone6.SpringRotationWs * rhs;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector2 = vector - position;
								vector2 = VectorUtil.ClampBend(vector2, position3 - position, bone4.BendAngleCap);
								vector = position + vector2;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector, rotation, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector, rotation, localScale, true);
						}
					}
				}
			}
		}

		// Token: 0x060064E9 RID: 25833 RVA: 0x0020099C File Offset: 0x001FEB9C
		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		// Token: 0x060064EA RID: 25834 RVA: 0x00200A84 File Offset: 0x001FEC84
		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		// Token: 0x060064EB RID: 25835 RVA: 0x00200B08 File Offset: 0x001FED08
		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.localPosition = bone.CachedPositionLs;
							bone.Transform.localRotation = bone.CachedRotationLs;
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		// Token: 0x04006F96 RID: 28566
		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		// Token: 0x04006F97 RID: 28567
		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		// Token: 0x04006F98 RID: 28568
		public bool TwistPropagation = true;

		// Token: 0x04006F99 RID: 28569
		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		// Token: 0x04006F9A RID: 28570
		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		// Token: 0x04006F9B RID: 28571
		public Collider[] UnityColliders = new Collider[0];

		// Token: 0x04006F9C RID: 28572
		public bool DebugDrawRawBones;

		// Token: 0x04006F9D RID: 28573
		public bool DebugDrawTargetBones;

		// Token: 0x04006F9E RID: 28574
		public bool DebugDrawBoingBones;

		// Token: 0x04006F9F RID: 28575
		public bool DebugDrawFinalBones;

		// Token: 0x04006FA0 RID: 28576
		public bool DebugDrawColliders;

		// Token: 0x04006FA1 RID: 28577
		public bool DebugDrawChainBounds;

		// Token: 0x04006FA2 RID: 28578
		public bool DebugDrawBoneNames;

		// Token: 0x04006FA3 RID: 28579
		public bool DebugDrawLengthFromRoot;

		// Token: 0x04006FA4 RID: 28580
		private float m_minScale = 1f;

		// Token: 0x02000FC2 RID: 4034
		[Serializable]
		public class Bone
		{
			// Token: 0x060064ED RID: 25837 RVA: 0x00200BF4 File Offset: 0x001FEDF4
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			// Token: 0x060064EE RID: 25838 RVA: 0x00200C28 File Offset: 0x001FEE28
			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			// Token: 0x04006FA5 RID: 28581
			internal BoingWork.Params.InstanceData Instance;

			// Token: 0x04006FA6 RID: 28582
			internal Transform Transform;

			// Token: 0x04006FA7 RID: 28583
			internal Vector3 ScaleWs;

			// Token: 0x04006FA8 RID: 28584
			internal Vector3 CachedScaleLs;

			// Token: 0x04006FA9 RID: 28585
			internal Vector3 BlendedPositionWs;

			// Token: 0x04006FAA RID: 28586
			internal Vector3 BlendedScaleLs;

			// Token: 0x04006FAB RID: 28587
			internal Vector3 CachedPositionWs;

			// Token: 0x04006FAC RID: 28588
			internal Vector3 CachedPositionLs;

			// Token: 0x04006FAD RID: 28589
			internal Bounds Bounds;

			// Token: 0x04006FAE RID: 28590
			internal Quaternion RotationInverseWs;

			// Token: 0x04006FAF RID: 28591
			internal Quaternion SpringRotationWs;

			// Token: 0x04006FB0 RID: 28592
			internal Quaternion SpringRotationInverseWs;

			// Token: 0x04006FB1 RID: 28593
			internal Quaternion CachedRotationWs;

			// Token: 0x04006FB2 RID: 28594
			internal Quaternion CachedRotationLs;

			// Token: 0x04006FB3 RID: 28595
			internal Quaternion BlendedRotationWs;

			// Token: 0x04006FB4 RID: 28596
			internal Quaternion RotationBackPropDeltaPs;

			// Token: 0x04006FB5 RID: 28597
			internal int ParentIndex;

			// Token: 0x04006FB6 RID: 28598
			internal int[] ChildIndices;

			// Token: 0x04006FB7 RID: 28599
			internal float LengthFromRoot;

			// Token: 0x04006FB8 RID: 28600
			internal float AnimationBlend;

			// Token: 0x04006FB9 RID: 28601
			internal float LengthStiffness;

			// Token: 0x04006FBA RID: 28602
			internal float LengthStiffnessT;

			// Token: 0x04006FBB RID: 28603
			internal float FullyStiffToParentLength;

			// Token: 0x04006FBC RID: 28604
			internal float PoseStiffness;

			// Token: 0x04006FBD RID: 28605
			internal float BendAngleCap;

			// Token: 0x04006FBE RID: 28606
			internal float CollisionRadius;

			// Token: 0x04006FBF RID: 28607
			internal float SquashAndStretch;
		}

		// Token: 0x02000FC3 RID: 4035
		[Serializable]
		public class Chain
		{
			// Token: 0x060064EF RID: 25839 RVA: 0x00200CDC File Offset: 0x001FEEDC
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			// Token: 0x04006FC0 RID: 28608
			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			// Token: 0x04006FC1 RID: 28609
			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			// Token: 0x04006FC2 RID: 28610
			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			// Token: 0x04006FC3 RID: 28611
			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			// Token: 0x04006FC4 RID: 28612
			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			// Token: 0x04006FC5 RID: 28613
			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			// Token: 0x04006FC6 RID: 28614
			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04006FC7 RID: 28615
			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			// Token: 0x04006FC8 RID: 28616
			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04006FC9 RID: 28617
			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			// Token: 0x04006FCA RID: 28618
			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04006FCB RID: 28619
			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			// Token: 0x04006FCC RID: 28620
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			// Token: 0x04006FCD RID: 28621
			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04006FCE RID: 28622
			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			// Token: 0x04006FCF RID: 28623
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			// Token: 0x04006FD0 RID: 28624
			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04006FD1 RID: 28625
			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			// Token: 0x04006FD2 RID: 28626
			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			// Token: 0x04006FD3 RID: 28627
			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			// Token: 0x04006FD4 RID: 28628
			public Vector3 Gravity = Vector3.zero;

			// Token: 0x04006FD5 RID: 28629
			internal Bounds Bounds;

			// Token: 0x04006FD6 RID: 28630
			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			// Token: 0x04006FD7 RID: 28631
			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04006FD8 RID: 28632
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			// Token: 0x04006FD9 RID: 28633
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			// Token: 0x04006FDA RID: 28634
			internal Transform m_scannedRoot;

			// Token: 0x04006FDB RID: 28635
			internal Transform[] m_scannedExclusion;

			// Token: 0x04006FDC RID: 28636
			internal int m_hierarchyHash = -1;

			// Token: 0x04006FDD RID: 28637
			internal float MaxLengthFromRoot;

			// Token: 0x02000FC4 RID: 4036
			public enum CurveType
			{
				// Token: 0x04006FDF RID: 28639
				ConstantOne,
				// Token: 0x04006FE0 RID: 28640
				ConstantHalf,
				// Token: 0x04006FE1 RID: 28641
				ConstantZero,
				// Token: 0x04006FE2 RID: 28642
				RootOneTailHalf,
				// Token: 0x04006FE3 RID: 28643
				RootOneTailZero,
				// Token: 0x04006FE4 RID: 28644
				RootHalfTailOne,
				// Token: 0x04006FE5 RID: 28645
				RootZeroTailOne,
				// Token: 0x04006FE6 RID: 28646
				Custom
			}
		}

		// Token: 0x02000FC5 RID: 4037
		private class RescanEntry
		{
			// Token: 0x060064F1 RID: 25841 RVA: 0x00200E8C File Offset: 0x001FF08C
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			// Token: 0x04006FE7 RID: 28647
			internal Transform Transform;

			// Token: 0x04006FE8 RID: 28648
			internal int ParentIndex;

			// Token: 0x04006FE9 RID: 28649
			internal float LengthFromRoot;
		}
	}
}
