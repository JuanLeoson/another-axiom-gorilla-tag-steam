using System;
using UnityEngine;

// Token: 0x020003FD RID: 1021
public class RigDuplicationZone : MonoBehaviour
{
	// Token: 0x14000041 RID: 65
	// (add) Token: 0x060017E0 RID: 6112 RVA: 0x00080018 File Offset: 0x0007E218
	// (remove) Token: 0x060017E1 RID: 6113 RVA: 0x0008004C File Offset: 0x0007E24C
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x060017E2 RID: 6114 RVA: 0x0008007F File Offset: 0x0007E27F
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x00080087 File Offset: 0x0007E287
	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x000800AC File Offset: 0x0007E2AC
	private void OnDisable()
	{
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x000800BF File Offset: 0x0007E2BF
	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.setOtherZone(z);
		z.setOtherZone(this);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x000800ED File Offset: 0x0007E2ED
	private void setOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		this.offsetToOtherZone = z.transform.position - base.transform.position;
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x00080118 File Offset: 0x0007E318
	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = true;
			return;
		}
		component.SetDuplicationZone(this);
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x00080150 File Offset: 0x0007E350
	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = false;
			return;
		}
		component.ClearDuplicationZone(this);
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x060017E9 RID: 6121 RVA: 0x00080185 File Offset: 0x0007E385
	public Vector3 VisualOffsetForRigs
	{
		get
		{
			if (!this.otherZone.playerInZone)
			{
				return Vector3.zero;
			}
			return this.offsetToOtherZone;
		}
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x060017EA RID: 6122 RVA: 0x000801A0 File Offset: 0x0007E3A0
	public bool IsApplyingDisplacement
	{
		get
		{
			return this.otherZone.playerInZone;
		}
	}

	// Token: 0x04001FA6 RID: 8102
	private RigDuplicationZone otherZone;

	// Token: 0x04001FA7 RID: 8103
	[SerializeField]
	private string id;

	// Token: 0x04001FA8 RID: 8104
	private bool playerInZone;

	// Token: 0x04001FA9 RID: 8105
	private Vector3 offsetToOtherZone;

	// Token: 0x020003FE RID: 1022
	// (Invoke) Token: 0x060017ED RID: 6125
	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}
