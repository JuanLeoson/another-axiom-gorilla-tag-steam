using System;
using UnityEngine;

// Token: 0x02000884 RID: 2180
public class MonoBehaviourStatic<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x060036AA RID: 13994 RVA: 0x0011DCB1 File Offset: 0x0011BEB1
	public static T Instance
	{
		get
		{
			return MonoBehaviourStatic<T>.gInstance;
		}
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x0011DCB8 File Offset: 0x0011BEB8
	protected void Awake()
	{
		if (MonoBehaviourStatic<T>.gInstance && MonoBehaviourStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
		}
		MonoBehaviourStatic<T>.gInstance = (this as T);
		this.OnAwake();
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnAwake()
	{
	}

	// Token: 0x040043A0 RID: 17312
	protected static T gInstance;
}
