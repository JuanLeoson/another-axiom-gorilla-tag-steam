using System;

// Token: 0x020004F2 RID: 1266
public struct GameBallId
{
	// Token: 0x06001EBC RID: 7868 RVA: 0x000A2331 File Offset: 0x000A0531
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000A233A File Offset: 0x000A053A
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000A2348 File Offset: 0x000A0548
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000A2358 File Offset: 0x000A0558
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000A236C File Offset: 0x000A056C
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000A238E File Offset: 0x000A058E
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x0400276F RID: 10095
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x04002770 RID: 10096
	public int index;
}
