using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007BC RID: 1980
public class TappableBeeHive : Tappable
{
	// Token: 0x060031A9 RID: 12713 RVA: 0x00102998 File Offset: 0x00100B98
	private void Awake()
	{
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			Debug.LogError("TappableBeeHive: Disabling because swarmEmergePoint is null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		base.GetComponent<SlingshotProjectileHitNotifier>().OnProjectileHit += this.OnSlingshotHit;
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x001029FC File Offset: 0x00100BFC
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x00102A70 File Offset: 0x00100C70
	public void OnSlingshotHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x04003D5C RID: 15708
	[SerializeField]
	private GameObject swarmEmergeFromPoint;

	// Token: 0x04003D5D RID: 15709
	[SerializeField]
	private GameObject swarmEmergeToPoint;

	// Token: 0x04003D5E RID: 15710
	[SerializeField]
	private GameObject honeycombSurface;

	// Token: 0x04003D5F RID: 15711
	[SerializeField]
	private float honeycombDisableDuration;

	// Token: 0x04003D60 RID: 15712
	[NonSerialized]
	private TimeSince _timeSinceLastTap;

	// Token: 0x04003D61 RID: 15713
	private float reenableHoneycombAtTimestamp;

	// Token: 0x04003D62 RID: 15714
	private Coroutine reenableHoneycombCoroutine;
}
