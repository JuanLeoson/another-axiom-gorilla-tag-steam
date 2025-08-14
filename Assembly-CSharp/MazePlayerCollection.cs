using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class MazePlayerCollection : MonoBehaviour
{
	// Token: 0x06000498 RID: 1176 RVA: 0x0001A91C File Offset: 0x00018B1C
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001A93F File Offset: 0x00018B3F
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001A964 File Offset: 0x00018B64
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001A9B4 File Offset: 0x00018BB4
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001AA08 File Offset: 0x00018C08
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creator : null) == null || r.creator == otherPlayer);
	}

	// Token: 0x04000567 RID: 1383
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x04000568 RID: 1384
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
