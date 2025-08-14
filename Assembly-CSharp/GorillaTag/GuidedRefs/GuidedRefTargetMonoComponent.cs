using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EBD RID: 3773
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06005E28 RID: 24104 RVA: 0x001DB6C0 File Offset: 0x001D98C0
		// (set) Token: 0x06005E29 RID: 24105 RVA: 0x001DB6C8 File Offset: 0x001D98C8
		GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
		{
			get
			{
				return this.guidedRefTargetInfo;
			}
			set
			{
				this.guidedRefTargetInfo = value;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06005E2A RID: 24106 RVA: 0x001DB6D1 File Offset: 0x001D98D1
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x06005E2B RID: 24107 RVA: 0x000E43D0 File Offset: 0x000E25D0
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x001DB6D9 File Offset: 0x001D98D9
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x001DB6E2 File Offset: 0x001D98E2
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005E2F RID: 24111 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400680C RID: 26636
		[SerializeField]
		private Component targetComponent;

		// Token: 0x0400680D RID: 26637
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
