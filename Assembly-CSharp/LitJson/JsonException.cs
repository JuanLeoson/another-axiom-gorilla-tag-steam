using System;

namespace LitJson
{
	// Token: 0x02000BB3 RID: 2995
	public class JsonException : ApplicationException
	{
		// Token: 0x06004868 RID: 18536 RVA: 0x0016106D File Offset: 0x0015F26D
		public JsonException()
		{
		}

		// Token: 0x06004869 RID: 18537 RVA: 0x00161075 File Offset: 0x0015F275
		internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x0600486A RID: 18538 RVA: 0x0016108D File Offset: 0x0015F28D
		internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x0600486B RID: 18539 RVA: 0x001610A6 File Offset: 0x0015F2A6
		internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x0600486C RID: 18540 RVA: 0x001610BF File Offset: 0x0015F2BF
		internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x0600486D RID: 18541 RVA: 0x001610D9 File Offset: 0x0015F2D9
		public JsonException(string message) : base(message)
		{
		}

		// Token: 0x0600486E RID: 18542 RVA: 0x001610E2 File Offset: 0x0015F2E2
		public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
		{
		}
	}
}
