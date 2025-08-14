using System;
using UnityEngine;

// Token: 0x02000580 RID: 1408
public class EmitSignalToBiter : GTSignalEmitter
{
	// Token: 0x06002261 RID: 8801 RVA: 0x000B9A58 File Offset: 0x000B7C58
	public override void Emit()
	{
		if (this.onEdibleState == EmitSignalToBiter.EdibleState.None)
		{
			return;
		}
		if (!this.targetEdible)
		{
			return;
		}
		if (this.targetEdible.lastBiterActorID == -1)
		{
			return;
		}
		TransferrableObject.ItemStates itemState = this.targetEdible.itemState;
		if (itemState - TransferrableObject.ItemStates.State0 <= 1 || itemState == TransferrableObject.ItemStates.State2 || itemState == TransferrableObject.ItemStates.State3)
		{
			int num = (int)itemState;
			if ((this.onEdibleState & (EmitSignalToBiter.EdibleState)num) == (EmitSignalToBiter.EdibleState)num)
			{
				GTSignal.Emit(this.targetEdible.lastBiterActorID, this.signal, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void Emit(int targetActor)
	{
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void Emit(params object[] data)
	{
	}

	// Token: 0x04002BD3 RID: 11219
	[Space]
	public EdibleHoldable targetEdible;

	// Token: 0x04002BD4 RID: 11220
	[Space]
	[SerializeField]
	private EmitSignalToBiter.EdibleState onEdibleState;

	// Token: 0x02000581 RID: 1409
	[Flags]
	private enum EdibleState
	{
		// Token: 0x04002BD6 RID: 11222
		None = 0,
		// Token: 0x04002BD7 RID: 11223
		State0 = 1,
		// Token: 0x04002BD8 RID: 11224
		State1 = 2,
		// Token: 0x04002BD9 RID: 11225
		State2 = 4,
		// Token: 0x04002BDA RID: 11226
		State3 = 8
	}
}
