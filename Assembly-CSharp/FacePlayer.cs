using System;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public class FacePlayer : MonoBehaviour
{
	// Token: 0x06001DFA RID: 7674 RVA: 0x000A0668 File Offset: 0x0009E868
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
