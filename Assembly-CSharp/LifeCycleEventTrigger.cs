using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200087D RID: 2173
public class LifeCycleEventTrigger : MonoBehaviour
{
	// Token: 0x0600367C RID: 13948 RVA: 0x0011D08F File Offset: 0x0011B28F
	private void Awake()
	{
		UnityEvent onAwake = this._onAwake;
		if (onAwake == null)
		{
			return;
		}
		onAwake.Invoke();
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x0011D0A1 File Offset: 0x0011B2A1
	private void Start()
	{
		UnityEvent onStart = this._onStart;
		if (onStart == null)
		{
			return;
		}
		onStart.Invoke();
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x0011D0B3 File Offset: 0x0011B2B3
	private void OnEnable()
	{
		UnityEvent onEnable = this._onEnable;
		if (onEnable == null)
		{
			return;
		}
		onEnable.Invoke();
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x0011D0C5 File Offset: 0x0011B2C5
	private void OnDisable()
	{
		UnityEvent onDisable = this._onDisable;
		if (onDisable == null)
		{
			return;
		}
		onDisable.Invoke();
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x0011D0D7 File Offset: 0x0011B2D7
	private void OnDestroy()
	{
		UnityEvent onDestroy = this._onDestroy;
		if (onDestroy == null)
		{
			return;
		}
		onDestroy.Invoke();
	}

	// Token: 0x04004347 RID: 17223
	[SerializeField]
	private UnityEvent _onAwake;

	// Token: 0x04004348 RID: 17224
	[SerializeField]
	private UnityEvent _onStart;

	// Token: 0x04004349 RID: 17225
	[SerializeField]
	private UnityEvent _onEnable;

	// Token: 0x0400434A RID: 17226
	[SerializeField]
	private UnityEvent _onDisable;

	// Token: 0x0400434B RID: 17227
	[SerializeField]
	private UnityEvent _onDestroy;
}
