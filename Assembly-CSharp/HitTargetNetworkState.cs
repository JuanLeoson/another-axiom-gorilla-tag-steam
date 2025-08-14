using System;
using System.Collections;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003D1 RID: 977
[NetworkBehaviourWeaved(1)]
public class HitTargetNetworkState : NetworkComponent
{
	// Token: 0x060016CA RID: 5834 RVA: 0x0007C1DC File Offset: 0x0007A3DC
	protected override void Awake()
	{
		base.Awake();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0007C23A File Offset: 0x0007A43A
	protected override void Start()
	{
		base.Start();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x0007C25D File Offset: 0x0007A45D
	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.GTStop();
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x0007C281 File Offset: 0x0007A481
	public void OnLeftRoom()
	{
		this.SetInitialState();
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0007C289 File Offset: 0x0007A489
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x0007C2B1 File Offset: 0x0007A4B1
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit(Vector3.zero, Vector3.one);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x0007C2C0 File Offset: 0x0007A4C0
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit(projectile.launchPosition, collision.contacts[0].point);
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0007C2E0 File Offset: 0x0007A4E0
	public void TargetHit(Vector3 launchPoint, Vector3 impactPoint)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (Time.time <= this.nextHittableTimestamp)
		{
			return;
		}
		int num = this.networkedScore.Value;
		if (this.scoreIsDistance)
		{
			int num2 = Mathf.RoundToInt((launchPoint - impactPoint).magnitude * 3.28f);
			if (num2 <= num)
			{
				return;
			}
			num = num2;
		}
		else
		{
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
		}
		if (this.resetAfterDuration > 0f && this.resetCoroutine == null)
		{
			this.resetAtTimestamp = Time.time + this.resetAfterDuration;
			this.resetCoroutine = base.StartCoroutine(this.ResetCo());
		}
		this.PlayAudio(this.networkedScore.Value, num);
		this.networkedScore.Value = num;
		this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060016D2 RID: 5842 RVA: 0x0007C3B8 File Offset: 0x0007A5B8
	// (set) Token: 0x060016D3 RID: 5843 RVA: 0x0007C3DE File Offset: 0x0007A5DE
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe int Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x0007C405 File Offset: 0x0007A605
	public override void WriteDataFusion()
	{
		this.Data = this.networkedScore.Value;
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x0007C418 File Offset: 0x0007A618
	public override void ReadDataFusion()
	{
		int data = this.Data;
		if (data != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, data);
		}
		this.networkedScore.Value = data;
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x0007C458 File Offset: 0x0007A658
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.networkedScore.Value);
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0007C480 File Offset: 0x0007A680
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0007C4D3 File Offset: 0x0007A6D3
	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore && !this.scoreIsDistance)
		{
			this.audioPlayer.GTPlayOneShot(this.audioClips[1], 1f);
			return;
		}
		this.audioPlayer.GTPlayOneShot(this.audioClips[0], 1f);
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x0007C512 File Offset: 0x0007A712
	private IEnumerator ResetCo()
	{
		while (Time.time < this.resetAtTimestamp)
		{
			yield return new WaitForSeconds(this.resetAtTimestamp - Time.time);
		}
		this.networkedScore.Value = 0;
		this.PlayAudio(this.networkedScore.Value, 0);
		this.resetCoroutine = null;
		yield break;
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0007C530 File Offset: 0x0007A730
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0007C548 File Offset: 0x0007A748
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04001EA6 RID: 7846
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04001EA7 RID: 7847
	[SerializeField]
	private int hitCooldownTime = 1;

	// Token: 0x04001EA8 RID: 7848
	[SerializeField]
	private bool testPress;

	// Token: 0x04001EA9 RID: 7849
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04001EAA RID: 7850
	[SerializeField]
	private bool scoreIsDistance;

	// Token: 0x04001EAB RID: 7851
	[SerializeField]
	private float resetAfterDuration;

	// Token: 0x04001EAC RID: 7852
	private AudioSource audioPlayer;

	// Token: 0x04001EAD RID: 7853
	private float nextHittableTimestamp;

	// Token: 0x04001EAE RID: 7854
	private float resetAtTimestamp;

	// Token: 0x04001EAF RID: 7855
	private Coroutine resetCoroutine;

	// Token: 0x04001EB0 RID: 7856
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _Data;
}
