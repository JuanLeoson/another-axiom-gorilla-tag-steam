using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BF9 RID: 3065
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06004A8B RID: 19083 RVA: 0x00169FEF File Offset: 0x001681EF
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06004A8C RID: 19084 RVA: 0x0016A008 File Offset: 0x00168208
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06004A8D RID: 19085 RVA: 0x0016A022 File Offset: 0x00168222
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x04005374 RID: 21364
		[SerializeField]
		private int uniqueId = -1;
	}
}
