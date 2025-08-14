using System;
using UnityEngine;

// Token: 0x02000400 RID: 1024
public class RotationAnimation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000290 RID: 656
	// (get) Token: 0x060017F4 RID: 6132 RVA: 0x0008049D File Offset: 0x0007E69D
	// (set) Token: 0x060017F5 RID: 6133 RVA: 0x000804A5 File Offset: 0x0007E6A5
	public bool TickRunning { get; set; }

	// Token: 0x060017F6 RID: 6134 RVA: 0x000804B0 File Offset: 0x0007E6B0
	public void Tick()
	{
		Vector3 vector = Vector3.zero;
		vector.x = this.amplitude.x * this.x.Evaluate((Time.time - this.baseTime) * this.period.x % 1f);
		vector.y = this.amplitude.y * this.y.Evaluate((Time.time - this.baseTime) * this.period.y % 1f);
		vector.z = this.amplitude.z * this.z.Evaluate((Time.time - this.baseTime) * this.period.z % 1f);
		if (this.releaseSet)
		{
			float num = this.release.Evaluate(Time.time - this.releaseTime);
			vector *= num;
			if (num < Mathf.Epsilon)
			{
				base.enabled = false;
			}
		}
		base.transform.localRotation = Quaternion.Euler(vector) * this.baseRotation;
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x000805CA File Offset: 0x0007E7CA
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x000805DD File Offset: 0x0007E7DD
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.releaseSet = false;
		this.baseTime = Time.time;
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x000805F7 File Offset: 0x0007E7F7
	public void ReleaseToDisable()
	{
		this.releaseSet = true;
		this.releaseTime = Time.time;
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x0008060B File Offset: 0x0007E80B
	public void CancelRelease()
	{
		this.releaseSet = false;
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x00080614 File Offset: 0x0007E814
	private void OnDisable()
	{
		base.transform.localRotation = this.baseRotation;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04001FC1 RID: 8129
	[SerializeField]
	private AnimationCurve x;

	// Token: 0x04001FC2 RID: 8130
	[SerializeField]
	private AnimationCurve y;

	// Token: 0x04001FC3 RID: 8131
	[SerializeField]
	private AnimationCurve z;

	// Token: 0x04001FC4 RID: 8132
	[SerializeField]
	private AnimationCurve attack;

	// Token: 0x04001FC5 RID: 8133
	[SerializeField]
	private AnimationCurve release;

	// Token: 0x04001FC6 RID: 8134
	[SerializeField]
	private Vector3 amplitude = Vector3.one;

	// Token: 0x04001FC7 RID: 8135
	[SerializeField]
	private Vector3 period = Vector3.one;

	// Token: 0x04001FC8 RID: 8136
	private Quaternion baseRotation;

	// Token: 0x04001FC9 RID: 8137
	private float baseTime;

	// Token: 0x04001FCA RID: 8138
	private float releaseTime;

	// Token: 0x04001FCB RID: 8139
	private bool releaseSet;
}
