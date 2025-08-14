using System;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000CCF RID: 3279
	public interface IState
	{
		// Token: 0x06005174 RID: 20852
		void Tick();

		// Token: 0x06005175 RID: 20853
		void OnEnter();

		// Token: 0x06005176 RID: 20854
		void OnExit();
	}
}
