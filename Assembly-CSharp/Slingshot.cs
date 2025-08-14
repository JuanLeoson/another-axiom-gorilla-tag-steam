using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003DD RID: 989
public class Slingshot : ProjectileWeapon
{
	// Token: 0x06001725 RID: 5925 RVA: 0x0007D5E8 File Offset: 0x0007B7E8
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x0007D64C File Offset: 0x0007B84C
	protected override void Awake()
	{
		base.Awake();
		this._elasticIntialWidthMultiplier = this.elasticLeft.widthMultiplier;
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x0007D665 File Offset: 0x0007B865
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.myRig = rig;
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0007D678 File Offset: 0x0007B878
	internal override void OnEnable()
	{
		this.leftHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapLeft).transform;
		this.rightHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapRight).transform;
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.elasticLeft.positionCount = 2;
		this.elasticRight.positionCount = 2;
		this.dummyProjectile = null;
		base.OnEnable();
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x0007D6F1 File Offset: 0x0007B8F1
	internal override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x0007D700 File Offset: 0x0007B900
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool blueTeam;
				bool orangeTeam;
				base.GetIsOnTeams(out blueTeam, out orangeTeam);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(blueTeam, orangeTeam, false, default(Color));
			}
			float d = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * d;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float d2 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * d2;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
		}
		else
		{
			this.DestroyDummyProjectile();
			vector = this.centerOrigin.position;
		}
		this.center.position = vector;
		this.elasticLeftPoints[0] = this.leftArm.position;
		this.elasticLeftPoints[1] = (this.elasticRightPoints[1] = vector);
		this.elasticRightPoints[0] = this.rightArm.position;
		this.elasticLeft.SetPositions(this.elasticLeftPoints);
		this.elasticRight.SetPositions(this.elasticRightPoints);
		this.elasticLeft.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		this.elasticRight.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		if (!NetworkSystem.Instance.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x0007D946 File Offset: 0x0007BB46
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
				return;
			}
			this.drawingHand = EquipmentInteractor.instance.leftHand;
		}
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x0007D983 File Offset: 0x0007BB83
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x0007D9BE File Offset: 0x0007BBBE
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x0007D9F8 File Offset: 0x0007BBF8
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0007DAD4 File Offset: 0x0007BCD4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch)
			{
				base.LaunchProjectile();
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
		return true;
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x0007DBB5 File Offset: 0x0007BDB5
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x0007DBCC File Offset: 0x0007BDCC
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x0007DBE2 File Offset: 0x0007BDE2
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x0007DBF8 File Offset: 0x0007BDF8
	protected override Vector3 GetLaunchPosition()
	{
		return this.dummyProjectile.transform.position;
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x0007DC0C File Offset: 0x0007BE0C
	protected override Vector3 GetLaunchVelocity()
	{
		float d = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 a = this.centerOrigin.position - this.center.position;
		a /= d;
		Vector3 a2 = Mathf.Min(this.springConstant * this.maxDraw, a.magnitude * this.springConstant) * a.normalized * d;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		return a2 + averagedVelocity;
	}

	// Token: 0x04001EFE RID: 7934
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x04001EFF RID: 7935
	public LineRenderer elasticRight;

	// Token: 0x04001F00 RID: 7936
	public Transform leftArm;

	// Token: 0x04001F01 RID: 7937
	public Transform rightArm;

	// Token: 0x04001F02 RID: 7938
	public Transform center;

	// Token: 0x04001F03 RID: 7939
	public Transform centerOrigin;

	// Token: 0x04001F04 RID: 7940
	private GameObject dummyProjectile;

	// Token: 0x04001F05 RID: 7941
	public GameObject drawingHand;

	// Token: 0x04001F06 RID: 7942
	public InteractionPoint nock;

	// Token: 0x04001F07 RID: 7943
	public InteractionPoint grip;

	// Token: 0x04001F08 RID: 7944
	public float springConstant;

	// Token: 0x04001F09 RID: 7945
	public float maxDraw;

	// Token: 0x04001F0A RID: 7946
	private Transform leftHandSnap;

	// Token: 0x04001F0B RID: 7947
	private Transform rightHandSnap;

	// Token: 0x04001F0C RID: 7948
	public bool disableWhenNotInRoom;

	// Token: 0x04001F0D RID: 7949
	private bool hasDummyProjectile;

	// Token: 0x04001F0E RID: 7950
	private float delayLaunchTime = 0.07f;

	// Token: 0x04001F0F RID: 7951
	private float minTimeToLaunch = -1f;

	// Token: 0x04001F10 RID: 7952
	private float dummyProjectileColliderRadius;

	// Token: 0x04001F11 RID: 7953
	private float dummyProjectileInitialScale;

	// Token: 0x04001F12 RID: 7954
	private int projectileCount;

	// Token: 0x04001F13 RID: 7955
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x04001F14 RID: 7956
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x04001F15 RID: 7957
	private float _elasticIntialWidthMultiplier;

	// Token: 0x04001F16 RID: 7958
	private new VRRig myRig;

	// Token: 0x020003DE RID: 990
	public enum SlingshotState
	{
		// Token: 0x04001F18 RID: 7960
		NoState = 1,
		// Token: 0x04001F19 RID: 7961
		OnChest,
		// Token: 0x04001F1A RID: 7962
		LeftHandDrawing = 4,
		// Token: 0x04001F1B RID: 7963
		RightHandDrawing = 8
	}

	// Token: 0x020003DF RID: 991
	public enum SlingshotActions
	{
		// Token: 0x04001F1D RID: 7965
		Grab,
		// Token: 0x04001F1E RID: 7966
		Release
	}
}
