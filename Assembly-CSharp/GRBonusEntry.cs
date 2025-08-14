using System;

// Token: 0x0200060A RID: 1546
[Serializable]
public class GRBonusEntry
{
	// Token: 0x060025F7 RID: 9719 RVA: 0x000CB212 File Offset: 0x000C9412
	private GRBonusEntry()
	{
		GRBonusEntry.idCounter++;
		this.id = GRBonusEntry.idCounter;
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x060025F8 RID: 9720 RVA: 0x000CB231 File Offset: 0x000C9431
	// (set) Token: 0x060025F9 RID: 9721 RVA: 0x000CB239 File Offset: 0x000C9439
	public int id { get; private set; }

	// Token: 0x060025FA RID: 9722 RVA: 0x000CB244 File Offset: 0x000C9444
	public new string ToString()
	{
		bool flag = this.customBonus != null;
		return string.Format("GRBonusEntry BonusType {0} AttributeType {1} BonusValue {2} Id {3} CustomBonusSet {4}", new object[]
		{
			this.bonusType,
			this.attributeType,
			this.bonusValue,
			this.id,
			flag
		});
	}

	// Token: 0x04003022 RID: 12322
	private static int idCounter;

	// Token: 0x04003023 RID: 12323
	public GRBonusEntry.GRBonusType bonusType;

	// Token: 0x04003024 RID: 12324
	public GRAttributeType attributeType;

	// Token: 0x04003025 RID: 12325
	public int bonusValue;

	// Token: 0x04003027 RID: 12327
	public Func<int, GRBonusEntry, int> customBonus;

	// Token: 0x0200060B RID: 1547
	public enum GRBonusType
	{
		// Token: 0x04003029 RID: 12329
		None,
		// Token: 0x0400302A RID: 12330
		Additive,
		// Token: 0x0400302B RID: 12331
		Multiplicative
	}
}
