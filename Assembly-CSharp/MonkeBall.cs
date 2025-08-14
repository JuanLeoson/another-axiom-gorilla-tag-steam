using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class MonkeBall : MonoBehaviour
{
	// Token: 0x06001F0C RID: 7948 RVA: 0x000A40E8 File Offset: 0x000A22E8
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000A40F0 File Offset: 0x000A22F0
	private void Update()
	{
		this.UpdateVisualOffset();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._resyncPosition)
			{
				this._resyncDelay -= Time.deltaTime;
				if (this._resyncDelay <= 0f)
				{
					this._resyncPosition = false;
					GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
				}
			}
			if (this._positionFailsafe)
			{
				if (base.transform.position.y < -500f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					if (PhotonNetwork.IsConnected)
					{
						GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
					}
					else
					{
						base.transform.position = GameBallManager.Instance.transform.position;
					}
					this._positionFailsafe = false;
					this._positionFailsafeTimer = 3f;
					return;
				}
			}
			else
			{
				this._positionFailsafeTimer -= Time.deltaTime;
				if (this._positionFailsafeTimer <= 0f)
				{
					this._positionFailsafe = true;
				}
			}
			return;
		}
		if (this.gameBall.onlyGrabTeamId != -1 && Time.timeAsDouble >= this.restrictTeamGrabEndTime)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, -1);
		}
		if (this.AlreadyDropped())
		{
			this._droppedTimer += Time.deltaTime;
			if (this._droppedTimer >= 7.5f)
			{
				this._droppedTimer = 0f;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.velocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._justGrabbed)
		{
			this._justGrabbedTimer -= Time.deltaTime;
			if (this._justGrabbedTimer <= 0f)
			{
				this._justGrabbed = false;
			}
		}
		if (this._resyncPosition)
		{
			this._resyncDelay -= Time.deltaTime;
			if (this._resyncDelay <= 0f)
			{
				this._resyncPosition = false;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.velocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._positionFailsafe)
		{
			if (base.transform.position.y < -250f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
			{
				MonkeBallGame.Instance.LaunchBallNeutral(this.gameBall.id);
				this._positionFailsafe = false;
				this._positionFailsafeTimer = 3f;
				return;
			}
		}
		else
		{
			this._positionFailsafeTimer -= Time.deltaTime;
			if (this._positionFailsafeTimer <= 0f)
			{
				this._positionFailsafe = true;
			}
		}
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x000A43FC File Offset: 0x000A25FC
	public void OnCollisionEnter(Collision collision)
	{
		if (this.AlreadyDropped() || this._justGrabbed)
		{
			return;
		}
		if (MonkeBall.IsGamePlayer(collision.collider))
		{
			return;
		}
		this.alreadyDropped = true;
		this._droppedTimer = 0f;
		this.gameBall.PlayBounceFX();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._rigidBody.velocity.sqrMagnitude > 1f)
			{
				this._resyncPosition = true;
				this._resyncDelay = 1.5f;
			}
			int lastHeldByActorNumber = this.gameBall.lastHeldByActorNumber;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			return;
		}
		if (this._rigidBody.velocity.sqrMagnitude > 1f)
		{
			this._resyncPosition = true;
			this._resyncDelay = 0.5f;
		}
		if (this._launchAfterScore)
		{
			this._launchAfterScore = false;
			MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
			return;
		}
		MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x000A4523 File Offset: 0x000A2723
	public void TriggerDelayedResync()
	{
		this._resyncPosition = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._resyncDelay = 0.5f;
			return;
		}
		this._resyncDelay = 1.5f;
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000A454A File Offset: 0x000A274A
	public void SetRigidbodyDiscrete()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000A4558 File Offset: 0x000A2758
	public void SetRigidbodyContinuous()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000A4566 File Offset: 0x000A2766
	public static MonkeBall Get(GameBall ball)
	{
		if (ball == null)
		{
			return null;
		}
		return ball.GetComponent<MonkeBall>();
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000A4579 File Offset: 0x000A2779
	public bool AlreadyDropped()
	{
		return this.alreadyDropped;
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000A4581 File Offset: 0x000A2781
	public void OnGrabbed()
	{
		this.alreadyDropped = false;
		this._justGrabbed = true;
		this._justGrabbedTimer = 0.1f;
		this._resyncPosition = false;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000A45A3 File Offset: 0x000A27A3
	public void OnSwitchHeldByTeam(int teamId)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, teamId);
		}
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000A45C2 File Offset: 0x000A27C2
	public void ClearCannotGrabTeamId()
	{
		this.gameBall.onlyGrabTeamId = -1;
		this.restrictTeamGrabEndTime = -1.0;
		this.Refresh();
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000A45E8 File Offset: 0x000A27E8
	public bool RestrictBallToTeam(int teamId, float duration)
	{
		if (teamId == this.gameBall.onlyGrabTeamId && Time.timeAsDouble + (double)duration < this.restrictTeamGrabEndTime)
		{
			return false;
		}
		this.gameBall.onlyGrabTeamId = teamId;
		this.restrictTeamGrabEndTime = Time.timeAsDouble + (double)duration;
		this.Refresh();
		return true;
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000A4636 File Offset: 0x000A2836
	private void Refresh()
	{
		if (this.gameBall.onlyGrabTeamId == -1)
		{
			this.mainRenderer.material = this.defaultMaterial;
			return;
		}
		this.mainRenderer.material = this.teamMaterial[this.gameBall.onlyGrabTeamId];
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000A4675 File Offset: 0x000A2875
	private static bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000A4684 File Offset: 0x000A2884
	public void SetVisualOffset(bool detach)
	{
		if (detach)
		{
			this.lastVisiblePosition = this.mainRenderer.transform.position;
			this._visualOffset = true;
			this._timeOffset = Time.time;
			this.mainRenderer.transform.SetParent(null, true);
			return;
		}
		this.ReattachVisuals();
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000A46D8 File Offset: 0x000A28D8
	private void ReattachVisuals()
	{
		if (!this._visualOffset)
		{
			return;
		}
		this.mainRenderer.transform.SetParent(base.transform);
		this.mainRenderer.transform.localPosition = Vector3.zero;
		this.mainRenderer.transform.localRotation = Quaternion.identity;
		this._visualOffset = false;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000A4738 File Offset: 0x000A2938
	private void UpdateVisualOffset()
	{
		if (this._visualOffset)
		{
			this.mainRenderer.transform.position = Vector3.Lerp(this.mainRenderer.transform.position, this._rigidBody.position, Mathf.Clamp((Time.time - this._timeOffset) / this.maxLerpTime, this.offsetLerp, 1f));
			if ((this.mainRenderer.transform.position - this._rigidBody.position).sqrMagnitude < this._offsetThreshold)
			{
				this.ReattachVisuals();
			}
		}
	}

	// Token: 0x040027A5 RID: 10149
	public GameBall gameBall;

	// Token: 0x040027A6 RID: 10150
	public MeshRenderer mainRenderer;

	// Token: 0x040027A7 RID: 10151
	public Material defaultMaterial;

	// Token: 0x040027A8 RID: 10152
	public Material[] teamMaterial;

	// Token: 0x040027A9 RID: 10153
	public double restrictTeamGrabEndTime;

	// Token: 0x040027AA RID: 10154
	public bool alreadyDropped;

	// Token: 0x040027AB RID: 10155
	private bool _justGrabbed;

	// Token: 0x040027AC RID: 10156
	private float _justGrabbedTimer;

	// Token: 0x040027AD RID: 10157
	private bool _launchAfterScore;

	// Token: 0x040027AE RID: 10158
	private float _droppedTimer;

	// Token: 0x040027AF RID: 10159
	private bool _resyncPosition;

	// Token: 0x040027B0 RID: 10160
	private float _resyncDelay;

	// Token: 0x040027B1 RID: 10161
	private bool _visualOffset;

	// Token: 0x040027B2 RID: 10162
	private float _offsetThreshold = 0.05f;

	// Token: 0x040027B3 RID: 10163
	private float _timeOffset;

	// Token: 0x040027B4 RID: 10164
	public float maxLerpTime = 0.5f;

	// Token: 0x040027B5 RID: 10165
	public float offsetLerp = 0.2f;

	// Token: 0x040027B6 RID: 10166
	private bool _positionFailsafe = true;

	// Token: 0x040027B7 RID: 10167
	private float _positionFailsafeTimer;

	// Token: 0x040027B8 RID: 10168
	public Vector3 lastVisiblePosition;

	// Token: 0x040027B9 RID: 10169
	[SerializeField]
	private Rigidbody _rigidBody;
}
