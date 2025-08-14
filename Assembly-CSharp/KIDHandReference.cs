using System;
using UnityEngine;

// Token: 0x020008D6 RID: 2262
public class KIDHandReference : MonoBehaviour
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x06003838 RID: 14392 RVA: 0x0012234A File Offset: 0x0012054A
	public static GameObject LeftHand
	{
		get
		{
			return KIDHandReference._leftHandRef;
		}
	}

	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x06003839 RID: 14393 RVA: 0x00122351 File Offset: 0x00120551
	public static GameObject RightHand
	{
		get
		{
			return KIDHandReference._rightHandRef;
		}
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x00122358 File Offset: 0x00120558
	private void Awake()
	{
		KIDHandReference._leftHandRef = this._leftHand;
		KIDHandReference._rightHandRef = this._rightHand;
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x0400450C RID: 17676
	[SerializeField]
	private GameObject _leftHand;

	// Token: 0x0400450D RID: 17677
	[SerializeField]
	private GameObject _rightHand;

	// Token: 0x0400450E RID: 17678
	private static GameObject _leftHandRef;

	// Token: 0x0400450F RID: 17679
	private static GameObject _rightHandRef;
}
