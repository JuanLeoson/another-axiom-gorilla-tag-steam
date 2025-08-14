using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000779 RID: 1913
public class SizeManager : MonoBehaviour
{
	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x06002FF2 RID: 12274 RVA: 0x000FC250 File Offset: 0x000FA450
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.ScaleMultiplier;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.ScaleMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x06002FF3 RID: 12275 RVA: 0x000FC28B File Offset: 0x000FA48B
	// (set) Token: 0x06002FF4 RID: 12276 RVA: 0x000FC2C0 File Offset: 0x000FA4C0
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x000FC31A File Offset: 0x000FA51A
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000FC334 File Offset: 0x000FA534
	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x000FC33C File Offset: 0x000FA53C
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x000FC38C File Offset: 0x000FA58C
	public void BuildInitialize()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.netView != null && this.targetRig.netView.Owner != NetworkSystem.Instance.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		this.buildInitialized = true;
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x000FC470 File Offset: 0x000FA670
	private void Awake()
	{
		if (!this.buildInitialized)
		{
			this.BuildInitialize();
		}
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000FC488 File Offset: 0x000FA688
	public void InvokeFixedUpdate()
	{
		float num = 1f;
		SizeChanger sizeChanger = this.ControllingChanger(this.targetRig.transform);
		switch (this.myType)
		{
		case SizeManager.SizeChangerType.LocalOffline:
			num = this.ScaleFromChanger(sizeChanger, this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.SetScaleMultiplier((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.LocalOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.OtherOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
			Vector3 scaleCenter;
			if (sizeChanger != null && sizeChanger.TryGetScaleCenterPoint(out scaleCenter))
			{
				if (this.myType == SizeManager.SizeChangerType.LocalOffline)
				{
					this.targetPlayer.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
				else if (this.myType == SizeManager.SizeChangerType.LocalOnline)
				{
					GTPlayer.Instance.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
			}
			if (this.myType == SizeManager.SizeChangerType.LocalOffline)
			{
				this.CheckSizeChangeEvents(num);
			}
		}
		this.lastScale = num;
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x000FC628 File Offset: 0x000FA828
	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.alwaysControlWhenEntered || (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold))
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x000FC6B4 File Offset: 0x000FA8B4
	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		SizeChanger.ChangerType changerType = sC.MyType;
		if (changerType == SizeChanger.ChangerType.Static)
		{
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		}
		if (changerType == SizeChanger.ChangerType.Continuous)
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		return 1f;
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x000FC786 File Offset: 0x000FA986
	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.ScaleMultiplier - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.ScaleMultiplier, targetSize, deltaTime / easing);
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x000FC7C0 File Offset: 0x000FA9C0
	private void CheckSizeChangeEvents(float newSize)
	{
		if (newSize < this.smallThreshold)
		{
			if (!this.isSmall)
			{
				this.isSmall = true;
				this.isLarge = false;
				PlayerGameEvents.MiscEvent("SizeSmall", 1);
				return;
			}
		}
		else if (newSize > this.largeThreshold)
		{
			if (!this.isLarge)
			{
				this.isLarge = true;
				this.isSmall = false;
				PlayerGameEvents.MiscEvent("SizeLarge", 1);
				return;
			}
		}
		else
		{
			this.isLarge = false;
			this.isSmall = false;
		}
	}

	// Token: 0x04003C05 RID: 15365
	public List<SizeChanger> touchingChangers;

	// Token: 0x04003C06 RID: 15366
	private LineRenderer[] lineRenderers;

	// Token: 0x04003C07 RID: 15367
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04003C08 RID: 15368
	public VRRig targetRig;

	// Token: 0x04003C09 RID: 15369
	public GTPlayer targetPlayer;

	// Token: 0x04003C0A RID: 15370
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04003C0B RID: 15371
	public float rate = 650f;

	// Token: 0x04003C0C RID: 15372
	public Transform mainCameraTransform;

	// Token: 0x04003C0D RID: 15373
	public SizeManager.SizeChangerType myType;

	// Token: 0x04003C0E RID: 15374
	public float lastScale;

	// Token: 0x04003C0F RID: 15375
	private bool buildInitialized;

	// Token: 0x04003C10 RID: 15376
	private const float returnToNormalEasing = 0.33f;

	// Token: 0x04003C11 RID: 15377
	private float smallThreshold = 0.6f;

	// Token: 0x04003C12 RID: 15378
	private float largeThreshold = 1.5f;

	// Token: 0x04003C13 RID: 15379
	private bool isSmall;

	// Token: 0x04003C14 RID: 15380
	private bool isLarge;

	// Token: 0x0200077A RID: 1914
	public enum SizeChangerType
	{
		// Token: 0x04003C16 RID: 15382
		LocalOffline,
		// Token: 0x04003C17 RID: 15383
		LocalOnline,
		// Token: 0x04003C18 RID: 15384
		OtherOnline
	}
}
