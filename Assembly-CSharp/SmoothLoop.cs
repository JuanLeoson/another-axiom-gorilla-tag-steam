using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public class SmoothLoop : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06001C57 RID: 7255 RVA: 0x00097F8D File Offset: 0x0009618D
	public bool BuildValidationCheck()
	{
		if (this.source == null)
		{
			Debug.LogError("missing audio source, this will fail", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x00097FB0 File Offset: 0x000961B0
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.GTStop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x00098030 File Offset: 0x00096230
	public void SliceUpdate()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.source.time > this.source.clip.length * this.loopEnd)
		{
			this.source.time = this.loopStart;
		}
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x00098070 File Offset: 0x00096270
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.sourceCheck())
		{
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x000980D4 File Offset: 0x000962D4
	private bool sourceCheck()
	{
		if (!this.source || !this.source.clip)
		{
			Debug.LogError("SmoothLoop: Disabling because AudioSource is null or has no clip assigned. Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			base.StopAllCoroutines();
			return false;
		}
		return true;
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x0009812B File Offset: 0x0009632B
	public IEnumerator DelayedStart()
	{
		if (!this.sourceCheck())
		{
			yield break;
		}
		yield return new WaitForSeconds(this.delay);
		this.source.GTPlay();
		yield break;
	}

	// Token: 0x040024CA RID: 9418
	public AudioSource source;

	// Token: 0x040024CB RID: 9419
	public float delay;

	// Token: 0x040024CC RID: 9420
	public bool randomStart;

	// Token: 0x040024CD RID: 9421
	[SerializeField]
	[Range(0f, 1f)]
	private float loopStart = 0.1f;

	// Token: 0x040024CE RID: 9422
	[SerializeField]
	[Range(0f, 1f)]
	private float loopEnd = 0.95f;
}
