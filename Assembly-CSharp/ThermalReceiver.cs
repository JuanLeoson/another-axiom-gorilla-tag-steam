using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000252 RID: 594
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000DDF RID: 3551 RVA: 0x00054B1F File Offset: 0x00052D1F
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x00054B33 File Offset: 0x00052D33
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00054B3B File Offset: 0x00052D3B
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
		this.wasAboveThreshold = false;
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00054B50 File Offset: 0x00052D50
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00054B58 File Offset: 0x00052D58
	protected void OnDisable()
	{
		this.wasAboveThreshold = false;
		ThermalManager.Unregister(this);
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00054B67 File Offset: 0x00052D67
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x040015A5 RID: 5541
	public float radius = 0.2f;

	// Token: 0x040015A6 RID: 5542
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x040015A7 RID: 5543
	[Tooltip("Optional: Fire events if temperature goes below or above this threshold - Celsius")]
	public float temperatureThreshold;

	// Token: 0x040015A8 RID: 5544
	[Space]
	public UnityEvent OnAboveThreshold;

	// Token: 0x040015A9 RID: 5545
	public UnityEvent OnBelowThreshold;

	// Token: 0x040015AA RID: 5546
	[DebugOption]
	public float celsius;

	// Token: 0x040015AB RID: 5547
	public bool wasAboveThreshold;

	// Token: 0x040015AC RID: 5548
	private float defaultCelsius;
}
