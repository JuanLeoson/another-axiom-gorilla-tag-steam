using System;
using UnityEngine;

// Token: 0x0200044E RID: 1102
public class PlantableObject : TransferrableObject
{
	// Token: 0x06001B18 RID: 6936 RVA: 0x00090B2F File Offset: 0x0008ED2F
	protected override void Awake()
	{
		base.Awake();
		this.materialPropertyBlock = new MaterialPropertyBlock();
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x00090B44 File Offset: 0x0008ED44
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x00090BA4 File Offset: 0x0008EDA4
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001B1B RID: 6939 RVA: 0x00090C74 File Offset: 0x0008EE74
	// (set) Token: 0x06001B1C RID: 6940 RVA: 0x00090C7C File Offset: 0x0008EE7C
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001B1D RID: 6941 RVA: 0x00090C8B File Offset: 0x0008EE8B
	// (set) Token: 0x06001B1E RID: 6942 RVA: 0x00090C93 File Offset: 0x0008EE93
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001B1F RID: 6943 RVA: 0x00090CA2 File Offset: 0x0008EEA2
	// (set) Token: 0x06001B20 RID: 6944 RVA: 0x00090CAA File Offset: 0x0008EEAA
	public bool planted { get; private set; }

	// Token: 0x06001B21 RID: 6945 RVA: 0x00090CB4 File Offset: 0x0008EEB4
	public void SetPlanted(bool newPlanted)
	{
		if (this.planted != newPlanted)
		{
			if (newPlanted)
			{
				if (!this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
			}
			else
			{
				this.respawnAtTimestamp = 0f;
			}
			this.planted = newPlanted;
		}
	}

	// Token: 0x06001B22 RID: 6946 RVA: 0x00090D0C File Offset: 0x0008EF0C
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x00090D15 File Offset: 0x0008EF15
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x00090D1E File Offset: 0x0008EF1E
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x00090D27 File Offset: 0x0008EF27
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x00090D30 File Offset: 0x0008EF30
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x00090D6C File Offset: 0x0008EF6C
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x00090DA4 File Offset: 0x0008EFA4
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[]
		{
			color.r,
			color.g,
			color.b
		});
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x00090EAD File Offset: 0x0008F0AD
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x00090EBB File Offset: 0x0008F0BB
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x00090EE8 File Offset: 0x0008F0E8
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
		if (this.respawnAtTimestamp != 0f && Time.time > this.respawnAtTimestamp)
		{
			this.respawnAtTimestamp = 0f;
			this.ResetToHome();
		}
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x00090F38 File Offset: 0x0008F138
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x00090F62 File Offset: 0x0008F162
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x00090F6C File Offset: 0x0008F16C
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x00090F84 File Offset: 0x0008F184
	public override void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		if (toPlayer.IsLocal && this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x0400237D RID: 9085
	public PlantablePoint point;

	// Token: 0x0400237E RID: 9086
	public float respawnAfterDuration;

	// Token: 0x0400237F RID: 9087
	private float respawnAtTimestamp;

	// Token: 0x04002380 RID: 9088
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04002381 RID: 9089
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04002382 RID: 9090
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x04002383 RID: 9091
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x04002385 RID: 9093
	public Transform flagTip;

	// Token: 0x04002386 RID: 9094
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x04002387 RID: 9095
	public int currentDipIndex;

	// Token: 0x0200044F RID: 1103
	public enum AppliedColors
	{
		// Token: 0x04002389 RID: 9097
		None,
		// Token: 0x0400238A RID: 9098
		Red,
		// Token: 0x0400238B RID: 9099
		Green,
		// Token: 0x0400238C RID: 9100
		Blue,
		// Token: 0x0400238D RID: 9101
		Black
	}
}
