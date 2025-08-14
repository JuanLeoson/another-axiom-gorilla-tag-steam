using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
[Serializable]
public class NativeSizeChangerSettings
{
	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0005C1FD File Offset: 0x0005A3FD
	// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x0005C205 File Offset: 0x0005A405
	public Vector3 WorldPosition
	{
		get
		{
			return this.worldPosition;
		}
		set
		{
			this.worldPosition = value;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0005C20E File Offset: 0x0005A40E
	// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x0005C216 File Offset: 0x0005A416
	public float ActivationTime
	{
		get
		{
			return this.activationTime;
		}
		set
		{
			this.activationTime = value;
		}
	}

	// Token: 0x04001858 RID: 6232
	public const float MinAllowedSize = 0.1f;

	// Token: 0x04001859 RID: 6233
	public const float MaxAllowedSize = 10f;

	// Token: 0x0400185A RID: 6234
	private Vector3 worldPosition;

	// Token: 0x0400185B RID: 6235
	private float activationTime;

	// Token: 0x0400185C RID: 6236
	[Range(0.1f, 10f)]
	public float playerSizeScale = 1f;

	// Token: 0x0400185D RID: 6237
	public bool ExpireOnRoomJoin = true;

	// Token: 0x0400185E RID: 6238
	public bool ExpireInWater = true;

	// Token: 0x0400185F RID: 6239
	public float ExpireAfterSeconds;

	// Token: 0x04001860 RID: 6240
	public float ExpireOnDistance;
}
