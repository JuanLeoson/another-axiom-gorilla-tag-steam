using System;
using System.Collections.Generic;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A22 RID: 2594
public class WaterInteractionEvents : MonoBehaviour
{
	// Token: 0x06003F4A RID: 16202 RVA: 0x001435B4 File Offset: 0x001417B4
	private void Update()
	{
		if (this.overlappingWaterVolumes.Count < 1)
		{
			if (this.inWater)
			{
				this.onExitWater.Invoke();
			}
			this.inWater = false;
			base.enabled = false;
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
		{
			WaterVolume.SurfaceQuery surfaceQuery;
			if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(this.waterContactSphere.transform.position, out surfaceQuery, false))
			{
				float num = Vector3.Dot(surfaceQuery.surfacePoint - this.waterContactSphere.transform.position, surfaceQuery.surfaceNormal);
				float num2 = Vector3.Dot(surfaceQuery.surfacePoint - Vector3.up * surfaceQuery.maxDepth - base.transform.position, surfaceQuery.surfaceNormal);
				if (num > -this.waterContactSphere.radius && num2 < this.waterContactSphere.radius)
				{
					flag = true;
				}
			}
		}
		bool flag2 = this.inWater;
		this.inWater = flag;
		if (!flag2 && this.inWater)
		{
			this.onEnterWater.Invoke();
			return;
		}
		if (flag2 && !this.inWater)
		{
			this.onExitWater.Invoke();
		}
	}

	// Token: 0x06003F4B RID: 16203 RVA: 0x001436F0 File Offset: 0x001418F0
	protected void OnTriggerEnter(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && !this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Add(component);
			base.enabled = true;
		}
	}

	// Token: 0x06003F4C RID: 16204 RVA: 0x00143730 File Offset: 0x00141930
	protected void OnTriggerExit(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Remove(component);
		}
	}

	// Token: 0x04004BA4 RID: 19364
	public UnityEvent onEnterWater = new UnityEvent();

	// Token: 0x04004BA5 RID: 19365
	public UnityEvent onExitWater = new UnityEvent();

	// Token: 0x04004BA6 RID: 19366
	[SerializeField]
	private SphereCollider waterContactSphere;

	// Token: 0x04004BA7 RID: 19367
	private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

	// Token: 0x04004BA8 RID: 19368
	private bool inWater;
}
