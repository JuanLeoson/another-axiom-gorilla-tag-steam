using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002B7 RID: 695
[NetworkBehaviourWeaved(0)]
public class FusionPlayerProperties : NetworkBehaviour
{
	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001028 RID: 4136 RVA: 0x0005D7AC File Offset: 0x0005B9AC
	[Capacity(10)]
	private NetworkDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo> netPlayerAttributes
	{
		get
		{
			return default(NetworkDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo>);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06001029 RID: 4137 RVA: 0x0005D7C4 File Offset: 0x0005B9C4
	public FusionPlayerProperties.PlayerInfo PlayerProperties
	{
		get
		{
			return this.netPlayerAttributes[base.Runner.LocalPlayer];
		}
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x0005D7EA File Offset: 0x0005B9EA
	private void OnAttributesChanged()
	{
		FusionPlayerProperties.PlayerAttributeOnChanged playerAttributeOnChanged = this.playerAttributeOnChanged;
		if (playerAttributeOnChanged == null)
		{
			return;
		}
		playerAttributeOnChanged();
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x0005D7FC File Offset: 0x0005B9FC
	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true, TickAligned = true)]
	public unsafe void RPC_UpdatePlayerAttributes(FusionPlayerProperties.PlayerInfo newInfo, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void FusionPlayerProperties::RPC_UpdatePlayerAttributes(FusionPlayerProperties/PlayerInfo,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 960;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(FusionPlayerProperties.PlayerInfo*)(data + num2) = newInfo;
						num2 += 960;
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		Debug.Log("Update Player attributes triggered");
		PlayerRef source = info.Source;
		if (this.netPlayerAttributes.ContainsKey(source))
		{
			Debug.Log("Current nickname is " + this.netPlayerAttributes[source].NickName.ToString());
			Debug.Log("Sent nickname is " + newInfo.NickName.ToString());
			if (this.netPlayerAttributes[source].Equals(newInfo))
			{
				Debug.Log("Info is already correct for this user. Shouldnt have received an RPC in this case.");
				return;
			}
		}
		this.netPlayerAttributes.Set(source, newInfo);
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x0005D9F9 File Offset: 0x0005BBF9
	public override void Spawned()
	{
		Debug.Log("Player props SPAWNED!");
		if (base.Runner.Mode == SimulationModes.Client)
		{
			Debug.Log("SET Player Properties manager!");
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0005DA20 File Offset: 0x0005BC20
	public string GetDisplayName(PlayerRef player)
	{
		return this.netPlayerAttributes[player].NickName.Value;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x0005DA4C File Offset: 0x0005BC4C
	public string GetLocalDisplayName()
	{
		return this.netPlayerAttributes[base.Runner.LocalPlayer].NickName.Value;
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x0005DA84 File Offset: 0x0005BC84
	public bool GetProperty(PlayerRef player, string propertyName, out string propertyValue)
	{
		NetworkString<_32> networkString;
		if (this.netPlayerAttributes[player].properties.TryGet(propertyName, out networkString))
		{
			propertyValue = networkString.Value;
			return true;
		}
		propertyValue = null;
		return false;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0005DACC File Offset: 0x0005BCCC
	public bool PlayerHasEntry(PlayerRef player)
	{
		return this.netPlayerAttributes.ContainsKey(player);
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0005DAE8 File Offset: 0x0005BCE8
	public void RemovePlayerEntry(PlayerRef player)
	{
		if (base.Object.HasStateAuthority)
		{
			string value = this.netPlayerAttributes[player].NickName.Value;
			this.netPlayerAttributes.Remove(player);
			Debug.Log("Removed " + value + "player properties as they just left.");
		}
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x0005DB50 File Offset: 0x0005BD50
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_UpdatePlayerAttributes@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		FusionPlayerProperties.PlayerInfo playerInfo = *(FusionPlayerProperties.PlayerInfo*)(data + num);
		num += 960;
		FusionPlayerProperties.PlayerInfo newInfo = playerInfo;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((FusionPlayerProperties)behaviour).RPC_UpdatePlayerAttributes(newInfo, info);
	}

	// Token: 0x0400187D RID: 6269
	public FusionPlayerProperties.PlayerAttributeOnChanged playerAttributeOnChanged;

	// Token: 0x020002B8 RID: 696
	[NetworkStructWeaved(240)]
	[StructLayout(LayoutKind.Explicit, Size = 960)]
	public struct PlayerInfo : INetworkStruct
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06001036 RID: 4150 RVA: 0x0005DBC7 File Offset: 0x0005BDC7
		// (set) Token: 0x06001037 RID: 4151 RVA: 0x0005DBD9 File Offset: 0x0005BDD9
		[Networked]
		public unsafe NetworkString<_32> NickName
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._NickName);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._NickName) = value;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x0005DBEC File Offset: 0x0005BDEC
		[Networked]
		public unsafe NetworkDictionary<NetworkString<_32>, NetworkString<_32>> properties
		{
			get
			{
				return new NetworkDictionary<NetworkString<_32>, NetworkString<_32>>((int*)Native.ReferenceToPointer<FixedStorage@207>(ref this._properties), 3, ReaderWriter@Fusion_NetworkString.GetInstance(), ReaderWriter@Fusion_NetworkString.GetInstance());
			}
		}

		// Token: 0x0400187E RID: 6270
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _NickName;

		// Token: 0x0400187F RID: 6271
		[FixedBufferProperty(typeof(NetworkDictionary<NetworkString<_32>, NetworkString<_32>>), typeof(UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString), 3, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@207 _properties;
	}

	// Token: 0x020002B9 RID: 697
	// (Invoke) Token: 0x0600103A RID: 4154
	public delegate void PlayerAttributeOnChanged();
}
