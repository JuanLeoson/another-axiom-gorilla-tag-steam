using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200044A RID: 1098
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x06001AFC RID: 6908 RVA: 0x0008FA98 File Offset: 0x0008DC98
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0008FAA0 File Offset: 0x0008DCA0
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.chestColliderLeft = this._GetChestColliderByPath(rig, "RigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		this.chestColliderRight = this._GetChestColliderByPath(rig, "RigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		this.currentChestCollider = this.chestColliderLeft;
		Transform[] array;
		string str;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out array, out str))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + str, this);
			return;
		}
		this.parentHandLeft = array[9];
		this.parentHandRight = array[27];
		this.parentHand = this.parentHandRight;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		this.selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x0008FBE8 File Offset: 0x0008DDE8
	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		Transform transform;
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out transform))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		Collider component = transform.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		return component;
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x0008FC58 File Offset: 0x0008DE58
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = GTPlayer.Instance.leftHandFollower;
			}
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = GTPlayer.Instance.rightHandFollower;
			}
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x0008FCEC File Offset: 0x0008DEEC
	internal override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x0008FD10 File Offset: 0x0008DF10
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
		return true;
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x0008FD3C File Offset: 0x0008DF3C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 positionTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight;
			Quaternion rotationTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight;
			this.UpdateNonPlayingPosition(positionTarget, rotationTarget);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 positionTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight;
			Quaternion rotationTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight;
			this.UpdateNonPlayingPosition(positionTarget2, rotationTarget2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion rhs = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight;
			Vector3 point = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight;
			Quaternion quaternion = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * rhs;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion;
			}
			Vector3 vector = this.currentChestCollider.transform.position + base.transform.rotation * point;
			if (!this.positionSnapped && (base.transform.position - vector).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * point, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * base.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!NetworkSystem.Instance.InRoom || this.selfInstrumentIndex <= -1)
									{
										break;
									}
									NetworkView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.SendRPC("RPC_PlaySelfOnlyInstrument", RpcTarget.Others, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x000902D8 File Offset: 0x0008E4D8
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x00090328 File Offset: 0x0008E528
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x00090360 File Offset: 0x0008E560
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, QueryTriggerInteraction.Collide);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x00090494 File Offset: 0x0008E694
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x00090558 File Offset: 0x0008E758
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x0009057B File Offset: 0x0008E77B
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x00090591 File Offset: 0x0008E791
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x000905B4 File Offset: 0x0008E7B4
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x00090614 File Offset: 0x0008E814
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x00090672 File Offset: 0x0008E872
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x00090696 File Offset: 0x0008E896
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x000906BA File Offset: 0x0008E8BA
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x000906DE File Offset: 0x0008E8DE
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x00090702 File Offset: 0x0008E902
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x00090726 File Offset: 0x0008E926
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x0009074C File Offset: 0x0008E94C
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x04002331 RID: 9009
	public Vector3 chestOffsetLeft;

	// Token: 0x04002332 RID: 9010
	public Vector3 chestOffsetRight;

	// Token: 0x04002333 RID: 9011
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x04002334 RID: 9012
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x04002335 RID: 9013
	public Quaternion chestRotationOffset;

	// Token: 0x04002336 RID: 9014
	[NonSerialized]
	public Collider currentChestCollider;

	// Token: 0x04002337 RID: 9015
	[NonSerialized]
	public Collider chestColliderLeft;

	// Token: 0x04002338 RID: 9016
	[NonSerialized]
	public Collider chestColliderRight;

	// Token: 0x04002339 RID: 9017
	public float lerpValue = 0.25f;

	// Token: 0x0400233A RID: 9018
	public AudioSource audioSource;

	// Token: 0x0400233B RID: 9019
	private Transform parentHand;

	// Token: 0x0400233C RID: 9020
	private Transform parentHandLeft;

	// Token: 0x0400233D RID: 9021
	private Transform parentHandRight;

	// Token: 0x0400233E RID: 9022
	public float unsnapDistance;

	// Token: 0x0400233F RID: 9023
	public float snapDistance;

	// Token: 0x04002340 RID: 9024
	public Vector3 startPositionLeft;

	// Token: 0x04002341 RID: 9025
	public Quaternion startQuatLeft;

	// Token: 0x04002342 RID: 9026
	public Vector3 reverseGripPositionLeft;

	// Token: 0x04002343 RID: 9027
	public Quaternion reverseGripQuatLeft;

	// Token: 0x04002344 RID: 9028
	public Vector3 startPositionRight;

	// Token: 0x04002345 RID: 9029
	public Quaternion startQuatRight;

	// Token: 0x04002346 RID: 9030
	public Vector3 reverseGripPositionRight;

	// Token: 0x04002347 RID: 9031
	public Quaternion reverseGripQuatRight;

	// Token: 0x04002348 RID: 9032
	public float angleLerpSnap = 1f;

	// Token: 0x04002349 RID: 9033
	public float vectorLerpSnap = 0.01f;

	// Token: 0x0400234A RID: 9034
	private bool angleSnapped;

	// Token: 0x0400234B RID: 9035
	private bool positionSnapped;

	// Token: 0x0400234C RID: 9036
	public Transform chestTouch;

	// Token: 0x0400234D RID: 9037
	private int collidersHitCount;

	// Token: 0x0400234E RID: 9038
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x0400234F RID: 9039
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x04002350 RID: 9040
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x04002351 RID: 9041
	private RaycastHit nullHit;

	// Token: 0x04002352 RID: 9042
	public Collider[] collidersToBeIn;

	// Token: 0x04002353 RID: 9043
	public LayerMask interactableMask;

	// Token: 0x04002354 RID: 9044
	public int currentFretIndex;

	// Token: 0x04002355 RID: 9045
	public int lastFretIndex;

	// Token: 0x04002356 RID: 9046
	public Collider[] frets;

	// Token: 0x04002357 RID: 9047
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x04002358 RID: 9048
	public AudioClip[] audioClips;

	// Token: 0x04002359 RID: 9049
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x0400235A RID: 9050
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x0400235B RID: 9051
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x0400235C RID: 9052
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x0400235D RID: 9053
	private float sphereRadius;

	// Token: 0x0400235E RID: 9054
	private bool anyHit;

	// Token: 0x0400235F RID: 9055
	private bool handIn;

	// Token: 0x04002360 RID: 9056
	private Vector3 spherecastSweep;

	// Token: 0x04002361 RID: 9057
	public Collider strumCollider;

	// Token: 0x04002362 RID: 9058
	public float maxVolume = 1f;

	// Token: 0x04002363 RID: 9059
	public float minVolume = 0.05f;

	// Token: 0x04002364 RID: 9060
	public float maxVelocity = 2f;

	// Token: 0x04002365 RID: 9061
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x04002366 RID: 9062
	private int selfInstrumentIndex = -1;

	// Token: 0x04002367 RID: 9063
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x04002368 RID: 9064
	private Vector3 startingLeftChestOffset;

	// Token: 0x04002369 RID: 9065
	private Vector3 startingRightChestOffset;

	// Token: 0x0400236A RID: 9066
	private float startingUnsnapDistance;

	// Token: 0x0200044B RID: 1099
	private enum GuitarStates
	{
		// Token: 0x0400236C RID: 9068
		Club = 1,
		// Token: 0x0400236D RID: 9069
		HeldReverseGrip,
		// Token: 0x0400236E RID: 9070
		Playing = 4
	}
}
