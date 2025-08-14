using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000E9E RID: 3742
	[RequireComponent(typeof(AudioSource))]
	[NetworkBehaviourWeaved(2)]
	public class SportScoreboard : NetworkComponent
	{
		// Token: 0x06005D98 RID: 23960 RVA: 0x001D82D0 File Offset: 0x001D64D0
		protected override void Awake()
		{
			base.Awake();
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x001D8339 File Offset: 0x001D6539
		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		// Token: 0x06005D9A RID: 23962 RVA: 0x001D834C File Offset: 0x001D654C
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
				}
			}
		}

		// Token: 0x06005D9B RID: 23963 RVA: 0x001D83E8 File Offset: 0x001D65E8
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.GTPlayOneShot(this.teamParameters[i].goalScoredAudio, 1f);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x06005D9C RID: 23964 RVA: 0x001D84DC File Offset: 0x001D66DC
		public void TeamScored(int team)
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x06005D9D RID: 23965 RVA: 0x001D852C File Offset: 0x001D672C
		public void ResetScores()
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x001D8572 File Offset: 0x001D6772
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio, 1f);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06005D9F RID: 23967 RVA: 0x001D8588 File Offset: 0x001D6788
		[Networked]
		[Capacity(2)]
		[NetworkedWeaved(0, 2)]
		public unsafe NetworkArray<int> Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<int>((byte*)(this.Ptr + 0), 2, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x06005DA0 RID: 23968 RVA: 0x001D85C4 File Offset: 0x001D67C4
		public override void WriteDataFusion()
		{
			this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
		}

		// Token: 0x06005DA1 RID: 23969 RVA: 0x001D85F4 File Offset: 0x001D67F4
		public override void ReadDataFusion()
		{
			this.teamScores.Clear();
			this.Data.CopyTo(this.teamScores);
			this.OnScoreUpdated();
		}

		// Token: 0x06005DA2 RID: 23970 RVA: 0x001D8628 File Offset: 0x001D6828
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				stream.SendNext(this.teamScores[i]);
			}
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x001D8664 File Offset: 0x001D6864
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				this.teamScores[i] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x001D86DF File Offset: 0x001D68DF
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
		}

		// Token: 0x06005DA6 RID: 23974 RVA: 0x001D8701 File Offset: 0x001D6901
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
		}

		// Token: 0x04006769 RID: 26473
		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

		// Token: 0x0400676A RID: 26474
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x0400676B RID: 26475
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x0400676C RID: 26476
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x0400676D RID: 26477
		private List<int> teamScores = new List<int>();

		// Token: 0x0400676E RID: 26478
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x0400676F RID: 26479
		private bool runningMatchEndCoroutine;

		// Token: 0x04006770 RID: 26480
		private AudioSource audioSource;

		// Token: 0x04006771 RID: 26481
		private SportScoreboardVisuals[] scoreVisuals;

		// Token: 0x04006772 RID: 26482
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 2)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int[] _Data;

		// Token: 0x02000E9F RID: 3743
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x04006773 RID: 26483
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x04006774 RID: 26484
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
