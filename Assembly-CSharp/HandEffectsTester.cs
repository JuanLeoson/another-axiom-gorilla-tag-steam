using System;
using GorillaExtensions;
using TagEffects;
using UnityEngine;

// Token: 0x02000282 RID: 642
[RequireComponent(typeof(Collider))]
public class HandEffectsTester : MonoBehaviour, IHandEffectsTrigger
{
	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000E9C RID: 3740 RVA: 0x00058605 File Offset: 0x00056805
	public bool Static
	{
		get
		{
			return this.isStatic;
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000E9D RID: 3741 RVA: 0x0005860D File Offset: 0x0005680D
	Transform IHandEffectsTrigger.Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000E9E RID: 3742 RVA: 0x00058615 File Offset: 0x00056815
	VRRig IHandEffectsTrigger.Rig
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000E9F RID: 3743 RVA: 0x00058618 File Offset: 0x00056818
	IHandEffectsTrigger.Mode IHandEffectsTrigger.EffectMode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x00058620 File Offset: 0x00056820
	bool IHandEffectsTrigger.FingersDown
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.FistBump || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000EA1 RID: 3745 RVA: 0x00058637 File Offset: 0x00056837
	bool IHandEffectsTrigger.FingersUp
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.HighFive || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x0005864D File Offset: 0x0005684D
	public bool RightHand { get; }

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00058655 File Offset: 0x00056855
	private void Awake()
	{
		this.triggerZone = base.GetComponent<Collider>();
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x00058663 File Offset: 0x00056863
	private void OnEnable()
	{
		if (!HandEffectsTriggerRegistry.HasInstance)
		{
			HandEffectsTriggerRegistry.FindInstance();
		}
		HandEffectsTriggerRegistry.Instance.Register(this);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0005867C File Offset: 0x0005687C
	private void OnDisable()
	{
		HandEffectsTriggerRegistry.Instance.Unregister(this);
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x00058689 File Offset: 0x00056889
	Vector3 IHandEffectsTrigger.Velocity
	{
		get
		{
			if (this.mode == IHandEffectsTrigger.Mode.HighFive)
			{
				return Vector3.zero;
			}
			IHandEffectsTrigger.Mode mode = this.mode;
			return Vector3.zero;
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x000586A7 File Offset: 0x000568A7
	TagEffectPack IHandEffectsTrigger.CosmeticEffectPack
	{
		get
		{
			return this.cosmeticEffectPack;
		}
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnTriggerEntered(IHandEffectsTrigger other)
	{
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000586B0 File Offset: 0x000568B0
	public bool InTriggerZone(IHandEffectsTrigger t)
	{
		if (!(base.transform.position - t.Transform.position).IsShorterThan(this.triggerZone.bounds.size))
		{
			return false;
		}
		RaycastHit raycastHit;
		switch (this.mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			return t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.FistBump:
			return t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.HighFive_And_FistBump:
			return (t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius)) || (t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius));
		}
		return this.triggerZone.Raycast(new Ray(t.Transform.position, this.triggerZone.bounds.center - t.Transform.position), out raycastHit, this.triggerRadius);
	}

	// Token: 0x0400178C RID: 6028
	[SerializeField]
	private TagEffectPack cosmeticEffectPack;

	// Token: 0x0400178D RID: 6029
	private Collider triggerZone;

	// Token: 0x0400178E RID: 6030
	public IHandEffectsTrigger.Mode mode;

	// Token: 0x0400178F RID: 6031
	[SerializeField]
	private float triggerRadius = 0.07f;

	// Token: 0x04001790 RID: 6032
	[SerializeField]
	private bool isStatic = true;
}
