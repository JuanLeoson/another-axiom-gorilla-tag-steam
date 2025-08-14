using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000AEC RID: 2796
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x06004357 RID: 17239 RVA: 0x00153108 File Offset: 0x00151308
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06004358 RID: 17240 RVA: 0x00153118 File Offset: 0x00151318
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
				}
				else
				{
					this.progress = 2f - this.progress;
					this.goingForward = false;
				}
			}
		}
		else
		{
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		if (this.linearSpline != null && this.walkLinearPath)
		{
			Vector3 vector = this.linearSpline.Evaluate(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = vector;
			}
			else
			{
				base.transform.localPosition = vector;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(vector + this.linearSpline.GetForwardTangent(this.progress, 0.01f));
				return;
			}
		}
		else if (this.spline != null)
		{
			Vector3 point = this.spline.GetPoint(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = point;
			}
			else
			{
				base.transform.localPosition = point;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(point + this.spline.GetDirection(this.progress));
			}
		}
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x001532C6 File Offset: 0x001514C6
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x04004E0F RID: 19983
	public BezierSpline spline;

	// Token: 0x04004E10 RID: 19984
	public LinearSpline linearSpline;

	// Token: 0x04004E11 RID: 19985
	public float duration;

	// Token: 0x04004E12 RID: 19986
	public bool lookForward;

	// Token: 0x04004E13 RID: 19987
	public SplineWalkerMode mode;

	// Token: 0x04004E14 RID: 19988
	public bool walkLinearPath;

	// Token: 0x04004E15 RID: 19989
	public bool useWorldPosition;

	// Token: 0x04004E16 RID: 19990
	public float progress;

	// Token: 0x04004E17 RID: 19991
	private bool goingForward = true;

	// Token: 0x04004E18 RID: 19992
	public bool DoNetworkSync = true;

	// Token: 0x04004E19 RID: 19993
	private PhotonView _view;
}
