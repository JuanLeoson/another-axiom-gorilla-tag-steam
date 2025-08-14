using System;
using System.Collections.Generic;

// Token: 0x02000275 RID: 629
[Serializable]
public class ScenePerformanceData
{
	// Token: 0x06000E7F RID: 3711 RVA: 0x000581DC File Offset: 0x000563DC
	public ScenePerformanceData(string mapName, int gorillaCount, int droppedFrames, int msHigh, int medianMS, int medianFPS, int medianDrawCalls, List<int> msCaptures)
	{
		this._mapName = mapName;
		this._gorillaCount = gorillaCount;
		this._droppedFrames = droppedFrames;
		this._msHigh = msHigh;
		this._medianMS = medianMS;
		this._medianFPS = medianFPS;
		this._medianDrawCallCount = medianDrawCalls;
		this._msCaptures = new List<int>(msCaptures);
	}

	// Token: 0x0400175C RID: 5980
	public string _mapName;

	// Token: 0x0400175D RID: 5981
	public int _gorillaCount;

	// Token: 0x0400175E RID: 5982
	public int _droppedFrames;

	// Token: 0x0400175F RID: 5983
	public int _msHigh;

	// Token: 0x04001760 RID: 5984
	public int _medianMS;

	// Token: 0x04001761 RID: 5985
	public int _medianFPS;

	// Token: 0x04001762 RID: 5986
	public int _medianDrawCallCount;

	// Token: 0x04001763 RID: 5987
	public List<int> _msCaptures;
}
