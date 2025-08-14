using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000B15 RID: 2837
public static class UnityYaml
{
	// Token: 0x04004E7D RID: 20093
	private static readonly Assembly EngineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));

	// Token: 0x04004E7E RID: 20094
	private static readonly Assembly TerrainAssembly = Assembly.GetAssembly(typeof(Tree));

	// Token: 0x04004E7F RID: 20095
	public static Dictionary<int, Type> ClassIDToType = new Dictionary<int, Type>();
}
