using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000211 RID: 529
public class LocalChestController : MonoBehaviour
{
	// Token: 0x06000C69 RID: 3177 RVA: 0x00042FBC File Offset: 0x000411BC
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x04000F6B RID: 3947
	public PlayableDirector director;

	// Token: 0x04000F6C RID: 3948
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x04000F6D RID: 3949
	private bool isOpen;
}
