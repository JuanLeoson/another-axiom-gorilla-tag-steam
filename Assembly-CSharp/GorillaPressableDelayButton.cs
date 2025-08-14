using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class GorillaPressableDelayButton : GorillaPressableButton, IGorillaSliceableSimple
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060003C9 RID: 969 RVA: 0x000170E8 File Offset: 0x000152E8
	// (remove) Token: 0x060003CA RID: 970 RVA: 0x00017120 File Offset: 0x00015320
	public event Action onPressBegin;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x060003CB RID: 971 RVA: 0x00017158 File Offset: 0x00015358
	// (remove) Token: 0x060003CC RID: 972 RVA: 0x00017190 File Offset: 0x00015390
	public event Action onPressAbort;

	// Token: 0x060003CD RID: 973 RVA: 0x000171C8 File Offset: 0x000153C8
	private void Awake()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00017204 File Offset: 0x00015404
	private new void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touching)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touching = collider;
		this.timer = 0f;
		this.UpdateFillBar();
		Action action = this.onPressBegin;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00017274 File Offset: 0x00015474
	private void OnTriggerExit(Collider other)
	{
		if (other != this.touching)
		{
			return;
		}
		this.touching = null;
		this.timer = 0f;
		this.UpdateFillBar();
		Action action = this.onPressAbort;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x000172C0 File Offset: 0x000154C0
	public void SliceUpdate()
	{
		if (this.touching == null)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.delayTime)
		{
			base.OnTriggerEnter(this.touching);
			this.touching = null;
			this.timer = 0f;
		}
		this.UpdateFillBar();
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00017320 File Offset: 0x00015520
	public void SetFillBar(Transform newFillBar)
	{
		this.fillBar = newFillBar;
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00017364 File Offset: 0x00015564
	private void UpdateFillBar()
	{
		if (this.fillBar == null)
		{
			return;
		}
		float num = (this.delayTime > 0f) ? Mathf.Clamp01(this.timer / this.delayTime) : ((float)((this.timer > 0f) ? 1 : 0));
		this.fillBarScale.x = this.fillbarStartingScale.x * num;
		this.fillBar.localScale = this.fillBarScale;
	}

	// Token: 0x04000451 RID: 1105
	private Collider touching;

	// Token: 0x04000452 RID: 1106
	private float timer;

	// Token: 0x04000453 RID: 1107
	[SerializeField]
	[Range(0.01f, 5f)]
	public float delayTime = 1f;

	// Token: 0x04000454 RID: 1108
	[SerializeField]
	private Transform fillBar;

	// Token: 0x04000455 RID: 1109
	private Vector3 fillbarStartingScale;

	// Token: 0x04000456 RID: 1110
	private Vector3 fillBarScale;
}
