using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000890 RID: 2192
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x06003727 RID: 14119 RVA: 0x0011EF88 File Offset: 0x0011D188
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = (subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1);
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x0011F044 File Offset: 0x0011D244
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x06003729 RID: 14121 RVA: 0x0011F054 File Offset: 0x0011D254
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x0600372A RID: 14122 RVA: 0x0011F074 File Offset: 0x0011D274
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x0600372B RID: 14123 RVA: 0x0011F07C File Offset: 0x0011D27C
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x0600372C RID: 14124 RVA: 0x0011F0AC File Offset: 0x0011D2AC
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x0600372D RID: 14125 RVA: 0x0011F117 File Offset: 0x0011D317
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600372E RID: 14126 RVA: 0x0011F129 File Offset: 0x0011D329
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x0011F13A File Offset: 0x0011D33A
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x040043CA RID: 17354
	public ParticleSystem target;

	// Token: 0x040043CB RID: 17355
	public ParticleSystem subEmitter;

	// Token: 0x040043CC RID: 17356
	public int subEmitterIndex;

	// Token: 0x040043CD RID: 17357
	public UnityEvent onSubEmit;

	// Token: 0x040043CE RID: 17358
	public float intervalScale = 1f;

	// Token: 0x040043CF RID: 17359
	public float interval;

	// Token: 0x040043D0 RID: 17360
	[NonSerialized]
	private bool _canListen;

	// Token: 0x040043D1 RID: 17361
	[NonSerialized]
	private bool _listening;

	// Token: 0x040043D2 RID: 17362
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x040043D3 RID: 17363
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
