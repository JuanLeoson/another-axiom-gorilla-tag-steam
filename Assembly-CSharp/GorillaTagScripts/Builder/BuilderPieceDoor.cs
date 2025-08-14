using System;
using BoingKit;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C8C RID: 3212
	public class BuilderPieceDoor : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004F84 RID: 20356 RVA: 0x0018BF9C File Offset: 0x0018A19C
		private void Awake()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered += this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited += this.OnHoldTriggerExited;
			}
			BuilderSmallHandTrigger[] array2 = this.doorButtonTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].TriggeredEvent.AddListener(new UnityAction(this.OnDoorButtonTriggered));
			}
		}

		// Token: 0x06004F85 RID: 20357 RVA: 0x0018C014 File Offset: 0x0018A214
		private void OnDestroy()
		{
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.doorHoldTriggers)
			{
				builderSmallMonkeTrigger.onTriggerFirstEntered -= this.OnHoldTriggerEntered;
				builderSmallMonkeTrigger.onTriggerLastExited -= this.OnHoldTriggerExited;
			}
			BuilderSmallHandTrigger[] array2 = this.doorButtonTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnDoorButtonTriggered));
			}
		}

		// Token: 0x06004F86 RID: 20358 RVA: 0x0018C08C File Offset: 0x0018A28C
		private void SetDoorState(BuilderPieceDoor.DoorState value)
		{
			bool flag = this.currentState == BuilderPieceDoor.DoorState.Closed || (this.currentState == BuilderPieceDoor.DoorState.Open && this.IsToggled);
			bool flag2 = value == BuilderPieceDoor.DoorState.Closed || (value == BuilderPieceDoor.DoorState.Open && this.IsToggled);
			this.currentState = value;
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

		// Token: 0x06004F87 RID: 20359 RVA: 0x0018C0FC File Offset: 0x0018A2FC
		private void UpdateDoorStateMaster()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoor.DoorState.Closing:
				if (this.doorSpring.Value < 1f)
				{
					this.doorSpring.Reset();
					this.doorTransform.localRotation = Quaternion.identity;
					if (this.isDoubleDoor && this.doorTransformB != null)
					{
						this.doorTransformB.localRotation = Quaternion.identity;
					}
					this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.Open:
				if (!this.IsToggled && Time.time - this.tLastOpened > this.timeUntilDoorCloses)
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
						this.CheckHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.Opening:
				if (this.doorSpring.Value > 89f)
				{
					this.SetDoorState(BuilderPieceDoor.DoorState.Open);
					return;
				}
				break;
			case BuilderPieceDoor.DoorState.HeldOpen:
				if (!this.IsToggled && (double)Time.time > this.CheckHoldTriggersTime)
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
						this.CheckHoldTriggersTime = (double)(Time.time + this.checkHoldTriggersDelay);
						return;
					}
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004F88 RID: 20360 RVA: 0x0018C310 File Offset: 0x0018A510
		private void UpdateDoorState()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closing)
			{
				if (doorState == BuilderPieceDoor.DoorState.Opening && this.doorSpring.Value > 89f)
				{
					this.SetDoorState(BuilderPieceDoor.DoorState.Open);
					return;
				}
			}
			else if (this.doorSpring.Value < 1f)
			{
				this.doorSpring.Reset();
				this.doorTransform.localRotation = Quaternion.identity;
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.identity;
				}
				this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			}
		}

		// Token: 0x06004F89 RID: 20361 RVA: 0x0018C3A0 File Offset: 0x0018A5A0
		private void CloseDoor()
		{
			switch (this.currentState)
			{
			case BuilderPieceDoor.DoorState.Closed:
			case BuilderPieceDoor.DoorState.Closing:
			case BuilderPieceDoor.DoorState.Opening:
				return;
			case BuilderPieceDoor.DoorState.Open:
			case BuilderPieceDoor.DoorState.HeldOpen:
				this.closeSound.Play();
				this.SetDoorState(BuilderPieceDoor.DoorState.Closing);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06004F8A RID: 20362 RVA: 0x0018C3EC File Offset: 0x0018A5EC
		private void OpenDoor()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState == BuilderPieceDoor.DoorState.Closed)
			{
				this.tLastOpened = Time.time;
				this.openSound.Play();
				this.SetDoorState(BuilderPieceDoor.DoorState.Opening);
				return;
			}
			if (doorState - BuilderPieceDoor.DoorState.Closing > 3)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x0018C430 File Offset: 0x0018A630
		private void UpdateDoorAnimation()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState > BuilderPieceDoor.DoorState.Closing && doorState - BuilderPieceDoor.DoorState.Open <= 2)
			{
				this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
					return;
				}
			}
			else
			{
				this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
				this.doorTransform.localRotation = Quaternion.Euler(this.rotateAxis * this.doorSpring.Value);
				if (this.isDoubleDoor && this.doorTransformB != null)
				{
					this.doorTransformB.localRotation = Quaternion.Euler(this.rotateAxisB * this.doorSpring.Value);
				}
			}
		}

		// Token: 0x06004F8C RID: 20364 RVA: 0x0018C570 File Offset: 0x0018A770
		private void OnDoorButtonTriggered()
		{
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closed)
			{
				if (doorState != BuilderPieceDoor.DoorState.Open)
				{
					return;
				}
				if (this.IsToggled)
				{
					if (NetworkSystem.Instance.IsMasterClient)
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
				}
				return;
			}
			else
			{
				if (NetworkSystem.Instance.IsMasterClient)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 3);
				return;
			}
		}

		// Token: 0x06004F8D RID: 20365 RVA: 0x0018C654 File Offset: 0x0018A854
		private void OnHoldTriggerEntered()
		{
			this.peopleInHoldOpenVolume = true;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderPieceDoor.DoorState doorState = this.currentState;
			if (doorState != BuilderPieceDoor.DoorState.Closed)
			{
				if (doorState != BuilderPieceDoor.DoorState.Closing)
				{
					return;
				}
				if (!this.IsToggled)
				{
					this.openSound.Play();
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
			}
			else if (this.isAutomatic)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			}
		}

		// Token: 0x06004F8E RID: 20366 RVA: 0x0018C700 File Offset: 0x0018A900
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
			if (this.currentState == BuilderPieceDoor.DoorState.HeldOpen && !this.peopleInHoldOpenVolume)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004F8F RID: 20367 RVA: 0x0018C794 File Offset: 0x0018A994
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.tLastOpened = 0f;
			this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.identity;
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.identity;
			}
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			if (this.lineRenderers != null)
			{
				LineRenderer[] array2 = this.lineRenderers;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].widthMultiplier = this.myPiece.GetScale();
				}
			}
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x0018C844 File Offset: 0x0018AA44
		public void OnPieceActivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x0018C870 File Offset: 0x0018AA70
		public void OnPieceDeactivate()
		{
			Collider[] array = this.triggerVolumes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.myPiece.functionalPieceState = 0;
			this.SetDoorState(BuilderPieceDoor.DoorState.Closed);
			this.doorSpring.Reset();
			this.doorTransform.localRotation = Quaternion.identity;
			if (this.isDoubleDoor && this.doorTransformB != null)
			{
				this.doorTransformB.localRotation = Quaternion.identity;
			}
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x0018C8F0 File Offset: 0x0018AAF0
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.IsStateValid(newState) && instigator != null && this.currentState != (BuilderPieceDoor.DoorState)newState)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x0018C948 File Offset: 0x0018AB48
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			switch (newState)
			{
			case 1:
				if (this.currentState == BuilderPieceDoor.DoorState.Open || this.currentState == BuilderPieceDoor.DoorState.HeldOpen)
				{
					this.CloseDoor();
				}
				break;
			case 3:
				if (this.currentState == BuilderPieceDoor.DoorState.Closed)
				{
					this.OpenDoor();
				}
				break;
			case 4:
				if (this.currentState == BuilderPieceDoor.DoorState.Closing)
				{
					this.openSound.Play();
				}
				break;
			}
			this.SetDoorState((BuilderPieceDoor.DoorState)newState);
		}

		// Token: 0x06004F96 RID: 20374 RVA: 0x0018C9C0 File Offset: 0x0018ABC0
		public bool IsStateValid(byte state)
		{
			return state < 5;
		}

		// Token: 0x06004F97 RID: 20375 RVA: 0x0018C9C8 File Offset: 0x0018ABC8
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece != null && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				if (!NetworkSystem.Instance.InRoom && this.currentState != BuilderPieceDoor.DoorState.Closed)
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

		// Token: 0x04005892 RID: 22674
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04005893 RID: 22675
		[SerializeField]
		private Vector3 rotateAxis = Vector3.up;

		// Token: 0x04005894 RID: 22676
		[Tooltip("True if the door stays open until the button is triggered again")]
		[SerializeField]
		private bool IsToggled;

		// Token: 0x04005895 RID: 22677
		[Tooltip("True if the door opens when players enter the Keep Open Trigger")]
		[SerializeField]
		private bool isAutomatic;

		// Token: 0x04005896 RID: 22678
		[SerializeField]
		private Transform doorTransform;

		// Token: 0x04005897 RID: 22679
		[SerializeField]
		private Collider[] triggerVolumes;

		// Token: 0x04005898 RID: 22680
		[SerializeField]
		private BuilderSmallHandTrigger[] doorButtonTriggers;

		// Token: 0x04005899 RID: 22681
		[SerializeField]
		private BuilderSmallMonkeTrigger[] doorHoldTriggers;

		// Token: 0x0400589A RID: 22682
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400589B RID: 22683
		[SerializeField]
		private SoundBankPlayer openSound;

		// Token: 0x0400589C RID: 22684
		[SerializeField]
		private SoundBankPlayer closeSound;

		// Token: 0x0400589D RID: 22685
		[SerializeField]
		private float doorOpenSpeed = 1f;

		// Token: 0x0400589E RID: 22686
		[SerializeField]
		private float doorCloseSpeed = 1f;

		// Token: 0x0400589F RID: 22687
		[SerializeField]
		[Range(1.5f, 10f)]
		private float timeUntilDoorCloses = 3f;

		// Token: 0x040058A0 RID: 22688
		[Header("Double Door Settings")]
		[SerializeField]
		private bool isDoubleDoor;

		// Token: 0x040058A1 RID: 22689
		[SerializeField]
		private Vector3 rotateAxisB = Vector3.down;

		// Token: 0x040058A2 RID: 22690
		[SerializeField]
		private Transform doorTransformB;

		// Token: 0x040058A3 RID: 22691
		[SerializeField]
		private LineRenderer[] lineRenderers;

		// Token: 0x040058A4 RID: 22692
		private BuilderPieceDoor.DoorState currentState;

		// Token: 0x040058A5 RID: 22693
		private float tLastOpened;

		// Token: 0x040058A6 RID: 22694
		private FloatSpring doorSpring;

		// Token: 0x040058A7 RID: 22695
		private bool peopleInHoldOpenVolume;

		// Token: 0x040058A8 RID: 22696
		private double CheckHoldTriggersTime;

		// Token: 0x040058A9 RID: 22697
		private float checkHoldTriggersDelay = 3f;

		// Token: 0x02000C8D RID: 3213
		public enum DoorState
		{
			// Token: 0x040058AB RID: 22699
			Closed,
			// Token: 0x040058AC RID: 22700
			Closing,
			// Token: 0x040058AD RID: 22701
			Open,
			// Token: 0x040058AE RID: 22702
			Opening,
			// Token: 0x040058AF RID: 22703
			HeldOpen
		}
	}
}
