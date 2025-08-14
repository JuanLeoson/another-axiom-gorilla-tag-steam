using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000F8D RID: 3981
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x06006395 RID: 25493 RVA: 0x001F5E64 File Offset: 0x001F4064
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody)
			{
				CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
				if (component.IsNotNull())
				{
					component.gameObject.SetActive(false);
				}
			}
		}
	}
}
