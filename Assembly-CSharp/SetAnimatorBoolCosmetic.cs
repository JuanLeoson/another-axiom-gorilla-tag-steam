using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class SetAnimatorBoolCosmetic : MonoBehaviour
{
	// Token: 0x06000412 RID: 1042 RVA: 0x00017FB8 File Offset: 0x000161B8
	public void SetAnimatorBool(bool value)
	{
		this.animator.SetBool(this.boolParameterName, value);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00017FCC File Offset: 0x000161CC
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x0400048A RID: 1162
	[SerializeField]
	private Animator animator;

	// Token: 0x0400048B RID: 1163
	[SerializeField]
	private string boolParameterName;
}
