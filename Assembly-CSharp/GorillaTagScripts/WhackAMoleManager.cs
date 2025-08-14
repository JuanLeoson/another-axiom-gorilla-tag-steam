using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BF6 RID: 3062
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06004A6C RID: 19052 RVA: 0x00169C2A File Offset: 0x00167E2A
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x000172AD File Offset: 0x000154AD
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x000172B6 File Offset: 0x000154B6
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004A6F RID: 19055 RVA: 0x00169C40 File Offset: 0x00167E40
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x06004A70 RID: 19056 RVA: 0x00169C90 File Offset: 0x00167E90
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x00169C98 File Offset: 0x00167E98
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x06004A72 RID: 19058 RVA: 0x00169CA7 File Offset: 0x00167EA7
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x04005366 RID: 21350
		public static WhackAMoleManager instance;

		// Token: 0x04005367 RID: 21351
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
