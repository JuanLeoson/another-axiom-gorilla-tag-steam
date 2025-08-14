using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C78 RID: 3192
	[NetworkBehaviourWeaved(9)]
	public class ObstacleCourseManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06004EF6 RID: 20214 RVA: 0x001890D6 File Offset: 0x001872D6
		// (set) Token: 0x06004EF7 RID: 20215 RVA: 0x001890DD File Offset: 0x001872DD
		public static ObstacleCourseManager Instance { get; private set; }

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06004EF8 RID: 20216 RVA: 0x001890E5 File Offset: 0x001872E5
		// (set) Token: 0x06004EF9 RID: 20217 RVA: 0x001890ED File Offset: 0x001872ED
		public bool TickRunning { get; set; }

		// Token: 0x06004EFA RID: 20218 RVA: 0x001890F6 File Offset: 0x001872F6
		protected override void Awake()
		{
			base.Awake();
			ObstacleCourseManager.Instance = this;
		}

		// Token: 0x06004EFB RID: 20219 RVA: 0x00189104 File Offset: 0x00187304
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06004EFC RID: 20220 RVA: 0x00189118 File Offset: 0x00187318
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06004EFD RID: 20221 RVA: 0x0018912C File Offset: 0x0018732C
		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		// Token: 0x06004EFE RID: 20222 RVA: 0x0018917C File Offset: 0x0018737C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			this.allObstaclesCourses.Clear();
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06004EFF RID: 20223 RVA: 0x0018918F File Offset: 0x0018738F
		// (set) Token: 0x06004F00 RID: 20224 RVA: 0x001891B9 File Offset: 0x001873B9
		[Networked]
		[NetworkedWeaved(0, 9)]
		public unsafe ObstacleCourseData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ObstacleCourseData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ObstacleCourseData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x001891E4 File Offset: 0x001873E4
		public override void WriteDataFusion()
		{
			this.Data = new ObstacleCourseData(this.allObstaclesCourses);
		}

		// Token: 0x06004F02 RID: 20226 RVA: 0x001891F8 File Offset: 0x001873F8
		public override void ReadDataFusion()
		{
			for (int i = 0; i < this.Data.ObstacleCourseCount; i++)
			{
				int winnerActorNumber = this.Data.WinnerActorNumber[i];
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)this.Data.CurrentRaceState[i];
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(winnerActorNumber, raceState);
				}
			}
		}

		// Token: 0x06004F03 RID: 20227 RVA: 0x00189278 File Offset: 0x00187478
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allObstaclesCourses.Count);
			for (int i = 0; i < this.allObstaclesCourses.Count; i++)
			{
				stream.SendNext(this.allObstaclesCourses[i].winnerActorNumber);
				stream.SendNext(this.allObstaclesCourses[i].currentState);
			}
		}

		// Token: 0x06004F04 RID: 20228 RVA: 0x001892F8 File Offset: 0x001874F8
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int winnerActorNumber = (int)stream.ReceiveNext();
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)stream.ReceiveNext();
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(winnerActorNumber, raceState);
				}
			}
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x0018937D File Offset: 0x0018757D
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x00189395 File Offset: 0x00187595
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04005804 RID: 22532
		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		// Token: 0x04005806 RID: 22534
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 9)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ObstacleCourseData _Data;
	}
}
