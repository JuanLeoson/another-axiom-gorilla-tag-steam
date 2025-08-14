using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EBE RID: 3774
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06005E31 RID: 24113 RVA: 0x001DB6F6 File Offset: 0x001D98F6
		// (set) Token: 0x06005E32 RID: 24114 RVA: 0x001DB6FE File Offset: 0x001D98FE
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

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06005E33 RID: 24115 RVA: 0x0001399F File Offset: 0x00011B9F
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06005E34 RID: 24116 RVA: 0x000E43D0 File Offset: 0x000E25D0
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06005E35 RID: 24117 RVA: 0x001DB707 File Offset: 0x001D9907
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x06005E36 RID: 24118 RVA: 0x001DB710 File Offset: 0x001D9910
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005E38 RID: 24120 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005E39 RID: 24121 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400680E RID: 26638
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
