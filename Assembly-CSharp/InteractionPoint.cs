using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003D6 RID: 982
public class InteractionPoint : MonoBehaviour, ISpawnable, IBuildValidation
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x060016F5 RID: 5877 RVA: 0x0007CC2F File Offset: 0x0007AE2F
	// (set) Token: 0x060016F6 RID: 5878 RVA: 0x0007CC37 File Offset: 0x0007AE37
	public bool ignoreLeftHand { get; private set; }

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x060016F7 RID: 5879 RVA: 0x0007CC40 File Offset: 0x0007AE40
	// (set) Token: 0x060016F8 RID: 5880 RVA: 0x0007CC48 File Offset: 0x0007AE48
	public bool ignoreRightHand { get; private set; }

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x060016F9 RID: 5881 RVA: 0x0007CC51 File Offset: 0x0007AE51
	public IHoldableObject Holdable
	{
		get
		{
			return this.parentHoldable;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x060016FA RID: 5882 RVA: 0x0007CC59 File Offset: 0x0007AE59
	// (set) Token: 0x060016FB RID: 5883 RVA: 0x0007CC61 File Offset: 0x0007AE61
	public bool IsSpawned { get; set; }

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x060016FC RID: 5884 RVA: 0x0007CC6A File Offset: 0x0007AE6A
	// (set) Token: 0x060016FD RID: 5885 RVA: 0x0007CC72 File Offset: 0x0007AE72
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060016FE RID: 5886 RVA: 0x0007CC7C File Offset: 0x0007AE7C
	public void OnSpawn(VRRig rig)
	{
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		if (this.parentHoldableObject != null)
		{
			this.parentHoldable = this.parentHoldableObject.GetComponent<IHoldableObject>();
		}
		else
		{
			this.parentHoldable = base.GetComponentInParent<IHoldableObject>(true);
			this.parentHoldableObject = this.parentHoldable.gameObject;
		}
		if (this.parentHoldable == null)
		{
			if (this.parentHoldableObject == null)
			{
				Debug.LogError("InteractionPoint: Disabling because expected field `parentHoldableObject` is null. Path=" + base.transform.GetPathQ());
				base.enabled = false;
				return;
			}
			Debug.LogError("InteractionPoint: Disabling because `parentHoldableObject` does not have a IHoldableObject component. Path=" + base.transform.GetPathQ());
		}
		TransferrableObject transferrableObject = this.parentHoldable as TransferrableObject;
		this.forLocalPlayer = (transferrableObject == null || transferrableObject.IsLocalObject() || transferrableObject.isSceneObject || transferrableObject.canDrop);
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x0007CD67 File Offset: 0x0007AF67
	private void Awake()
	{
		if (this.isNonSpawnedObject)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x0007CD78 File Offset: 0x0007AF78
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x0007CD88 File Offset: 0x0007AF88
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		this.interactor.InteractionPointDisabled(this);
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x0007CDB0 File Offset: 0x0007AFB0
	protected void LateUpdate()
	{
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			this.myCollider.enabled = false;
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.interactionRadius > 0f || this.myCollider != null)
		{
			if (!this.ignoreLeftHand && this.OverlapCheck(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (!this.ignoreRightHand && this.OverlapCheck(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x0007CF40 File Offset: 0x0007B140
	private bool OverlapCheck(Vector3 point)
	{
		if (this.interactionRadius > 0f)
		{
			return (base.transform.position - point).IsShorterThan(this.interactionRadius * base.transform.lossyScale);
		}
		return this.myCollider != null && this.myCollider.bounds.Contains(point);
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x04001ED0 RID: 7888
	[SerializeField]
	[FormerlySerializedAs("parentTransferrableObject")]
	private GameObject parentHoldableObject;

	// Token: 0x04001ED1 RID: 7889
	private IHoldableObject parentHoldable;

	// Token: 0x04001ED4 RID: 7892
	[SerializeField]
	private bool isNonSpawnedObject;

	// Token: 0x04001ED5 RID: 7893
	[SerializeField]
	private float interactionRadius;

	// Token: 0x04001ED6 RID: 7894
	public Collider myCollider;

	// Token: 0x04001ED7 RID: 7895
	public EquipmentInteractor interactor;

	// Token: 0x04001ED8 RID: 7896
	public bool wasInLeft;

	// Token: 0x04001ED9 RID: 7897
	public bool wasInRight;

	// Token: 0x04001EDA RID: 7898
	public bool forLocalPlayer;
}
