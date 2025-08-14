using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x060004E0 RID: 1248 RVA: 0x0001C826 File Offset: 0x0001AA26
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0001C830 File Offset: 0x0001AA30
	public void EnableLazer(Transform[] eyes_, VRRig rig_)
	{
		if (rig_ == this.rig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.rig = rig_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].positionCount = 2;
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001C880 File Offset: 0x0001AA80
	public void DisableLazer()
	{
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].positionCount = 0;
			}
		}
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001C8BC File Offset: 0x0001AABC
	private void Update()
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.rig.transform.position);
		}
	}

	// Token: 0x040005CE RID: 1486
	private Transform[] eyeBones;

	// Token: 0x040005CF RID: 1487
	private VRRig rig;

	// Token: 0x040005D0 RID: 1488
	public LineRenderer[] lines;
}
