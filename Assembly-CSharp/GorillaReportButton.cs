using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003C6 RID: 966
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x0600165F RID: 5727 RVA: 0x00079AB2 File Offset: 0x00077CB2
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00079ABC File Offset: 0x00077CBC
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					67,
					false,
					0.05f
				});
			}
		}
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x00079BAF File Offset: 0x00077DAF
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x00079BC7 File Offset: 0x00077DC7
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x04001E14 RID: 7700
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x04001E15 RID: 7701
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04001E16 RID: 7702
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04001E17 RID: 7703
	public bool isOn;

	// Token: 0x04001E18 RID: 7704
	public Material offMaterial;

	// Token: 0x04001E19 RID: 7705
	public Material onMaterial;

	// Token: 0x04001E1A RID: 7706
	public string offText;

	// Token: 0x04001E1B RID: 7707
	public string onText;

	// Token: 0x04001E1C RID: 7708
	public Text myText;

	// Token: 0x04001E1D RID: 7709
	public float debounceTime = 0.25f;

	// Token: 0x04001E1E RID: 7710
	public float touchTime;

	// Token: 0x04001E1F RID: 7711
	public bool testPress;

	// Token: 0x04001E20 RID: 7712
	public bool selected;

	// Token: 0x020003C7 RID: 967
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x04001E22 RID: 7714
		HateSpeech,
		// Token: 0x04001E23 RID: 7715
		Cheating,
		// Token: 0x04001E24 RID: 7716
		Toxicity,
		// Token: 0x04001E25 RID: 7717
		Bullying,
		// Token: 0x04001E26 RID: 7718
		Doxing,
		// Token: 0x04001E27 RID: 7719
		Impersonation,
		// Token: 0x04001E28 RID: 7720
		Submit,
		// Token: 0x04001E29 RID: 7721
		Cancel
	}
}
