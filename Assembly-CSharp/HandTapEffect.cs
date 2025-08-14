using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A9 RID: 425
public class HandTapEffect : MonoBehaviour
{
	// Token: 0x06000A93 RID: 2707 RVA: 0x0003902C File Offset: 0x0003722C
	private void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this.leftHandEffect.handContext = componentInParent.LeftHandEffect;
		this.rightHandEffect.handContext = componentInParent.RightHandEffect;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00039062 File Offset: 0x00037262
	private void OnEnable()
	{
		this.leftHandEffect.OnEnable();
		this.rightHandEffect.OnEnable();
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0003907A File Offset: 0x0003727A
	private void OnDisable()
	{
		this.leftHandEffect.OnDisable();
		this.rightHandEffect.OnDisable();
	}

	// Token: 0x04000CED RID: 3309
	public HandTapEffect.HandTapEffectLeftRight leftHandEffect;

	// Token: 0x04000CEE RID: 3310
	public HandTapEffect.HandTapEffectLeftRight rightHandEffect;

	// Token: 0x020001AA RID: 426
	[Serializable]
	public class HandTapEffectDownUp
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000A97 RID: 2711 RVA: 0x00039092 File Offset: 0x00037292
		public bool HasOverrides
		{
			get
			{
				return this.overrides.overrideSurfacePrefab || this.overrides.overrideGamemodePrefab || this.overrides.overrideSound;
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x000390BC File Offset: 0x000372BC
		internal void OnTap(HandEffectContext handContext)
		{
			UnityEvent unityEvent = this.onTapUnityEvents;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			for (int i = 0; i < this.onTapBehaviours.Length; i++)
			{
				this.onTapBehaviours[i].OnTap(handContext);
			}
		}

		// Token: 0x04000CEF RID: 3311
		public HandTapBehaviour[] onTapBehaviours;

		// Token: 0x04000CF0 RID: 3312
		public UnityEvent onTapUnityEvents;

		// Token: 0x04000CF1 RID: 3313
		public HandTapOverrides overrides;
	}

	// Token: 0x020001AB RID: 427
	[Serializable]
	public class HandTapEffectLeftRight
	{
		// Token: 0x06000A9A RID: 2714 RVA: 0x000390FC File Offset: 0x000372FC
		public void OnEnable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = true;
			}
			if (this.downTapEffect.HasOverrides)
			{
				this.handContext.DownTapOverrides = this.downTapEffect.overrides;
			}
			if (this.upTapEffect.HasOverrides)
			{
				this.handContext.UpTapOverrides = this.upTapEffect.overrides;
			}
			this.handContext.handTapDown += this.downTapEffect.OnTap;
			this.handContext.handTapUp += this.upTapEffect.OnTap;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0003919C File Offset: 0x0003739C
		public void OnDisable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = false;
			}
			if (this.downTapEffect.HasOverrides && this.handContext.DownTapOverrides == this.downTapEffect.overrides)
			{
				this.handContext.DownTapOverrides = null;
			}
			if (this.upTapEffect.HasOverrides && this.handContext.UpTapOverrides == this.upTapEffect.overrides)
			{
				this.handContext.UpTapOverrides = null;
			}
			this.handContext.handTapDown -= this.downTapEffect.OnTap;
			this.handContext.handTapUp -= this.upTapEffect.OnTap;
		}

		// Token: 0x04000CF2 RID: 3314
		public bool separateUpTapCooldown;

		// Token: 0x04000CF3 RID: 3315
		public HandTapEffect.HandTapEffectDownUp downTapEffect;

		// Token: 0x04000CF4 RID: 3316
		public HandTapEffect.HandTapEffectDownUp upTapEffect;

		// Token: 0x04000CF5 RID: 3317
		internal HandEffectContext handContext;
	}
}
