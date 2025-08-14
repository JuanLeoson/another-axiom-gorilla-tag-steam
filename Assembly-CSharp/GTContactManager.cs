using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class GTContactManager : MonoBehaviour
{
	// Token: 0x06000BCF RID: 3023 RVA: 0x000023F5 File Offset: 0x000005F5
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00040950 File Offset: 0x0003EB50
	private static GTContactPoint[] InitContactPoints(int count)
	{
		GTContactPoint[] array = new GTContactPoint[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new GTContactPoint();
		}
		return array;
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0004097C File Offset: 0x0003EB7C
	public static void RaiseContact(Vector3 point, Vector3 normal)
	{
		if (GTContactManager.gNextFree == -1)
		{
			return;
		}
		float time = GTShaderGlobals.Time;
		GTContactPoint gtcontactPoint = GTContactManager._gContactPoints[GTContactManager.gNextFree];
		gtcontactPoint.contactPoint = point;
		gtcontactPoint.radius = 0.04f;
		gtcontactPoint.counterVelocity = normal;
		gtcontactPoint.timestamp = time;
		gtcontactPoint.lifetime = 2f;
		gtcontactPoint.color = GTContactManager.gRND.NextColor();
		gtcontactPoint.free = 0U;
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x000409E4 File Offset: 0x0003EBE4
	public static void ProcessContacts()
	{
		Matrix4x4[] shaderData = GTContactManager.ShaderData;
		GTContactPoint[] gContactPoints = GTContactManager._gContactPoints;
		int frame = GTShaderGlobals.Frame;
		for (int i = 0; i < 32; i++)
		{
			GTContactManager.Transfer(ref gContactPoints[i].data, ref shaderData[i]);
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00040A24 File Offset: 0x0003EC24
	private static void Transfer(ref Matrix4x4 from, ref Matrix4x4 to)
	{
		to.m00 = from.m00;
		to.m01 = from.m01;
		to.m02 = from.m02;
		to.m03 = from.m03;
		to.m10 = from.m10;
		to.m11 = from.m11;
		to.m12 = from.m12;
		to.m13 = from.m13;
		to.m20 = from.m20;
		to.m21 = from.m21;
		to.m22 = from.m22;
		to.m23 = from.m23;
		to.m30 = from.m30;
		to.m31 = from.m31;
		to.m32 = from.m32;
		to.m33 = from.m33;
	}

	// Token: 0x04000EC3 RID: 3779
	public const int MAX_CONTACTS = 32;

	// Token: 0x04000EC4 RID: 3780
	public static Matrix4x4[] ShaderData = new Matrix4x4[32];

	// Token: 0x04000EC5 RID: 3781
	private static GTContactPoint[] _gContactPoints = GTContactManager.InitContactPoints(32);

	// Token: 0x04000EC6 RID: 3782
	private static int gNextFree = 0;

	// Token: 0x04000EC7 RID: 3783
	private static SRand gRND = new SRand(DateTime.UtcNow);
}
