using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000DE5 RID: 3557
	public class EnviromentMovement : MonoBehaviour
	{
		// Token: 0x06005854 RID: 22612 RVA: 0x001B653B File Offset: 0x001B473B
		private void Start()
		{
			this.thisTransform = base.gameObject.GetComponent<Transform>();
			this.nextPosition = this.pos1;
		}

		// Token: 0x06005855 RID: 22613 RVA: 0x001B655C File Offset: 0x001B475C
		private void Update()
		{
			if (Vector3.Distance(this.thisTransform.position, this.nextPosition) > 0.5f)
			{
				base.transform.position = Vector3.Lerp(this.thisTransform.position, this.nextPosition, 2f * Time.deltaTime);
				return;
			}
			if (this.nextPosition == this.pos1)
			{
				this.nextPosition = this.pos2;
				return;
			}
			if (this.nextPosition == this.pos2)
			{
				this.nextPosition = this.pos1;
				return;
			}
		}

		// Token: 0x040061F1 RID: 25073
		private Vector3 nextPosition = Vector3.zero;

		// Token: 0x040061F2 RID: 25074
		private Transform thisTransform;

		// Token: 0x040061F3 RID: 25075
		public Vector3 pos1;

		// Token: 0x040061F4 RID: 25076
		public Vector3 pos2;
	}
}
