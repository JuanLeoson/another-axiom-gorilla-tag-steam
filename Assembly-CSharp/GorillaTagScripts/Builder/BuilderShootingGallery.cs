using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C9E RID: 3230
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06005024 RID: 20516 RVA: 0x0018F9DC File Offset: 0x0018DBDC
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x06005025 RID: 20517 RVA: 0x0018FA60 File Offset: 0x0018DC60
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x06005026 RID: 20518 RVA: 0x0018FA90 File Offset: 0x0018DC90
		private void OnWheelHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06005027 RID: 20519 RVA: 0x0018FB00 File Offset: 0x0018DD00
		private void OnCowboyHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 2);
			}
		}

		// Token: 0x06005028 RID: 20520 RVA: 0x0018FB70 File Offset: 0x0018DD70
		private void CowboyHitEffects()
		{
			if (this.cowboyHitSound != null)
			{
				this.cowboyHitSound.Play();
			}
			if (this.cowboyHitAnimation != null && this.cowboyHitAnimation.clip != null)
			{
				this.cowboyHitAnimation.Play();
			}
		}

		// Token: 0x06005029 RID: 20521 RVA: 0x0018FBC4 File Offset: 0x0018DDC4
		private void WheelHitEffects()
		{
			if (this.wheelHitSound != null)
			{
				this.wheelHitSound.Play();
			}
			if (this.wheelHitAnimation != null && this.wheelHitAnimation.clip != null)
			{
				this.wheelHitAnimation.Play();
			}
		}

		// Token: 0x0600502A RID: 20522 RVA: 0x0018FC18 File Offset: 0x0018DE18
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderShootingGallery.FunctionalState.Idle;
			this.cowboyInitLocalPos = this.cowboyTransform.transform.localPosition;
			this.cowboyInitLocalRotation = this.cowboyTransform.transform.localRotation;
			this.wheelInitLocalRot = this.wheelTransform.transform.localRotation;
			this.distance = Vector3.Distance(this.cowboyStart.position, this.cowboyEnd.position);
			this.cowboyCycleDuration = this.distance / (this.cowboyVelocity * this.myPiece.GetScale());
			this.wheelCycleDuration = 1f / this.wheelVelocity;
		}

		// Token: 0x0600502B RID: 20523 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600502C RID: 20524 RVA: 0x0018FCC0 File Offset: 0x0018DEC0
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x0600502D RID: 20525 RVA: 0x0018FCF0 File Offset: 0x0018DEF0
		public void OnPieceActivate()
		{
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
			if (!this.activated)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x0600502E RID: 20526 RVA: 0x0018FD50 File Offset: 0x0018DF50
		public void OnPieceDeactivate()
		{
			if (this.currentState != BuilderShootingGallery.FunctionalState.Idle)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			if (this.activated)
			{
				this.myPiece.GetTable().UnregisterFunctionalPieceFixedUpdate(this);
				this.activated = false;
			}
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
		}

		// Token: 0x0600502F RID: 20527 RVA: 0x0018FDEC File Offset: 0x0018DFEC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (instigator == null)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.WheelHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			else if (newState == 2 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.CowboyHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderShootingGallery.FunctionalState)newState;
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x0018FE70 File Offset: 0x0018E070
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06005031 RID: 20529 RVA: 0x0018FED5 File Offset: 0x0018E0D5
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x06005032 RID: 20530 RVA: 0x0018FEE0 File Offset: 0x0018E0E0
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06005033 RID: 20531 RVA: 0x0018FF34 File Offset: 0x0018E134
		public void FunctionalPieceFixedUpdate()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			this.currT = this.CowboyCycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			float time = this.currForward ? this.currT : (1f - this.currT);
			float num = this.WheelCycleCompletionPercent();
			float t = this.cowboyCurve.Evaluate(time);
			this.cowboyTransform.localPosition = Vector3.Lerp(this.cowboyStart.localPosition, this.cowboyEnd.localPosition, t);
			Quaternion localRotation = Quaternion.AngleAxis(num * 360f, Vector3.right);
			this.wheelTransform.localRotation = localRotation;
		}

		// Token: 0x06005034 RID: 20532 RVA: 0x0018FFDB File Offset: 0x0018E1DB
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06005035 RID: 20533 RVA: 0x0018FFFD File Offset: 0x0018E1FD
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x06005036 RID: 20534 RVA: 0x0019000C File Offset: 0x0018E20C
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x06005037 RID: 20535 RVA: 0x0019001C File Offset: 0x0018E21C
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005038 RID: 20536 RVA: 0x00190048 File Offset: 0x0018E248
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005039 RID: 20537 RVA: 0x00190073 File Offset: 0x0018E273
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x0600503A RID: 20538 RVA: 0x00190083 File Offset: 0x0018E283
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x0600503B RID: 20539 RVA: 0x001900A3 File Offset: 0x0018E2A3
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x0600503C RID: 20540 RVA: 0x001900C3 File Offset: 0x0018E2C3
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x0400594F RID: 22863
		public BuilderPiece myPiece;

		// Token: 0x04005950 RID: 22864
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x04005951 RID: 22865
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x04005952 RID: 22866
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x04005953 RID: 22867
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x04005954 RID: 22868
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04005955 RID: 22869
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x04005956 RID: 22870
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x04005957 RID: 22871
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x04005958 RID: 22872
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x04005959 RID: 22873
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x0400595A RID: 22874
		private double lastHitTime;

		// Token: 0x0400595B RID: 22875
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x0400595C RID: 22876
		private bool activated;

		// Token: 0x0400595D RID: 22877
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x0400595E RID: 22878
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x0400595F RID: 22879
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x04005960 RID: 22880
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x04005961 RID: 22881
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x04005962 RID: 22882
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x04005963 RID: 22883
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x04005964 RID: 22884
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x04005965 RID: 22885
		private float cowboyCycleDuration;

		// Token: 0x04005966 RID: 22886
		private float wheelCycleDuration;

		// Token: 0x04005967 RID: 22887
		private float distance;

		// Token: 0x04005968 RID: 22888
		private float currT;

		// Token: 0x04005969 RID: 22889
		private bool currForward;

		// Token: 0x0400596A RID: 22890
		private float dtSinceServerUpdate;

		// Token: 0x0400596B RID: 22891
		private int lastServerTimeStamp;

		// Token: 0x0400596C RID: 22892
		private float rotateStartAmt;

		// Token: 0x0400596D RID: 22893
		private float rotateAmt;

		// Token: 0x02000C9F RID: 3231
		private enum FunctionalState
		{
			// Token: 0x0400596F RID: 22895
			Idle,
			// Token: 0x04005970 RID: 22896
			HitWheel,
			// Token: 0x04005971 RID: 22897
			HitCowboy
		}
	}
}
