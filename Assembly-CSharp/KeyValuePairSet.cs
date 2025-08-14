using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
[CreateAssetMenu(fileName = "New KeyValuePairSet", menuName = "Data/KeyValuePairSet", order = 0)]
public class KeyValuePairSet : ScriptableObject
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000402 RID: 1026 RVA: 0x00017D55 File Offset: 0x00015F55
	public KeyValueStringPair[] Entries
	{
		get
		{
			return this.m_entries;
		}
	}

	// Token: 0x0400047C RID: 1148
	[SerializeField]
	private KeyValueStringPair[] m_entries;
}
