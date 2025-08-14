using System;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007F3 RID: 2035
public class PlayFabTitleDataTextDisplay : MonoBehaviour, IBuildValidation
{
	// Token: 0x170004CC RID: 1228
	// (get) Token: 0x060032EB RID: 13035 RVA: 0x001090A6 File Offset: 0x001072A6
	public string playFabKeyValue
	{
		get
		{
			return this.playfabKey;
		}
	}

	// Token: 0x060032EC RID: 13036 RVA: 0x001090B0 File Offset: 0x001072B0
	private void Start()
	{
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x0010912B File Offset: 0x0010732B
	private void OnPlayFabError(PlayFabError error)
	{
		if (this.textBox != null)
		{
			this.textBox.text = this.fallbackText;
		}
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x0010914C File Offset: 0x0010734C
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		if (this.textBox != null)
		{
			string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
			if (text[0] == '"' && text[text.Length - 1] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x060032EF RID: 13039 RVA: 0x001091C0 File Offset: 0x001073C0
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey && this.textBox != null)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x060032F0 RID: 13040 RVA: 0x001091EF File Offset: 0x001073EF
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x060032F1 RID: 13041 RVA: 0x0010920C File Offset: 0x0010740C
	public bool BuildValidationCheck()
	{
		if (this.textBox == null)
		{
			Debug.LogError("text reference is null! sign text will be broken");
			return false;
		}
		return true;
	}

	// Token: 0x060032F2 RID: 13042 RVA: 0x0010922C File Offset: 0x0010742C
	public void ChangeTitleDataAtRuntime(string newTitleDataKey)
	{
		this.playfabKey = newTitleDataKey;
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x04003FD9 RID: 16345
	[SerializeField]
	private TextMeshPro textBox;

	// Token: 0x04003FDA RID: 16346
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x04003FDB RID: 16347
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x04003FDC RID: 16348
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x04003FDD RID: 16349
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;
}
