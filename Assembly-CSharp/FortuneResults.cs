using System;
using UnityEngine;

// Token: 0x02000591 RID: 1425
public class FortuneResults : ScriptableObject
{
	// Token: 0x060022C8 RID: 8904 RVA: 0x000BBFB0 File Offset: 0x000BA1B0
	private void OnValidate()
	{
		this.totalChance = 0f;
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			this.totalChance += this.fortuneResults[i].weightedChance;
		}
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000BBFFC File Offset: 0x000BA1FC
	public FortuneResults.FortuneResult GetResult()
	{
		float num = Random.Range(0f, this.totalChance);
		int i = 0;
		while (i < this.fortuneResults.Length)
		{
			FortuneResults.FortuneCategory fortuneCategory = this.fortuneResults[i];
			if (num <= fortuneCategory.weightedChance)
			{
				if (fortuneCategory.textResults.Length == 0)
				{
					return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
				}
				int resultIndex = Random.Range(0, fortuneCategory.textResults.Length);
				return new FortuneResults.FortuneResult(fortuneCategory.fortuneType, resultIndex);
			}
			else
			{
				num -= fortuneCategory.weightedChance;
				i++;
			}
		}
		return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000BC080 File Offset: 0x000BA280
	public string GetResultText(FortuneResults.FortuneResult result)
	{
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			if (this.fortuneResults[i].fortuneType == result.fortuneType && result.resultIndex >= 0 && result.resultIndex < this.fortuneResults[i].textResults.Length)
			{
				return this.fortuneResults[i].textResults[result.resultIndex];
			}
		}
		return "!! Invalid Fortune !!";
	}

	// Token: 0x04002C6B RID: 11371
	[SerializeField]
	private FortuneResults.FortuneCategory[] fortuneResults;

	// Token: 0x04002C6C RID: 11372
	[SerializeField]
	private float totalChance;

	// Token: 0x02000592 RID: 1426
	public enum FortuneCategoryType
	{
		// Token: 0x04002C6E RID: 11374
		Invalid,
		// Token: 0x04002C6F RID: 11375
		Positive,
		// Token: 0x04002C70 RID: 11376
		Neutral,
		// Token: 0x04002C71 RID: 11377
		Negative,
		// Token: 0x04002C72 RID: 11378
		Seasonal
	}

	// Token: 0x02000593 RID: 1427
	[Serializable]
	public struct FortuneCategory
	{
		// Token: 0x04002C73 RID: 11379
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04002C74 RID: 11380
		public float weightedChance;

		// Token: 0x04002C75 RID: 11381
		public string[] textResults;
	}

	// Token: 0x02000594 RID: 1428
	public struct FortuneResult
	{
		// Token: 0x060022CC RID: 8908 RVA: 0x000BC0FB File Offset: 0x000BA2FB
		public FortuneResult(FortuneResults.FortuneCategoryType fortuneType, int resultIndex)
		{
			this.fortuneType = fortuneType;
			this.resultIndex = resultIndex;
		}

		// Token: 0x04002C76 RID: 11382
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04002C77 RID: 11383
		public int resultIndex;
	}
}
