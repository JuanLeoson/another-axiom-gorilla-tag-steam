using System;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class GorillaMouthFlap : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002BEF RID: 11247 RVA: 0x000E8E73 File Offset: 0x000E7073
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
		this.targetFaceRenderer = this.targetFace.GetComponent<Renderer>();
		this.facePropBlock = new MaterialPropertyBlock();
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000E8E9D File Offset: 0x000E709D
	public void EnableLeafBlower()
	{
		this.leafBlowerActiveUntilTimestamp = Time.time + 0.1f;
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000E8EB0 File Offset: 0x000E70B0
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTimeUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x000E8ED0 File Offset: 0x000E70D0
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.lastTimeUpdated;
		this.lastTimeUpdated = Time.time;
		if (this.speaker == null)
		{
			this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
			return;
		}
		float currentLoudness = 0f;
		if (this.speaker.IsSpeaking)
		{
			currentLoudness = this.speaker.Loudness;
		}
		this.CheckMouthflapChange(this.speaker.IsMicEnabled, currentLoudness);
		MouthFlapLevel mouthFlap = this.noMicFace;
		if (this.leafBlowerActiveUntilTimestamp > Time.time)
		{
			mouthFlap = this.leafBlowerFace;
		}
		else if (this.useMicEnabled)
		{
			mouthFlap = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlap);
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x000E8F84 File Offset: 0x000E7184
	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x000E9010 File Offset: 0x000E7210
	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFaceRenderer.material;
		this.activeFlipbookPlayTime += this.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x000E9088 File Offset: 0x000E7288
	public void SetMouthTextureReplacement(Texture2D replacementMouthAtlas)
	{
		Material material = this.targetFaceRenderer.material;
		if (!this.hasDefaultMouthAtlas)
		{
			this.defaultMouthAtlas = material.GetTexture(this._MouthMap);
			this.hasDefaultMouthAtlas = true;
		}
		material.SetTexture(this._MouthMap, replacementMouthAtlas);
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x000E90D9 File Offset: 0x000E72D9
	public void ClearMouthTextureReplacement()
	{
		this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000E90FC File Offset: 0x000E72FC
	public Material SetFaceMaterialReplacement(Material replacementFaceMaterial)
	{
		if (!this.hasDefaultFaceMaterial)
		{
			this.defaultFaceMaterial = this.targetFaceRenderer.material;
			this.hasDefaultFaceMaterial = true;
		}
		this.targetFaceRenderer.material = replacementFaceMaterial;
		return this.targetFaceRenderer.material;
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000E9135 File Offset: 0x000E7335
	public void ClearFaceMaterialReplacement()
	{
		if (this.hasDefaultFaceMaterial)
		{
			this.targetFaceRenderer.material = this.defaultFaceMaterial;
		}
	}

	// Token: 0x0400375C RID: 14172
	public GameObject targetFace;

	// Token: 0x0400375D RID: 14173
	public MouthFlapLevel[] mouthFlapLevels;

	// Token: 0x0400375E RID: 14174
	public MouthFlapLevel noMicFace;

	// Token: 0x0400375F RID: 14175
	public MouthFlapLevel leafBlowerFace;

	// Token: 0x04003760 RID: 14176
	private bool useMicEnabled;

	// Token: 0x04003761 RID: 14177
	private float leafBlowerActiveUntilTimestamp;

	// Token: 0x04003762 RID: 14178
	private int activeFlipbookIndex;

	// Token: 0x04003763 RID: 14179
	private float activeFlipbookPlayTime;

	// Token: 0x04003764 RID: 14180
	private GorillaSpeakerLoudness speaker;

	// Token: 0x04003765 RID: 14181
	private float lastTimeUpdated;

	// Token: 0x04003766 RID: 14182
	private float deltaTime;

	// Token: 0x04003767 RID: 14183
	private Renderer targetFaceRenderer;

	// Token: 0x04003768 RID: 14184
	private MaterialPropertyBlock facePropBlock;

	// Token: 0x04003769 RID: 14185
	private Texture defaultMouthAtlas;

	// Token: 0x0400376A RID: 14186
	private Material defaultFaceMaterial;

	// Token: 0x0400376B RID: 14187
	private bool hasDefaultMouthAtlas;

	// Token: 0x0400376C RID: 14188
	private bool hasDefaultFaceMaterial;

	// Token: 0x0400376D RID: 14189
	private ShaderHashId _MouthMap = "_MouthMap";

	// Token: 0x0400376E RID: 14190
	private ShaderHashId _BaseMap = "_BaseMap";
}
