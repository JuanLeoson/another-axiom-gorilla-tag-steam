using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F30 RID: 3888
	public class BackpackGrabbableCosmetic : HoldableObject
	{
		// Token: 0x06006067 RID: 24679 RVA: 0x001EA645 File Offset: 0x001E8845
		private void Awake()
		{
			this.currentItemsCount = this.startItemsCount;
			this.canGrab = true;
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void DropItemCleanup()
		{
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x001EA65A File Offset: 0x001E885A
		public void Update()
		{
			if (!this.canGrab && Time.time - this.lastGrabTime >= this.coolDownTimer)
			{
				this.canGrab = true;
			}
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x001EA680 File Offset: 0x001E8880
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.IsEmpty())
			{
				Debug.LogWarning("Can't remove item, Backpack is empty, need to refill.");
				return;
			}
			if (!this.canGrab)
			{
				return;
			}
			this.lastGrabTime = Time.time;
			this.canGrab = false;
			SnowballThrowable snowballThrowable;
			((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
			this.RemoveItem();
		}

		// Token: 0x0600606C RID: 24684 RVA: 0x001EA6EF File Offset: 0x001E88EF
		public void AddItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.maxCapacity <= this.currentItemsCount)
			{
				Debug.LogWarning("Can't add item, backpack is at full capacity.");
				return;
			}
			this.currentItemsCount++;
			this.UpdateState();
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x001EA727 File Offset: 0x001E8927
		public void RemoveItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount < 0)
			{
				Debug.LogWarning("Can't remove item, Backpack is empty.");
				return;
			}
			this.currentItemsCount--;
			this.UpdateState();
		}

		// Token: 0x0600606E RID: 24686 RVA: 0x001EA75A File Offset: 0x001E895A
		public void RefillBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.startItemsCount)
			{
				return;
			}
			this.currentItemsCount = this.startItemsCount;
			this.UpdateState();
		}

		// Token: 0x0600606F RID: 24687 RVA: 0x001EA786 File Offset: 0x001E8986
		public void EmptyBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == 0)
			{
				return;
			}
			this.currentItemsCount = 0;
			this.UpdateState();
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x001EA7A7 File Offset: 0x001E89A7
		public bool IsFull()
		{
			return !this.useCapacity || this.maxCapacity == this.currentItemsCount;
		}

		// Token: 0x06006071 RID: 24689 RVA: 0x001EA7C2 File Offset: 0x001E89C2
		public bool IsEmpty()
		{
			return this.useCapacity && this.currentItemsCount == 0;
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x001EA7D8 File Offset: 0x001E89D8
		private void UpdateState()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.maxCapacity)
			{
				UnityEvent onReachedMaxCapacity = this.OnReachedMaxCapacity;
				if (onReachedMaxCapacity == null)
				{
					return;
				}
				onReachedMaxCapacity.Invoke();
				return;
			}
			else
			{
				if (this.currentItemsCount != 0)
				{
					if (this.currentItemsCount == this.startItemsCount)
					{
						UnityEvent onRefilled = this.OnRefilled;
						if (onRefilled == null)
						{
							return;
						}
						onRefilled.Invoke();
					}
					return;
				}
				UnityEvent onFullyEmptied = this.OnFullyEmptied;
				if (onFullyEmptied == null)
				{
					return;
				}
				onFullyEmptied.Invoke();
				return;
			}
		}

		// Token: 0x04006BE7 RID: 27623
		[GorillaSoundLookup]
		public int materialIndex;

		// Token: 0x04006BE8 RID: 27624
		[SerializeField]
		private bool useCapacity = true;

		// Token: 0x04006BE9 RID: 27625
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04006BEA RID: 27626
		[SerializeField]
		private int maxCapacity;

		// Token: 0x04006BEB RID: 27627
		[SerializeField]
		private int startItemsCount;

		// Token: 0x04006BEC RID: 27628
		[Space]
		public UnityEvent OnReachedMaxCapacity;

		// Token: 0x04006BED RID: 27629
		public UnityEvent OnFullyEmptied;

		// Token: 0x04006BEE RID: 27630
		public UnityEvent OnRefilled;

		// Token: 0x04006BEF RID: 27631
		private int currentItemsCount;

		// Token: 0x04006BF0 RID: 27632
		private bool canGrab;

		// Token: 0x04006BF1 RID: 27633
		private float lastGrabTime;
	}
}
