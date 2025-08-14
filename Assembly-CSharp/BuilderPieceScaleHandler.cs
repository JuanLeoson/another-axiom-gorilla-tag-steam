using System;
using System.Collections.Generic;
using GorillaTagScripts.Builder;
using UnityEngine;

// Token: 0x02000514 RID: 1300
public class BuilderPieceScaleHandler : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x06001FAD RID: 8109 RVA: 0x000A7680 File Offset: 0x000A5880
	public void OnPieceCreate(int pieceType, int pieceId)
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.SetScale(this.myPiece.GetScale());
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.SetScale(this.myPiece.GetScale());
		}
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000A7728 File Offset: 0x000A5928
	public void OnPieceDestroy()
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.RevertScale();
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.RevertScale();
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceActivate()
	{
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceDeactivate()
	{
	}

	// Token: 0x04002855 RID: 10325
	[SerializeField]
	private BuilderPiece myPiece;

	// Token: 0x04002856 RID: 10326
	[SerializeField]
	private List<BuilderScaleAudioRadius> audioScalers = new List<BuilderScaleAudioRadius>();

	// Token: 0x04002857 RID: 10327
	[SerializeField]
	private List<BuilderScaleParticles> particleScalers = new List<BuilderScaleParticles>();
}
