using System;
using UnityEngine;

// Token: 0x020007FA RID: 2042
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x0600331A RID: 13082 RVA: 0x0010A1EC File Offset: 0x001083EC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.IsDummyButton)
		{
			SynchedMusicController[] array = this.musicControllers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MuteAudio(this);
			}
			return;
		}
		if (this._targetMuteButton != null)
		{
			this._targetMuteButton.ButtonActivation();
		}
	}

	// Token: 0x0400401F RID: 16415
	public SynchedMusicController[] musicControllers;

	// Token: 0x04004020 RID: 16416
	[Tooltip("If true, then this button will passthrough clicks to a connected SoundPostMuteButton.")]
	public bool IsDummyButton;

	// Token: 0x04004021 RID: 16417
	[SerializeField]
	[Tooltip("The targetted SoundPostMuteButton if this is a dummy button.")]
	private SoundPostMuteButton _targetMuteButton;
}
