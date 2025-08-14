using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020006BC RID: 1724
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x06002AB0 RID: 10928 RVA: 0x000E3286 File Offset: 0x000E1486
	// (set) Token: 0x06002AB1 RID: 10929 RVA: 0x000E328E File Offset: 0x000E148E
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000E3298 File Offset: 0x000E1498
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(out this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = (this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null);
		this.Update();
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000E3314 File Offset: 0x000E1514
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000E3324 File Offset: 0x000E1524
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(GorillaCaveCrystalVisuals._MainTex, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000E3399 File Offset: 0x000E1599
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000E33A8 File Offset: 0x000E15A8
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color value = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color value2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(GorillaCaveCrystalVisuals._Color, value);
		this._block.SetColor(GorillaCaveCrystalVisuals._EmissionColor, value2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000E349E File Offset: 0x000E169E
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000E34B0 File Offset: 0x000E16B0
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x0400362D RID: 13869
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x0400362E RID: 13870
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x0400362F RID: 13871
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x04003630 RID: 13872
	public Material _sharedMaterial;

	// Token: 0x04003631 RID: 13873
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x04003632 RID: 13874
	[SerializeField]
	private bool _initialized;

	// Token: 0x04003633 RID: 13875
	[SerializeField]
	private int _lastState;

	// Token: 0x04003634 RID: 13876
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x04003635 RID: 13877
	private MaterialPropertyBlock _block;

	// Token: 0x04003636 RID: 13878
	[NonSerialized]
	private bool _ranSetupOnce;

	// Token: 0x04003637 RID: 13879
	private static readonly ShaderHashId _Color = "_Color";

	// Token: 0x04003638 RID: 13880
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";

	// Token: 0x04003639 RID: 13881
	private static readonly ShaderHashId _MainTex = "_MainTex";
}
