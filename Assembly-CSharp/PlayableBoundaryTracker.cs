using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class PlayableBoundaryTracker : MonoBehaviour
{
	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000927 RID: 2343 RVA: 0x000323EB File Offset: 0x000305EB
	// (set) Token: 0x06000928 RID: 2344 RVA: 0x000323F3 File Offset: 0x000305F3
	public float signedDistanceToBoundary { get; private set; }

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000929 RID: 2345 RVA: 0x000323FC File Offset: 0x000305FC
	// (set) Token: 0x0600092A RID: 2346 RVA: 0x00032404 File Offset: 0x00030604
	public float prevSignedDistanceToBoundary { get; private set; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x0600092B RID: 2347 RVA: 0x0003240D File Offset: 0x0003060D
	// (set) Token: 0x0600092C RID: 2348 RVA: 0x00032415 File Offset: 0x00030615
	public float timeSinceCrossingBorder { get; private set; }

	// Token: 0x0600092D RID: 2349 RVA: 0x0003241E File Offset: 0x0003061E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInsideZone()
	{
		return Mathf.Sign(this.signedDistanceToBoundary) < 0f;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00032434 File Offset: 0x00030634
	public void UpdateSignedDistanceToBoundary(float newDistance, float elapsed)
	{
		this.prevSignedDistanceToBoundary = this.signedDistanceToBoundary;
		this.signedDistanceToBoundary = newDistance;
		if ((int)Mathf.Sign(this.prevSignedDistanceToBoundary) != (int)Mathf.Sign(this.signedDistanceToBoundary))
		{
			this.timeSinceCrossingBorder = 0f;
			return;
		}
		this.timeSinceCrossingBorder += elapsed;
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00032488 File Offset: 0x00030688
	internal void ResetValues()
	{
		this.timeSinceCrossingBorder = 0f;
	}

	// Token: 0x04000AD4 RID: 2772
	public float radius = 1f;
}
