using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000A4A RID: 2634
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x06004070 RID: 16496 RVA: 0x001469D4 File Offset: 0x00144BD4
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x06004071 RID: 16497 RVA: 0x001469E7 File Offset: 0x00144BE7
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x06004072 RID: 16498 RVA: 0x00146A07 File Offset: 0x00144C07
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x06004073 RID: 16499 RVA: 0x00146A1B File Offset: 0x00144C1B
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x06004074 RID: 16500 RVA: 0x00146A38 File Offset: 0x00144C38
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x04004C1B RID: 19483
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
