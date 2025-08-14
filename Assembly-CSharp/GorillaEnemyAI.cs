using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007F4 RID: 2036
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x060032F4 RID: 13044 RVA: 0x001092CC File Offset: 0x001074CC
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x060032F5 RID: 13045 RVA: 0x00109324 File Offset: 0x00107524
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x060032F6 RID: 13046 RVA: 0x00109388 File Offset: 0x00107588
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.velocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x00109478 File Offset: 0x00107678
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsOfType<VRRig>();
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x060032F8 RID: 13048 RVA: 0x001094E9 File Offset: 0x001076E9
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x060032F9 RID: 13049 RVA: 0x00109505 File Offset: 0x00107705
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x060032FA RID: 13050 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060032FB RID: 13051 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x060032FC RID: 13052 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060032FD RID: 13053 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x04003FDE RID: 16350
	public Transform playerTransform;

	// Token: 0x04003FDF RID: 16351
	private NavMeshAgent agent;

	// Token: 0x04003FE0 RID: 16352
	private Rigidbody r;

	// Token: 0x04003FE1 RID: 16353
	private Vector3 targetPosition;

	// Token: 0x04003FE2 RID: 16354
	private Vector3 targetRotation;

	// Token: 0x04003FE3 RID: 16355
	public float lerpValue;
}
