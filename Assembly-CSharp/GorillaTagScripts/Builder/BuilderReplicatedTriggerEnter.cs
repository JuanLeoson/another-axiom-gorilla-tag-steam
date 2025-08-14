using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C9A RID: 3226
	public class BuilderReplicatedTriggerEnter : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06005006 RID: 20486 RVA: 0x0018ECD0 File Offset: 0x0018CED0
		private void Awake()
		{
			this.colliders.Clear();
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				builderSmallHandTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerEntered));
				Collider component = builderSmallHandTrigger.GetComponent<Collider>();
				if (component != null)
				{
					this.colliders.Add(component);
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				builderSmallMonkeTrigger.onPlayerEnteredTrigger += this.OnBodyTriggerEntered;
				Collider component2 = builderSmallMonkeTrigger.GetComponent<Collider>();
				if (component2 != null)
				{
					this.colliders.Add(component2);
				}
			}
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x0018ED7C File Offset: 0x0018CF7C
		private void OnDestroy()
		{
			BuilderSmallHandTrigger[] array = this.handTriggers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
			}
			BuilderSmallMonkeTrigger[] array2 = this.bodyTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].onPlayerEnteredTrigger -= this.OnBodyTriggerEntered;
			}
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x0018EDE0 File Offset: 0x0018CFE0
		private void PlayTriggerEffects(NetPlayer target)
		{
			UnityEvent onTriggered = this.OnTriggered;
			if (onTriggered != null)
			{
				onTriggered.Invoke();
			}
			if (this.animationOnTrigger != null && this.animationOnTrigger.clip != null)
			{
				this.animationOnTrigger.Rewind();
				this.animationOnTrigger.Play();
			}
			if (this.activateSoundBank != null)
			{
				this.activateSoundBank.Play();
			}
			if (target.IsLocal)
			{
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				if (rig != null)
				{
					float num = 1.5f * rig.scaleFactor;
					if ((rig.transform.position - base.transform.position).sqrMagnitude > num * num)
					{
						return;
					}
					GTPlayer.Instance.SetMaximumSlipThisFrame();
					GTPlayer.Instance.ApplyKnockback(this.knockbackDirection.forward, this.knockbackVelocity * rig.scaleFactor, false);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
				}
			}
		}

		// Token: 0x06005009 RID: 20489 RVA: 0x0018EF19 File Offset: 0x0018D119
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x0018EF44 File Offset: 0x0018D144
		private void OnBodyTriggerEntered(int playerNumber)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerNumber);
			if (player == null)
			{
				return;
			}
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, player.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x0018EFA7 File Offset: 0x0018D1A7
		private bool CanTrigger()
		{
			return this.isPieceActive && this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.Idle && Time.time > this.lastTriggerTime + this.triggerCooldown;
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x0018EFCF File Offset: 0x0018D1CF
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderReplicatedTriggerEnter.FunctionalState.Idle;
		}

		// Token: 0x0600500D RID: 20493 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600500E RID: 20494 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600500F RID: 20495 RVA: 0x0018EFD8 File Offset: 0x0018D1D8
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x0018F030 File Offset: 0x0018D230
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x06005011 RID: 20497 RVA: 0x0018F0C4 File Offset: 0x0018D2C4
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.lastTriggerTime = Time.time;
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
				this.PlayTriggerEffects(instigator);
			}
			this.currentState = (BuilderReplicatedTriggerEnter.FunctionalState)newState;
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x0018F114 File Offset: 0x0018D314
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
			if (newState == 1 && this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x0018DAA7 File Offset: 0x0018BCA7
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x0018F170 File Offset: 0x0018D370
		public void FunctionalPieceUpdate()
		{
			if (this.lastTriggerTime + this.triggerCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x04005919 RID: 22809
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x0400591A RID: 22810
		[SerializeField]
		protected float triggerCooldown = 0.5f;

		// Token: 0x0400591B RID: 22811
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x0400591C RID: 22812
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x0400591D RID: 22813
		[Tooltip("Optional Animation to play when triggered")]
		[SerializeField]
		private Animation animationOnTrigger;

		// Token: 0x0400591E RID: 22814
		[Tooltip("Optional Sound to play when triggered")]
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x0400591F RID: 22815
		[Tooltip("Knockback the triggering player?")]
		[SerializeField]
		private bool knockbackOnTriggerEnter;

		// Token: 0x04005920 RID: 22816
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x04005921 RID: 22817
		[Tooltip("uses Forward of the transform provided")]
		[SerializeField]
		private Transform knockbackDirection;

		// Token: 0x04005922 RID: 22818
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x04005923 RID: 22819
		private bool isPieceActive;

		// Token: 0x04005924 RID: 22820
		private float lastTriggerTime;

		// Token: 0x04005925 RID: 22821
		private BuilderReplicatedTriggerEnter.FunctionalState currentState;

		// Token: 0x04005926 RID: 22822
		public UnityEvent OnTriggered;

		// Token: 0x02000C9B RID: 3227
		private enum FunctionalState
		{
			// Token: 0x04005928 RID: 22824
			Idle,
			// Token: 0x04005929 RID: 22825
			TriggerEntered
		}
	}
}
