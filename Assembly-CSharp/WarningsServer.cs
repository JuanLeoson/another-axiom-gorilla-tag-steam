using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200095A RID: 2394
internal abstract class WarningsServer : MonoBehaviour
{
	// Token: 0x06003ABB RID: 15035
	public abstract Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token);

	// Token: 0x06003ABC RID: 15036
	public abstract Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token);

	// Token: 0x04004829 RID: 18473
	public static volatile WarningsServer Instance;
}
