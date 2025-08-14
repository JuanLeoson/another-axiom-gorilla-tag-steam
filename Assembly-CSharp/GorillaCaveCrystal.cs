using System;
using UnityEngine;

// Token: 0x020006BB RID: 1723
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x06002AAC RID: 10924 RVA: 0x000E31EF File Offset: 0x000E13EF
	private void Awake()
	{
		if (this.tapScript == null)
		{
			this.tapScript = base.GetComponent<TapInnerGlow>();
		}
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x000E320B File Offset: 0x000E140B
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x000E321A File Offset: 0x000E141A
	private void AnimateCrystal()
	{
		if (this.tapScript)
		{
			this.tapScript.Tap();
		}
	}

	// Token: 0x04003622 RID: 13858
	public bool overrideSoundAndMaterial;

	// Token: 0x04003623 RID: 13859
	public CrystalOctave octave;

	// Token: 0x04003624 RID: 13860
	public CrystalNote note;

	// Token: 0x04003625 RID: 13861
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x04003626 RID: 13862
	public TapInnerGlow tapScript;

	// Token: 0x04003627 RID: 13863
	[HideInInspector]
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x04003628 RID: 13864
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04003629 RID: 13865
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x0400362A RID: 13866
	[HideInInspector]
	[SerializeField]
	private bool _animating;

	// Token: 0x0400362B RID: 13867
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x0400362C RID: 13868
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
