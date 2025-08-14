using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class ElfLauncherElf : MonoBehaviour
{
	// Token: 0x06000A27 RID: 2599 RVA: 0x0003797C File Offset: 0x00035B7C
	private void OnEnable()
	{
		base.StartCoroutine(this.ReturnToPoolAfterDelayCo());
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0003798B File Offset: 0x00035B8B
	private IEnumerator ReturnToPoolAfterDelayCo()
	{
		yield return new WaitForSeconds(this.destroyAfterDuration);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0003799A File Offset: 0x00035B9A
	private void OnCollisionEnter(Collision collision)
	{
		if (this.bounceAudioCoolingDownUntilTimestamp > Time.time)
		{
			return;
		}
		this.bounceAudio.Play();
		this.bounceAudioCoolingDownUntilTimestamp = Time.time + this.bounceAudioCooldownDuration;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x000379C7 File Offset: 0x00035BC7
	private void FixedUpdate()
	{
		this.rb.AddForce(base.transform.lossyScale.x * Physics.gravity, ForceMode.Acceleration);
	}

	// Token: 0x04000C4B RID: 3147
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000C4C RID: 3148
	[SerializeField]
	private SoundBankPlayer bounceAudio;

	// Token: 0x04000C4D RID: 3149
	[SerializeField]
	private float bounceAudioCooldownDuration;

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private float destroyAfterDuration;

	// Token: 0x04000C4F RID: 3151
	private float bounceAudioCoolingDownUntilTimestamp;
}
