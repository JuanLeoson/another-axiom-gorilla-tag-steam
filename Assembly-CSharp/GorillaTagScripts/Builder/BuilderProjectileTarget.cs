using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C99 RID: 3225
	public class BuilderProjectileTarget : MonoBehaviour, IBuilderPieceFunctional
	{
		// Token: 0x06004FFB RID: 20475 RVA: 0x0018EA54 File Offset: 0x0018CC54
		private void Awake()
		{
			this.hitNotifier.OnProjectileHit += this.OnProjectileHit;
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
		}

		// Token: 0x06004FFC RID: 20476 RVA: 0x0018EAC0 File Offset: 0x0018CCC0
		private void OnDestroy()
		{
			this.hitNotifier.OnProjectileHit -= this.OnProjectileHit;
		}

		// Token: 0x06004FFD RID: 20477 RVA: 0x0018EAD9 File Offset: 0x0018CCD9
		private void OnDisable()
		{
			this.hitCount = 0;
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06004FFE RID: 20478 RVA: 0x0018EB0C File Offset: 0x0018CD0C
		private void OnProjectileHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 11);
			}
		}

		// Token: 0x06004FFF RID: 20479 RVA: 0x0018EB7A File Offset: 0x0018CD7A
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
			if (newState == 11)
			{
				return;
			}
			this.lastHitTime = (double)Time.time;
			this.hitCount = Mathf.Clamp((int)newState, 0, 10);
			this.PlayHitEffects();
		}

		// Token: 0x06005000 RID: 20480 RVA: 0x0018EBB4 File Offset: 0x0018CDB4
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (instigator == null)
			{
				return;
			}
			if (newState != 11)
			{
				return;
			}
			this.hitCount++;
			this.hitCount %= 11;
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)this.hitCount, instigator.GetPlayerRef(), timeStamp);
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x0018EC2D File Offset: 0x0018CE2D
		public bool IsStateValid(byte state)
		{
			return state <= 11;
		}

		// Token: 0x06005002 RID: 20482 RVA: 0x0018EC38 File Offset: 0x0018CE38
		private void PlayHitEffects()
		{
			if (this.hitSoundbank != null)
			{
				this.hitSoundbank.Play();
			}
			if (this.hitAnimation != null && this.hitAnimation.clip != null)
			{
				this.hitAnimation.Play();
			}
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06005003 RID: 20483 RVA: 0x000023F5 File Offset: 0x000005F5
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x06005004 RID: 20484 RVA: 0x0018ECB4 File Offset: 0x0018CEB4
		public float GetInteractionDistace()
		{
			return 20f;
		}

		// Token: 0x0400590E RID: 22798
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400590F RID: 22799
		[SerializeField]
		private SlingshotProjectileHitNotifier hitNotifier;

		// Token: 0x04005910 RID: 22800
		[SerializeField]
		protected float hitCooldown = 2f;

		// Token: 0x04005911 RID: 22801
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected SoundBankPlayer hitSoundbank;

		// Token: 0x04005912 RID: 22802
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected Animation hitAnimation;

		// Token: 0x04005913 RID: 22803
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04005914 RID: 22804
		[SerializeField]
		private TMP_Text scoreText;

		// Token: 0x04005915 RID: 22805
		private double lastHitTime;

		// Token: 0x04005916 RID: 22806
		private int hitCount;

		// Token: 0x04005917 RID: 22807
		private const byte MAX_SCORE = 10;

		// Token: 0x04005918 RID: 22808
		private const byte HIT = 11;
	}
}
