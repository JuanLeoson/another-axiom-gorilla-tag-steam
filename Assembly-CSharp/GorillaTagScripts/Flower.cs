using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C2B RID: 3115
	public class Flower : MonoBehaviour
	{
		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06004CA4 RID: 19620 RVA: 0x0017C173 File Offset: 0x0017A373
		// (set) Token: 0x06004CA5 RID: 19621 RVA: 0x0017C17B File Offset: 0x0017A37B
		public bool IsWatered { get; private set; }

		// Token: 0x06004CA6 RID: 19622 RVA: 0x0017C184 File Offset: 0x0017A384
		private void Awake()
		{
			this.shouldUpdateVisuals = true;
			this.anim = base.GetComponent<Animator>();
			this.timer = base.GetComponent<GorillaTimer>();
			this.perchPoint = base.GetComponent<BeePerchPoint>();
			this.timer.onTimerStopped.AddListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
			this.currentState = Flower.FlowerState.None;
			this.wateredFx = this.wateredFx.GetComponent<ParticleSystem>();
			this.IsWatered = false;
			this.meshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			this.meshRenderer.enabled = false;
			this.anim.enabled = false;
		}

		// Token: 0x06004CA7 RID: 19623 RVA: 0x0017C21B File Offset: 0x0017A41B
		private void OnDestroy()
		{
			this.timer.onTimerStopped.RemoveListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
		}

		// Token: 0x06004CA8 RID: 19624 RVA: 0x0017C23C File Offset: 0x0017A43C
		public void WaterFlower(bool isWatered = false)
		{
			this.IsWatered = isWatered;
			switch (this.currentState)
			{
			case Flower.FlowerState.None:
				this.UpdateFlowerState(Flower.FlowerState.Healthy, false, true);
				return;
			case Flower.FlowerState.Healthy:
				if (!isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, false, true);
					return;
				}
				break;
			case Flower.FlowerState.Middle:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Healthy, true, true);
					return;
				}
				this.UpdateFlowerState(Flower.FlowerState.Wilted, false, true);
				return;
			case Flower.FlowerState.Wilted:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, true, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004CA9 RID: 19625 RVA: 0x0017C2AC File Offset: 0x0017A4AC
		public void UpdateFlowerState(Flower.FlowerState newState, bool isWatered = false, bool updateVisual = true)
		{
			if (FlowersManager.Instance.IsMine)
			{
				this.timer.RestartTimer();
			}
			this.ChangeState(newState);
			if (this.perchPoint)
			{
				this.perchPoint.enabled = (this.currentState == Flower.FlowerState.Healthy);
			}
			if (updateVisual)
			{
				this.LocalUpdateFlowers(newState, isWatered);
			}
		}

		// Token: 0x06004CAA RID: 19626 RVA: 0x0017C304 File Offset: 0x0017A504
		private void LocalUpdateFlowers(Flower.FlowerState state, bool isWatered = false)
		{
			GameObject[] array = this.meshStates;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (!this.shouldUpdateVisuals)
			{
				this.meshStates[(int)this.currentState].SetActive(true);
				return;
			}
			if (isWatered && this.wateredFx)
			{
				this.wateredFx.Play();
			}
			this.meshRenderer.enabled = true;
			this.anim.enabled = true;
			switch (state)
			{
			case Flower.FlowerState.Healthy:
				this.anim.SetTrigger(Flower.middle_to_healthy);
				return;
			case Flower.FlowerState.Middle:
				if (this.lastState == Flower.FlowerState.Wilted)
				{
					this.anim.SetTrigger(Flower.wilted_to_middle);
					return;
				}
				this.anim.SetTrigger(Flower.healthy_to_middle);
				return;
			case Flower.FlowerState.Wilted:
				this.anim.SetTrigger(Flower.middle_to_wilted);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004CAB RID: 19627 RVA: 0x0017C3DD File Offset: 0x0017A5DD
		private void HandleOnFlowerTimerEnded(GorillaTimer _timer)
		{
			if (!FlowersManager.Instance.IsMine)
			{
				return;
			}
			if (this.timer == _timer)
			{
				this.WaterFlower(false);
			}
		}

		// Token: 0x06004CAC RID: 19628 RVA: 0x0017C401 File Offset: 0x0017A601
		private void ChangeState(Flower.FlowerState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
		}

		// Token: 0x06004CAD RID: 19629 RVA: 0x0017C416 File Offset: 0x0017A616
		public Flower.FlowerState GetCurrentState()
		{
			return this.currentState;
		}

		// Token: 0x06004CAE RID: 19630 RVA: 0x0017C420 File Offset: 0x0017A620
		public void OnAnimationIsDone(int state)
		{
			if (this.meshRenderer.enabled)
			{
				for (int i = 0; i < this.meshStates.Length; i++)
				{
					bool active = i == (int)this.currentState;
					this.meshStates[i].SetActive(active);
				}
				this.anim.enabled = false;
				this.meshRenderer.enabled = false;
			}
		}

		// Token: 0x06004CAF RID: 19631 RVA: 0x0017C47D File Offset: 0x0017A67D
		public void UpdateVisuals(bool enable)
		{
			this.shouldUpdateVisuals = enable;
			this.meshStatesGameObject.SetActive(enable);
		}

		// Token: 0x06004CB0 RID: 19632 RVA: 0x0017C494 File Offset: 0x0017A694
		public void AnimCatch()
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.OnAnimationIsDone(0);
			}
		}

		// Token: 0x040055B9 RID: 21945
		private Animator anim;

		// Token: 0x040055BA RID: 21946
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x040055BB RID: 21947
		[HideInInspector]
		public GorillaTimer timer;

		// Token: 0x040055BC RID: 21948
		private BeePerchPoint perchPoint;

		// Token: 0x040055BD RID: 21949
		public ParticleSystem wateredFx;

		// Token: 0x040055BE RID: 21950
		public ParticleSystem sparkleFx;

		// Token: 0x040055BF RID: 21951
		public GameObject meshStatesGameObject;

		// Token: 0x040055C0 RID: 21952
		public GameObject[] meshStates;

		// Token: 0x040055C1 RID: 21953
		private static readonly int healthy_to_middle = Animator.StringToHash("healthy_to_middle");

		// Token: 0x040055C2 RID: 21954
		private static readonly int middle_to_healthy = Animator.StringToHash("middle_to_healthy");

		// Token: 0x040055C3 RID: 21955
		private static readonly int wilted_to_middle = Animator.StringToHash("wilted_to_middle");

		// Token: 0x040055C4 RID: 21956
		private static readonly int middle_to_wilted = Animator.StringToHash("middle_to_wilted");

		// Token: 0x040055C5 RID: 21957
		private Flower.FlowerState currentState;

		// Token: 0x040055C6 RID: 21958
		private string id;

		// Token: 0x040055C7 RID: 21959
		private bool shouldUpdateVisuals;

		// Token: 0x040055C8 RID: 21960
		private Flower.FlowerState lastState;

		// Token: 0x02000C2C RID: 3116
		public enum FlowerState
		{
			// Token: 0x040055CB RID: 21963
			None = -1,
			// Token: 0x040055CC RID: 21964
			Healthy,
			// Token: 0x040055CD RID: 21965
			Middle,
			// Token: 0x040055CE RID: 21966
			Wilted
		}
	}
}
