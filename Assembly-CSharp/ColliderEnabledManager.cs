using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class ColliderEnabledManager : MonoBehaviour
{
	// Token: 0x060000AF RID: 175 RVA: 0x0000500D File Offset: 0x0000320D
	private void Start()
	{
		this.floorEnabled = true;
		this.floorCollidersEnabled = true;
		ColliderEnabledManager.instance = this;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005023 File Offset: 0x00003223
	private void OnDestroy()
	{
		ColliderEnabledManager.instance = null;
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x0000502B File Offset: 0x0000322B
	public void DisableFloorForFrame()
	{
		this.floorEnabled = false;
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00005034 File Offset: 0x00003234
	private void LateUpdate()
	{
		if (!this.floorEnabled && this.floorCollidersEnabled)
		{
			this.DisableFloor();
		}
		if (!this.floorCollidersEnabled && Time.time > this.timeDisabled + this.disableLength)
		{
			this.floorCollidersEnabled = true;
		}
		Collider[] array = this.floorCollider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.floorCollidersEnabled;
		}
		if (this.floorCollidersEnabled)
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsBeforeMaterial;
			}
		}
		else
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsAfterMaterial;
			}
		}
		this.floorEnabled = true;
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x000050F4 File Offset: 0x000032F4
	private void DisableFloor()
	{
		this.floorCollidersEnabled = false;
		this.timeDisabled = Time.time;
	}

	// Token: 0x040000C1 RID: 193
	public static ColliderEnabledManager instance;

	// Token: 0x040000C2 RID: 194
	public Collider[] floorCollider;

	// Token: 0x040000C3 RID: 195
	public bool floorEnabled;

	// Token: 0x040000C4 RID: 196
	public bool wasFloorEnabled;

	// Token: 0x040000C5 RID: 197
	public bool floorCollidersEnabled;

	// Token: 0x040000C6 RID: 198
	[GorillaSoundLookup]
	public int wallsBeforeMaterial;

	// Token: 0x040000C7 RID: 199
	[GorillaSoundLookup]
	public int wallsAfterMaterial;

	// Token: 0x040000C8 RID: 200
	public GorillaSurfaceOverride[] walls;

	// Token: 0x040000C9 RID: 201
	public float timeDisabled;

	// Token: 0x040000CA RID: 202
	public float disableLength;
}
