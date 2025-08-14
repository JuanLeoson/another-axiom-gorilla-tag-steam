using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000466 RID: 1126
public class TransferrableObjectSyncedBool : TransferrableObject
{
	// Token: 0x06001BF2 RID: 7154 RVA: 0x000965D3 File Offset: 0x000947D3
	protected override void LateUpdateReplicated()
	{
		TransferrableObject.ItemStates itemState = this.itemState;
		base.LateUpdateReplicated();
		if (itemState != this.itemState)
		{
			this.OnItemStateChanged();
		}
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000965EF File Offset: 0x000947EF
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000965FE File Offset: 0x000947FE
	protected override void OnStateChanged()
	{
		if (!base.InHand() && this.itemState != TransferrableObject.ItemStates.State0)
		{
			this.SetItemState(false);
		}
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x00096618 File Offset: 0x00094818
	public override void ResetToDefaultState()
	{
		TransferrableObject.ItemStates itemState = this.itemState;
		base.ResetToDefaultState();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.itemState != itemState)
		{
			this.OnItemStateChanged();
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x00096648 File Offset: 0x00094848
	public void SetItemState(bool state)
	{
		if (!base.IsLocalObject())
		{
			return;
		}
		TransferrableObject.ItemStates itemStates = state ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0;
		if (this.itemState != itemStates)
		{
			this.itemState = itemStates;
			this.OnItemStateChanged();
		}
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x0009667C File Offset: 0x0009487C
	public void ToggleItemState()
	{
		if (!base.IsLocalObject())
		{
			return;
		}
		TransferrableObject.ItemStates itemState = (this.itemState == TransferrableObject.ItemStates.State0) ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0;
		this.itemState = itemState;
		this.OnItemStateChanged();
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000966AD File Offset: 0x000948AD
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			UnityEvent onItemStateSetFalse = this.OnItemStateSetFalse;
			if (onItemStateSetFalse == null)
			{
				return;
			}
			onItemStateSetFalse.Invoke();
			return;
		}
		else
		{
			UnityEvent onItemStateSetTrue = this.OnItemStateSetTrue;
			if (onItemStateSetTrue == null)
			{
				return;
			}
			onItemStateSetTrue.Invoke();
			return;
		}
	}

	// Token: 0x0400247B RID: 9339
	[SerializeField]
	private UnityEvent OnItemStateSetTrue;

	// Token: 0x0400247C RID: 9340
	[SerializeField]
	private UnityEvent OnItemStateSetFalse;
}
