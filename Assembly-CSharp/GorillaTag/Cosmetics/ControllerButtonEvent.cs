using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F35 RID: 3893
	public class ControllerButtonEvent : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06006087 RID: 24711 RVA: 0x001EAEB6 File Offset: 0x001E90B6
		// (set) Token: 0x06006088 RID: 24712 RVA: 0x001EAEBE File Offset: 0x001E90BE
		public bool IsSpawned { get; set; }

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06006089 RID: 24713 RVA: 0x001EAEC7 File Offset: 0x001E90C7
		// (set) Token: 0x0600608A RID: 24714 RVA: 0x001EAECF File Offset: 0x001E90CF
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600608B RID: 24715 RVA: 0x001EAED8 File Offset: 0x001E90D8
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDespawn()
		{
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x001EAEE1 File Offset: 0x001E90E1
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x001EAEFE File Offset: 0x001E90FE
		private void Awake()
		{
			this.triggerLastValue = 0f;
			this.gripLastValue = 0f;
			this.primaryLastValue = false;
			this.secondaryLastValue = false;
			this.frameCounter = 0;
		}

		// Token: 0x0600608F RID: 24719 RVA: 0x001EAF2C File Offset: 0x001E912C
		public void LateUpdate()
		{
			if (!this.IsMyItem())
			{
				return;
			}
			XRNode node = this.inLeftHand ? XRNode.LeftHand : XRNode.RightHand;
			switch (this.buttonType)
			{
			case ControllerButtonEvent.ButtonType.trigger:
			{
				float num = ControllerInputPoller.TriggerFloat(node);
				if (num > this.triggerValue)
				{
					this.frameCounter++;
				}
				if (num > this.triggerValue && this.triggerLastValue < this.triggerValue)
				{
					UnityEvent<bool, float> unityEvent = this.onButtonPressed;
					if (unityEvent != null)
					{
						unityEvent.Invoke(this.inLeftHand, num);
					}
				}
				else if (num <= this.triggerReleaseValue && this.triggerLastValue > this.triggerReleaseValue)
				{
					UnityEvent<bool, float> unityEvent2 = this.onButtonReleased;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				else if (num > this.triggerValue && this.triggerLastValue >= this.triggerValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent3 = this.onButtonPressStayed;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				this.triggerLastValue = num;
				return;
			}
			case ControllerButtonEvent.ButtonType.primary:
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(node);
				if (flag)
				{
					this.frameCounter++;
				}
				if (flag && !this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent4 = this.onButtonPressed;
					if (unityEvent4 != null)
					{
						unityEvent4.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag && this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent5 = this.onButtonReleased;
					if (unityEvent5 != null)
					{
						unityEvent5.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent6 = this.onButtonPressStayed;
					if (unityEvent6 != null)
					{
						unityEvent6.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.primaryLastValue = flag;
				return;
			}
			case ControllerButtonEvent.ButtonType.secondary:
			{
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(node);
				if (flag2)
				{
					this.frameCounter++;
				}
				if (flag2 && !this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent7 = this.onButtonPressed;
					if (unityEvent7 != null)
					{
						unityEvent7.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag2 && this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent8 = this.onButtonReleased;
					if (unityEvent8 != null)
					{
						unityEvent8.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent9 = this.onButtonPressStayed;
					if (unityEvent9 != null)
					{
						unityEvent9.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.secondaryLastValue = flag2;
				return;
			}
			case ControllerButtonEvent.ButtonType.grip:
			{
				float num2 = ControllerInputPoller.GripFloat(node);
				if (num2 > this.gripValue)
				{
					this.frameCounter++;
				}
				if (num2 > this.gripValue && this.gripLastValue < this.gripValue)
				{
					UnityEvent<bool, float> unityEvent10 = this.onButtonPressed;
					if (unityEvent10 != null)
					{
						unityEvent10.Invoke(this.inLeftHand, num2);
					}
				}
				else if (num2 <= this.gripReleaseValue && this.gripLastValue > this.gripReleaseValue)
				{
					UnityEvent<bool, float> unityEvent11 = this.onButtonReleased;
					if (unityEvent11 != null)
					{
						unityEvent11.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				else if (num2 > this.gripValue && this.gripLastValue >= this.gripValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent12 = this.onButtonPressStayed;
					if (unityEvent12 != null)
					{
						unityEvent12.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				this.gripLastValue = num2;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04006C14 RID: 27668
		[SerializeField]
		private float gripValue = 0.75f;

		// Token: 0x04006C15 RID: 27669
		[SerializeField]
		private float gripReleaseValue = 0.01f;

		// Token: 0x04006C16 RID: 27670
		[SerializeField]
		private float triggerValue = 0.75f;

		// Token: 0x04006C17 RID: 27671
		[SerializeField]
		private float triggerReleaseValue = 0.01f;

		// Token: 0x04006C18 RID: 27672
		[SerializeField]
		private ControllerButtonEvent.ButtonType buttonType;

		// Token: 0x04006C19 RID: 27673
		[Tooltip("How many frames should pass to trigger a press stayed button")]
		[SerializeField]
		private int frameInterval = 20;

		// Token: 0x04006C1A RID: 27674
		public UnityEvent<bool, float> onButtonPressed;

		// Token: 0x04006C1B RID: 27675
		public UnityEvent<bool, float> onButtonReleased;

		// Token: 0x04006C1C RID: 27676
		public UnityEvent<bool, float> onButtonPressStayed;

		// Token: 0x04006C1D RID: 27677
		private float triggerLastValue;

		// Token: 0x04006C1E RID: 27678
		private float gripLastValue;

		// Token: 0x04006C1F RID: 27679
		private bool primaryLastValue;

		// Token: 0x04006C20 RID: 27680
		private bool secondaryLastValue;

		// Token: 0x04006C21 RID: 27681
		private int frameCounter;

		// Token: 0x04006C22 RID: 27682
		private bool inLeftHand;

		// Token: 0x04006C23 RID: 27683
		private VRRig myRig;

		// Token: 0x02000F36 RID: 3894
		private enum ButtonType
		{
			// Token: 0x04006C27 RID: 27687
			trigger,
			// Token: 0x04006C28 RID: 27688
			primary,
			// Token: 0x04006C29 RID: 27689
			secondary,
			// Token: 0x04006C2A RID: 27690
			grip
		}
	}
}
