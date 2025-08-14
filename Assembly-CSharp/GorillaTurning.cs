using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000733 RID: 1843
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06002E29 RID: 11817 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x04003A00 RID: 14848
	public Material redMaterial;

	// Token: 0x04003A01 RID: 14849
	public Material blueMaterial;

	// Token: 0x04003A02 RID: 14850
	public Material greenMaterial;

	// Token: 0x04003A03 RID: 14851
	public Material transparentBlueMaterial;

	// Token: 0x04003A04 RID: 14852
	public Material transparentRedMaterial;

	// Token: 0x04003A05 RID: 14853
	public Material transparentGreenMaterial;

	// Token: 0x04003A06 RID: 14854
	public MeshRenderer smoothTurnBox;

	// Token: 0x04003A07 RID: 14855
	public MeshRenderer snapTurnBox;

	// Token: 0x04003A08 RID: 14856
	public MeshRenderer noTurnBox;

	// Token: 0x04003A09 RID: 14857
	public GorillaSnapTurn snapTurn;

	// Token: 0x04003A0A RID: 14858
	public string currentChoice;

	// Token: 0x04003A0B RID: 14859
	public float currentSpeed;
}
