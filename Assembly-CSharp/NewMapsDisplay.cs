using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using ModIO;
using TMPro;
using UnityEngine;

// Token: 0x0200083A RID: 2106
public class NewMapsDisplay : MonoBehaviour
{
	// Token: 0x060034AF RID: 13487 RVA: 0x00112768 File Offset: 0x00110968
	public void OnEnable()
	{
		if (ModIOManager.GetNewMapsModId() == ModId.Null)
		{
			return;
		}
		this.mapImage.gameObject.SetActive(false);
		this.modNameText.text = "";
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.text = "";
		this.modCreatorText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (GorillaServer.Instance == null || !GorillaServer.Instance.FeatureFlagsReady)
		{
			this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			return;
		}
		this.Initialize();
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x00112834 File Offset: 0x00110A34
	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = default(ModProfile);
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.modNameText.text = "";
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.text = "";
		this.modCreatorText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x00112901 File Offset: 0x00110B01
	private IEnumerator DelayedInitialize()
	{
		bool flag = GorillaServer.Instance != null && GorillaServer.Instance.FeatureFlagsReady;
		while (!flag)
		{
			yield return new WaitForSecondsRealtime(3f);
			flag = (GorillaServer.Instance != null && GorillaServer.Instance.FeatureFlagsReady);
		}
		this.Initialize();
		this.initCoroutine = null;
		yield break;
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x00112910 File Offset: 0x00110B10
	private void Initialize()
	{
		if (!this.requestingNewMapsModProfile && !this.downloadingImages)
		{
			ModIOManager.Initialize(delegate(ModIORequestResult result)
			{
				if (result.success)
				{
					if (!base.isActiveAndEnabled)
					{
						return;
					}
					this.requestingNewMapsModProfile = true;
					ModIOManager.GetModProfile(ModIOManager.GetNewMapsModId(), new Action<ModIORequestResultAnd<ModProfile>>(this.OnGetNewMapsModProfile));
				}
			});
		}
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x00112934 File Offset: 0x00110B34
	private void OnGetNewMapsModProfile(ModIORequestResultAnd<ModProfile> resultAndProfile)
	{
		NewMapsDisplay.<OnGetNewMapsModProfile>d__19 <OnGetNewMapsModProfile>d__;
		<OnGetNewMapsModProfile>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGetNewMapsModProfile>d__.<>4__this = this;
		<OnGetNewMapsModProfile>d__.resultAndProfile = resultAndProfile;
		<OnGetNewMapsModProfile>d__.<>1__state = -1;
		<OnGetNewMapsModProfile>d__.<>t__builder.Start<NewMapsDisplay.<OnGetNewMapsModProfile>d__19>(ref <OnGetNewMapsModProfile>d__);
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x00112973 File Offset: 0x00110B73
	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x00112997 File Offset: 0x00110B97
	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x001129BC File Offset: 0x00110BBC
	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			this.mapImage.sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.modNameText.text = this.newMapDatas[this.slideshowIndex].name;
		this.modCreatorText.text = this.newMapDatas[this.slideshowIndex].creator;
		this.modNameText.gameObject.SetActive(true);
		this.modCreatorLabelText.gameObject.SetActive(true);
		this.modCreatorText.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	// Token: 0x04004184 RID: 16772
	[SerializeField]
	private SpriteRenderer mapImage;

	// Token: 0x04004185 RID: 16773
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04004186 RID: 16774
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04004187 RID: 16775
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04004188 RID: 16776
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04004189 RID: 16777
	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	// Token: 0x0400418A RID: 16778
	private ModProfile newMapsModProfile;

	// Token: 0x0400418B RID: 16779
	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	// Token: 0x0400418C RID: 16780
	private bool slideshowActive;

	// Token: 0x0400418D RID: 16781
	private int slideshowIndex;

	// Token: 0x0400418E RID: 16782
	private float lastSlideshowUpdate;

	// Token: 0x0400418F RID: 16783
	private bool requestingNewMapsModProfile;

	// Token: 0x04004190 RID: 16784
	private bool downloadingImages;

	// Token: 0x04004191 RID: 16785
	private Coroutine initCoroutine;

	// Token: 0x0200083B RID: 2107
	private struct NewMapData
	{
		// Token: 0x04004192 RID: 16786
		public Texture2D image;

		// Token: 0x04004193 RID: 16787
		public string name;

		// Token: 0x04004194 RID: 16788
		public string creator;
	}
}
