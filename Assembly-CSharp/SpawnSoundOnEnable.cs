using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class SpawnSoundOnEnable : MonoBehaviour
{
	// Token: 0x0600034F RID: 847 RVA: 0x00013D64 File Offset: 0x00011F64
	private void OnEnable()
	{
		if (CrittersManager.instance == null || !CrittersManager.instance.LocalAuthority() || !CrittersManager.instance.LocalInZone)
		{
			return;
		}
		if (!this.triggerOnFirstEnable && !this.firstEnabledOccured)
		{
			this.firstEnabledOccured = true;
			return;
		}
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
	}

	// Token: 0x040003E9 RID: 1001
	public int soundSubIndex = 3;

	// Token: 0x040003EA RID: 1002
	public bool triggerOnFirstEnable;

	// Token: 0x040003EB RID: 1003
	private bool firstEnabledOccured;
}
