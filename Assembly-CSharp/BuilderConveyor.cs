using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Token: 0x02000522 RID: 1314
public class BuilderConveyor : MonoBehaviour
{
	// Token: 0x06001FE1 RID: 8161 RVA: 0x000A8344 File Offset: 0x000A6544
	private void Start()
	{
		this.InitIfNeeded();
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000A8344 File Offset: 0x000A6544
	public void Setup()
	{
		this.InitIfNeeded();
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000A834C File Offset: 0x000A654C
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.nextPieceToSpawn = 0;
		this.grabbedPieceTypes = new Queue<int>(10);
		this.grabbedPieceMaterials = new Queue<int>(10);
		this.setSelector.Setup(this._includeCategories);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		this.setSelector.OnSelectedSet.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.initialized = true;
		this.splineLength = this.spline.Splines[0].GetLength();
		this.maxItemsOnSpline = Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
		this.nativeSpline = new NativeSpline(this.spline.Splines[0], this.spline.transform.localToWorldMatrix, Allocator.Persistent);
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000A84C0 File Offset: 0x000A66C0
	public int GetMaxItemsOnConveyor()
	{
		return Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000A84DD File Offset: 0x000A66DD
	public float GetFrameMovement()
	{
		return this.conveyorMoveSpeed / this.splineLength;
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000A84EC File Offset: 0x000A66EC
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedSet.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
		this.nativeSpline.Dispose();
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000A8523 File Offset: 0x000A6723
	public void OnSelectedSetChange(int setId)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, setId, true);
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000A8548 File Offset: 0x000A6748
	public void SetSelection(int setId)
	{
		this.setSelector.SetSelection(setId);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		this.nextPieceToSpawn = 0;
		this.loopCount = 0;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000A85F4 File Offset: 0x000A67F4
	public int GetSelectedSetID()
	{
		return this.setSelector.GetSelectedSet().GetIntIdentifier();
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000A8608 File Offset: 0x000A6808
	public void UpdateConveyor()
	{
		if (!this.initialized)
		{
			this.Setup();
		}
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (builderPiece.state != BuilderPiece.State.OnConveyor)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient && builderPiece.state != BuilderPiece.State.None)
				{
					this.grabbedPieceTypes.Enqueue(builderPiece.pieceType);
					this.grabbedPieceMaterials.Enqueue(builderPiece.materialType);
				}
				builderPiece.shelfOwner = -1;
				this.piecesOnConveyor.RemoveAt(i);
				this.table.conveyorManager.RemovePieceFromJob(builderPiece);
			}
		}
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000A86AC File Offset: 0x000A68AC
	public void RemovePieceFromConveyor(Transform pieceTransform)
	{
		foreach (BuilderPiece builderPiece in this.piecesOnConveyor)
		{
			if (builderPiece.transform == pieceTransform)
			{
				this.piecesOnConveyor.Remove(builderPiece);
				builderPiece.shelfOwner = -1;
				this.table.RequestRecyclePiece(builderPiece, false, -1);
				break;
			}
		}
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000A872C File Offset: 0x000A692C
	private Vector3 EvaluateSpline(float t)
	{
		float t2;
		this._evaluateCurve = this.nativeSpline.GetCurve(this.nativeSpline.SplineToCurveT(t, out t2));
		return CurveUtility.EvaluatePosition(this._evaluateCurve, t2);
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000A876C File Offset: 0x000A696C
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
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
		if (this.waitForResourceChange)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble >= this.nextSpawnTime)
		{
			this.SpawnNextPiece();
			this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		}
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000A8802 File Offset: 0x000A6A02
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000A880B File Offset: 0x000A6A0B
	public void OnAvailableResourcesChange()
	{
		this.waitForResourceChange = false;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000A8814 File Offset: 0x000A6A14
	public Transform GetSpawnTransform()
	{
		return this.spawnTransform;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000A881C File Offset: 0x000A6A1C
	public void OnShelfPieceCreated(BuilderPiece piece, float timeOffset)
	{
		float num = timeOffset * this.conveyorMoveSpeed / this.splineLength;
		if (num > 1f)
		{
			Debug.LogWarningFormat("Piece {0} add to shelf time {1}", new object[]
			{
				piece.pieceId,
				num
			});
		}
		int count = this.piecesOnConveyor.Count;
		this.piecesOnConveyor.Add(piece);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		Vector3 a = this.EvaluateSpline(num2);
		Quaternion rotation = this.spawnTransform.rotation * Quaternion.Euler(piece.desiredShelfRotationOffset);
		Vector3 position = a + this.spawnTransform.rotation * piece.desiredShelfOffset;
		piece.transform.SetPositionAndRotation(position, rotation);
		this.table.conveyorManager.AddPieceToJob(piece, num2, this.shelfID);
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000A88F5 File Offset: 0x000A6AF5
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		this.piecesOnConveyor.Remove(piece);
		if (piece != null)
		{
			this.table.conveyorManager.RemovePieceFromJob(piece);
		}
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000A891E File Offset: 0x000A6B1E
	public void OnClearTable()
	{
		this.piecesOnConveyor.Clear();
		this.grabbedPieceTypes.Clear();
		this.grabbedPieceMaterials.Clear();
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000A8944 File Offset: 0x000A6B44
	public void ResetConveyorState()
	{
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (!(builderPiece == null))
			{
				BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
				{
					type = BuilderTable.BuilderCommandType.Recycle,
					pieceId = builderPiece.pieceId,
					localPosition = builderPiece.transform.position,
					localRotation = builderPiece.transform.rotation,
					player = NetworkSystem.Instance.LocalPlayer,
					isLeft = false,
					parentPieceId = -1
				};
				this.table.ExecutePieceRecycled(cmd);
			}
		}
		this.OnClearTable();
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000A89FC File Offset: 0x000A6BFC
	private void SpawnNextPiece()
	{
		int num;
		int materialType;
		this.FindNextAffordablePieceType(out num, out materialType);
		if (num == -1)
		{
			return;
		}
		this.table.RequestCreateConveyorPiece(num, materialType, this.shelfID);
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000A8A2C File Offset: 0x000A6C2C
	private void FindNextAffordablePieceType(out int pieceType, out int materialType)
	{
		if (this.grabbedPieceTypes.Count > 0)
		{
			pieceType = this.grabbedPieceTypes.Dequeue();
			materialType = this.grabbedPieceMaterials.Dequeue();
			return;
		}
		pieceType = -1;
		materialType = -1;
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		for (int i = this.nextPieceToSpawn; i < this.piecesInSet.Count; i++)
		{
			BuilderPiece piecePrefab = this.piecesInSet[i].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab))
			{
				if (i + 1 >= this.piecesInSet.Count)
				{
					this.loopCount++;
					this.loopCount = Mathf.Max(0, this.loopCount);
				}
				this.nextPieceToSpawn = (i + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[i]);
				return;
			}
		}
		this.loopCount++;
		this.loopCount = Mathf.Max(0, this.loopCount);
		for (int j = 0; j < this.nextPieceToSpawn; j++)
		{
			BuilderPiece piecePrefab2 = this.piecesInSet[j].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab2))
			{
				this.nextPieceToSpawn = (j + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab2.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[j]);
				return;
			}
		}
		this.waitForResourceChange = true;
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000A8BB0 File Offset: 0x000A6DB0
	private int GetMaterialType(BuilderPieceSet.PieceInfo info)
	{
		if (info.piecePrefab.materialOptions != null && info.overrideSetMaterial && info.pieceMaterialTypes.Length != 0)
		{
			int num = this.loopCount % info.pieceMaterialTypes.Length;
			string text = info.pieceMaterialTypes[num];
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogErrorFormat("Empty Material Override for piece {0} in set {1}", new object[]
				{
					info.piecePrefab.name,
					this.currentSet.name
				});
				return -1;
			}
			return text.GetHashCode();
		}
		else
		{
			if (string.IsNullOrEmpty(this.currentSet.materialId))
			{
				return -1;
			}
			return this.currentSet.materialId.GetHashCode();
		}
	}

	// Token: 0x040028A0 RID: 10400
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x040028A1 RID: 10401
	public List<BuilderPieceSet.BuilderPieceCategory> _includeCategories;

	// Token: 0x040028A2 RID: 10402
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x040028A3 RID: 10403
	public int shelfID = -1;

	// Token: 0x040028A4 RID: 10404
	[Header("Conveyor Properties")]
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x040028A5 RID: 10405
	[SerializeField]
	private SplineContainer spline;

	// Token: 0x040028A6 RID: 10406
	private float conveyorMoveSpeed = 0.2f;

	// Token: 0x040028A7 RID: 10407
	private float spawnDelay = 1.5f;

	// Token: 0x040028A8 RID: 10408
	private double nextSpawnTime;

	// Token: 0x040028A9 RID: 10409
	private int nextPieceToSpawn;

	// Token: 0x040028AA RID: 10410
	private BuilderPieceSet currentSet;

	// Token: 0x040028AB RID: 10411
	private int loopCount;

	// Token: 0x040028AC RID: 10412
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x040028AD RID: 10413
	private Queue<int> grabbedPieceTypes;

	// Token: 0x040028AE RID: 10414
	private Queue<int> grabbedPieceMaterials;

	// Token: 0x040028AF RID: 10415
	private List<BuilderPiece> piecesOnConveyor = new List<BuilderPiece>(10);

	// Token: 0x040028B0 RID: 10416
	private Vector3 moveDirection;

	// Token: 0x040028B1 RID: 10417
	private bool waitForResourceChange;

	// Token: 0x040028B2 RID: 10418
	private bool initialized;

	// Token: 0x040028B3 RID: 10419
	private float splineLength = 1f;

	// Token: 0x040028B4 RID: 10420
	private int maxItemsOnSpline;

	// Token: 0x040028B5 RID: 10421
	private UnityEngine.Splines.BezierCurve _evaluateCurve;

	// Token: 0x040028B6 RID: 10422
	public NativeSpline nativeSpline;

	// Token: 0x040028B7 RID: 10423
	private bool shouldVerifySetSelection;
}
