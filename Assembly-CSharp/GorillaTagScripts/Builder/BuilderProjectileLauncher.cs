using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C97 RID: 3223
	public class BuilderProjectileLauncher : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent
	{
		// Token: 0x06004FEE RID: 20462 RVA: 0x0018E6CC File Offset: 0x0018C8CC
		private void LaunchProjectile(int timeStamp)
		{
			if (Time.time > this.lastFireTime + this.fireCooldown)
			{
				this.lastFireTime = Time.time;
				int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				try
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
					this.projectileScale = this.myPiece.GetScale();
					gameObject.transform.localScale = Vector3.one * this.projectileScale;
					BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
					int num = HashCode.Combine<int, int>(this.myPiece.pieceId, timeStamp);
					if (this.allProjectiles.ContainsKey(num))
					{
						this.allProjectiles.Remove(num);
					}
					this.allProjectiles.Add(num, component);
					SlingshotProjectile.AOEKnockbackConfig value = new SlingshotProjectile.AOEKnockbackConfig
					{
						aeoOuterRadius = this.knockbackConfig.aeoOuterRadius * this.projectileScale,
						aeoInnerRadius = this.knockbackConfig.aeoInnerRadius * this.projectileScale,
						applyAOEKnockback = this.knockbackConfig.applyAOEKnockback,
						impactVelocityThreshold = this.knockbackConfig.impactVelocityThreshold * this.projectileScale,
						knockbackVelocity = this.knockbackConfig.knockbackVelocity * this.projectileScale,
						playerProximityEffect = this.knockbackConfig.playerProximityEffect
					};
					component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(value);
					component.gravityMultiplier = this.gravityMultiplier;
					component.Launch(this.launchPosition.position, this.launchVelocity * this.projectileScale * this.launchPosition.up, this, num, this.projectileScale, timeStamp);
					if (this.launchSound != null && this.launchSound.clip != null)
					{
						this.launchSound.Play();
					}
				}
				catch (Exception value2)
				{
					Console.WriteLine(value2);
					throw;
				}
			}
		}

		// Token: 0x06004FEF RID: 20463 RVA: 0x0018E8B4 File Offset: 0x0018CAB4
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if ((BuilderProjectileLauncher.FunctionalState)newState == this.currentState)
			{
				return;
			}
			this.currentState = (BuilderProjectileLauncher.FunctionalState)newState;
			if (newState == 1)
			{
				this.LaunchProjectile(timeStamp);
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004FF0 RID: 20464 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x06004FF1 RID: 20465 RVA: 0x0018DAA7 File Offset: 0x0018BCA7
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06004FF2 RID: 20466 RVA: 0x0018E908 File Offset: 0x0018CB08
		public void FunctionalPieceUpdate()
		{
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].UpdateProjectile();
			}
			if (PhotonNetwork.IsMasterClient && this.lastFireTime + this.fireCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004FF3 RID: 20467 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06004FF4 RID: 20468 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004FF5 RID: 20469 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x0018E989 File Offset: 0x0018CB89
		public void OnPieceActivate()
		{
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06004FF7 RID: 20471 RVA: 0x0018E99C File Offset: 0x0018CB9C
		public void OnPieceDeactivate()
		{
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].Deactivate();
			}
		}

		// Token: 0x06004FF8 RID: 20472 RVA: 0x0018E9E3 File Offset: 0x0018CBE3
		public void RegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Add(projectile);
		}

		// Token: 0x06004FF9 RID: 20473 RVA: 0x0018E9F1 File Offset: 0x0018CBF1
		public void UnRegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Remove(projectile);
			this.allProjectiles.Remove(projectile.projectileId);
		}

		// Token: 0x040058FE RID: 22782
		private List<BuilderProjectile> launchedProjectiles = new List<BuilderProjectile>();

		// Token: 0x040058FF RID: 22783
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04005900 RID: 22784
		[SerializeField]
		protected float fireCooldown = 2f;

		// Token: 0x04005901 RID: 22785
		[Tooltip("launch in Y direction")]
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x04005902 RID: 22786
		[SerializeField]
		private float launchVelocity;

		// Token: 0x04005903 RID: 22787
		[SerializeField]
		private AudioSource launchSound;

		// Token: 0x04005904 RID: 22788
		[SerializeField]
		protected GameObject projectilePrefab;

		// Token: 0x04005905 RID: 22789
		protected float projectileScale = 0.06f;

		// Token: 0x04005906 RID: 22790
		[SerializeField]
		protected float gravityMultiplier = 1f;

		// Token: 0x04005907 RID: 22791
		public SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

		// Token: 0x04005908 RID: 22792
		private float lastFireTime;

		// Token: 0x04005909 RID: 22793
		private BuilderProjectileLauncher.FunctionalState currentState;

		// Token: 0x0400590A RID: 22794
		private Dictionary<int, BuilderProjectile> allProjectiles = new Dictionary<int, BuilderProjectile>();

		// Token: 0x02000C98 RID: 3224
		private enum FunctionalState
		{
			// Token: 0x0400590C RID: 22796
			Idle,
			// Token: 0x0400590D RID: 22797
			Fire
		}
	}
}
