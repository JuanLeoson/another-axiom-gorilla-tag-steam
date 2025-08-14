using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class RaceCheckpointManager : MonoBehaviour
{
	// Token: 0x06000CBD RID: 3261 RVA: 0x00044670 File Offset: 0x00042870
	private void Start()
	{
		this.visual = base.GetComponent<RaceVisual>();
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].Init(this, i);
		}
		this.OnRaceEnd();
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x000446B4 File Offset: 0x000428B4
	public void OnRaceStart()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(i == 0);
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x000446E8 File Offset: 0x000428E8
	public void OnRaceEnd()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(false);
		}
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00044716 File Offset: 0x00042916
	public void OnCheckpointReached(int index, SoundBankPlayer checkpointSound)
	{
		this.checkpoints[index].SetIsCorrectCheckpoint(false);
		this.checkpoints[(index + 1) % this.checkpoints.Length].SetIsCorrectCheckpoint(true);
		this.visual.OnCheckpointPassed(index, checkpointSound);
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0004474C File Offset: 0x0004294C
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpointIdx)
	{
		return checkpointIdx >= 0 && checkpointIdx < this.checkpoints.Length && player.IsPositionInRange(this.checkpoints[checkpointIdx].transform.position, 6f);
	}

	// Token: 0x04000FB1 RID: 4017
	[SerializeField]
	private RaceCheckpoint[] checkpoints;

	// Token: 0x04000FB2 RID: 4018
	private RaceVisual visual;
}
