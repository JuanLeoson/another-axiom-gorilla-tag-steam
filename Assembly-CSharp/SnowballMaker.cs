using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class SnowballMaker : MonoBehaviour
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060005F0 RID: 1520 RVA: 0x0002279F File Offset: 0x0002099F
	// (set) Token: 0x060005F1 RID: 1521 RVA: 0x000227A6 File Offset: 0x000209A6
	public static SnowballMaker leftHandInstance { get; private set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060005F2 RID: 1522 RVA: 0x000227AE File Offset: 0x000209AE
	// (set) Token: 0x060005F3 RID: 1523 RVA: 0x000227B5 File Offset: 0x000209B5
	public static SnowballMaker rightHandInstance { get; private set; }

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060005F4 RID: 1524 RVA: 0x000227BD File Offset: 0x000209BD
	// (set) Token: 0x060005F5 RID: 1525 RVA: 0x000227C5 File Offset: 0x000209C5
	public SnowballThrowable[] snowballs { get; private set; }

	// Token: 0x060005F6 RID: 1526 RVA: 0x000227D0 File Offset: 0x000209D0
	private void Awake()
	{
		if (this.isLeftHand)
		{
			if (SnowballMaker.leftHandInstance == null)
			{
				SnowballMaker.leftHandInstance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		else if (SnowballMaker.rightHandInstance == null)
		{
			SnowballMaker.rightHandInstance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (this.handTransform == null)
		{
			this.handTransform = base.transform;
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00022841 File Offset: 0x00020A41
	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		this.snowballs = newThrowables;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0002284C File Offset: 0x00020A4C
	protected void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		if (this.snowballs == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation())
		{
			return;
		}
		if (!GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig || this.snowballs.Length == 0)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		int num = this.isLeftHand ? instance.leftHandMaterialTouchIndex : instance.rightHandMaterialTouchIndex;
		if (num == 0)
		{
			if (Time.time > this.lastGroundContactTime + this.snowballCreationCooldownTime)
			{
				this.requiresFreshMaterialContact = false;
			}
			return;
		}
		this.lastGroundContactTime = Time.time;
		EquipmentInteractor instance2 = EquipmentInteractor.instance;
		bool flag = (this.isLeftHand ? instance2.leftHandHeldEquipment : instance2.rightHandHeldEquipment) != null;
		bool flag2 = this.isLeftHand ? instance2.isLeftGrabbing : instance2.isRightGrabbing;
		bool flag3 = false;
		if (flag2 && !this.requiresFreshMaterialContact)
		{
			int num2 = -1;
			for (int i = 0; i < this.snowballs.Length; i++)
			{
				if (this.snowballs[i].gameObject.activeSelf)
				{
					num2 = i;
					break;
				}
			}
			SnowballThrowable snowballThrowable = (num2 > -1) ? this.snowballs[num2] : null;
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			bool flag4 = this.isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid);
			if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing || flag4 || flag3))
			{
				if (snowballThrowable.matDataIndexes.Contains(num))
				{
					growingSnowballThrowable.IncreaseSize(1);
					GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.requiresFreshMaterialContact = true;
					return;
				}
			}
			else if (!flag)
			{
				foreach (SnowballThrowable snowballThrowable2 in this.snowballs)
				{
					Transform transform = snowballThrowable2.transform;
					if (snowballThrowable2.matDataIndexes.Contains(num))
					{
						Transform transform2 = this.handTransform;
						snowballThrowable2.SetSnowballActiveLocal(true);
						snowballThrowable2.velocityEstimator = this.velocityEstimator;
						transform.position = transform2.position;
						transform.rotation = transform2.rotation;
						GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						this.requiresFreshMaterialContact = true;
						return;
					}
				}
			}
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00022AEC File Offset: 0x00020CEC
	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = this.handTransform;
				snowballThrowable.SetSnowballActiveLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.position;
				transform.rotation = transform2.rotation;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x04000710 RID: 1808
	public bool isLeftHand;

	// Token: 0x04000712 RID: 1810
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000713 RID: 1811
	private float snowballCreationCooldownTime = 0.1f;

	// Token: 0x04000714 RID: 1812
	private float lastGroundContactTime;

	// Token: 0x04000715 RID: 1813
	private bool requiresFreshMaterialContact;

	// Token: 0x04000716 RID: 1814
	[SerializeField]
	private Transform handTransform;
}
