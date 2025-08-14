using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000E8C RID: 3724
	[Serializable]
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06005D49 RID: 23881 RVA: 0x001D7B9C File Offset: 0x001D5D9C
		// (set) Token: 0x06005D4A RID: 23882 RVA: 0x001D7BA4 File Offset: 0x001D5DA4
		bool ITickSystemPre.PreTickRunning
		{
			get
			{
				return this.registered;
			}
			set
			{
				this.registered = value;
			}
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06005D4B RID: 23883 RVA: 0x001D7B9C File Offset: 0x001D5D9C
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x001D7BAD File Offset: 0x001D5DAD
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x001D7BB5 File Offset: 0x001D5DB5
		protected TickSystemTimerAbstract(float cd) : base(cd)
		{
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x001D7BBE File Offset: 0x001D5DBE
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001D7BCC File Offset: 0x001D5DCC
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001D7BDA File Offset: 0x001D5DDA
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x06005D51 RID: 23889
		public abstract void OnTimedEvent();

		// Token: 0x06005D52 RID: 23890 RVA: 0x001D7BE2 File Offset: 0x001D5DE2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x04006758 RID: 26456
		[NonSerialized]
		internal bool registered;
	}
}
