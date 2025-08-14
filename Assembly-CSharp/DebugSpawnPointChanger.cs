using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x06001DD9 RID: 7641 RVA: 0x0009FD00 File Offset: 0x0009DF00
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		GTPlayer gtplayer = Object.FindObjectOfType<GTPlayer>();
		if (gtplayer == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				gtplayer.transform.position = transform.position;
				gtplayer.transform.rotation = transform.rotation;
				gtplayer.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					gtplayer.SetScaleMultiplier(component.startSize);
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x0009FDF4 File Offset: 0x0009DFF4
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindObjectOfType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] spawnPts = spawnManager.ChildrenXfs();
			foreach (VRRig rig in (VRRig[])Object.FindObjectsOfType(typeof(VRRig)))
			{
				this.AttachSpawnPoint(rig, spawnPts, index);
			}
		}
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x0009FE49 File Offset: 0x0009E049
	public List<string> GetPlausibleJumpLocation()
	{
		return (from index in this.levelTriggers[this.lastLocationIndex].canJumpToIndex
		select this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x0009FE78 File Offset: 0x0009E078
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x0009FEA8 File Offset: 0x0009E0A8
	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	// Token: 0x0400266C RID: 9836
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x0400266D RID: 9837
	private int lastLocationIndex;

	// Token: 0x020004B8 RID: 1208
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x0400266E RID: 9838
		public string levelName;

		// Token: 0x0400266F RID: 9839
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04002670 RID: 9840
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04002671 RID: 9841
		public int[] canJumpToIndex;
	}
}
