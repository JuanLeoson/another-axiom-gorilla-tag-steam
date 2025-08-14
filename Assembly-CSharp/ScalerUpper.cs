using System;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class ScalerUpper : MonoBehaviour
{
	// Token: 0x060017FD RID: 6141 RVA: 0x0008064C File Offset: 0x0007E84C
	private void Update()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one * this.scaleCurve.Evaluate(this.t);
		}
		this.t += Time.deltaTime;
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x000806AB File Offset: 0x0007E8AB
	private void OnEnable()
	{
		this.t = 0f;
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x000806B8 File Offset: 0x0007E8B8
	private void OnDisable()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one;
		}
	}

	// Token: 0x04001FCD RID: 8141
	[SerializeField]
	private Transform[] target;

	// Token: 0x04001FCE RID: 8142
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x04001FCF RID: 8143
	private float t;
}
