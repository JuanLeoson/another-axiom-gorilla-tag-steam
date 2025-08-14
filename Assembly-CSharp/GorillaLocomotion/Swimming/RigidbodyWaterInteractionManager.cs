using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E13 RID: 3603
	public class RigidbodyWaterInteractionManager : MonoBehaviour
	{
		// Token: 0x0600598A RID: 22922 RVA: 0x001C218A File Offset: 0x001C038A
		protected void Awake()
		{
			if (RigidbodyWaterInteractionManager.hasInstance && RigidbodyWaterInteractionManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			RigidbodyWaterInteractionManager.SetInstance(this);
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x001C21AD File Offset: 0x001C03AD
		public static void CreateManager()
		{
			RigidbodyWaterInteractionManager.SetInstance(new GameObject("RigidbodyWaterInteractionManager").AddComponent<RigidbodyWaterInteractionManager>());
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x001C21C3 File Offset: 0x001C03C3
		private static void SetInstance(RigidbodyWaterInteractionManager manager)
		{
			RigidbodyWaterInteractionManager.instance = manager;
			RigidbodyWaterInteractionManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x0600598D RID: 22925 RVA: 0x001C21DE File Offset: 0x001C03DE
		public static void RegisterRBWI(RigidbodyWaterInteraction rbWI)
		{
			if (!RigidbodyWaterInteractionManager.hasInstance)
			{
				RigidbodyWaterInteractionManager.CreateManager();
			}
			if (!RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
			{
				RigidbodyWaterInteractionManager.allrBWI.Add(rbWI);
			}
		}

		// Token: 0x0600598E RID: 22926 RVA: 0x001C2204 File Offset: 0x001C0404
		public static void UnregisterRBWI(RigidbodyWaterInteraction rbWI)
		{
			if (!RigidbodyWaterInteractionManager.hasInstance)
			{
				RigidbodyWaterInteractionManager.CreateManager();
			}
			if (RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
			{
				RigidbodyWaterInteractionManager.allrBWI.Remove(rbWI);
			}
		}

		// Token: 0x0600598F RID: 22927 RVA: 0x001C222C File Offset: 0x001C042C
		public void FixedUpdate()
		{
			for (int i = 0; i < RigidbodyWaterInteractionManager.allrBWI.Count; i++)
			{
				RigidbodyWaterInteractionManager.allrBWI[i].InvokeFixedUpdate();
			}
		}

		// Token: 0x0400641F RID: 25631
		public static RigidbodyWaterInteractionManager instance;

		// Token: 0x04006420 RID: 25632
		[OnEnterPlay_Set(false)]
		public static bool hasInstance = false;

		// Token: 0x04006421 RID: 25633
		public static List<RigidbodyWaterInteraction> allrBWI = new List<RigidbodyWaterInteraction>();
	}
}
