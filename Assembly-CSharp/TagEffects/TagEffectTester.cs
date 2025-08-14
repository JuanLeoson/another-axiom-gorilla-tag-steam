using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000E06 RID: 3590
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x060058D9 RID: 22745 RVA: 0x001B9A4B File Offset: 0x001B7C4B
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x060058DA RID: 22746 RVA: 0x001B9A53 File Offset: 0x001B7C53
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x060058DB RID: 22747 RVA: 0x001B9A5B File Offset: 0x001B7C5B
		public Transform Transform { get; }

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x060058DC RID: 22748 RVA: 0x00058615 File Offset: 0x00056815
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x060058DD RID: 22749 RVA: 0x001B9A63 File Offset: 0x001B7C63
		public bool FingersDown { get; }

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060058DE RID: 22750 RVA: 0x001B9A6B File Offset: 0x001B7C6B
		public bool FingersUp { get; }

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060058DF RID: 22751 RVA: 0x001B9A73 File Offset: 0x001B7C73
		public Vector3 Velocity { get; }

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060058E0 RID: 22752 RVA: 0x001B9A7B File Offset: 0x001B7C7B
		public bool RightHand { get; }

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060058E1 RID: 22753 RVA: 0x001B9A83 File Offset: 0x001B7C83
		public float Magnitude { get; }

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x060058E2 RID: 22754 RVA: 0x001B9A8B File Offset: 0x001B7C8B
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x060058E3 RID: 22755 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x040062A4 RID: 25252
		[SerializeField]
		private bool isStatic = true;
	}
}
