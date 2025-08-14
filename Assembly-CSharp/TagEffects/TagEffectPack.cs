using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000DFC RID: 3580
	[CreateAssetMenu(fileName = "New Tag Effect Pack", menuName = "Tag Effect Pack")]
	public class TagEffectPack : ScriptableObject
	{
		// Token: 0x04006278 RID: 25208
		public GameObject thirdPerson;

		// Token: 0x04006279 RID: 25209
		public bool thirdPersonParentEffect = true;

		// Token: 0x0400627A RID: 25210
		public GameObject firstPerson;

		// Token: 0x0400627B RID: 25211
		public bool firstPersonParentEffect = true;

		// Token: 0x0400627C RID: 25212
		public GameObject highFive;

		// Token: 0x0400627D RID: 25213
		public bool highFiveParentEffect;

		// Token: 0x0400627E RID: 25214
		public GameObject fistBump;

		// Token: 0x0400627F RID: 25215
		public bool fistBumpParentEffect;

		// Token: 0x04006280 RID: 25216
		public bool shouldFaceTagger;
	}
}
