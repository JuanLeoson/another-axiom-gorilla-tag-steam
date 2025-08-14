using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002C RID: 44
[RequireComponent(typeof(AudioSource))]
public class PlayAudioSourceDelay : MonoBehaviour
{
	// Token: 0x0600009C RID: 156 RVA: 0x00004E4D File Offset: 0x0000304D
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		base.GetComponent<AudioSource>().GTPlay();
		yield break;
	}

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	private float _delay;
}
