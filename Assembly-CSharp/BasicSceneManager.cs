using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class BasicSceneManager : MonoBehaviour
{
	// Token: 0x06001531 RID: 5425 RVA: 0x000734CF File Offset: 0x000716CF
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000734DC File Offset: 0x000716DC
	private void LoadSceneAsync()
	{
		BasicSceneManager.<LoadSceneAsync>d__1 <LoadSceneAsync>d__;
		<LoadSceneAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadSceneAsync>d__.<>4__this = this;
		<LoadSceneAsync>d__.<>1__state = -1;
		<LoadSceneAsync>d__.<>t__builder.Start<BasicSceneManager.<LoadSceneAsync>d__1>(ref <LoadSceneAsync>d__);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00073514 File Offset: 0x00071714
	private Task CreateSceneAnchors(GameObject roomGameObject, List<OVRAnchor> anchors)
	{
		BasicSceneManager.<CreateSceneAnchors>d__2 <CreateSceneAnchors>d__;
		<CreateSceneAnchors>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CreateSceneAnchors>d__.roomGameObject = roomGameObject;
		<CreateSceneAnchors>d__.anchors = anchors;
		<CreateSceneAnchors>d__.<>1__state = -1;
		<CreateSceneAnchors>d__.<>t__builder.Start<BasicSceneManager.<CreateSceneAnchors>d__2>(ref <CreateSceneAnchors>d__);
		return <CreateSceneAnchors>d__.<>t__builder.Task;
	}
}
