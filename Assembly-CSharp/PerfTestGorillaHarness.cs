using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000277 RID: 631
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaHarness : MonoBehaviour
{
	// Token: 0x06000E81 RID: 3713 RVA: 0x00058234 File Offset: 0x00056434
	private void Awake()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in base.GetComponentsInChildren<PerfTestGorillaSlot>())
		{
			if (perfTestGorillaSlot.slotType == PerfTestGorillaSlot.SlotType.VR_PLAYER)
			{
				this._vrSlot = perfTestGorillaSlot;
			}
			else
			{
				this.dummySlots.Add(perfTestGorillaSlot);
			}
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00058278 File Offset: 0x00056478
	private void Update()
	{
		if (!this._isRecording)
		{
			return;
		}
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			float y = perfTestGorillaSlot.localStartPosition.y + Mathf.Sin(Time.time * this.bounceSpeed) * this.bounceAmplitude;
			perfTestGorillaSlot.transform.localPosition = new Vector3(perfTestGorillaSlot.localStartPosition.x, y, perfTestGorillaSlot.localStartPosition.z);
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0005831C File Offset: 0x0005651C
	public void StartRecording()
	{
		this._isRecording = true;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x00058328 File Offset: 0x00056528
	public void StopRecording()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			perfTestGorillaSlot.transform.localPosition = perfTestGorillaSlot.localStartPosition;
		}
		this._isRecording = false;
	}

	// Token: 0x04001765 RID: 5989
	public PerfTestGorillaSlot _vrSlot;

	// Token: 0x04001766 RID: 5990
	public List<PerfTestGorillaSlot> dummySlots = new List<PerfTestGorillaSlot>(9);

	// Token: 0x04001767 RID: 5991
	[OnEnterPlay_Set(false)]
	private bool _isRecording;

	// Token: 0x04001768 RID: 5992
	private float _nextRandomMoveTime;

	// Token: 0x04001769 RID: 5993
	private float bounceSpeed = 5f;

	// Token: 0x0400176A RID: 5994
	private float bounceAmplitude = 0.5f;
}
