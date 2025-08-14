using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200047E RID: 1150
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x06001C89 RID: 7305 RVA: 0x0009983C File Offset: 0x00097A3C
	private void Awake()
	{
		this.codeText.text = "CREATOR CODE: <NONE>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x00099860 File Offset: 0x00097A60
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "CREATOR CODE: <NONE>";
			return;
		}
		this.codeText.text = "CREATOR CODE: " + code;
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x00099896 File Offset: 0x00097A96
	public void SuccessfulPurchase(string memberName)
	{
		if (!string.IsNullOrWhiteSpace(memberName))
		{
			this.codeText.text = "SUPPORTED: " + memberName + "!";
		}
	}

	// Token: 0x040024FB RID: 9467
	public Text codeText;

	// Token: 0x040024FC RID: 9468
	private const string CreatorCode = "CREATOR CODE: ";

	// Token: 0x040024FD RID: 9469
	private const string CreatorSupported = "SUPPORTED: ";

	// Token: 0x040024FE RID: 9470
	private const string NoCreator = "<NONE>";
}
