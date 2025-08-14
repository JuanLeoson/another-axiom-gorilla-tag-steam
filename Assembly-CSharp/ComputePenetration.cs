using System;
using Drawing;
using UnityEngine;

// Token: 0x02000860 RID: 2144
public class ComputePenetration : MonoBehaviour
{
	// Token: 0x060035F4 RID: 13812 RVA: 0x0011BA7B File Offset: 0x00119C7B
	public void Compute()
	{
		if (this.colliderA == null)
		{
			return;
		}
		this.colliderB == null;
	}

	// Token: 0x060035F5 RID: 13813 RVA: 0x0011BA9C File Offset: 0x00119C9C
	public void OnDrawGizmos()
	{
		if (this.colliderA.AsNull<Collider>() == null)
		{
			return;
		}
		if (this.colliderB.AsNull<Collider>() == null)
		{
			return;
		}
		Transform transform = this.colliderA.transform;
		Transform transform2 = this.colliderB.transform;
		if (this.lastUpdate.HasElapsed(0.5f, true))
		{
			this.overlapped = Physics.ComputePenetration(this.colliderA, transform.position, transform.rotation, this.colliderB, transform2.position, transform2.rotation, out this.direction, out this.distance);
		}
		Color color = this.overlapped ? Color.red : Color.green;
		this.DrawCollider(this.colliderA, color);
		this.DrawCollider(this.colliderB, color);
		if (this.overlapped)
		{
			Vector3 position = this.colliderB.transform.position;
			Vector3 to = position + this.direction * this.distance;
			Gizmos.DrawLine(position, to);
		}
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x0011BB9C File Offset: 0x00119D9C
	private unsafe void DrawCollider(Collider c, Color color)
	{
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.PopColor();
		}
	}

	// Token: 0x040042DF RID: 17119
	public Collider colliderA;

	// Token: 0x040042E0 RID: 17120
	public Collider colliderB;

	// Token: 0x040042E1 RID: 17121
	public bool overlapped;

	// Token: 0x040042E2 RID: 17122
	public Vector3 direction;

	// Token: 0x040042E3 RID: 17123
	public float distance;

	// Token: 0x040042E4 RID: 17124
	private TimeSince lastUpdate = TimeSince.Now();
}
