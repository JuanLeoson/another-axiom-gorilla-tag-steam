using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C91 RID: 3217
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06004FB5 RID: 20405 RVA: 0x0018D7AC File Offset: 0x0018B9AC
		private void OnZoneChanged()
		{
			this.inBuilderZone = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.inBuilderZone && this.isPieceActive)
			{
				this.StartParticles();
				return;
			}
			if (!this.inBuilderZone)
			{
				this.StopParticles();
			}
		}

		// Token: 0x06004FB6 RID: 20406 RVA: 0x0018D800 File Offset: 0x0018BA00
		private void StopParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x0018D860 File Offset: 0x0018BA60
		private void StartParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x0018D8BC File Offset: 0x0018BABC
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x0018D8F0 File Offset: 0x0018BAF0
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x06004FBA RID: 20410 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x0018D918 File Offset: 0x0018BB18
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x0018D92F File Offset: 0x0018BB2F
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x040058D4 RID: 22740
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040058D5 RID: 22741
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x040058D6 RID: 22742
		private bool inBuilderZone;

		// Token: 0x040058D7 RID: 22743
		private bool isPieceActive;
	}
}
