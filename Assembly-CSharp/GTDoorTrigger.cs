using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x02000200 RID: 512
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000C23 RID: 3107 RVA: 0x00041B34 File Offset: 0x0003FD34
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00041B41 File Offset: 0x0003FD41
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00041B50 File Offset: 0x0003FD50
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x00041BC0 File Offset: 0x0003FDC0
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		this.TriggeredEvent.Invoke();
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00041C44 File Offset: 0x0003FE44
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x04000EFE RID: 3838
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x04000EFF RID: 3839
	private int lastTriggeredFrame = -1;

	// Token: 0x04000F00 RID: 3840
	private List<Collider> overlappingColliders = new List<Collider>(20);

	// Token: 0x04000F01 RID: 3841
	internal UnityEvent TriggeredEvent = new UnityEvent();
}
