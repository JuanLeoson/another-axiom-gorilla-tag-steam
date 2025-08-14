using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000ACA RID: 2762
internal class NetworkVector3
{
	// Token: 0x17000656 RID: 1622
	// (get) Token: 0x060042AE RID: 17070 RVA: 0x0014F666 File Offset: 0x0014D866
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x060042AF RID: 17071 RVA: 0x0014F670 File Offset: 0x0014D870
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		ref currentSyncTarget.SetValueSafe(newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x060042B0 RID: 17072 RVA: 0x0014F6B4 File Offset: 0x0014D8B4
	public Vector3 GetPredictedFuture()
	{
		float d = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 b = this.distanceTraveled * d;
		return this._currentSyncTarget + b;
	}

	// Token: 0x060042B1 RID: 17073 RVA: 0x0014F6EF File Offset: 0x0014D8EF
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04004DB6 RID: 19894
	private double lastSetNetTime;

	// Token: 0x04004DB7 RID: 19895
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04004DB8 RID: 19896
	private Vector3 distanceTraveled = Vector3.zero;
}
