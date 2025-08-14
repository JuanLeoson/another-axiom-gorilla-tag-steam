using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B2 RID: 1202
[RequireComponent(typeof(TransferrableObject))]
public class UnityEventOnGrab : MonoBehaviour
{
	// Token: 0x06001DBB RID: 7611 RVA: 0x0009F764 File Offset: 0x0009D964
	private void Awake()
	{
		TransferrableObject componentInParent = base.GetComponentInParent<TransferrableObject>();
		Behaviour[] behavioursEnabledOnlyWhileHeld = componentInParent.behavioursEnabledOnlyWhileHeld;
		Behaviour[] array = new Behaviour[behavioursEnabledOnlyWhileHeld.Length + 1];
		for (int i = 0; i < behavioursEnabledOnlyWhileHeld.Length; i++)
		{
			array[i] = behavioursEnabledOnlyWhileHeld[i];
		}
		array[behavioursEnabledOnlyWhileHeld.Length] = this;
		componentInParent.behavioursEnabledOnlyWhileHeld = array;
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x0009F7AB File Offset: 0x0009D9AB
	private void OnEnable()
	{
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x0009F7BD File Offset: 0x0009D9BD
	private void OnDisable()
	{
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002653 RID: 9811
	[SerializeField]
	private UnityEvent onGrab;

	// Token: 0x04002654 RID: 9812
	[SerializeField]
	private UnityEvent onRelease;
}
