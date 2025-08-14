using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200054B RID: 1355
public class BuilderResourceMeter : MonoBehaviour
{
	// Token: 0x0600211C RID: 8476 RVA: 0x000B331C File Offset: 0x000B151C
	private void Awake()
	{
		this.fillColor = this.resourceColors.GetColorForType(this._resourceType);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.fillCube.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.fillColor);
		this.fillCube.SetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.emptyColor);
		this.emptyCube.SetPropertyBlock(materialPropertyBlock);
		this.fillAmount = this.fillTarget;
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000B3398 File Offset: 0x000B1598
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000B33C6 File Offset: 0x000B15C6
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000B33FC File Offset: 0x000B15FC
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag != this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			if (!flag)
			{
				this.fillCube.enabled = false;
				this.emptyCube.enabled = false;
				return;
			}
			this.fillCube.enabled = true;
			this.emptyCube.enabled = true;
			this.OnAvailableResourcesChange();
		}
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000B3460 File Offset: 0x000B1660
	public void OnAvailableResourcesChange()
	{
		if (this.table == null || this.table.maxResources == null)
		{
			return;
		}
		this.resourceMax = this.table.maxResources[(int)this._resourceType];
		int num = this.table.usedResources[(int)this._resourceType];
		if (num != this.usedResource)
		{
			this.usedResource = num;
			this.SetNormalizedFillTarget((float)(this.resourceMax - this.usedResource) / (float)this.resourceMax);
		}
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000B34E0 File Offset: 0x000B16E0
	public void UpdateMeterFill()
	{
		if (this.animatingMeter)
		{
			float newFill = Mathf.MoveTowards(this.fillAmount, this.fillTarget, this.lerpSpeed * Time.deltaTime);
			this.UpdateFill(newFill);
		}
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000B351C File Offset: 0x000B171C
	private void UpdateFill(float newFill)
	{
		this.fillAmount = newFill;
		if (Mathf.Approximately(this.fillAmount, this.fillTarget))
		{
			this.fillAmount = this.fillTarget;
			this.animatingMeter = false;
		}
		if (!this.inBuilderZone)
		{
			return;
		}
		if (this.fillAmount <= 1E-45f)
		{
			this.fillCube.enabled = false;
			float y = this.meterHeight / this.meshHeight;
			Vector3 localScale = new Vector3(this.emptyCube.transform.localScale.x, y, this.emptyCube.transform.localScale.z);
			Vector3 localPosition = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.emptyCube.transform.localScale = localScale;
			this.emptyCube.transform.localPosition = localPosition;
			this.emptyCube.enabled = true;
			return;
		}
		if (this.fillAmount >= 1f)
		{
			float y2 = this.meterHeight / this.meshHeight;
			Vector3 localScale2 = new Vector3(this.fillCube.transform.localScale.x, y2, this.fillCube.transform.localScale.z);
			Vector3 localPosition2 = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.fillCube.transform.localScale = localScale2;
			this.fillCube.transform.localPosition = localPosition2;
			this.fillCube.enabled = true;
			this.emptyCube.enabled = false;
			return;
		}
		float num = this.meterHeight / this.meshHeight * this.fillAmount;
		Vector3 localScale3 = new Vector3(this.fillCube.transform.localScale.x, num, this.fillCube.transform.localScale.z);
		Vector3 localPosition3 = new Vector3(0f, num * this.meshHeight / 2f, 0f);
		this.fillCube.transform.localScale = localScale3;
		this.fillCube.transform.localPosition = localPosition3;
		this.fillCube.enabled = true;
		float num2 = this.meterHeight / this.meshHeight * (1f - this.fillAmount);
		Vector3 localScale4 = new Vector3(this.emptyCube.transform.localScale.x, num2, this.emptyCube.transform.localScale.z);
		Vector3 localPosition4 = new Vector3(0f, this.meterHeight - num2 * this.meshHeight / 2f, 0f);
		this.emptyCube.transform.localScale = localScale4;
		this.emptyCube.transform.localPosition = localPosition4;
		this.emptyCube.enabled = true;
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000B37F0 File Offset: 0x000B19F0
	public void SetNormalizedFillTarget(float fill)
	{
		this.fillTarget = Mathf.Clamp(fill, 0f, 1f);
		this.animatingMeter = true;
	}

	// Token: 0x04002A5A RID: 10842
	public BuilderResourceColors resourceColors;

	// Token: 0x04002A5B RID: 10843
	public MeshRenderer fillCube;

	// Token: 0x04002A5C RID: 10844
	public MeshRenderer emptyCube;

	// Token: 0x04002A5D RID: 10845
	private Color fillColor = Color.white;

	// Token: 0x04002A5E RID: 10846
	public Color emptyColor = Color.black;

	// Token: 0x04002A5F RID: 10847
	[FormerlySerializedAs("MeterHeight")]
	public float meterHeight = 2f;

	// Token: 0x04002A60 RID: 10848
	public float meshHeight = 1f;

	// Token: 0x04002A61 RID: 10849
	public BuilderResourceType _resourceType;

	// Token: 0x04002A62 RID: 10850
	private float fillAmount;

	// Token: 0x04002A63 RID: 10851
	[Range(0f, 1f)]
	[SerializeField]
	private float fillTarget;

	// Token: 0x04002A64 RID: 10852
	public float lerpSpeed = 0.5f;

	// Token: 0x04002A65 RID: 10853
	private bool animatingMeter;

	// Token: 0x04002A66 RID: 10854
	private int resourceMax = -1;

	// Token: 0x04002A67 RID: 10855
	private int usedResource = -1;

	// Token: 0x04002A68 RID: 10856
	private bool inBuilderZone;

	// Token: 0x04002A69 RID: 10857
	internal BuilderTable table;
}
