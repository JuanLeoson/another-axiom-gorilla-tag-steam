using System;
using System.IO;
using UnityEngine;

// Token: 0x02000534 RID: 1332
[Serializable]
public struct SnapBounds
{
	// Token: 0x06002061 RID: 8289 RVA: 0x000AB49E File Offset: 0x000A969E
	public SnapBounds(Vector2Int min, Vector2Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000AB4AE File Offset: 0x000A96AE
	public SnapBounds(int minX, int minY, int maxX, int maxY)
	{
		this.min = new Vector2Int(minX, minY);
		this.max = new Vector2Int(maxX, maxY);
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000AB4CB File Offset: 0x000A96CB
	public void Clear()
	{
		this.min = new Vector2Int(int.MinValue, int.MinValue);
		this.max = new Vector2Int(int.MinValue, int.MinValue);
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000AB4F8 File Offset: 0x000A96F8
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.min.x);
		writer.Write(this.min.y);
		writer.Write(this.max.x);
		writer.Write(this.max.y);
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000AB54C File Offset: 0x000A974C
	public void Read(BinaryReader reader)
	{
		this.min.x = reader.ReadInt32();
		this.min.y = reader.ReadInt32();
		this.max.x = reader.ReadInt32();
		this.max.y = reader.ReadInt32();
	}

	// Token: 0x04002927 RID: 10535
	public Vector2Int min;

	// Token: 0x04002928 RID: 10536
	public Vector2Int max;
}
