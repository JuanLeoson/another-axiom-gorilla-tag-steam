using System;
using UnityEngine;

// Token: 0x02000464 RID: 1124
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x06001BE6 RID: 7142 RVA: 0x0009648F File Offset: 0x0009468F
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000964C7 File Offset: 0x000946C7
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04002476 RID: 9334
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04002477 RID: 9335
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
