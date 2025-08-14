using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C82 RID: 3202
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x06004F2B RID: 20267 RVA: 0x0018980B File Offset: 0x00187A0B
		public override void OnTapReplicated()
		{
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x04005815 RID: 22549
		[SerializeField]
		private Animation anim;
	}
}
