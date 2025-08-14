using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x06000C64 RID: 3172 RVA: 0x00042E2C File Offset: 0x0004102C
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00042E57 File Offset: 0x00041057
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00042E60 File Offset: 0x00041060
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, DateTimeKind.Utc);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, DateTimeKind.Utc);
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x00042ED0 File Offset: 0x000410D0
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x04000F59 RID: 3929
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x04000F5A RID: 3930
	public int activationMonth = 4;

	// Token: 0x04000F5B RID: 3931
	public int activationDay = 1;

	// Token: 0x04000F5C RID: 3932
	public int activationHour = 7;

	// Token: 0x04000F5D RID: 3933
	public int activationMinute;

	// Token: 0x04000F5E RID: 3934
	public int activationSecond;

	// Token: 0x04000F5F RID: 3935
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x04000F60 RID: 3936
	public int deactivationMonth = 4;

	// Token: 0x04000F61 RID: 3937
	public int deactivationDay = 2;

	// Token: 0x04000F62 RID: 3938
	public int deactivationHour = 7;

	// Token: 0x04000F63 RID: 3939
	public int deactivationMinute;

	// Token: 0x04000F64 RID: 3940
	public int deactivationSecond;

	// Token: 0x04000F65 RID: 3941
	public GameObject[] gameObjectsToActivate;

	// Token: 0x04000F66 RID: 3942
	private bool isActive;

	// Token: 0x04000F67 RID: 3943
	private DateTime activationTime;

	// Token: 0x04000F68 RID: 3944
	private DateTime deactivationTime;

	// Token: 0x04000F69 RID: 3945
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x04000F6A RID: 3946
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}
