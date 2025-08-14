using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C8E RID: 3214
	public class BuilderPieceDoorSwinging : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004F99 RID: 20377 RVA: 0x0018CA84 File Offset: 0x0018AC84
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x06004F9A RID: 20378 RVA: 0x0018CB04 File Offset: 0x0018AD04
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			this.frontTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnFrontTriggerEntered));
			this.backTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnBackTriggerEntered));
		}

		// Token: 0x06004F9B RID: 20379 RVA: 0x0018CB84 File Offset: 0x0018AD84
		private void OnFrontTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 7, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 7);
			}
		}

		// Token: 0x06004F9C RID: 20380 RVA: 0x0018CBF8 File Offset: 0x0018ADF8
		private void OnBackTriggerEntered()
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
			}
		}

		// Token: 0x06004F9D RID: 20381 RVA: 0x0018CC6C File Offset: 0x0018AE6C
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = this.currentState;
			if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				if (swingingDoorState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (swingingDoorState != BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					return;
				}
				this.openSound.Play();
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 8, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x0018CD1C File Offset: 0x0018AF1C
		private void OnHoldTriggerExited()
		{
			this.peopleInHoldOpenVolume = false;
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.ValidateOverlappingColliders();
				if (builderSmallMonkeTrigger.overlapCount > 0)
				{
					this.peopleInHoldOpenVolume = true;
					break;
				}
			}
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 5, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004F9F RID: 20383 RVA: 0x0018CDF0 File Offset: 0x0018AFF0
		private void SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			bool flag2 = value == BuilderPieceDoorSwinging.SwingingDoorState.Closed;
			this.currentState = value;
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.frontTrigger.enabled = true;
				this.backTrigger.enabled = true;
			}
			else
			{
				this.frontTrigger.enabled = false;
				this.backTrigger.enabled = false;
			}
			if (flag != flag2)
			{
				if (flag2)
				{
					this.myPiece.GetTable().UnregisterFunctionalPiece(this);
					return;
				}
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x0018CE78 File Offset: 0x0018B078
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
				{
					this.peopleInHoldOpenVolume = false;
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn : BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut;
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState2 = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				if ((double)Time.time > this.checkHoldTriggersTime)
				{
					foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger2 in this.doorHoldTriggers)
					{
						builderSmallMonkeTrigger2.ValidateOverlappingColliders();
						if (builderSmallMonkeTrigger2.overlapCount > 0)
						{
							this.peopleInHoldOpenVolume = true;
							break;
						}
					}
					if (this.peopleInHoldOpenVolume)
					{
						this.checkHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					BuilderPieceDoorSwinging.SwingingDoorState swingingDoorState3 = (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn) ? BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn : BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)swingingDoorState3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004FA1 RID: 20385 RVA: 0x0018D0C0 File Offset: 0x0018B2C0
		private void UpdateDoorState()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
				if (Mathf.Abs(this.doorSpring.Value) < 1f && Mathf.Abs(this.doorSpring.Velocity) < this.doorClosedVelocityMag)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenOut);
					return;
				}
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				if (Mathf.Abs(this.doorSpring.Value) > 89f)
				{
					this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.OpenIn);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004FA2 RID: 20386 RVA: 0x0018D170 File Offset: 0x0018B370
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut);
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
				break;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004FA3 RID: 20387 RVA: 0x0018D1CE File Offset: 0x0018B3CE
		private void OpenDoor(bool openIn)
		{
			if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(openIn ? BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn : BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut);
			}
		}

		// Token: 0x06004FA4 RID: 20388 RVA: 0x0018D1FC File Offset: 0x0018B3FC
		private void UpdateDoorAnimation()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningOut:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut:
				this.doorSpring.TrackDampingRatio(-90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			case BuilderPieceDoorSwinging.SwingingDoorState.OpenIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.OpeningIn:
			case BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn:
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
				return;
			}
			this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, this.dampingRatio, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x0018D3EC File Offset: 0x0018B5EC
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004FA8 RID: 20392 RVA: 0x0018D434 File Offset: 0x0018B634
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x06004FA9 RID: 20393 RVA: 0x0018D460 File Offset: 0x0018B660
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoorSwinging.SwingingDoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
			}
		}

		// Token: 0x06004FAA RID: 20394 RVA: 0x0018D50C File Offset: 0x0018B70C
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenOut || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenOut)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(false);
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingOut)
				{
					this.openSound.Play();
				}
				break;
			case 5:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.OpenIn || this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.HeldOpenIn)
				{
					this.CloseDoor();
				}
				break;
			case 7:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.OpenDoor(true);
				}
				break;
			case 8:
				if (this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.ClosingIn)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoorSwinging.SwingingDoorState)newState);
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x0018D5DC File Offset: 0x0018B7DC
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && (newState == 7 || newState == 3) && this.currentState == BuilderPieceDoorSwinging.SwingingDoorState.Closed)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x0018D63A File Offset: 0x0018B83A
		public bool IsStateValid(byte state)
		{
			return state <= 8;
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x0018D644 File Offset: 0x0018B844
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoorSwinging.SwingingDoorState.Closed)
				{
					this.CloseDoor();
				}
				else if (NetworkSystem.Instance.IsMasterClient)
				{
					this.UpdateDoorStateMaster();
				}
				else
				{
					this.UpdateDoorState();
				}
				this.UpdateDoorAnimation();
			}
		}

		// Token: 0x040058B0 RID: 22704
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040058B1 RID: 22705
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x040058B2 RID: 22706
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x040058B3 RID: 22707
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x040058B4 RID: 22708
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x040058B5 RID: 22709
		[SerializeField]
		private BuilderSmallHandTrigger frontTrigger;

		// Token: 0x040058B6 RID: 22710
		[SerializeField]
		private BuilderSmallHandTrigger backTrigger;

		// Token: 0x040058B7 RID: 22711
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040058B8 RID: 22712
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x040058B9 RID: 22713
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x040058BA RID: 22714
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x040058BB RID: 22715
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x040058BC RID: 22716
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x040058BD RID: 22717
		[SerializeField]
		private float doorClosedVelocityMag = 30f;

		// Token: 0x040058BE RID: 22718
		[SerializeField]
		private float dampingRatio = 0.5f;

		// Token: 0x040058BF RID: 22719
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x040058C0 RID: 22720
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x040058C1 RID: 22721
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x040058C2 RID: 22722
		private BuilderPieceDoorSwinging.SwingingDoorState currentState;

		// Token: 0x040058C3 RID: 22723
		private float tLastOpened;

		// Token: 0x040058C4 RID: 22724
		private FloatSpring doorSpring;

		// Token: 0x040058C5 RID: 22725
		private bool peopleInHoldOpenVolume;

		// Token: 0x040058C6 RID: 22726
		private double checkHoldTriggersTime;

		// Token: 0x040058C7 RID: 22727
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x040058C8 RID: 22728
		private int pushDirection = 1;

		// Token: 0x02000C8F RID: 3215
		private enum SwingingDoorState
		{
			// Token: 0x040058CA RID: 22730
			Closed,
			// Token: 0x040058CB RID: 22731
			ClosingOut,
			// Token: 0x040058CC RID: 22732
			OpenOut,
			// Token: 0x040058CD RID: 22733
			OpeningOut,
			// Token: 0x040058CE RID: 22734
			HeldOpenOut,
			// Token: 0x040058CF RID: 22735
			ClosingIn,
			// Token: 0x040058D0 RID: 22736
			OpenIn,
			// Token: 0x040058D1 RID: 22737
			OpeningIn,
			// Token: 0x040058D2 RID: 22738
			HeldOpenIn
		}
	}
}
