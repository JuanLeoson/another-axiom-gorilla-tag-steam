using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x06000630 RID: 1584 RVA: 0x00023D23 File Offset: 0x00021F23
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00023D56 File Offset: 0x00021F56
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00023D64 File Offset: 0x00021F64
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00023D72 File Offset: 0x00021F72
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00023DA4 File Offset: 0x00021FA4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00023E28 File Offset: 0x00022028
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 end = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, end, out raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023F18 File Offset: 0x00022118
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x04000761 RID: 1889
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04000762 RID: 1890
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x04000763 RID: 1891
	public float tipCollisionRadius = 0.05f;

	// Token: 0x04000764 RID: 1892
	public LayerMask tipCollisionLayerMask;

	// Token: 0x04000765 RID: 1893
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000766 RID: 1894
	public float cooldown = 5f;

	// Token: 0x04000767 RID: 1895
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x04000768 RID: 1896
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x04000769 RID: 1897
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x0400076A RID: 1898
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x0400076B RID: 1899
	private Vector3 tipTargetLocalPosition;

	// Token: 0x0400076C RID: 1900
	private bool hasEffectsGameObject;

	// Token: 0x0400076D RID: 1901
	private bool effectsHaveBeenPlayed;
}
