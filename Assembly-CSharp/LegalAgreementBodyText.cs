using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200095D RID: 2397
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06003AD6 RID: 15062 RVA: 0x001308DA File Offset: 0x0012EADA
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x001308F0 File Offset: 0x0012EAF0
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x00130980 File Offset: 0x0012EB80
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x001309DC File Offset: 0x0012EBDC
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x00130A2F File Offset: 0x0012EC2F
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06003ADB RID: 15067 RVA: 0x00130A4D File Offset: 0x0012EC4D
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x06003ADC RID: 15068 RVA: 0x00130A60 File Offset: 0x0012EC60
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04004838 RID: 18488
	[SerializeField]
	private Text textBox;

	// Token: 0x04004839 RID: 18489
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x0400483A RID: 18490
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x0400483B RID: 18491
	private List<Text> textCollection = new List<Text>();

	// Token: 0x0400483C RID: 18492
	private string cachedText;

	// Token: 0x0400483D RID: 18493
	private LegalAgreementBodyText.State state;

	// Token: 0x0200095E RID: 2398
	private enum State
	{
		// Token: 0x0400483F RID: 18495
		Ready,
		// Token: 0x04004840 RID: 18496
		Loading,
		// Token: 0x04004841 RID: 18497
		Error
	}
}
