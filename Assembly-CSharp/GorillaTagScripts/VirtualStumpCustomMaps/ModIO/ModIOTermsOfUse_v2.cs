using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModIO;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps.ModIO
{
	// Token: 0x02000C6B RID: 3179
	public class ModIOTermsOfUse_v2 : LegalAgreements
	{
		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06004EBB RID: 20155 RVA: 0x001877FB File Offset: 0x001859FB
		// (set) Token: 0x06004EBC RID: 20156 RVA: 0x00187802 File Offset: 0x00185A02
		public static ModIOTermsOfUse_v2 modioTermsInstance { get; private set; }

		// Token: 0x06004EBD RID: 20157 RVA: 0x0018780C File Offset: 0x00185A0C
		protected override void Awake()
		{
			if (ModIOTermsOfUse_v2.modioTermsInstance != null)
			{
				Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
				base.gameObject.SetActive(false);
				return;
			}
			ModIOTermsOfUse_v2.modioTermsInstance = this;
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
			base.enabled = false;
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x00187862 File Offset: 0x00185A62
		public void Initialize(TermsOfUse terms, Action<bool> callback)
		{
			if (terms.hash.md5hash.Length != 0)
			{
				this.termsOfUse = terms;
				this.hasTermsOfUse = true;
				this.termsAcknowledgedCallback = callback;
				base.enabled = true;
				this.StartLegalAgreements();
			}
		}

		// Token: 0x06004EBF RID: 20159 RVA: 0x0018789C File Offset: 0x00185A9C
		public override Task StartLegalAgreements()
		{
			ModIOTermsOfUse_v2.<StartLegalAgreements>d__11 <StartLegalAgreements>d__;
			<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartLegalAgreements>d__.<>4__this = this;
			<StartLegalAgreements>d__.<>1__state = -1;
			<StartLegalAgreements>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<StartLegalAgreements>d__11>(ref <StartLegalAgreements>d__);
			return <StartLegalAgreements>d__.<>t__builder.Task;
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x001878E0 File Offset: 0x00185AE0
		private Task<bool> UpdateTextFromTerms()
		{
			ModIOTermsOfUse_v2.<UpdateTextFromTerms>d__12 <UpdateTextFromTerms>d__;
			<UpdateTextFromTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateTextFromTerms>d__.<>4__this = this;
			<UpdateTextFromTerms>d__.<>1__state = -1;
			<UpdateTextFromTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<UpdateTextFromTerms>d__12>(ref <UpdateTextFromTerms>d__);
			return <UpdateTextFromTerms>d__.<>t__builder.Task;
		}

		// Token: 0x06004EC1 RID: 20161 RVA: 0x00187924 File Offset: 0x00185B24
		public Task<bool> UpdateTextWithFullTerms()
		{
			ModIOTermsOfUse_v2.<UpdateTextWithFullTerms>d__13 <UpdateTextWithFullTerms>d__;
			<UpdateTextWithFullTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateTextWithFullTerms>d__.<>4__this = this;
			<UpdateTextWithFullTerms>d__.<>1__state = -1;
			<UpdateTextWithFullTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<UpdateTextWithFullTerms>d__13>(ref <UpdateTextWithFullTerms>d__);
			return <UpdateTextWithFullTerms>d__.<>t__builder.Task;
		}

		// Token: 0x06004EC2 RID: 20162 RVA: 0x00187968 File Offset: 0x00185B68
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

		// Token: 0x06004EC3 RID: 20163 RVA: 0x00187DEC File Offset: 0x00185FEC
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

		// Token: 0x04005788 RID: 22408
		[SerializeField]
		private string confirmString = "Press and Hold to Confirm";

		// Token: 0x0400578A RID: 22410
		private TermsOfUse termsOfUse;

		// Token: 0x0400578B RID: 22411
		private bool hasTermsOfUse;

		// Token: 0x0400578C RID: 22412
		private Action<bool> termsAcknowledgedCallback;

		// Token: 0x0400578D RID: 22413
		private string cachedTermsText;
	}
}
