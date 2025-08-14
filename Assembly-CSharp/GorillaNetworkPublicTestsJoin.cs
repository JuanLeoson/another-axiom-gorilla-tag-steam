using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A03 RID: 2563
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox
{
	// Token: 0x06003E9F RID: 16031 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Awake()
	{
	}

	// Token: 0x06003EA0 RID: 16032 RVA: 0x0013EFD0 File Offset: 0x0013D1D0
	public void LateUpdate()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (GTPlayer.Instance.transform.position - this.lastPosition).magnitude;
			}
			this.lastPosition = GTPlayer.Instance.transform.position;
		}
		catch
		{
		}
	}

	// Token: 0x06003EA1 RID: 16033 RVA: 0x0013F104 File Offset: 0x0013D304
	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					GorillaNot.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						GTPlayer.Instance.jumpMultiplier.ToString(),
						".",
						GTPlayer.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	// Token: 0x04004AA7 RID: 19111
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04004AA8 RID: 19112
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04004AA9 RID: 19113
	public string gameModeName;

	// Token: 0x04004AAA RID: 19114
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04004AAB RID: 19115
	public string componentTypeToAdd;

	// Token: 0x04004AAC RID: 19116
	public GameObject componentTarget;

	// Token: 0x04004AAD RID: 19117
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04004AAE RID: 19118
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04004AAF RID: 19119
	private Transform tosPition;

	// Token: 0x04004AB0 RID: 19120
	private Transform othsTosPosition;

	// Token: 0x04004AB1 RID: 19121
	private PhotonView fotVew;

	// Token: 0x04004AB2 RID: 19122
	private bool waiting;

	// Token: 0x04004AB3 RID: 19123
	private Vector3 lastPosition;

	// Token: 0x04004AB4 RID: 19124
	private VRRig tempRig;
}
