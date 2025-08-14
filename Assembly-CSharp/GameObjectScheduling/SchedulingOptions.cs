using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F9E RID: 3998
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x060063E3 RID: 25571 RVA: 0x001F6C21 File Offset: 0x001F4E21
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04006EBC RID: 28348
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04006EBD RID: 28349
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04006EBE RID: 28350
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
