using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001C7 RID: 455
public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000B50 RID: 2896 RVA: 0x0001D447 File Offset: 0x0001B647
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0001D44F File Offset: 0x0001B64F
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0003C248 File Offset: 0x0003A448
	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000B53 RID: 2899 RVA: 0x0003C297 File Offset: 0x0003A497
	// (set) Token: 0x06000B54 RID: 2900 RVA: 0x0003C29F File Offset: 0x0003A49F
	public bool TickRunning { get; set; }

	// Token: 0x04000DEA RID: 3562
	[SerializeField]
	private float speedThreshold;

	// Token: 0x04000DEB RID: 3563
	[SerializeField]
	private UnityEvent onFaster;

	// Token: 0x04000DEC RID: 3564
	[SerializeField]
	private UnityEvent onSlower;

	// Token: 0x04000DED RID: 3565
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000DEE RID: 3566
	private bool wasFaster;
}
