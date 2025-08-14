using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200084D RID: 2125
public class VirtualStumpBarrierSFX : MonoBehaviour
{
	// Token: 0x06003560 RID: 13664 RVA: 0x00117DFC File Offset: 0x00115FFC
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.PlaySFX();
			return;
		}
		VRRig vrrig;
		if (other.gameObject.TryGetComponent<VRRig>(out vrrig) && !vrrig.isLocal)
		{
			bool value = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			this.trackedGameObjects.Add(other.gameObject, value);
			this.OnTriggerStay(other);
		}
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x00117E8C File Offset: 0x0011608C
	public void OnTriggerStay(Collider other)
	{
		bool flag;
		if (!this.trackedGameObjects.TryGetValue(other.gameObject, out flag))
		{
			return;
		}
		bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
		if (flag != flag2)
		{
			this.PlaySFX();
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x00117EF8 File Offset: 0x001160F8
	public void OnTriggerExit(Collider other)
	{
		bool flag;
		if (this.trackedGameObjects.TryGetValue(other.gameObject, out flag))
		{
			bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			if (flag != flag2)
			{
				this.PlaySFX();
			}
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x00117F64 File Offset: 0x00116164
	public void PlaySFX()
	{
		if (this.barrierAudioSource.IsNull())
		{
			return;
		}
		if (this.PassThroughBarrierSoundClips.IsNullOrEmpty<AudioClip>())
		{
			return;
		}
		this.barrierAudioSource.clip = this.PassThroughBarrierSoundClips[Random.Range(0, this.PassThroughBarrierSoundClips.Count)];
		this.barrierAudioSource.Play();
	}

	// Token: 0x0400425D RID: 16989
	[SerializeField]
	private AudioSource barrierAudioSource;

	// Token: 0x0400425E RID: 16990
	[FormerlySerializedAs("teleportingPlayerSoundClips")]
	[SerializeField]
	private List<AudioClip> PassThroughBarrierSoundClips = new List<AudioClip>();

	// Token: 0x0400425F RID: 16991
	private Dictionary<GameObject, bool> trackedGameObjects = new Dictionary<GameObject, bool>();
}
