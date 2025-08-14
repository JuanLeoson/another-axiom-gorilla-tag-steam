using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FDB RID: 4059
	public class BoingManagerPostUpdatePump : MonoBehaviour
	{
		// Token: 0x06006573 RID: 25971 RVA: 0x00072F04 File Offset: 0x00071104
		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x00201F0B File Offset: 0x0020010B
		private bool TryDestroyDuplicate()
		{
			if (BoingManager.s_managerGo == base.gameObject)
			{
				return false;
			}
			Object.Destroy(base.gameObject);
			return true;
		}

		// Token: 0x06006575 RID: 25973 RVA: 0x00201F2D File Offset: 0x0020012D
		private void FixedUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.FixedUpdate);
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x00201F3E File Offset: 0x0020013E
		private void Update()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.EarlyUpdate);
		}

		// Token: 0x06006577 RID: 25975 RVA: 0x00201F61 File Offset: 0x00200161
		private void LateUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.Execute(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.LateUpdate);
		}
	}
}
