using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200049B RID: 1179
[RequireComponent(typeof(OnTriggerEventsCosmetic))]
public class OnTriggerEventsHandlerCosmetic : MonoBehaviour
{
	// Token: 0x06001D2E RID: 7470 RVA: 0x0009C956 File Offset: 0x0009AB56
	public void OnTriggerEntered()
	{
		if (this.toggleOnceOnly && this.triggerEntered)
		{
			return;
		}
		this.triggerEntered = true;
		UnityEvent<OnTriggerEventsHandlerCosmetic> unityEvent = this.onTriggerEntered;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.ToggleEffects();
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x0009C988 File Offset: 0x0009AB88
	public void ToggleEffects()
	{
		if (this.particleToPlay)
		{
			this.particleToPlay.Play();
		}
		if (this.soundBankPlayer)
		{
			this.soundBankPlayer.Play();
		}
		if (this.destroyOnTriggerEnter)
		{
			if (this.destroyDelay > 0f)
			{
				base.Invoke("Destroy", this.destroyDelay);
				return;
			}
			this.Destroy();
		}
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x0009C9F2 File Offset: 0x0009ABF2
	private void Destroy()
	{
		this.triggerEntered = false;
		if (ObjectPools.instance.DoesPoolExist(base.gameObject))
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002598 RID: 9624
	[SerializeField]
	private ParticleSystem particleToPlay;

	// Token: 0x04002599 RID: 9625
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x0400259A RID: 9626
	[SerializeField]
	private bool destroyOnTriggerEnter;

	// Token: 0x0400259B RID: 9627
	[SerializeField]
	private float destroyDelay = 1f;

	// Token: 0x0400259C RID: 9628
	[SerializeField]
	private bool toggleOnceOnly;

	// Token: 0x0400259D RID: 9629
	[HideInInspector]
	public UnityEvent<OnTriggerEventsHandlerCosmetic> onTriggerEntered;

	// Token: 0x0400259E RID: 9630
	private bool triggerEntered;
}
