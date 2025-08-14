using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B3 RID: 179
public class SteeringWheelCosmetic : MonoBehaviour
{
	// Token: 0x06000465 RID: 1125 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x000199D1 File Offset: 0x00017BD1
	public void TryHornHit()
	{
		if (Time.time > this.lastHornTime + this.cooldown)
		{
			this.lastHornTime = Time.time;
			UnityEvent unityEvent = this.onHornHit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00019A04 File Offset: 0x00017C04
	private void Update()
	{
		float z = base.transform.localEulerAngles.z;
		if (Mathf.Abs(Mathf.DeltaAngle(this.lastZAngle, z)) >= this.dramaticTurnThreshold)
		{
			UnityEvent unityEvent = this.onDramaticTurn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.lastZAngle = z;
	}

	// Token: 0x0400051A RID: 1306
	[SerializeField]
	private float cooldown = 1.5f;

	// Token: 0x0400051B RID: 1307
	[SerializeField]
	private float dramaticTurnThreshold = 35f;

	// Token: 0x0400051C RID: 1308
	[SerializeField]
	private UnityEvent onHornHit;

	// Token: 0x0400051D RID: 1309
	[SerializeField]
	private UnityEvent onDramaticTurn;

	// Token: 0x0400051E RID: 1310
	private float lastHornTime = -999f;

	// Token: 0x0400051F RID: 1311
	private float lastZAngle;
}
