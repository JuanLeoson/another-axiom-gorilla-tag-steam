using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C00 RID: 3072
	public class BuilderFactory : MonoBehaviour
	{
		// Token: 0x06004ABB RID: 19131 RVA: 0x0016AF65 File Offset: 0x00169165
		private void Awake()
		{
			this.InitIfNeeded();
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x0016AF70 File Offset: 0x00169170
		public void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.buildItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnBuildItem));
			this.currPieceTypeIndex = 0;
			this.prevItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevItem));
			this.nextItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextItem));
			this.currPieceMaterialIndex = 0;
			this.prevMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevMaterial));
			this.nextMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextMaterial));
			this.pieceTypeToIndex = new Dictionary<int, int>(256);
			this.initialized = true;
			if (this.resourceCostUIs != null)
			{
				for (int i = 0; i < this.resourceCostUIs.Count; i++)
				{
					if (this.resourceCostUIs[i] != null)
					{
						this.resourceCostUIs[i].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x0016B068 File Offset: 0x00169268
		public void Setup(BuilderTable tableOwner)
		{
			this.table = tableOwner;
			this.InitIfNeeded();
			List<BuilderPiece> list = this.pieceList;
			this.pieceTypes = new List<int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string name = list[i].name;
				int staticHash = name.GetStaticHash();
				int num;
				if (this.pieceTypeToIndex.TryAdd(staticHash, i))
				{
					this.pieceTypes.Add(staticHash);
				}
				else if (this.pieceTypeToIndex.TryGetValue(staticHash, out num))
				{
					string text = "BuilderFactory: ERROR!! " + string.Format("Could not add pieceType \"{0}\" with hash {1} ", name, staticHash) + "to 'pieceTypeToIndex' Dictionary because because it was already added!";
					if (num < 0 || num >= list.Count)
					{
						text += " Also the index to the conflicting piece is out of range of the pieceList!";
					}
					else
					{
						BuilderPiece builderPiece = list[num];
						if (builderPiece != null)
						{
							if (name == builderPiece.name)
							{
								text += "The conflicting piece has the same name (as expected).";
							}
							else
							{
								text = text + "Also the conflicting pieceType has the same hash but different name \"" + builderPiece.name + "\"!";
							}
						}
						else
						{
							text += "And (should never happen) the piece at that slot is null!";
						}
					}
					Debug.LogError(text, this);
				}
			}
			int num2 = this.pieceTypes.Count;
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash2 = pieceInfo.piecePrefab.name.GetStaticHash();
						if (!this.pieceTypeToIndex.ContainsKey(staticHash2))
						{
							this.pieceList.Add(pieceInfo.piecePrefab);
							this.pieceTypes.Add(staticHash2);
							this.pieceTypeToIndex.Add(staticHash2, num2);
							num2++;
						}
					}
				}
			}
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x0016B2CC File Offset: 0x001694CC
		public void Show()
		{
			this.RefreshUI();
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x0016B2D4 File Offset: 0x001694D4
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			int index;
			if (this.pieceTypeToIndex.TryGetValue(pieceType, out index))
			{
				return this.pieceList[index];
			}
			Debug.LogErrorFormat("No Prefab found for type {0}", new object[]
			{
				pieceType
			});
			return null;
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x0016B318 File Offset: 0x00169518
		public void OnBuildItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > this.currPieceTypeIndex)
			{
				int selectedMaterialType = this.GetSelectedMaterialType();
				this.table.RequestCreatePiece(this.pieceTypes[this.currPieceTypeIndex], this.spawnLocation.position, this.spawnLocation.rotation, selectedMaterialType);
				if (this.audioSource != null && this.buildPieceSound != null)
				{
					this.audioSource.GTPlayOneShot(this.buildPieceSound, 1f);
				}
			}
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x0016B3AC File Offset: 0x001695AC
		public void OnPrevItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex - 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x0016B42C File Offset: 0x0016962C
		public void OnNextItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex + 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x06004AC3 RID: 19139 RVA: 0x0016B4AC File Offset: 0x001696AC
		public void OnPrevMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex - 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x06004AC4 RID: 19140 RVA: 0x0016B57C File Offset: 0x0016977C
		public void OnNextMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex + 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x06004AC5 RID: 19141 RVA: 0x0016B64C File Offset: 0x0016984C
		private int GetSelectedMaterialType()
		{
			int result = -1;
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					result = materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode();
				}
			}
			return result;
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x0016B6D0 File Offset: 0x001698D0
		private string GetSelectedMaterialName()
		{
			string result = "DEFAULT";
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					result = materialOptions.options[this.currPieceMaterialIndex].materialId;
				}
			}
			return result;
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x0016B750 File Offset: 0x00169950
		public bool CanBuildPieceType(int pieceType)
		{
			BuilderPiece piecePrefab = this.GetPiecePrefab(pieceType);
			return !(piecePrefab == null) && !piecePrefab.isBuiltIntoTable;
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x0001D558 File Offset: 0x0001B758
		public bool CanUseMaterialType(int materalType)
		{
			return true;
		}

		// Token: 0x06004AC9 RID: 19145 RVA: 0x0016B77C File Offset: 0x0016997C
		public void RefreshUI()
		{
			if (this.pieceList != null && this.pieceList.Count > this.currPieceTypeIndex)
			{
				this.itemLabel.SetText(this.pieceList[this.currPieceTypeIndex].displayName, true);
			}
			else
			{
				this.itemLabel.SetText("No Items", true);
			}
			if (this.previewPiece != null)
			{
				this.table.builderPool.DestroyPiece(this.previewPiece);
				this.previewPiece = null;
			}
			if (this.currPieceTypeIndex < 0 || this.currPieceTypeIndex >= this.pieceTypes.Count)
			{
				return;
			}
			int pieceType = this.pieceTypes[this.currPieceTypeIndex];
			this.previewPiece = this.table.builderPool.CreatePiece(pieceType, false);
			this.previewPiece.SetTable(this.table);
			this.previewPiece.pieceType = pieceType;
			string selectedMaterialName = this.GetSelectedMaterialName();
			this.materialLabel.SetText(selectedMaterialName, true);
			this.previewPiece.SetScale(this.table.pieceScale * 0.75f);
			this.previewPiece.SetupPiece(this.table.gridSize);
			int selectedMaterialType = this.GetSelectedMaterialType();
			this.previewPiece.SetMaterial(selectedMaterialType, true);
			this.previewPiece.transform.SetPositionAndRotation(this.previewMarker.position, this.previewMarker.rotation);
			this.previewPiece.SetState(BuilderPiece.State.Displayed, false);
			this.previewPiece.enabled = false;
			this.RefreshCostUI();
		}

		// Token: 0x06004ACA RID: 19146 RVA: 0x0016B908 File Offset: 0x00169B08
		private void RefreshCostUI()
		{
			List<BuilderResourceQuantity> list = null;
			if (this.previewPiece != null)
			{
				list = this.previewPiece.cost.quantities;
			}
			for (int i = 0; i < this.resourceCostUIs.Count; i++)
			{
				if (!(this.resourceCostUIs[i] == null))
				{
					bool flag = list != null && i < list.Count;
					this.resourceCostUIs[i].gameObject.SetActive(flag);
					if (flag)
					{
						this.resourceCostUIs[i].SetResourceCost(list[i], this.table);
					}
				}
			}
		}

		// Token: 0x06004ACB RID: 19147 RVA: 0x0016B9A8 File Offset: 0x00169BA8
		public void OnAvailableResourcesChange()
		{
			this.RefreshCostUI();
		}

		// Token: 0x06004ACC RID: 19148 RVA: 0x0016B9B0 File Offset: 0x00169BB0
		public void CreateRandomPiece()
		{
			Debug.LogError("Create Random Piece No longer implemented");
		}

		// Token: 0x040053A4 RID: 21412
		public Transform spawnLocation;

		// Token: 0x040053A5 RID: 21413
		private List<int> pieceTypes;

		// Token: 0x040053A6 RID: 21414
		public List<GameObject> itemList;

		// Token: 0x040053A7 RID: 21415
		[HideInInspector]
		public List<BuilderPiece> pieceList;

		// Token: 0x040053A8 RID: 21416
		public BuilderOptionButton buildItemButton;

		// Token: 0x040053A9 RID: 21417
		public TextMeshPro itemLabel;

		// Token: 0x040053AA RID: 21418
		public BuilderOptionButton prevItemButton;

		// Token: 0x040053AB RID: 21419
		public BuilderOptionButton nextItemButton;

		// Token: 0x040053AC RID: 21420
		public TextMeshPro materialLabel;

		// Token: 0x040053AD RID: 21421
		public BuilderOptionButton prevMaterialButton;

		// Token: 0x040053AE RID: 21422
		public BuilderOptionButton nextMaterialButton;

		// Token: 0x040053AF RID: 21423
		public AudioSource audioSource;

		// Token: 0x040053B0 RID: 21424
		public AudioClip buildPieceSound;

		// Token: 0x040053B1 RID: 21425
		public Transform previewMarker;

		// Token: 0x040053B2 RID: 21426
		public List<BuilderUIResource> resourceCostUIs;

		// Token: 0x040053B3 RID: 21427
		private BuilderPiece previewPiece;

		// Token: 0x040053B4 RID: 21428
		private int currPieceTypeIndex;

		// Token: 0x040053B5 RID: 21429
		private int currPieceMaterialIndex;

		// Token: 0x040053B6 RID: 21430
		private Dictionary<int, int> pieceTypeToIndex;

		// Token: 0x040053B7 RID: 21431
		private BuilderTable table;

		// Token: 0x040053B8 RID: 21432
		private bool initialized;
	}
}
