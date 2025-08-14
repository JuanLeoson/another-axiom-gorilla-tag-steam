using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FEE RID: 4078
	public static class BoingWorkAsynchronous
	{
		// Token: 0x060065D1 RID: 26065 RVA: 0x00206994 File Offset: 0x00204B94
		internal static void PostUnregisterBehaviorCleanUp()
		{
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hBehaviorJob.Complete();
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x002069C1 File Offset: 0x00204BC1
		internal static void PostUnregisterEffectorReactorCleanUp()
		{
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hReactorJob.Complete();
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x002069F8 File Offset: 0x00204BF8
		internal static void ExecuteBehaviors(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aBehaviorParams = new NativeArray<BoingWork.Params>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aBehaviorOutput = new NativeArray<BoingWork.Output>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aBehaviorParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.BehaviorJob jobData = new BoingWorkAsynchronous.BehaviorJob
				{
					Params = BoingWorkAsynchronous.s_aBehaviorParams,
					Output = BoingWorkAsynchronous.s_aBehaviorOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				};
				int innerloopBatchCount = (int)Mathf.Ceil((float)num / (float)Environment.ProcessorCount);
				BoingWorkAsynchronous.s_hBehaviorJob = jobData.Schedule(num, innerloopBatchCount, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			BoingWorkAsynchronous.s_behaviorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hBehaviorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aBehaviorOutput[i].GatherOutput(behaviorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x060065D4 RID: 26068 RVA: 0x00206B58 File Offset: 0x00204D58
		internal static void ExecuteReactors(Dictionary<int, BoingEffector> effectorMap, Dictionary<int, BoingReactor> reactorMap, Dictionary<int, BoingReactorField> fieldMap, Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aEffectors = new NativeArray<BoingEffector.Params>(effectorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aReactorExecParams = new NativeArray<BoingWork.Params>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aReactorExecOutput = new NativeArray<BoingWork.Output>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aReactorExecParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				int num2 = 0;
				BoingEffector.Params value2 = default(BoingEffector.Params);
				foreach (KeyValuePair<int, BoingEffector> keyValuePair2 in effectorMap)
				{
					BoingEffector value3 = keyValuePair2.Value;
					value2.Fill(keyValuePair2.Value);
					BoingWorkAsynchronous.s_aEffectors[num2++] = value2;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.s_hReactorJob = new BoingWorkAsynchronous.ReactorJob
				{
					Effectors = BoingWorkAsynchronous.s_aEffectors,
					Params = BoingWorkAsynchronous.s_aReactorExecParams,
					Output = BoingWorkAsynchronous.s_aReactorExecOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				}.Schedule(num, 32, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair3 in fieldMap)
			{
				BoingReactorField value4 = keyValuePair3.Value;
				if (value4.HardwareMode == BoingReactorField.HardwareModeEnum.CPU)
				{
					value4.ExecuteCpu(BoingManager.DeltaTime);
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair4 in cpuSamplerMap)
			{
				BoingReactorFieldCPUSampler value5 = keyValuePair4.Value;
			}
			BoingWorkAsynchronous.s_reactorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hReactorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aReactorExecOutput[i].GatherOutput(reactorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x060065D5 RID: 26069 RVA: 0x00206DD8 File Offset: 0x00204FD8
		internal static void ExecuteBones(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			float deltaTime = BoingManager.DeltaTime;
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					if (aEffectorParams != null)
					{
						for (int i = 0; i < aEffectorParams.Length; i++)
						{
							value.AccumulateTarget(ref aEffectorParams[i], deltaTime);
						}
					}
					value.EndAccumulateTargets();
					BoingManager.UpdateMode updateMode2 = value.UpdateMode;
					if (updateMode2 != BoingManager.UpdateMode.FixedUpdate)
					{
						if (updateMode2 - BoingManager.UpdateMode.EarlyUpdate <= 1)
						{
							value.Params.Execute(value, BoingManager.DeltaTime);
						}
					}
					else
					{
						value.Params.Execute(value, BoingManager.FixedDeltaTime);
					}
				}
			}
		}

		// Token: 0x060065D6 RID: 26070 RVA: 0x00206EA4 File Offset: 0x002050A4
		internal static void PullBonesResults(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.Params.PullResults(value);
				}
			}
		}

		// Token: 0x040070EF RID: 28911
		private static bool s_behaviorJobNeedsGather;

		// Token: 0x040070F0 RID: 28912
		private static JobHandle s_hBehaviorJob;

		// Token: 0x040070F1 RID: 28913
		private static NativeArray<BoingWork.Params> s_aBehaviorParams;

		// Token: 0x040070F2 RID: 28914
		private static NativeArray<BoingWork.Output> s_aBehaviorOutput;

		// Token: 0x040070F3 RID: 28915
		private static bool s_reactorJobNeedsGather;

		// Token: 0x040070F4 RID: 28916
		private static JobHandle s_hReactorJob;

		// Token: 0x040070F5 RID: 28917
		private static NativeArray<BoingEffector.Params> s_aEffectors;

		// Token: 0x040070F6 RID: 28918
		private static NativeArray<BoingWork.Params> s_aReactorExecParams;

		// Token: 0x040070F7 RID: 28919
		private static NativeArray<BoingWork.Output> s_aReactorExecOutput;

		// Token: 0x02000FEF RID: 4079
		private struct BehaviorJob : IJobParallelFor
		{
			// Token: 0x060065D7 RID: 26071 RVA: 0x00206F08 File Offset: 0x00205108
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(this.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x040070F8 RID: 28920
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x040070F9 RID: 28921
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x040070FA RID: 28922
			public float DeltaTime;

			// Token: 0x040070FB RID: 28923
			public float FixedDeltaTime;
		}

		// Token: 0x02000FF0 RID: 4080
		private struct ReactorJob : IJobParallelFor
		{
			// Token: 0x060065D8 RID: 26072 RVA: 0x00206F8C File Offset: 0x0020518C
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				int i = 0;
				int length = this.Effectors.Length;
				while (i < length)
				{
					BoingEffector.Params params2 = this.Effectors[i];
					@params.AccumulateTarget(ref params2, this.DeltaTime);
					i++;
				}
				@params.EndAccumulateTargets();
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(BoingManager.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x040070FC RID: 28924
			[ReadOnly]
			public NativeArray<BoingEffector.Params> Effectors;

			// Token: 0x040070FD RID: 28925
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x040070FE RID: 28926
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x040070FF RID: 28927
			public float DeltaTime;

			// Token: 0x04007100 RID: 28928
			public float FixedDeltaTime;
		}
	}
}
