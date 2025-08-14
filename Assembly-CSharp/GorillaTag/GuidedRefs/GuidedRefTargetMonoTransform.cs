using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EBF RID: 3775
	public class GuidedRefTargetMonoTransform : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06005E3A RID: 24122 RVA: 0x001DB724 File Offset: 0x001D9924
		// (set) Token: 0x06005E3B RID: 24123 RVA: 0x001DB72C File Offset: 0x001D992C
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

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06005E3C RID: 24124 RVA: 0x0005860D File Offset: 0x0005680D
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x06005E3D RID: 24125 RVA: 0x000E43D0 File Offset: 0x000E25D0
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06005E3E RID: 24126 RVA: 0x001DB735 File Offset: 0x001D9935
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoTransform>(this, true);
		}

		// Token: 0x06005E3F RID: 24127 RVA: 0x001DB73E File Offset: 0x001D993E
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoTransform>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005E41 RID: 24129 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005E42 RID: 24130 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400680F RID: 26639
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
