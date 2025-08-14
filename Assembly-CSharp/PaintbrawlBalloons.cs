using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003DC RID: 988
public class PaintbrawlBalloons : MonoBehaviour
{
	// Token: 0x0600171F RID: 5919 RVA: 0x0007D30C File Offset: 0x0007B50C
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = ShaderProps._Color;
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x0007D392 File Offset: 0x0007B592
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x0007D39C File Offset: 0x0007B59C
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0007D488 File Offset: 0x0007B688
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x0007D4E0 File Offset: 0x0007B6E0
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else
			{
				this.teamColor = this.blueColor;
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty(ShaderProps._BaseColor))
							{
								material.SetColor(ShaderProps._BaseColor, this.teamColor);
							}
							if (material.HasProperty(ShaderProps._Color))
							{
								material.SetColor(ShaderProps._Color, this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x04001EF0 RID: 7920
	public VRRig myRig;

	// Token: 0x04001EF1 RID: 7921
	public GameObject[] balloons;

	// Token: 0x04001EF2 RID: 7922
	public Color orangeColor;

	// Token: 0x04001EF3 RID: 7923
	public Color blueColor;

	// Token: 0x04001EF4 RID: 7924
	public Color defaultColor;

	// Token: 0x04001EF5 RID: 7925
	public Color lastColor;

	// Token: 0x04001EF6 RID: 7926
	public GameObject balloonPopFXPrefab;

	// Token: 0x04001EF7 RID: 7927
	[HideInInspector]
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x04001EF8 RID: 7928
	public Player myPlayer;

	// Token: 0x04001EF9 RID: 7929
	private int colorShaderPropID;

	// Token: 0x04001EFA RID: 7930
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001EFB RID: 7931
	private bool[] balloonsCachedActiveState;

	// Token: 0x04001EFC RID: 7932
	private Renderer[] renderers;

	// Token: 0x04001EFD RID: 7933
	private Color teamColor;
}
