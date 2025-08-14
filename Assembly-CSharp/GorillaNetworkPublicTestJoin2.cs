using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A01 RID: 2561
public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	// Token: 0x06003E95 RID: 16021 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Awake()
	{
	}

	// Token: 0x06003E96 RID: 16022 RVA: 0x0013ECB8 File Offset: 0x0013CEB8
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

	// Token: 0x06003E97 RID: 16023 RVA: 0x0013EDEC File Offset: 0x0013CFEC
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

	// Token: 0x04004A96 RID: 19094
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04004A97 RID: 19095
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04004A98 RID: 19096
	public string gameModeName;

	// Token: 0x04004A99 RID: 19097
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04004A9A RID: 19098
	public string componentTypeToAdd;

	// Token: 0x04004A9B RID: 19099
	public GameObject componentTarget;

	// Token: 0x04004A9C RID: 19100
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04004A9D RID: 19101
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04004A9E RID: 19102
	private Transform tosPition;

	// Token: 0x04004A9F RID: 19103
	private Transform othsTosPosition;

	// Token: 0x04004AA0 RID: 19104
	private PhotonView fotVew;

	// Token: 0x04004AA1 RID: 19105
	private bool waiting;

	// Token: 0x04004AA2 RID: 19106
	private Vector3 lastPosition;

	// Token: 0x04004AA3 RID: 19107
	private VRRig tempRig;
}
