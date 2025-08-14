using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002F1 RID: 753
public class NetworkWrapper : MonoBehaviour
{
	// Token: 0x06001212 RID: 4626 RVA: 0x0006347F File Offset: 0x0006167F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void AutoInstantiate()
	{
		Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(Resources.Load<GameObject>("P_NetworkWrapper")));
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x00063498 File Offset: 0x00061698
	private void Awake()
	{
		if (this.titleRef != null)
		{
			this.titleRef.text = "PUN";
		}
		this.activeNetworkSystem = base.gameObject.AddComponent<NetworkSystemPUN>();
		this.activeNetworkSystem.AddVoiceSettings(this.VoiceSettings);
		this.activeNetworkSystem.config = this.netSysConfig;
		this.activeNetworkSystem.regionNames = this.networkRegionNames;
		this.activeNetworkSystem.OnPlayerJoined += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnPlayerLeft += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnMultiplayerStarted += this.UpdatePlayerCount;
		this.activeNetworkSystem.OnReturnedToSinglePlayer += this.UpdatePlayerCount;
		Debug.Log("<color=green>initialize Network System</color>");
		this.activeNetworkSystem.Initialise();
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x000635A4 File Offset: 0x000617A4
	private void UpdatePlayerCountWrapper(NetPlayer player)
	{
		this.UpdatePlayerCount();
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x000635AC File Offset: 0x000617AC
	private void UpdatePlayerCount()
	{
		if (this.playerCountTextRef == null)
		{
			return;
		}
		if (!this.activeNetworkSystem.IsOnline)
		{
			this.playerCountTextRef.text = string.Format("0/{0}", this.netSysConfig.MaxPlayerCount);
			Debug.Log("Player count updated");
			return;
		}
		Debug.Log("Player count not updated");
		this.playerCountTextRef.text = string.Format("{0}/{1}", this.activeNetworkSystem.AllNetPlayers.Length, this.netSysConfig.MaxPlayerCount);
	}

	// Token: 0x040019BC RID: 6588
	[HideInInspector]
	public NetworkSystem activeNetworkSystem;

	// Token: 0x040019BD RID: 6589
	public Text titleRef;

	// Token: 0x040019BE RID: 6590
	[Header("NetSys settings")]
	public NetworkSystemConfig netSysConfig;

	// Token: 0x040019BF RID: 6591
	public string[] networkRegionNames;

	// Token: 0x040019C0 RID: 6592
	public string[] devNetworkRegionNames;

	// Token: 0x040019C1 RID: 6593
	[Header("Debug output refs")]
	public Text stateTextRef;

	// Token: 0x040019C2 RID: 6594
	public Text playerCountTextRef;

	// Token: 0x040019C3 RID: 6595
	[SerializeField]
	private SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x040019C4 RID: 6596
	private const string WrapperResourcePath = "P_NetworkWrapper";
}
