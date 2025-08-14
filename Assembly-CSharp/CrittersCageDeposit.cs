using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class CrittersCageDeposit : CrittersActorDeposit
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600017F RID: 383 RVA: 0x00009B24 File Offset: 0x00007D24
	// (remove) Token: 0x06000180 RID: 384 RVA: 0x00009B5C File Offset: 0x00007D5C
	public event Action<Menagerie.CritterData, int> OnDepositCritter;

	// Token: 0x06000181 RID: 385 RVA: 0x00009B91 File Offset: 0x00007D91
	private void Awake()
	{
		this.attachPoint.OnGrabbedChild += this.StartProcessCage;
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00009BAA File Offset: 0x00007DAA
	protected override bool CanDeposit(CrittersActor depositActor)
	{
		return base.CanDeposit(depositActor) && !this.isHandlingDeposit;
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00009BC0 File Offset: 0x00007DC0
	private void StartProcessCage(CrittersActor depositedActor)
	{
		this.currentCage = depositedActor;
		base.StartCoroutine(this.ProcessCage());
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00009BD6 File Offset: 0x00007DD6
	private IEnumerator ProcessCage()
	{
		this.isHandlingDeposit = true;
		bool isLocalDeposit = this.currentCage.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
		this.depositAudio.GTPlayOneShot(this.depositStartSound, isLocalDeposit ? 1f : 0.5f);
		float transition = 0f;
		CrittersPawn crittersPawn = this.currentCage.GetComponentInChildren<CrittersPawn>();
		int lastGrabbedPlayer = this.currentCage.lastGrabbedPlayer;
		Menagerie.CritterData critterData;
		if (crittersPawn.IsNotNull())
		{
			critterData = new Menagerie.CritterData(crittersPawn.visuals);
		}
		else
		{
			critterData = new Menagerie.CritterData();
		}
		while (transition < this.submitDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositStartLocation, this.depositEndLocation, Mathf.Min(transition / this.submitDuration, 1f));
			yield return null;
		}
		if (crittersPawn.IsNotNull())
		{
			Action<Menagerie.CritterData, int> onDepositCritter = this.OnDepositCritter;
			if (onDepositCritter != null)
			{
				onDepositCritter(critterData, lastGrabbedPlayer);
			}
			CrittersActor crittersActor = crittersPawn;
			bool keepWorldPosition = false;
			Vector3 zero = Vector3.zero;
			crittersActor.Released(keepWorldPosition, default(Quaternion), zero, default(Vector3), default(Vector3));
			crittersPawn.gameObject.SetActive(false);
			this.depositAudio.GTPlayOneShot(this.depositCritterSound, isLocalDeposit ? 1f : 0.5f);
		}
		else
		{
			this.depositAudio.GTPlayOneShot(this.depositEmptySound, isLocalDeposit ? 1f : 0.5f);
		}
		this.currentCage.transform.position = Vector3.zero;
		this.currentCage.gameObject.SetActive(false);
		this.currentCage = null;
		transition = 0f;
		while (transition < this.returnDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositEndLocation, this.depositStartLocation, Mathf.Min(transition / this.returnDuration, 1f));
			yield return null;
		}
		this.isHandlingDeposit = false;
		yield break;
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00009BE8 File Offset: 0x00007DE8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositStartLocation), 0.1f);
		Gizmos.DrawLine(base.transform.TransformPoint(this.depositStartLocation), base.transform.TransformPoint(this.depositEndLocation));
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositEndLocation), 0.1f);
	}

	// Token: 0x040001BC RID: 444
	private bool isHandlingDeposit;

	// Token: 0x040001BD RID: 445
	public Vector3 depositStartLocation;

	// Token: 0x040001BE RID: 446
	public Vector3 depositEndLocation;

	// Token: 0x040001BF RID: 447
	public float submitDuration = 0.5f;

	// Token: 0x040001C0 RID: 448
	public float returnDuration = 1f;

	// Token: 0x040001C1 RID: 449
	public AudioSource depositAudio;

	// Token: 0x040001C2 RID: 450
	public AudioClip depositStartSound;

	// Token: 0x040001C3 RID: 451
	public AudioClip depositEmptySound;

	// Token: 0x040001C4 RID: 452
	public AudioClip depositCritterSound;

	// Token: 0x040001C5 RID: 453
	private CrittersActor currentCage;
}
