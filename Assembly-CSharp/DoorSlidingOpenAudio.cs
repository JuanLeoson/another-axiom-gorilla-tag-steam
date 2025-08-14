using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class DoorSlidingOpenAudio : MonoBehaviour, IBuildValidation, ITickSystemTick
{
	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600050B RID: 1291 RVA: 0x0001D436 File Offset: 0x0001B636
	// (set) Token: 0x0600050C RID: 1292 RVA: 0x0001D43E File Offset: 0x0001B63E
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600050D RID: 1293 RVA: 0x0001D447 File Offset: 0x0001B647
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001D44F File Offset: 0x0001B64F
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001D458 File Offset: 0x0001B658
	public bool BuildValidationCheck()
	{
		if (this.button == null)
		{
			Debug.LogError("reference button missing for doorslidingopenaudio", base.gameObject);
			return false;
		}
		if (this.audioSource == null)
		{
			Debug.LogError("missing audio source on doorslidingopenaudio", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001D4A8 File Offset: 0x0001B6A8
	void ITickSystemTick.Tick()
	{
		if (this.button.ghostLab.IsDoorMoving(this.button.forSingleDoor, this.button.buttonIndex))
		{
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				return;
			}
		}
		else if (this.audioSource.isPlaying)
		{
			this.audioSource.time = 0f;
			this.audioSource.GTStop();
		}
	}

	// Token: 0x0400060C RID: 1548
	public GhostLabButton button;

	// Token: 0x0400060D RID: 1549
	public AudioSource audioSource;
}
