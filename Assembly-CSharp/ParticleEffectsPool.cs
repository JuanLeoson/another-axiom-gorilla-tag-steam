using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class ParticleEffectsPool : MonoBehaviour
{
	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003A4B9 File Offset: 0x000386B9
	public void Awake()
	{
		this.OnPoolAwake();
		this.Setup();
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnPoolAwake()
	{
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0003A4C8 File Offset: 0x000386C8
	private void Setup()
	{
		this.MoveToSceneWorldRoot();
		this._pools = new RingBuffer<ParticleEffect>[this.effects.Length];
		this._effectToPool = new Dictionary<long, int>(this.effects.Length);
		for (int i = 0; i < this.effects.Length; i++)
		{
			ParticleEffect particleEffect = this.effects[i];
			this._pools[i] = this.InitPoolForPrefab(i, this.effects[i]);
			this._effectToPool.TryAdd(particleEffect.effectID, i);
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0003A547 File Offset: 0x00038747
	private void MoveToSceneWorldRoot()
	{
		Transform transform = base.transform;
		transform.parent = null;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003A578 File Offset: 0x00038778
	private RingBuffer<ParticleEffect> InitPoolForPrefab(int index, ParticleEffect prefab)
	{
		RingBuffer<ParticleEffect> ringBuffer = new RingBuffer<ParticleEffect>(this.poolSize);
		string arg = prefab.name.Trim();
		for (int i = 0; i < this.poolSize; i++)
		{
			ParticleEffect particleEffect = Object.Instantiate<ParticleEffect>(prefab, base.transform);
			particleEffect.gameObject.SetActive(false);
			particleEffect.pool = this;
			particleEffect.poolIndex = index;
			particleEffect.name = ZString.Concat<string, string, int>(arg, "*", i);
			ringBuffer.Push(particleEffect);
		}
		return ringBuffer;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0003A5F0 File Offset: 0x000387F0
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos)
	{
		this.PlayEffect(effect.effectID, worldPos);
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0003A5FF File Offset: 0x000387FF
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos, float delay)
	{
		this.PlayEffect(effect.effectID, worldPos, delay);
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003A60F File Offset: 0x0003880F
	public void PlayEffect(long effectID, Vector3 worldPos)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0003A61F File Offset: 0x0003881F
	public void PlayEffect(long effectID, Vector3 worldPos, float delay)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos, delay);
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0003A630 File Offset: 0x00038830
	public void PlayEffect(int index, Vector3 worldPos)
	{
		if (index == -1)
		{
			return;
		}
		ParticleEffect particleEffect;
		if (!this._pools[index].TryPop(out particleEffect))
		{
			return;
		}
		particleEffect.transform.localPosition = worldPos;
		particleEffect.Play();
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0003A666 File Offset: 0x00038866
	public void PlayEffect(int index, Vector3 worldPos, float delay)
	{
		if (delay.Approx(0f, 1E-06f))
		{
			this.PlayEffect(index, worldPos);
			return;
		}
		base.StartCoroutine(this.PlayDelayed(index, worldPos, delay));
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003A693 File Offset: 0x00038893
	private IEnumerator PlayDelayed(int index, Vector3 worldPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayEffect(index, worldPos);
		yield break;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003A6B7 File Offset: 0x000388B7
	public void Return(ParticleEffect effect)
	{
		this._pools[effect.poolIndex].Push(effect);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003A6D0 File Offset: 0x000388D0
	public int GetPoolIndex(long effectID)
	{
		int result;
		if (this._effectToPool.TryGetValue(effectID, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x04000D4F RID: 3407
	public ParticleEffect[] effects = new ParticleEffect[0];

	// Token: 0x04000D50 RID: 3408
	[Space]
	public int poolSize = 10;

	// Token: 0x04000D51 RID: 3409
	[Space]
	private RingBuffer<ParticleEffect>[] _pools = new RingBuffer<ParticleEffect>[0];

	// Token: 0x04000D52 RID: 3410
	private Dictionary<long, int> _effectToPool = new Dictionary<long, int>();
}
