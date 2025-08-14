using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009F RID: 159
public interface IEyeScannable
{
	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060003FA RID: 1018
	int scannableId { get; }

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060003FB RID: 1019
	Vector3 Position { get; }

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003FC RID: 1020
	Bounds Bounds { get; }

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003FD RID: 1021
	IList<KeyValueStringPair> Entries { get; }

	// Token: 0x060003FE RID: 1022
	void OnEnable();

	// Token: 0x060003FF RID: 1023
	void OnDisable();

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000400 RID: 1024
	// (remove) Token: 0x06000401 RID: 1025
	event Action OnDataChange;
}
