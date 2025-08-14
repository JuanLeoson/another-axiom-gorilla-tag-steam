using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FAD RID: 4013
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x04006F0D RID: 28429
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x04006F0E RID: 28430
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x04006F0F RID: 28431
		[SerializeField]
		private string m_formula = "";

		// Token: 0x04006F10 RID: 28432
		private Texture m_texture;
	}
}
