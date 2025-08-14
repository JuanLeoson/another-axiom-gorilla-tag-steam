using System;
using Fusion;

// Token: 0x020004C7 RID: 1223
[NetworkBehaviourWeaved(1)]
public class CasualGameModeData : FusionGameModeData
{
	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001E18 RID: 7704 RVA: 0x000A0B13 File Offset: 0x0009ED13
	// (set) Token: 0x06001E19 RID: 7705 RVA: 0x000023F5 File Offset: 0x000005F5
	public override object Data
	{
		get
		{
			return this.casualData;
		}
		set
		{
		}
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001E1A RID: 7706 RVA: 0x000A0B20 File Offset: 0x0009ED20
	// (set) Token: 0x06001E1B RID: 7707 RVA: 0x000A0B4A File Offset: 0x0009ED4A
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe CasualData casualData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(CasualData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(CasualData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000A0B75 File Offset: 0x0009ED75
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.casualData = this._casualData;
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000A0B8D File Offset: 0x0009ED8D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._casualData = this.casualData;
	}

	// Token: 0x040026AF RID: 9903
	[WeaverGenerated]
	[DefaultForProperty("casualData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private CasualData _casualData;
}
