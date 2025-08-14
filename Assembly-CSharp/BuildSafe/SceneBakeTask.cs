using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x02000CF0 RID: 3312
	public abstract class SceneBakeTask : MonoBehaviour
	{
		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x0600521E RID: 21022 RVA: 0x00198757 File Offset: 0x00196957
		// (set) Token: 0x0600521F RID: 21023 RVA: 0x0019875F File Offset: 0x0019695F
		public SceneBakeMode bakeMode
		{
			get
			{
				return this.m_bakeMode;
			}
			set
			{
				this.m_bakeMode = value;
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06005220 RID: 21024 RVA: 0x00198768 File Offset: 0x00196968
		// (set) Token: 0x06005221 RID: 21025 RVA: 0x00198770 File Offset: 0x00196970
		public virtual int callbackOrder
		{
			get
			{
				return this.m_callbackOrder;
			}
			set
			{
				this.m_callbackOrder = value;
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06005222 RID: 21026 RVA: 0x00198779 File Offset: 0x00196979
		// (set) Token: 0x06005223 RID: 21027 RVA: 0x00198781 File Offset: 0x00196981
		public bool runIfInactive
		{
			get
			{
				return this.m_runIfInactive;
			}
			set
			{
				this.m_runIfInactive = value;
			}
		}

		// Token: 0x06005224 RID: 21028
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x06005225 RID: 21029 RVA: 0x000023F5 File Offset: 0x000005F5
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x04005BA7 RID: 23463
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x04005BA8 RID: 23464
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x04005BA9 RID: 23465
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
