using System;
using UnityEngine;

// Token: 0x020008D5 RID: 2261
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x06003831 RID: 14385 RVA: 0x0012231E File Offset: 0x0012051E
	// (set) Token: 0x06003832 RID: 14386 RVA: 0x00122326 File Offset: 0x00120526
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x06003833 RID: 14387 RVA: 0x0012232F File Offset: 0x0012052F
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x06003834 RID: 14388 RVA: 0x00122338 File Offset: 0x00120538
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x00122341 File Offset: 0x00120541
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x06003836 RID: 14390 RVA: 0x0012232F File Offset: 0x0012052F
	public void Reset()
	{
		this.Result = KidAgeConfirmationResult.None;
	}
}
