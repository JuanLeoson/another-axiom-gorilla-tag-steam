using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000E39 RID: 3641
	public class GorillaVelocityTracker : MonoBehaviour
	{
		// Token: 0x06005A86 RID: 23174 RVA: 0x001C9868 File Offset: 0x001C7A68
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|15_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|15_0(this.worldSpaceData);
			this.isRelativeTo = (this.relativeTo != null);
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
			this.wasAboveThreshold = false;
		}

		// Token: 0x06005A87 RID: 23175 RVA: 0x001C98EE File Offset: 0x001C7AEE
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x001C98EE File Offset: 0x001C7AEE
		private void OnDisable()
		{
			this.ResetState();
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x001C98F6 File Offset: 0x001C7AF6
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x001C9931 File Offset: 0x001C7B31
		private void Update()
		{
			this.Tick();
			if (this.useVelocityEvents)
			{
				this.GetLatestVelocity(false);
			}
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x001C994C File Offset: 0x001C7B4C
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x001C9A39 File Offset: 0x001C7C39
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x001C9A5C File Offset: 0x001C7C5C
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass22_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|22_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|22_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x06005A8E RID: 23182 RVA: 0x001C9B6C File Offset: 0x001C7D6C
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array[this.currentDataPointIndex].delta.magnitude >= this.latestVelocityThreshold && !this.wasAboveThreshold)
			{
				UnityEvent onLatestAboveThreshold = this.OnLatestAboveThreshold;
				if (onLatestAboveThreshold != null)
				{
					onLatestAboveThreshold.Invoke();
				}
				this.wasAboveThreshold = true;
			}
			else if (array[this.currentDataPointIndex].delta.magnitude < this.latestVelocityThreshold && this.wasAboveThreshold)
			{
				UnityEvent onLatestBelowThreshold = this.OnLatestBelowThreshold;
				if (onLatestBelowThreshold != null)
				{
					onLatestBelowThreshold.Invoke();
				}
				this.wasAboveThreshold = false;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x001C9C10 File Offset: 0x001C7E10
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 b = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						b = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - b, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x06005A91 RID: 23185 RVA: 0x001C9CD0 File Offset: 0x001C7ED0
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|15_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x06005A92 RID: 23186 RVA: 0x001C9CF8 File Offset: 0x001C7EF8
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|22_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass22_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x04006565 RID: 25957
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x04006566 RID: 25958
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x04006567 RID: 25959
		[Tooltip("Use in Editor to trigger events when above or higher than a desired latest velocity.")]
		[SerializeField]
		private bool useVelocityEvents;

		// Token: 0x04006568 RID: 25960
		[SerializeField]
		private float latestVelocityThreshold;

		// Token: 0x04006569 RID: 25961
		public UnityEvent OnLatestBelowThreshold;

		// Token: 0x0400656A RID: 25962
		public UnityEvent OnLatestAboveThreshold;

		// Token: 0x0400656B RID: 25963
		private bool wasAboveThreshold;

		// Token: 0x0400656C RID: 25964
		private int currentDataPointIndex;

		// Token: 0x0400656D RID: 25965
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x0400656E RID: 25966
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x0400656F RID: 25967
		private Transform trans;

		// Token: 0x04006570 RID: 25968
		private Vector3 lastWorldSpacePos;

		// Token: 0x04006571 RID: 25969
		private Vector3 lastLocalSpacePos;

		// Token: 0x04006572 RID: 25970
		private bool isRelativeTo;

		// Token: 0x04006573 RID: 25971
		private int lastTickedFrame = -1;

		// Token: 0x02000E3A RID: 3642
		public class VelocityDataPoint
		{
			// Token: 0x04006574 RID: 25972
			public Vector3 delta;

			// Token: 0x04006575 RID: 25973
			public float time = -1f;
		}
	}
}
