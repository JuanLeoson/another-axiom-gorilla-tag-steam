using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200043A RID: 1082
public class GeodeItem : TransferrableObject
{
	// Token: 0x06001A5E RID: 6750 RVA: 0x0008D2E5 File Offset: 0x0008B4E5
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x0008D307 File Offset: 0x0008B507
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x0008D323 File Offset: 0x0008B523
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x0008D338 File Offset: 0x0008B538
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && this.itemState != TransferrableObject.ItemStates.State0 && !base.InHand();
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x0008D35C File Offset: 0x0008B55C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		UnityEvent<GeodeItem> onGeodeGrabbed = this.OnGeodeGrabbed;
		if (onGeodeGrabbed == null)
		{
			return;
		}
		onGeodeGrabbed.Invoke(this);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x0008D378 File Offset: 0x0008B578
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x0008D3E0 File Offset: 0x0008B5E0
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, QueryTriggerInteraction.Collide);
			this.hitLastFrame = (num > 0);
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		UnityEvent<GeodeItem> onGeodeCracked = this.OnGeodeCracked;
		if (onGeodeCracked != null)
		{
			onGeodeCracked.Invoke(this);
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = (this.randomizeGeode ? this.RandomPickCrackedGeode() : 0);
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x0008D508 File Offset: 0x0008B708
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x0008D53C File Offset: 0x0008B73C
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		RigContainer rigContainer;
		if (NetworkSystem.Instance.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
		{
			rigContainer.Rig.netView.SendRPC("RPC_PlayGeodeEffect", RpcTarget.All, new object[]
			{
				this.geodeFullMesh.transform.position
			});
			this.effectsHaveBeenPlayed = true;
		}
		if (!NetworkSystem.Instance.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.GTPlay();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x0008D635 File Offset: 0x0008B835
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x040022AD RID: 8877
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x040022AE RID: 8878
	public LayerMask collisionLayerMask;

	// Token: 0x040022AF RID: 8879
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040022B0 RID: 8880
	public float cooldown = 5f;

	// Token: 0x040022B1 RID: 8881
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x040022B2 RID: 8882
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x040022B3 RID: 8883
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x040022B4 RID: 8884
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x040022B5 RID: 8885
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x040022B6 RID: 8886
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x040022B7 RID: 8887
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x040022B8 RID: 8888
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040022B9 RID: 8889
	public bool randomizeGeode = true;

	// Token: 0x040022BA RID: 8890
	public UnityEvent<GeodeItem> OnGeodeCracked;

	// Token: 0x040022BB RID: 8891
	public UnityEvent<GeodeItem> OnGeodeGrabbed;

	// Token: 0x040022BC RID: 8892
	private bool hasEffectsGameObject;

	// Token: 0x040022BD RID: 8893
	private bool effectsHaveBeenPlayed;

	// Token: 0x040022BE RID: 8894
	private RaycastHit hit;

	// Token: 0x040022BF RID: 8895
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x040022C0 RID: 8896
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x040022C1 RID: 8897
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x040022C2 RID: 8898
	private int index;
}
