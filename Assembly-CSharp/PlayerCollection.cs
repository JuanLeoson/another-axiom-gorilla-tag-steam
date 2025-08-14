using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020003C2 RID: 962
public class PlayerCollection : MonoBehaviour
{
	// Token: 0x0600163F RID: 5695 RVA: 0x00078F2C File Offset: 0x0007712C
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x00078F4F File Offset: 0x0007714F
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x00078F74 File Offset: 0x00077174
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

	// Token: 0x06001642 RID: 5698 RVA: 0x00078FC4 File Offset: 0x000771C4
	public void OnTriggerExit(Collider other)
	{
		SphereCollider component = other.GetComponent<SphereCollider>();
		if (!component)
		{
			return;
		}
		VRRig component2 = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component2 == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component2))
		{
			Collider[] components = base.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(components[i], base.transform.position, base.transform.rotation, component, component.transform.position, component.transform.rotation, out vector, out num))
				{
					return;
				}
			}
			this.containedRigs.Remove(component2);
		}
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x00079068 File Offset: 0x00077268
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => r.creator == null || r.creator == otherPlayer);
	}

	// Token: 0x04001E01 RID: 7681
	[DebugReadout]
	[NonSerialized]
	public readonly List<VRRig> containedRigs = new List<VRRig>(10);
}
