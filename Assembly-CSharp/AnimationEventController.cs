using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class AnimationEventController : MonoBehaviour
{
	// Token: 0x060004E5 RID: 1253 RVA: 0x0001C91A File Offset: 0x0001AB1A
	public void TriggerAttackVFX()
	{
		this.fxAttack.SetActive(false);
		this.fxAttack.SetActive(true);
	}

	// Token: 0x040005D1 RID: 1489
	public GameObject fxAttack;
}
