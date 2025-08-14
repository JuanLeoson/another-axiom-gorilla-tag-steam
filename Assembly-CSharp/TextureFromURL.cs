using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

// Token: 0x02000B2A RID: 2858
public class TextureFromURL : MonoBehaviour
{
	// Token: 0x060044C9 RID: 17609 RVA: 0x0015760E File Offset: 0x0015580E
	private void OnEnable()
	{
		if (this.data.Length == 0)
		{
			return;
		}
		if (this.source == TextureFromURL.Source.TitleData)
		{
			this.LoadFromTitleData();
			return;
		}
		this.applyRemoteTexture(this.data);
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0015763C File Offset: 0x0015583C
	private void LoadFromTitleData()
	{
		TextureFromURL.<LoadFromTitleData>d__7 <LoadFromTitleData>d__;
		<LoadFromTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadFromTitleData>d__.<>4__this = this;
		<LoadFromTitleData>d__.<>1__state = -1;
		<LoadFromTitleData>d__.<>t__builder.Start<TextureFromURL.<LoadFromTitleData>d__7>(ref <LoadFromTitleData>d__);
	}

	// Token: 0x060044CB RID: 17611 RVA: 0x00157673 File Offset: 0x00155873
	private void OnDisable()
	{
		if (this.texture != null)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x060044CC RID: 17612 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnPlayFabError(PlayFabError error)
	{
	}

	// Token: 0x060044CD RID: 17613 RVA: 0x00157698 File Offset: 0x00155898
	private void OnTitleDataRequestComplete(string imageUrl)
	{
		imageUrl = imageUrl.Replace("\\r", "\r").Replace("\\n", "\n");
		if (imageUrl[0] == '"' && imageUrl[imageUrl.Length - 1] == '"')
		{
			imageUrl = imageUrl.Substring(1, imageUrl.Length - 2);
		}
		this.applyRemoteTexture(imageUrl);
	}

	// Token: 0x060044CE RID: 17614 RVA: 0x001576FC File Offset: 0x001558FC
	private void applyRemoteTexture(string imageUrl)
	{
		TextureFromURL.<applyRemoteTexture>d__11 <applyRemoteTexture>d__;
		<applyRemoteTexture>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<applyRemoteTexture>d__.<>4__this = this;
		<applyRemoteTexture>d__.imageUrl = imageUrl;
		<applyRemoteTexture>d__.<>1__state = -1;
		<applyRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<applyRemoteTexture>d__11>(ref <applyRemoteTexture>d__);
	}

	// Token: 0x060044CF RID: 17615 RVA: 0x0015773C File Offset: 0x0015593C
	private Task<Texture2D> GetRemoteTexture(string url)
	{
		TextureFromURL.<GetRemoteTexture>d__12 <GetRemoteTexture>d__;
		<GetRemoteTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture2D>.Create();
		<GetRemoteTexture>d__.url = url;
		<GetRemoteTexture>d__.<>1__state = -1;
		<GetRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<GetRemoteTexture>d__12>(ref <GetRemoteTexture>d__);
		return <GetRemoteTexture>d__.<>t__builder.Task;
	}

	// Token: 0x04004EEF RID: 20207
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04004EF0 RID: 20208
	[SerializeField]
	private TextureFromURL.Source source;

	// Token: 0x04004EF1 RID: 20209
	[Tooltip("If Source is set to 'TitleData' Data should be the id of the title data entry that defines an image URL. If Source is set to 'URL' Data should be a URL that points to an image.")]
	[SerializeField]
	private string data;

	// Token: 0x04004EF2 RID: 20210
	private Texture2D texture;

	// Token: 0x04004EF3 RID: 20211
	private int maxTitleDataAttempts = 10;

	// Token: 0x02000B2B RID: 2859
	private enum Source
	{
		// Token: 0x04004EF5 RID: 20213
		TitleData,
		// Token: 0x04004EF6 RID: 20214
		URL
	}
}
