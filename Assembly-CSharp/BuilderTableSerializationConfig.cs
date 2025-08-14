using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200051A RID: 1306
[CreateAssetMenu(fileName = "BuilderTableSerializationConfig", menuName = "Gorilla Tag/Builder/Serialization", order = 0)]
public class BuilderTableSerializationConfig : ScriptableObject
{
	// Token: 0x0400286A RID: 10346
	public string tableConfigurationKey;

	// Token: 0x0400286B RID: 10347
	public string titleDataKey;

	// Token: 0x0400286C RID: 10348
	public string startingMapConfigKey;

	// Token: 0x0400286D RID: 10349
	public List<string> scanSlotMothershipKeys;

	// Token: 0x0400286E RID: 10350
	public string scanSlotDevKey;

	// Token: 0x0400286F RID: 10351
	public string publishedScanMothershipKey;

	// Token: 0x04002870 RID: 10352
	public string timeAppend;

	// Token: 0x04002871 RID: 10353
	public string playfabScanKey;

	// Token: 0x04002872 RID: 10354
	public string sharedBlocksApiBaseURL;

	// Token: 0x04002873 RID: 10355
	public string recentVotesPrefsKey;

	// Token: 0x04002874 RID: 10356
	public string localMapsPrefsKey;
}
