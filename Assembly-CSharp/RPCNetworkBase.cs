using System;
using UnityEngine;

// Token: 0x02000A59 RID: 2649
internal abstract class RPCNetworkBase : MonoBehaviour
{
	// Token: 0x0600409F RID: 16543
	public abstract void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler);
}
