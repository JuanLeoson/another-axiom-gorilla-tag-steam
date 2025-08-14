using System;
using UnityEngine;

// Token: 0x02000B0A RID: 2826
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06004411 RID: 17425 RVA: 0x001558AA File Offset: 0x00153AAA
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
