using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public static class ShaderPlatformSetter
{
	// Token: 0x06000D47 RID: 3399 RVA: 0x0004EFF9 File Offset: 0x0004D1F9
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
