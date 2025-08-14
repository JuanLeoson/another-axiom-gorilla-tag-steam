using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200000C RID: 12
[NetworkBehaviourWeaved(0)]
public class ArcadeMachineJoystickNetworkState : NetworkComponent
{
	// Token: 0x06000030 RID: 48 RVA: 0x0000261A File Offset: 0x0000081A
	private new void Awake()
	{
		this.joystick = base.GetComponent<ArcadeMachineJoystick>();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002628 File Offset: 0x00000828
	public override void ReadDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002628 File Offset: 0x00000828
	public override void WriteDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000018 RID: 24
	private ArcadeMachineJoystick joystick;
}
