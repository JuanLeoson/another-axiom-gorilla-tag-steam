using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EB1 RID: 3761
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06005DF9 RID: 24057 RVA: 0x000E43D0 File Offset: 0x000E25D0
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06005DFA RID: 24058 RVA: 0x001DA4C6 File Offset: 0x001D86C6
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06005DFB RID: 24059 RVA: 0x001DA4CF File Offset: 0x001D86CF
		// (set) Token: 0x06005DFC RID: 24060 RVA: 0x001DA4D7 File Offset: 0x001D86D7
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

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06005DFD RID: 24061 RVA: 0x0005570E File Offset: 0x0005390E
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005DFE RID: 24062 RVA: 0x001DA4E0 File Offset: 0x001D86E0
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005E00 RID: 24064 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005E01 RID: 24065 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040067F2 RID: 26610
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
