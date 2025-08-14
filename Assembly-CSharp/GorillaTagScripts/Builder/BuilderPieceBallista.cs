using System;
using System.Collections;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C89 RID: 3209
	public class BuilderPieceBallista : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004F69 RID: 20329 RVA: 0x0018AE20 File Offset: 0x00189020
		private void Awake()
		{
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerPressed));
			}
			this.hasLaunchParticles = (this.launchParticles != null);
		}

		// Token: 0x06004F6A RID: 20330 RVA: 0x0018AEE6 File Offset: 0x001890E6
		private void OnDestroy()
		{
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerPressed));
			}
		}

		// Token: 0x06004F6B RID: 20331 RVA: 0x0018AF12 File Offset: 0x00189112
		private void OnHandTriggerPressed()
		{
			if (this.autoLaunch)
			{
				return;
			}
			if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 4);
			}
		}

		// Token: 0x06004F6C RID: 20332 RVA: 0x0018AF48 File Offset: 0x00189148
		private void UpdateStateMaster()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			switch (this.ballistaState)
			{
			case BuilderPieceBallista.BallistaState.Idle:
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			case BuilderPieceBallista.BallistaState.Loading:
				if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash && (double)Time.time > this.loadCompleteTime)
				{
					if (this.playerInTrigger && this.playerRigInTrigger != null && (this.launchBigMonkes || (double)this.playerRigInTrigger.scaleFactor < 0.99))
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ballistaState = BuilderPieceBallista.BallistaState.WaitingForTrigger;
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.WaitingForTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					return;
				}
				if (this.playerInTrigger)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PlayerInTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (this.autoLaunch && (double)Time.time > this.enteredTriggerTime + (double)this.autoLaunchDelay)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PrepareForLaunch:
			case BuilderPieceBallista.BallistaState.PrepareForLaunchLocal:
			{
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ResetFlags();
					this.myPiece.functionalPieceState = 0;
					this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
					return;
				}
				Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(this.playerRigInTrigger.transform, this.playerRigInTrigger.scaleFactor);
				Vector3 b = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 b2 = playerBodyCenterPosition - b;
				if (Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * Time.deltaTime)).sqrMagnitude < this.playerReadyToFireDist * this.myPiece.GetScale() * this.playerReadyToFireDist * this.myPiece.GetScale())
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 6, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			}
			case BuilderPieceBallista.BallistaState.Launching:
			case BuilderPieceBallista.BallistaState.LaunchingLocal:
				if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004F6D RID: 20333 RVA: 0x0018B357 File Offset: 0x00189557
		private void ResetFlags()
		{
			this.playerLaunched = false;
			this.loadCompleteTime = double.MaxValue;
		}

		// Token: 0x06004F6E RID: 20334 RVA: 0x0018B370 File Offset: 0x00189570
		private void UpdatePlayerPosition()
		{
			if (this.ballistaState != BuilderPieceBallista.BallistaState.PrepareForLaunchLocal && this.ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			GTPlayer instance = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale);
			Vector3 lhs = playerBodyCenterPosition - this.launchStart.position;
			BuilderPieceBallista.BallistaState ballistaState = this.ballistaState;
			if (ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				Vector3 b = Vector3.Dot(lhs, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 b2 = playerBodyCenterPosition - b;
				Vector3 a = Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * deltaTime));
				instance.transform.position = instance.transform.position + (a - b2);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				return;
			}
			if (ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			if (!this.playerLaunched)
			{
				float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
				float b3 = Vector3.Dot(lhs, this.launchDirection) / this.launchRampDistance;
				float num2 = 0.25f * this.myPiece.GetScale() / this.launchRampDistance;
				float num3 = Mathf.Max(num + num2, b3);
				float d = num3 * this.launchRampDistance;
				Vector3 a2 = this.launchDirection * d + this.launchStart.position;
				instance.transform.position + (a2 - playerBodyCenterPosition);
				instance.transform.position = instance.transform.position + (a2 - playerBodyCenterPosition);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				if (num3 >= 1f)
				{
					this.playerLaunched = true;
					this.launchedTime = (double)Time.time;
					instance.SetPlayerVelocity(this.launchSpeed * this.myPiece.GetScale() * this.launchDirection);
					instance.SetMaximumSlipThisFrame();
					return;
				}
			}
			else if ((double)Time.time < this.launchedTime + (double)this.slipOverrideDuration)
			{
				instance.SetMaximumSlipThisFrame();
			}
		}

		// Token: 0x06004F6F RID: 20335 RVA: 0x0018B5A8 File Offset: 0x001897A8
		private Vector3 GetPlayerBodyCenterPosition(Transform headTransform, float playerScale)
		{
			return headTransform.position + Quaternion.Euler(0f, headTransform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, this.playerBodyOffsetFromHead.z * playerScale) + Vector3.down * (this.playerBodyOffsetFromHead.y * playerScale);
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x0018B620 File Offset: 0x00189820
		private void OnTriggerEnter(Collider other)
		{
			if (this.playerRigInTrigger != null)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.launchBigMonkes && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			this.playerRigInTrigger = vrrig;
			this.playerInTrigger = true;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x0018B6C0 File Offset: 0x001898C0
		private void OnTriggerExit(Collider other)
		{
			if (this.playerRigInTrigger == null || !this.playerInTrigger)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (this.playerRigInTrigger.Equals(vrrig))
			{
				this.playerInTrigger = false;
				this.playerRigInTrigger = null;
			}
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x0018B758 File Offset: 0x00189958
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			if (!this.myPiece.GetTable().isTableMutable)
			{
				this.launchBigMonkes = true;
			}
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.playerLaunched = false;
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x0018B78F File Offset: 0x0018998F
		public void OnPieceDestroy()
		{
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x0018B7A4 File Offset: 0x001899A4
		public void OnPiecePlacementDeserialized()
		{
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x0018B7FC File Offset: 0x001899FC
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = true;
			}
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x0018B8CC File Offset: 0x00189ACC
		public void OnPieceDeactivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = false;
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Stop();
				this.launchParticles.Clear();
			}
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.ResetFlags();
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x0018B974 File Offset: 0x00189B74
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
			if ((BuilderPieceBallista.BallistaState)newState == this.ballistaState)
			{
				return;
			}
			if (newState == 4)
			{
				if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger && this.playerInTrigger && this.playerRigInTrigger != null)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), timeStamp);
					return;
				}
			}
			else
			{
				Debug.LogWarning("BuilderPiece Ballista unexpected state request for " + newState.ToString());
			}
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x0018BA14 File Offset: 0x00189C14
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			BuilderPieceBallista.BallistaState ballistaState = (BuilderPieceBallista.BallistaState)newState;
			if (ballistaState == this.ballistaState)
			{
				return;
			}
			switch (newState)
			{
			case 0:
				this.ResetFlags();
				goto IL_2C2;
			case 1:
				this.ResetFlags();
				foreach (Collider collider in this.disableWhileLaunching)
				{
					collider.enabled = true;
				}
				if (this.ballistaState == BuilderPieceBallista.BallistaState.Launching || this.ballistaState == BuilderPieceBallista.BallistaState.LaunchingLocal)
				{
					this.loadCompleteTime = (double)(Time.time + this.reloadDelay);
					if (this.loadSFX != null)
					{
						this.loadSFX.Play();
					}
				}
				else
				{
					this.loadCompleteTime = (double)(Time.time + this.loadTime);
				}
				this.animator.SetTrigger(this.loadTriggerHash);
				goto IL_2C2;
			case 2:
			case 5:
				goto IL_2C2;
			case 3:
				this.enteredTriggerTime = (double)Time.time;
				if (this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
					goto IL_2C2;
				}
				goto IL_2C2;
			case 4:
			{
				this.playerLaunched = false;
				if (!this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
				}
				if (!instigator.IsLocal)
				{
					goto IL_2C2;
				}
				GTPlayer instance = GTPlayer.Instance;
				if (Vector3.Distance(this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale), this.launchStart.position) > this.prepareForLaunchDistance * this.myPiece.GetScale() || (!this.launchBigMonkes && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99))
				{
					goto IL_2C2;
				}
				ballistaState = BuilderPieceBallista.BallistaState.PrepareForLaunchLocal;
				using (List<Collider>.Enumerator enumerator = this.disableWhileLaunching.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_2C2;
				}
				break;
			}
			case 6:
				break;
			default:
				goto IL_2C2;
			}
			this.playerLaunched = false;
			this.animator.SetTrigger(this.fireTriggerHash);
			if (this.launchSFX != null)
			{
				this.launchSFX.Play();
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Play();
			}
			if (this.debugDrawTrajectoryOnLaunch)
			{
				base.StartCoroutine(this.DebugDrawTrajectory(8f));
			}
			if (instigator.IsLocal && this.ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				ballistaState = BuilderPieceBallista.BallistaState.LaunchingLocal;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
			}
			IL_2C2:
			this.ballistaState = ballistaState;
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x0018BD08 File Offset: 0x00189F08
		public bool IsStateValid(byte state)
		{
			return state < 8;
		}

		// Token: 0x06004F7A RID: 20346 RVA: 0x0018BD0E File Offset: 0x00189F0E
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece == null || this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.UpdateStateMaster();
			}
			this.UpdatePlayerPosition();
		}

		// Token: 0x06004F7B RID: 20347 RVA: 0x0018BD44 File Offset: 0x00189F44
		private void UpdatePredictionLine()
		{
			float d = 0.033333335f;
			Vector3 vector = this.launchEnd.position;
			Vector3 a = (this.launchEnd.position - this.launchStart.position).normalized * this.launchSpeed;
			for (int i = 0; i < 240; i++)
			{
				this.predictionLinePoints[i] = vector;
				vector += a * d;
				a += Vector3.down * 9.8f * d;
			}
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x0018BDDE File Offset: 0x00189FDE
		private IEnumerator DebugDrawTrajectory(float duration)
		{
			this.UpdatePredictionLine();
			float startTime = Time.time;
			while (Time.time < startTime + duration)
			{
				DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
				DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
				yield return null;
			}
			yield break;
		}

		// Token: 0x04005856 RID: 22614
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04005857 RID: 22615
		[SerializeField]
		private List<Collider> triggers;

		// Token: 0x04005858 RID: 22616
		[SerializeField]
		private List<Collider> disableWhileLaunching;

		// Token: 0x04005859 RID: 22617
		[SerializeField]
		private BuilderSmallHandTrigger handTrigger;

		// Token: 0x0400585A RID: 22618
		[SerializeField]
		private bool autoLaunch;

		// Token: 0x0400585B RID: 22619
		[SerializeField]
		private float autoLaunchDelay = 0.75f;

		// Token: 0x0400585C RID: 22620
		private double enteredTriggerTime;

		// Token: 0x0400585D RID: 22621
		public Animator animator;

		// Token: 0x0400585E RID: 22622
		public Transform launchStart;

		// Token: 0x0400585F RID: 22623
		public Transform launchEnd;

		// Token: 0x04005860 RID: 22624
		public Transform launchBone;

		// Token: 0x04005861 RID: 22625
		[SerializeField]
		private SoundBankPlayer loadSFX;

		// Token: 0x04005862 RID: 22626
		[SerializeField]
		private SoundBankPlayer launchSFX;

		// Token: 0x04005863 RID: 22627
		[SerializeField]
		private SoundBankPlayer cockSFX;

		// Token: 0x04005864 RID: 22628
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x04005865 RID: 22629
		private bool hasLaunchParticles;

		// Token: 0x04005866 RID: 22630
		public float reloadDelay = 1f;

		// Token: 0x04005867 RID: 22631
		public float loadTime = 1.933f;

		// Token: 0x04005868 RID: 22632
		public float slipOverrideDuration = 0.1f;

		// Token: 0x04005869 RID: 22633
		private double launchedTime;

		// Token: 0x0400586A RID: 22634
		public float playerMagnetismStrength = 3f;

		// Token: 0x0400586B RID: 22635
		[Tooltip("Speed will be scaled by piece scale")]
		public float launchSpeed = 20f;

		// Token: 0x0400586C RID: 22636
		[Range(0f, 1f)]
		public float pitch;

		// Token: 0x0400586D RID: 22637
		private bool debugDrawTrajectoryOnLaunch;

		// Token: 0x0400586E RID: 22638
		private int loadTriggerHash = Animator.StringToHash("Load");

		// Token: 0x0400586F RID: 22639
		private int fireTriggerHash = Animator.StringToHash("Fire");

		// Token: 0x04005870 RID: 22640
		private int pitchParamHash = Animator.StringToHash("Pitch");

		// Token: 0x04005871 RID: 22641
		private int idleStateHash = Animator.StringToHash("Idle");

		// Token: 0x04005872 RID: 22642
		private int loadStateHash = Animator.StringToHash("Load");

		// Token: 0x04005873 RID: 22643
		private int fireStateHash = Animator.StringToHash("Fire");

		// Token: 0x04005874 RID: 22644
		private bool playerInTrigger;

		// Token: 0x04005875 RID: 22645
		private VRRig playerRigInTrigger;

		// Token: 0x04005876 RID: 22646
		private bool playerLaunched;

		// Token: 0x04005877 RID: 22647
		private float playerReadyToFireDist = 1.6667f;

		// Token: 0x04005878 RID: 22648
		private float prepareForLaunchDistance = 2.5f;

		// Token: 0x04005879 RID: 22649
		private Vector3 launchDirection;

		// Token: 0x0400587A RID: 22650
		private float launchRampDistance;

		// Token: 0x0400587B RID: 22651
		private float playerPullInRate;

		// Token: 0x0400587C RID: 22652
		private float appliedAnimatorPitch;

		// Token: 0x0400587D RID: 22653
		private bool launchBigMonkes;

		// Token: 0x0400587E RID: 22654
		private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

		// Token: 0x0400587F RID: 22655
		private double loadCompleteTime;

		// Token: 0x04005880 RID: 22656
		private BuilderPieceBallista.BallistaState ballistaState;

		// Token: 0x04005881 RID: 22657
		private const int predictionLineSamples = 240;

		// Token: 0x04005882 RID: 22658
		private Vector3[] predictionLinePoints = new Vector3[240];

		// Token: 0x02000C8A RID: 3210
		private enum BallistaState
		{
			// Token: 0x04005884 RID: 22660
			Idle,
			// Token: 0x04005885 RID: 22661
			Loading,
			// Token: 0x04005886 RID: 22662
			WaitingForTrigger,
			// Token: 0x04005887 RID: 22663
			PlayerInTrigger,
			// Token: 0x04005888 RID: 22664
			PrepareForLaunch,
			// Token: 0x04005889 RID: 22665
			PrepareForLaunchLocal,
			// Token: 0x0400588A RID: 22666
			Launching,
			// Token: 0x0400588B RID: 22667
			LaunchingLocal,
			// Token: 0x0400588C RID: 22668
			Count
		}
	}
}
