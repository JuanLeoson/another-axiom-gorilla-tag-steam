using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EF1 RID: 3825
	public class LoudSpeakerTrigger : MonoBehaviour
	{
		// Token: 0x06005EEA RID: 24298 RVA: 0x001DE9D6 File Offset: 0x001DCBD6
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x06005EEB RID: 24299 RVA: 0x001DE9E0 File Offset: 0x001DCBE0
		public void OnPlayerEnter(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._network.StartBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x06005EEC RID: 24300 RVA: 0x001DEA34 File Offset: 0x001DCC34
		public void OnPlayerExit(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x04006953 RID: 26963
		public float PitchAdjustment = 1f;

		// Token: 0x04006954 RID: 26964
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x04006955 RID: 26965
		[SerializeField]
		private GTRecorder _recorder;
	}
}
