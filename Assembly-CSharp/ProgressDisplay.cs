using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000139 RID: 313
public class ProgressDisplay : MonoBehaviour
{
	// Token: 0x0600083D RID: 2109 RVA: 0x0002D865 File Offset: 0x0002BA65
	private void Reset()
	{
		this.root = base.gameObject;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0002D873 File Offset: 0x0002BA73
	public void SetVisible(bool visible)
	{
		this.root.SetActive(visible);
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0002D884 File Offset: 0x0002BA84
	public void SetProgress(int progress, int total)
	{
		if (this.text)
		{
			if (total < this.largestNumberToShow)
			{
				this.text.text = ((progress >= total) ? string.Format("{0}", total) : string.Format("{0}/{1}", progress, total));
				this.SetTextVisible(true);
			}
			else
			{
				this.SetTextVisible(false);
			}
		}
		this.progressImage.fillAmount = (float)progress / (float)total;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0002D8FE File Offset: 0x0002BAFE
	public void SetProgress(float progress)
	{
		this.progressImage.fillAmount = progress;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0002D90C File Offset: 0x0002BB0C
	private void SetTextVisible(bool visible)
	{
		if (this.text.gameObject.activeSelf == visible)
		{
			return;
		}
		this.text.gameObject.SetActive(visible);
	}

	// Token: 0x040009D5 RID: 2517
	[SerializeField]
	private GameObject root;

	// Token: 0x040009D6 RID: 2518
	[SerializeField]
	private TMP_Text text;

	// Token: 0x040009D7 RID: 2519
	[SerializeField]
	private Image progressImage;

	// Token: 0x040009D8 RID: 2520
	[SerializeField]
	private int largestNumberToShow = 99;
}
