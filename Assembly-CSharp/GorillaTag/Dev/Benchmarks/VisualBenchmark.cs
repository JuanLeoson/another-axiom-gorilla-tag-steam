using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace GorillaTag.Dev.Benchmarks
{
	// Token: 0x02000ECA RID: 3786
	public class VisualBenchmark : MonoBehaviour
	{
		// Token: 0x06005E64 RID: 24164 RVA: 0x001DBC64 File Offset: 0x001D9E64
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				VisualBenchmark.isQuitting = true;
			};
			List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>(5500);
			ProfilerRecorderHandle.GetAvailable(list);
			Debug.Log(string.Format("poop Available stats: {0}", list.Count), this);
			List<VisualBenchmark.StatInfo> list2 = new List<VisualBenchmark.StatInfo>(600);
			foreach (ProfilerRecorderHandle handle in list)
			{
				ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(handle);
				if (description.Category == ProfilerCategory.Render)
				{
					list2.Add(new VisualBenchmark.StatInfo
					{
						name = description.Name,
						unit = description.UnitType
					});
				}
			}
			this.availableRenderStats = list2.ToArray();
			Debug.Log(string.Format("poop availableRenderStats: {0}", list2.Count), this);
			List<Transform> list3 = new List<Transform>(this.benchmarkLocations.Length);
			foreach (Transform transform in this.benchmarkLocations)
			{
				if (transform != null)
				{
					list3.Add(transform);
				}
			}
			this.benchmarkLocations = list3.ToArray();
		}

		// Token: 0x06005E65 RID: 24165 RVA: 0x001DBDC8 File Offset: 0x001D9FC8
		protected void OnEnable()
		{
			this.renderStatsRecorders = new ProfilerRecorder[this.availableRenderStats.Length];
			for (int i = 0; i < this.availableRenderStats.Length; i++)
			{
				this.renderStatsRecorders[i] = ProfilerRecorder.StartNew(ProfilerCategory.Render, this.availableRenderStats[i].name, 1, ProfilerRecorderOptions.Default);
			}
			this.state = VisualBenchmark.EState.Setup;
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x001DBE2C File Offset: 0x001DA02C
		protected void OnDisable()
		{
			foreach (ProfilerRecorder profilerRecorder in this.renderStatsRecorders)
			{
				profilerRecorder.Dispose();
			}
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x001DBE60 File Offset: 0x001DA060
		protected void LateUpdate()
		{
			if (VisualBenchmark.isQuitting)
			{
				return;
			}
			switch (this.state)
			{
			case VisualBenchmark.EState.Setup:
				Debug.Log("poop start");
				this.sb.Clear();
				this.currentLocationIndex = 0;
				this.lastTime = Time.realtimeSinceStartup;
				this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
				return;
			case VisualBenchmark.EState.WaitingBeforeCollectingGarbage:
				Debug.Log("poop wait 1");
				if (Time.realtimeSinceStartup - this.lastTime >= this.collectGarbageDelay)
				{
					this.lastTime = Time.time;
					GC.Collect();
					this.state = VisualBenchmark.EState.WaitingBeforeRecordingStats;
					return;
				}
				break;
			case VisualBenchmark.EState.WaitingBeforeRecordingStats:
				Debug.Log("poop wait 2");
				if (Time.time - this.lastTime >= this.recordStatsDelay)
				{
					this.lastTime = Time.time;
					this.RecordLocationStats(this.benchmarkLocations[this.currentLocationIndex]);
					if (this.currentLocationIndex < this.benchmarkLocations.Length - 1)
					{
						this.currentLocationIndex++;
						this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
						return;
					}
					this.state = VisualBenchmark.EState.TearDown;
					return;
				}
				break;
			case VisualBenchmark.EState.TearDown:
				Debug.Log("poop teardown");
				Debug.Log(this.sb.ToString());
				this.state = VisualBenchmark.EState.Setup;
				if (this.sb.Length > this.sb.Capacity)
				{
					Debug.Log("Capacity exceeded on string builder, increase string builder's capacity. " + string.Format("capacity={0}, length={1}", this.sb.Capacity, this.sb.Length), this);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x001DBFE0 File Offset: 0x001DA1E0
		private void RecordLocationStats(Transform xform)
		{
			this.sb.Append("Location: ");
			this.sb.Append(xform.name);
			this.sb.Append("\n");
			this.sb.Append("pos=");
			this.sb.Append(xform.position.ToString("F3"));
			this.sb.Append(" rot=");
			this.sb.Append(xform.rotation.ToString("F3"));
			this.sb.Append(" scale=");
			this.sb.Append(xform.lossyScale.ToString("F3"));
			this.sb.Append("\n");
			for (int i = 0; i < this.renderStatsRecorders.Length; i++)
			{
				this.sb.Append(this.availableRenderStats[i].name);
				this.sb.Append(": ");
				ProfilerMarkerDataUnit unit = this.availableRenderStats[i].unit;
				if (unit != ProfilerMarkerDataUnit.TimeNanoseconds)
				{
					if (unit == ProfilerMarkerDataUnit.Bytes)
					{
						this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1024.0);
						this.sb.Append("kb");
					}
					else
					{
						this.sb.Append(this.renderStatsRecorders[i].LastValue);
						this.sb.Append(' ');
						this.sb.Append(this.availableRenderStats[i].unit.ToString());
					}
				}
				else
				{
					this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1000000.0);
					this.sb.Append("ms");
				}
				this.sb.Append('\n');
			}
		}

		// Token: 0x04006836 RID: 26678
		[Tooltip("the camera will be moved and rotated to these spots and record stats.")]
		public Transform[] benchmarkLocations;

		// Token: 0x04006837 RID: 26679
		[Tooltip("How long to wait before calling GC.Collect() to clean up memory.")]
		public float collectGarbageDelay = 2f;

		// Token: 0x04006838 RID: 26680
		[Tooltip("How long to wait before recording stats after the camera was moved to a new location.\nThis + collectGarbageDelay is the total time spent at each location.")]
		private float recordStatsDelay = 2f;

		// Token: 0x04006839 RID: 26681
		[Tooltip("The camera to use for profiling. If null, a new camera will be created.")]
		private Camera cam;

		// Token: 0x0400683A RID: 26682
		private VisualBenchmark.StatInfo[] availableRenderStats;

		// Token: 0x0400683B RID: 26683
		private ProfilerRecorder[] renderStatsRecorders;

		// Token: 0x0400683C RID: 26684
		private static bool isQuitting = true;

		// Token: 0x0400683D RID: 26685
		private int currentLocationIndex;

		// Token: 0x0400683E RID: 26686
		private VisualBenchmark.EState state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;

		// Token: 0x0400683F RID: 26687
		private float lastTime;

		// Token: 0x04006840 RID: 26688
		private readonly StringBuilder sb = new StringBuilder(1024);

		// Token: 0x02000ECB RID: 3787
		private struct StatInfo
		{
			// Token: 0x04006841 RID: 26689
			public string name;

			// Token: 0x04006842 RID: 26690
			public ProfilerMarkerDataUnit unit;
		}

		// Token: 0x02000ECC RID: 3788
		private enum EState
		{
			// Token: 0x04006844 RID: 26692
			Setup,
			// Token: 0x04006845 RID: 26693
			WaitingBeforeCollectingGarbage,
			// Token: 0x04006846 RID: 26694
			WaitingBeforeRecordingStats,
			// Token: 0x04006847 RID: 26695
			TearDown
		}
	}
}
