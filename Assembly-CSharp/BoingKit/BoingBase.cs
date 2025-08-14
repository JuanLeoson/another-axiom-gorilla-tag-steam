using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FBD RID: 4029
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x060064BD RID: 25789 RVA: 0x001FF1D6 File Offset: 0x001FD3D6
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x060064BE RID: 25790 RVA: 0x001FF1DE File Offset: 0x001FD3DE
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x060064BF RID: 25791 RVA: 0x001FF1E6 File Offset: 0x001FD3E6
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x001FF1EE File Offset: 0x001FD3EE
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x04006F70 RID: 28528
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x04006F71 RID: 28529
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x04006F72 RID: 28530
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
