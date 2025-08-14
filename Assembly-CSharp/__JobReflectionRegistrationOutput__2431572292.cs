using System;
using BoingKit;
using GorillaLocomotion.Gameplay;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x0200104D RID: 4173
[DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__2431572292
{
	// Token: 0x06006720 RID: 26400 RVA: 0x0020A4EC File Offset: 0x002086EC
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<HandEffectsTriggerRegistry.HandEffectsJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMesh>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMeshStatic>();
			IJobParallelForExtensions.EarlyJobInit<GorillaIKMgr.IKJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<GorillaIKMgr.IKTransformJob>();
			IJobExtensions.EarlyJobInit<DayNightCycle.LerpBakedLightingJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<VRRigJobManager.VRRigTransformJob>();
			IJobParallelForExtensions.EarlyJobInit<BuilderFindPotentialSnaps>();
			IJobParallelForTransformExtensions.EarlyJobInit<FindNearbyPiecesJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderConveyorManager.EvaluateSplineJob>();
			IJobExtensions.EarlyJobInit<SolveRopeJob>();
			IJobExtensions.EarlyJobInit<VectorizedSolveRopeJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.BehaviorJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.ReactorJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	// Token: 0x06006721 RID: 26401 RVA: 0x0020A564 File Offset: 0x00208764
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		__JobReflectionRegistrationOutput__2431572292.CreateJobReflectionData();
	}
}
