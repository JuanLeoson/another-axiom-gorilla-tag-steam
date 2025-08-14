using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;

namespace FXP
{
	// Token: 0x02000CD8 RID: 3288
	public class CosmeticItemPrefab : MonoBehaviour
	{
		// Token: 0x060051B7 RID: 20919 RVA: 0x0019700B File Offset: 0x0019520B
		private void Awake()
		{
			this.JonsAwakeCode();
		}

		// Token: 0x060051B8 RID: 20920 RVA: 0x00197014 File Offset: 0x00195214
		private void JonsAwakeCode()
		{
			this.lastUpdated = -this.updateClock;
			this.isValid = (this.goPedestal && this.goMannequin && this.goCosmeticItem && this.goCosmeticItemNameplate && this.goClock && this.goPreviewMode && this.goAttractMode && this.goPurchaseMode);
			this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			this.goAttractModeSFX = this.goAttractMode.transform.FindChildRecursive("SFXAttractMode").GetComponent<AudioSource>();
			this.goPurchaseModeSFX = this.goPurchaseMode.transform.FindChildRecursive("SFXPurchaseMode").GetComponent<AudioSource>();
			this.goAttractModeVFX = this.goAttractMode.transform.FindChildRecursive("VFXAttractMode").GetComponent<ParticleSystem>();
			this.goPurchaseModeVFX = this.goPurchaseMode.transform.FindChildRecursive("VFXPurchaseMode").GetComponent<ParticleSystem>();
			this.clockTextMesh = this.goClock.GetComponent<TextMeshPro>();
			this.clockTextMeshIsValid = (this.clockTextMesh != null);
			if (this.clockTextMeshIsValid)
			{
				this.defaultCountdownTextTemplate = this.clockTextMesh.text;
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
		}

		// Token: 0x060051B9 RID: 20921 RVA: 0x0019719D File Offset: 0x0019539D
		private void OnDisable()
		{
			if (StoreUpdater.instance != null)
			{
				this.countdownTimerCoRoutine = null;
				this.StopCountdownCoroutine();
				StoreUpdater.instance.PedestalAsleep(this);
			}
		}

		// Token: 0x060051BA RID: 20922 RVA: 0x001971C8 File Offset: 0x001953C8
		private void OnEnable()
		{
			if (this.goPreviewModeSFX == null)
			{
				this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goAttractModeSFX == null)
			{
				this.goAttractModeSFX = this.goAttractMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goPurchaseModeSFX == null)
			{
				this.goPurchaseModeSFX = this.goPurchaseMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
			if (StoreUpdater.instance != null)
			{
				StoreUpdater.instance.PedestalAwakened(this);
			}
		}

		// Token: 0x060051BB RID: 20923 RVA: 0x00197298 File Offset: 0x00195498
		public void SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode NewDisplayMode)
		{
			if (!this.isValid)
			{
				return;
			}
			if (NewDisplayMode.Equals(CosmeticItemPrefab.EDisplayMode.NULL))
			{
				return;
			}
			if (NewDisplayMode == this.currentDisplayMode)
			{
				return;
			}
			switch (NewDisplayMode)
			{
			case CosmeticItemPrefab.EDisplayMode.HIDDEN:
			{
				this.goPedestal.SetActive(false);
				this.goMannequin.SetActive(false);
				this.goCosmeticItem.SetActive(false);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				AudioSource audioSource = this.goPreviewModeSFX;
				if (audioSource != null)
				{
					audioSource.GTStop();
				}
				this.goAttractMode.SetActive(false);
				AudioSource audioSource2 = this.goAttractModeSFX;
				if (audioSource2 != null)
				{
					audioSource2.GTStop();
				}
				this.goPurchaseMode.SetActive(false);
				AudioSource audioSource3 = this.goPurchaseModeSFX;
				if (audioSource3 != null)
				{
					audioSource3.GTStop();
				}
				this.StopPreviewTimer();
				this.StopAttractTimer();
				break;
			}
			case CosmeticItemPrefab.EDisplayMode.PREVIEW:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(true);
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goPreviewMode.SetActive(true);
				this.goPreviewModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.ATTRACT:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(true);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goAttractMode.SetActive(true);
				this.goAttractModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartAttractTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.PURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(true);
				this.goPurchaseModeSFX.GTPlay();
				this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = "Purchased!";
				this.StopPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.POSTPURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.StopPreviewTimer();
				break;
			}
			this.currentDisplayMode = NewDisplayMode;
		}

		// Token: 0x060051BC RID: 20924 RVA: 0x001975E6 File Offset: 0x001957E6
		private void Update()
		{
			if (Time.time > this.lastUpdated + this.updateClock)
			{
				this.lastUpdated = Time.time;
				this.UpdateClock();
			}
		}

		// Token: 0x060051BD RID: 20925 RVA: 0x00197610 File Offset: 0x00195810
		private void UpdateClock()
		{
			if (this.currentUpdateEvent != null && this.clockTextMeshIsValid && this.clockTextMesh.isActiveAndEnabled)
			{
				TimeSpan ts = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				this.clockTextMesh.text = CountdownText.GetTimeDisplay(ts, this.defaultCountdownTextTemplate);
			}
		}

		// Token: 0x060051BE RID: 20926 RVA: 0x00197674 File Offset: 0x00195874
		public void SetDefaultProperties()
		{
			if (!this.isValid)
			{
				return;
			}
			this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.defaultPedestalMesh;
			this.goPedestal.GetComponent<MeshRenderer>().sharedMaterial = this.defaultPedestalMaterial;
			this.goMannequin.GetComponent<MeshFilter>().sharedMesh = this.defaultMannequinMesh;
			this.goMannequin.GetComponent<MeshRenderer>().sharedMaterial = this.defaultMannequinMaterial;
			this.goCosmeticItem.GetComponent<MeshFilter>().sharedMesh = this.defaultCosmeticMesh;
			this.goCosmeticItem.GetComponent<MeshRenderer>().sharedMaterial = this.defaultCosmeticMaterial;
			this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = this.defaultItemText;
			this.goPreviewModeSFX.clip = this.defaultSFXPreviewMode;
			this.goAttractModeSFX.clip = this.defaultSFXAttractMode;
			this.goPurchaseModeSFX.clip = this.defaultSFXPurchaseMode;
		}

		// Token: 0x060051BF RID: 20927 RVA: 0x00197757 File Offset: 0x00195957
		private void ClearCosmeticMesh()
		{
			Object.Destroy(this.goCosmeticItemGameObject);
		}

		// Token: 0x060051C0 RID: 20928 RVA: 0x00197764 File Offset: 0x00195964
		private void ClearCosmeticAtlas()
		{
			if (this.goCosmeticItemMeshAtlas.IsNotNull())
			{
				Object.Destroy(this.goCosmeticItemMeshAtlas);
			}
		}

		// Token: 0x060051C1 RID: 20929 RVA: 0x00197780 File Offset: 0x00195980
		public void SetCosmeticItemFromCosmeticController(CosmeticsController.CosmeticItem item)
		{
			if (!this.isValid)
			{
				return;
			}
			this.ClearCosmeticAtlas();
			this.ClearCosmeticMesh();
			this.oldItemID = this.itemID;
			this.itemID = item.itemName;
			this.itemName = item.displayName;
			if (item.overrideDisplayName != string.Empty)
			{
				this.itemName = item.overrideDisplayName;
			}
			this.HeadModel.SetCosmeticActive(this.itemID, false);
			this.SetCosmeticStand();
		}

		// Token: 0x060051C2 RID: 20930 RVA: 0x001977FC File Offset: 0x001959FC
		public void SetCosmeticStand()
		{
			this.cosmeticStand.thisCosmeticName = this.itemID;
			this.cosmeticStand.InitializeCosmetic();
			if (this.oldItemID.Length > 0)
			{
				if (this.oldItemID != this.itemID)
				{
					this.cosmeticStand.isOn = false;
				}
				this.cosmeticStand.UpdateColor();
			}
		}

		// Token: 0x060051C3 RID: 20931 RVA: 0x00197860 File Offset: 0x00195A60
		public void SetStoreUpdateEvent(StoreUpdateEvent storeUpdateEvent, bool playFX)
		{
			if (!this.isValid)
			{
				return;
			}
			if (playFX)
			{
				this.goAttractMode.SetActive(true);
				this.goAttractModeVFX.Play();
			}
			this.currentUpdateEvent = storeUpdateEvent;
			this.SetCosmeticItemFromCosmeticController(CosmeticsController.instance.GetItemFromDict(storeUpdateEvent.ItemName));
			if (base.isActiveAndEnabled)
			{
				this.countdownTimerCoRoutine = base.StartCoroutine(this.PlayCountdownTimer());
			}
			this.UpdateClock();
		}

		// Token: 0x060051C4 RID: 20932 RVA: 0x001978CF File Offset: 0x00195ACF
		private IEnumerator PlayCountdownTimer()
		{
			yield return new WaitForSeconds(Mathf.Clamp((float)((this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted).TotalSeconds - 10.0), 0f, float.MaxValue));
			this.PlaySFX();
			yield break;
		}

		// Token: 0x060051C5 RID: 20933 RVA: 0x001978DE File Offset: 0x00195ADE
		public void StopCountdownCoroutine()
		{
			this.CountdownSFX.GTStop();
			this.goAttractModeVFX.Stop();
			if (this.countdownTimerCoRoutine != null)
			{
				base.StopCoroutine(this.countdownTimerCoRoutine);
				this.countdownTimerCoRoutine = null;
			}
		}

		// Token: 0x060051C6 RID: 20934 RVA: 0x00197914 File Offset: 0x00195B14
		private void PlaySFX()
		{
			if (this.currentUpdateEvent != null)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				if (timeSpan.TotalSeconds >= 10.0)
				{
					this.CountdownSFX.time = 0f;
					this.CountdownSFX.GTPlay();
					return;
				}
				this.CountdownSFX.time = 10f - (float)timeSpan.TotalSeconds;
				this.CountdownSFX.GTPlay();
			}
		}

