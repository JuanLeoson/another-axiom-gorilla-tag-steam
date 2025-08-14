using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001B2 RID: 434
public class OnSqueezeTrigger : MonoBehaviour
{
	// Token: 0x06000AC2 RID: 2754 RVA: 0x00039D73 File Offset: 0x00037F73
	private void Start()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00039D84 File Offset: 0x00037F84
	private void Update()
	{
		bool flag;
		if (this.myHoldable.InLeftHand())
		{
			flag = ((this.indexFinger ? this.myRig.leftIndex.calcT : this.myRig.leftMiddle.calcT) > 0.5f);
		}
		else
		{
			flag = (this.myHoldable.InRightHand() && (this.indexFinger ? this.myRig.rightIndex.calcT : this.myRig.rightMiddle.calcT) > 0.5f);
		}
		if (flag != this.triggerWasDown)
		{
			if (flag)
			{
				this.onPress.Invoke();
				this.updateWhilePressed.Invoke();
			}
			else
			{
				this.onRelease.Invoke();
			}
		}
		else if (flag)
		{
			this.updateWhilePressed.Invoke();
		}
		this.triggerWasDown = flag;
	}

	// Token: 0x04000D2C RID: 3372
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04000D2D RID: 3373
	[SerializeField]
	private UnityEvent onPress;

	// Token: 0x04000D2E RID: 3374
	[SerializeField]
	private UnityEvent onRelease;

	// Token: 0x04000D2F RID: 3375
	[SerializeField]
	private UnityEvent updateWhilePressed;

	// Token: 0x04000D30 RID: 3376
	private VRRig myRig;

	// Token: 0x04000D31 RID: 3377
	private bool indexFinger = true;

	// Token: 0x04000D32 RID: 3378
	private bool triggerWasDown;
}
