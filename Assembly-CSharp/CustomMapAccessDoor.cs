using System;
using UnityEngine;

// Token: 0x02000807 RID: 2055
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x06003372 RID: 13170 RVA: 0x0010B7CE File Offset: 0x001099CE
	public void OpenDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(true);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(false);
		}
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x0010B804 File Offset: 0x00109A04
	public void CloseDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(false);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(true);
		}
	}

	// Token: 0x04004068 RID: 16488
	public GameObject openDoorObject;

	// Token: 0x04004069 RID: 16489
	public GameObject closedDoorObject;
}
