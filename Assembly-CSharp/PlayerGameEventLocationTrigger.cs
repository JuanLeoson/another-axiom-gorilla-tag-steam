using System;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class PlayerGameEventLocationTrigger : MonoBehaviour
{
	// Token: 0x06000813 RID: 2067 RVA: 0x0002D1DA File Offset: 0x0002B3DA
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			PlayerGameEvents.TriggerEnterLocation(this.locationName);
		}
	}

	// Token: 0x040009B9 RID: 2489
	[SerializeField]
	private string locationName;
}
