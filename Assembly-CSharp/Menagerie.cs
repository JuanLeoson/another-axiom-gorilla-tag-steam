using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000079 RID: 121
public class Menagerie : MonoBehaviour
{
	// Token: 0x060002F1 RID: 753 RVA: 0x00012744 File Offset: 0x00010944
	private void Start()
	{
		CrittersCageDeposit[] array = Object.FindObjectsByType<CrittersCageDeposit>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnDepositCritter += this.OnDepositCritter;
		}
		CrittersManager.CheckInitialize();
		this._totalPages = this.critterIndex.critterTypes.Count / this.collection.Length + ((this.critterIndex.critterTypes.Count % this.collection.Length == 0) ? 0 : 1);
		this.Load();
		MenagerieDepositBox donationBox = this.DonationBox;
		donationBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(donationBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInDonationBox));
		MenagerieDepositBox favoriteBox = this.FavoriteBox;
		favoriteBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(favoriteBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInFavoriteBox));
		MenagerieDepositBox collectionBox = this.CollectionBox;
		collectionBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(collectionBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInCollectionBox));
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0001283C File Offset: 0x00010A3C
	private void CritterDepositedInDonationBox(MenagerieCritter critter)
	{
		if (this.newCritterPen.Contains(critter.Slot))
		{
			critter.currentState = MenagerieCritter.MenagerieCritterState.Donating;
			this.DonateCritter(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			PlayerGameEvents.CritterEvent("Donate" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x000128C4 File Offset: 0x00010AC4
	private void CritterDepositedInFavoriteBox(MenagerieCritter critter)
	{
		if (this.collection.Contains(critter.Slot))
		{
			this._savedCritters.favoriteCritter = critter.CritterData.critterType;
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Favorite" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00012930 File Offset: 0x00010B30
	private void CritterDepositedInCollectionBox(MenagerieCritter critter)
	{
		if (this.newCritterPen.Contains(critter.Slot))
		{
			this.AddCritterToCollection(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Collect" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x000129B8 File Offset: 0x00010BB8
	private void OnDepositCritter(Menagerie.CritterData depositedCritter, int playerID)
	{
		try
		{
			if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.AddCritterToNewCritterPen(depositedCritter);
				this.Save();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x000129F8 File Offset: 0x00010BF8
	private void AddCritterToNewCritterPen(Menagerie.CritterData critterData)
	{
		if (this._savedCritters.newCritters.Count < this.newCritterPen.Length)
		{
			foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
			{
				if (!menagerieSlot.critter)
				{
					this.SpawnCritterInSlot(menagerieSlot, critterData);
					this._savedCritters.newCritters.Add(critterData);
					return;
				}
			}
		}
		this.DonateCritter(critterData);
		this.Save();
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00012A6C File Offset: 0x00010C6C
	private void AddCritterToCollection(Menagerie.CritterData critterData)
	{
		Menagerie.CritterData critterData2;
		if (this._savedCritters.collectedCritters.TryGetValue(critterData.critterType, out critterData2))
		{
			this.DonateCritter(critterData2);
		}
		this._savedCritters.collectedCritters[critterData.critterType] = critterData;
		this.SpawnCollectionCritterIfShowing(critterData);
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00012AB8 File Offset: 0x00010CB8
	private void DonateCritter(Menagerie.CritterData critterData)
	{
		this._savedCritters.donatedCritterCount++;
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount), true);
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00012AF4 File Offset: 0x00010CF4
	private void SpawnCritterInSlot(MenagerieSlot slot, Menagerie.CritterData critterData)
	{
		if (slot.IsNull() || critterData == null)
		{
			return;
		}
		this.DespawnCritterFromSlot(slot);
		MenagerieCritter menagerieCritter = Object.Instantiate<MenagerieCritter>(this.prefab, slot.critterMountPoint);
		menagerieCritter.Slot = slot;
		menagerieCritter.ApplyCritterData(critterData);
		this._critters.Add(menagerieCritter);
		if (slot.label)
		{
			slot.label.text = this.critterIndex[critterData.critterType].critterName;
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00012B70 File Offset: 0x00010D70
	private void SpawnCollectionCritterIfShowing(Menagerie.CritterData critter)
	{
		int num = critter.critterType - this._collectionPageIndex * this.collection.Length;
		if (num < 0 || num >= this.collection.Length)
		{
			return;
		}
		this.SpawnCritterInSlot(this.collection[num], critter);
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00012BB3 File Offset: 0x00010DB3
	private void UpdateMenagerie()
	{
		this.UpdateNewCritterPen();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount), true);
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00012BF0 File Offset: 0x00010DF0
	private void UpdateNewCritterPen()
	{
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			if (i < this._savedCritters.newCritters.Count)
			{
				this.SpawnCritterInSlot(this.newCritterPen[i], this._savedCritters.newCritters[i]);
			}
			else
			{
				this.DespawnCritterFromSlot(this.newCritterPen[i]);
			}
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00012C54 File Offset: 0x00010E54
	private void UpdateCollection()
	{
		int num = this._collectionPageIndex * this.collection.Length;
		for (int i = 0; i < this.collection.Length; i++)
		{
			int num2 = num + i;
			MenagerieSlot menagerieSlot = this.collection[i];
			Menagerie.CritterData critterData;
			if (this._savedCritters.collectedCritters.TryGetValue(num2, out critterData))
			{
				this.SpawnCritterInSlot(menagerieSlot, critterData);
			}
			else
			{
				this.DespawnCritterFromSlot(menagerieSlot);
				CritterConfiguration critterConfiguration = this.critterIndex[num2];
				menagerieSlot.label.text = ((critterConfiguration == null) ? "" : "??????");
			}
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00012CE4 File Offset: 0x00010EE4
	private void UpdateFavoriteCritter()
	{
		Menagerie.CritterData critterData;
		if (this._savedCritters.collectedCritters.TryGetValue(this._savedCritters.favoriteCritter, out critterData))
		{
			this.SpawnCritterInSlot(this.favoriteCritterSlot, critterData);
			return;
		}
		this.ClearSlot(this.favoriteCritterSlot);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00012D2A File Offset: 0x00010F2A
	public void NextGroupCollectedCritters()
	{
		this._collectionPageIndex++;
		if (this._collectionPageIndex >= this._totalPages)
		{
			this._collectionPageIndex = 0;
		}
		this.UpdateCollection();
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00012D55 File Offset: 0x00010F55
	public void PrevGroupCollectedCritters()
	{
		this._collectionPageIndex--;
		if (this._collectionPageIndex < 0)
		{
			this._collectionPageIndex = this._totalPages - 1;
		}
		this.UpdateCollection();
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00012D82 File Offset: 0x00010F82
	private void GenerateNewCritters()
	{
		this.GenerateNewCritterCount(Random.Range(Mathf.Min(1, this.newCritterPen.Length), this.newCritterPen.Length + 1));
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00012DA8 File Offset: 0x00010FA8
	private void GenerateLegalNewCritters()
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			int randomCritterType = this.critterIndex.GetRandomCritterType(null);
			if (randomCritterType < 0)
			{
				Debug.LogError("Failed to spawn valid critter. No critter configuration found.");
				return;
			}
			Menagerie.CritterData critterData = new Menagerie.CritterData(randomCritterType, this.critterIndex[randomCritterType].GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00012E0C File Offset: 0x0001100C
	private void GenerateNewCritterCount(int critterCount)
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < critterCount; i++)
		{
			int num = Random.Range(0, this.critterIndex.critterTypes.Count);
			CritterConfiguration critterConfiguration = this.critterIndex[num];
			Menagerie.CritterData critterData = new Menagerie.CritterData(num, critterConfiguration.GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00012E64 File Offset: 0x00011064
	private void GenerateCollectedCritters(float spawnChance)
	{
		this.ClearCollection();
		for (int i = 0; i < this.critterIndex.critterTypes.Count; i++)
		{
			if (Random.value <= spawnChance)
			{
				CritterConfiguration critterConfiguration = this.critterIndex[i];
				Menagerie.CritterData critterData = new Menagerie.CritterData(i, critterConfiguration.GenerateAppearance());
				this.AddCritterToCollection(critterData);
				critterData.instance;
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00012EC8 File Offset: 0x000110C8
	private void MoveNewCrittersToCollection()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInCollectionBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00012F08 File Offset: 0x00011108
	private void DonateNewCritters()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInDonationBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00012F47 File Offset: 0x00011147
	private void ClearSlot(MenagerieSlot slot)
	{
		this.DespawnCritterFromSlot(slot);
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00012F70 File Offset: 0x00011170
	private void DespawnCritterFromSlot(MenagerieSlot slot)
	{
		if (slot.IsNull())
		{
			return;
		}
		if (!slot.critter)
		{
			return;
		}
		this._critters.Remove(slot.critter);
		Object.Destroy(slot.critter.gameObject);
		slot.critter = null;
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00012FDA File Offset: 0x000111DA
	private void ClearNewCritterPen()
	{
		this._savedCritters.newCritters.Clear();
		this.UpdateNewCritterPen();
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00012FF2 File Offset: 0x000111F2
	private void ClearCollection()
	{
		this._savedCritters.collectedCritters.Clear();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00013010 File Offset: 0x00011210
	private void ClearAll()
	{
		this._savedCritters.Clear();
		this.UpdateMenagerie();
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00013023 File Offset: 0x00011223
	private void ResetSavedCreatures()
	{
		this.ClearAll();
		this.Save();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00013034 File Offset: 0x00011234
	private void Load()
	{
		this.ClearAll();
		string @string = PlayerPrefs.GetString("_SavedCritters", string.Empty);
		this.LoadCrittersFromJson(@string);
		this.UpdateMenagerie();
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00013064 File Offset: 0x00011264
	private void Save()
	{
		Debug.Log(string.Format("Saving {0} critters", this._critters.Count));
		string value = this.SaveCrittersToJson();
		PlayerPrefs.SetString("_SavedCritters", value);
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000130A4 File Offset: 0x000112A4
	private void LoadCrittersFromJson(string jsonString)
	{
		this._savedCritters.Clear();
		if (!string.IsNullOrEmpty(jsonString))
		{
			try
			{
				this._savedCritters = JsonConvert.DeserializeObject<Menagerie.CritterSaveData>(jsonString);
			}
			catch (Exception exception)
			{
				Debug.LogError("Unable to deserialize critters from json: " + jsonString);
				Debug.LogException(exception);
			}
		}
		this.ValidateSaveData();
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00013100 File Offset: 0x00011300
	private string SaveCrittersToJson()
	{
		this.ValidateSaveData();
		string text = JsonConvert.SerializeObject(this._savedCritters, Formatting.Indented);
		Debug.Log("Critters save to JSON: " + text);
		return text;
	}

	// Token: 0x06000311 RID: 785 RVA: 0x00013134 File Offset: 0x00011334
	private void ValidateSaveData()
	{
		if (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
		{
			Debug.LogError(string.Format("Too many new critters in CrittersSaveData ({0} vs {1}) - correcting.", this._savedCritters.newCritters.Count, this.newCritterPen.Length));
			while (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
			{
				this._savedCritters.newCritters.RemoveAt(this.newCritterPen.Length);
			}
			this.Save();
		}
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000131C8 File Offset: 0x000113C8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		MenagerieSlot[] array = this.newCritterPen;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		array = this.collection;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		Gizmos.DrawWireSphere(this.favoriteCritterSlot.critterMountPoint.position, 0.1f);
	}

	// Token: 0x0400039D RID: 925
	[FormerlySerializedAs("creatureIndex")]
	public CritterIndex critterIndex;

	// Token: 0x0400039E RID: 926
	public MenagerieCritter prefab;

	// Token: 0x0400039F RID: 927
	private List<MenagerieCritter> _critters = new List<MenagerieCritter>();

	// Token: 0x040003A0 RID: 928
	private Menagerie.CritterSaveData _savedCritters = new Menagerie.CritterSaveData();

	// Token: 0x040003A1 RID: 929
	public MenagerieSlot[] collection;

	// Token: 0x040003A2 RID: 930
	public MenagerieSlot[] newCritterPen;

	// Token: 0x040003A3 RID: 931
	public MenagerieSlot favoriteCritterSlot;

	// Token: 0x040003A4 RID: 932
	private int _collectionPageIndex;

	// Token: 0x040003A5 RID: 933
	private int _totalPages;

	// Token: 0x040003A6 RID: 934
	public MenagerieDepositBox DonationBox;

	// Token: 0x040003A7 RID: 935
	public MenagerieDepositBox FavoriteBox;

	// Token: 0x040003A8 RID: 936
	public MenagerieDepositBox CollectionBox;

	// Token: 0x040003A9 RID: 937
	public TextMeshPro donationCounter;

	// Token: 0x040003AA RID: 938
	public string DonationText = "DONATED:{0}";

	// Token: 0x040003AB RID: 939
	private const string CrittersSavePrefsKey = "_SavedCritters";

	// Token: 0x0200007A RID: 122
	public class CritterData
	{
		// Token: 0x06000314 RID: 788 RVA: 0x0001327A File Offset: 0x0001147A
		public CritterConfiguration GetConfiguration()
		{
			return CrittersManager.instance.creatureIndex[this.critterType];
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00002050 File Offset: 0x00000250
		public CritterData()
		{
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00013293 File Offset: 0x00011493
		public CritterData(CritterConfiguration config, CritterAppearance appearance)
		{
			this.critterType = CrittersManager.instance.creatureIndex.critterTypes.IndexOf(config);
			this.appearance = appearance;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000132BF File Offset: 0x000114BF
		public CritterData(int critterType, CritterAppearance appearance)
		{
			this.critterType = critterType;
			this.appearance = appearance;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000132D5 File Offset: 0x000114D5
		public CritterData(CritterVisuals visuals)
		{
			this.critterType = visuals.critterType;
			this.appearance = visuals.Appearance;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x000132F5 File Offset: 0x000114F5
		public CritterData(Menagerie.CritterData source)
		{
			this.critterType = source.critterType;
			this.appearance = source.appearance;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00013315 File Offset: 0x00011515
		public override string ToString()
		{
			return string.Format("{0} {1} [instance]", this.critterType, this.appearance);
		}

		// Token: 0x040003AC RID: 940
		public int critterType;

		// Token: 0x040003AD RID: 941
		public CritterAppearance appearance;

		// Token: 0x040003AE RID: 942
		[NonSerialized]
		public MenagerieCritter instance;
	}

	// Token: 0x0200007B RID: 123
	[Serializable]
	public class CritterSaveData
	{
		// Token: 0x0600031B RID: 795 RVA: 0x00013337 File Offset: 0x00011537
		public void Clear()
		{
			this.newCritters.Clear();
			this.collectedCritters.Clear();
			this.donatedCritterCount = 0;
			this.favoriteCritter = -1;
		}

		// Token: 0x040003AF RID: 943
		public List<Menagerie.CritterData> newCritters = new List<Menagerie.CritterData>();

		// Token: 0x040003B0 RID: 944
		public Dictionary<int, Menagerie.CritterData> collectedCritters = new Dictionary<int, Menagerie.CritterData>();

		// Token: 0x040003B1 RID: 945
		public int donatedCritterCount;

		// Token: 0x040003B2 RID: 946
		public int favoriteCritter = -1;
	}
}
