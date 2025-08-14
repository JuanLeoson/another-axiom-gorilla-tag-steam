using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class PartyInABox : MonoBehaviour
{
	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003A7E6 File Offset: 0x000389E6
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0003A7E6 File Offset: 0x000389E6
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003A7EE File Offset: 0x000389EE
	public void Cranked_ReleaseParty()
	{
		if (!this.parentHoldable.IsLocalObject())
		{
			return;
		}
		this.ReleaseParty();
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003A804 File Offset: 0x00038A04
	public void ReleaseParty()
	{
		if (this.isReleased)
		{
			return;
		}
		if (this.parentHoldable.IsLocalObject())
		{
			this.parentHoldable.itemState |= TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(true, this.partyHapticStrength, this.partyHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.partyHapticStrength, this.partyHapticDuration);
		}
		this.isReleased = true;
		this.spring.enabled = true;
		this.anim.Play();
		this.particles.Play();
		this.partyAudio.Play();
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0003A8A0 File Offset: 0x00038AA0
	private void Update()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			return;
		}
		if (this.parentHoldable.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.isReleased)
			{
				this.ReleaseParty();
				return;
			}
		}
		else if (this.isReleased)
		{
			this.Reset();
		}
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0003A8F8 File Offset: 0x00038AF8
	public void Reset()
	{
		this.isReleased = false;
		this.parentHoldable.itemState &= (TransferrableObject.ItemStates)(-2);
		this.spring.enabled = false;
		this.anim.Stop();
		foreach (PartyInABox.ForceTransform forceTransform in this.forceTransforms)
		{
			forceTransform.Apply();
		}
	}

	// Token: 0x04000D5A RID: 3418
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04000D5B RID: 3419
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04000D5C RID: 3420
	[SerializeField]
	private Animation anim;

	// Token: 0x04000D5D RID: 3421
	[SerializeField]
	private SpringyWobbler spring;

	// Token: 0x04000D5E RID: 3422
	[SerializeField]
	private AudioSource partyAudio;

	// Token: 0x04000D5F RID: 3423
	[SerializeField]
	private float partyHapticStrength;

	// Token: 0x04000D60 RID: 3424
	[SerializeField]
	private float partyHapticDuration;

	// Token: 0x04000D61 RID: 3425
	private bool isReleased;

	// Token: 0x04000D62 RID: 3426
	[SerializeField]
	private PartyInABox.ForceTransform[] forceTransforms;

	// Token: 0x020001B9 RID: 441
	[Serializable]
	private struct ForceTransform
	{
		// Token: 0x06000AF8 RID: 2808 RVA: 0x0003A95B File Offset: 0x00038B5B
		public void Apply()
		{
			this.transform.localPosition = this.localPosition;
			this.transform.localRotation = this.localRotation;
		}

		// Token: 0x04000D63 RID: 3427
		public Transform transform;

		// Token: 0x04000D64 RID: 3428
		public Vector3 localPosition;

		// Token: 0x04000D65 RID: 3429
		public Quaternion localRotation;
	}
}
