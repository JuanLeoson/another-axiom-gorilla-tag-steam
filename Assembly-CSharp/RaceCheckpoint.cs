using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class RaceCheckpoint : MonoBehaviour
{
	// Token: 0x06000CB9 RID: 3257 RVA: 0x000445EF File Offset: 0x000427EF
	public void Init(RaceCheckpointManager manager, int index)
	{
		this.manager = manager;
		this.checkpointIndex = index;
		this.SetIsCorrectCheckpoint(index == 0);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00044609 File Offset: 0x00042809
	public void SetIsCorrectCheckpoint(bool isCorrect)
	{
		this.isCorrect = isCorrect;
		this.banner.sharedMaterial = (isCorrect ? this.activeCheckpointMat : this.wrongCheckpointMat);
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x0004462E File Offset: 0x0004282E
	private void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		if (this.isCorrect)
		{
			this.manager.OnCheckpointReached(this.checkpointIndex, this.checkpointSound);
			return;
		}
		this.wrongCheckpointSound.Play();
	}

	// Token: 0x04000FA9 RID: 4009
	[SerializeField]
	private MeshRenderer banner;

	// Token: 0x04000FAA RID: 4010
	[SerializeField]
	private Material activeCheckpointMat;

	// Token: 0x04000FAB RID: 4011
	[SerializeField]
	private Material wrongCheckpointMat;

	// Token: 0x04000FAC RID: 4012
	[SerializeField]
	private SoundBankPlayer checkpointSound;

	// Token: 0x04000FAD RID: 4013
	[SerializeField]
	private SoundBankPlayer wrongCheckpointSound;

	// Token: 0x04000FAE RID: 4014
	private RaceCheckpointManager manager;

	// Token: 0x04000FAF RID: 4015
	private int checkpointIndex;

	// Token: 0x04000FB0 RID: 4016
	private bool isCorrect;
}
