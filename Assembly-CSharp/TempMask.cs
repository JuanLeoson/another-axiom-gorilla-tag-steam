using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class TempMask : MonoBehaviour
{
	// Token: 0x060004FD RID: 1277 RVA: 0x0001D1FC File Offset: 0x0001B3FC
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.netView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001D26A File Offset: 0x0001B46A
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00004F01 File Offset: 0x00003101
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001D279 File Offset: 0x0001B479
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x040005FD RID: 1533
	public int year;

	// Token: 0x040005FE RID: 1534
	public int month;

	// Token: 0x040005FF RID: 1535
	public int day;

	// Token: 0x04000600 RID: 1536
	public DateTime dayOn;

	// Token: 0x04000601 RID: 1537
	public MeshRenderer myRenderer;

	// Token: 0x04000602 RID: 1538
	private DateTime myDate;

	// Token: 0x04000603 RID: 1539
	private VRRig myRig;
}
