using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000276 RID: 630
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	// Token: 0x04001764 RID: 5988
	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;
}
