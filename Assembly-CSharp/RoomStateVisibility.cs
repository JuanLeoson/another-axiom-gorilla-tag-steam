using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class RoomStateVisibility : MonoBehaviour
{
	// Token: 0x06000DAB RID: 3499 RVA: 0x00053D2B File Offset: 0x00051F2B
	private void Start()
	{
		this.OnRoomChanged();
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent += new Action(this.OnRoomChanged);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00053D69 File Offset: 0x00051F69
	private void OnDestroy()
	{
		RoomSystem.JoinedRoomEvent -= new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent -= new Action(this.OnRoomChanged);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00053DA4 File Offset: 0x00051FA4
	private void OnRoomChanged()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			base.gameObject.SetActive(this.enableOutOfRoom);
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			base.gameObject.SetActive(this.enableInPrivateRoom);
			return;
		}
		base.gameObject.SetActive(this.enableInRoom);
	}

	// Token: 0x0400157D RID: 5501
	[SerializeField]
	private bool enableOutOfRoom;

	// Token: 0x0400157E RID: 5502
	[SerializeField]
	private bool enableInRoom = true;

	// Token: 0x0400157F RID: 5503
	[SerializeField]
	private bool enableInPrivateRoom = true;
}
