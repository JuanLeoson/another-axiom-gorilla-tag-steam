using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000251 RID: 593
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000DD6 RID: 3542 RVA: 0x0005492C File Offset: 0x00052B2C
	public void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTime = Time.time;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00054960 File Offset: 0x00052B60
	public void SliceUpdate()
	{
		float num = Time.time - this.lastTime;
		this.lastTime = Time.time;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num2 = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				Transform transform2 = thermalSourceVolume.transform;
				float x2 = transform2.lossyScale.x;
				float num3 = Vector3.Distance(transform2.position, position);
				float num4 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num3 - thermalReceiver.radius * x);
				num2 += thermalSourceVolume.celsius * num4;
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num2, num * thermalReceiver.conductivity);
			if (!thermalReceiver.wasAboveThreshold && thermalReceiver.celsius > thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = true;
				UnityEvent onAboveThreshold = thermalReceiver.OnAboveThreshold;
				if (onAboveThreshold != null)
				{
					onAboveThreshold.Invoke();
				}
			}
			else if (thermalReceiver.wasAboveThreshold && thermalReceiver.celsius < thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = false;
				UnityEvent onBelowThreshold = thermalReceiver.OnBelowThreshold;
				if (onBelowThreshold != null)
				{
					onBelowThreshold.Invoke();
				}
			}
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00054AC9 File Offset: 0x00052CC9
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00054AD6 File Offset: 0x00052CD6
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00054AE4 File Offset: 0x00052CE4
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00054AF1 File Offset: 0x00052CF1
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x040015A1 RID: 5537
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x040015A2 RID: 5538
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x040015A3 RID: 5539
	[NonSerialized]
	public static ThermalManager instance;

	// Token: 0x040015A4 RID: 5540
	private float lastTime;
}
