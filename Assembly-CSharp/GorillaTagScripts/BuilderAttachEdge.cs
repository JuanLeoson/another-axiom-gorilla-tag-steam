using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BFC RID: 3068
	public class BuilderAttachEdge : MonoBehaviour
	{
		// Token: 0x06004AA5 RID: 19109 RVA: 0x0016A4FE File Offset: 0x001686FE
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06004AA6 RID: 19110 RVA: 0x0016A51C File Offset: 0x0016871C
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Transform transform = this.center;
			if (transform == null)
			{
				transform = base.transform;
			}
			Vector3 a = transform.rotation * Vector3.right;
			Gizmos.DrawLine(transform.position - a * this.length * 0.5f, transform.position + a * this.length * 0.5f);
		}

		// Token: 0x0400538B RID: 21387
		public Transform center;

		// Token: 0x0400538C RID: 21388
		public float length;
	}
}
