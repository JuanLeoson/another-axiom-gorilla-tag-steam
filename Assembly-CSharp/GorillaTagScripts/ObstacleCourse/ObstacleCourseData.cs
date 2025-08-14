using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C79 RID: 3193
	[NetworkStructWeaved(9)]
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct ObstacleCourseData : INetworkStruct
	{
		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06004F08 RID: 20232 RVA: 0x001893A9 File Offset: 0x001875A9
		// (set) Token: 0x06004F09 RID: 20233 RVA: 0x001893B1 File Offset: 0x001875B1
		public int ObstacleCourseCount { readonly get; set; }

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06004F0A RID: 20234 RVA: 0x001893BC File Offset: 0x001875BC
		[Networked]
		[Capacity(4)]
		public NetworkArray<int> WinnerActorNumber
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._WinnerActorNumber), 4, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06004F0B RID: 20235 RVA: 0x001893E0 File Offset: 0x001875E0
		[Networked]
		[Capacity(4)]
		public NetworkArray<int> CurrentRaceState
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._CurrentRaceState), 4, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x00189404 File Offset: 0x00187604
		public ObstacleCourseData(List<ObstacleCourse> courses)
		{
			this.ObstacleCourseCount = courses.Count;
			int[] array = new int[this.ObstacleCourseCount];
			int[] array2 = new int[this.ObstacleCourseCount];
			for (int i = 0; i < courses.Count; i++)
			{
				array[i] = courses[i].winnerActorNumber;
				array2[i] = (int)courses[i].currentState;
			}
			this.WinnerActorNumber.CopyFrom(array, 0, this.ObstacleCourseCount);
			this.CurrentRaceState.CopyFrom(array2, 0, this.ObstacleCourseCount);
		}

		// Token: 0x04005808 RID: 22536
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@4 _WinnerActorNumber;

		// Token: 0x04005809 RID: 22537
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(20)]
		private FixedStorage@4 _CurrentRaceState;
	}
}
