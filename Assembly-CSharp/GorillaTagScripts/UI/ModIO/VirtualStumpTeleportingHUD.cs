using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000C75 RID: 3189
	public class VirtualStumpTeleportingHUD : MonoBehaviour
	{
		// Token: 0x06004EE2 RID: 20194 RVA: 0x00188B84 File Offset: 0x00186D84
		public void Initialize(bool isEntering)
		{
			this.isEnteringVirtualStump = isEntering;
			if (isEntering)
			{
				this.teleportingStatusText.text = this.enteringVirtualStumpString;
				this.teleportingStatusText.gameObject.SetActive(true);
				return;
			}
			this.teleportingStatusText.text = this.leavingVirtualStumpString;
			this.teleportingStatusText.gameObject.SetActive(true);
		}

		// Token: 0x06004EE3 RID: 20195 RVA: 0x00188BE0 File Offset: 0x00186DE0
		private void Update()
		{
			if (Time.time - this.lastTextUpdateTime > this.textUpdateInterval)
			{
				this.lastTextUpdateTime = Time.time;
				this.IncrementProgressDots();
				this.teleportingStatusText.text = (this.isEnteringVirtualStump ? this.enteringVirtualStumpString : this.leavingVirtualStumpString);
				for (int i = 0; i < this.numProgressDots; i++)
				{
					TMP_Text tmp_Text = this.teleportingStatusText;
					tmp_Text.text += ".";
				}
			}
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x00188C5F File Offset: 0x00186E5F
		private void IncrementProgressDots()
		{
			this.numProgressDots++;
			if (this.numProgressDots > this.maxNumProgressDots)
			{
				this.numProgressDots = 0;
			}
		}

		// Token: 0x040057E8 RID: 22504
		[SerializeField]
		private string enteringVirtualStumpString = "Now Entering the Virtual Stump";

		// Token: 0x040057E9 RID: 22505
		[SerializeField]
		private string leavingVirtualStumpString = "Now Leaving the Virtual Stump";

		// Token: 0x040057EA RID: 22506
		[SerializeField]
		private TMP_Text teleportingStatusText;

		// Token: 0x040057EB RID: 22507
		[SerializeField]
		private int maxNumProgressDots = 3;

		// Token: 0x040057EC RID: 22508
		[SerializeField]
		private float textUpdateInterval = 0.5f;

		// Token: 0x040057ED RID: 22509
		private float lastTextUpdateTime;

		// Token: 0x040057EE RID: 22510
		private int numProgressDots;

		// Token: 0x040057EF RID: 22511
		private bool isEnteringVirtualStump;
	}
}
