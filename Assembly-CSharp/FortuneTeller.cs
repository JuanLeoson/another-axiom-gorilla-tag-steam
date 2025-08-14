using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000595 RID: 1429
public class FortuneTeller : MonoBehaviourPunCallbacks
{
	// Token: 0x060022CD RID: 8909 RVA: 0x000BC10C File Offset: 0x000BA30C
	private void Awake()
	{
		if (this.changeMaterialsInGreyZone && GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Combine(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Combine(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000BC180 File Offset: 0x000BA380
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Remove(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Remove(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000BC1EC File Offset: 0x000BA3EC
	public override void OnEnable()
	{
		base.OnEnable();
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		if (this.button)
		{
			this.button.onPressed += this.HandlePressedButton;
		}
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000BC22A File Offset: 0x000BA42A
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.button)
		{
			this.button.onPressed -= this.HandlePressedButton;
		}
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000BC256 File Offset: 0x000BA456
	private void GreyZoneActivated()
	{
		this.boothRenderer.material = this.boothGreyZoneMaterial;
		this.beardRenderer.material = this.beardGreyZoneMaterial;
		this.tellerRenderer.SetMaterials(this.tellerGreyZoneMaterials);
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000BC28B File Offset: 0x000BA48B
	private void GreyZoneDeactivated()
	{
		this.boothRenderer.material = this.boothDefaultMaterial;
		this.beardRenderer.material = this.beardDefaultMaterial;
		this.tellerRenderer.SetMaterials(this.tellerDefaultMaterials);
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000BC2C0 File Offset: 0x000BA4C0
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			base.photonView.RPC("TriggerUpdateFortuneRPC", newPlayer, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000BC324 File Offset: 0x000BA524
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000BC324 File Offset: 0x000BA524
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000BC333 File Offset: 0x000BA533
	private void HandlePressedButton(GorillaPressableButton button, bool isLeft)
	{
		if (base.photonView.IsMine)
		{
			this.SendNewFortune();
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("RequestFortuneRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000BC368 File Offset: 0x000BA568
	[PunRPC]
	private void RequestFortuneRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestFortune");
		RigContainer rigContainer;
		if (info.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			CallLimitType<CallLimiter> callLimitType = rigContainer.Rig.fxSettings.callSettings[(int)this.limiterType];
			if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
			{
				this.SendNewFortune();
			}
		}
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000BC3E8 File Offset: 0x000BA5E8
	private void SendNewFortune()
	{
		if (this.playable.time > 0.0 && this.playable.time < this.playable.duration)
		{
			return;
		}
		this.latestFortune = this.results.GetResult();
		this.UpdateFortune(this.latestFortune, true);
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("TriggerNewFortuneRPC", RpcTarget.Others, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000BC488 File Offset: 0x000BA688
	[PunRPC]
	private void TriggerUpdateFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerUpdateFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerUpdateFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerUpdateFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.UpdateFortune(this.latestFortune, false);
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000BC504 File Offset: 0x000BA704
	[PunRPC]
	private void TriggerNewFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerNewFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerNewFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerNewFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		this.UpdateFortune(this.latestFortune, true);
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000BC590 File Offset: 0x000BA790
	private void StartAttractModeMonitor()
	{
		if (this.attractModeMonitor == null)
		{
			this.attractModeMonitor = base.StartCoroutine(this.AttractModeMonitor());
		}
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000BC5AC File Offset: 0x000BA7AC
	private IEnumerator AttractModeMonitor()
	{
		while (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			if (Time.time >= this.nextAttractAnimTimestamp)
			{
				this.SendAttractAnim();
			}
			yield return new WaitForSeconds(this.nextAttractAnimTimestamp - Time.time);
		}
		this.attractModeMonitor = null;
		yield break;
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000BC5BB File Offset: 0x000BA7BB
	private void SendAttractAnim()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			base.photonView.RPC("TriggerAttractAnimRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000BC5E4 File Offset: 0x000BA7E4
	[PunRPC]
	private void TriggerAttractAnimRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerAttractAnim");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerAttractAnim when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.animator.SetTrigger(this.trigger_attract);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000BC65C File Offset: 0x000BA85C
	private void UpdateFortune(FortuneResults.FortuneResult result, bool newFortune)
	{
		if (this.results)
		{
			PlayableAsset resultFanfare = this.GetResultFanfare(result.fortuneType);
			if (resultFanfare)
			{
				this.playable.initialTime = (newFortune ? 0.0 : resultFanfare.duration);
				this.playable.Play(resultFanfare, DirectorWrapMode.Hold);
				this.animator.SetTrigger(this.trigger_prediction);
				this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
			}
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000BC6DF File Offset: 0x000BA8DF
	public void ApplyFortuneText()
	{
		this.text.text = this.results.GetResultText(this.latestFortune).ToUpper();
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x000BC704 File Offset: 0x000BA904
	private PlayableAsset GetResultFanfare(FortuneResults.FortuneCategoryType fortuneType)
	{
		foreach (FortuneTeller.FortuneTellerResultFanfare fortuneTellerResultFanfare in this.resultFanfares)
		{
			if (fortuneTellerResultFanfare.type == fortuneType)
			{
				return fortuneTellerResultFanfare.fanfare;
			}
		}
		return null;
	}

	// Token: 0x04002C78 RID: 11384
	[SerializeField]
	private FXType limiterType;

	// Token: 0x04002C79 RID: 11385
	[SerializeField]
	private FortuneTellerButton button;

	// Token: 0x04002C7A RID: 11386
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x04002C7B RID: 11387
	[SerializeField]
	private FortuneResults results;

	// Token: 0x04002C7C RID: 11388
	[SerializeField]
	private PlayableDirector playable;

	// Token: 0x04002C7D RID: 11389
	[SerializeField]
	private Animator animator;

	// Token: 0x04002C7E RID: 11390
	[SerializeField]
	private float waitDurationBeforeAttractAnim;

	// Token: 0x04002C7F RID: 11391
	[SerializeField]
	private FortuneTeller.FortuneTellerResultFanfare[] resultFanfares;

	// Token: 0x04002C80 RID: 11392
	[Header("Grey Zone Visuals")]
	[SerializeField]
	private bool changeMaterialsInGreyZone;

	// Token: 0x04002C81 RID: 11393
	[SerializeField]
	private MeshRenderer boothRenderer;

	// Token: 0x04002C82 RID: 11394
	[SerializeField]
	private Material boothDefaultMaterial;

	// Token: 0x04002C83 RID: 11395
	[SerializeField]
	private Material boothGreyZoneMaterial;

	// Token: 0x04002C84 RID: 11396
	[SerializeField]
	private MeshRenderer beardRenderer;

	// Token: 0x04002C85 RID: 11397
	[SerializeField]
	private Material beardDefaultMaterial;

	// Token: 0x04002C86 RID: 11398
	[SerializeField]
	private Material beardGreyZoneMaterial;

	// Token: 0x04002C87 RID: 11399
	[SerializeField]
	private SkinnedMeshRenderer tellerRenderer;

	// Token: 0x04002C88 RID: 11400
	[SerializeField]
	private List<Material> tellerDefaultMaterials;

	// Token: 0x04002C89 RID: 11401
	[SerializeField]
	private List<Material> tellerGreyZoneMaterials;

	// Token: 0x04002C8A RID: 11402
	private FortuneResults.FortuneResult latestFortune;

	// Token: 0x04002C8B RID: 11403
	private CallLimiter triggerNewFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x04002C8C RID: 11404
	private CallLimiter triggerUpdateFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x04002C8D RID: 11405
	private AnimHashId trigger_attract = "Attract";

	// Token: 0x04002C8E RID: 11406
	private AnimHashId trigger_prediction = "Prediction";

	// Token: 0x04002C8F RID: 11407
	private float nextAttractAnimTimestamp;

	// Token: 0x04002C90 RID: 11408
	private Coroutine attractModeMonitor;

	// Token: 0x02000596 RID: 1430
	[Serializable]
	public struct FortuneTellerResultFanfare
	{
		// Token: 0x04002C91 RID: 11409
		public FortuneResults.FortuneCategoryType type;

		// Token: 0x04002C92 RID: 11410
		public PlayableAsset fanfare;
	}
}
