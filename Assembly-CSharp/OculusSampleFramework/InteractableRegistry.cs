using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D14 RID: 3348
	public class InteractableRegistry : MonoBehaviour
	{
		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x060052CA RID: 21194 RVA: 0x0019AC54 File Offset: 0x00198E54
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		// Token: 0x060052CB RID: 21195 RVA: 0x0019AC5B File Offset: 0x00198E5B
		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		// Token: 0x060052CC RID: 21196 RVA: 0x0019AC69 File Offset: 0x00198E69
		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		// Token: 0x04005C2F RID: 23599
		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
