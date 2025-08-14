using System;
using System.Collections.Generic;
using System.Text;
using KID.Model;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C46 RID: 3142
	public class PlayerTimerBoard : MonoBehaviour
	{
		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06004DBA RID: 19898 RVA: 0x001823ED File Offset: 0x001805ED
		// (set) Token: 0x06004DBB RID: 19899 RVA: 0x001823F5 File Offset: 0x001805F5
		public bool IsDirty { get; set; } = true;

		// Token: 0x06004DBC RID: 19900 RVA: 0x001823FE File Offset: 0x001805FE
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x001823FE File Offset: 0x001805FE
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x00182406 File Offset: 0x00180606
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.RegisterTimerBoard(this);
			this.isInitialized = true;
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x00182431 File Offset: 0x00180631
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.UnregisterTimerBoard(this);
			}
			this.isInitialized = false;
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x00182452 File Offset: 0x00180652
		public void SetSleepState(bool awake)
		{
			this.playerColumn.enabled = awake;
			this.timeColumn.enabled = awake;
			if (this.linesParent != null)
			{
				this.linesParent.SetActive(awake);
			}
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x00182486 File Offset: 0x00180686
		public void SortLines()
		{
			this.lines.Sort(new Comparison<PlayerTimerBoardLine>(PlayerTimerBoardLine.CompareByTotalTime));
		}

		// Token: 0x06004DC2 RID: 19906 RVA: 0x001824A0 File Offset: 0x001806A0
		public void RedrawPlayerLines()
		{
			this.stringBuilder.Clear();
			this.stringBuilderTime.Clear();
			this.stringBuilder.Append("<b><color=yellow>PLAYER</color></b>");
			this.stringBuilderTime.Append("<b><color=yellow>LATEST TIME</color></b>");
			this.SortLines();
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
			for (int i = 0; i < this.lines.Count; i++)
			{
				try
				{
					if (this.lines[i].gameObject.activeInHierarchy)
					{
						this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
						if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
						{
							this.stringBuilder.Append("\n ");
							this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
							this.stringBuilderTime.Append("\n ");
							this.stringBuilderTime.Append(this.lines[i].playerTimeStr);
						}
					}
				}
				catch
				{
				}
			}
			this.playerColumn.text = this.stringBuilder.ToString();
			this.timeColumn.text = this.stringBuilderTime.ToString();
			this.IsDirty = false;
		}

		// Token: 0x040056BC RID: 22204
		[SerializeField]
		private GameObject linesParent;

		// Token: 0x040056BD RID: 22205
		public List<PlayerTimerBoardLine> lines;

		// Token: 0x040056BE RID: 22206
		public TextMeshPro notInRoomText;

		// Token: 0x040056BF RID: 22207
		public TextMeshPro playerColumn;

		// Token: 0x040056C0 RID: 22208
		public TextMeshPro timeColumn;

		// Token: 0x040056C1 RID: 22209
		[SerializeField]
		private int startingYValue;

		// Token: 0x040056C2 RID: 22210
		[SerializeField]
		private int lineHeight;

		// Token: 0x040056C3 RID: 22211
		private StringBuilder stringBuilder = new StringBuilder(220);

		// Token: 0x040056C4 RID: 22212
		private StringBuilder stringBuilderTime = new StringBuilder(220);

		// Token: 0x040056C5 RID: 22213
		private bool isInitialized;
	}
}
