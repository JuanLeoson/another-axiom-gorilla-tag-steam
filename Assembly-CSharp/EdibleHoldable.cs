using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000437 RID: 1079
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001A4E RID: 6734 RVA: 0x0008CCFE File Offset: 0x0008AEFE
	// (set) Token: 0x06001A4F RID: 6735 RVA: 0x0008CD06 File Offset: 0x0008AF06
	public int lastBiterActorID { get; private set; } = -1;

	// Token: 0x06001A50 RID: 6736 RVA: 0x0008CD0F File Offset: 0x0008AF0F
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x0008CD44 File Offset: 0x0008AF44
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x0008CD60 File Offset: 0x0008AF60
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x0008CD68 File Offset: 0x0008AF68
	internal override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000236A3 File Offset: 0x000218A3
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x0008CD70 File Offset: 0x0008AF70
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0008CD78 File Offset: 0x0008AF78
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x0008CD94 File Offset: 0x0008AF94
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig vrrig = null;
			VRRig vrrig2 = null;
			for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
			{
				VRRig vrrig3 = GorillaParent.instance.vrrigs[i];
				if (!vrrig3.isOfflineVRRig)
				{
					if (vrrig3.head == null || vrrig3.head.rigTarget == null)
					{
						break;
					}
					Transform transform = vrrig3.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						vrrig2 = vrrig3;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					int lastBiterActorID;
					if (!vrrig)
					{
						lastBiterActorID = -1;
					}
					else
					{
						NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
						lastBiterActorID = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID;
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(vrrig, (int)this.itemState);
					}
				}
				else
				{
					int lastBiterActorID2;
					if (!vrrig2)
					{
						lastBiterActorID2 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer2 = vrrig2.OwningNetPlayer;
						lastBiterActorID2 = ((owningNetPlayer2 != null) ? owningNetPlayer2.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID2;
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(vrrig2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x0008D014 File Offset: 0x0008B214
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x0008D044 File Offset: 0x0008B244
	protected virtual void OnEdibleHoldableStateChange()
	{
		float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float volumeScale = 0.08f;
		int num = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num = 3;
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num2].SetActive(false);
		this.edibleMeshObjects[num].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num3 = float.PositiveInfinity;
			for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
			{
				VRRig vrrig2 = GorillaParent.instance.vrrigs[j];
				if (vrrig2.head == null || vrrig2.head.rigTarget == null)
				{
					break;
				}
				Transform transform = vrrig2.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
			if (vrrig != null)
			{
				EdibleHoldable.BiteEvent biteEvent = vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld;
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
				if (vrrig.isOfflineVRRig && this.itemState != TransferrableObject.ItemStates.State0)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
			}
		}
		this.eatSoundSource.GTPlayOneShot(this.eatSounds[num], volumeScale);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), amplitude, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, amplitude, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001A5B RID: 6747 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04002298 RID: 8856
	public AudioClip[] eatSounds;

	// Token: 0x04002299 RID: 8857
	public GameObject[] edibleMeshObjects;

	// Token: 0x0400229B RID: 8859
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x0400229C RID: 8860
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x0400229D RID: 8861
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x0400229E RID: 8862
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x0400229F RID: 8863
	public float eatMinimumCooldown = 1f;

	// Token: 0x040022A0 RID: 8864
	public float respawnTime = 7f;

	// Token: 0x040022A1 RID: 8865
	public float biteDistance = 0.1666667f;

	// Token: 0x040022A2 RID: 8866
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x040022A3 RID: 8867
	public Transform biteSpot;

	// Token: 0x040022A4 RID: 8868
	public bool inBiteZone;

	// Token: 0x040022A5 RID: 8869
	public AudioSource eatSoundSource;

	// Token: 0x040022A6 RID: 8870
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x040022A7 RID: 8871
	private IResettableItem[] iResettableItems;

	// Token: 0x02000438 RID: 1080
	private enum EdibleHoldableStates
	{
		// Token: 0x040022A9 RID: 8873
		EatingState0 = 1,
		// Token: 0x040022AA RID: 8874
		EatingState1,
		// Token: 0x040022AB RID: 8875
		EatingState2 = 4,
		// Token: 0x040022AC RID: 8876
		EatingState3 = 8
	}

	// Token: 0x02000439 RID: 1081
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
