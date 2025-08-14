using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E2D RID: 3629
	public sealed class SplineFollow : MonoBehaviour
	{
		// Token: 0x06005A44 RID: 23108 RVA: 0x001C7694 File Offset: 0x001C5894
		public void Start()
		{
			base.transform.rotation *= this._rotationFix;
			this._smoothRotationTrackingRateExp = Mathf.Exp(this._smoothRotationTrackingRate);
			this._progress = this._splineProgressOffset;
			this._progressPerFixedUpdate = Time.fixedDeltaTime / this._duration;
			this._secondsToCycles = (double)(1f / this._duration);
			this._nativeSpline = new NativeSpline(this._unitySpline.Spline, this._unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			if (this._approximate)
			{
				this.CalculateApproximationNodes();
			}
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x001C773C File Offset: 0x001C593C
		private void CalculateApproximationNodes()
		{
			for (int i = 0; i < this._approximationResolution; i++)
			{
				float3 v;
				float3 v2;
				float3 v3;
				this._nativeSpline.Evaluate((float)i / (float)this._approximationResolution, out v, out v2, out v3);
				SplineFollow.SplineNode item = new SplineFollow.SplineNode(v, v2, v3);
				this._approximationNodes.Add(item);
			}
			if (this._nativeSpline.Closed)
			{
				this._approximationNodes.Add(this._approximationNodes[0]);
			}
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x001C77C0 File Offset: 0x001C59C0
		private void FixedUpdate()
		{
			if (!this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x06005A47 RID: 23111 RVA: 0x001C77D0 File Offset: 0x001C59D0
		private void Update()
		{
			if (this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x06005A48 RID: 23112 RVA: 0x001C77E0 File Offset: 0x001C59E0
		private void FollowSpline()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this._secondsToCycles + (double)this._splineProgressOffset;
				this._progress = (float)(num % 1.0);
			}
			else
			{
				this._progress = (this._progress + this._progressPerFixedUpdate) % 1f;
			}
			SplineFollow.SplineNode splineNode = this.EvaluateSpline(this._progress);
			base.transform.position = splineNode.Position;
			Quaternion a = Quaternion.LookRotation(splineNode.Tangent) * this._rotationFix;
			base.transform.rotation = Quaternion.Slerp(a, base.transform.rotation, Mathf.Exp(-this._smoothRotationTrackingRateExp * Time.deltaTime));
		}

		// Token: 0x06005A49 RID: 23113 RVA: 0x001C789C File Offset: 0x001C5A9C
		private SplineFollow.SplineNode EvaluateSpline(float t)
		{
			t %= 1f;
			if (this._approximate)
			{
				float num = t * (float)this._approximationNodes.Count;
				int num2 = (int)num;
				float t2 = num - (float)num2;
				num2 %= this._approximationNodes.Count;
				SplineFollow.SplineNode a = this._approximationNodes[num2];
				SplineFollow.SplineNode b = this._approximationNodes[(num2 + 1) % this._approximationNodes.Count];
				return SplineFollow.SplineNode.Lerp(a, b, t2);
			}
			float3 v;
			float3 v2;
			float3 v3;
			this._nativeSpline.Evaluate(t, out v, out v2, out v3);
			return new SplineFollow.SplineNode(v, v2, v3);
		}

		// Token: 0x06005A4A RID: 23114 RVA: 0x001C7938 File Offset: 0x001C5B38
		private void OnDestroy()
		{
			this._nativeSpline.Dispose();
		}

		// Token: 0x04006506 RID: 25862
		[SerializeField]
		[Tooltip("If true, approximates the spline position. Only use when exact position does not matter.")]
		private bool _approximate;

		// Token: 0x04006507 RID: 25863
		[SerializeField]
		private SplineContainer _unitySpline;

		// Token: 0x04006508 RID: 25864
		[SerializeField]
		private float _duration;

		// Token: 0x04006509 RID: 25865
		private double _secondsToCycles;

		// Token: 0x0400650A RID: 25866
		[SerializeField]
		private float _smoothRotationTrackingRate = 0.5f;

		// Token: 0x0400650B RID: 25867
		private float _smoothRotationTrackingRateExp;

		// Token: 0x0400650C RID: 25868
		private float _progressPerFixedUpdate;

		// Token: 0x0400650D RID: 25869
		[SerializeField]
		private float _splineProgressOffset;

		// Token: 0x0400650E RID: 25870
		[SerializeField]
		private Quaternion _rotationFix = Quaternion.identity;

		// Token: 0x0400650F RID: 25871
		private NativeSpline _nativeSpline;

		// Token: 0x04006510 RID: 25872
		private float _progress;

		// Token: 0x04006511 RID: 25873
		[Header("Approximate Spline Parameters")]
		[SerializeField]
		[Range(4f, 200f)]
		private int _approximationResolution = 100;

		// Token: 0x04006512 RID: 25874
		private readonly List<SplineFollow.SplineNode> _approximationNodes = new List<SplineFollow.SplineNode>();

		// Token: 0x02000E2E RID: 3630
		private struct SplineNode
		{
			// Token: 0x06005A4C RID: 23116 RVA: 0x001C7978 File Offset: 0x001C5B78
			public SplineNode(Vector3 position, Vector3 tangent, Vector3 up)
			{
				this.Position = position;
				this.Tangent = tangent;
				this.Up = up;
			}

			// Token: 0x06005A4D RID: 23117 RVA: 0x001C79A0 File Offset: 0x001C5BA0
			public static SplineFollow.SplineNode Lerp(SplineFollow.SplineNode a, SplineFollow.SplineNode b, float t)
			{
				return new SplineFollow.SplineNode(Vector3.Lerp(a.Position, b.Position, t), Vector3.Lerp(a.Tangent, b.Tangent, t), Vector3.Lerp(a.Up, b.Up, t));
			}

			// Token: 0x04006513 RID: 25875
			public readonly Vector3 Position;

			// Token: 0x04006514 RID: 25876
			public readonly Vector3 Tangent;

			// Token: 0x04006515 RID: 25877
			public readonly Vector3 Up;
		}
	}
}
