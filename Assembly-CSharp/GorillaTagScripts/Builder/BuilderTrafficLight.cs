using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA3 RID: 3235
	public class BuilderTrafficLight : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06005056 RID: 20566 RVA: 0x00190E6B File Offset: 0x0018F06B
		private void Start()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x06005057 RID: 20567 RVA: 0x00190E78 File Offset: 0x0018F078
		private void SetState(BuilderTrafficLight.LightState state)
		{
			this.lightState = state;
			if (this.materialProps == null)
			{
				this.materialProps = new MaterialPropertyBlock();
			}
			Color value = this.yellowOff;
			Color value2 = this.redOff;
			Color value3 = this.greenOff;
			switch (state)
			{
			case BuilderTrafficLight.LightState.Red:
				value2 = this.redOn;
				break;
			case BuilderTrafficLight.LightState.Yellow:
				value = this.yellowOn;
				break;
			case BuilderTrafficLight.LightState.Green:
				value3 = this.greenOn;
				break;
			}
			this.redLight.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value2);
			this.redLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value);
			this.yellowLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value3);
			this.greenLight.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x06005058 RID: 20568 RVA: 0x00190F58 File Offset: 0x0018F158
		private void Update()
		{
			if (this.piece == null || this.piece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				float num = Time.time;
				if (PhotonNetwork.InRoom)
				{
					uint num2 = (uint)PhotonNetwork.ServerTimestamp;
					if (this.piece != null)
					{
						num2 = (uint)(PhotonNetwork.ServerTimestamp - this.piece.activatedTimeStamp);
					}
					num = num2 / 1000f;
				}
				float num3 = num % this.cycleDuration / this.cycleDuration;
				num3 = (num3 + this.startPercentageOffset) % 1f;
				int num4 = (int)this.stateCurve.Evaluate(num3);
				if (num4 != (int)this.lightState)
				{
					this.SetState((BuilderTrafficLight.LightState)num4);
				}
			}
		}

		// Token: 0x06005059 RID: 20569 RVA: 0x00190FFC File Offset: 0x0018F1FC
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x0600505A RID: 20570 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceActivate()
		{
		}

		// Token: 0x0600505D RID: 20573 RVA: 0x00190FFC File Offset: 0x0018F1FC
		public void OnPieceDeactivate()
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x0400599B RID: 22939
		[SerializeField]
		private BuilderPiece piece;

		// Token: 0x0400599C RID: 22940
		[SerializeField]
		private MeshRenderer redLight;

		// Token: 0x0400599D RID: 22941
		[SerializeField]
		private MeshRenderer yellowLight;

		// Token: 0x0400599E RID: 22942
		[SerializeField]
		private MeshRenderer greenLight;

		// Token: 0x0400599F RID: 22943
		[SerializeField]
		private float cycleDuration = 10f;

		// Token: 0x040059A0 RID: 22944
		[SerializeField]
		private float startPercentageOffset = 0.5f;

		// Token: 0x040059A1 RID: 22945
		[SerializeField]
		private Color redOn = Color.red;

		// Token: 0x040059A2 RID: 22946
		[SerializeField]
		private Color redOff = Color.gray;

		// Token: 0x040059A3 RID: 22947
		[SerializeField]
		private Color yellowOn = Color.yellow;

		// Token: 0x040059A4 RID: 22948
		[SerializeField]
		private Color yellowOff = Color.gray;

		// Token: 0x040059A5 RID: 22949
		[SerializeField]
		private Color greenOn = Color.green;

		// Token: 0x040059A6 RID: 22950
		[SerializeField]
		private Color greenOff = Color.gray;

		// Token: 0x040059A7 RID: 22951
		private MaterialPropertyBlock materialProps;

		// Token: 0x040059A8 RID: 22952
		[SerializeField]
		private AnimationCurve stateCurve;

		// Token: 0x040059A9 RID: 22953
		private BuilderTrafficLight.LightState lightState = BuilderTrafficLight.LightState.Off;

		// Token: 0x02000CA4 RID: 3236
		private enum LightState
		{
			// Token: 0x040059AB RID: 22955
			Red,
			// Token: 0x040059AC RID: 22956
			Yellow,
			// Token: 0x040059AD RID: 22957
			Green,
			// Token: 0x040059AE RID: 22958
			Off
		}
	}
}
