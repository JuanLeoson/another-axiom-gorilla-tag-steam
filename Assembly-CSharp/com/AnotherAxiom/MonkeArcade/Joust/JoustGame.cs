using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000DF7 RID: 3575
	public class JoustGame : ArcadeGame
	{
		// Token: 0x06005888 RID: 22664 RVA: 0x001B88FE File Offset: 0x001B6AFE
		public override byte[] GetNetworkState()
		{
			return new byte[0];
		}

		// Token: 0x06005889 RID: 22665 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void SetNetworkState(byte[] obj)
		{
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x001B8906 File Offset: 0x001B6B06
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button != ArcadeButtons.GRAB)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					this.joustPlayers[player].Flap();
					return;
				}
			}
			else
			{
				this.joustPlayers[player].gameObject.SetActive(true);
			}
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x001B8935 File Offset: 0x001B6B35
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.GRAB)
			{
				this.joustPlayers[player].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x001B8950 File Offset: 0x001B6B50
		private void Start()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				this.joustPlayers[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x001B8984 File Offset: 0x001B6B84
		private void Update()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				if (this.joustPlayers[i].gameObject.activeInHierarchy)
				{
					int num = (base.getButtonState(i, ArcadeButtons.LEFT) ? -1 : 0) + (base.getButtonState(i, ArcadeButtons.RIGHT) ? 1 : 0);
					this.joustPlayers[i].HorizontalSpeed = Mathf.Clamp(this.joustPlayers[i].HorizontalSpeed + (float)num * Time.deltaTime, -1f, 1f);
				}
			}
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnTimeout()
		{
		}

		// Token: 0x04006265 RID: 25189
		[SerializeField]
		private JoustPlayer[] joustPlayers;
	}
}
