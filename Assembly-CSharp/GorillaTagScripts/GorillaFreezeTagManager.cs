using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C38 RID: 3128
	public sealed class GorillaFreezeTagManager : GorillaTagManager
	{
		// Token: 0x06004D31 RID: 19761 RVA: 0x00144DAB File Offset: 0x00142FAB
		public override GameModeType GameType()
		{
			return GameModeType.FreezeTag;
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x0017FC42 File Offset: 0x0017DE42
		public override string GameModeName()
		{
			return "FREEZE TAG";
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x0017FC49 File Offset: 0x0017DE49
		public override void Awake()
		{
			base.Awake();
			this.fastJumpLimitCached = this.fastJumpLimit;
			this.fastJumpMultiplierCached = this.fastJumpMultiplier;
			this.slowJumpLimitCached = this.slowJumpLimit;
			this.slowJumpMultiplierCached = this.slowJumpMultiplier;
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x0017FC84 File Offset: 0x0017DE84
		public override void UpdateState()
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen.ToList<KeyValuePair<NetPlayer, float>>())
				{
					if (Time.time - keyValuePair.Value >= this.freezeDuration)
					{
						this.currentFrozen.Remove(keyValuePair.Key);
						this.AddInfectedPlayer(keyValuePair.Key, false);
						RoomSystem.SendSoundEffectAll(11, 0.25f, false);
					}
				}
				if (GameMode.ParticipatingPlayers.Count < 1)
				{
					this.Reset();
					base.SetisCurrentlyTag(true);
					return;
				}
				if (this.isCurrentlyTag && this.currentIt == null)
				{
					int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
				}
				else if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
				{
					this.Reset();
					int index2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index2], true);
				}
				else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
				{
					this.Reset();
					base.SetisCurrentlyTag(true);
					int index3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index3], false);
				}
				else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
				{
					int index4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index4], true);
				}
				bool flag = true;
				foreach (NetPlayer player in GameMode.ParticipatingPlayers)
				{
					if (!this.IsFrozen(player) && !base.IsInfected(player))
					{
						flag = false;
						break;
					}
				}
				if (flag && !this.isCurrentlyTag)
				{
					this.InfectionRoundEnd();
				}
			}
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x0017FEB4 File Offset: 0x0017E0B4
		public override void Tick()
		{
			base.Tick();
			if (this.localVRRig)
			{
				this.localVRRig.IsFrozen = this.IsFrozen(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x0017FEE4 File Offset: 0x0017E0E4
		public override void StartPlaying()
		{
			base.StartPlaying();
			this.localVRRig = this.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (NetPlayer netPlayer in this.lastRoundInfectedPlayers.ToArray())
				{
					if (netPlayer != null && !netPlayer.InRoom)
					{
						this.lastRoundInfectedPlayers.Remove(netPlayer);
					}
				}
				foreach (NetPlayer netPlayer2 in this.currentRoundInfectedPlayers.ToArray())
				{
					if (netPlayer2 != null && !netPlayer2.InRoom)
					{
						this.currentRoundInfectedPlayers.Remove(netPlayer2);
					}
				}
			}
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x0017FF88 File Offset: 0x0017E188
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
				Debug.LogWarning("Report TAG - tagged " + this.taggedRig.playerNameVisible + ", tagging " + this.taggingRig.playerNameVisible);
				if (this.isCurrentlyTag)
				{
					if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
					{
						base.AddLastTagged(taggedPlayer, taggingPlayer);
						this.ChangeCurrentIt(taggedPlayer, false);
						this.lastTag = (double)Time.time;
						return;
					}
				}
				else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && !this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.AddFrozenPlayer(taggedPlayer);
					return;
				}
				else if (!this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					this.UnfreezePlayer(taggedPlayer);
				}
			}
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x001801B8 File Offset: 0x0017E3B8
		public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
		{
			if (this.isCurrentlyTag)
			{
				return myPlayer == this.currentIt && myPlayer != otherPlayer;
			}
			return (this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(otherPlayer) && !this.currentInfected.Contains(otherPlayer)) || (!this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(myPlayer) && (this.currentInfected.Contains(otherPlayer) || this.currentFrozen.ContainsKey(otherPlayer)));
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x00180247 File Offset: 0x0017E447
		public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GameMode.RefreshPlayers();
				if (!this.isCurrentlyTag && !base.IsInfected(player))
				{
					this.AddInfectedPlayer(player, true);
					this.currentRoundInfectedPlayers.Add(player);
				}
				this.UpdateInfectionState();
			}
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x00180285 File Offset: 0x0017E485
		protected override IEnumerator InfectionRoundEndingCoroutine()
		{
			while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.currentFrozen.Clear();
				GameMode.RefreshPlayers();
				this.lastRoundInfectedPlayers.Clear();
				this.lastRoundInfectedPlayers.AddRange(this.currentRoundInfectedPlayers);
				this.currentRoundInfectedPlayers.Clear();
				List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
				int num = 0;
				if (participatingPlayers.Count > 0 && participatingPlayers.Count < this.infectMorePlayerLowerThreshold)
				{
					num = 1;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerLowerThreshold && participatingPlayers.Count < this.infectMorePlayerUpperThreshold)
				{
					num = 2;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerUpperThreshold)
				{
					num = 3;
				}
				for (int i = 0; i < num; i++)
				{
					this.TryAddNewInfectedPlayer();
				}
				this.lastTag = (double)Time.time;
			}
			yield return null;
			yield break;
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x00180294 File Offset: 0x0017E494
		public override void Reset()
		{
			base.Reset();
			this.currentFrozen.Clear();
			this.currentRoundInfectedPlayers.Clear();
			this.lastRoundInfectedPlayers.Clear();
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x000F2DD6 File Offset: 0x000F0FD6
		private new void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
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

		// Token: 0x06004D3D RID: 19773 RVA: 0x001802C0 File Offset: 0x0017E4C0
		private void TryAddNewInfectedPlayer()
		{
			List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
			int index = Random.Range(0, participatingPlayers.Count);
			int num = 0;
			while (num < 10 && this.lastRoundInfectedPlayers.Contains(participatingPlayers[index]))
			{
				index = Random.Range(0, participatingPlayers.Count);
				num++;
			}
			this.AddInfectedPlayer(participatingPlayers[index], true);
			this.currentRoundInfectedPlayers.Add(participatingPlayers[index]);
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x0018032E File Offset: 0x0017E52E
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (this.isCurrentlyTag && forPlayer == this.currentIt)
			{
				return 14;
			}
			if (this.currentInfected.Contains(forPlayer))
			{
				return 14;
			}
			return 0;
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x00180358 File Offset: 0x0017E558
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			NetPlayer netPlayer = rig.isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : rig.creator;
			rig.UpdateFrozenEffect(this.IsFrozen(netPlayer));
			int materialIndex = this.MyMatIndex(netPlayer);
			rig.ChangeMaterialLocal(materialIndex);
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x0018039C File Offset: 0x0017E59C
		private void UnfreezePlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Remove(taggedPlayer);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.UnTagged, taggedPlayer);
				RoomSystem.SendSoundEffectAll(10, 0.25f, true);
			}
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x001803DC File Offset: 0x0017E5DC
		private void AddFrozenPlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && !this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Add(taggedPlayer, Time.time);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.FrozenTime, taggedPlayer);
				RoomSystem.SendSoundEffectAll(9, 0.25f, false);
				RoomSystem.SendSoundEffectToPlayer(12, 0.05f, taggedPlayer, false);
			}
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x00180436 File Offset: 0x0017E636
		public bool IsFrozen(NetPlayer player)
		{
			return this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x00180444 File Offset: 0x0017E644
		public override float[] LocalPlayerSpeed()
		{
			this.fastJumpLimit = this.fastJumpLimitCached;
			this.fastJumpMultiplier = this.fastJumpMultiplierCached;
			this.slowJumpLimit = this.slowJumpLimitCached;
			this.slowJumpMultiplier = this.slowJumpMultiplierCached;
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
				if (!this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer) && !this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.playerSpeed[0] = base.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
					this.playerSpeed[1] = base.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
					return this.playerSpeed;
				}
				if (this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.fastJumpLimit = this.frozenPlayerFastJumpLimit;
					this.fastJumpMultiplier = this.frozenPlayerFastJumpMultiplier;
					this.slowJumpLimit = this.frozenPlayerSlowJumpLimit;
					this.slowJumpMultiplier = this.frozenPlayerSlowJumpMultiplier;
				}
				this.playerSpeed[0] = base.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = base.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
		}

		// Token: 0x06004D44 RID: 19780 RVA: 0x001805C8 File Offset: 0x0017E7C8
		public int GetFrozenHandTapAudioIndex()
		{
			int num = Random.Range(0, this.frozenHandTapIndices.Length);
			return this.frozenHandTapIndices[num];
		}

		// Token: 0x06004D45 RID: 19781 RVA: 0x001805EC File Offset: 0x0017E7EC
		public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber) && GameMode.ParticipatingPlayers.Count > 0)
				{
					int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
				}
				if (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				if (this.currentFrozen.ContainsKey(otherPlayer))
				{
					this.currentFrozen.Remove(otherPlayer);
				}
				this.UpdateState();
			}
		}

		// Token: 0x06004D46 RID: 19782 RVA: 0x001806A4 File Offset: 0x0017E8A4
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ForceResetFrozenEffect();
			}
		}

		// Token: 0x06004D47 RID: 19783 RVA: 0x00180700 File Offset: 0x0017E900
		public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeRead(stream, info);
			this.currentFrozen.Clear();
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int playerID = (int)stream.ReceiveNext();
				float value = (float)stream.ReceiveNext();
				NetPlayer player = NetworkSystem.Instance.GetPlayer(playerID);
				this.currentFrozen.Add(player, value);
			}
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x0018076C File Offset: 0x0017E96C
		public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeWrite(stream, info);
			stream.SendNext(this.currentFrozen.Count);
			foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen)
			{
				stream.SendNext(keyValuePair.Key.ActorNumber);
				stream.SendNext(keyValuePair.Value);
			}
		}

		// Token: 0x0400563C RID: 22076
		public Dictionary<NetPlayer, float> currentFrozen = new Dictionary<NetPlayer, float>(10);

		// Token: 0x0400563D RID: 22077
		public float freezeDuration;

		// Token: 0x0400563E RID: 22078
		public int infectMorePlayerLowerThreshold = 6;

		// Token: 0x0400563F RID: 22079
		public int infectMorePlayerUpperThreshold = 10;

		// Token: 0x04005640 RID: 22080
		[Space]
		[Header("Frozen player jump settings")]
		public float frozenPlayerFastJumpLimit;

		// Token: 0x04005641 RID: 22081
		public float frozenPlayerFastJumpMultiplier;

		// Token: 0x04005642 RID: 22082
		public float frozenPlayerSlowJumpLimit;

		// Token: 0x04005643 RID: 22083
		public float frozenPlayerSlowJumpMultiplier;

		// Token: 0x04005644 RID: 22084
		[GorillaSoundLookup]
		public int[] frozenHandTapIndices;

		// Token: 0x04005645 RID: 22085
		private float fastJumpLimitCached;

		// Token: 0x04005646 RID: 22086
		private float fastJumpMultiplierCached;

		// Token: 0x04005647 RID: 22087
		private float slowJumpLimitCached;

		// Token: 0x04005648 RID: 22088
		private float slowJumpMultiplierCached;

		// Token: 0x04005649 RID: 22089
		private VRRig localVRRig;

		// Token: 0x0400564A RID: 22090
		private int hapticStrength;

		// Token: 0x0400564B RID: 22091
		private List<NetPlayer> currentRoundInfectedPlayers = new List<NetPlayer>(10);

		// Token: 0x0400564C RID: 22092
		private List<NetPlayer> lastRoundInfectedPlayers = new List<NetPlayer>(10);
	}
}
