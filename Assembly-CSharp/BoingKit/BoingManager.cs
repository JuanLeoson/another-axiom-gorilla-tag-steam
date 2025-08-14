using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FCA RID: 4042
	public static class BoingManager
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x0600650A RID: 25866 RVA: 0x002013CC File Offset: 0x001FF5CC
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x0600650B RID: 25867 RVA: 0x002013D8 File Offset: 0x001FF5D8
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x0600650C RID: 25868 RVA: 0x002013E4 File Offset: 0x001FF5E4
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x0600650D RID: 25869 RVA: 0x002013F0 File Offset: 0x001FF5F0
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x0600650E RID: 25870 RVA: 0x002013FC File Offset: 0x001FF5FC
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x0600650F RID: 25871 RVA: 0x00201408 File Offset: 0x001FF608
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x06006510 RID: 25872 RVA: 0x00201414 File Offset: 0x001FF614
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06006511 RID: 25873 RVA: 0x0020141B File Offset: 0x001FF61B
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06006512 RID: 25874 RVA: 0x00201422 File Offset: 0x001FF622
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06006513 RID: 25875 RVA: 0x0020142E File Offset: 0x001FF62E
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06006514 RID: 25876 RVA: 0x0020143A File Offset: 0x001FF63A
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06006515 RID: 25877 RVA: 0x00201446 File Offset: 0x001FF646
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06006516 RID: 25878 RVA: 0x00201452 File Offset: 0x001FF652
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06006517 RID: 25879 RVA: 0x0020145E File Offset: 0x001FF65E
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x06006518 RID: 25880 RVA: 0x0020146C File Offset: 0x001FF66C
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06006519 RID: 25881 RVA: 0x002014C6 File Offset: 0x001FF6C6
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x0600651A RID: 25882 RVA: 0x002014E1 File Offset: 0x001FF6E1
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x0600651B RID: 25883 RVA: 0x0020150B File Offset: 0x001FF70B
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x0600651C RID: 25884 RVA: 0x00201535 File Offset: 0x001FF735
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x0020155F File Offset: 0x001FF75F
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x00201589 File Offset: 0x001FF789
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x002015B3 File Offset: 0x001FF7B3
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06006520 RID: 25888 RVA: 0x002015DD File Offset: 0x001FF7DD
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x06006521 RID: 25889 RVA: 0x00201607 File Offset: 0x001FF807
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06006522 RID: 25890 RVA: 0x00201631 File Offset: 0x001FF831
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x06006523 RID: 25891 RVA: 0x0020165B File Offset: 0x001FF85B
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06006524 RID: 25892 RVA: 0x00201685 File Offset: 0x001FF885
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x06006525 RID: 25893 RVA: 0x002016AF File Offset: 0x001FF8AF
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06006526 RID: 25894 RVA: 0x002016D9 File Offset: 0x001FF8D9
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x06006527 RID: 25895 RVA: 0x00201703 File Offset: 0x001FF903
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x06006528 RID: 25896 RVA: 0x0020172D File Offset: 0x001FF92D
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06006529 RID: 25897 RVA: 0x00201734 File Offset: 0x001FF934
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x0600652A RID: 25898 RVA: 0x0020174C File Offset: 0x001FF94C
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x002017DC File Offset: 0x001FF9DC
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x0020172D File Offset: 0x001FF92D
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x000023F5 File Offset: 0x000005F5
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x0600652E RID: 25902 RVA: 0x00201846 File Offset: 0x001FFA46
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x00201870 File Offset: 0x001FFA70
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x00201904 File Offset: 0x001FFB04
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x06006531 RID: 25905 RVA: 0x0020196C File Offset: 0x001FFB6C
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x06006532 RID: 25906 RVA: 0x002019C4 File Offset: 0x001FFBC4
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x00201A98 File Offset: 0x001FFC98
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x00201B70 File Offset: 0x001FFD70
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x00201C2C File Offset: 0x001FFE2C
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x06006536 RID: 25910 RVA: 0x00201CCC File Offset: 0x001FFECC
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x06006537 RID: 25911 RVA: 0x00201D58 File Offset: 0x001FFF58
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x00201DF8 File Offset: 0x001FFFF8
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06006539 RID: 25913 RVA: 0x00201E30 File Offset: 0x00200030
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x0400700C RID: 28684
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x0400700D RID: 28685
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x0400700E RID: 28686
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x0400700F RID: 28687
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x04007010 RID: 28688
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x04007011 RID: 28689
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x04007012 RID: 28690
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x04007013 RID: 28691
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x04007014 RID: 28692
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x04007015 RID: 28693
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x04007016 RID: 28694
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x04007017 RID: 28695
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x04007018 RID: 28696
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x04007019 RID: 28697
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x0400701A RID: 28698
		private static float s_deltaTime = 0f;

		// Token: 0x0400701B RID: 28699
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x0400701C RID: 28700
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x0400701D RID: 28701
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x0400701E RID: 28702
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x0400701F RID: 28703
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x04007020 RID: 28704
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x04007021 RID: 28705
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x04007022 RID: 28706
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x04007023 RID: 28707
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x04007024 RID: 28708
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x04007025 RID: 28709
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x04007026 RID: 28710
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x04007027 RID: 28711
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x04007028 RID: 28712
		internal static GameObject s_managerGo;

		// Token: 0x02000FCB RID: 4043
		public enum UpdateMode
		{
			// Token: 0x0400702A RID: 28714
			FixedUpdate,
			// Token: 0x0400702B RID: 28715
			EarlyUpdate,
			// Token: 0x0400702C RID: 28716
			LateUpdate
		}

		// Token: 0x02000FCC RID: 4044
		public enum TranslationLockSpace
		{
			// Token: 0x0400702E RID: 28718
			Global,
			// Token: 0x0400702F RID: 28719
			Local
		}

		// Token: 0x02000FCD RID: 4045
		// (Invoke) Token: 0x0600653C RID: 25916
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x02000FCE RID: 4046
		// (Invoke) Token: 0x06006540 RID: 25920
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x02000FCF RID: 4047
		// (Invoke) Token: 0x06006544 RID: 25924
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x02000FD0 RID: 4048
		// (Invoke) Token: 0x06006548 RID: 25928
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02000FD1 RID: 4049
		// (Invoke) Token: 0x0600654C RID: 25932
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02000FD2 RID: 4050
		// (Invoke) Token: 0x06006550 RID: 25936
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02000FD3 RID: 4051
		// (Invoke) Token: 0x06006554 RID: 25940
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02000FD4 RID: 4052
		// (Invoke) Token: 0x06006558 RID: 25944
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x02000FD5 RID: 4053
		// (Invoke) Token: 0x0600655C RID: 25948
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02000FD6 RID: 4054
		// (Invoke) Token: 0x06006560 RID: 25952
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02000FD7 RID: 4055
		// (Invoke) Token: 0x06006564 RID: 25956
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02000FD8 RID: 4056
		// (Invoke) Token: 0x06006568 RID: 25960
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02000FD9 RID: 4057
		// (Invoke) Token: 0x0600656C RID: 25964
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x02000FDA RID: 4058
		// (Invoke) Token: 0x06006570 RID: 25968
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}
