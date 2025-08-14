using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x0200014A RID: 330
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestType
{
	// Token: 0x04000A36 RID: 2614
	none,
	// Token: 0x04000A37 RID: 2615
	gameModeObjective,
	// Token: 0x04000A38 RID: 2616
	gameModeRound,
	// Token: 0x04000A39 RID: 2617
	grabObject,
	// Token: 0x04000A3A RID: 2618
	dropObject,
	// Token: 0x04000A3B RID: 2619
	eatObject,
	// Token: 0x04000A3C RID: 2620
	tapObject,
	// Token: 0x04000A3D RID: 2621
	launchedProjectile,
	// Token: 0x04000A3E RID: 2622
	moveDistance,
	// Token: 0x04000A3F RID: 2623
	swimDistance,
	// Token: 0x04000A40 RID: 2624
	triggerHandEffect,
	// Token: 0x04000A41 RID: 2625
	enterLocation,
	// Token: 0x04000A42 RID: 2626
	misc,
	// Token: 0x04000A43 RID: 2627
	critter
}
