using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class PlantableFlagManager : MonoBehaviourPun, IPunObservable
{
	// Token: 0x06000CA6 RID: 3238 RVA: 0x00044230 File Offset: 0x00042430
	public void ResetMyFlags()
	{
		foreach (PlantableObject plantableObject in this.flags)
		{
			if (plantableObject.IsMyItem())
			{
				if (plantableObject.currentState != TransferrableObject.PositionState.Dropped)
				{
					plantableObject.DropItem();
				}
				plantableObject.ResetToHome();
			}
		}
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00044278 File Offset: 0x00042478
	public void ResetAllFlags()
	{
		foreach (PlantableObject plantableObject in this.flags)
		{
			if (!plantableObject.IsMyItem())
			{
				plantableObject.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (plantableObject.currentState != TransferrableObject.PositionState.Dropped)
			{
				plantableObject.DropItem();
			}
			plantableObject.ResetToHome();
		}
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000442F0 File Offset: 0x000424F0
	public void RainbowifyAllFlags(float saturation = 1f, float value = 1f)
	{
		Color red = Color.red;
		for (int i = 0; i < this.flags.Length; i++)
		{
			Color colorR = Color.HSVToRGB((float)i / (float)this.flags.Length, saturation, value);
			PlantableObject plantableObject = this.flags[i];
			if (plantableObject)
			{
				plantableObject.colorR = colorR;
				plantableObject.colorG = Color.black;
			}
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00044350 File Offset: 0x00042550
	public void Awake()
	{
		this.mode = new FlagCauldronColorer.ColorMode[this.flags.Length];
		this.flagColors = new PlantableObject.AppliedColors[this.flags.Length][];
		for (int i = 0; i < this.flags.Length; i++)
		{
			this.flagColors[i] = new PlantableObject.AppliedColors[20];
		}
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x000443A8 File Offset: 0x000425A8
	public void Update()
	{
		if (this.mode == null)
		{
			this.mode = new FlagCauldronColorer.ColorMode[this.flags.Length];
		}
		if (this.flagColors == null)
		{
			this.flagColors = new PlantableObject.AppliedColors[this.flags.Length][];
			for (int i = 0; i < this.flags.Length; i++)
			{
				this.flagColors[i] = new PlantableObject.AppliedColors[20];
			}
		}
		for (int j = 0; j < this.flags.Length; j++)
		{
			PlantableObject plantableObject = this.flags[j];
			if (plantableObject.IsMyItem())
			{
				Vector3.SqrMagnitude(plantableObject.flagTip.position - base.transform.position);
			}
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00044458 File Offset: 0x00042658
	[PunRPC]
	public void UpdateFlagColorRPC(int flagIndex, int colorIndex, PhotonMessageInfo info)
	{
		PlantableObject plantableObject = this.flags[flagIndex];
		if (colorIndex == 0)
		{
			plantableObject.ClearColors();
			return;
		}
		plantableObject.AddColor((PlantableObject.AppliedColors)colorIndex);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00044484 File Offset: 0x00042684
	public void UpdateFlagColors()
	{
		for (int i = 0; i < this.flagColors.Length; i++)
		{
			PlantableObject.AppliedColors[] array = this.flagColors[i];
			PlantableObject plantableObject = this.flags[i];
			if (!plantableObject.IsMyItem() && array.Length <= 20)
			{
				plantableObject.dippedColors = array;
				plantableObject.UpdateDisplayedDippedColor();
			}
		}
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x000444D4 File Offset: 0x000426D4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			for (int i = 0; i < this.flagColors.Length; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					stream.SendNext((int)this.flagColors[i][j]);
				}
			}
			return;
		}
		for (int k = 0; k < this.flagColors.Length; k++)
		{
			for (int l = 0; l < 20; l++)
			{
				this.flagColors[k][l] = (PlantableObject.AppliedColors)stream.ReceiveNext();
			}
		}
		this.UpdateFlagColors();
	}

	// Token: 0x04000F9E RID: 3998
	public PlantableObject[] flags;

	// Token: 0x04000F9F RID: 3999
	public FlagCauldronColorer[] cauldrons;

	// Token: 0x04000FA0 RID: 4000
	public FlagCauldronColorer.ColorMode[] mode;

	// Token: 0x04000FA1 RID: 4001
	public PlantableObject.AppliedColors[][] flagColors;
}
