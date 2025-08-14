using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
[Serializable]
public struct NetworkSystemConfig
{
	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001146 RID: 4422 RVA: 0x000622D1 File Offset: 0x000604D1
	public static string AppVersion
	{
		get
		{
			return NetworkSystemConfig.prependCode + "." + NetworkSystemConfig.AppVersionStripped;
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001147 RID: 4423 RVA: 0x000622E8 File Offset: 0x000604E8
	public static string AppVersionStripped
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.gameVersionType,
				".",
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001148 RID: 4424 RVA: 0x00062348 File Offset: 0x00060548
	public static string BundleVersion
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001149 RID: 4425 RVA: 0x00062397 File Offset: 0x00060597
	public static string GameVersionType
	{
		get
		{
			return NetworkSystemConfig.gameVersionType;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x0600114A RID: 4426 RVA: 0x0006239E File Offset: 0x0006059E
	public static int GameMajorVersion
	{
		get
		{
			return NetworkSystemConfig.majorVersion;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x0600114B RID: 4427 RVA: 0x000623A5 File Offset: 0x000605A5
	public static int GameMinorVersion
	{
		get
		{
			return NetworkSystemConfig.minorVersion;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x0600114C RID: 4428 RVA: 0x000623AC File Offset: 0x000605AC
	public static int GameMinorVersion2
	{
		get
		{
			return NetworkSystemConfig.minorVersion2;
		}
	}

	// Token: 0x04001968 RID: 6504
	[HideInInspector]
	public int MaxPlayerCount;

	// Token: 0x04001969 RID: 6505
	private static string gameVersionType = "live1";

	// Token: 0x0400196A RID: 6506
	public static string prependCode = "PrependSpooky4562Catacombs";

	// Token: 0x0400196B RID: 6507
	public static int majorVersion = 1;

	// Token: 0x0400196C RID: 6508
	public static int minorVersion = 1;

	// Token: 0x0400196D RID: 6509
	public static int minorVersion2 = 117;
}
