using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BF5 RID: 3061
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WhackAMoleLevelSetting", order = 1)]
	public class WhackAMoleLevelSO : ScriptableObject
	{
		// Token: 0x06004A6A RID: 19050 RVA: 0x00169BC4 File Offset: 0x00167DC4
		public int GetMinScore(bool isCoop)
		{
			if (!isCoop)
			{
				return this.minScore;
			}
			return this.minScore * 2;
		}

		// Token: 0x0400535E RID: 21342
		public int levelNumber;

		// Token: 0x0400535F RID: 21343
		public float levelDuration;

		// Token: 0x04005360 RID: 21344
		[Tooltip("For how long do the moles stay visible?")]
		public float showMoleDuration;

		// Token: 0x04005361 RID: 21345
		[Tooltip("How fast we pick a random new mole?")]
		public float pickNextMoleTime;

		// Token: 0x04005362 RID: 21346
		[Tooltip("Minimum score to get in order to be able to proceed to the next level")]
		[SerializeField]
		private int minScore;

		// Token: 0x04005363 RID: 21347
		[Tooltip("Chance of each mole being a hazard mole at the start, and end, of the level.")]
		public Vector2 hazardMoleChance = new Vector2(0f, 0.5f);

		// Token: 0x04005364 RID: 21348
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 minimumMoleCount = new Vector2(1f, 2f);

		// Token: 0x04005365 RID: 21349
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 maximumMoleCount = new Vector2(1.5f, 3f);
	}
}
