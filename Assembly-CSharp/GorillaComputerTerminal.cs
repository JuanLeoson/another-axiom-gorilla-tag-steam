using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007D8 RID: 2008
public class GorillaComputerTerminal : MonoBehaviour, IBuildValidation
{
	// Token: 0x06003246 RID: 12870 RVA: 0x001061DC File Offset: 0x001043DC
	public bool BuildValidationCheck()
	{
		if (this.myScreenText == null || this.myFunctionText == null || this.monitorMesh == null)
		{
			Debug.LogErrorFormat(base.gameObject, "gorilla computer terminal {0} is missing screen text, function text, or monitor mesh. this will break lots of computer stuff", new object[]
			{
				base.gameObject.name
			});
			return false;
		}
		return true;
	}

	// Token: 0x06003247 RID: 12871 RVA: 0x0010623A File Offset: 0x0010443A
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|4_0());
			return;
		}
		this.Init();
	}

	// Token: 0x06003248 RID: 12872 RVA: 0x00106260 File Offset: 0x00104460
	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.materials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x00106318 File Offset: 0x00104518
	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x00106367 File Offset: 0x00104567
	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x00106375 File Offset: 0x00104575
	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x00106383 File Offset: 0x00104583
	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.materials = materials;
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x00106391 File Offset: 0x00104591
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|4_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null);
		yield return null;
		this.Init();
		yield break;
	}

	// Token: 0x04003F03 RID: 16131
	public TextMeshPro myScreenText;

	// Token: 0x04003F04 RID: 16132
	public TextMeshPro myFunctionText;

	// Token: 0x04003F05 RID: 16133
	public MeshRenderer monitorMesh;
}
