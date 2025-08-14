using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000DE7 RID: 3559
	[AddComponentMenu("")]
	public class MTAssetsMathematics : MonoBehaviour
	{
		// Token: 0x06005858 RID: 22616 RVA: 0x001B6608 File Offset: 0x001B4808
		public static List<T> RandomizeThisList<T>(List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(i, count);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
			return list;
		}

		// Token: 0x06005859 RID: 22617 RVA: 0x001B6655 File Offset: 0x001B4855
		public static Vector3 GetHalfPositionBetweenTwoPoints(Vector3 pointA, Vector3 pointB)
		{
			return Vector3.Lerp(pointA, pointB, 0.5f);
		}
	}
}
