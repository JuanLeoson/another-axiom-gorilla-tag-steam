using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DC1 RID: 3521
	public class BundleManager : MonoBehaviour
	{
		// Token: 0x06005751 RID: 22353 RVA: 0x001B12E2 File Offset: 0x001AF4E2
		private IEnumerable GetStoreBundles()
		{
			List<StoreBundleData> list = new List<StoreBundleData>();
			list.Add(this.nullBundleData);
			list.AddRange(this._bundleScriptableObjects);
			return list;
		}

		// Token: 0x06005752 RID: 22354 RVA: 0x001B1301 File Offset: 0x001AF501
		public void Awake()
		{
			if (BundleManager.instance == null)
			{
				BundleManager.instance = this;
				return;
			}
			if (BundleManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x06005753 RID: 22355 RVA: 0x001B1336 File Offset: 0x001AF536
		private void Start()
		{
			this.GenerateBundleDictionaries();
			this.Initialize();
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x001B1344 File Offset: 0x001AF544
		private void Initialize()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.InitializebundleStands();
			}
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x001B1394 File Offset: 0x001AF594
		private void ValidateBundleData()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.ValidateBundleData();
			}
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x001B13E4 File Offset: 0x001AF5E4
		private void SpawnBundleStands()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
			for (int j = 0; j < this.BundleStands.Count; j++)
			{
				if (this.BundleStands[j].spawnLocation == null)
				{
					Debug.LogError("No spawn location set for Bundle Stand " + j.ToString());
				}
				else if (this.BundleStands[j].bundleStand == null)
				{
					Debug.LogError("No Bundle Stand set for Bundle Stand " + j.ToString());
				}
			}
			this.GenerateAllStoreBundleReferences();
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton1))
			{
				this.tryOnBundleButton1 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton2))
			{
				this.tryOnBundleButton2 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton3))
			{
				this.tryOnBundleButton3 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton4))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton5))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x001B15F8 File Offset: 0x001AF7F8
		public void ClearEverything()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			this.tryOnBundleButton1 = this.nullBundleData;
			this.tryOnBundleButton2 = this.nullBundleData;
			this.tryOnBundleButton3 = this.nullBundleData;
			this.tryOnBundleButton4 = this.nullBundleData;
			this.tryOnBundleButton5 = this.nullBundleData;
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x000023F5 File Offset: 0x000005F5
		public void GenerateAllStoreBundleReferences()
		{
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x001B172C File Offset: 0x001AF92C
		private void AddNewBundleStand(BundleStand bundleStand)
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				if (storeBundle.playfabBundleID == bundleStand._bundleDataReference.playfabBundleID)
				{
					storeBundle.bundleStands.Add(bundleStand);
					return;
				}
			}
			StoreBundle storeBundle2 = new StoreBundle(bundleStand._bundleDataReference);
			storeBundle2.bundleStands.Add(bundleStand);
			this._storeBundles.Add(storeBundle2);
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x001B17C4 File Offset: 0x001AF9C4
		public void GenerateBundleDictionaries()
		{
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				this.storeBundlesById.Add(storeBundle.playfabBundleID, storeBundle);
				this.storeBundlesBySKU.Add(storeBundle.bundleSKU, storeBundle);
			}
		}

		// Token: 0x0600575B RID: 22363 RVA: 0x001B184C File Offset: 0x001AFA4C
		public void BundlePurchaseButtonPressed(string playFabItemName)
		{
			CosmeticsController.instance.PurchaseBundle(this.storeBundlesById[playFabItemName]);
		}

		// Token: 0x0600575C RID: 22364 RVA: 0x001B1868 File Offset: 0x001AFA68
		public void FixBundles()
		{
			this._storeBundles.Clear();
			for (int i = this._spawnedBundleStands.Count - 1; i >= 0; i--)
			{
				if (this._spawnedBundleStands[i].bundleStand == null)
				{
					this._spawnedBundleStands.RemoveAt(i);
				}
			}
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int j = 0; j < array.Length; j++)
			{
				BundleStand bundle = array[j];
				if (this._spawnedBundleStands.Any((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3)))
				{
					SpawnedBundle spawnedBundle = this._spawnedBundleStands.First((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3));
					if (spawnedBundle != null && spawnedBundle.bundleStand != bundle)
					{
						Object.DestroyImmediate(spawnedBundle.bundleStand.gameObject);
						spawnedBundle.bundleStand = bundle;
					}
				}
				else
				{
					this._spawnedBundleStands.Add(new SpawnedBundle
					{
						spawnLocationPath = bundle.transform.parent.gameObject.GetPath(3),
						bundleStand = bundle
					});
				}
			}
			this.GenerateAllStoreBundleReferences();
		}

		// Token: 0x0600575D RID: 22365 RVA: 0x001B1993 File Offset: 0x001AFB93
		public StoreBundleData[] GetTryOnButtons()
		{
			return new StoreBundleData[]
			{
				this.tryOnBundleButton1,
				this.tryOnBundleButton2,
				this.tryOnBundleButton3,
				this.tryOnBundleButton4,
				this.tryOnBundleButton5
			};
		}

		// Token: 0x0600575E RID: 22366 RVA: 0x001B19C8 File Offset: 0x001AFBC8
		public void NotifyBundleOfErrorByPlayFabID(string ItemId)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesById.TryGetValue(ItemId, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x0600575F RID: 22367 RVA: 0x001B1A28 File Offset: 0x001AFC28
		public void NotifyBundleOfErrorBySKU(string ItemSKU)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesBySKU.TryGetValue(ItemSKU, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06005760 RID: 22368 RVA: 0x001B1A88 File Offset: 0x001AFC88
		public void MarkBundleOwnedByPlayFabID(string ItemId)
		{
			if (this.storeBundlesById.ContainsKey(ItemId))
			{
				this.storeBundlesById[ItemId].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesById[ItemId].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06005761 RID: 22369 RVA: 0x001B1B04 File Offset: 0x001AFD04
		public void MarkBundleOwnedBySKU(string SKU)
		{
			if (this.storeBundlesBySKU.ContainsKey(SKU))
			{
				this.storeBundlesBySKU[SKU].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesBySKU[SKU].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06005762 RID: 22370 RVA: 0x001B1B80 File Offset: 0x001AFD80
		public void CheckIfBundlesOwned()
		{
			foreach (StoreBundle storeBundle in this.storeBundlesById.Values)
			{
				if (storeBundle.isOwned)
				{
					foreach (BundleStand bundleStand in storeBundle.bundleStands)
					{
						bundleStand.NotifyAlreadyOwn();
					}
				}
			}
		}

		// Token: 0x06005763 RID: 22371 RVA: 0x001B1C18 File Offset: 0x001AFE18
		public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
		{
			if (this._tryOnBundlesStand.IsNotNull())
			{
				this._tryOnBundlesStand.PressTryOnBundleButton(pressedTryOnBundleButton, isLeftHand);
			}
		}

		// Token: 0x06005764 RID: 22372 RVA: 0x001B1C34 File Offset: 0x001AFE34
		public void PressPurchaseTryOnBundleButton()
		{
			this._tryOnBundlesStand.PurchaseButtonPressed();
		}

		// Token: 0x06005765 RID: 22373 RVA: 0x001B1C41 File Offset: 0x001AFE41
		public void UpdateBundlePrice(string productSku, string productFormattedPrice)
		{
			if (this.storeBundlesBySKU.ContainsKey(productSku))
			{
				this.storeBundlesBySKU[productSku].TryUpdatePrice(productFormattedPrice);
			}
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x001B1C64 File Offset: 0x001AFE64
		public void CheckForNoPriceBundlesAndDefaultPrice()
		{
			foreach (KeyValuePair<string, StoreBundle> keyValuePair in this.storeBundlesBySKU)
			{
				string text;
				StoreBundle storeBundle;
				keyValuePair.Deconstruct(out text, out storeBundle);
				StoreBundle storeBundle2 = storeBundle;
				if (!storeBundle2.HasPrice)
				{
					storeBundle2.TryUpdatePrice(null);
				}
			}
		}

		// Token: 0x04006135 RID: 24885
		public static volatile BundleManager instance;

		// Token: 0x04006136 RID: 24886
		[FormerlySerializedAs("_TryOnBundlesStand")]
		public TryOnBundlesStand _tryOnBundlesStand;

		// Token: 0x04006137 RID: 24887
		[SerializeField]
		private StoreBundleData nullBundleData;

		// Token: 0x04006138 RID: 24888
		private List<StoreBundleData> _bundleScriptableObjects = new List<StoreBundleData>();

		// Token: 0x04006139 RID: 24889
		[SerializeField]
		private List<StoreBundle> _storeBundles = new List<StoreBundle>();

		// Token: 0x0400613A RID: 24890
		[FormerlySerializedAs("_SpawnedBundleStands")]
		[SerializeField]
		private List<SpawnedBundle> _spawnedBundleStands = new List<SpawnedBundle>();

		// Token: 0x0400613B RID: 24891
		public Dictionary<string, StoreBundle> storeBundlesById = new Dictionary<string, StoreBundle>();

		// Token: 0x0400613C RID: 24892
		public Dictionary<string, StoreBundle> storeBundlesBySKU = new Dictionary<string, StoreBundle>();

		// Token: 0x0400613D RID: 24893
		[Header("Enable Advanced Search window in your settings to easily see all bundle prefabs")]
		[SerializeField]
		private List<BundleManager.BundleStandSpawn> BundleStands = new List<BundleManager.BundleStandSpawn>();

		// Token: 0x0400613E RID: 24894
		[SerializeField]
		private StoreBundleData tryOnBundleButton1;

		// Token: 0x0400613F RID: 24895
		[SerializeField]
		private StoreBundleData tryOnBundleButton2;

		// Token: 0x04006140 RID: 24896
		[SerializeField]
		private StoreBundleData tryOnBundleButton3;

		// Token: 0x04006141 RID: 24897
		[SerializeField]
		private StoreBundleData tryOnBundleButton4;

		// Token: 0x04006142 RID: 24898
		[SerializeField]
		private StoreBundleData tryOnBundleButton5;

		// Token: 0x02000DC2 RID: 3522
		[Serializable]
		public class BundleStandSpawn
		{
			// Token: 0x06005768 RID: 22376 RVA: 0x001B1D25 File Offset: 0x001AFF25
			private static IEnumerable GetEndCapSpawnPoints()
			{
				return from x in Object.FindObjectsOfType<EndCapSpawnPoint>()
				select new ValueDropdownItem(string.Concat(new string[]
				{
					x.transform.parent.parent.name,
					"/",
					x.transform.parent.name,
					"/",
					x.name
				}), x);
			}

			// Token: 0x04006143 RID: 24899
			public EndCapSpawnPoint spawnLocation;

			// Token: 0x04006144 RID: 24900
			public BundleStand bundleStand;
		}
	}
}
