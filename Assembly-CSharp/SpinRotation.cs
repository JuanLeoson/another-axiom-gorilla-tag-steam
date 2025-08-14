using System;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000B22 RID: 2850 RVA: 0x0003B4CC File Offset: 0x000396CC
	// (set) Token: 0x06000B23 RID: 2851 RVA: 0x0003B4D4 File Offset: 0x000396D4
	public bool TickRunning { get; set; }

	// Token: 0x06000B24 RID: 2852 RVA: 0x0003B4DD File Offset: 0x000396DD
	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0003B511 File Offset: 0x00039711
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0003B524 File Offset: 0x00039724
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0002EF9D File Offset: 0x0002D19D
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04000DA1 RID: 3489
	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	// Token: 0x04000DA2 RID: 3490
	private Quaternion baseRotation;

	// Token: 0x04000DA3 RID: 3491
	private float baseTime;
}
