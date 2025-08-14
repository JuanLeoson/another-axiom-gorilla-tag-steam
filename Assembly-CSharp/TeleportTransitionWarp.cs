using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000346 RID: 838
public class TeleportTransitionWarp : TeleportTransition
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x0006AC3F File Offset: 0x00068E3F
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.DoWarp());
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0006AC4E File Offset: 0x00068E4E
	private IEnumerator DoWarp()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		Vector3 startPosition = base.LocomotionTeleport.GetCharacterPosition();
		float elapsedTime = 0f;
		while (elapsedTime < this.TransitionDuration)
		{
			elapsedTime += Time.deltaTime;
			float time = elapsedTime / this.TransitionDuration;
			float positionPercent = this.PositionLerp.Evaluate(time);
			base.LocomotionTeleport.DoWarp(startPosition, positionPercent);
			yield return null;
		}
		base.LocomotionTeleport.DoWarp(startPosition, 1f);
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	// Token: 0x04001B61 RID: 7009
	[Tooltip("How much time the warp transition takes to complete.")]
	[Range(0.01f, 1f)]
	public float TransitionDuration = 0.5f;

	// Token: 0x04001B62 RID: 7010
	[HideInInspector]
	public AnimationCurve PositionLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
