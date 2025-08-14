using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C90 RID: 3216
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06004FAF RID: 20399 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06004FB0 RID: 20400 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004FB1 RID: 20401 RVA: 0x0018D71C File Offset: 0x0018B91C
		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x06004FB2 RID: 20402 RVA: 0x0018D764 File Offset: 0x0018B964
		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x06004FB3 RID: 20403 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x040058D3 RID: 22739
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
