using System;
using UnityEngine;

// Token: 0x0200073C RID: 1852
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x06002E4E RID: 11854 RVA: 0x000F54F3 File Offset: 0x000F36F3
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000F5510 File Offset: 0x000F3710
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000F5528 File Offset: 0x000F3728
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x04003A23 RID: 14883
	[Space]
	public GTSignalID signal;

	// Token: 0x04003A24 RID: 14884
	public GTSignal.EmitMode emitMode;
}
