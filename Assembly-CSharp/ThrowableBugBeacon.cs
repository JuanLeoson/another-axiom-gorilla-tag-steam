using System;
using UnityEngine;

// Token: 0x02000B32 RID: 2866
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x1400007D RID: 125
	// (add) Token: 0x060044E9 RID: 17641 RVA: 0x001586C4 File Offset: 0x001568C4
	// (remove) Token: 0x060044EA RID: 17642 RVA: 0x001586F8 File Offset: 0x001568F8
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x1400007E RID: 126
	// (add) Token: 0x060044EB RID: 17643 RVA: 0x0015872C File Offset: 0x0015692C
	// (remove) Token: 0x060044EC RID: 17644 RVA: 0x00158760 File Offset: 0x00156960
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x1400007F RID: 127
	// (add) Token: 0x060044ED RID: 17645 RVA: 0x00158794 File Offset: 0x00156994
	// (remove) Token: 0x060044EE RID: 17646 RVA: 0x001587C8 File Offset: 0x001569C8
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x060044EF RID: 17647 RVA: 0x001587FC File Offset: 0x001569FC
	// (remove) Token: 0x060044F0 RID: 17648 RVA: 0x00158830 File Offset: 0x00156A30
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x060044F1 RID: 17649 RVA: 0x00158864 File Offset: 0x00156A64
	// (remove) Token: 0x060044F2 RID: 17650 RVA: 0x00158898 File Offset: 0x00156A98
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x1700067D RID: 1661
	// (get) Token: 0x060044F3 RID: 17651 RVA: 0x001588CB File Offset: 0x00156ACB
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x1700067E RID: 1662
	// (get) Token: 0x060044F4 RID: 17652 RVA: 0x001588D3 File Offset: 0x00156AD3
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x060044F5 RID: 17653 RVA: 0x001588DB File Offset: 0x00156ADB
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x060044F6 RID: 17654 RVA: 0x001588EF File Offset: 0x00156AEF
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x060044F7 RID: 17655 RVA: 0x00158903 File Offset: 0x00156B03
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x060044F8 RID: 17656 RVA: 0x00158917 File Offset: 0x00156B17
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x060044F9 RID: 17657 RVA: 0x0015892B File Offset: 0x00156B2B
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x060044FA RID: 17658 RVA: 0x00158917 File Offset: 0x00156B17
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x04004F46 RID: 20294
	[SerializeField]
	private float range;

	// Token: 0x04004F47 RID: 20295
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000B33 RID: 2867
	// (Invoke) Token: 0x060044FD RID: 17661
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000B34 RID: 2868
	// (Invoke) Token: 0x06004501 RID: 17665
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}
