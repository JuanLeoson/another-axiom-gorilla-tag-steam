using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000725 RID: 1829
public class GorillaTagManager : GorillaGameManager
{
	// Token: 0x06002DC6 RID: 11718 RVA: 0x000F2168 File Offset: 0x000F0368
	public override void Awake()
	{
		base.Awake();
		this.currentInfectedArray = new int[10];
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x000F21A4 File Offset: 0x000F03A4
	public override void StartPlaying()
	{
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentInfected.Count; i++)
			{
				this.tempPlayer = this.currentInfected[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentInfected.RemoveAt(i);
					i--;
				}
			}
			if (this.currentIt != null && !this.currentIt.InRoom())
			{
				this.currentIt = null;
			}
			if (this.lastInfectedPlayer != null && !this.lastInfectedPlayer.InRoom())
			{
				this.lastInfectedPlayer = null;
			}
			this.UpdateState();
		}
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x000F2251 File Offset: 0x000F0451
	public override void StopPlaying()
	{
		base.StopPlaying();
		base.StopAllCoroutines();
		this.lastTaggedActorNr.Clear();
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x000F226C File Offset: 0x000F046C
	public override void Reset()
	{
		base.Reset();
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
		this.currentInfected.Clear();
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.allInfected = false;
		this.isCurrentlyTag = false;
		this.waitingToStartNextInfectionGame = false;
		this.currentIt = null;
		this.lastInfectedPlayer = null;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x000F22E8 File Offset: 0x000F04E8
	public virtual void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (GorillaGameModes.GameMode.ParticipatingPlayers.Count < 1)
			{
				this.isCurrentlyTag = true;
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.currentIt = null;
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int index = Random.Range(0, GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GorillaGameModes.GameMode.ParticipatingPlayers[index], false);
				return;
			}
			if (this.isCurrentlyTag && GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int index2 = Random.Range(0, GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GorillaGameModes.GameMode.ParticipatingPlayers[index2], true);
				this.lastInfectedPlayer = GorillaGameModes.GameMode.ParticipatingPlayers[index2];
				return;
			}
			if (!this.isCurrentlyTag && GorillaGameModes.GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int index3 = Random.Range(0, GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GorillaGameModes.GameMode.ParticipatingPlayers[index3], false);
				return;
			}
			if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int index4 = Random.Range(0, GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GorillaGameModes.GameMode.ParticipatingPlayers[index4], true);
				return;
			}
			if (!this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x000F2456 File Offset: 0x000F0656
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateState();
		}
		this.inspectorLocalPlayerSpeed = this.LocalPlayerSpeed();
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x000F247C File Offset: 0x000F067C
	protected virtual IEnumerator InfectionRoundEndingCoroutine()
	{
		while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
		{
			this.InfectionRoundStart();
		}
		yield return null;
		yield break;
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x000F248C File Offset: 0x000F068C
	protected virtual void InfectionRoundStart()
	{
		this.ClearInfectionState();
		GorillaGameModes.GameMode.RefreshPlayers();
		List<NetPlayer> participatingPlayers = GorillaGameModes.GameMode.ParticipatingPlayers;
		if (participatingPlayers.Count > 0)
		{
			int index = Random.Range(0, participatingPlayers.Count);
			int num = 0;
			while (num < 10 && participatingPlayers[index] == this.lastInfectedPlayer)
			{
				index = Random.Range(0, participatingPlayers.Count);
				num++;
			}
			this.AddInfectedPlayer(participatingPlayers[index], true);
			this.lastInfectedPlayer = participatingPlayers[index];
			this.lastTag = (double)Time.time;
		}
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x000F2510 File Offset: 0x000F0710
	public virtual void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.allInfected = true;
		foreach (NetPlayer item in GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(item))
			{
				this.allInfected = false;
				break;
			}
		}
		if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
		{
			this.InfectionRoundEnd();
		}
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x000F25A4 File Offset: 0x000F07A4
	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		foreach (NetPlayer netPlayer in GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (this.currentIt == netPlayer)
			{
				if (withTagFreeze)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, netPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, netPlayer, false);
				break;
			}
		}
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x000F2628 File Offset: 0x000F0828
	protected virtual void InfectionRoundEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer player in GorillaGameModes.GameMode.ParticipatingPlayers)
			{
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, player, true);
			}
			PlayerGameEvents.GameModeCompleteRound();
			GorillaGameModes.GameMode.BroadcastRoundComplete();
			this.lastTaggedActorNr.Clear();
			this.waitingToStartNextInfectionGame = true;
			this.timeInfectedGameEnded = (double)Time.time;
			base.StartCoroutine(this.InfectionRoundEndingCoroutine());
		}
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x000F26C4 File Offset: 0x000F08C4
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x000F2700 File Offset: 0x000F0900
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		if (this.LocalCanTag(NetworkSystem.Instance.LocalPlayer, taggedPlayer) && (double)Time.time > this.lastQuestTagTime + (double)this.tagCoolDown)
		{
			PlayerGameEvents.MiscEvent("GameModeTag", 1);
			this.lastQuestTagTime = (double)Time.time;
			if (!this.isCurrentlyTag)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x000F275C File Offset: 0x000F095C
	protected float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpMultiplier;
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x000F27B0 File Offset: 0x000F09B0
	protected float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpLimit;
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x000F2804 File Offset: 0x000F0A04
	protected float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x000F2854 File Offset: 0x000F0A54
	protected float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x000F28A4 File Offset: 0x000F0AA4
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggingRig = this.FindPlayerVRRig(taggingPlayer);
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggingRig == null || this.taggedRig == null)
			{
				return;
			}
			this.taggedRig.SetTaggedBy(this.taggingRig);
			if (this.isCurrentlyTag)
			{
				if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.ChangeCurrentIt(taggedPlayer, true);
					this.lastTag = (double)Time.time;
					return;
				}
			}
			else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
			{
				if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
				{
					GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
					return;
				}
				this.HandleTagBroadcast(taggedPlayer, taggingPlayer);
				GorillaGameModes.GameMode.BroadcastTag(taggedPlayer, taggingPlayer);
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.AddInfectedPlayer(taggedPlayer, true);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x000F2A1C File Offset: 0x000F0C1C
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggedRig == null || this.waitingToStartNextInfectionGame || (double)Time.time < this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown))
			{
				return;
			}
			if (this.isCurrentlyTag)
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.ChangeCurrentIt(taggedPlayer, false);
				return;
			}
			if (!this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.AddInfectedPlayer(taggedPlayer, false);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000F2ABC File Offset: 0x000F0CBC
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && thisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000F2B15 File Offset: 0x000F0D15
	public bool IsInfected(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000F2B35 File Offset: 0x000F0D35
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (didTutorial)
				{
					this.AddInfectedPlayer(player, false);
				}
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x000F2B74 File Offset: 0x000F0D74
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while (this.currentInfected.Contains(otherPlayer))
			{
				this.currentInfected.Remove(otherPlayer);
			}
			if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber))
			{
				if (GorillaGameModes.GameMode.ParticipatingPlayers.Count > 0)
				{
					int index = Random.Range(0, GorillaGameModes.GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GorillaGameModes.GameMode.ParticipatingPlayers[index], false);
				}
			}
			else if (!this.isCurrentlyTag && GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.UpdateInfectionState();
			}
			this.UpdateState();
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000F2C34 File Offset: 0x000F0E34
	private void CopyInfectedListToArray()
	{
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			this.currentInfectedArray[this.iterator1] = -1;
			this.iterator1++;
		}
		this.iterator1 = this.currentInfected.Count - 1;
		while (this.iterator1 >= 0)
		{
			if (this.currentInfected[this.iterator1] == null)
			{
				this.currentInfected.RemoveAt(this.iterator1);
			}
			this.iterator1--;
		}
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfected.Count)
		{
			this.currentInfectedArray[this.iterator1] = this.currentInfected[this.iterator1].ActorNumber;
			this.iterator1++;
		}
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000F2D14 File Offset: 0x000F0F14
	private void CopyInfectedArrayToList()
	{
		this.currentInfected.Clear();
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			if (this.currentInfectedArray[this.iterator1] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentInfectedArray[this.iterator1]);
				if (this.tempPlayer != null)
				{
					this.currentInfected.Add(this.tempPlayer);
				}
			}
			this.iterator1++;
		}
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000F2D99 File Offset: 0x000F0F99
	protected virtual void ChangeCurrentIt(NetPlayer newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x000F2DB5 File Offset: 0x000F0FB5
	public void SetisCurrentlyTag(bool newTagSetting)
	{
		if (newTagSetting)
		{
			this.isCurrentlyTag = true;
		}
		else
		{
			this.isCurrentlyTag = false;
		}
		RoomSystem.SendSoundEffectAll(2, 0.25f, false);
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000F2DD6 File Offset: 0x000F0FD6
	public virtual void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.currentInfected.Add(infectedPlayer);
			if (!withTagStop)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, infectedPlayer);
			}
			else
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
			}
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer, false);
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000F2E16 File Offset: 0x000F1016
	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.waitingToStartNextInfectionGame = false;
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000F2E2A File Offset: 0x000F102A
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000F2E4B File Offset: 0x000F104B
	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.isCurrentlyTag)
		{
			this.UpdateTagState(true);
			return;
		}
		this.UpdateInfectionState();
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000F2E88 File Offset: 0x000F1088
	public override void OnSerializeRead(object newData)
	{
		TagData tagData = (TagData)newData;
		this.isCurrentlyTag = tagData.isCurrentlyTag;
		this.tempItInt = tagData.currentItID;
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		tagData.infectedPlayerList.CopyTo(this.currentInfectedArray, true);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000F2EF8 File Offset: 0x000F10F8
	public override object OnSerializeWrite()
	{
		this.CopyInfectedListToArray();
		TagData tagData = default(TagData);
		tagData.isCurrentlyTag = this.isCurrentlyTag;
		tagData.currentItID = ((this.currentIt != null) ? this.currentIt.ActorNumber : -1);
		tagData.infectedPlayerList.CopyFrom(this.currentInfectedArray, 0, this.currentInfectedArray.Length);
		return tagData;
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000F2F68 File Offset: 0x000F1168
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyInfectedListToArray();
		stream.SendNext(this.isCurrentlyTag);
		stream.SendNext((this.currentIt != null) ? this.currentIt.ActorNumber : -1);
		stream.SendNext(this.currentInfectedArray[0]);
		stream.SendNext(this.currentInfectedArray[1]);
		stream.SendNext(this.currentInfectedArray[2]);
		stream.SendNext(this.currentInfectedArray[3]);
		stream.SendNext(this.currentInfectedArray[4]);
		stream.SendNext(this.currentInfectedArray[5]);
		stream.SendNext(this.currentInfectedArray[6]);
		stream.SendNext(this.currentInfectedArray[7]);
		stream.SendNext(this.currentInfectedArray[8]);
		stream.SendNext(this.currentInfectedArray[9]);
		base.WriteLastTagged(stream);
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000F3074 File Offset: 0x000F1274
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		this.isCurrentlyTag = (bool)stream.ReceiveNext();
		this.tempItInt = (int)stream.ReceiveNext();
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		this.currentInfectedArray[0] = (int)stream.ReceiveNext();
		this.currentInfectedArray[1] = (int)stream.ReceiveNext();
		this.currentInfectedArray[2] = (int)stream.ReceiveNext();
		this.currentInfectedArray[3] = (int)stream.ReceiveNext();
		this.currentInfectedArray[4] = (int)stream.ReceiveNext();
		this.currentInfectedArray[5] = (int)stream.ReceiveNext();
		this.currentInfectedArray[6] = (int)stream.ReceiveNext();
		this.currentInfectedArray[7] = (int)stream.ReceiveNext();
		this.currentInfectedArray[8] = (int)stream.ReceiveNext();
		this.currentInfectedArray[9] = (int)stream.ReceiveNext();
		base.ReadLastTagged(stream);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x0001D558 File Offset: 0x0001B758
	public override GameModeType GameType()
	{
		return GameModeType.Infection;
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000F31A2 File Offset: 0x000F13A2
	public override void AddFusionDataBehaviour(NetworkObject netObject)
	{
		netObject.AddBehaviour<TagGameModeData>();
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000F31AB File Offset: 0x000F13AB
	public override string GameModeName()
	{
		return "INFECTION";
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000F31B2 File Offset: 0x000F13B2
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		if (this.isCurrentlyTag && forPlayer == this.currentIt)
		{
			return 1;
		}
		if (this.currentInfected.Contains(forPlayer))
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000F31D8 File Offset: 0x000F13D8
	public override float[] LocalPlayerSpeed()
	{
		if (this.isCurrentlyTag)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.currentIt)
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.slowJumpLimit;
			this.playerSpeed[1] = this.slowJumpMultiplier;
			return this.playerSpeed;
		}
		else
		{
			if (this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
			{
				this.playerSpeed[0] = this.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = this.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
			this.playerSpeed[1] = this.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
			return this.playerSpeed;
		}
	}

	// Token: 0x0400396E RID: 14702
	public float tagCoolDown = 5f;

	// Token: 0x0400396F RID: 14703
	public int infectedModeThreshold = 4;

	// Token: 0x04003970 RID: 14704
	public const byte ReportTagEvent = 1;

	// Token: 0x04003971 RID: 14705
	public const byte ReportInfectionTagEvent = 2;

	// Token: 0x04003972 RID: 14706
	[NonSerialized]
	public List<NetPlayer> currentInfected = new List<NetPlayer>(10);

	// Token: 0x04003973 RID: 14707
	[NonSerialized]
	public int[] currentInfectedArray;

	// Token: 0x04003974 RID: 14708
	[NonSerialized]
	public NetPlayer currentIt;

	// Token: 0x04003975 RID: 14709
	[NonSerialized]
	public NetPlayer lastInfectedPlayer;

	// Token: 0x04003976 RID: 14710
	public double lastTag;

	// Token: 0x04003977 RID: 14711
	public double timeInfectedGameEnded;

	// Token: 0x04003978 RID: 14712
	public bool waitingToStartNextInfectionGame;

	// Token: 0x04003979 RID: 14713
	public bool isCurrentlyTag;

	// Token: 0x0400397A RID: 14714
	private int tempItInt;

	// Token: 0x0400397B RID: 14715
	private int iterator1;

	// Token: 0x0400397C RID: 14716
	private NetPlayer tempPlayer;

	// Token: 0x0400397D RID: 14717
	private bool allInfected;

	// Token: 0x0400397E RID: 14718
	public float[] inspectorLocalPlayerSpeed;

	// Token: 0x0400397F RID: 14719
	private protected VRRig taggingRig;

	// Token: 0x04003980 RID: 14720
	private protected VRRig taggedRig;

	// Token: 0x04003981 RID: 14721
	private NetPlayer lastTaggedPlayer;

	// Token: 0x04003982 RID: 14722
	private double lastQuestTagTime;
}
