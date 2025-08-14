using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class TeleportTransitionBlink : TeleportTransition
{
	// Token: 0x060013EA RID: 5098 RVA: 0x0006AAAC File Offset: 0x00068CAC
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.BlinkCoroutine());
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x0006AABB File Offset: 0x00068CBB
	protected IEnumerator BlinkCoroutine()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		float elapsedTime = 0f;
		float teleportTime = this.TransitionDuration * this.TeleportDelay;
		bool teleported = false;
		while (elapsedTime < this.TransitionDuration)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			if (!teleported && elapsedTime >= teleportTime)
			{
				teleported = true;
				base.LocomotionTeleport.DoTeleport();
			}
		}
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	// Token: 0x04001B58 RID: 7000
	[Tooltip("How long the transition takes. Usually this is greater than Teleport Delay.")]
	[Range(0.01f, 2f)]
	public float TransitionDuration = 0.5f;

	// Token: 0x04001B59 RID: 7001
	[Tooltip("At what percentage of the elapsed transition time does the teleport occur?")]
	[Range(0f, 1f)]
	public float TeleportDelay = 0.5f;

	// Token: 0x04001B5A RID: 7002
	[Tooltip("Fade to black over the duration of the transition")]
	public AnimationCurve FadeLevels = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});
}
