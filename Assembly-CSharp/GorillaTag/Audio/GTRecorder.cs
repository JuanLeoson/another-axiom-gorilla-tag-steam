using System;
using System.Collections;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EEB RID: 3819
	public class GTRecorder : Recorder
	{
		// Token: 0x06005EBF RID: 24255 RVA: 0x001DDE82 File Offset: 0x001DC082
		protected override MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, logger);
			return this._micWrapper;
		}

		// Token: 0x06005EC0 RID: 24256 RVA: 0x001DDEB0 File Offset: 0x001DC0B0
		private IEnumerator DoTestEcho()
		{
			base.DebugEchoMode = true;
			yield return new WaitForSeconds(this.DebugEchoLength);
			base.DebugEchoMode = false;
			yield return null;
			this._testEchoCoroutine = null;
			yield break;
		}

		// Token: 0x06005EC1 RID: 24257 RVA: 0x001DDEBF File Offset: 0x001DC0BF
		public void LateUpdate()
		{
			if (this._micWrapper != null)
			{
				this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
			}
		}

		// Token: 0x04006931 RID: 26929
		public bool AllowPitchAdjustment;

		// Token: 0x04006932 RID: 26930
		public float PitchAdjustment = 1f;

		// Token: 0x04006933 RID: 26931
		public bool AllowVolumeAdjustment;

		// Token: 0x04006934 RID: 26932
		public float VolumeAdjustment = 1f;

		// Token: 0x04006935 RID: 26933
		public float DebugEchoLength = 5f;

		// Token: 0x04006936 RID: 26934
		private GTMicWrapper _micWrapper;

		// Token: 0x04006937 RID: 26935
		private Coroutine _testEchoCoroutine;
	}
}
