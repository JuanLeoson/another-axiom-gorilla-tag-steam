using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D25 RID: 3365
	public class ControllerBoxController : MonoBehaviour
	{
		// Token: 0x06005347 RID: 21319 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Awake()
		{
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x0019C695 File Offset: 0x0019A895
		public void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.StartStopStateChanged();
			}
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x0019C6AB File Offset: 0x0019A8AB
		public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.DecreaseSpeedStateChanged();
			}
		}

		// Token: 0x0600534A RID: 21322 RVA: 0x0019C6C1 File Offset: 0x0019A8C1
		public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.IncreaseSpeedStateChanged();
			}
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x0019C6D7 File Offset: 0x0019A8D7
		public void SmokeButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.SmokeButtonStateChanged();
			}
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x0019C6ED File Offset: 0x0019A8ED
		public void WhistleButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.WhistleButtonStateChanged();
			}
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x0019C703 File Offset: 0x0019A903
		public void ReverseButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.ReverseButtonStateChanged();
			}
		}

		// Token: 0x0600534E RID: 21326 RVA: 0x0019C719 File Offset: 0x0019A919
		public void SwitchVisualization(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				HandsManager.Instance.SwitchVisualization();
			}
		}

		// Token: 0x0600534F RID: 21327 RVA: 0x0019C72E File Offset: 0x0019A92E
		public void GoMoo(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._cowController.GoMooCowGo();
			}
		}

		// Token: 0x04005C8F RID: 23695
		[SerializeField]
		private TrainLocomotive _locomotive;

		// Token: 0x04005C90 RID: 23696
		[SerializeField]
		private CowController _cowController;
	}
}
