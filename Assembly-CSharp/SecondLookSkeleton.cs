using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class SecondLookSkeleton : MonoBehaviour
{
	// Token: 0x0600053B RID: 1339 RVA: 0x0001E0F0 File Offset: 0x0001C2F0
	private void Start()
	{
		this.playersSeen = new List<NetPlayer>();
		this.synchValues = base.GetComponent<SecondLookSkeletonSynchValues>();
		this.playerTransform = Camera.main.transform;
		this.tapped = !this.requireTappingToActivate;
		this.localCaught = false;
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.spookyGhost.SetActive(false);
		this.angerPointIndex = Random.Range(0, this.angerPoint.Length);
		this.angerPointChangedTime = Time.time;
		this.synchValues.angerPoint = this.angerPointIndex;
		this.spookyGhost.transform.position = this.angerPoint[this.synchValues.angerPoint].position;
		this.spookyGhost.transform.rotation = this.angerPoint[this.synchValues.angerPoint].rotation;
		this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
		this.rHits = new RaycastHit[20];
		this.lookedAway = false;
		this.firstLookActivated = false;
		this.animator.Play("ArmsOut");
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001E205 File Offset: 0x0001C405
	private void Update()
	{
		this.ProcessGhostState();
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001E210 File Offset: 0x0001C410
	public void ChangeState(SecondLookSkeleton.GhostState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		switch (newState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.gameObject.SetActive(false);
			this.audioSource.GTStop();
			this.audioSource.loop = false;
			if (this.IsMine())
			{
				this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
				this.angerPointIndex = this.synchValues.angerPoint;
				this.angerPointChangedTime = Time.time;
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			}
			this.currentState = SecondLookSkeleton.GhostState.Unactivated;
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.currentState = SecondLookSkeleton.GhostState.Activated;
			if (this.tapped)
			{
				GTAudioSourceExtensions.GTPlayClipAtPoint(this.initialScream, this.audioSource.transform.position, 1f);
				if (this.spookyText != null)
				{
					this.spookyText.SetActive(true);
				}
				this.spookyGhost.SetActive(true);
			}
			this.animator.Play("ArmsOut");
			this.spookyGhost.transform.rotation = Quaternion.LookRotation(this.playerTransform.position - this.spookyGhost.transform.position, Vector3.up);
			if (this.IsMine())
			{
				this.timeFirstAppeared = Time.time;
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.playersSeen.Clear();
			if (this.tapped)
			{
				this.spookyGhost.SetActive(true);
				this.animator.Play("CrawlPatrol");
				this.audioSource.loop = true;
				this.audioSource.clip = this.patrolLoop;
				this.audioSource.GTPlay();
			}
			if (this.IsMine())
			{
				this.currentNode = this.pathPoints[Random.Range(0, this.pathPoints.Length)];
				this.nextNode = this.currentNode.connectedNodes[Random.Range(0, this.currentNode.connectedNodes.Length)];
				this.SyncNodes();
				this.spookyGhost.transform.position = this.currentNode.transform.position;
			}
			this.currentState = SecondLookSkeleton.GhostState.Patrolling;
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.currentState = SecondLookSkeleton.GhostState.Chasing;
			this.resetChaseHistory.Clear();
			this.animator.Play("CrawlChase");
			this.localThrown = false;
			this.localCaught = false;
			if (this.tapped)
			{
				this.audioSource.clip = this.chaseLoop;
				this.audioSource.loop = true;
				this.audioSource.GTPlay();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.currentState = SecondLookSkeleton.GhostState.CaughtPlayer;
			this.heightOffset.localPosition = Vector3.zero;
			if (this.tapped)
			{
				this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
				this.audioSource.loop = true;
				this.audioSource.clip = this.carryingLoop;
				this.audioSource.GTPlay();
				this.animator.Play("ArmsOut");
			}
			if (!this.IsMine())
			{
				this.SetNodes();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.currentState = SecondLookSkeleton.GhostState.PlayerThrown;
			this.timeThrown = Time.time;
			this.localThrown = false;
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001E578 File Offset: 0x0001C778
	private void ProcessGhostState()
	{
		if (this.IsMine())
		{
			switch (this.currentState)
			{
			case SecondLookSkeleton.GhostState.Unactivated:
				if (this.changeAngerPointOnTimeInterval && Time.time - this.angerPointChangedTime > this.changeAngerPointTimeMinutes * 60f)
				{
					this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
					this.angerPointIndex = this.synchValues.angerPoint;
					this.angerPointChangedTime = Time.time;
				}
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
				this.CheckActivateGhost();
				return;
			case SecondLookSkeleton.GhostState.Activated:
				if (Time.time > this.timeFirstAppeared + this.timeToFirstDisappear)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
					return;
				}
				break;
			case SecondLookSkeleton.GhostState.Patrolling:
				if (!this.CheckPlayerSeen() && this.playersSeen.Count == 0)
				{
					this.PatrolMove();
					return;
				}
				this.StartChasing();
				return;
			case SecondLookSkeleton.GhostState.Chasing:
				if (!this.CheckPlayerSeen() || !this.CanGrab())
				{
					this.ChaseMove();
					return;
				}
				this.GrabPlayer();
				return;
			case SecondLookSkeleton.GhostState.CaughtPlayer:
				this.CaughtPlayerUpdate();
				return;
			case SecondLookSkeleton.GhostState.PlayerThrown:
				if (Time.time > this.timeThrown + this.timeThrownCooldown)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
				}
				break;
			case SecondLookSkeleton.GhostState.Reset:
				break;
			default:
				return;
			}
			return;
		}
		this.SetTappedState();
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.SetNodes();
			this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
			this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			this.CheckActivateGhost();
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.FollowPosition();
			this.CheckPlayerSeen();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			if (this.CheckPlayerSeen() && this.CanGrab())
			{
				this.GrabPlayer();
			}
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.CaughtPlayerUpdate();
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001E798 File Offset: 0x0001C998
	private void CaughtPlayerUpdate()
	{
		if (this.localThrown)
		{
			return;
		}
		if (this.GhostAtExit())
		{
			if (this.localCaught)
			{
				this.ChuckPlayer();
			}
			if (this.IsMine())
			{
				this.DeactivateGhost();
			}
			return;
		}
		this.CaughtMove();
		if (this.localCaught)
		{
			this.FloatPlayer();
			return;
		}
		if (this.CheckPlayerSeen() && this.CanGrab())
		{
			this.localCaught = true;
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001E800 File Offset: 0x0001CA00
	private void SetTappedState()
	{
		if (!this.tapped)
		{
			return;
		}
		if (this.spookyText != null && !this.spookyText.activeSelf)
		{
			this.spookyText.SetActive(true);
		}
		if (this.spookyGhost.activeSelf && this.currentState != SecondLookSkeleton.GhostState.Unactivated)
		{
			return;
		}
		this.spookyGhost.SetActive(true);
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.SetActive(false);
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.animator.Play("CrawlPatrol");
			this.audioSource.loop = true;
			this.audioSource.clip = this.patrolLoop;
			this.audioSource.GTPlay();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.audioSource.clip = this.chaseLoop;
			this.audioSource.loop = true;
			this.audioSource.GTPlay();
			this.animator.Play("CrawlChase");
			this.spookyGhost.SetActive(true);
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
			this.audioSource.loop = true;
			this.audioSource.clip = this.carryingLoop;
			this.audioSource.GTPlay();
			this.animator.Play("ArmsOut");
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001E984 File Offset: 0x0001CB84
	private void FollowPosition()
	{
		this.spookyGhost.transform.position = Vector3.Lerp(this.spookyGhost.transform.position, this.synchValues.position, 0.66f);
		this.spookyGhost.transform.rotation = Quaternion.Lerp(this.spookyGhost.transform.rotation, this.synchValues.rotation, 0.66f);
		if (this.currentState == SecondLookSkeleton.GhostState.Patrolling || this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.SetHeightOffset();
			return;
		}
		this.heightOffset.localPosition = Vector3.zero;
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001EA24 File Offset: 0x0001CC24
	private void CheckActivateGhost()
	{
		if (!this.tapped || this.currentState != SecondLookSkeleton.GhostState.Unactivated || this.playerTransform == null)
		{
			return;
		}
		this.currentlyLooking = this.IsCurrentlyLooking();
		if (this.requireSecondLookToActivate)
		{
			if (!this.firstLookActivated && this.currentlyLooking)
			{
				this.firstLookActivated = this.currentlyLooking;
				return;
			}
			if (this.firstLookActivated && !this.currentlyLooking)
			{
				this.lookedAway = true;
				return;
			}
			if (this.firstLookActivated && this.lookedAway && this.currentlyLooking)
			{
				this.firstLookActivated = false;
				this.lookedAway = false;
				this.ActivateGhost();
				return;
			}
		}
		else if (this.currentlyLooking)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0001EAD4 File Offset: 0x0001CCD4
	private bool CanSeePlayer()
	{
		return this.CanSeePlayerWithResults(out this.closest);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001EAE4 File Offset: 0x0001CCE4
	private bool CanSeePlayerWithResults(out RaycastHit closest)
	{
		Vector3 vector = this.playerTransform.position - this.lookSource.position;
		int num = Physics.RaycastNonAlloc(this.lookSource.position, vector.normalized, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		closest = this.rHits[0];
		if (num == 0)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (closest.distance > this.rHits[i].distance)
			{
				closest = this.rHits[i];
			}
		}
		return (this.playerMask & 1 << closest.collider.gameObject.layer) != 0;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001EBAF File Offset: 0x0001CDAF
	private void ActivateGhost()
	{
		if (this.IsMine())
		{
			this.ChangeState(SecondLookSkeleton.GhostState.Activated);
			return;
		}
		this.synchValues.SendRPC("RemoteActivateGhost", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001EBD7 File Offset: 0x0001CDD7
	private void StartChasing()
	{
		if (!this.IsMine())
		{
			return;
		}
		this.ChangeState(SecondLookSkeleton.GhostState.Chasing);
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0001EBEC File Offset: 0x0001CDEC
	private bool CheckPlayerSeen()
	{
		if (!this.tapped)
		{
			return false;
		}
		if (this.playersSeen.Contains(NetworkSystem.Instance.LocalPlayer))
		{
			return true;
		}
		if (!this.CanSeePlayer())
		{
			return false;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.synchValues.SendRPC("RemotePlayerSeen", RpcTarget.Others, Array.Empty<object>());
		}
		this.playersSeen.Add(NetworkSystem.Instance.LocalPlayer);
		return true;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001EC5E File Offset: 0x0001CE5E
	public void RemoteActivateGhost()
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Unactivated)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001EC76 File Offset: 0x0001CE76
	public void RemotePlayerSeen(NetPlayer player)
	{
		if (this.IsMine() && !this.playersSeen.Contains(player))
		{
			this.playersSeen.Add(player);
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001EC9C File Offset: 0x0001CE9C
	public void RemotePlayerCaught(NetPlayer player)
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			RigContainer x;
			VRRigCache.Instance.TryGetVrrig(player, out x);
			if (x != null && this.playersSeen.Contains(player))
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001ECE8 File Offset: 0x0001CEE8
	private bool IsCurrentlyLooking()
	{
		return Vector3.Dot(this.playerTransform.forward, -this.spookyGhost.transform.forward) > 0f && (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.ghostActivationDistance && this.CanSeePlayer();
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001ED59 File Offset: 0x0001CF59
	private void PatrolMove()
	{
		this.GhostMove(this.nextNode.transform, this.patrolSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, false);
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001ED80 File Offset: 0x0001CF80
	private void CheckReachedNextNode(bool forChuck, bool forChase)
	{
		if ((this.nextNode.transform.position - this.spookyGhost.transform.position).magnitude < this.reachNodeDist)
		{
			if (this.nextNode.connectedNodes.Length == 1)
			{
				this.currentNode = this.nextNode;
				this.nextNode = this.nextNode.connectedNodes[0];
				this.SyncNodes();
				return;
			}
			if (forChuck)
			{
				float distanceToExitNode = this.nextNode.distanceToExitNode;
				SkeletonPathingNode skeletonPathingNode = this.nextNode.connectedNodes[0];
				for (int i = 0; i < this.nextNode.connectedNodes.Length; i++)
				{
					if (this.nextNode.connectedNodes[i].distanceToExitNode <= distanceToExitNode)
					{
						skeletonPathingNode = this.nextNode.connectedNodes[i];
						distanceToExitNode = skeletonPathingNode.distanceToExitNode;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode;
				this.SyncNodes();
				return;
			}
			if (forChase)
			{
				float num = float.MaxValue;
				float num2 = num;
				RigContainer rigContainer = GorillaTagger.Instance.offlineVRRig.rigContainer;
				RigContainer rigContainer2 = rigContainer;
				for (int j = 0; j < this.playersSeen.Count; j++)
				{
					VRRigCache.Instance.TryGetVrrig(this.playersSeen[j], out rigContainer);
					if (!(rigContainer == null))
					{
						num = (rigContainer.transform.position - this.nextNode.transform.position).sqrMagnitude;
						if (num < num2)
						{
							rigContainer2 = rigContainer;
							num2 = num;
						}
					}
				}
				Vector3 vector = rigContainer2.transform.position - this.nextNode.transform.position;
				SkeletonPathingNode skeletonPathingNode2 = this.nextNode.connectedNodes[0];
				num2 = 0f;
				for (int k = 0; k < this.nextNode.connectedNodes.Length; k++)
				{
					Vector3 vector2 = this.nextNode.connectedNodes[k].transform.position - this.nextNode.transform.position;
					num = Mathf.Sign(Vector3.Dot(vector, vector2)) * Vector3.Project(vector, vector2).sqrMagnitude;
					if (num >= num2)
					{
						skeletonPathingNode2 = this.nextNode.connectedNodes[k];
						num2 = num;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode2;
				this.SyncNodes();
				this.resetChaseHistory.Add(this.nextNode);
				if (this.resetChaseHistory.Count > 8)
				{
					this.resetChaseHistory.RemoveAt(0);
				}
				if (this.resetChaseHistory.Count >= 8 && this.resetChaseHistory[0] == this.resetChaseHistory[2] == this.resetChaseHistory[4] == this.resetChaseHistory[6] && this.resetChaseHistory[1] == this.resetChaseHistory[3] == this.resetChaseHistory[5] == this.resetChaseHistory[7])
				{
					this.resetChaseHistory.Clear();
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
				}
				return;
			}
			SkeletonPathingNode skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
			for (int l = 0; l < 10; l++)
			{
				skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
				if (!skeletonPathingNode3.ejectionPoint && skeletonPathingNode3 != this.currentNode)
				{
					break;
				}
			}
			this.currentNode = this.nextNode;
			this.nextNode = skeletonPathingNode3;
			this.SyncNodes();
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001F14A File Offset: 0x0001D34A
	private void ChaseMove()
	{
		this.GhostMove(this.nextNode.transform, this.chaseSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, true);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001F171 File Offset: 0x0001D371
	private void CaughtMove()
	{
		this.GhostMove(this.nextNode.transform, this.caughtSpeed);
		this.CheckReachedNextNode(true, false);
		this.SyncNodes();
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001F198 File Offset: 0x0001D398
	private void SyncNodes()
	{
		this.synchValues.currentNode = this.pathPoints.IndexOfRef(this.currentNode);
		this.synchValues.nextNode = this.pathPoints.IndexOfRef(this.nextNode);
		this.synchValues.angerPoint = this.angerPointIndex;
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001F1F0 File Offset: 0x0001D3F0
	public void SetNodes()
	{
		if (this.synchValues.currentNode > this.pathPoints.Length || this.synchValues.currentNode < 0)
		{
			return;
		}
		this.currentNode = this.pathPoints[this.synchValues.currentNode];
		this.nextNode = this.pathPoints[this.synchValues.nextNode];
		this.angerPointIndex = this.synchValues.angerPoint;
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001F264 File Offset: 0x0001D464
	private bool GhostAtExit()
	{
		return this.currentNode.distanceToExitNode == 0f && (this.spookyGhost.transform.position - this.currentNode.transform.position).magnitude < this.reachNodeDist;
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001F2BC File Offset: 0x0001D4BC
	private void GhostMove(Transform target, float speed)
	{
		this.spookyGhost.transform.rotation = Quaternion.RotateTowards(this.spookyGhost.transform.rotation, Quaternion.LookRotation(target.position - this.spookyGhost.transform.position, Vector3.up), this.maxRotSpeed * Time.deltaTime);
		this.spookyGhost.transform.position += (target.position - this.spookyGhost.transform.position).normalized * speed * Time.deltaTime;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001F36D File Offset: 0x0001D56D
	private void DeactivateGhost()
	{
		this.ChangeState(SecondLookSkeleton.GhostState.PlayerThrown);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001F378 File Offset: 0x0001D578
	private bool CanGrab()
	{
		return (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.catchDistance;
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001F3B5 File Offset: 0x0001D5B5
	private void GrabPlayer()
	{
		if (this.IsMine())
		{
			if (this.currentState == SecondLookSkeleton.GhostState.Chasing)
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
			this.localCaught = true;
		}
		this.synchValues.SendRPC("RemotePlayerCaught", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001F3EC File Offset: 0x0001D5EC
	private void FloatPlayer()
	{
		RaycastHit raycastHit;
		if (this.CanSeePlayerWithResults(out raycastHit))
		{
			GorillaTagger.Instance.rigidbody.MovePosition(Vector3.MoveTowards(GorillaTagger.Instance.rigidbody.position, this.spookyGhost.transform.position + this.spookyGhost.transform.rotation * this.offsetGrabPosition, this.caughtSpeed * 10f * Time.deltaTime));
		}
		else
		{
			Vector3 vector = raycastHit.point - this.playerTransform.position;
			vector += GTPlayer.Instance.headCollider.radius * 1.05f * vector.normalized;
			GorillaTagger.Instance.transform.parent.position += vector;
			GTPlayer.Instance.InitializeValues();
		}
		GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
		EquipmentInteractor.instance.ForceStopClimbing();
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, 0.25f);
		GorillaTagger.Instance.StartVibration(true, this.hapticStrength / 4f, Time.deltaTime);
		GorillaTagger.Instance.StartVibration(false, this.hapticStrength / 4f, Time.deltaTime);
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001F53C File Offset: 0x0001D73C
	private void ChuckPlayer()
	{
		this.localCaught = false;
		this.localThrown = true;
		Vector3 vector = this.currentNode.transform.position - this.currentNode.connectedNodes[0].transform.position;
		GorillaTagger instance = GorillaTagger.Instance;
		Rigidbody rigidbody = (instance != null) ? instance.rigidbody : null;
		GTAudioSourceExtensions.GTPlayClipAtPoint(this.throwSound, this.audioSource.transform.position, 0.25f);
		this.audioSource.GTStop();
		this.audioSource.loop = false;
		if (rigidbody != null)
		{
			rigidbody.velocity = vector.normalized * this.throwForce;
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001F5F0 File Offset: 0x0001D7F0
	private void SetHeightOffset()
	{
		int num = Physics.RaycastNonAlloc(this.spookyGhost.transform.position + Vector3.up * this.bodyHeightOffset, Vector3.down, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		if (num == 0)
		{
			this.heightOffset.localPosition = Vector3.zero;
			return;
		}
		RaycastHit raycastHit = this.rHits[0];
		for (int i = 0; i < num; i++)
		{
			if (raycastHit.distance < this.rHits[i].distance)
			{
				raycastHit = this.rHits[i];
			}
		}
		this.heightOffset.localPosition = new Vector3(0f, -raycastHit.distance, 0f);
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001F6BB File Offset: 0x0001D8BB
	private bool IsMine()
	{
		return !NetworkSystem.Instance.InRoom || this.synchValues.IsMine;
	}

	// Token: 0x0400062A RID: 1578
	public Transform[] angerPoint;

	// Token: 0x0400062B RID: 1579
	public int angerPointIndex;

	// Token: 0x0400062C RID: 1580
	public SkeletonPathingNode[] pathPoints;

	// Token: 0x0400062D RID: 1581
	public SkeletonPathingNode[] exitPoints;

	// Token: 0x0400062E RID: 1582
	public Transform heightOffset;

	// Token: 0x0400062F RID: 1583
	public bool requireSecondLookToActivate;

	// Token: 0x04000630 RID: 1584
	public bool requireTappingToActivate;

	// Token: 0x04000631 RID: 1585
	public bool changeAngerPointOnTimeInterval;

	// Token: 0x04000632 RID: 1586
	public float changeAngerPointTimeMinutes = 3f;

	// Token: 0x04000633 RID: 1587
	private bool firstLookActivated;

	// Token: 0x04000634 RID: 1588
	private bool lookedAway;

	// Token: 0x04000635 RID: 1589
	private bool currentlyLooking;

	// Token: 0x04000636 RID: 1590
	public float ghostActivationDistance;

	// Token: 0x04000637 RID: 1591
	public GameObject spookyGhost;

	// Token: 0x04000638 RID: 1592
	public float timeFirstAppeared;

	// Token: 0x04000639 RID: 1593
	public float timeToFirstDisappear;

	// Token: 0x0400063A RID: 1594
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x0400063B RID: 1595
	public GameObject spookyText;

	// Token: 0x0400063C RID: 1596
	public float patrolSpeed;

	// Token: 0x0400063D RID: 1597
	public float chaseSpeed;

	// Token: 0x0400063E RID: 1598
	public float caughtSpeed;

	// Token: 0x0400063F RID: 1599
	public SkeletonPathingNode firstNode;

	// Token: 0x04000640 RID: 1600
	public SkeletonPathingNode currentNode;

	// Token: 0x04000641 RID: 1601
	public SkeletonPathingNode nextNode;

	// Token: 0x04000642 RID: 1602
	public Transform lookSource;

	// Token: 0x04000643 RID: 1603
	private Transform playerTransform;

	// Token: 0x04000644 RID: 1604
	public float reachNodeDist;

	// Token: 0x04000645 RID: 1605
	public float maxRotSpeed;

	// Token: 0x04000646 RID: 1606
	public float hapticStrength;

	// Token: 0x04000647 RID: 1607
	public float hapticDuration;

	// Token: 0x04000648 RID: 1608
	public Vector3 offsetGrabPosition;

	// Token: 0x04000649 RID: 1609
	public float throwForce;

	// Token: 0x0400064A RID: 1610
	public Animator animator;

	// Token: 0x0400064B RID: 1611
	public float bodyHeightOffset;

	// Token: 0x0400064C RID: 1612
	private float timeThrown;

	// Token: 0x0400064D RID: 1613
	public float timeThrownCooldown = 1f;

	// Token: 0x0400064E RID: 1614
	public float catchDistance;

	// Token: 0x0400064F RID: 1615
	public float maxSeeDistance;

	// Token: 0x04000650 RID: 1616
	private RaycastHit[] rHits;

	// Token: 0x04000651 RID: 1617
	public LayerMask mask;

	// Token: 0x04000652 RID: 1618
	public LayerMask playerMask;

	// Token: 0x04000653 RID: 1619
	public AudioSource audioSource;

	// Token: 0x04000654 RID: 1620
	public AudioClip initialScream;

	// Token: 0x04000655 RID: 1621
	public AudioClip patrolLoop;

	// Token: 0x04000656 RID: 1622
	public AudioClip chaseLoop;

	// Token: 0x04000657 RID: 1623
	public AudioClip grabbedSound;

	// Token: 0x04000658 RID: 1624
	public AudioClip carryingLoop;

	// Token: 0x04000659 RID: 1625
	public AudioClip throwSound;

	// Token: 0x0400065A RID: 1626
	public List<SkeletonPathingNode> resetChaseHistory = new List<SkeletonPathingNode>();

	// Token: 0x0400065B RID: 1627
	private SecondLookSkeletonSynchValues synchValues;

	// Token: 0x0400065C RID: 1628
	private bool localCaught;

	// Token: 0x0400065D RID: 1629
	private bool localThrown;

	// Token: 0x0400065E RID: 1630
	public List<NetPlayer> playersSeen;

	// Token: 0x0400065F RID: 1631
	public bool tapped;

	// Token: 0x04000660 RID: 1632
	private RaycastHit closest;

	// Token: 0x04000661 RID: 1633
	private float angerPointChangedTime;

	// Token: 0x020000D8 RID: 216
	public enum GhostState
	{
		// Token: 0x04000663 RID: 1635
		Unactivated,
		// Token: 0x04000664 RID: 1636
		Activated,
		// Token: 0x04000665 RID: 1637
		Patrolling,
		// Token: 0x04000666 RID: 1638
		Chasing,
		// Token: 0x04000667 RID: 1639
		CaughtPlayer,
		// Token: 0x04000668 RID: 1640
		PlayerThrown,
		// Token: 0x04000669 RID: 1641
		Reset
	}
}
