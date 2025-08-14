using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000770 RID: 1904
public class MovingPlatform : BasePlatform
{
	// Token: 0x06002FAB RID: 12203 RVA: 0x000FB570 File Offset: 0x000F9770
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x000FB57F File Offset: 0x000F977F
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x000FB58E File Offset: 0x000F978E
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x000FB5B7 File Offset: 0x000F97B7
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x000FB5C8 File Offset: 0x000F97C8
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x000FB5F3 File Offset: 0x000F97F3
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x000FB604 File Offset: 0x000F9804
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x000FB666 File Offset: 0x000F9866
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x000FB680 File Offset: 0x000F9880
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x000FB750 File Offset: 0x000F9950
	private void OnEnable()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		base.transform.localRotation = this.initLocalRotation;
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x000FB7D9 File Offset: 0x000F99D9
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x000FB7F4 File Offset: 0x000F99F4
	private Vector3 UpdateArc()
	{
		float angle = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 b = Quaternion.AngleAxis(angle, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + b;
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x000FB852 File Offset: 0x000F9A52
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x000FB86B File Offset: 0x000F9A6B
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x000FB898 File Offset: 0x000F9A98
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x000FB968 File Offset: 0x000F9B68
	private void Update()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		bool flag = false;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			flag = true;
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			flag = true;
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			flag = true;
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			if (flag)
			{
				this.rb.MoveRotation(quaternion);
			}
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			if (flag)
			{
				base.transform.rotation = quaternion;
			}
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000FBA89 File Offset: 0x000F9C89
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04003BA6 RID: 15270
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04003BA7 RID: 15271
	public float cycleLength;

	// Token: 0x04003BA8 RID: 15272
	public float smoothingHalflife = 0.1f;

	// Token: 0x04003BA9 RID: 15273
	public float rotateStartAmt;

	// Token: 0x04003BAA RID: 15274
	public float rotateAmt;

	// Token: 0x04003BAB RID: 15275
	public bool reverseDirOnCycle = true;

	// Token: 0x04003BAC RID: 15276
	public bool reverseDir;

	// Token: 0x04003BAD RID: 15277
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04003BAE RID: 15278
	private Rigidbody rb;

	// Token: 0x04003BAF RID: 15279
	public Transform startXf;

	// Token: 0x04003BB0 RID: 15280
	public Transform endXf;

	// Token: 0x04003BB1 RID: 15281
	public Vector3 platformInitLocalPos;

	// Token: 0x04003BB2 RID: 15282
	private Vector3 startPos;

	// Token: 0x04003BB3 RID: 15283
	private Vector3 endPos;

	// Token: 0x04003BB4 RID: 15284
	private Quaternion startRot;

	// Token: 0x04003BB5 RID: 15285
	private Quaternion endRot;

	// Token: 0x04003BB6 RID: 15286
	public float startPercentage;

	// Token: 0x04003BB7 RID: 15287
	public float startDelay;

	// Token: 0x04003BB8 RID: 15288
	public bool startNextCycle;

	// Token: 0x04003BB9 RID: 15289
	public Transform pivot;

	// Token: 0x04003BBA RID: 15290
	private Quaternion initLocalRotation;

	// Token: 0x04003BBB RID: 15291
	private Vector3 initOffset;

	// Token: 0x04003BBC RID: 15292
	private float currT;

	// Token: 0x04003BBD RID: 15293
	private float percent;

	// Token: 0x04003BBE RID: 15294
	private float smoothedPercent = -1f;

	// Token: 0x04003BBF RID: 15295
	private bool currForward;

	// Token: 0x04003BC0 RID: 15296
	private float dtSinceServerUpdate;

	// Token: 0x04003BC1 RID: 15297
	private double lastServerTime;

	// Token: 0x04003BC2 RID: 15298
	public Vector3 currentVelocity;

	// Token: 0x04003BC3 RID: 15299
	public Vector3 rotationalAxis;

	// Token: 0x04003BC4 RID: 15300
	public float angularVelocity;

	// Token: 0x04003BC5 RID: 15301
	public Vector3 rotationPivot;

	// Token: 0x04003BC6 RID: 15302
	public Vector3 lastPos;

	// Token: 0x04003BC7 RID: 15303
	public Quaternion lastRot;

	// Token: 0x04003BC8 RID: 15304
	public Vector3 deltaPosition;

	// Token: 0x04003BC9 RID: 15305
	public bool debugMovement;

	// Token: 0x04003BCA RID: 15306
	private double lastNT;

	// Token: 0x04003BCB RID: 15307
	private float lastT;

	// Token: 0x02000771 RID: 1905
	public enum PlatformType
	{
		// Token: 0x04003BCD RID: 15309
		PointToPoint,
		// Token: 0x04003BCE RID: 15310
		Arc,
		// Token: 0x04003BCF RID: 15311
		Rotation,
		// Token: 0x04003BD0 RID: 15312
		Child,
		// Token: 0x04003BD1 RID: 15313
		ContinuousRotation
	}
}
