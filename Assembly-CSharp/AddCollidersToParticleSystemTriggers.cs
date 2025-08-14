using System;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x06001E8A RID: 7818 RVA: 0x000A1A6C File Offset: 0x0009FC6C
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x04002738 RID: 10040
	public Collider[] collidersToAdd;

	// Token: 0x04002739 RID: 10041
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x0400273A RID: 10042
	private int count;

	// Token: 0x0400273B RID: 10043
	private int index;
}
