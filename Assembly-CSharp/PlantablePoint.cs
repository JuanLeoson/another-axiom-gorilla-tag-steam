using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x06000CB2 RID: 3250 RVA: 0x0004456E File Offset: 0x0004276E
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(true);
		}
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0004459A File Offset: 0x0004279A
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(false);
		}
	}

	// Token: 0x04000FA4 RID: 4004
	public bool shouldBeSet;

	// Token: 0x04000FA5 RID: 4005
	public LayerMask floorMask;

	// Token: 0x04000FA6 RID: 4006
	public PlantableObject plantableObject;
}
