using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D2B RID: 3371
	public class TrackSegment : MonoBehaviour
	{
		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06005367 RID: 21351 RVA: 0x0019CB7F File Offset: 0x0019AD7F
		// (set) Token: 0x06005368 RID: 21352 RVA: 0x0019CB87 File Offset: 0x0019AD87
		public float StartDistance { get; set; }

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x06005369 RID: 21353 RVA: 0x0019CB90 File Offset: 0x0019AD90
		// (set) Token: 0x0600536A RID: 21354 RVA: 0x0019CB98 File Offset: 0x0019AD98
		public float GridSize
		{
			get
			{
				return this._gridSize;
			}
			private set
			{
				this._gridSize = value;
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x0600536B RID: 21355 RVA: 0x0019CBA1 File Offset: 0x0019ADA1
		// (set) Token: 0x0600536C RID: 21356 RVA: 0x0019CBA9 File Offset: 0x0019ADA9
		public int SubDivCount
		{
			get
			{
				return this._subDivCount;
			}
			set
			{
				this._subDivCount = value;
			}
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x0600536D RID: 21357 RVA: 0x0019CBB2 File Offset: 0x0019ADB2
		public TrackSegment.SegmentType Type
		{
			get
			{
				return this._segmentType;
			}
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x0600536E RID: 21358 RVA: 0x0019CBBA File Offset: 0x0019ADBA
		public Pose EndPose
		{
			get
			{
				this.UpdatePose(this.SegmentLength, this._endPose);
				return this._endPose;
			}
		}

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x0600536F RID: 21359 RVA: 0x0019CBD4 File Offset: 0x0019ADD4
		public float Radius
		{
			get
			{
				return 0.5f * this.GridSize;
			}
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x0019CBE2 File Offset: 0x0019ADE2
		public float setGridSize(float size)
		{
			this.GridSize = size;
			return this.GridSize / 0.8f;
		}

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x06005371 RID: 21361 RVA: 0x0019CBF8 File Offset: 0x0019ADF8
		public float SegmentLength
		{
			get
			{
				TrackSegment.SegmentType type = this.Type;
				if (type == TrackSegment.SegmentType.Straight)
				{
					return this.GridSize;
				}
				if (type - TrackSegment.SegmentType.LeftTurn > 1)
				{
					return 1f;
				}
				return 1.5707964f * this.Radius;
			}
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Awake()
		{
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x0019CC30 File Offset: 0x0019AE30
		public void UpdatePose(float distanceIntoSegment, Pose pose)
		{
			if (this.Type == TrackSegment.SegmentType.Straight)
			{
				pose.Position = base.transform.position + distanceIntoSegment * base.transform.forward;
				pose.Rotation = base.transform.rotation;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.LeftTurn)
			{
				float num = distanceIntoSegment / this.SegmentLength;
				float num2 = 1.5707964f * num;
				Vector3 position = new Vector3(this.Radius * Mathf.Cos(num2) - this.Radius, 0f, this.Radius * Mathf.Sin(num2));
				Quaternion rhs = Quaternion.Euler(0f, -num2 * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(position);
				pose.Rotation = base.transform.rotation * rhs;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.RightTurn)
			{
				float num3 = 3.1415927f - 1.5707964f * distanceIntoSegment / this.SegmentLength;
				Vector3 position2 = new Vector3(this.Radius * Mathf.Cos(num3) + this.Radius, 0f, this.Radius * Mathf.Sin(num3));
				Quaternion rhs2 = Quaternion.Euler(0f, (3.1415927f - num3) * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(position2);
				pose.Rotation = base.transform.rotation * rhs2;
				return;
			}
			pose.Position = Vector3.zero;
			pose.Rotation = Quaternion.identity;
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Update()
		{
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x0019CDBC File Offset: 0x0019AFBC
		private void OnDisable()
		{
			Object.Destroy(this._mesh);
		}

		// Token: 0x06005376 RID: 21366 RVA: 0x0019CDCC File Offset: 0x0019AFCC
		private void DrawDebugLines()
		{
			for (int i = 1; i < this.SubDivCount + 1; i++)
			{
				float num = this.SegmentLength / (float)this.SubDivCount;
				this.UpdatePose((float)(i - 1) * num, this._p1);
				this.UpdatePose((float)i * num, this._p2);
				float d = 0.075f;
				Debug.DrawLine(this._p1.Position + d * (this._p1.Rotation * Vector3.right), this._p2.Position + d * (this._p2.Rotation * Vector3.right));
				Debug.DrawLine(this._p1.Position - d * (this._p1.Rotation * Vector3.right), this._p2.Position - d * (this._p2.Rotation * Vector3.right));
			}
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position + 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
		}

		// Token: 0x06005377 RID: 21367 RVA: 0x0019D0D4 File Offset: 0x0019B2D4
		public void RegenerateTrackAndMesh()
		{
			if (base.transform.childCount > 0 && !this._mesh)
			{
				this._mesh = base.transform.GetChild(0).gameObject;
			}
			if (this._mesh)
			{
				Object.DestroyImmediate(this._mesh);
			}
			if (this._segmentType == TrackSegment.SegmentType.LeftTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._leftTurn.gameObject);
			}
			else if (this._segmentType == TrackSegment.SegmentType.RightTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._rightTurn.gameObject);
			}
			else
			{
				this._mesh = Object.Instantiate<GameObject>(this._straight.gameObject);
			}
			this._mesh.transform.SetParent(base.transform, false);
			this._mesh.transform.position += this.GridSize / 2f * base.transform.forward;
			this._mesh.transform.localScale = new Vector3(this.GridSize / 0.8f, this.GridSize / 0.8f, this.GridSize / 0.8f);
		}

		// Token: 0x04005CAD RID: 23725
		[SerializeField]
		private TrackSegment.SegmentType _segmentType;

		// Token: 0x04005CAE RID: 23726
		[SerializeField]
		private MeshFilter _straight;

		// Token: 0x04005CAF RID: 23727
		[SerializeField]
		private MeshFilter _leftTurn;

		// Token: 0x04005CB0 RID: 23728
		[SerializeField]
		private MeshFilter _rightTurn;

		// Token: 0x04005CB1 RID: 23729
		private float _gridSize = 0.8f;

		// Token: 0x04005CB2 RID: 23730
		private int _subDivCount = 20;

		// Token: 0x04005CB3 RID: 23731
		private const float _originalGridSize = 0.8f;

		// Token: 0x04005CB4 RID: 23732
		private const float _trackWidth = 0.15f;

		// Token: 0x04005CB5 RID: 23733
		private GameObject _mesh;

		// Token: 0x04005CB7 RID: 23735
		private Pose _p1 = new Pose();

		// Token: 0x04005CB8 RID: 23736
		private Pose _p2 = new Pose();

		// Token: 0x04005CB9 RID: 23737
		private Pose _endPose = new Pose();

		// Token: 0x02000D2C RID: 3372
		public enum SegmentType
		{
			// Token: 0x04005CBB RID: 23739
			Straight,
			// Token: 0x04005CBC RID: 23740
			LeftTurn,
			// Token: 0x04005CBD RID: 23741
			RightTurn,
			// Token: 0x04005CBE RID: 23742
			Switch
		}
	}
}
