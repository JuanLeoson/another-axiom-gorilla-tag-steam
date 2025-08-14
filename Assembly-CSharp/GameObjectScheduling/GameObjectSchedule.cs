using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F97 RID: 3991
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x060063C3 RID: 25539 RVA: 0x001F662A File Offset: 0x001F482A
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x060063C4 RID: 25540 RVA: 0x001F6632 File Offset: 0x001F4832
		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		// Token: 0x060063C5 RID: 25541 RVA: 0x001F663C File Offset: 0x001F483C
		public int GetCurrentNodeIndex(DateTime currentDate, int startFrom = 0)
		{
			if (startFrom >= this.nodes.Length)
			{
				return int.MaxValue;
			}
			for (int i = -1; i < this.nodes.Length - 1; i++)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					return i;
				}
			}
			return int.MaxValue;
		}

		// Token: 0x060063C6 RID: 25542 RVA: 0x001F668D File Offset: 0x001F488D
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		// Token: 0x060063C7 RID: 25543 RVA: 0x001F66A8 File Offset: 0x001F48A8
		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		// Token: 0x060063C8 RID: 25544 RVA: 0x001F6714 File Offset: 0x001F4914
		public static void GenerateDailyShuffle(DateTime startDate, DateTime endDate, GameObjectSchedule[] schedules)
		{
			TimeSpan t = TimeSpan.FromDays(1.0);
			int num = schedules.Length - 1;
			int num2 = schedules.Length - 2;
			DateTime dateTime = startDate;
			List<GameObjectSchedule.GameObjectScheduleNode>[] array = new List<GameObjectSchedule.GameObjectScheduleNode>[schedules.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<GameObjectSchedule.GameObjectScheduleNode>();
			}
			while (dateTime < endDate)
			{
				int num3 = Random.Range(0, schedules.Length - 2);
				if (num <= num3)
				{
					num3++;
					if (num2 <= num3)
					{
						num3++;
					}
				}
				else if (num2 <= num3)
				{
					num3++;
					if (num <= num3)
					{
						num3++;
					}
				}
				array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = false
				});
				array[num3].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = true
				});
				dateTime += t;
				num2 = num;
				num = num3;
			}
			array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
			{
				activeDateTime = dateTime.ToString(),
				activeState = false
			});
			for (int j = 0; j < array.Length; j++)
			{
				schedules[j].nodes = array[j].ToArray();
			}
		}

		// Token: 0x04006EA6 RID: 28326
		[SerializeField]
		private bool initialState;

		// Token: 0x04006EA7 RID: 28327
		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		// Token: 0x04006EA8 RID: 28328
		[SerializeField]
		private SchedulingOptions options;

		// Token: 0x04006EA9 RID: 28329
		private bool validated;

		// Token: 0x02000F98 RID: 3992
		[Serializable]
		public class GameObjectScheduleNode
		{
			// Token: 0x17000971 RID: 2417
			// (get) Token: 0x060063CA RID: 25546 RVA: 0x001F684B File Offset: 0x001F4A4B
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			// Token: 0x17000972 RID: 2418
			// (get) Token: 0x060063CB RID: 25547 RVA: 0x001F6853 File Offset: 0x001F4A53
			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			// Token: 0x060063CC RID: 25548 RVA: 0x001F685C File Offset: 0x001F4A5C
			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			// Token: 0x04006EAA RID: 28330
			[SerializeField]
			public string activeDateTime = "1/1/0001 00:00:00";

			// Token: 0x04006EAB RID: 28331
			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			public bool activeState = true;

			// Token: 0x04006EAC RID: 28332
			private DateTime dateTime;
		}
	}
}
