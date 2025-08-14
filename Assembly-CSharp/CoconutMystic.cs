using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x02000432 RID: 1074
public class CoconutMystic : MonoBehaviour
{
	// Token: 0x06001A1C RID: 6684 RVA: 0x0008BE40 File Offset: 0x0008A040
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x0008BE4E File Offset: 0x0008A04E
	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this.OnPhotonEvent;
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x0008BE66 File Offset: 0x0008A066
	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.OnPhotonEvent;
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x0008BE80 File Offset: 0x0008A080
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != CoconutMystic.kUpdateLabelEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
		if (player != owningNetPlayer)
		{
			return;
		}
		int index = (int)array[1];
		this.label.text = this.answers.GetItem(index);
		this.soundPlayer.Play();
		this.breakEffect.Play();
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x0008BF20 File Offset: 0x0008A120
	public void UpdateLabel()
	{
		bool flag = this.geodeItem.currentState == TransferrableObject.PositionState.InLeftHand;
		this.label.rectTransform.localRotation = Quaternion.Euler(0f, flag ? 270f : 90f, 0f);
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x0008BF6C File Offset: 0x0008A16C
	public void ShowAnswer()
	{
		this.answers.distinct = this.distinct;
		this.label.text = this.answers.NextItem();
		this.soundPlayer.Play();
		this.breakEffect.Play();
		object eventContent = new object[]
		{
			CoconutMystic.kUpdateLabelEvent,
			this.answers.lastItemIndex
		};
		PhotonNetwork.RaiseEvent(176, eventContent, RaiseEventOptions.Default, SendOptions.SendReliable);
	}

	// Token: 0x04002258 RID: 8792
	public VRRig rig;

	// Token: 0x04002259 RID: 8793
	public GeodeItem geodeItem;

	// Token: 0x0400225A RID: 8794
	public SoundBankPlayer soundPlayer;

	// Token: 0x0400225B RID: 8795
	public ParticleSystem breakEffect;

	// Token: 0x0400225C RID: 8796
	public RandomStrings answers;

	// Token: 0x0400225D RID: 8797
	public TMP_Text label;

	// Token: 0x0400225E RID: 8798
	public bool distinct;

	// Token: 0x0400225F RID: 8799
	private static readonly int kUpdateLabelEvent = "CoconutMystic.UpdateLabel".GetStaticHash();
}
