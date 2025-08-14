using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C41 RID: 3137
	[NetworkBehaviourWeaved(6)]
	public class LurkerGhost : NetworkComponent
	{
		// Token: 0x06004D88 RID: 19848 RVA: 0x00181246 File Offset: 0x0017F446
		protected override void Awake()
		{
			base.Awake();
			this.possibleTargets = new List<NetPlayer>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x0018126E File Offset: 0x0017F46E
		protected override void Start()
		{
			base.Start();
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x00181294 File Offset: 0x0017F494
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x001812A4 File Offset: 0x0017F4A4
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
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
					Transform item = (Transform)obj;
					this.waypoints.Add(item);
				}
			}
			int index = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[index];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(index);
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x001813C4 File Offset: 0x0017F5C4
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			if (transform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
			}
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x0018143C File Offset: 0x0017F63C
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x001814A8 File Offset: 0x0017F6A8
		private bool PickPlayer(float maxDistance)
		{
			if (base.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
				{
					if ((GorillaParent.instance.vrrigs[i].transform.position - base.transform.position).magnitude < maxDistance && GorillaParent.instance.vrrigs[i].creator != this.targetPlayer)
					{
						this.possibleTargets.Add(GorillaParent.instance.vrrigs[i].creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int index = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[index]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x001815D8 File Offset: 0x0017F7D8
		private void PickPlayer(NetPlayer player)
		{
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == player);
			if (num > -1 && num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.targetTransform = GorillaParent.instance.vrrigs[num].head.rigTarget;
				this.targetVRRig = GorillaParent.instance.vrrigs[num];
			}
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x00181680 File Offset: 0x0017F880
		private void SeekPlayer()
		{
			if (this.targetTransform == null)
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x00181754 File Offset: 0x0017F954
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x001817BC File Offset: 0x0017F9BC
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x001818E0 File Offset: 0x0017FAE0
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

		// Token: 0x06004D94 RID: 19860 RVA: 0x00181944 File Offset: 0x0017FB44
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == NetworkSystem.Instance.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x00181AC2 File Offset: 0x0017FCC2
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x06004D96 RID: 19862 RVA: 0x00181AE0 File Offset: 0x0017FCE0
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.IsMine)
				{
					if (this.currentWaypoint == null || Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float y = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 target = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), y, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, target, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06004D97 RID: 19863 RVA: 0x00181DED File Offset: 0x0017FFED
		// (set) Token: 0x06004D98 RID: 19864 RVA: 0x00181E17 File Offset: 0x00180017
		[Networked]
		[NetworkedWeaved(0, 6)]
		private unsafe LurkerGhost.LurkerGhostData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(LurkerGhost.LurkerGhostData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(LurkerGhost.LurkerGhostData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004D99 RID: 19865 RVA: 0x00181E42 File Offset: 0x00180042
		public override void WriteDataFusion()
		{
			this.Data = new LurkerGhost.LurkerGhostData(this.currentState, this.currentIndex, this.targetPlayer.ActorNumber, this.targetPosition);
		}

		// Token: 0x06004D9A RID: 19866 RVA: 0x00181E6C File Offset: 0x0018006C
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentIndex, this.Data.TargetActor, this.Data.TargetPos);
		}

		// Token: 0x06004D9B RID: 19867 RVA: 0x00181EB8 File Offset: 0x001800B8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentIndex);
			if (this.targetPlayer != null)
			{
				stream.SendNext(this.targetPlayer.ActorNumber);
			}
			else
			{
				stream.SendNext(-1);
			}
			stream.SendNext(this.targetPosition);
		}

		// Token: 0x06004D9C RID: 19868 RVA: 0x00181F34 File Offset: 0x00180134
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			LurkerGhost.ghostState state = (LurkerGhost.ghostState)stream.ReceiveNext();
			int index = (int)stream.ReceiveNext();
			int targetActorNumber = (int)stream.ReceiveNext();
			Vector3 targetPos = (Vector3)stream.ReceiveNext();
			this.ReadDataShared(state, index, targetActorNumber, targetPos);
		}

		// Token: 0x06004D9D RID: 19869 RVA: 0x00181F8C File Offset: 0x0018018C
		private void ReadDataShared(LurkerGhost.ghostState state, int index, int targetActorNumber, Vector3 targetPos)
		{
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = state;
			this.currentIndex = index;
			NetPlayer netPlayer = this.targetPlayer;
			this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
			this.targetPosition = targetPos;
			float num = 10000f;
			if (!this.targetPosition.IsValid(num))
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(this.targetPlayer, out rigContainer))
				{
					this.targetPosition = (this.targetPlayer.IsLocal ? rigContainer.Rig.transform.position : rigContainer.Rig.syncPos);
				}
				else
				{
					this.targetPosition = base.transform.position;
				}
			}
			if (this.targetPlayer != netPlayer)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != netPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x0018206D File Offset: 0x0018026D
		public override void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			base.OnOwnerChange(newOwner, previousOwner);
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06004DA0 RID: 19872 RVA: 0x00182190 File Offset: 0x00180390
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004DA1 RID: 19873 RVA: 0x001821A8 File Offset: 0x001803A8
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04005673 RID: 22131
		public float patrolSpeed = 3f;

		// Token: 0x04005674 RID: 22132
		public float seekSpeed = 6f;

		// Token: 0x04005675 RID: 22133
		public float chargeSpeed = 6f;

		// Token: 0x04005676 RID: 22134
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x04005677 RID: 22135
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x04005678 RID: 22136
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x04005679 RID: 22137
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x0400567A RID: 22138
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x0400567B RID: 22139
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x0400567C RID: 22140
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x0400567D RID: 22141
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x0400567E RID: 22142
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x0400567F RID: 22143
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04005680 RID: 22144
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x04005681 RID: 22145
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x04005682 RID: 22146
		public float hapticDuration = 1.5f;

		// Token: 0x04005683 RID: 22147
		public GameObject waypointsContainer;

		// Token: 0x04005684 RID: 22148
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x04005685 RID: 22149
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x04005686 RID: 22150
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x04005687 RID: 22151
		private Transform currentWaypoint;

		// Token: 0x04005688 RID: 22152
		public Material visibleMaterial;

		// Token: 0x04005689 RID: 22153
		public Material scryableMaterial;

		// Token: 0x0400568A RID: 22154
		public Material visibleMaterialBones;

		// Token: 0x0400568B RID: 22155
		public Material scryableMaterialBones;

		// Token: 0x0400568C RID: 22156
		public MeshRenderer meshRenderer;

		// Token: 0x0400568D RID: 22157
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x0400568E RID: 22158
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400568F RID: 22159
		public AudioClip patrolAudio;

		// Token: 0x04005690 RID: 22160
		public AudioClip huntAudio;

		// Token: 0x04005691 RID: 22161
		public AudioClip possessedAudio;

		// Token: 0x04005692 RID: 22162
		public ThrowableSetDressing scryingGlass;

		// Token: 0x04005693 RID: 22163
		public float scryingAngerAngle;

		// Token: 0x04005694 RID: 22164
		public float scryingAngerDelay;

		// Token: 0x04005695 RID: 22165
		public float seekAheadDistance;

		// Token: 0x04005696 RID: 22166
		public float seekCloseEnoughDistance;

		// Token: 0x04005697 RID: 22167
		private float scryingAngerAfterTimestamp;

		// Token: 0x04005698 RID: 22168
		private int currentRepeatHuntTimes;

		// Token: 0x04005699 RID: 22169
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x0400569A RID: 22170
		private int currentIndex;

		// Token: 0x0400569B RID: 22171
		private LurkerGhost.ghostState currentState;

		// Token: 0x0400569C RID: 22172
		private float cooldownTimeRemaining;

		// Token: 0x0400569D RID: 22173
		private List<NetPlayer> possibleTargets;

		// Token: 0x0400569E RID: 22174
		private NetPlayer targetPlayer;

		// Token: 0x0400569F RID: 22175
		private Transform targetTransform;

		// Token: 0x040056A0 RID: 22176
		private float huntedPassedTime;

		// Token: 0x040056A1 RID: 22177
		private Vector3 targetPosition;

		// Token: 0x040056A2 RID: 22178
		private Quaternion targetRotation;

		// Token: 0x040056A3 RID: 22179
		private VRRig targetVRRig;

		// Token: 0x040056A4 RID: 22180
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x040056A5 RID: 22181
		private VRRig lastHauntedVRRig;

		// Token: 0x040056A6 RID: 22182
		private float nextTagTime;

		// Token: 0x040056A7 RID: 22183
		private NetPlayer passingPlayer;

		// Token: 0x040056A8 RID: 22184
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x040056A9 RID: 22185
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 6)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private LurkerGhost.LurkerGhostData _Data;

		// Token: 0x02000C42 RID: 3138
		private enum ghostState
		{
			// Token: 0x040056AB RID: 22187
			patrol,
			// Token: 0x040056AC RID: 22188
			seek,
			// Token: 0x040056AD RID: 22189
			charge,
			// Token: 0x040056AE RID: 22190
			possess
		}

		// Token: 0x02000C43 RID: 3139
		[NetworkStructWeaved(6)]
		[StructLayout(LayoutKind.Explicit, Size = 24)]
		private struct LurkerGhostData : INetworkStruct
		{
			// Token: 0x17000752 RID: 1874
			// (get) Token: 0x06004DA2 RID: 19874 RVA: 0x001821BC File Offset: 0x001803BC
			// (set) Token: 0x06004DA3 RID: 19875 RVA: 0x001821C4 File Offset: 0x001803C4
			public LurkerGhost.ghostState CurrentState { readonly get; set; }

			// Token: 0x17000753 RID: 1875
			// (get) Token: 0x06004DA4 RID: 19876 RVA: 0x001821CD File Offset: 0x001803CD
			// (set) Token: 0x06004DA5 RID: 19877 RVA: 0x001821D5 File Offset: 0x001803D5
			public int CurrentIndex { readonly get; set; }

			// Token: 0x17000754 RID: 1876
			// (get) Token: 0x06004DA6 RID: 19878 RVA: 0x001821DE File Offset: 0x001803DE
			// (set) Token: 0x06004DA7 RID: 19879 RVA: 0x001821E6 File Offset: 0x001803E6
			public int TargetActor { readonly get; set; }

			// Token: 0x17000755 RID: 1877
			// (get) Token: 0x06004DA8 RID: 19880 RVA: 0x001821EF File Offset: 0x001803EF
			// (set) Token: 0x06004DA9 RID: 19881 RVA: 0x00182201 File Offset: 0x00180401
			[Networked]
			public unsafe Vector3 TargetPos
			{
				readonly get
				{
					return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos);
				}
				set
				{
					*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos) = value;
				}
			}

			// Token: 0x06004DAA RID: 19882 RVA: 0x00182214 File Offset: 0x00180414
			public LurkerGhostData(LurkerGhost.ghostState state, int index, int actor, Vector3 pos)
			{
				this.CurrentState = state;
				this.CurrentIndex = index;
				this.TargetActor = actor;
				this.TargetPos = pos;
			}

			// Token: 0x040056B2 RID: 22194
			[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(12)]
			private FixedStorage@3 _TargetPos;
		}
	}
}
