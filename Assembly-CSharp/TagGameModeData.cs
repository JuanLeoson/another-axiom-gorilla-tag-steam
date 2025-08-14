using System;
using Fusion;

// Token: 0x020004CC RID: 1228
[NetworkBehaviourWeaved(12)]
public class TagGameModeData : FusionGameModeData
{
	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001E30 RID: 7728 RVA: 0x000A0CC8 File Offset: 0x0009EEC8
	// (set) Token: 0x06001E31 RID: 7729 RVA: 0x000A0CD5 File Offset: 0x0009EED5
	public override object Data
	{
		get
		{
			return this.tagData;
		}
		set
		{
			this.tagData = (TagData)value;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001E32 RID: 7730 RVA: 0x000A0CE3 File Offset: 0x0009EEE3
	// (set) Token: 0x06001E33 RID: 7731 RVA: 0x000A0D0D File Offset: 0x0009EF0D
	[Networked]
	[NetworkedWeaved(0, 12)]
	private unsafe TagData tagData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(TagData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(TagData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x000A0D38 File Offset: 0x0009EF38
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.tagData = this._tagData;
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000A0D50 File Offset: 0x0009EF50
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._tagData = this.tagData;
	}

	// Token: 0x040026BA RID: 9914
	[WeaverGenerated]
	[DefaultForProperty("tagData", 0, 12)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private TagData _tagData;
}
