using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E8B RID: 3723
	[Serializable]
	public class CoolDownHelper
	{
		// Token: 0x06005D43 RID: 23875 RVA: 0x001D7B0F File Offset: 0x001D5D0F
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x001D7B2D File Offset: 0x001D5D2D
		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x001D7B48 File Offset: 0x001D5D48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CheckCooldown()
		{
			float unscaledTime = Time.unscaledTime;
			if (unscaledTime < this.checkTime)
			{
				return false;
			}
			this.OnCheckPass();
			this.checkTime = unscaledTime + this.coolDown;
			return true;
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x001D7B7B File Offset: 0x001D5D7B
		public virtual void Start()
		{
			this.checkTime = Time.unscaledTime + this.coolDown;
		}

		// Token: 0x06005D47 RID: 23879 RVA: 0x001D7B8F File Offset: 0x001D5D8F
		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		// Token: 0x06005D48 RID: 23880 RVA: 0x000023F5 File Offset: 0x000005F5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void OnCheckPass()
		{
		}

		// Token: 0x04006756 RID: 26454
		public float coolDown;

		// Token: 0x04006757 RID: 26455
		[NonSerialized]
		public float checkTime;
	}
}
