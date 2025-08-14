using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001BE RID: 446
public class RadioButtonGroupWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000B12 RID: 2834 RVA: 0x0003B246 File Offset: 0x00039446
	// (set) Token: 0x06000B13 RID: 2835 RVA: 0x0003B24E File Offset: 0x0003944E
	public bool IsSpawned { get; set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000B14 RID: 2836 RVA: 0x0003B257 File Offset: 0x00039457
	// (set) Token: 0x06000B15 RID: 2837 RVA: 0x0003B25F File Offset: 0x0003945F
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000B16 RID: 2838 RVA: 0x0003B268 File Offset: 0x00039468
	private void Start()
	{
		this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.assignedSlot];
		if (!this.ownerRig.isLocal)
		{
			GorillaPressableButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Collider component = array[i].GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003B2C6 File Offset: 0x000394C6
	private void OnEnable()
	{
		this.SharedRefreshState();
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0003B2CE File Offset: 0x000394CE
	private int GetCurrentState()
	{
		return GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003B2F6 File Offset: 0x000394F6
	private void Update()
	{
		if (this.ownerRig.isLocal)
		{
			return;
		}
		if (this.lastReportedState != this.GetCurrentState())
		{
			this.SharedRefreshState();
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0003B31C File Offset: 0x0003951C
	public void SharedRefreshState()
	{
		int currentState = this.GetCurrentState();
		int num = this.AllowSelectNone ? (currentState - 1) : currentState;
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].isOn = (num == i);
			this.buttons[i].UpdateColor();
		}
		if (this.lastReportedState != currentState)
		{
			this.lastReportedState = currentState;
			this.OnSelectionChanged.Invoke(currentState);
		}
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0003B38C File Offset: 0x0003958C
	public void OnPress(GorillaPressableButton button)
	{
		int currentState = this.GetCurrentState();
		int num = Array.IndexOf<GorillaPressableButton>(this.buttons, button);
		if (this.AllowSelectNone)
		{
			num++;
		}
		int value = num;
		if (this.AllowSelectNone && num == currentState)
		{
			value = 0;
		}
		this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, value);
		this.SharedRefreshState();
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0003B3F1 File Offset: 0x000395F1
	public void OnSpawn(VRRig rig)
	{
		this.ownerRig = rig;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x04000D90 RID: 3472
	[SerializeField]
	private bool AllowSelectNone = true;

	// Token: 0x04000D91 RID: 3473
	[SerializeField]
	private GorillaPressableButton[] buttons;

	// Token: 0x04000D92 RID: 3474
	[SerializeField]
	private UnityEvent<int> OnSelectionChanged;

	// Token: 0x04000D93 RID: 3475
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlot = VRRig.WearablePackedStateSlots.Pants1;

	// Token: 0x04000D94 RID: 3476
	private int lastReportedState;

	// Token: 0x04000D95 RID: 3477
	private VRRig ownerRig;

	// Token: 0x04000D96 RID: 3478
	private GTBitOps.BitWriteInfo stateBitsWriteInfo;
}
