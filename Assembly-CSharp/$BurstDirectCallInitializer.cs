using System;
using UnityEngine;

// Token: 0x0200104E RID: 4174
internal static class $BurstDirectCallInitializer
{
	// Token: 0x06006722 RID: 26402 RVA: 0x0020A56C File Offset: 0x0020876C
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Initialize();
		BurstClassInfo.Index_00003B80$BurstDirectCall.Initialize();
		BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Initialize();
		BurstClassInfo.NameCall_00003B82$BurstDirectCall.Initialize();
	}
}
