using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200085C RID: 2140
public static class AnimationCurves
{
	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x060035C9 RID: 13769 RVA: 0x0011A22C File Offset: 0x0011842C
	public static AnimationCurve EaseInQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 2.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x060035CA RID: 13770 RVA: 0x0011A298 File Offset: 0x00118498
	public static AnimationCurve EaseOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 2.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x060035CB RID: 13771 RVA: 0x0011A304 File Offset: 0x00118504
	public static AnimationCurve EaseInOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 1.999994f, 1.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x060035CC RID: 13772 RVA: 0x0011A39C File Offset: 0x0011859C
	public static AnimationCurve EaseInCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 3.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x060035CD RID: 13773 RVA: 0x0011A408 File Offset: 0x00118608
	public static AnimationCurve EaseOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x060035CE RID: 13774 RVA: 0x0011A474 File Offset: 0x00118674
	public static AnimationCurve EaseInOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 2.999994f, 2.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x060035CF RID: 13775 RVA: 0x0011A50C File Offset: 0x0011870C
	public static AnimationCurve EaseInQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0139424f, 0f, 0.434789f),
				new Keyframe(1f, 1f, 3.985819f, 0f, 0.269099f, 0f)
			});
		}
	}

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x060035D0 RID: 13776 RVA: 0x0011A578 File Offset: 0x00118778
	public static AnimationCurve EaseOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.985823f, 0f, 0.269099f),
				new Keyframe(1f, 1f, 0.01394233f, 0f, 0.434789f, 0f)
			});
		}
	}

	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x060035D1 RID: 13777 RVA: 0x0011A5E4 File Offset: 0x001187E4
	public static AnimationCurve EaseInOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01394243f, 0f, 0.434788f),
				new Keyframe(0.5f, 0.5f, 3.985842f, 3.985834f, 0.269098f, 0.269098f),
				new Keyframe(1f, 1f, 0.0139425f, 0f, 0.434788f, 0f)
			});
		}
	}

	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x060035D2 RID: 13778 RVA: 0x0011A67C File Offset: 0x0011887C
	public static AnimationCurve EaseInQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02411811f, 0f, 0.519568f),
				new Keyframe(1f, 1f, 4.951815f, 0f, 0.225963f, 0f)
			});
		}
	}

	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x060035D3 RID: 13779 RVA: 0x0011A6E8 File Offset: 0x001188E8
	public static AnimationCurve EaseOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.953289f, 0f, 0.225963f),
				new Keyframe(1f, 1f, 0.02414908f, 0f, 0.518901f, 0f)
			});
		}
	}

	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x060035D4 RID: 13780 RVA: 0x0011A754 File Offset: 0x00118954
	public static AnimationCurve EaseInOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02412004f, 0f, 0.519568f),
				new Keyframe(0.5f, 0.5f, 4.951789f, 4.953269f, 0.225964f, 0.225964f),
				new Keyframe(1f, 1f, 0.02415099f, 0f, 0.5189019f, 0f)
			});
		}
	}

	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x060035D5 RID: 13781 RVA: 0x0011A7EC File Offset: 0x001189EC
	public static AnimationCurve EaseInSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001208493f, 0f, 0.36078f),
				new Keyframe(1f, 1f, 1.572508f, 0f, 0.326514f, 0f)
			});
		}
	}

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x060035D6 RID: 13782 RVA: 0x0011A858 File Offset: 0x00118A58
	public static AnimationCurve EaseOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1.573552f, 0f, 0.330931f),
				new Keyframe(1f, 1f, -0.0009282457f, 0f, 0.358689f, 0f)
			});
		}
	}

	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x060035D7 RID: 13783 RVA: 0x0011A8C4 File Offset: 0x00118AC4
	public static AnimationCurve EaseInOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001202949f, 0f, 0.36078f),
				new Keyframe(0.5f, 0.5f, 1.572508f, 1.573372f, 0.326514f, 0.33093f),
				new Keyframe(1f, 1f, -0.0009312395f, 0f, 0.358688f, 0f)
			});
		}
	}

	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x060035D8 RID: 13784 RVA: 0x0011A95C File Offset: 0x00118B5C
	public static AnimationCurve EaseInExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124388f, 0f, 0.636963f),
				new Keyframe(1f, 1f, 6.815432f, 0f, 0.155667f, 0f)
			});
		}
	}

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x060035D9 RID: 13785 RVA: 0x0011A9C8 File Offset: 0x00118BC8
	public static AnimationCurve EaseOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 6.815433f, 0f, 0.155667f),
				new Keyframe(1f, 1f, 0.03124354f, 0f, 0.636963f, 0f)
			});
		}
	}

	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x060035DA RID: 13786 RVA: 0x0011AA34 File Offset: 0x00118C34
	public static AnimationCurve EaseInOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124509f, 0f, 0.636964f),
				new Keyframe(0.5f, 0.5f, 6.815477f, 6.815476f, 0.155666f, 0.155666f),
				new Keyframe(1f, 1f, 0.03124377f, 0f, 0.636964f, 0f)
			});
		}
	}

	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x060035DB RID: 13787 RVA: 0x0011AACC File Offset: 0x00118CCC
	public static AnimationCurve EaseInCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162338f, 0f, 0.55403f),
				new Keyframe(1f, 1f, 459.267f, 0f, 0.001197994f, 0f)
			});
		}
	}

	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x060035DC RID: 13788 RVA: 0x0011AB38 File Offset: 0x00118D38
	public static AnimationCurve EaseOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 461.7679f, 0f, 0.001198f),
				new Keyframe(1f, 1f, 0.00216235f, 0f, 0.554024f, 0f)
			});
		}
	}

	// Token: 0x1700051B RID: 1307
	// (get) Token: 0x060035DD RID: 13789 RVA: 0x0011ABA4 File Offset: 0x00118DA4
	public static AnimationCurve EaseInOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162353f, 0f, 0.554026f),
				new Keyframe(0.5f, 0.5f, 461.7703f, 461.7474f, 0.001197994f, 0.001198053f),
				new Keyframe(1f, 1f, 0.00216245f, 0f, 0.554026f, 0f)
			});
		}
	}

	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x060035DE RID: 13790 RVA: 0x0011AC3C File Offset: 0x00118E3C
	public static AnimationCurve EaseInBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6874897f, 0f, 0.3333663f),
				new Keyframe(0.0909f, 0f, -0.687694f, 1.374792f, 0.3332673f, 0.3334159f),
				new Keyframe(0.2727f, 0f, -1.375608f, 2.749388f, 0.3332179f, 0.3333489f),
				new Keyframe(0.6364f, 0f, -2.749183f, 5.501642f, 0.3333737f, 0.3332673f),
				new Keyframe(1f, 1f, 0f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x1700051D RID: 1309
	// (get) Token: 0x060035DF RID: 13791 RVA: 0x0011AD28 File Offset: 0x00118F28
	public static AnimationCurve EaseOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.3333663f),
				new Keyframe(0.3636f, 1f, 5.501643f, -2.749183f, 0.3332673f, 0.3333737f),
				new Keyframe(0.7273f, 1f, 2.749366f, -1.375609f, 0.3333516f, 0.3332178f),
				new Keyframe(0.9091f, 1f, 1.374792f, -0.6877043f, 0.3334158f, 0.3332673f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x1700051E RID: 1310
	// (get) Token: 0x060035E0 RID: 13792 RVA: 0x0011AE14 File Offset: 0x00119014
	public static AnimationCurve EaseInOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6875001f, 0f, 0.333011f),
				new Keyframe(0.0455f, 0f, -0.6854643f, 1.377057f, 0.334f, 0.3328713f),
				new Keyframe(0.1364f, 0f, -1.373381f, 2.751643f, 0.3337624f, 0.3331683f),
				new Keyframe(0.3182f, 0f, -2.749192f, 5.501634f, 0.3334654f, 0.3332673f),
				new Keyframe(0.5f, 0.5f, 0f, 0f, 0.3333663f, 0.3333663f),
				new Keyframe(0.6818f, 1f, 5.501634f, -2.749191f, 0.3332673f, 0.3334653f),
				new Keyframe(0.8636f, 1f, 2.751642f, -1.37338f, 0.3331683f, 0.3319367f),
				new Keyframe(0.955f, 1f, 1.354673f, -0.7087823f, 0.3365205f, 0.3266002f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3367105f, 0f)
			});
		}
	}

	// Token: 0x1700051F RID: 1311
	// (get) Token: 0x060035E1 RID: 13793 RVA: 0x0011AFA8 File Offset: 0x001191A8
	public static AnimationCurve EaseInBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 4.701583f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000520 RID: 1312
	// (get) Token: 0x060035E2 RID: 13794 RVA: 0x0011B014 File Offset: 0x00119214
	public static AnimationCurve EaseOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.701584f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000521 RID: 1313
	// (get) Token: 0x060035E3 RID: 13795 RVA: 0x0011B080 File Offset: 0x00119280
	public static AnimationCurve EaseInOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 5.594898f, 5.594899f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x17000522 RID: 1314
	// (get) Token: 0x060035E4 RID: 13796 RVA: 0x0011B118 File Offset: 0x00119318
	public static AnimationCurve EaseInElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0143284f, 0f, 1f),
				new Keyframe(0.175f, 0f, 0f, -0.06879552f, 0.008331452f, 0.8916667f),
				new Keyframe(0.475f, 0f, -0.4081632f, -0.5503653f, 0.4083333f, 0.8666668f),
				new Keyframe(0.775f, 0f, -3.26241f, -4.402922f, 0.3916665f, 0.5916666f),
				new Keyframe(1f, 1f, 12.51956f, 0f, 0.5916666f, 0f)
			});
		}
	}

	// Token: 0x17000523 RID: 1315
	// (get) Token: 0x060035E5 RID: 13797 RVA: 0x0011B204 File Offset: 0x00119404
	public static AnimationCurve EaseOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 12.51956f, 0f, 0.5916667f),
				new Keyframe(0.225f, 1f, -4.402922f, -3.262408f, 0.5916666f, 0.3916667f),
				new Keyframe(0.525f, 1f, -0.5503654f, -0.4081634f, 0.8666667f, 0.4083333f),
				new Keyframe(0.825f, 1f, -0.06879558f, 0f, 0.8916666f, 0.008331367f),
				new Keyframe(1f, 1f, 0.01432861f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x060035E6 RID: 13798 RVA: 0x0011B2F0 File Offset: 0x001194F0
	public static AnimationCurve EaseInOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01433143f, 0f, 1f),
				new Keyframe(0.0875f, 0f, 0f, -0.06879253f, 0.008331452f, 0.8916667f),
				new Keyframe(0.2375f, 0f, -0.4081632f, -0.5503692f, 0.4083333f, 0.8666668f),
				new Keyframe(0.3875f, 0f, -3.262419f, -4.402895f, 0.3916665f, 0.5916712f),
				new Keyframe(0.5f, 0.5f, 12.51967f, 12.51958f, 0.5916621f, 0.5916664f),
				new Keyframe(0.6125f, 1f, -4.402927f, -3.262402f, 0.5916669f, 0.3916666f),
				new Keyframe(0.7625f, 1f, -0.5503691f, -0.4081627f, 0.8666668f, 0.4083335f),
				new Keyframe(0.9125f, 1f, -0.06879289f, 0f, 0.8916666f, 0.008331029f),
				new Keyframe(1f, 1f, 0.01432828f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x060035E7 RID: 13799 RVA: 0x0011B484 File Offset: 0x00119684
	public static AnimationCurve Spring
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.582263f, 0f, 0.2385296f),
				new Keyframe(0.336583f, 0.828268f, 1.767519f, 1.767491f, 0.4374225f, 0.2215123f),
				new Keyframe(0.550666f, 1.079651f, 0.3095257f, 0.3095275f, 0.4695607f, 0.4154884f),
				new Keyframe(0.779498f, 0.974607f, -0.2321364f, -0.2321428f, 0.3585643f, 0.3623514f),
				new Keyframe(0.897999f, 1.003668f, 0.2797853f, 0.2797431f, 0.3331026f, 0.3306926f),
				new Keyframe(1f, 1f, -0.2023914f, 0f, 0.3296829f, 0f)
			});
		}
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x060035E8 RID: 13800 RVA: 0x0011B598 File Offset: 0x00119798
	public static AnimationCurve Linear
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1f, 0f, 0f),
				new Keyframe(1f, 1f, 1f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x060035E9 RID: 13801 RVA: 0x0011B604 File Offset: 0x00119804
	public static AnimationCurve Step
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 1f, 0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x0011B6C4 File Offset: 0x001198C4
	static AnimationCurves()
	{
		Dictionary<AnimationCurves.EaseType, AnimationCurve> dictionary = new Dictionary<AnimationCurves.EaseType, AnimationCurve>();
		dictionary[AnimationCurves.EaseType.EaseInQuad] = AnimationCurves.EaseInQuad;
		dictionary[AnimationCurves.EaseType.EaseOutQuad] = AnimationCurves.EaseOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInOutQuad] = AnimationCurves.EaseInOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInCubic] = AnimationCurves.EaseInCubic;
		dictionary[AnimationCurves.EaseType.EaseOutCubic] = AnimationCurves.EaseOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInOutCubic] = AnimationCurves.EaseInOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInQuart] = AnimationCurves.EaseInQuart;
		dictionary[AnimationCurves.EaseType.EaseOutQuart] = AnimationCurves.EaseOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInOutQuart] = AnimationCurves.EaseInOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInQuint] = AnimationCurves.EaseInQuint;
		dictionary[AnimationCurves.EaseType.EaseOutQuint] = AnimationCurves.EaseOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInOutQuint] = AnimationCurves.EaseInOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInSine] = AnimationCurves.EaseInSine;
		dictionary[AnimationCurves.EaseType.EaseOutSine] = AnimationCurves.EaseOutSine;
		dictionary[AnimationCurves.EaseType.EaseInOutSine] = AnimationCurves.EaseInOutSine;
		dictionary[AnimationCurves.EaseType.EaseInExpo] = AnimationCurves.EaseInExpo;
		dictionary[AnimationCurves.EaseType.EaseOutExpo] = AnimationCurves.EaseOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInOutExpo] = AnimationCurves.EaseInOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInCirc] = AnimationCurves.EaseInCirc;
		dictionary[AnimationCurves.EaseType.EaseOutCirc] = AnimationCurves.EaseOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInOutCirc] = AnimationCurves.EaseInOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInBounce] = AnimationCurves.EaseInBounce;
		dictionary[AnimationCurves.EaseType.EaseOutBounce] = AnimationCurves.EaseOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInOutBounce] = AnimationCurves.EaseInOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInBack] = AnimationCurves.EaseInBack;
		dictionary[AnimationCurves.EaseType.EaseOutBack] = AnimationCurves.EaseOutBack;
		dictionary[AnimationCurves.EaseType.EaseInOutBack] = AnimationCurves.EaseInOutBack;
		dictionary[AnimationCurves.EaseType.EaseInElastic] = AnimationCurves.EaseInElastic;
		dictionary[AnimationCurves.EaseType.EaseOutElastic] = AnimationCurves.EaseOutElastic;
		dictionary[AnimationCurves.EaseType.EaseInOutElastic] = AnimationCurves.EaseInOutElastic;
		dictionary[AnimationCurves.EaseType.Spring] = AnimationCurves.Spring;
		dictionary[AnimationCurves.EaseType.Linear] = AnimationCurves.Linear;
		dictionary[AnimationCurves.EaseType.Step] = AnimationCurves.Step;
		AnimationCurves.gEaseTypeToCurve = dictionary;
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x0011B880 File Offset: 0x00119A80
	public static AnimationCurve GetCurveForEase(AnimationCurves.EaseType ease)
	{
		return AnimationCurves.gEaseTypeToCurve[ease];
	}

	// Token: 0x040042B8 RID: 17080
	private static Dictionary<AnimationCurves.EaseType, AnimationCurve> gEaseTypeToCurve;

	// Token: 0x0200085D RID: 2141
	public enum EaseType
	{
		// Token: 0x040042BA RID: 17082
		EaseInQuad = 1,
		// Token: 0x040042BB RID: 17083
		EaseOutQuad,
		// Token: 0x040042BC RID: 17084
		EaseInOutQuad,
		// Token: 0x040042BD RID: 17085
		EaseInCubic,
		// Token: 0x040042BE RID: 17086
		EaseOutCubic,
		// Token: 0x040042BF RID: 17087
		EaseInOutCubic,
		// Token: 0x040042C0 RID: 17088
		EaseInQuart,
		// Token: 0x040042C1 RID: 17089
		EaseOutQuart,
		// Token: 0x040042C2 RID: 17090
		EaseInOutQuart,
		// Token: 0x040042C3 RID: 17091
		EaseInQuint,
		// Token: 0x040042C4 RID: 17092
		EaseOutQuint,
		// Token: 0x040042C5 RID: 17093
		EaseInOutQuint,
		// Token: 0x040042C6 RID: 17094
		EaseInSine,
		// Token: 0x040042C7 RID: 17095
		EaseOutSine,
		// Token: 0x040042C8 RID: 17096
		EaseInOutSine,
		// Token: 0x040042C9 RID: 17097
		EaseInExpo,
		// Token: 0x040042CA RID: 17098
		EaseOutExpo,
		// Token: 0x040042CB RID: 17099
		EaseInOutExpo,
		// Token: 0x040042CC RID: 17100
		EaseInCirc,
		// Token: 0x040042CD RID: 17101
		EaseOutCirc,
		// Token: 0x040042CE RID: 17102
		EaseInOutCirc,
		// Token: 0x040042CF RID: 17103
		EaseInBounce,
		// Token: 0x040042D0 RID: 17104
		EaseOutBounce,
		// Token: 0x040042D1 RID: 17105
		EaseInOutBounce,
		// Token: 0x040042D2 RID: 17106
		EaseInBack,
		// Token: 0x040042D3 RID: 17107
		EaseOutBack,
		// Token: 0x040042D4 RID: 17108
		EaseInOutBack,
		// Token: 0x040042D5 RID: 17109
		EaseInElastic,
		// Token: 0x040042D6 RID: 17110
		EaseOutElastic,
		// Token: 0x040042D7 RID: 17111
		EaseInOutElastic,
		// Token: 0x040042D8 RID: 17112
		Spring,
		// Token: 0x040042D9 RID: 17113
		Linear,
		// Token: 0x040042DA RID: 17114
		Step
	}
}
