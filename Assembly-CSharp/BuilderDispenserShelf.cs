using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000525 RID: 1317
public class BuilderDispenserShelf : MonoBehaviour
{
	// Token: 0x0600200B RID: 8203 RVA: 0x000A94F3 File Offset: 0x000A76F3
	private void BuildDispenserPool()
	{
		this.dispenserPool = new List<BuilderDispenser>(12);
		this.activeDispensers = new List<BuilderDispenser>(6);
		this.AddToDispenserPool(6);
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000A9518 File Offset: 0x000A7718
	private void AddToDispenserPool(int count)
	{
		if (this.dispenserPrefab == null)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BuilderDispenser builderDispenser = Object.Instantiate<BuilderDispenser>(this.dispenserPrefab, this.shelfCenter);
			builderDispenser.gameObject.SetActive(false);
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
			this.dispenserPool.Add(builderDispenser);
		}
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x000A9584 File Offset: 0x000A7784
	private void ActivateDispensers()
	{
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		int count = this.piecesInSet.Count;
		if (this.dispenserPool.Count < count)
		{
			this.AddToDispenserPool(count - this.dispenserPool.Count);
		}
		this.activeDispensers.Clear();
		for (int i = 0; i < this.dispenserPool.Count; i++)
		{
			if (i < count)
			{
				BuilderDispenser builderDispenser = this.dispenserPool[i];
				builderDispenser.gameObject.SetActive(true);
				float x = this.shelfWidth / -2f + this.shelfWidth / (float)(count * 2) + this.shelfWidth / (float)count * (float)i;
				builderDispenser.transform.localPosition = new Vector3(x, 0f, 0f);
				builderDispenser.AssignPieceType(this.piecesInSet[i], this.currentSet.materialId.GetHashCode());
				this.activeDispensers.Add(builderDispenser);
			}
			else
			{
				this.dispenserPool[i].ClearDispenser();
				this.dispenserPool[i].gameObject.SetActive(false);
			}
		}
		this.dispenserToUpdate = 0;
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x000A9730 File Offset: 0x000A7930
	public void Setup()
	{
		this.InitIfNeeded();
		foreach (BuilderDispenser builderDispenser in this.dispenserPool)
		{
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
		}
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000A9798 File Offset: 0x000A7998
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.setSelector.Setup(this._includedCategories);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.setSelector.OnSelectedSet.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.BuildDispenserPool();
		this.ActivateDispensers();
		this.initialized = true;
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000A97FF File Offset: 0x000A79FF
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedSet.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000A982B File Offset: 0x000A7A2B
	public void OnSelectedSetChange(int setId)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, setId, false);
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000A9850 File Offset: 0x000A7A50
	public void SetSelection(int setId)
	{
		this.setSelector.SetSelection(setId);
		BuilderPieceSet selectedSet = this.setSelector.GetSelectedSet();
		if ((this.initialized && this.currentSet == null) || selectedSet.setName != this.currentSet.setName)
		{
			this.currentSet = selectedSet;
			if (this.table.GetTableState() == BuilderTable.TableState.Ready)
			{
				if (!this.animatingShelf)
				{
					this.StartShelfSwap();
					return;
				}
			}
			else
			{
				this.animatingShelf = false;
				this.ImmediateShelfSwap();
			}
		}
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000A98D4 File Offset: 0x000A7AD4
	public int GetSelectedSetID()
	{
		return this.setSelector.GetSelectedSet().GetIntIdentifier();
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x000A98E8 File Offset: 0x000A7AE8
	private void ImmediateShelfSwap()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
		this.ActivateDispensers();
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x000A9940 File Offset: 0x000A7B40
	private void StartShelfSwap()
	{
		this.dispenserToClear = 0;
		this.timeToClearShelf = (double)(Time.time + 0.15f);
		this.resetAnimation.Rewind();
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ParentPieceToShelf(this.resetAnimation.transform);
		}
		this.resetAnimation.Play();
		this.animatingShelf = true;
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x000A99D4 File Offset: 0x000A7BD4
	public void UpdateShelf()
	{
		if (this.animatingShelf && (double)Time.time > this.timeToClearShelf)
		{
			if (this.dispenserToClear < this.activeDispensers.Count)
			{
				if (this.dispenserToClear == 0)
				{
					this.resetSoundBank.Play();
				}
				this.activeDispensers[this.dispenserToClear].ClearDispenser();
				this.dispenserToClear++;
				return;
			}
			if (!this.resetAnimation.isPlaying)
			{
				this.playSpawnSetSound = true;
				this.ActivateDispensers();
				this.animatingShelf = false;
			}
		}
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000A9A64 File Offset: 0x000A7C64
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (this.animatingShelf)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet selectedSet = this.setSelector.GetSelectedSet();
			if (selectedSet == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedSet.GetIntIdentifier()))
			{
				int defaultSetID = this.setSelector.GetDefaultSetID();
				if (defaultSetID != -1)
				{
					this.OnSelectedSetChange(defaultSetID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.activeDispensers.Count > 0)
		{
			this.activeDispensers[this.dispenserToUpdate].UpdateDispenser();
			this.dispenserToUpdate = (this.dispenserToUpdate + 1) % this.activeDispensers.Count;
		}
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000A9B1D File Offset: 0x000A7D1D
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000A9B28 File Offset: 0x000A7D28
	public void OnShelfPieceCreated(BuilderPiece piece, bool playfx)
	{
		if (this.playSpawnSetSound && playfx)
		{
			this.audioSource.GTPlayOneShot(this.spawnNewSetSound, 1f);
			this.playSpawnSetSound = false;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceCreated(piece, playfx);
		}
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000A9BA4 File Offset: 0x000A7DA4
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceRecycled(piece);
		}
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000A9BF8 File Offset: 0x000A7DF8
	public void OnClearTable()
	{
		if (!this.initialized)
		{
			return;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.OnClearTable();
		}
		base.StopAllCoroutines();
		if (this.animatingShelf)
		{
			this.resetAnimation.Rewind();
			this.animatingShelf = false;
		}
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x000A9C74 File Offset: 0x000A7E74
	public void ClearShelf()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
	}

	// Token: 0x040028CD RID: 10445
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x040028CE RID: 10446
	public List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x040028CF RID: 10447
	[Header("Dispenser Shelf Properties")]
	public Transform shelfCenter;

	// Token: 0x040028D0 RID: 10448
	public float shelfWidth = 1.4f;

	// Token: 0x040028D1 RID: 10449
	public Animation resetAnimation;

	// Token: 0x040028D2 RID: 10450
	[SerializeField]
	private SoundBankPlayer resetSoundBank;

	// Token: 0x040028D3 RID: 10451
	[SerializeField]
	private AudioClip spawnNewSetSound;

	// Token: 0x040028D4 RID: 10452
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040028D5 RID: 10453
	private bool playSpawnSetSound;

	// Token: 0x040028D6 RID: 10454
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x040028D7 RID: 10455
	public int shelfID = -1;

	// Token: 0x040028D8 RID: 10456
	private BuilderPieceSet currentSet;

	// Token: 0x040028D9 RID: 10457
	private bool initialized;

	// Token: 0x040028DA RID: 10458
	public BuilderDispenser dispenserPrefab;

	// Token: 0x040028DB RID: 10459
	private List<BuilderDispenser> dispenserPool;

	// Token: 0x040028DC RID: 10460
	private List<BuilderDispenser> activeDispensers;

	// Token: 0x040028DD RID: 10461
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x040028DE RID: 10462
	private bool animatingShelf;

	// Token: 0x040028DF RID: 10463
	private double timeToClearShelf = double.MaxValue;

	// Token: 0x040028E0 RID: 10464
	private int dispenserToClear;

	// Token: 0x040028E1 RID: 10465
	private int dispenserToUpdate;

	// Token: 0x040028E2 RID: 10466
	private bool shouldVerifySetSelection;
}
