using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C05 RID: 3077
	public class BuilderPool : MonoBehaviour
	{
		// Token: 0x06004AEF RID: 19183 RVA: 0x0016C0EA File Offset: 0x0016A2EA
		private void Awake()
		{
			if (BuilderPool.instance == null)
			{
				BuilderPool.instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06004AF0 RID: 19184 RVA: 0x0016C108 File Offset: 0x0016A308
		public void Setup()
		{
			if (this.isSetup)
			{
				return;
			}
			this.piecePools = new List<List<BuilderPiece>>(512);
			this.piecePoolLookup = new Dictionary<int, int>(512);
			this.bumpGlowPool = new List<BuilderBumpGlow>(256);
			this.AddToGlowBumpPool(256);
			this.snapOverlapPool = new List<SnapOverlap>(4096);
			this.AddToSnapOverlapPool(4096);
			this.isSetup = true;
		}

		// Token: 0x06004AF1 RID: 19185 RVA: 0x0016C17C File Offset: 0x0016A37C
		public void BuildFromShelves(List<BuilderShelf> shelves)
		{
			for (int i = 0; i < shelves.Count; i++)
			{
				BuilderShelf builderShelf = shelves[i];
				for (int j = 0; j < builderShelf.buildPieceSpawns.Count; j++)
				{
					BuilderShelf.BuildPieceSpawn buildPieceSpawn = builderShelf.buildPieceSpawns[j];
					this.AddToPool(buildPieceSpawn.buildPiecePrefab.name.GetStaticHash(), buildPieceSpawn.count);
				}
			}
		}

		// Token: 0x06004AF2 RID: 19186 RVA: 0x0016C1E4 File Offset: 0x0016A3E4
		public void BuildFromPieceSets()
		{
			if (this.hasBuiltPieceSets)
			{
				return;
			}
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				bool flag = BuilderSetManager.instance.GetStarterSetsConcat().Contains(builderPieceSet.playfabID);
				bool flag2 = builderPieceSet.setName.Equals("HIDDEN");
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
						int count;
						if (!this.piecePoolLookup.TryGetValue(staticHash, out count))
						{
							count = this.piecePools.Count;
							this.piecePools.Add(new List<BuilderPiece>(128));
							this.piecePoolLookup.Add(staticHash, count);
							if (!flag2)
							{
								int count2 = flag ? 64 : 16;
								this.AddToPool(staticHash, count2);
							}
						}
					}
				}
			}
			this.hasBuiltPieceSets = true;
		}

		// Token: 0x06004AF3 RID: 19187 RVA: 0x0016C380 File Offset: 0x0016A580
		private void AddToPool(int pieceType, int count)
		{
			int count2;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count2))
			{
				count2 = this.piecePools.Count;
				this.piecePools.Add(new List<BuilderPiece>(count * 8));
				this.piecePoolLookup.Add(pieceType, count2);
				Debug.LogWarningFormat("Creating Pool for piece {0} of size {1}. Is this piece not in a piece set?", new object[]
				{
					pieceType,
					count * 8
				});
			}
			BuilderPiece piecePrefab = BuilderSetManager.instance.GetPiecePrefab(pieceType);
			if (piecePrefab == null)
			{
				return;
			}
			List<BuilderPiece> list = this.piecePools[count2];
			for (int i = 0; i < count; i++)
			{
				BuilderPiece builderPiece = Object.Instantiate<BuilderPiece>(piecePrefab);
				builderPiece.OnCreatedByPool();
				builderPiece.gameObject.SetActive(false);
				list.Add(builderPiece);
			}
		}

		// Token: 0x06004AF4 RID: 19188 RVA: 0x0016C444 File Offset: 0x0016A644
		public BuilderPiece CreatePiece(int pieceType, bool assertNotEmpty)
		{
			int count;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count))
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("No Pool Found for {0} Adding 4", new object[]
					{
						pieceType
					});
				}
				count = this.piecePools.Count;
				this.AddToPool(pieceType, 4);
			}
			List<BuilderPiece> list = this.piecePools[count];
			if (list.Count == 0)
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("Pool for {0} is Empty Adding 4", new object[]
					{
						pieceType
					});
				}
				this.AddToPool(pieceType, 4);
			}
			BuilderPiece result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}

		// Token: 0x06004AF5 RID: 19189 RVA: 0x0016C4E8 File Offset: 0x0016A6E8
		public void DestroyPiece(BuilderPiece piece)
		{
			if (piece == null)
			{
				Debug.LogError("Why is a null piece being destroyed");
				return;
			}
			int index;
			if (!this.piecePoolLookup.TryGetValue(piece.pieceType, out index))
			{
				Debug.LogErrorFormat("No Pool Found for {0} Cannot return to pool", new object[]
				{
					piece.pieceType
				});
				return;
			}
			List<BuilderPiece> list = this.piecePools[index];
			if (list.Count == 128)
			{
				piece.OnReturnToPool();
				Object.Destroy(piece.gameObject);
				return;
			}
			piece.gameObject.SetActive(false);
			piece.transform.SetParent(null);
			piece.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			piece.OnReturnToPool();
			list.Add(piece);
		}

		// Token: 0x06004AF6 RID: 19190 RVA: 0x0016C5B0 File Offset: 0x0016A7B0
		private void AddToGlowBumpPool(int count)
		{
			if (this.bumpGlowPrefab == null)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				BuilderBumpGlow builderBumpGlow = Object.Instantiate<BuilderBumpGlow>(this.bumpGlowPrefab);
				builderBumpGlow.gameObject.SetActive(false);
				this.bumpGlowPool.Add(builderBumpGlow);
			}
		}

		// Token: 0x06004AF7 RID: 19191 RVA: 0x0016C5FC File Offset: 0x0016A7FC
		public BuilderBumpGlow CreateGlowBump()
		{
			if (this.bumpGlowPool.Count == 0)
			{
				this.AddToGlowBumpPool(4);
			}
			BuilderBumpGlow result = this.bumpGlowPool[this.bumpGlowPool.Count - 1];
			this.bumpGlowPool.RemoveAt(this.bumpGlowPool.Count - 1);
			return result;
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x0016C650 File Offset: 0x0016A850
		public void DestroyBumpGlow(BuilderBumpGlow bump)
		{
			if (bump == null)
			{
				return;
			}
			bump.gameObject.SetActive(false);
			bump.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			this.bumpGlowPool.Add(bump);
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x0016C6A0 File Offset: 0x0016A8A0
		private void AddToSnapOverlapPool(int count)
		{
			this.snapOverlapPool.Capacity = this.snapOverlapPool.Capacity + count;
			for (int i = 0; i < count; i++)
			{
				this.snapOverlapPool.Add(new SnapOverlap());
			}
		}

		// Token: 0x06004AFA RID: 19194 RVA: 0x0016C6E4 File Offset: 0x0016A8E4
		public SnapOverlap CreateSnapOverlap(BuilderAttachGridPlane otherPlane, SnapBounds bounds)
		{
			if (this.snapOverlapPool.Count == 0)
			{
				this.AddToSnapOverlapPool(1024);
			}
			SnapOverlap snapOverlap = this.snapOverlapPool[this.snapOverlapPool.Count - 1];
			this.snapOverlapPool.RemoveAt(this.snapOverlapPool.Count - 1);
			snapOverlap.otherPlane = otherPlane;
			snapOverlap.bounds = bounds;
			snapOverlap.nextOverlap = null;
			return snapOverlap;
		}

		// Token: 0x06004AFB RID: 19195 RVA: 0x0016C74E File Offset: 0x0016A94E
		public void DestroySnapOverlap(SnapOverlap snapOverlap)
		{
			snapOverlap.otherPlane = null;
			snapOverlap.nextOverlap = null;
			this.snapOverlapPool.Add(snapOverlap);
		}

		// Token: 0x06004AFC RID: 19196 RVA: 0x0016C76C File Offset: 0x0016A96C
		private void OnDestroy()
		{
			for (int i = 0; i < this.piecePools.Count; i++)
			{
				if (this.piecePools[i] != null)
				{
					foreach (BuilderPiece builderPiece in this.piecePools[i])
					{
						if (builderPiece != null)
						{
							Object.Destroy(builderPiece);
						}
					}
					this.piecePools[i].Clear();
				}
			}
			this.piecePoolLookup.Clear();
			foreach (BuilderBumpGlow obj in this.bumpGlowPool)
			{
				Object.Destroy(obj);
			}
			this.bumpGlowPool.Clear();
		}

		// Token: 0x040053DA RID: 21466
		public List<List<BuilderPiece>> piecePools;

		// Token: 0x040053DB RID: 21467
		public Dictionary<int, int> piecePoolLookup;

		// Token: 0x040053DC RID: 21468
		[HideInInspector]
		public List<BuilderBumpGlow> bumpGlowPool;

		// Token: 0x040053DD RID: 21469
		public BuilderBumpGlow bumpGlowPrefab;

		// Token: 0x040053DE RID: 21470
		[HideInInspector]
		public List<SnapOverlap> snapOverlapPool;

		// Token: 0x040053DF RID: 21471
		public static BuilderPool instance;

		// Token: 0x040053E0 RID: 21472
		private const int POOl_CAPACITY = 128;

		// Token: 0x040053E1 RID: 21473
		private const int INITIAL_INSTANCE_COUNT_STARTER = 64;

		// Token: 0x040053E2 RID: 21474
		private const int INITIAL_INSTANCE_COUNT_PREMIUM = 16;

		// Token: 0x040053E3 RID: 21475
		private bool isSetup;

		// Token: 0x040053E4 RID: 21476
		private bool hasBuiltPieceSets;
	}
}
