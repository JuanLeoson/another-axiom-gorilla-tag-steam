using System;
using UnityEngine;

// Token: 0x02000474 RID: 1140
public class RandomAudioStart : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001C4F RID: 7247 RVA: 0x00097F05 File Offset: 0x00096105
	public bool BuildValidationCheck()
	{
		if (this.audioSource == null)
		{
			Debug.LogError("audio source is missing for RandomAudioStart, it won't work correctly", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x00097F28 File Offset: 0x00096128
	private void OnEnable()
	{
		this.audioSource.time = Random.value * this.audioSource.clip.length;
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x00097F4B File Offset: 0x0009614B
	[ContextMenu("Assign Audio Source")]
	public void AssignAudioSource()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x040024C8 RID: 9416
	public AudioSource audioSource;
}
