using System;
using TMPro;
using UnityEngine;

// Token: 0x02000690 RID: 1680
public class GRUIEmployeeBadgeDispenser : MonoBehaviour
{
	// Token: 0x06002922 RID: 10530 RVA: 0x000DD61C File Offset: 0x000DB81C
	public void Refresh()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.actorNr);
		if (player != null && player.InRoom)
		{
			this.playerName.text = player.SanitizedNickName;
			if (this.idBadge != null)
			{
				this.idBadge.RefreshText(player);
				return;
			}
		}
		else
		{
			this.playerName.text = "";
		}
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000DD684 File Offset: 0x000DB884
	public void CreateBadge(NetPlayer player, GameEntityManager entityManager)
	{
		if (entityManager.IsAuthority())
		{
			entityManager.RequestCreateItem(this.idBadgePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)(player.ActorNumber * 100 + this.index));
		}
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000DD6D7 File Offset: 0x000DB8D7
	public Transform GetSpawnMarker()
	{
		return this.spawnLocation;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000DD6DF File Offset: 0x000DB8DF
	public bool IsDispenserForBadge(GRBadge badge)
	{
		return badge == this.idBadge;
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000DD6ED File Offset: 0x000DB8ED
	public Vector3 GetSpawnPosition()
	{
		return this.spawnLocation.position;
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000DD6FA File Offset: 0x000DB8FA
	public Quaternion GetSpawnRotation()
	{
		return this.spawnLocation.rotation;
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000DD707 File Offset: 0x000DB907
	public void ClearBadge()
	{
		this.actorNr = -1;
		this.idBadge = null;
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000DD718 File Offset: 0x000DB918
	public void AttachIDBadge(GRBadge linkedBadge, NetPlayer _player)
	{
		this.actorNr = ((_player == null) ? -1 : _player.ActorNumber);
		this.idBadge = linkedBadge;
		this.playerName.text = ((_player == null) ? null : _player.SanitizedNickName);
		this.idBadge.Setup(_player, this.index);
	}

	// Token: 0x04003510 RID: 13584
	[SerializeField]
	private TMP_Text msg;

	// Token: 0x04003511 RID: 13585
	[SerializeField]
	private TMP_Text playerName;

	// Token: 0x04003512 RID: 13586
	[SerializeField]
	private Transform spawnLocation;

	// Token: 0x04003513 RID: 13587
	[SerializeField]
	private GameEntity idBadgePrefab;

	// Token: 0x04003514 RID: 13588
	[SerializeField]
	private LayerMask badgeLayerMask;

	// Token: 0x04003515 RID: 13589
	public int index;

	// Token: 0x04003516 RID: 13590
	public int actorNr;

	// Token: 0x04003517 RID: 13591
	public GRBadge idBadge;

	// Token: 0x04003518 RID: 13592
	private Coroutine getSpawnedBadgeCoroutine;

	// Token: 0x04003519 RID: 13593
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x0400351A RID: 13594
	private bool isEmployee;

	// Token: 0x0400351B RID: 13595
	private const string GR_DATA_KEY = "GRData";
}
