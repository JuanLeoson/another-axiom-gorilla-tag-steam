using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x060039FF RID: 14847 RVA: 0x0012BF64 File Offset: 0x0012A164
	public Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__2 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.description = description;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__2>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06003A00 RID: 14848 RVA: 0x0012BFB0 File Offset: 0x0012A1B0
	private Task WaitForCompletion()
	{
		KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__3 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__3>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	// Token: 0x06003A01 RID: 14849 RVA: 0x0012BFF3 File Offset: 0x0012A1F3
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x000A1481 File Offset: 0x0009F681
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x04004724 RID: 18212
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x04004725 RID: 18213
	private bool _hasCompleted;
}
