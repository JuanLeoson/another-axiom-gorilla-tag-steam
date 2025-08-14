using System;
using UnityEngine;

// Token: 0x0200096D RID: 2413
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04004897 RID: 18583
	public string title;

	// Token: 0x04004898 RID: 18584
	public string playFabKey;

	// Token: 0x04004899 RID: 18585
	public string latestVersionKey;

	// Token: 0x0400489A RID: 18586
	[TextArea(3, 5)]
	public string errorMessage;

	// Token: 0x0400489B RID: 18587
	public bool optional;

	// Token: 0x0400489C RID: 18588
	public LegalAgreementTextAsset.PostAcceptAction optInAction;

	// Token: 0x0400489D RID: 18589
	public string confirmString;

	// Token: 0x0200096E RID: 2414
	public enum PostAcceptAction
	{
		// Token: 0x0400489F RID: 18591
		NONE
	}
}
