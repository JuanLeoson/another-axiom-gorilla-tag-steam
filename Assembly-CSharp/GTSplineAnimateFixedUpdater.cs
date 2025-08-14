using System;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000206 RID: 518
[NetworkBehaviourWeaved(1)]
public class GTSplineAnimateFixedUpdater : NetworkComponent
{
	// Token: 0x06000C3F RID: 3135 RVA: 0x00042663 File Offset: 0x00040863
	protected override void Awake()
	{
		base.Awake();
		this.splineAnimateRef.AddCallbackOnLoad(new Action(this.InitSplineAnimate));
		this.splineAnimateRef.AddCallbackOnUnload(new Action(this.ClearSplineAnimate));
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00042699 File Offset: 0x00040899
	private void InitSplineAnimate()
	{
		this.isSplineLoaded = this.splineAnimateRef.TryResolve<SplineAnimate>(out this.splineAnimate);
		if (this.isSplineLoaded && this.splineAnimate != null)
		{
			this.splineAnimate.enabled = false;
		}
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x000426D4 File Offset: 0x000408D4
	private void ClearSplineAnimate()
	{
		this.splineAnimate = null;
		this.isSplineLoaded = false;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x000426E4 File Offset: 0x000408E4
	private void FixedUpdate()
	{
		if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
		{
			if (this.isSplineLoaded)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f) % this.Duration;
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
				return;
			}
		}
		else
		{
			this.progress = (this.progress + Time.fixedDeltaTime) % this.Duration;
			if (this.isSplineLoaded)
			{
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
			}
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000C43 RID: 3139 RVA: 0x00042799 File Offset: 0x00040999
	// (set) Token: 0x06000C44 RID: 3140 RVA: 0x000427BF File Offset: 0x000409BF
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe float Netdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(float*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(float*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x000427E6 File Offset: 0x000409E6
	public override void WriteDataFusion()
	{
		this.Netdata = this.progress + 1f;
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x000427FA File Offset: 0x000409FA
	public override void ReadDataFusion()
	{
		this.SharedReadData(this.Netdata);
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00042808 File Offset: 0x00040A08
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.progress + 1f);
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00042830 File Offset: 0x00040A30
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float incomingValue = (float)stream.ReceiveNext();
		this.SharedReadData(incomingValue);
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00042860 File Offset: 0x00040A60
	private void SharedReadData(float incomingValue)
	{
		if (float.IsNaN(incomingValue) || incomingValue > this.Duration + 1f || incomingValue < 0f)
		{
			return;
		}
		this.progressLerpEnd = incomingValue;
		if (this.progressLerpEnd < this.progress)
		{
			if (this.progress < this.Duration)
			{
				this.progressLerpEnd += this.Duration;
			}
			else
			{
				this.progress -= this.Duration;
			}
		}
		this.progressLerpStart = this.progress;
		this.progressLerpStartTime = Time.time;
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x000428EF File Offset: 0x00040AEF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Netdata = this._Netdata;
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00042907 File Offset: 0x00040B07
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Netdata = this.Netdata;
	}

	// Token: 0x04000F1B RID: 3867
	[SerializeField]
	private XSceneRef splineAnimateRef;

	// Token: 0x04000F1C RID: 3868
	[SerializeField]
	private float Duration;

	// Token: 0x04000F1D RID: 3869
	private const float progressLerpDuration = 1f;

	// Token: 0x04000F1E RID: 3870
	private SplineAnimate splineAnimate;

	// Token: 0x04000F1F RID: 3871
	private bool isSplineLoaded;

	// Token: 0x04000F20 RID: 3872
	private float progress;

	// Token: 0x04000F21 RID: 3873
	private float progressLerpStart;

	// Token: 0x04000F22 RID: 3874
	private float progressLerpEnd;

	// Token: 0x04000F23 RID: 3875
	private float progressLerpStartTime;

	// Token: 0x04000F24 RID: 3876
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Netdata", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private float _Netdata;
}
