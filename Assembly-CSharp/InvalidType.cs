using System;

// Token: 0x02000B12 RID: 2834
public class InvalidType : ProxyType
{
	// Token: 0x1700066B RID: 1643
	// (get) Token: 0x06004436 RID: 17462 RVA: 0x00155BDD File Offset: 0x00153DDD
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x1700066C RID: 1644
	// (get) Token: 0x06004437 RID: 17463 RVA: 0x00155BEA File Offset: 0x00153DEA
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x1700066D RID: 1645
	// (get) Token: 0x06004438 RID: 17464 RVA: 0x00155BF7 File Offset: 0x00153DF7
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x04004E74 RID: 20084
	private Type _self = typeof(InvalidType);
}
