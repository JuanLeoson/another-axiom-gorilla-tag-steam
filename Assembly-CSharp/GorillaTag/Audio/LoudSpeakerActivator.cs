using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EEF RID: 3823
	public class LoudSpeakerActivator : MonoBehaviour
	{
		// Token: 0x06005ED7 RID: 24279 RVA: 0x001DE37B File Offset: 0x001DC57B
		private void Awake()
		{
			this._isLocal = this.IsParentedToLocalRig();
			if (!this._isLocal)
			{
				this._nonlocalRig = base.transform.root.GetComponent<VRRig>();
			}
		}

		// Token: 0x06005ED8 RID: 24280 RVA: 0x001DE3A8 File Offset: 0x001DC5A8
		private bool IsParentedToLocalRig()
		{
			if (VRRigCache.Instance.localRig == null)
			{
				return false;
			}
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				if (parent == VRRigCache.Instance.localRig.transform)
				{
					return true;
				}
				parent = parent.parent;
			}
			return false;
		}

		// Token: 0x06005ED9 RID: 24281 RVA: 0x001DE401 File Offset: 0x001DC601
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x06005EDA RID: 24282 RVA: 0x001DE40C File Offset: 0x001DC60C
		public void StartLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = true;
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._recorder.AllowVolumeAdjustment = true;
				this._recorder.VolumeAdjustment = this.VolumeAdjustment;
				this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x06005EDB RID: 24283 RVA: 0x001DE504 File Offset: 0x001DC704
		public void StopLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (!this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = false;
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._recorder.AllowVolumeAdjustment = false;
				this._recorder.VolumeAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x04006946 RID: 26950
		public float PitchAdjustment = 1f;

		// Token: 0x04006947 RID: 26951
		public float VolumeAdjustment = 2.5f;

		// Token: 0x04006948 RID: 26952
		public bool IsBroadcasting;

		// Token: 0x04006949 RID: 26953
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x0400694A RID: 26954
		[SerializeField]
		private GTRecorder _recorder;

		// Token: 0x0400694B RID: 26955
		private bool _isLocal;

		// Token: 0x0400694C RID: 26956
		private VRRig _nonlocalRig;
	}
}
