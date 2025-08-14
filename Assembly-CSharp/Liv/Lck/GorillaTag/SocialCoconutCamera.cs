using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x02000DF0 RID: 3568
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x06005867 RID: 22631 RVA: 0x001B747F File Offset: 0x001B567F
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x001B74B7 File Offset: 0x001B56B7
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06005869 RID: 22633 RVA: 0x001B74CC File Offset: 0x001B56CC
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x0400621B RID: 25115
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x0400621C RID: 25116
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x0400621D RID: 25117
		private bool _isActive;

		// Token: 0x0400621E RID: 25118
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x0400621F RID: 25119
		private string IS_RECORDING = "_Is_Recording";
	}
}
