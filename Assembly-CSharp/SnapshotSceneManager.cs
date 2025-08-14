using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class SnapshotSceneManager : MonoBehaviour
{
	// Token: 0x06001583 RID: 5507 RVA: 0x000758CE File Offset: 0x00073ACE
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		base.StartCoroutine(this.UpdateScenePeriodically());
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x00073CAA File Offset: 0x00071EAA
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			SceneManagerHelper.RequestSceneCapture();
		}
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000758E2 File Offset: 0x00073AE2
	private IEnumerator UpdateScenePeriodically()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
			this.UpdateScene();
		}
		yield break;
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x000758F4 File Offset: 0x00073AF4
	private void UpdateScene()
	{
		SnapshotSceneManager.<UpdateScene>d__5 <UpdateScene>d__;
		<UpdateScene>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateScene>d__.<>4__this = this;
		<UpdateScene>d__.<>1__state = -1;
		<UpdateScene>d__.<>t__builder.Start<SnapshotSceneManager.<UpdateScene>d__5>(ref <UpdateScene>d__);
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x0007592C File Offset: 0x00073B2C
	private Task<SnapshotSceneManager.SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6 <LoadSceneSnapshotAsync>d__;
		<LoadSceneSnapshotAsync>d__.<>t__builder = AsyncTaskMethodBuilder<SnapshotSceneManager.SceneSnapshot>.Create();
		<LoadSceneSnapshotAsync>d__.<>1__state = -1;
		<LoadSceneSnapshotAsync>d__.<>t__builder.Start<SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref <LoadSceneSnapshotAsync>d__);
		return <LoadSceneSnapshotAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00075968 File Offset: 0x00073B68
	private string AnchorInfo(OVRAnchor anchor)
	{
		OVRRoomLayout ovrroomLayout;
		if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled)
		{
			return string.Format("{0} - ROOM", anchor.Uuid);
		}
		OVRSemanticLabels ovrsemanticLabels;
		if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels) && ovrsemanticLabels.IsEnabled)
		{
			return string.Format("{0} - {1}", anchor.Uuid, ovrsemanticLabels.Labels);
		}
		return string.Format("{0}", anchor.Uuid);
	}

	// Token: 0x04001D42 RID: 7490
	public float UpdateFrequencySeconds = 5f;

	// Token: 0x04001D43 RID: 7491
	private SnapshotSceneManager.SceneSnapshot _snapshot = new SnapshotSceneManager.SceneSnapshot();

	// Token: 0x0200039B RID: 923
	private class SceneSnapshot
	{
		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600158A RID: 5514 RVA: 0x00075A06 File Offset: 0x00073C06
		public List<OVRAnchor> Anchors { get; } = new List<OVRAnchor>();
	}

	// Token: 0x0200039C RID: 924
	private class SnapshotComparer
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x00075A21 File Offset: 0x00073C21
		public SnapshotSceneManager.SceneSnapshot BaseSnapshot { get; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600158D RID: 5517 RVA: 0x00075A29 File Offset: 0x00073C29
		public SnapshotSceneManager.SceneSnapshot NewSnapshot { get; }

		// Token: 0x0600158E RID: 5518 RVA: 0x00075A31 File Offset: 0x00073C31
		public SnapshotComparer(SnapshotSceneManager.SceneSnapshot baseSnapshot, SnapshotSceneManager.SceneSnapshot newSnapshot)
		{
			this.BaseSnapshot = baseSnapshot;
			this.NewSnapshot = newSnapshot;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x00075A48 File Offset: 0x00073C48
		public Task<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> Compare()
		{
			SnapshotSceneManager.SnapshotComparer.<Compare>d__8 <Compare>d__;
			<Compare>d__.<>t__builder = AsyncTaskMethodBuilder<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>.Create();
			<Compare>d__.<>4__this = this;
			<Compare>d__.<>1__state = -1;
			<Compare>d__.<>t__builder.Start<SnapshotSceneManager.SnapshotComparer.<Compare>d__8>(ref <Compare>d__);
			return <Compare>d__.<>t__builder.Task;
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x00075A8C File Offset: 0x00073C8C
		private Task CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes)
		{
			SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9 <CheckRoomChanges>d__;
			<CheckRoomChanges>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckRoomChanges>d__.<>4__this = this;
			<CheckRoomChanges>d__.changes = changes;
			<CheckRoomChanges>d__.<>1__state = -1;
			<CheckRoomChanges>d__.<>t__builder.Start<SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9>(ref <CheckRoomChanges>d__);
			return <CheckRoomChanges>d__.<>t__builder.Task;
		}

		// Token: 0x0200039D RID: 925
		public enum ChangeType
		{
			// Token: 0x04001D48 RID: 7496
			New,
			// Token: 0x04001D49 RID: 7497
			Missing,
			// Token: 0x04001D4A RID: 7498
			Changed
		}
	}
}
