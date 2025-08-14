using System;
using UnityEngine;

// Token: 0x020004AC RID: 1196
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x06001D91 RID: 7569 RVA: 0x0009E890 File Offset: 0x0009CA90
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x04002622 RID: 9762
	public Rigidbody[] rigidBodies;

	// Token: 0x04002623 RID: 9763
	public GameObject target;
}
