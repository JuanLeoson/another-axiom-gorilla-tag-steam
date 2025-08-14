using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000B35 RID: 2869
public class ThrowableBugBeaconActivation : MonoBehaviour
{
	// Token: 0x06004504 RID: 17668 RVA: 0x00158940 File Offset: 0x00156B40
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	// Token: 0x06004505 RID: 17669 RVA: 0x0015894E File Offset: 0x00156B4E
	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	// Token: 0x06004506 RID: 17670 RVA: 0x00004F01 File Offset: 0x00003101
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06004507 RID: 17671 RVA: 0x0015895D File Offset: 0x00156B5D
	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	// Token: 0x04004F48 RID: 20296
	[SerializeField]
	private float minCallTime = 1f;

	// Token: 0x04004F49 RID: 20297
	[SerializeField]
	private float maxCallTime = 5f;

	// Token: 0x04004F4A RID: 20298
	[SerializeField]
	private uint signalCount;

	// Token: 0x04004F4B RID: 20299
	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	// Token: 0x04004F4C RID: 20300
	private ThrowableBugBeacon tbb;

	// Token: 0x02000B36 RID: 2870
	private enum ActivationMode
	{
		// Token: 0x04004F4E RID: 20302
		CALL,
		// Token: 0x04004F4F RID: 20303
		DISMISS,
		// Token: 0x04004F50 RID: 20304
		LOCK
	}
}
