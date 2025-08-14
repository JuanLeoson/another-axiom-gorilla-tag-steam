using System;
using UnityEngine;

// Token: 0x020004F1 RID: 1265
public class GameBall : MonoBehaviour
{
	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001EAF RID: 7855 RVA: 0x000A206E File Offset: 0x000A026E
	public bool IsLaunched
	{
		get
		{
			return this._launched;
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000A2078 File Offset: 0x000A0278
	private void Awake()
	{
		this.id = GameBallId.Invalid;
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.lastHeldByTeamId = -1;
		this.onlyGrabTeamId = -1;
		this._monkeBall = base.GetComponent<MonkeBall>();
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000A210C File Offset: 0x000A030C
	private void FixedUpdate()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this._launched)
		{
			this._launchedTimer += Time.fixedDeltaTime;
			if (this.collider.isTrigger && this._launchedTimer > 1f && this.rigidBody.velocity.y <= 0f)
			{
				this._launched = false;
				this.collider.isTrigger = false;
			}
		}
		Vector3 force = -Physics.gravity * (1f - this.gravityMult);
		this.rigidBody.AddForce(force, ForceMode.Acceleration);
		this._catchSoundDecay -= Time.deltaTime;
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000A21C1 File Offset: 0x000A03C1
	public void WasLaunched()
	{
		this._launched = true;
		this.collider.isTrigger = true;
		this._launchedTimer = 0f;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000A21E1 File Offset: 0x000A03E1
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000A2202 File Offset: 0x000A0402
	public void SetVelocity(Vector3 velocity)
	{
		this.rigidBody.velocity = velocity;
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000A2210 File Offset: 0x000A0410
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this._catchSoundDecay <= 0f)
		{
			this.audioSource.clip = this.catchSound;
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.Play();
			this._catchSoundDecay = 0.1f;
		}
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000A2270 File Offset: 0x000A0470
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.throwSound;
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000A22AD File Offset: 0x000A04AD
	public void PlayBounceFX()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.groundSound;
			this.audioSource.volume = this.groundSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000A22EA File Offset: 0x000A04EA
	public void SetHeldByTeamId(int teamId)
	{
		this.lastHeldByTeamId = teamId;
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000A22F3 File Offset: 0x000A04F3
	private bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000A2302 File Offset: 0x000A0502
	public void SetVisualOffset(bool detach)
	{
		if (this._monkeBall != null)
		{
			this._monkeBall.SetVisualOffset(detach);
		}
	}

	// Token: 0x0400275A RID: 10074
	public GameBallId id;

	// Token: 0x0400275B RID: 10075
	public float gravityMult = 1f;

	// Token: 0x0400275C RID: 10076
	public bool disc;

	// Token: 0x0400275D RID: 10077
	public Vector3 localDiscUp;

	// Token: 0x0400275E RID: 10078
	public AudioSource audioSource;

	// Token: 0x0400275F RID: 10079
	public AudioClip catchSound;

	// Token: 0x04002760 RID: 10080
	public float catchSoundVolume;

	// Token: 0x04002761 RID: 10081
	private float _catchSoundDecay;

	// Token: 0x04002762 RID: 10082
	public AudioClip throwSound;

	// Token: 0x04002763 RID: 10083
	public float throwSoundVolume;

	// Token: 0x04002764 RID: 10084
	public AudioClip groundSound;

	// Token: 0x04002765 RID: 10085
	public float groundSoundVolume;

	// Token: 0x04002766 RID: 10086
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04002767 RID: 10087
	[SerializeField]
	private Collider collider;

	// Token: 0x04002768 RID: 10088
	public int heldByActorNumber;

	// Token: 0x04002769 RID: 10089
	public int lastHeldByActorNumber;

	// Token: 0x0400276A RID: 10090
	public int lastHeldByTeamId;

	// Token: 0x0400276B RID: 10091
	public int onlyGrabTeamId;

	// Token: 0x0400276C RID: 10092
	private bool _launched;

	// Token: 0x0400276D RID: 10093
	private float _launchedTimer;

	// Token: 0x0400276E RID: 10094
	public MonkeBall _monkeBall;
}
