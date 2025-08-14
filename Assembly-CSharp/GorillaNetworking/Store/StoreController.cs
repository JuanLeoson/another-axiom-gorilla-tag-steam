using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DCF RID: 3535
	public class StoreController : MonoBehaviour
	{
		// Token: 0x060057C4 RID: 22468 RVA: 0x001B3F6F File Offset: 0x001B216F
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
				return;
			}
			if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x060057C5 RID: 22469 RVA: 0x000023F5 File Offset: 0x000005F5
		public void Start()
		{
		}

		// Token: 0x060057C6 RID: 22470 RVA: 0x001B3FA4 File Offset: 0x001B21A4
		public void CreateDynamicCosmeticStandsDictionatary()
		{
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									if (!this.CosmeticStandsDict.ContainsKey(string.Concat(new string[]
									{
										storeDepartment.departmentName,
										"|",
										storeDisplay.displayName,
										"|",
										dynamicCosmeticStand.StandName
									})))
									{
										this.CosmeticStandsDict.Add(string.Concat(new string[]
										{
											storeDepartment.departmentName,
											"|",
											storeDisplay.displayName,
											"|",
											dynamicCosmeticStand.StandName
										}), dynamicCosmeticStand);
									}
									else
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											storeDepartment.departmentName,
											"|",
											storeDisplay.displayName,
											"|",
											dynamicCosmeticStand.StandName,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060057C7 RID: 22471 RVA: 0x001B4180 File Offset: 0x001B2380
		private void Create_StandsByPlayfabIDDictionary()
		{
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
			}
		}

		// Token: 0x060057C8 RID: 22472 RVA: 0x001B41E4 File Offset: 0x001B23E4
		public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
			{
				if (dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					return;
				}
				if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
				{
					this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					return;
				}
				this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand>
				{
					dynamicCosmeticStand
				});
			}
		}

		// Token: 0x060057C9 RID: 22473 RVA: 0x001B4254 File Offset: 0x001B2454
		public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			List<DynamicCosmeticStand> list;
			if (this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, out list))
			{
				list.Remove(dynamicCosmeticStand);
			}
		}

		// Token: 0x060057CA RID: 22474 RVA: 0x000023F5 File Offset: 0x000005F5
		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		// Token: 0x060057CB RID: 22475 RVA: 0x000023F5 File Offset: 0x000005F5
		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		// Token: 0x060057CC RID: 22476 RVA: 0x000023F5 File Offset: 0x000005F5
		public void ImportCosmeticStandLayout()
		{
		}

		// Token: 0x060057CD RID: 22477 RVA: 0x001B427E File Offset: 0x001B247E
		private void InitializeFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", delegate(string data)
			{
				this.ImportCosmeticStandLayoutFromTitleData(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting StoreLayoutData data: {0}", e));
			});
		}

		// Token: 0x060057CE RID: 22478 RVA: 0x001B42BC File Offset: 0x001B24BC
		private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
		{
			StandImport standImport = new StandImport();
			standImport.DecomposeFromTitleDataString(TSVData);
			foreach (StandTypeData standTypeData in standImport.standData)
			{
				string text = string.Concat(new string[]
				{
					standTypeData.departmentID,
					"|",
					standTypeData.displayID,
					"|",
					standTypeData.standID
				});
				if (this.CosmeticStandsDict.ContainsKey(text))
				{
					Debug.Log(string.Concat(new string[]
					{
						"StoreStuff: Stand Updated: ",
						standTypeData.departmentID,
						"|",
						standTypeData.displayID,
						"|",
						standTypeData.standID,
						"|",
						standTypeData.bustType,
						"|",
						standTypeData.playFabID,
						"|"
					}));
					this.CosmeticStandsDict[text].SetStandTypeString(standTypeData.bustType);
					Debug.Log("Manually Initializing Stand: " + text + " |||| " + standTypeData.playFabID);
					this.CosmeticStandsDict[text].SpawnItemOntoStand(standTypeData.playFabID);
					this.CosmeticStandsDict[text].InitializeCosmetic();
				}
			}
		}

		// Token: 0x060057CF RID: 22479 RVA: 0x001B4438 File Offset: 0x001B2638
		public void InitalizeCosmeticStands()
		{
			this.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.InitializeCosmetic();
			}
			this.Create_StandsByPlayfabIDDictionary();
			if (this.LoadFromTitleData)
			{
				this.InitializeFromTitleData();
			}
		}

		// Token: 0x060057D0 RID: 22480 RVA: 0x001B44A8 File Offset: 0x001B26A8
		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		// Token: 0x060057D1 RID: 22481 RVA: 0x001B44E0 File Offset: 0x001B26E0
		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		// Token: 0x060057D2 RID: 22482 RVA: 0x001B4564 File Offset: 0x001B2764
		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindObjectOfType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		// Token: 0x060057D3 RID: 22483 RVA: 0x001B4594 File Offset: 0x001B2794
		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		// Token: 0x060057D4 RID: 22484 RVA: 0x001B45FC File Offset: 0x001B27FC
		public void FindAllDepartments()
		{
			this.Departments = Object.FindObjectsOfType<StoreDepartment>().ToList<StoreDepartment>();
		}

		// Token: 0x060057D5 RID: 22485 RVA: 0x001B4610 File Offset: 0x001B2810
		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		// Token: 0x060057D6 RID: 22486 RVA: 0x001B4730 File Offset: 0x001B2930
		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindObjectOfType<StoreController>();
			}
			StoreController.instance.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		// Token: 0x0400618F RID: 24975
		public static volatile StoreController instance;

		// Token: 0x04006190 RID: 24976
		public List<StoreDepartment> Departments;

		// Token: 0x04006191 RID: 24977
		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		// Token: 0x04006192 RID: 24978
		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		// Token: 0x04006193 RID: 24979
		public AllCosmeticsArraySO AllCosmeticsArraySO;

		// Token: 0x04006194 RID: 24980
		public bool LoadFromTitleData;

		// Token: 0x04006195 RID: 24981
		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";
	}
}
