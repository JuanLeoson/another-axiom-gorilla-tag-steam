using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200087A RID: 2170
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x0600366A RID: 13930 RVA: 0x0011CF3C File Offset: 0x0011B13C
	// (set) Token: 0x0600366B RID: 13931 RVA: 0x0011CF44 File Offset: 0x0011B144
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x0600366C RID: 13932 RVA: 0x0011CF7F File Offset: 0x0011B17F
	// (set) Token: 0x0600366D RID: 13933 RVA: 0x0011CF87 File Offset: 0x0011B187
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x0600366E RID: 13934 RVA: 0x0001D558 File Offset: 0x0001B758
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600366F RID: 13935
	protected abstract void OnLerp(float t);

	// Token: 0x06003670 RID: 13936 RVA: 0x0011CF9F File Offset: 0x0011B19F
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x0011CFB0 File Offset: 0x0011B1B0
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x0011CFDB File Offset: 0x0011B1DB
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06003674 RID: 13940 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06003675 RID: 13941 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06003677 RID: 13943 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x04004338 RID: 17208
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x04004339 RID: 17209
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x0400433A RID: 17210
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x0400433B RID: 17211
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x0400433C RID: 17212
	[NonSerialized]
	private bool _previewing;

	// Token: 0x0400433D RID: 17213
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x0400433E RID: 17214
	[NonSerialized]
	private bool _rendering;

	// Token: 0x0400433F RID: 17215
	[NonSerialized]
	private int _lastState;

	// Token: 0x04004340 RID: 17216
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04004341 RID: 17217
	[NonSerialized]
	private float _prevLerpTo;
}
