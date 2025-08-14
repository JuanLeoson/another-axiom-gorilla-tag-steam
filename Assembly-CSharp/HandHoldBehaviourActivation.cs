using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000285 RID: 645
public class HandHoldBehaviourActivation : Tappable
{
	// Token: 0x06000EB9 RID: 3769 RVA: 0x00058C2C File Offset: 0x00056E2C
	protected override void OnEnable()
	{
		base.OnEnable();
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x00058C6C File Offset: 0x00056E6C
	public override void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b = this.m_playerGrabCounts.GetValueOrDefault(sender.Sender.ActorNumber, 0);
		b += 1;
		if (b > 2)
		{
			return;
		}
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		this.grabs++;
		if (this.grabs < 2)
		{
			this.ActivationStart.Invoke();
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00058CD8 File Offset: 0x00056ED8
	public override void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(sender.Sender.ActorNumber, out b) || b < 1)
		{
			return;
		}
		b -= 1;
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - 1);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x00058D58 File Offset: 0x00056F58
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(player.ActorNumber, out b))
		{
			return;
		}
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - (int)b);
		this.m_playerGrabCounts.Remove(player.ActorNumber);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00058DC0 File Offset: 0x00056FC0
	private void OnLeftRoom()
	{
		byte valueOrDefault = this.m_playerGrabCounts.GetValueOrDefault(NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
		if (this.grabs > 0 && valueOrDefault < 1)
		{
			this.ActivationStop.Invoke();
		}
		this.grabs = (int)valueOrDefault;
		this.m_playerGrabCounts.Clear();
		this.m_playerGrabCounts[NetworkSystem.Instance.LocalPlayer.ActorNumber] = valueOrDefault;
	}

	// Token: 0x040017A4 RID: 6052
	[SerializeField]
	private UnityEvent ActivationStart;

	// Token: 0x040017A5 RID: 6053
	[SerializeField]
	private UnityEvent ActivationStop;

	// Token: 0x040017A6 RID: 6054
	private int grabs;

	// Token: 0x040017A7 RID: 6055
	private readonly Dictionary<int, byte> m_playerGrabCounts = new Dictionary<int, byte>(10);
}
