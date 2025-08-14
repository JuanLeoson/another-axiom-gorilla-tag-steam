using System;
using Fusion;

// Token: 0x020004CA RID: 1226
[NetworkBehaviourWeaved(23)]
public class HuntGameModeData : FusionGameModeData
{
	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001E26 RID: 7718 RVA: 0x000A0BF3 File Offset: 0x0009EDF3
	// (set) Token: 0x06001E27 RID: 7719 RVA: 0x000A0C00 File Offset: 0x0009EE00
	public override object Data
	{
		get
		{
			return this.huntdata;
		}
		set
		{
			this.huntdata = (HuntData)value;
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001E28 RID: 7720 RVA: 0x000A0C0E File Offset: 0x0009EE0E
	// (set) Token: 0x06001E29 RID: 7721 RVA: 0x000A0C38 File Offset: 0x0009EE38
	[Networked]
	[NetworkedWeaved(0, 23)]
	private unsafe HuntData huntdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HuntData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HuntData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x000A0C63 File Offset: 0x0009EE63
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.huntdata = this._huntdata;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000A0C7B File Offset: 0x0009EE7B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._huntdata = this.huntdata;
	}

	// Token: 0x040026B6 RID: 9910
	[WeaverGenerated]
	[DefaultForProperty("huntdata", 0, 23)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HuntData _huntdata;
}
