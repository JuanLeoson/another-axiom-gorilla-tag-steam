using System;
using System.Diagnostics;
using Cysharp.Text;
using Drawing;
using UnityEngine;

// Token: 0x020001FC RID: 508
public static class GTDev
{
	// Token: 0x06000BF9 RID: 3065 RVA: 0x00040EA0 File Offset: 0x0003F0A0
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void Log<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void Log<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogError<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogError<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogWarning<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogWarning<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogSilent<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	public static void LogSilent<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000C0B RID: 3083 RVA: 0x00040EA8 File Offset: 0x0003F0A8
	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00040EB0 File Offset: 0x0003F0B0
	private static int FetchDevID()
	{
		if (GTDev.gHasDevID)
		{
			return GTDev.gDevID;
		}
		int i = StaticHash.Compute(SystemInfo.deviceUniqueIdentifier);
		int i2 = StaticHash.Compute(Environment.UserDomainName);
		int i3 = StaticHash.Compute(Environment.UserName);
		int i4 = StaticHash.Compute(Application.unityVersion);
		GTDev.gDevID = StaticHash.Compute(i, i2, i3, i4);
		GTDev.gHasDevID = true;
		return GTDev.gDevID;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("_GTDEV_ON_")]
	private static void _Log<T>(Action<object, Object> log, Action<object> logNoCtx, T msg, Object ctx, string channel)
	{
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00040F0D File Offset: 0x0003F10D
	private static Mesh SphereMesh()
	{
		if (!GTDev.gSphereMesh)
		{
			GTDev.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
		}
		return GTDev.gSphereMesh;
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00040F30 File Offset: 0x0003F130
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Collider col, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		if (color.a.Approx0(1E-06f))
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = col.transform.localToWorldMatrix;
		SRand srand = new SRand(localToWorldMatrix.QuantizedId128().GetHashCode());
		color.r = srand.NextFloat();
		color.g = srand.NextFloat();
		color.b = srand.NextFloat();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.PushMatrix(localToWorldMatrix);
			commandBuilder.PushLineWidth(2f, true);
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = col as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = col as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = col as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius, color);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius, color);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.Label2D(Vector3.zero, col.name, 16f, LabelAlignment.Center);
			commandBuilder.PopColor();
			commandBuilder.PopLineWidth();
			commandBuilder.PopMatrix();
		}
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x000410D4 File Offset: 0x0003F2D4
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Vector3 vec, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		else
		{
			color.a = GTDev.gDefaultColor.a;
		}
		string text = ZString.Format<float, float, float>("{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}", vec.x, vec.y, vec.z);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.Cross(vec, 0.64f, color);
			}
			commandBuilder.Label2D(vec + Vector3.down * 0.64f, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x000411C8 File Offset: 0x0003F3C8
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D<T>(this T value, Vector3 position, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		string text = ZString.Concat<T>(value);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.Label2D(position, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x04000EDD RID: 3805
	[OnEnterPlay_Set(0)]
	private static int gDevID;

	// Token: 0x04000EDE RID: 3806
	[OnEnterPlay_Set(false)]
	private static bool gHasDevID;

	// Token: 0x04000EDF RID: 3807
	private static readonly Color gDefaultColor = new Color(0f, 1f, 1f, 0.32f);

	// Token: 0x04000EE0 RID: 3808
	private const string kFormatF = "{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}";

	// Token: 0x04000EE1 RID: 3809
	private const float kDuration = 8f;

	// Token: 0x04000EE2 RID: 3810
	private static Mesh gSphereMesh;
}
