using System;
using System.Collections;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000604 RID: 1540
public class GRBadge : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060025D6 RID: 9686 RVA: 0x000CAB40 File Offset: 0x000C8D40
	public void OnEntityInit()
	{
		this.gameEntity.manager.ghostReactorManager.reactor.employeeBadges.LinkBadgeToDispenser(this, (long)((int)this.gameEntity.createData));
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x000CAB70 File Offset: 0x000C8D70
	private void OnDestroy()
	{
		GhostReactor ghostReactor = GhostReactor.Get(this.gameEntity);
		if (ghostReactor != null && ghostReactor.employeeBadges != null)
		{
			ghostReactor.employeeBadges.RemoveBadge(this);
		}
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000CABAC File Offset: 0x000C8DAC
	public void Setup(NetPlayer player, int index)
	{
		this.gameEntity.onlyGrabActorNumber = player.ActorNumber;
		this.dispenserIndex = index;
		this.actorNr = player.ActorNumber;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && (int)this.gameEntity.GetState() == 1)
		{
			base.transform.position = grplayer.badgeBodyAnchor.position;
			grplayer.AttachBadge(this);
		}
		this.RefreshText(player);
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000CAC28 File Offset: 0x000C8E28
	public void RefreshText(NetPlayer player)
	{
		this.playerName.text = player.SanitizedNickName;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && this.lastRedeemedPoints != grplayer.CurrentProgression.redeemedPoints)
		{
			this.lastRedeemedPoints = grplayer.CurrentProgression.redeemedPoints;
			this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			this.playerLevel.text = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints).ToString();
		}
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000CACC0 File Offset: 0x000C8EC0
	public void Hide()
	{
		this.badgeMesh.enabled = false;
		this.playerName.gameObject.SetActive(false);
		this.playerTitle.gameObject.SetActive(false);
		this.playerLevel.gameObject.SetActive(false);
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000CAD0C File Offset: 0x000C8F0C
	public void UnHide()
	{
		this.badgeMesh.enabled = true;
		this.playerName.gameObject.SetActive(true);
		this.playerTitle.gameObject.SetActive(true);
		this.playerLevel.gameObject.SetActive(true);
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000CAD58 File Offset: 0x000C8F58
	public void StartRetracting()
	{
		this.gameEntity.RequestState(this.gameEntity.id, 1L);
		this.PlayAttachFx();
		if (this.retractCoroutine != null)
		{
			base.StopCoroutine(this.retractCoroutine);
		}
		this.retractCoroutine = base.StartCoroutine(this.RetractCoroutine());
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000CADA9 File Offset: 0x000C8FA9
	private IEnumerator RetractCoroutine()
	{
		base.transform.localRotation = Quaternion.identity;
		Vector3 vector = base.transform.localPosition;
		for (float sqrMagnitude = vector.sqrMagnitude; sqrMagnitude > 1E-05f; sqrMagnitude = vector.sqrMagnitude)
		{
			vector = Vector3.MoveTowards(vector, Vector3.zero, this.retractSpeed * Time.deltaTime);
			base.transform.localPosition = vector;
			yield return null;
			vector = base.transform.localPosition;
		}
		base.transform.localPosition = Vector3.zero;
		yield break;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000CADB8 File Offset: 0x000C8FB8
	private void PlayAttachFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.badgeAttachSoundVolume;
			this.audioSource.clip = this.badgeAttachSound;
			this.audioSource.Play();
		}
	}

	// Token: 0x04002FF9 RID: 12281
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04002FFA RID: 12282
	[SerializeField]
	public TMP_Text playerName;

	// Token: 0x04002FFB RID: 12283
	[SerializeField]
	public TMP_Text playerTitle;

	// Token: 0x04002FFC RID: 12284
	[SerializeField]
	public TMP_Text playerLevel;

	// Token: 0x04002FFD RID: 12285
	[SerializeField]
	private MeshRenderer badgeMesh;

	// Token: 0x04002FFE RID: 12286
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002FFF RID: 12287
	[SerializeField]
	private float retractSpeed = 4f;

	// Token: 0x04003000 RID: 12288
	[SerializeField]
	private AudioClip badgeAttachSound;

	// Token: 0x04003001 RID: 12289
	[SerializeField]
	private float badgeAttachSoundVolume;

	// Token: 0x04003002 RID: 12290
	[SerializeField]
	public int dispenserIndex;

	// Token: 0x04003003 RID: 12291
	public int actorNr;

	// Token: 0x04003004 RID: 12292
	private Coroutine retractCoroutine;

	// Token: 0x04003005 RID: 12293
	private int lastRedeemedPoints = -1;

	// Token: 0x02000605 RID: 1541
	public enum BadgeState
	{
		// Token: 0x04003007 RID: 12295
		AtDispenser,
		// Token: 0x04003008 RID: 12296
		WithPlayer
	}
}
