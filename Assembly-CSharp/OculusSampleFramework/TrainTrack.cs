using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D36 RID: 3382
	public class TrainTrack : MonoBehaviour
	{
		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060053BD RID: 21437 RVA: 0x0019E290 File Offset: 0x0019C490
		// (set) Token: 0x060053BE RID: 21438 RVA: 0x0019E298 File Offset: 0x0019C498
		public float TrackLength
		{
			get
			{
				return this._trainLength;
			}
			private set
			{
				this._trainLength = value;
			}
		}

		// Token: 0x060053BF RID: 21439 RVA: 0x0019E2A1 File Offset: 0x0019C4A1
		private void Awake()
		{
			this.Regenerate();
		}

		// Token: 0x060053C0 RID: 21440 RVA: 0x0019E2AC File Offset: 0x0019C4AC
		public TrackSegment GetSegment(float distance)
		{
			int childCount = this._segmentParent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment = this._trackSegments[i];
				TrackSegment trackSegment2 = this._trackSegments[(i + 1) % childCount];
				if (distance >= trackSegment.StartDistance && (distance < trackSegment2.StartDistance || i == childCount - 1))
				{
					return trackSegment;
				}
			}
			return null;
		}

		// Token: 0x060053C1 RID: 21441 RVA: 0x0019E304 File Offset: 0x0019C504
		public void Regenerate()
		{
			this._trackSegments = this._segmentParent.GetComponentsInChildren<TrackSegment>();
			this.TrackLength = 0f;
			int childCount = this._segmentParent.childCount;
			TrackSegment trackSegment = null;
			float scale = 0f;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment2 = this._trackSegments[i];
				trackSegment2.SubDivCount = this._subDivCount;
				scale = trackSegment2.setGridSize(this._gridSize);
				if (trackSegment != null)
				{
					Pose endPose = trackSegment.EndPose;
					trackSegment2.transform.position = endPose.Position;
					trackSegment2.transform.rotation = endPose.Rotation;
					trackSegment2.StartDistance = this.TrackLength;
				}
				if (this._regnerateTrackMeshOnAwake)
				{
					trackSegment2.RegenerateTrackAndMesh();
				}
				this.TrackLength += trackSegment2.SegmentLength;
				trackSegment = trackSegment2;
			}
			this.SetScale(scale);
		}

		// Token: 0x060053C2 RID: 21442 RVA: 0x0019E3EC File Offset: 0x0019C5EC
		private void SetScale(float ratio)
		{
			this._trainParent.localScale = new Vector3(ratio, ratio, ratio);
			TrainCar[] componentsInChildren = this._trainParent.GetComponentsInChildren<TrainCar>();
			this._trainParent.GetComponentInChildren<TrainLocomotive>().Scale = ratio;
			TrainCar[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale = ratio;
			}
		}

		// Token: 0x04005D22 RID: 23842
		[SerializeField]
		private float _gridSize = 0.5f;

		// Token: 0x04005D23 RID: 23843
		[SerializeField]
		private int _subDivCount = 20;

		// Token: 0x04005D24 RID: 23844
		[SerializeField]
		private Transform _segmentParent;

		// Token: 0x04005D25 RID: 23845
		[SerializeField]
		private Transform _trainParent;

		// Token: 0x04005D26 RID: 23846
		[SerializeField]
		private bool _regnerateTrackMeshOnAwake;

		// Token: 0x04005D27 RID: 23847
		private float _trainLength = -1f;

		// Token: 0x04005D28 RID: 23848
		private TrackSegment[] _trackSegments;
	}
}
