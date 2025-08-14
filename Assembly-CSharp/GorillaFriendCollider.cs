using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020009FF RID: 2559
public class GorillaFriendCollider : MonoBehaviour
{
	// Token: 0x06003E8E RID: 16014 RVA: 0x0013E858 File Offset: 0x0013CA58
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		this.jiggleAmount = Random.Range(0f, 1f);
		this.tagAndBodyLayerMask = (LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		}));
	}

	// Token: 0x06003E8F RID: 16015 RVA: 0x0013E8BF File Offset: 0x0013CABF
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x06003E90 RID: 16016 RVA: 0x0013E8E0 File Offset: 0x0013CAE0
	private void Update()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f + this.jiggleAmount;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (NetworkSystem.Instance.InRoom || this.runCheckWhileNotInRoom)
		{
			this.RefreshPlayersInSphere();
		}
	}

	// Token: 0x06003E91 RID: 16017 RVA: 0x0013E948 File Offset: 0x0013CB48
	public void RefreshPlayersInSphere()
	{
		this.playerIDsCurrentlyTouching.Clear();
		if (this.thisBox != null)
		{
			this.collisions = Physics.OverlapBoxNonAlloc(this.thisBox.transform.position, this.thisBox.size / 2f, this.overlapColliders, this.thisBox.transform.rotation, this.tagAndBodyLayerMask);
		}
		else
		{
			this.collisions = Physics.OverlapSphereNonAlloc(base.transform.position, this.thisCapsule.radius, this.overlapColliders, this.tagAndBodyLayerMask);
		}
		this.collisions = Mathf.Min(this.collisions, this.overlapColliders.Length);
		if (this.collisions > 0)
		{
			for (int i = 0; i < this.collisions; i++)
			{
				this.otherCollider = this.overlapColliders[i];
				if (!(this.otherCollider == null) && !(this.otherCollider.attachedRigidbody == null))
				{
					this.otherColliderGO = this.otherCollider.attachedRigidbody.gameObject;
					this.collidingRig = this.otherColliderGO.GetComponent<VRRig>();
					if (this.collidingRig == null || this.collidingRig.creator == null || this.collidingRig.creator.IsNull || string.IsNullOrEmpty(this.collidingRig.creator.UserId))
					{
						GTPlayer component = this.otherColliderGO.GetComponent<GTPlayer>();
						if (component == null || NetworkSystem.Instance.LocalPlayer == null)
						{
							goto IL_264;
						}
						if (this.thisCapsule != null && this.applyCapsuleYLimits)
						{
							float y = component.bodyCollider.transform.position.y;
							if (y < this.capsuleColliderYLimits.x || y > this.capsuleColliderYLimits.y)
							{
								goto IL_264;
							}
						}
						string userId = NetworkSystem.Instance.LocalPlayer.UserId;
						this.AddUserID(userId);
					}
					else
					{
						if (this.thisCapsule != null && this.applyCapsuleYLimits)
						{
							float y2 = this.collidingRig.bodyTransform.transform.position.y;
							if (y2 < this.capsuleColliderYLimits.x || y2 > this.capsuleColliderYLimits.y)
							{
								goto IL_264;
							}
						}
						string userId = this.collidingRig.creator.UserId;
						this.AddUserID(userId);
					}
					this.overlapColliders[i] = null;
				}
				IL_264:;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
			{
				GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
				GorillaComputer.instance.friendJoinCollider = this;
				GorillaComputer.instance.UpdateScreen();
			}
			this.otherCollider = null;
			this.otherColliderGO = null;
			this.collidingRig = null;
		}
	}

	// Token: 0x04004A85 RID: 19077
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x04004A86 RID: 19078
	private CapsuleCollider thisCapsule;

	// Token: 0x04004A87 RID: 19079
	private BoxCollider thisBox;

	// Token: 0x04004A88 RID: 19080
	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	// Token: 0x04004A89 RID: 19081
	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	// Token: 0x04004A8A RID: 19082
	public bool runCheckWhileNotInRoom;

	// Token: 0x04004A8B RID: 19083
	public string[] myAllowedMapsToJoin;

	// Token: 0x04004A8C RID: 19084
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x04004A8D RID: 19085
	private int tagAndBodyLayerMask;

	// Token: 0x04004A8E RID: 19086
	private float jiggleAmount;

	// Token: 0x04004A8F RID: 19087
	private Collider otherCollider;

	// Token: 0x04004A90 RID: 19088
	private GameObject otherColliderGO;

	// Token: 0x04004A91 RID: 19089
	private VRRig collidingRig;

	// Token: 0x04004A92 RID: 19090
	private int collisions;

	// Token: 0x04004A93 RID: 19091
	private WaitForSeconds wait1Sec = new WaitForSeconds(1f);

	// Token: 0x04004A94 RID: 19092
	public bool manualRefreshOnly;

	// Token: 0x04004A95 RID: 19093
	private float _nextUpdateTime = -1f;
}
