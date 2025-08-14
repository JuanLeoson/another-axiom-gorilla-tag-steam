using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000855 RID: 2133
[NetworkBehaviourWeaved(1)]
public class WanderingGhost : NetworkComponent
{
	// Token: 0x0600359A RID: 13722 RVA: 0x00118F20 File Offset: 0x00117120
	protected override void Start()
	{
		base.Start();
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x0600359B RID: 13723 RVA: 0x00118F87 File Offset: 0x00117187
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x0600359C RID: 13724 RVA: 0x00118FB8 File Offset: 0x001171B8
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x0011907C File Offset: 0x0011727C
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
			if (zoneBasedObject == null)
			{
				zoneBasedObject = this.lastWaypointRegion;
			}
			if (zoneBasedObject == null)
			{
				return;
			}
			this.lastWaypointRegion = zoneBasedObject;
			this.waypoints.Clear();
			foreach (object obj in zoneBasedObject.transform)
			{
				Transform transform = (Transform)obj;
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int index = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[index];
		this.waypoints.RemoveAt(index);
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x0011918C File Offset: 0x0011738C
	private void Patrol()
	{
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x00119230 File Offset: 0x00117430
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaHand) || this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060035A0 RID: 13728 RVA: 0x0011929C File Offset: 0x0011749C
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.GTStop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.GTPlay();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.GTPlayOneShot(this.appearAudio.GetRandomItem<AudioClip>(), 1f);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00119358 File Offset: 0x00117558
	private void UpdateState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x0011941C File Offset: 0x0011761C
	private void HauntObjects()
	{
		Collider[] array = new Collider[20];
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
		for (int i = 0; i < num; i++)
		{
			if (array[i].CompareTag("HauntedObject"))
			{
				UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
				if (triggerHauntedObjects != null)
				{
					triggerHauntedObjects(array[i].gameObject);
				}
			}
		}
	}

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x060035A3 RID: 13731 RVA: 0x0011947D File Offset: 0x0011767D
	// (set) Token: 0x060035A4 RID: 13732 RVA: 0x001194A7 File Offset: 0x001176A7
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe WanderingGhost.ghostState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (WanderingGhost.ghostState)this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = (int)value;
		}
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x001194D2 File Offset: 0x001176D2
	public override void WriteDataFusion()
	{
		this.Data = this.currentState;
	}

	// Token: 0x060035A6 RID: 13734 RVA: 0x001194E0 File Offset: 0x001176E0
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data);
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x001194EE File Offset: 0x001176EE
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.currentState);
	}

	// Token: 0x060035A8 RID: 13736 RVA: 0x00119510 File Offset: 0x00117710
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		WanderingGhost.ghostState state = (WanderingGhost.ghostState)stream.ReceiveNext();
		this.ReadDataShared(state);
	}

	// Token: 0x060035A9 RID: 13737 RVA: 0x0011953E File Offset: 0x0011773E
	private void ReadDataShared(WanderingGhost.ghostState state)
	{
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = state;
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x00119561 File Offset: 0x00117761
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x060035AB RID: 13739 RVA: 0x00119580 File Offset: 0x00117780
	private void SpawnFlowerNearby()
	{
		Vector3 position = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), out raycastHit, 3f, this.flowerGroundMask))
		{
			position = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = position;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x001196C0 File Offset: 0x001178C0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x001196D8 File Offset: 0x001178D8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400427C RID: 17020
	public float patrolSpeed = 3f;

	// Token: 0x0400427D RID: 17021
	public float idleStayDuration = 5f;

	// Token: 0x0400427E RID: 17022
	public float sphereColliderRadius = 2f;

	// Token: 0x0400427F RID: 17023
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x04004280 RID: 17024
	public Vector3 flowerDisabledPosition;

	// Token: 0x04004281 RID: 17025
	public float flowerSpawnRadius;

	// Token: 0x04004282 RID: 17026
	public float flowerSpawnDuration;

	// Token: 0x04004283 RID: 17027
	public LayerMask flowerGroundMask;

	// Token: 0x04004284 RID: 17028
	public MeshRenderer mrenderer;

	// Token: 0x04004285 RID: 17029
	public Material visibleMaterial;

	// Token: 0x04004286 RID: 17030
	public Material scryableMaterial;

	// Token: 0x04004287 RID: 17031
	public GameObject waypointsContainer;

	// Token: 0x04004288 RID: 17032
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x04004289 RID: 17033
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x0400428A RID: 17034
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x0400428B RID: 17035
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x0400428C RID: 17036
	public string debugForceWaypointRegion;

	// Token: 0x0400428D RID: 17037
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400428E RID: 17038
	public AudioClip[] appearAudio;

	// Token: 0x0400428F RID: 17039
	public float idleVolume;

	// Token: 0x04004290 RID: 17040
	public AudioClip patrolAudio;

	// Token: 0x04004291 RID: 17041
	public float patrolVolume;

	// Token: 0x04004292 RID: 17042
	private WanderingGhost.ghostState currentState;

	// Token: 0x04004293 RID: 17043
	private float idlePassedTime;

	// Token: 0x04004294 RID: 17044
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x04004295 RID: 17045
	private Vector3 hoverVelocity;

	// Token: 0x04004296 RID: 17046
	public float hoverRectifyForce;

	// Token: 0x04004297 RID: 17047
	public float hoverRandomForce;

	// Token: 0x04004298 RID: 17048
	public float hoverDrag;

	// Token: 0x04004299 RID: 17049
	private const int maxColliders = 10;

	// Token: 0x0400429A RID: 17050
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x0400429B RID: 17051
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private WanderingGhost.ghostState _Data;

	// Token: 0x02000856 RID: 2134
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x060035AF RID: 13743 RVA: 0x001196EC File Offset: 0x001178EC
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x0400429C RID: 17052
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x0400429D RID: 17053
		public Transform _transform;
	}

	// Token: 0x02000857 RID: 2135
	private enum ghostState
	{
		// Token: 0x0400429F RID: 17055
		patrol,
		// Token: 0x040042A0 RID: 17056
		idle
	}
}
