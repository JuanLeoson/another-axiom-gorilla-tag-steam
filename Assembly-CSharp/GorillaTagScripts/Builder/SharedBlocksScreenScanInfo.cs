using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CC6 RID: 3270
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x0600511C RID: 20764 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnUpPressed()
		{
		}

		// Token: 0x0600511D RID: 20765 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnDownPressed()
		{
		}

		// Token: 0x0600511E RID: 20766 RVA: 0x00194754 File Offset: 0x00192954
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed();
		}

		// Token: 0x0600511F RID: 20767 RVA: 0x00194761 File Offset: 0x00192961
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x06005120 RID: 20768 RVA: 0x00194770 File Offset: 0x00192970
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x04005AA7 RID: 23207
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
