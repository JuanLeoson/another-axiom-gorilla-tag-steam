using System;

namespace LitJson
{
	// Token: 0x02000BC8 RID: 3016
	internal enum ParserToken
	{
		// Token: 0x04005245 RID: 21061
		None = 65536,
		// Token: 0x04005246 RID: 21062
		Number,
		// Token: 0x04005247 RID: 21063
		True,
		// Token: 0x04005248 RID: 21064
		False,
		// Token: 0x04005249 RID: 21065
		Null,
		// Token: 0x0400524A RID: 21066
		CharSeq,
		// Token: 0x0400524B RID: 21067
		Char,
		// Token: 0x0400524C RID: 21068
		Text,
		// Token: 0x0400524D RID: 21069
		Object,
		// Token: 0x0400524E RID: 21070
		ObjectPrime,
		// Token: 0x0400524F RID: 21071
		Pair,
		// Token: 0x04005250 RID: 21072
		PairRest,
		// Token: 0x04005251 RID: 21073
		Array,
		// Token: 0x04005252 RID: 21074
		ArrayPrime,
		// Token: 0x04005253 RID: 21075
		Value,
		// Token: 0x04005254 RID: 21076
		ValueRest,
		// Token: 0x04005255 RID: 21077
		String,
		// Token: 0x04005256 RID: 21078
		End,
		// Token: 0x04005257 RID: 21079
		Epsilon
	}
}
