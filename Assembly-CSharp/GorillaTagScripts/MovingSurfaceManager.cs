using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BFA RID: 3066
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06004A8F RID: 19087 RVA: 0x0016A03C File Offset: 0x0016823C
		private void Awake()
		{
			if (MovingSurfaceManager.instance != null && MovingSurfaceManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of MovingSurfaceManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (MovingSurfaceManager.instance == null)
			{
				MovingSurfaceManager.instance = this;
			}
		}

		// Token: 0x06004A90 RID: 19088 RVA: 0x0016A088 File Offset: 0x00168288
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x06004A91 RID: 19089 RVA: 0x0016A09D File Offset: 0x0016829D
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x06004A92 RID: 19090 RVA: 0x0016A0B1 File Offset: 0x001682B1
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x0016A0D3 File Offset: 0x001682D3
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x0016A0E2 File Offset: 0x001682E2
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, out result) && result != null;
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x0016A100 File Offset: 0x00168300
		private void FixedUpdate()
		{
			foreach (SurfaceMover surfaceMover in this.surfaceMovers)
			{
				if (surfaceMover.isActiveAndEnabled)
				{
					surfaceMover.Move();
				}
			}
		}

		// Token: 0x04005375 RID: 21365
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04005376 RID: 21366
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04005377 RID: 21367
		public static MovingSurfaceManager instance;
	}
}