		// Token: 0x060051C7 RID: 20935 RVA: 0x001979A0 File Offset: 0x00195BA0
		public void SetCosmeticItemProperties(string WhichGUID, string Name, List<Transform> SocketsList, int Socket, string PedestalMesh = null, string MannequinMesh = null)
		{
			if (!this.isValid)
			{
				return;
			}
			Guid guid;
			if (!Guid.TryParse(WhichGUID, out guid))
			{
				return;
			}
			this.itemName = Name;
			this.itemSocket = Socket;
			if (this.pedestalMesh != null)
			{
				this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.pedestalMesh;
			}
		}

		// Token: 0x060051C8 RID: 20936 RVA: 0x001979F4 File Offset: 0x00195BF4
		private void StartPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.coroutinePreviewTimer = this.DoPreviewTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInPreviewMode ?? this.defaultHoursInPreviewMode) * 60 * 60)));
			base.StartCoroutine(this.coroutinePreviewTimer);
		}

		// Token: 0x060051C9 RID: 20937 RVA: 0x00197A73 File Offset: 0x00195C73
		private void StopPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.clockTextMesh.text = "Clock";
		}

		// Token: 0x060051CA RID: 20938 RVA: 0x00197AA9 File Offset: 0x00195CA9
		private IEnumerator DoPreviewTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.clockTextMesh.text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.ATTRACT);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x060051CB RID: 20939 RVA: 0x00197AC0 File Offset: 0x00195CC0
		public void StartAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.coroutineAttractTimer = this.DoAttractTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInAttractMode ?? this.defaultHoursInAttractMode) * 60 * 60)));
			base.StartCoroutine(this.coroutineAttractTimer);
		}

		// Token: 0x060051CC RID: 20940 RVA: 0x00197B3F File Offset: 0x00195D3F
		private void StopAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.goClock.GetComponent<TextMesh>().text = "Clock";
		}

		// Token: 0x060051CD RID: 20941 RVA: 0x00197B7A File Offset: 0x00195D7A
		private IEnumerator DoAttractTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.goClock.GetComponent<TextMesh>().text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.HIDDEN);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x04005B2D RID: 23341
		public string PedestalID = "";

		// Token: 0x04005B2E RID: 23342
		public HeadModel HeadModel;

		// Token: 0x04005B2F RID: 23343
		[SerializeField]
		private Guid? itemGUID;

		// Token: 0x04005B30 RID: 23344
		[SerializeField]
		private string itemName = string.Empty;

		// Token: 0x04005B31 RID: 23345
		[SerializeField]
		private List<Transform> sockets = new List<Transform>();

		// Token: 0x04005B32 RID: 23346
		[SerializeField]
		private int itemSocket = int.MinValue;

		// Token: 0x04005B33 RID: 23347
		[SerializeField]
		private int? hoursInPreviewMode;

		// Token: 0x04005B34 RID: 23348
		[SerializeField]
		private int? hoursInAttractMode;

		// Token: 0x04005B35 RID: 23349
		[SerializeField]
		private Mesh pedestalMesh;

		// Token: 0x04005B36 RID: 23350
		[SerializeField]
		private Mesh mannequinMesh;

		// Token: 0x04005B37 RID: 23351
		[SerializeField]
		private Mesh cosmeticMesh;

		// Token: 0x04005B38 RID: 23352
		[SerializeField]
		private AudioClip sfxPreviewMode;

		// Token: 0x04005B39 RID: 23353
		[SerializeField]
		private AudioClip sfxAttractMode;

		// Token: 0x04005B3A RID: 23354
		[SerializeField]
		private AudioClip sfxPurchaseMode;

		// Token: 0x04005B3B RID: 23355
		[SerializeField]
		private ParticleSystem vfxPreviewMode;

		// Token: 0x04005B3C RID: 23356
		[SerializeField]
		private ParticleSystem vfxAttractMode;

		// Token: 0x04005B3D RID: 23357
		[SerializeField]
		private ParticleSystem vfxPurchaseMode;

		// Token: 0x04005B3E RID: 23358
		[SerializeField]
		private GameObject goPedestal;

		// Token: 0x04005B3F RID: 23359
		[SerializeField]
		private GameObject goMannequin;

		// Token: 0x04005B40 RID: 23360
		[SerializeField]
		private GameObject goCosmeticItem;

		// Token: 0x04005B41 RID: 23361
		[SerializeField]
		private GameObject goCosmeticItemGameObject;

		// Token: 0x04005B42 RID: 23362
		[SerializeField]
		private GameObject goCosmeticItemNameplate;

		// Token: 0x04005B43 RID: 23363
		[SerializeField]
		private GameObject goClock;

		// Token: 0x04005B44 RID: 23364
		[SerializeField]
		private GameObject goPreviewMode;

		// Token: 0x04005B45 RID: 23365
		[SerializeField]
		private GameObject goAttractMode;

		// Token: 0x04005B46 RID: 23366
		[SerializeField]
		private GameObject goPurchaseMode;

		// Token: 0x04005B47 RID: 23367
		[SerializeField]
		private Mesh defaultPedestalMesh;

		// Token: 0x04005B48 RID: 23368
		[SerializeField]
		private Material defaultPedestalMaterial;

		// Token: 0x04005B49 RID: 23369
		[SerializeField]
		private Mesh defaultMannequinMesh;

		// Token: 0x04005B4A RID: 23370
		[SerializeField]
		private Material defaultMannequinMaterial;

		// Token: 0x04005B4B RID: 23371
		[SerializeField]
		private Mesh defaultCosmeticMesh;

		// Token: 0x04005B4C RID: 23372
		[SerializeField]
		private Material defaultCosmeticMaterial;

		// Token: 0x04005B4D RID: 23373
		[SerializeField]
		private string defaultItemText;

		// Token: 0x04005B4E RID: 23374
		[SerializeField]
		private int defaultHoursInPreviewMode;

		// Token: 0x04005B4F RID: 23375
		[SerializeField]
		private int defaultHoursInAttractMode;

		// Token: 0x04005B50 RID: 23376
		[SerializeField]
		private AudioClip defaultSFXPreviewMode;

		// Token: 0x04005B51 RID: 23377
		[SerializeField]
		private AudioClip defaultSFXAttractMode;

		// Token: 0x04005B52 RID: 23378
		[SerializeField]
		private AudioClip defaultSFXPurchaseMode;

		// Token: 0x04005B53 RID: 23379
		private GameObject goCosmeticItemMeshAtlas;

		// Token: 0x04005B54 RID: 23380
		public AudioSource CountdownSFX;

		// Token: 0x04005B55 RID: 23381
		private CosmeticItemPrefab.EDisplayMode currentDisplayMode;

		// Token: 0x04005B56 RID: 23382
		private bool isValid;

		// Token: 0x04005B57 RID: 23383
		[Nullable(2)]
		private AudioSource goPreviewModeSFX;

		// Token: 0x04005B58 RID: 23384
		[Nullable(2)]
		private AudioSource goAttractModeSFX;

		// Token: 0x04005B59 RID: 23385
		[Nullable(2)]
		private AudioSource goPurchaseModeSFX;

		// Token: 0x04005B5A RID: 23386
		[Nullable(2)]
		private ParticleSystem goAttractModeVFX;

		// Token: 0x04005B5B RID: 23387
		[Nullable(2)]
		private ParticleSystem goPurchaseModeVFX;

		// Token: 0x04005B5C RID: 23388
		private IEnumerator coroutinePreviewTimer;

		// Token: 0x04005B5D RID: 23389
		private IEnumerator coroutineAttractTimer;

		// Token: 0x04005B5E RID: 23390
		private DateTime startTime;

		// Token: 0x04005B5F RID: 23391
		private TextMeshPro clockTextMesh;

		// Token: 0x04005B60 RID: 23392
		private bool clockTextMeshIsValid;

		// Token: 0x04005B61 RID: 23393
		private StoreUpdateEvent currentUpdateEvent;

		// Token: 0x04005B62 RID: 23394
		private string defaultCountdownTextTemplate = "";

		// Token: 0x04005B63 RID: 23395
		public CosmeticStand cosmeticStand;

		// Token: 0x04005B64 RID: 23396
		public string itemID = "";

		// Token: 0x04005B65 RID: 23397
		public string oldItemID = "";

		// Token: 0x04005B66 RID: 23398
		private Coroutine countdownTimerCoRoutine;

		// Token: 0x04005B67 RID: 23399
		private float updateClock = 60f;

		// Token: 0x04005B68 RID: 23400
		private float lastUpdated;

		// Token: 0x02000CD9 RID: 3289
		[SerializeField]
		public enum EDisplayMode
		{
			// Token: 0x04005B6A RID: 23402
			NULL,
			// Token: 0x04005B6B RID: 23403
			HIDDEN,
			// Token: 0x04005B6C RID: 23404
			PREVIEW,
			// Token: 0x04005B6D RID: 23405
			ATTRACT,
			// Token: 0x04005B6E RID: 23406
			PURCHASE,
			// Token: 0x04005B6F RID: 23407
			POSTPURCHASE
		}
	}
}
