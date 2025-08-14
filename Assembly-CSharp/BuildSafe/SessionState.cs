using System;

namespace BuildSafe
{
	// Token: 0x02000CF5 RID: 3317
	public class SessionState
	{
		// Token: 0x170007B0 RID: 1968
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x04005BAB RID: 23467
		public static readonly SessionState Shared = new SessionState();
	}
}
