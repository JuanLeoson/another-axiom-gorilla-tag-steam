using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020002AD RID: 685
public class NativeSizeVolume : MonoBehaviour
{
	// Token: 0x06000FD9 RID: 4057 RVA: 0x0005C254 File Offset: 0x0005A454
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onEnterAction = this.OnEnterAction;
		if (onEnterAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onEnterAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0005C2AC File Offset: 0x0005A4AC
	private void OnTriggerExit(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onExitAction = this.OnExitAction;
		if (onExitAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onExitAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x04001863 RID: 6243
	[SerializeField]
	private Collider triggerVolume;

	// Token: 0x04001864 RID: 6244
	[SerializeField]
	private NativeSizeChangerSettings settings;

	// Token: 0x04001865 RID: 6245
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnEnterAction;

	// Token: 0x04001866 RID: 6246
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnExitAction;

	// Token: 0x020002AE RID: 686
	[Serializable]
	private enum NativeSizeVolumeAction
	{
		// Token: 0x04001868 RID: 6248
		None,
		// Token: 0x04001869 RID: 6249
		ApplySettings,
		// Token: 0x0400186A RID: 6250
		ResetSize
	}
}
