using System;
using UnityEngine;

// Token: 0x0200049F RID: 1183
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemEventShortcut : MonoBehaviour
{
	// Token: 0x06001D51 RID: 7505 RVA: 0x0009D4DD File Offset: 0x0009B6DD
	private void Awake()
	{
		this.ps = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x0009D4EB File Offset: 0x0009B6EB
	public void StopAndClear()
	{
		this.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x0009D4FA File Offset: 0x0009B6FA
	public void ClearAndPlay()
	{
		this.ps.Clear();
		this.ps.Play();
	}

	// Token: 0x040025C8 RID: 9672
	private ParticleSystem ps;
}
