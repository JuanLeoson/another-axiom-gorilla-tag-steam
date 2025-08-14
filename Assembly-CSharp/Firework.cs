using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000B16 RID: 2838
public class Firework : MonoBehaviour
{
	// Token: 0x0600446B RID: 17515 RVA: 0x00155FB6 File Offset: 0x001541B6
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x0600446C RID: 17516 RVA: 0x00155FDC File Offset: 0x001541DC
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (array.Contains(this))
		{
			return;
		}
		array = (from x in array.Concat(new Firework[]
		{
			this
		})
		where x != null
		select x).ToArray<Firework>();
		this._controller.fireworks = array;
	}

	// Token: 0x0600446D RID: 17517 RVA: 0x0015606C File Offset: 0x0015426C
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x0600446E RID: 17518 RVA: 0x0015608D File Offset: 0x0015428D
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x04004E80 RID: 20096
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x04004E81 RID: 20097
	[Space]
	public Transform origin;

	// Token: 0x04004E82 RID: 20098
	public Transform target;

	// Token: 0x04004E83 RID: 20099
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x04004E84 RID: 20100
	public Color colorTarget = Color.magenta;

	// Token: 0x04004E85 RID: 20101
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x04004E86 RID: 20102
	public AudioSource sourceTarget;

	// Token: 0x04004E87 RID: 20103
	[Space]
	public ParticleSystem trail;

	// Token: 0x04004E88 RID: 20104
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x04004E89 RID: 20105
	[Space]
	public bool doTrail = true;

	// Token: 0x04004E8A RID: 20106
	public bool doTrailAudio = true;

	// Token: 0x04004E8B RID: 20107
	public bool doExplosion = true;
}
