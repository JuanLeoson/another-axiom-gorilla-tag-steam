using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200088A RID: 2186
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x060036C3 RID: 14019 RVA: 0x0011E17C File Offset: 0x0011C37C
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x0011E18C File Offset: 0x0011C38C
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x040043B0 RID: 17328
	public ParticleSystem target;

	// Token: 0x040043B1 RID: 17329
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
