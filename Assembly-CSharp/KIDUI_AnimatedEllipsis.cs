using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000933 RID: 2355
public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	// Token: 0x06003A08 RID: 14856 RVA: 0x0012C1AA File Offset: 0x0012A3AA
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	// Token: 0x06003A09 RID: 14857 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06003A0A RID: 14858 RVA: 0x0012C1BB File Offset: 0x0012A3BB
	private void OnDisable()
	{
		this.StopAnimation();
	}

	// Token: 0x06003A0B RID: 14859 RVA: 0x0012C1C4 File Offset: 0x0012A3C4
	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	// Token: 0x06003A0C RID: 14860 RVA: 0x0012C29A File Offset: 0x0012A49A
	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float d = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * d;
			}
			int num2 = currIndex;
			currIndex = num2 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	// Token: 0x06003A0D RID: 14861 RVA: 0x0012C2A9 File Offset: 0x0012A4A9
	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float offsetTime = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num = this.LerpLoop(this._startingScale, this._endScale, time, offsetTime, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003A0E RID: 14862 RVA: 0x0012C2B8 File Offset: 0x0012A4B8
	public Task StartAnimation()
	{
		KIDUI_AnimatedEllipsis.<StartAnimation>d__24 <StartAnimation>d__;
		<StartAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAnimation>d__.<>4__this = this;
		<StartAnimation>d__.<>1__state = -1;
		<StartAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StartAnimation>d__24>(ref <StartAnimation>d__);
		return <StartAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06003A0F RID: 14863 RVA: 0x0012C2FC File Offset: 0x0012A4FC
	public Task StopAnimation()
	{
		KIDUI_AnimatedEllipsis.<StopAnimation>d__25 <StopAnimation>d__;
		<StopAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StopAnimation>d__.<>4__this = this;
		<StopAnimation>d__.<>1__state = -1;
		<StopAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StopAnimation>d__25>(ref <StopAnimation>d__);
		return <StopAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06003A10 RID: 14864 RVA: 0x0012C340 File Offset: 0x0012A540
	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float time2 = (offsetTime - time) % duration / duration;
		float t = this._ellipsisAnimationCurve.Evaluate(time2);
		return Mathf.Lerp(start, end, t);
	}

	// Token: 0x04004730 RID: 18224
	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	// Token: 0x04004731 RID: 18225
	[SerializeField]
	private int _ellipsisCount = 3;

	// Token: 0x04004732 RID: 18226
	[SerializeField]
	private GameObject _ellipsisPrefab;

	// Token: 0x04004733 RID: 18227
	[SerializeField]
	private GameObject _ellipsisRoot;

	// Token: 0x04004734 RID: 18228
	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	// Token: 0x04004735 RID: 18229
	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	// Token: 0x04004736 RID: 18230
	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	// Token: 0x04004737 RID: 18231
	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	// Token: 0x04004738 RID: 18232
	[SerializeField]
	private float _startingScale = 0.33f;

	// Token: 0x04004739 RID: 18233
	[SerializeField]
	private float _intermediaryScale = 0.66f;

	// Token: 0x0400473A RID: 18234
	[SerializeField]
	private float _endScale = 1f;

	// Token: 0x0400473B RID: 18235
	[SerializeField]
	private float _scaleDuration = 0.25f;

	// Token: 0x0400473C RID: 18236
	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	// Token: 0x0400473D RID: 18237
	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	// Token: 0x0400473E RID: 18238
	private bool _runAnimation;

	// Token: 0x0400473F RID: 18239
	private float _nextChange;

	// Token: 0x04004740 RID: 18240
	[TupleElementNames(new string[]
	{
		"ellipsis",
		"startingScale",
		"currentScale",
		"lerpT"
	})]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	// Token: 0x04004741 RID: 18241
	private Coroutine _animationCoroutine;
}
