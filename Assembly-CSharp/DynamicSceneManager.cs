using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DynamicSceneManagerHelper;
using UnityEngine;

// Token: 0x02000389 RID: 905
public class DynamicSceneManager : MonoBehaviour
{
	// Token: 0x06001540 RID: 5440 RVA: 0x00073C96 File Offset: 0x00071E96
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		base.StartCoroutine(this.UpdateScenePeriodically());
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00073CAA File Offset: 0x00071EAA
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			SceneManagerHelper.RequestSceneCapture();
		}
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00073CBF File Offset: 0x00071EBF
	private IEnumerator UpdateScenePeriodically()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
			this._updateSceneTask = this.UpdateScene();
			yield return new WaitUntil(() => this._updateSceneTask.IsCompleted);
		}
		yield break;
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00073CD0 File Offset: 0x00071ED0
	private Task UpdateScene()
	{
		DynamicSceneManager.<UpdateScene>d__7 <UpdateScene>d__;
		<UpdateScene>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateScene>d__.<>4__this = this;
		<UpdateScene>d__.<>1__state = -1;
		<UpdateScene>d__.<>t__builder.Start<DynamicSceneManager.<UpdateScene>d__7>(ref <UpdateScene>d__);
		return <UpdateScene>d__.<>t__builder.Task;
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00073D14 File Offset: 0x00071F14
	private Task<SceneSnapshot> LoadSceneSnapshotAsync()
	{
		DynamicSceneManager.<LoadSceneSnapshotAsync>d__8 <LoadSceneSnapshotAsync>d__;
		<LoadSceneSnapshotAsync>d__.<>t__builder = AsyncTaskMethodBuilder<SceneSnapshot>.Create();
		<LoadSceneSnapshotAsync>d__.<>1__state = -1;
		<LoadSceneSnapshotAsync>d__.<>t__builder.Start<DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref <LoadSceneSnapshotAsync>d__);
		return <LoadSceneSnapshotAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x00073D50 File Offset: 0x00071F50
	private Task UpdateUnityObjects(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SceneSnapshot newSnapshot)
	{
		DynamicSceneManager.<UpdateUnityObjects>d__9 <UpdateUnityObjects>d__;
		<UpdateUnityObjects>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateUnityObjects>d__.<>4__this = this;
		<UpdateUnityObjects>d__.changes = changes;
		<UpdateUnityObjects>d__.newSnapshot = newSnapshot;
		<UpdateUnityObjects>d__.<>1__state = -1;
		<UpdateUnityObjects>d__.<>t__builder.Start<DynamicSceneManager.<UpdateUnityObjects>d__9>(ref <UpdateUnityObjects>d__);
		return <UpdateUnityObjects>d__.<>t__builder.Task;
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00073DA4 File Offset: 0x00071FA4
	private List<OVRAnchor> FilterChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SnapshotComparer.ChangeType changeType)
	{
		return (from tuple in changes
		where tuple.Item2 == changeType
		select tuple.Item1).ToList<OVRAnchor>();
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x00073DFC File Offset: 0x00071FFC
	private List<ValueTuple<OVRAnchor, OVRAnchor>> FindAnchorPairs(List<OVRAnchor> allAnchors, SceneSnapshot newSnapshot)
	{
		IEnumerable<OVRAnchor> enumerable = allAnchors.Where(new Func<OVRAnchor, bool>(this._snapshot.Contains));
		IEnumerable<OVRAnchor> enumerable2 = allAnchors.Where(new Func<OVRAnchor, bool>(newSnapshot.Contains));
		List<ValueTuple<OVRAnchor, OVRAnchor>> list = new List<ValueTuple<OVRAnchor, OVRAnchor>>();
		foreach (OVRAnchor ovranchor in enumerable)
		{
			foreach (OVRAnchor ovranchor2 in enumerable2)
			{
				if (this.AreAnchorsEqual(this._snapshot.Anchors[ovranchor], newSnapshot.Anchors[ovranchor2]))
				{
					list.Add(new ValueTuple<OVRAnchor, OVRAnchor>(ovranchor, ovranchor2));
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x00073EDC File Offset: 0x000720DC
	private bool AreAnchorsEqual(SceneSnapshot.Data anchor1Data, SceneSnapshot.Data anchor2Data)
	{
		return anchor1Data.Children != null && anchor2Data.Children != null && (anchor1Data.Children.Any(new Func<OVRAnchor, bool>(anchor2Data.Children.Contains)) || anchor2Data.Children.Any(new Func<OVRAnchor, bool>(anchor1Data.Children.Contains)));
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00073F3C File Offset: 0x0007213C
	private OVRAnchor GetParentAnchor(OVRAnchor childAnchor, SceneSnapshot snapshot)
	{
		foreach (KeyValuePair<OVRAnchor, SceneSnapshot.Data> keyValuePair in snapshot.Anchors)
		{
			List<OVRAnchor> children = keyValuePair.Value.Children;
			if (children != null && children.Contains(childAnchor))
			{
				return keyValuePair.Key;
			}
		}
		return OVRAnchor.Null;
	}

	// Token: 0x04001CEB RID: 7403
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001CEC RID: 7404
	private SceneSnapshot _snapshot = new SceneSnapshot();

	// Token: 0x04001CED RID: 7405
	private Dictionary<OVRAnchor, GameObject> _sceneGameObjects = new Dictionary<OVRAnchor, GameObject>();

	// Token: 0x04001CEE RID: 7406
	private Task _updateSceneTask;
}
