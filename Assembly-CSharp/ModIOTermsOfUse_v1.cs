using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModIO;
using TMPro;
using UnityEngine;

// Token: 0x02000835 RID: 2101
public class ModIOTermsOfUse_v1 : MonoBehaviour
{
	// Token: 0x06003499 RID: 13465 RVA: 0x0011172A File Offset: 0x0010F92A
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x0011174E File Offset: 0x0010F94E
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x00111772 File Offset: 0x0010F972
	public void Initialize(TermsOfUse terms, Action<bool> callback)
	{
		if (terms.hash.md5hash.Length != 0)
		{
			this.termsOfUse = terms;
			this.hasTermsOfUse = true;
			this.termsAcknowledgedCallback = callback;
		}
	}

	// Token: 0x0600349C RID: 13468 RVA: 0x0011179B File Offset: 0x0010F99B
	private void PostUpdate()
	{
		if (ControllerBehaviour.Instance.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (ControllerBehaviour.Instance.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = ControllerBehaviour.Instance.ButtonDown;
		}
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x001117DC File Offset: 0x0010F9DC
	private void Start()
	{
		ModIOTermsOfUse_v1.<Start>d__21 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<Start>d__21>(ref <Start>d__);
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x00111814 File Offset: 0x0010FA14
	private Task<bool> UpdateTextFromTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__22 <UpdateTextFromTerms>d__;
		<UpdateTextFromTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromTerms>d__.<>4__this = this;
		<UpdateTextFromTerms>d__.<>1__state = -1;
		<UpdateTextFromTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__22>(ref <UpdateTextFromTerms>d__);
		return <UpdateTextFromTerms>d__.<>t__builder.Task;
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x00111858 File Offset: 0x0010FA58
	public Task<bool> UpdateTextWithFullTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__23 <UpdateTextWithFullTerms>d__;
		<UpdateTextWithFullTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextWithFullTerms>d__.<>4__this = this;
		<UpdateTextWithFullTerms>d__.<>1__state = -1;
		<UpdateTextWithFullTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__23>(ref <UpdateTextWithFullTerms>d__);
		return <UpdateTextWithFullTerms>d__.<>t__builder.Task;
	}

	// Token: 0x060034A0 RID: 13472 RVA: 0x0011189C File Offset: 0x0010FA9C
	private string FormatAgreementText(CurrentAgreement agreement)
	{
		string text = string.Concat(new string[]
		{
			agreement.name,
			"\n\nEffective Date: ",
			agreement.dateLive.ToLongDateString(),
			"\n\n",
			agreement.content
		});
		text = Regex.Replace(text, "<!--[^>]*(-->)", "");
		text = text.Replace("<h1>", "<b>");
		text = text.Replace("</h1>", "</b>");
		text = text.Replace("<h2>", "<b>");
		text = text.Replace("</h2>", "</b>");
		text = text.Replace("<h3>", "<b>");
		text = text.Replace("</h3>", "</b>");
		text = text.Replace("<hr>", "");
		text = text.Replace("<br>", "\n");
		text = text.Replace("</li>", "</indent>\n");
		text = text.Replace("<strong>", "<b>");
		text = text.Replace("</strong>", "</b>");
		text = text.Replace("<em>", "<i>");
		text = text.Replace("</em>", "</i>");
		text = Regex.Replace(text, "<a[^>]*>{1}", "");
		text = text.Replace("</a>", "");
		Match match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
		while (match.Success)
		{
			text = text.Remove(match.Index, match.Length);
			text = text.Insert(match.Index, "\n<align=\"center\">");
			int startIndex = text.IndexOf("</p>", match.Index, StringComparison.Ordinal);
			text = text.Remove(startIndex, 4);
			text = text.Insert(startIndex, "</align>");
			match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
		}
		text = text.Replace("<p>", "\n");
		text = text.Replace("</p>", "");
		text = Regex.Replace(text, "<ol[^>]*>{1}", "<ol>");
		int num = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
		bool flag = num != -1;
		while (flag)
		{
			int num2 = text.IndexOf("</ol>", num, StringComparison.OrdinalIgnoreCase);
			text = text.Remove(num, "<ol>".Length);
			int num3 = text.IndexOf("<li>", num, StringComparison.OrdinalIgnoreCase);
			bool flag2 = num3 != -1;
			int num4 = 0;
			while (flag2)
			{
				text = text.Remove(num3, "<li>".Length);
				text = text.Insert(num3, this.GetStringForListItemIdx_LowerAlpha(num4++));
				num2 = text.IndexOf("</ol>", num, StringComparison.OrdinalIgnoreCase);
				num3 = text.IndexOf("<li>", num, StringComparison.OrdinalIgnoreCase);
				flag2 = (num3 != -1 && num3 < num2);
			}
			text = text.Remove(num2, "</ol>".Length);
			num = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
			flag = (num != -1);
		}
		text = Regex.Replace(text, "<ul[^>]*>{1}", "<ul>");
		int num5 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
		bool flag3 = num5 != -1;
		while (flag3)
		{
			int num6 = text.IndexOf("</ul>", num5, StringComparison.OrdinalIgnoreCase);
			text = text.Remove(num5, "<ul>".Length);
			int num7 = text.IndexOf("<li>", num5, StringComparison.OrdinalIgnoreCase);
			bool flag4 = num7 != -1;
			while (flag4)
			{
				text = text.Remove(num7, "<li>".Length);
				text = text.Insert(num7, "  - <indent=5%>");
				num6 = text.IndexOf("</ul>", num5, StringComparison.OrdinalIgnoreCase);
				num7 = text.IndexOf("<li>", num5, StringComparison.OrdinalIgnoreCase);
				flag4 = (num7 != -1 && num7 < num6);
			}
			text = text.Remove(num6, "</ul>".Length);
			num5 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
			flag3 = (num5 != -1);
		}
		text = Regex.Replace(text, "<table[^>]*>{1}", "");
		text = text.Replace("<tbody>", "");
		text = text.Replace("<tr>", "");
		text = text.Replace("<td>", "");
		text = text.Replace("<center>", "");
		text = text.Replace("</table>", "");
		text = text.Replace("</tbody>", "");
		text = text.Replace("</tr>", "\n");
		text = text.Replace("</td>", "");
		return text.Replace("</center>", "");
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x00111D20 File Offset: 0x0010FF20
	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x00111E44 File Offset: 0x00110044
	private Task WaitForAcknowledgement()
	{
		ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__26 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__26>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x060034A3 RID: 13475 RVA: 0x00111E88 File Offset: 0x00110088
	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x00111F44 File Offset: 0x00110144
	private void ActivateAcceptButtonGroup()
	{
		bool active = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(active);
		this.waitingForAcknowledge = active;
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x00111F82 File Offset: 0x00110182
	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	// Token: 0x0400415F RID: 16735
	[SerializeField]
	private Transform uiParent;

	// Token: 0x04004160 RID: 16736
	[SerializeField]
	private string title;

	// Token: 0x04004161 RID: 16737
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x04004162 RID: 16738
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x04004163 RID: 16739
	[SerializeField]
	private TMP_Text tmpPage;

	// Token: 0x04004164 RID: 16740
	[SerializeField]
	public GameObject yesNoButtons;

	// Token: 0x04004165 RID: 16741
	[SerializeField]
	public GameObject nextButton;

	// Token: 0x04004166 RID: 16742
	[SerializeField]
	public GameObject prevButton;

	// Token: 0x04004167 RID: 16743
	private TermsOfUse termsOfUse;

	// Token: 0x04004168 RID: 16744
	private bool hasTermsOfUse;

	// Token: 0x04004169 RID: 16745
	private Action<bool> termsAcknowledgedCallback;

	// Token: 0x0400416A RID: 16746
	private string cachedTermsText;

	// Token: 0x0400416B RID: 16747
	private bool waitingForAcknowledge;

	// Token: 0x0400416C RID: 16748
	private bool accepted;

	// Token: 0x0400416D RID: 16749
	private bool acceptButtonDown;

	// Token: 0x0400416E RID: 16750
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x0400416F RID: 16751
	[SerializeField]
	private LineRenderer progressBar;
}
