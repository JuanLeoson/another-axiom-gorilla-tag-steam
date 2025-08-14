using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x06000650 RID: 1616 RVA: 0x00024764 File Offset: 0x00022964
	protected void OnEnable()
	{
		if (!this.gorillaPlayerXform && !base.transform.TryFindByPath(this.gorillaPlayerXform_path, out this.gorillaPlayerXform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"HorseStickNoiseMaker: DEACTIVATING! Could not find gorillaPlayerXform using path: \"",
				this.gorillaPlayerXform_path,
				"\"\nThis component's transform path: \"",
				base.transform.GetPath(),
				"\""
			}));
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0002480C File Offset: 0x00022A0C
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play();
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x04000794 RID: 1940
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x04000795 RID: 1941
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x04000796 RID: 1942
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000797 RID: 1943
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x04000798 RID: 1944
	[Delayed]
	public string gorillaPlayerXform_path;

	// Token: 0x04000799 RID: 1945
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x0400079A RID: 1946
	private Vector3 oldPos;

	// Token: 0x0400079B RID: 1947
	private float timeSincePlay;

	// Token: 0x0400079C RID: 1948
	private float distElapsed;
}
