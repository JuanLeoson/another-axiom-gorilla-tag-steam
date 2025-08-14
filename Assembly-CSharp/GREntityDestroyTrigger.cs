using System;
using UnityEngine;

// Token: 0x0200063A RID: 1594
public class GREntityDestroyTrigger : MonoBehaviour
{
	// Token: 0x06002752 RID: 10066 RVA: 0x000D408C File Offset: 0x000D228C
	private void OnTriggerEnter(Collider other)
	{
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.IsAuthority())
		{
			component.manager.RequestDestroyItem(component.id);
		}
	}
}
