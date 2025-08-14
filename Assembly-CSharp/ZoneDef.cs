using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000B5C RID: 2908
public class ZoneDef : MonoBehaviour
{
	// Token: 0x17000690 RID: 1680
	// (get) Token: 0x06004594 RID: 17812 RVA: 0x0015B894 File Offset: 0x00159A94
	public GroupJoinZoneAB groupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.groupZone,
				b = this.groupZoneB
			};
		}
	}

	// Token: 0x17000691 RID: 1681
	// (get) Token: 0x06004595 RID: 17813 RVA: 0x0015B8C4 File Offset: 0x00159AC4
	public GroupJoinZoneAB excludeGroupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.excludeGroupZone,
				b = this.excludeGroupZoneB
			};
		}
	}

	// Token: 0x04005095 RID: 20629
	public GTZone zoneId;

	// Token: 0x04005096 RID: 20630
	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	// Token: 0x04005097 RID: 20631
	public GroupJoinZoneA groupZone;

	// Token: 0x04005098 RID: 20632
	public GroupJoinZoneB groupZoneB;

	// Token: 0x04005099 RID: 20633
	public GroupJoinZoneA excludeGroupZone;

	// Token: 0x0400509A RID: 20634
	public GroupJoinZoneB excludeGroupZoneB;

	// Token: 0x0400509B RID: 20635
	[Space]
	public bool trackEnter = true;

	// Token: 0x0400509C RID: 20636
	public bool trackExit;

	// Token: 0x0400509D RID: 20637
	public bool trackStay = true;

	// Token: 0x0400509E RID: 20638
	public int priority = 1;

	// Token: 0x0400509F RID: 20639
	[Space]
	public BoxCollider[] colliders = new BoxCollider[0];

	// Token: 0x040050A0 RID: 20640
	[Space]
	public ZoneNode[] nodes = new ZoneNode[0];

	// Token: 0x040050A1 RID: 20641
	[Space]
	public Bounds bounds;

	// Token: 0x040050A2 RID: 20642
	[Space]
	public ZoneDef[] zoneOverlaps = new ZoneDef[0];
}
