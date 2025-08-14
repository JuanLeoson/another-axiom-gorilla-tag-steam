using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001BB RID: 443
public class ProximityEffect : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x0003AD0D File Offset: 0x00038F0D
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.enableVisualization = false;
		if (this.visualizer)
		{
			Object.Destroy(this.visualizer);
		}
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003AD3C File Offset: 0x00038F3C
	public void AddTrigger()
	{
		if (this.numTriggers < this.triggersToActivate)
		{
			this.numTriggers++;
			if (this.numTriggers == this.triggersToActivate)
			{
				this.centerTransform.position = (this.leftTransform.position + this.rightTransform.position) / 2f;
				TickSystem<object>.AddTickCallback(this);
			}
		}
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0003ADAC File Offset: 0x00038FAC
	public void RemoveTrigger()
	{
		if (this.numTriggers > 0)
		{
			if (this.numTriggers == this.triggersToActivate)
			{
				UnityEvent<float> unityEvent = this.onScoreCalculated;
				if (unityEvent != null)
				{
					unityEvent.Invoke(0f);
				}
				ProximityEffect.ProximityEvent[] array = this.events;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ResetAllEvents();
				}
				TickSystem<object>.RemoveTickCallback(this);
			}
			this.numTriggers--;
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0003AE18 File Offset: 0x00039018
	private void CalculateProximityScore()
	{
		float num;
		Vector3 vector;
		this.CalculateProximityScore(true, out num, out vector);
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0003AE30 File Offset: 0x00039030
	private void CalculateProximityScore(out float score, out Vector3 midpoint)
	{
		this.CalculateProximityScore(false, out score, out midpoint);
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0003AE3C File Offset: 0x0003903C
	private void CalculateProximityScore(bool drawGizmos, out float score, out Vector3 midpoint)
	{
		float d = (this.rig != null) ? this.rig.scaleFactor : 1f;
		Vector3 position = this.leftTransform.position;
		Vector3 position2 = this.rightTransform.position;
		Vector3 forward = this.leftTransform.forward;
		Vector3 forward2 = this.rightTransform.forward;
		Vector3 a = (position2 - position) / d;
		float magnitude = a.magnitude;
		Vector3 vector = a / magnitude;
		float num = this.scoreCurves.distanceModifierCurve.Evaluate(magnitude);
		float num2 = this.scoreCurves.alignmentModifierCurve.Evaluate(-Vector3.Dot(forward, forward2));
		float num3 = this.scoreCurves.parallelModifierCurve.Evaluate((Vector3.Dot(forward, vector) + Vector3.Dot(forward2, -vector)) / 2f);
		score = num * num2 * num3;
		midpoint = position + 0.5f * a;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003AF3C File Offset: 0x0003913C
	private void MoveTransform(Transform target, float score, Vector3 midpoint)
	{
		Vector3 vector;
		Quaternion a;
		target.GetPositionAndRotation(out vector, out a);
		Vector3 vector2 = Vector3.Lerp(vector, midpoint, ProximityEffect.<MoveTransform>g__ExpT|32_0(this.positionCTLerpSpeed));
		if (this.rotateCT)
		{
			Vector3 vector3 = (vector2 - vector) / Time.deltaTime;
			if (vector3 != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(vector3);
				Quaternion a2 = Quaternion.LookRotation(vector2 - this.rig.syncPos);
				Quaternion rotation = Quaternion.Slerp(a, Quaternion.Slerp(a2, b, vector3.magnitude), ProximityEffect.<MoveTransform>g__ExpT|32_0(this.rotationCTLerpSpeed));
				target.SetPositionAndRotation(vector2, rotation);
			}
		}
		else
		{
			target.position = vector2;
		}
		if (this.scaleCT)
		{
			target.localScale = Vector3.Lerp(target.localScale, score * this.scaleCTMult * Vector3.one, ProximityEffect.<MoveTransform>g__ExpT|32_0(this.scaleCTLerpSpeed));
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000B09 RID: 2825 RVA: 0x0003B018 File Offset: 0x00039218
	// (set) Token: 0x06000B0A RID: 2826 RVA: 0x0003B020 File Offset: 0x00039220
	public bool TickRunning { get; set; }

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003B02C File Offset: 0x0003922C
	public void Tick()
	{
		float num;
		Vector3 midpoint;
		this.CalculateProximityScore(out num, out midpoint);
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num);
		}
		if (this.centerTransform != null)
		{
			this.MoveTransform(this.centerTransform, num, midpoint);
		}
		this.anyAboveThreshold = false;
		foreach (ProximityEffect.ProximityEvent proximityEvent in this.events)
		{
			this.anyAboveThreshold = (proximityEvent.Evaluate(num) || this.anyAboveThreshold);
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0003B131 File Offset: 0x00039331
	[CompilerGenerated]
	internal static float <MoveTransform>g__ExpT|32_0(float speed)
	{
		return 1f - Mathf.Exp(-speed * Time.deltaTime);
	}

	// Token: 0x04000D6A RID: 3434
	[SerializeField]
	private Transform leftTransform;

	// Token: 0x04000D6B RID: 3435
	[SerializeField]
	private Transform rightTransform;

	// Token: 0x04000D6C RID: 3436
	[SerializeField]
	[Tooltip("How many times AddTrigger() needs to be called before the events are allowed to be invoked. Used for pausing events until certain actions are performed (like squeezing the triggers of both controllers).")]
	private int triggersToActivate;

	// Token: 0x04000D6D RID: 3437
	[Space]
	[SerializeField]
	[Tooltip("The transform that moves to follow the midpoint of the left and right transforms.")]
	private Transform centerTransform;

	// Token: 0x04000D6E RID: 3438
	private const string SHOW_CONDITION = "@centerTransform != null";

	// Token: 0x04000D6F RID: 3439
	[SerializeField]
	private float positionCTLerpSpeed = 10f;

	// Token: 0x04000D70 RID: 3440
	[SerializeField]
	private bool rotateCT;

	// Token: 0x04000D71 RID: 3441
	private const string SHOW_ROTATE_CONDITION = "@centerTransform != null && rotateCT";

	// Token: 0x04000D72 RID: 3442
	[SerializeField]
	private float rotationCTLerpSpeed = 10f;

	// Token: 0x04000D73 RID: 3443
	[SerializeField]
	private bool scaleCT;

	// Token: 0x04000D74 RID: 3444
	private const string SHOW_SCALE_CONDITION = "@centerTransform != null && scaleCT";

	// Token: 0x04000D75 RID: 3445
	[SerializeField]
	private float scaleCTLerpSpeed = 10f;

	// Token: 0x04000D76 RID: 3446
	[SerializeField]
	private float scaleCTMult = 1f;

	// Token: 0x04000D77 RID: 3447
	[Space]
	[SerializeField]
	[Tooltip("The curves that get evaluated to determine the alignment score. They get multiplied together, so their Y values should all range from 0-1. The result is compared against the thresholds of the ProximityEvents.")]
	private ProximityEffectScoreCurvesSO scoreCurves;

	// Token: 0x04000D78 RID: 3448
	[Space]
	public UnityEvent<float> onScoreCalculated;

	// Token: 0x04000D79 RID: 3449
	[SerializeField]
	private ProximityEffect.ProximityEvent[] events;

	// Token: 0x04000D7A RID: 3450
	[Header("Editor Only")]
	[SerializeField]
	private Vector3 defaultLeftHandLocalPosition = new Vector3(-0.0568f, 0.04311f, 0.00249f);

	// Token: 0x04000D7B RID: 3451
	[SerializeField]
	private Vector3 defaultLeftHandLocalEuler = new Vector3(173.176f, 80.201f, 3.615f);

	// Token: 0x04000D7C RID: 3452
	[Header("Visualization is currently NOT WORKING due to tick optimization")]
	[SerializeField]
	private bool enableVisualization = true;

	// Token: 0x04000D7D RID: 3453
	[SerializeField]
	private Material visualizationMaterial;

	// Token: 0x04000D7E RID: 3454
	[SerializeField]
	[Range(0f, 1f)]
	private float visualizationLineThickness = 0.01f;

	// Token: 0x04000D7F RID: 3455
	[SerializeField]
	[HideInInspector]
	private LineRenderer visualizer;

	// Token: 0x04000D80 RID: 3456
	private VRRig rig;

	// Token: 0x04000D81 RID: 3457
	private bool anyAboveThreshold;

	// Token: 0x04000D82 RID: 3458
	private int numTriggers;

	// Token: 0x020001BC RID: 444
	[Serializable]
	private class ProximityEvent
	{
		// Token: 0x06000B0E RID: 2830 RVA: 0x0003B148 File Offset: 0x00039348
		public bool Evaluate(float score)
		{
			if (score >= this.highThreshold)
			{
				if (!this.wasAboveThreshold && Time.time - this.lastThresholdTime >= this.highThresholdBufferTime)
				{
					UnityEvent unityEvent = this.onThresholdHigh;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.wasAboveThreshold = true;
					this.wasBelowThreshold = false;
				}
				if (this.wasAboveThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
				return true;
			}
			if (score < this.lowThreshold)
			{
				if (!this.wasBelowThreshold && Time.time - this.lastThresholdTime >= this.lowThresholdBufferTime)
				{
					UnityEvent unityEvent2 = this.onThresholdLow;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasAboveThreshold = false;
					this.wasBelowThreshold = true;
				}
				if (this.wasBelowThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
			}
			return false;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0003B206 File Offset: 0x00039406
		public void ResetAllEvents()
		{
			this.wasAboveThreshold = false;
			this.wasBelowThreshold = true;
		}

		// Token: 0x04000D84 RID: 3460
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("High-threshold events will only fire if the alignment score is above this value.")]
		private float highThreshold = 0.5f;

		// Token: 0x04000D85 RID: 3461
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the high-threshold events.")]
		private float highThresholdBufferTime;

		// Token: 0x04000D86 RID: 3462
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Low-threshold events will only fire if the alignment score is below this value.")]
		private float lowThreshold = 0.3f;

		// Token: 0x04000D87 RID: 3463
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the low-threshold events.")]
		private float lowThresholdBufferTime;

		// Token: 0x04000D88 RID: 3464
		public UnityEvent onThresholdHigh;

		// Token: 0x04000D89 RID: 3465
		public UnityEvent onThresholdLow;

		// Token: 0x04000D8A RID: 3466
		private bool wasAboveThreshold;

		// Token: 0x04000D8B RID: 3467
		private bool wasBelowThreshold = true;

		// Token: 0x04000D8C RID: 3468
		private float lastThresholdTime = -100f;
	}
}
