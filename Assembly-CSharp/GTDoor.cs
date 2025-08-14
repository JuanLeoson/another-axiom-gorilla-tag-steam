using System;
using BoingKit;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Token: 0x020001FE RID: 510
public class GTDoor : NetworkSceneObject
{
	// Token: 0x06000C15 RID: 3093 RVA: 0x00041278 File Offset: 0x0003F478
	protected override void Start()
	{
		base.Start();
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
		GTDoorTrigger[] array2 = this.doorButtonTriggers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].TriggeredEvent.AddListener(new UnityAction(this.DoorButtonTriggered));
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000412E4 File Offset: 0x0003F4E4
	private void Update()
	{
		if (this.currentState == GTDoor.DoorState.Open || this.currentState == GTDoor.DoorState.Closed)
		{
			if (Time.time < this.lastChecked + this.secondsCheck)
			{
				return;
			}
			this.lastChecked = Time.time;
		}
		this.UpdateDoorState();
		this.UpdateDoorAnimation();
		Collider[] array;
		if (this.currentState == GTDoor.DoorState.Closed)
		{
			array = this.doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00041374 File Offset: 0x0003F574
	private void UpdateDoorState()
	{
		this.peopleInHoldOpenVolume = false;
		foreach (GTDoorTrigger gtdoorTrigger in this.doorHoldOpenTriggers)
		{
			gtdoorTrigger.ValidateOverlappingColliders();
			if (gtdoorTrigger.overlapCount > 0)
			{
				this.peopleInHoldOpenVolume = true;
				break;
			}
		}
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
			if (this.buttonTriggeredThisFrame)
			{
				this.buttonTriggeredThisFrame = false;
				if (!NetworkSystem.Instance.InRoom)
				{
					this.OpenDoor();
				}
				else
				{
					this.currentState = GTDoor.DoorState.OpeningWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Opening
					});
				}
			}
			break;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			break;
		case GTDoor.DoorState.Closing:
			if (this.doorSpring.Value < 1f)
			{
				this.currentState = GTDoor.DoorState.Closed;
			}
			if (this.peopleInHoldOpenVolume)
			{
				this.currentState = GTDoor.DoorState.HeldOpenLocally;
				if (NetworkSystem.Instance.InRoom && base.IsMine)
				{
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.HeldOpen
					});
				}
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
			}
			break;
		case GTDoor.DoorState.Open:
			if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
			{
				if (this.peopleInHoldOpenVolume)
				{
					this.currentState = GTDoor.DoorState.HeldOpenLocally;
					if (NetworkSystem.Instance.InRoom && base.IsMine)
					{
						this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
						{
							GTDoor.DoorState.HeldOpen
						});
					}
				}
				else if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Closing
					});
				}
			}
			break;
		case GTDoor.DoorState.Opening:
			if (this.doorSpring.Value > 89f)
			{
				this.currentState = GTDoor.DoorState.Open;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			if (!this.peopleInHoldOpenVolume)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Closing
					});
				}
			}
			break;
		case GTDoor.DoorState.HeldOpenLocally:
			if (!this.peopleInHoldOpenVolume)
			{
				this.CloseDoor();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			GTDoor.DoorState doorState = this.currentState;
			if (doorState == GTDoor.DoorState.ClosingWaitingOnRPC)
			{
				this.CloseDoor();
				return;
			}
			if (doorState != GTDoor.DoorState.OpeningWaitingOnRPC)
			{
				return;
			}
			this.OpenDoor();
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00041618 File Offset: 0x0003F818
	private void DoorButtonTriggered()
	{
		GTDoor.DoorState doorState = this.currentState;
		if (doorState - GTDoor.DoorState.Open <= 4)
		{
			return;
		}
		this.buttonTriggeredThisFrame = true;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0004163C File Offset: 0x0003F83C
	private void OpenDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			this.ResetDoorOpenedTime();
			this.audioSource.GTPlayOneShot(this.openSound, 1f);
			this.currentState = GTDoor.DoorState.Opening;
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x000416A4 File Offset: 0x0003F8A4
	private void CloseDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.Opening:
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.audioSource.GTPlayOneShot(this.closeSound, 1f);
			this.currentState = GTDoor.DoorState.Closing;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00041704 File Offset: 0x0003F904
	private void UpdateDoorAnimation()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
			return;
		}
		this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
		this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x000417E3 File Offset: 0x0003F9E3
	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x000417F0 File Offset: 0x0003F9F0
	[PunRPC]
	public void ChangeDoorState(GTDoor.DoorState shouldOpenState)
	{
		this.ChangeDoorStateShared(shouldOpenState);
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x000417FC File Offset: 0x0003F9FC
	[Rpc]
	public unsafe static void RPC_ChangeDoorState(NetworkRunner runner, GTDoor.DoorState shouldOpenState, int doorId)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += 4;
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")), data);
				*(GTDoor.DoorState*)(data + num2) = shouldOpenState;
				num2 += 4;
				*(int*)(data + num2) = doorId;
				num2 += 4;
				ptr->Offset = num2 * 8;
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
		}
		GTDoor[] array = UnityEngine.Object.FindObjectsOfType<GTDoor>(true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (GTDoor gtdoor in array)
		{
			if (gtdoor.GTDoorID == doorId)
			{
				gtdoor.ChangeDoorStateShared(shouldOpenState);
			}
		}
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00041934 File Offset: 0x0003FB34
	private void ChangeDoorStateShared(GTDoor.DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.HeldOpenLocally:
			break;
		case GTDoor.DoorState.Closing:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpen:
				this.CloseDoor();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.Opening:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
				this.OpenDoor();
				return;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
				break;
			case GTDoor.DoorState.Closing:
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpenLocally:
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("shouldOpenState", shouldOpenState, null);
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00041A54 File Offset: 0x0003FC54
	public void SetupDoorIDs()
	{
		GTDoor[] array = UnityEngine.Object.FindObjectsOfType<GTDoor>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GTDoorID = i + 1;
		}
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00041AB8 File Offset: 0x0003FCB8
	[NetworkRpcStaticWeavedInvoker("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ChangeDoorState@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		GTDoor.DoorState doorState = *(GTDoor.DoorState*)(data + num);
		num += 4;
		GTDoor.DoorState shouldOpenState = doorState;
		int num2 = *(int*)(data + num);
		num += 4;
		int doorId = num2;
		NetworkBehaviourUtils.InvokeRpc = true;
		GTDoor.RPC_ChangeDoorState(runner, shouldOpenState, doorId);
	}

	// Token: 0x04000EE3 RID: 3811
	[SerializeField]
	private Transform doorTransform;

	// Token: 0x04000EE4 RID: 3812
	[SerializeField]
	private Collider[] doorColliders;

	// Token: 0x04000EE5 RID: 3813
	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	// Token: 0x04000EE6 RID: 3814
	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	// Token: 0x04000EE7 RID: 3815
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000EE8 RID: 3816
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x04000EE9 RID: 3817
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x04000EEA RID: 3818
	[SerializeField]
	private float doorOpenSpeed = 1f;

	// Token: 0x04000EEB RID: 3819
	[SerializeField]
	private float doorCloseSpeed = 1f;

	// Token: 0x04000EEC RID: 3820
	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	// Token: 0x04000EED RID: 3821
	private int GTDoorID;

	// Token: 0x04000EEE RID: 3822
	[DebugOption]
	private GTDoor.DoorState currentState;

	// Token: 0x04000EEF RID: 3823
	private float tLastOpened;

	// Token: 0x04000EF0 RID: 3824
	private FloatSpring doorSpring;

	// Token: 0x04000EF1 RID: 3825
	[DebugOption]
	private bool peopleInHoldOpenVolume;

	// Token: 0x04000EF2 RID: 3826
	[DebugOption]
	private bool buttonTriggeredThisFrame;

	// Token: 0x04000EF3 RID: 3827
	private float lastChecked;

	// Token: 0x04000EF4 RID: 3828
	private float secondsCheck = 1f;

	// Token: 0x020001FF RID: 511
	public enum DoorState
	{
		// Token: 0x04000EF6 RID: 3830
		Closed,
		// Token: 0x04000EF7 RID: 3831
		ClosingWaitingOnRPC,
		// Token: 0x04000EF8 RID: 3832
		Closing,
		// Token: 0x04000EF9 RID: 3833
		Open,
		// Token: 0x04000EFA RID: 3834
		OpeningWaitingOnRPC,
		// Token: 0x04000EFB RID: 3835
		Opening,
		// Token: 0x04000EFC RID: 3836
		HeldOpen,
		// Token: 0x04000EFD RID: 3837
		HeldOpenLocally
	}
}
