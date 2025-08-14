using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public class GRToolUpgradeStationDepositBox : MonoBehaviour
{
	// Token: 0x06002919 RID: 10521 RVA: 0x000DD51C File Offset: 0x000DB71C
	public void OnTriggerEnter(Collider other)
	{
		GRTool component = other.attachedRigidbody.GetComponent<GRTool>();
		if (component.IsNotNull() && component.gameEntity.IsNotNull() && component.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && component.gameEntity.IsHeldByLocalPlayer())
		{
			Debug.LogError("Tool Deposited");
			this.upgradeStation.ToolInserted(component);
		}
	}

	// Token: 0x04003506 RID: 13574
	public GRToolUpgradeStation upgradeStation;
}
