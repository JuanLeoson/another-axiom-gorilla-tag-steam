using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200053E RID: 1342
[CreateAssetMenu(fileName = "BuilderPieceSet01", menuName = "Gorilla Tag/Builder/PieceSet", order = 0)]
public class BuilderPieceSet : ScriptableObject
{
	// Token: 0x060020EF RID: 8431 RVA: 0x000B0FF1 File Offset: 0x000AF1F1
	public int GetIntIdentifier()
	{
		return this.playfabID.GetStaticHash();
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000B1000 File Offset: 0x000AF200
	public DateTime GetScheduleDateTime()
	{
		if (this.isScheduled)
		{
			try
			{
				return DateTime.Parse(this.scheduledDate, CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
		return DateTime.MinValue;
	}

	// Token: 0x040029E1 RID: 10721
	[Tooltip("Display Name")]
	public string setName;

	// Token: 0x040029E2 RID: 10722
	public GameObject displayModel;

	// Token: 0x040029E3 RID: 10723
	[FormerlySerializedAs("uniqueId")]
	[Tooltip("Playfab ID")]
	public string playfabID;

	// Token: 0x040029E4 RID: 10724
	public string materialId;

	// Token: 0x040029E5 RID: 10725
	public bool isScheduled;

	// Token: 0x040029E6 RID: 10726
	public string scheduledDate = "1/1/0001 00:00:00";

	// Token: 0x040029E7 RID: 10727
	public List<BuilderPieceSet.BuilderPieceSubset> subsets;

	// Token: 0x0200053F RID: 1343
	public enum BuilderPieceCategory
	{
		// Token: 0x040029E9 RID: 10729
		FLAT,
		// Token: 0x040029EA RID: 10730
		TALL,
		// Token: 0x040029EB RID: 10731
		HALF_HEIGHT,
		// Token: 0x040029EC RID: 10732
		BEAM,
		// Token: 0x040029ED RID: 10733
		SLOPE,
		// Token: 0x040029EE RID: 10734
		OVERSIZED,
		// Token: 0x040029EF RID: 10735
		SPECIAL_DISPLAY,
		// Token: 0x040029F0 RID: 10736
		FUNCTIONAL = 18,
		// Token: 0x040029F1 RID: 10737
		DECORATIVE,
		// Token: 0x040029F2 RID: 10738
		MISC
	}

	// Token: 0x02000540 RID: 1344
	[Serializable]
	public class BuilderPieceSubset
	{
		// Token: 0x040029F3 RID: 10739
		public string subsetName;

		// Token: 0x040029F4 RID: 10740
		public BuilderPieceSet.BuilderPieceCategory pieceCategory;

		// Token: 0x040029F5 RID: 10741
		public List<BuilderPieceSet.PieceInfo> pieceInfos;
	}

	// Token: 0x02000541 RID: 1345
	[Serializable]
	public struct PieceInfo
	{
		// Token: 0x040029F6 RID: 10742
		public BuilderPiece piecePrefab;

		// Token: 0x040029F7 RID: 10743
		public bool overrideSetMaterial;

		// Token: 0x040029F8 RID: 10744
		public string[] pieceMaterialTypes;
	}
}
