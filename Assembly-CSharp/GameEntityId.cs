using System;

// Token: 0x020005AA RID: 1450
public struct GameEntityId
{
	// Token: 0x0600237C RID: 9084 RVA: 0x000BDE30 File Offset: 0x000BC030
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000BDE3E File Offset: 0x000BC03E
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000BDE4E File Offset: 0x000BC04E
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000BDE64 File Offset: 0x000BC064
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000BDE86 File Offset: 0x000BC086
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x04002D0E RID: 11534
	public static GameEntityId Invalid = new GameEntityId
	{
		index = -1
	};

	// Token: 0x04002D0F RID: 11535
	public int index;
}
