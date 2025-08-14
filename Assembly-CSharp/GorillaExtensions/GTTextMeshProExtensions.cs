using System;
using Cysharp.Text;
using TMPro;

namespace GorillaExtensions
{
	// Token: 0x02000E3E RID: 3646
	public static class GTTextMeshProExtensions
	{
		// Token: 0x06005AA2 RID: 23202 RVA: 0x001CA0C0 File Offset: 0x001C82C0
		public static void SetTextToZString(this TMP_Text textMono, Utf16ValueStringBuilder zStringBuilder)
		{
			ArraySegment<char> arraySegment = zStringBuilder.AsArraySegment();
			textMono.SetCharArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
	}
}
