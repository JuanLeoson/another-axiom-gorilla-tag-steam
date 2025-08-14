using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C3A RID: 3130
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x06004D50 RID: 19792 RVA: 0x001809A6 File Offset: 0x0017EBA6
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x001809B3 File Offset: 0x0017EBB3
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x001809B3 File Offset: 0x0017EBB3
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x001809BC File Offset: 0x0017EBBC
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBothStartAndStop)
			{
				this.isStartButton = !PlayerTimerManager.instance.IsLocalTimerStarted();
			}
			this.isInitialized = true;
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x00180A38 File Offset: 0x0017EC38
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x00180A8F File Offset: 0x0017EC8F
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x00180AA0 File Offset: 0x0017ECA0
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x00180AC4 File Offset: 0x0017ECC4
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (Time.time < this.lastTriggeredTime + this.debounceTime)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.pressColor);
			this.mesh.SetPropertyBlock(this.materialProps);
			PlayerTimerManager.instance.RequestTimerToggle(this.isStartButton);
			this.lastTriggeredTime = Time.time;
		}

		// Token: 0x06004D58 RID: 19800 RVA: 0x00180B84 File Offset: 0x0017ED84
		private void OnTriggerExit(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
			{
				return;
			}
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.notPressedColor);
			this.mesh.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x04005650 RID: 22096
		private float lastTriggeredTime;

		// Token: 0x04005651 RID: 22097
		[SerializeField]
		private bool isStartButton;

		// Token: 0x04005652 RID: 22098
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x04005653 RID: 22099
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04005654 RID: 22100
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x04005655 RID: 22101
		[SerializeField]
		private Color pressColor;

		// Token: 0x04005656 RID: 22102
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x04005657 RID: 22103
		private MaterialPropertyBlock materialProps;

		// Token: 0x04005658 RID: 22104
		private bool isInitialized;
	}
}
