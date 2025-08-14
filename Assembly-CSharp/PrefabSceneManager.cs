using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000390 RID: 912
public class PrefabSceneManager : MonoBehaviour
{
	// Token: 0x0600155D RID: 5469 RVA: 0x000748D2 File Offset: 0x00072AD2
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
		base.StartCoroutine(this.UpdateAnchorsPeriodically());
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000748EC File Offset: 0x00072AEC
	private void LoadSceneAsync()
	{
		PrefabSceneManager.<LoadSceneAsync>d__7 <LoadSceneAsync>d__;
		<LoadSceneAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadSceneAsync>d__.<>4__this = this;
		<LoadSceneAsync>d__.<>1__state = -1;
		<LoadSceneAsync>d__.<>t__builder.Start<PrefabSceneManager.<LoadSceneAsync>d__7>(ref <LoadSceneAsync>d__);
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x00074924 File Offset: 0x00072B24
	private Task CreateSceneAnchors(GameObject roomGameObject, OVRRoomLayout roomLayout, List<OVRAnchor> anchors)
	{
		PrefabSceneManager.<CreateSceneAnchors>d__8 <CreateSceneAnchors>d__;
		<CreateSceneAnchors>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CreateSceneAnchors>d__.<>4__this = this;
		<CreateSceneAnchors>d__.roomGameObject = roomGameObject;
		<CreateSceneAnchors>d__.roomLayout = roomLayout;
		<CreateSceneAnchors>d__.anchors = anchors;
		<CreateSceneAnchors>d__.<>1__state = -1;
		<CreateSceneAnchors>d__.<>t__builder.Start<PrefabSceneManager.<CreateSceneAnchors>d__8>(ref <CreateSceneAnchors>d__);
		return <CreateSceneAnchors>d__.<>t__builder.Task;
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x0007497F File Offset: 0x00072B7F
	private IEnumerator UpdateAnchorsPeriodically()
	{
		for (;;)
		{
			foreach (ValueTuple<GameObject, OVRLocatable> valueTuple in this._locatableObjects)
			{
				GameObject item = valueTuple.Item1;
				OVRLocatable item2 = valueTuple.Item2;
				new SceneManagerHelper(item).SetLocation(item2, null);
			}
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
		}
		yield break;
	}

	// Token: 0x04001D10 RID: 7440
	public GameObject WallPrefab;

	// Token: 0x04001D11 RID: 7441
	public GameObject CeilingPrefab;

	// Token: 0x04001D12 RID: 7442
	public GameObject FloorPrefab;

	// Token: 0x04001D13 RID: 7443
	public GameObject FallbackPrefab;

	// Token: 0x04001D14 RID: 7444
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001D15 RID: 7445
	private List<ValueTuple<GameObject, OVRLocatable>> _locatableObjects = new List<ValueTuple<GameObject, OVRLocatable>>();
}
