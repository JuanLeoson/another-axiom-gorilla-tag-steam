using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060005BB RID: 1467 RVA: 0x000213E1 File Offset: 0x0001F5E1
	// (set) Token: 0x060005BC RID: 1468 RVA: 0x000213E9 File Offset: 0x0001F5E9
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060005BD RID: 1469 RVA: 0x000213F2 File Offset: 0x0001F5F2
	// (set) Token: 0x060005BE RID: 1470 RVA: 0x000213FA File Offset: 0x0001F5FA
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x00021403 File Offset: 0x0001F603
	// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0002140B File Offset: 0x0001F60B
	public Vector3 handPos { get; private set; }

	// Token: 0x060005C1 RID: 1473 RVA: 0x00021414 File Offset: 0x0001F614
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00021428 File Offset: 0x0001F628
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00021489 File Offset: 0x0001F689
	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00021489 File Offset: 0x0001F689
	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00021494 File Offset: 0x0001F694
	public void TriggeredLateUpdate()
	{
		Vector3 vector;
		Quaternion lhs;
		base.transform.GetPositionAndRotation(out vector, out lhs);
		Vector3 b = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			b = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 linear = (vector - this.lastPos) / Time.deltaTime - b;
		Vector3 vector2 = (lhs * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector2.x > 180f)
		{
			vector2.x -= 360f;
		}
		if (vector2.y > 180f)
		{
			vector2.y -= 360f;
		}
		if (vector2.z > 180f)
		{
			vector2.z -= 360f;
		}
		vector2 *= 0.017453292f / Time.fixedDeltaTime;
		this.history[this.currentFrame % this.numFrames] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = linear,
			angular = vector2
		};
		this.linearVelocity = this.history[0].linear;
		this.angularVelocity = this.history[0].angular;
		for (int i = 1; i < this.numFrames; i++)
		{
			this.linearVelocity += this.history[i].linear;
			this.angularVelocity += this.history[i].angular;
		}
		this.linearVelocity /= (float)this.numFrames;
		this.angularVelocity /= (float)this.numFrames;
		this.handPos = vector;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = vector;
		this.lastRotation = lhs;
	}

	// Token: 0x040006E1 RID: 1761
	[Min(1f)]
	[SerializeField]
	private int numFrames = 8;

	// Token: 0x040006E5 RID: 1765
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x040006E6 RID: 1766
	private int currentFrame;

	// Token: 0x040006E7 RID: 1767
	private Vector3 lastPos;

	// Token: 0x040006E8 RID: 1768
	private Quaternion lastRotation;

	// Token: 0x040006E9 RID: 1769
	private Vector3 lastRotationVec;

	// Token: 0x040006EA RID: 1770
	public bool useGlobalSpace;

	// Token: 0x020000E8 RID: 232
	public struct VelocityHistorySample
	{
		// Token: 0x040006EB RID: 1771
		public Vector3 linear;

		// Token: 0x040006EC RID: 1772
		public Vector3 angular;
	}
}
