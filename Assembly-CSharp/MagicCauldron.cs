using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusion;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000760 RID: 1888
[NetworkBehaviourWeaved(4)]
public class MagicCauldron : NetworkComponent
{
	// Token: 0x06002F49 RID: 12105 RVA: 0x000F9E38 File Offset: 0x000F8038
	private new void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x000F9F16 File Offset: 0x000F8116
	private new void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x000F9F1F File Offset: 0x000F811F
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x000F9F27 File Offset: 0x000F8127
	private IEnumerator LevitationSpellCoroutine()
	{
		GTPlayer.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		GTPlayer.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x000F9F38 File Offset: 0x000F8138
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(GTPlayer.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.GTPlayOneShot(this.recipes[this.currentRecipeIndex].successAudio, 1f);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.GTPlayOneShot(this.recipeFailedAudio, 1f);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x000FA130 File Offset: 0x000F8330
	private void UpdateState()
	{
		if (base.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x000FA1F9 File Offset: 0x000F83F9
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x000F9F16 File Offset: 0x000F8116
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x000FA202 File Offset: 0x000F8402
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x000FA214 File Offset: 0x000F8414
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPC_OnIngredientAdd(int _ingredientIndex, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", base.Object, 1);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = _ingredientIndex;
						num2 += 4;
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
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x000FA354 File Offset: 0x000F8554
	private void OnIngredientAddShared(int _ingredientIndex, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, info);
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x000FA3B8 File Offset: 0x000F85B8
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.GTPlayOneShot(this.ingredientAddedAudio, 1f);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x000FA4EC File Offset: 0x000F86EC
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (this.currentIngredients.SequenceEqual(recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x000FA564 File Offset: 0x000F8764
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x000FA620 File Offset: 0x000F8820
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.SendRPC("OnIngredientAdd", RpcTarget.Others, new object[]
				{
					num
				});
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x000FA6FC File Offset: 0x000F88FC
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06002F59 RID: 12121 RVA: 0x000FA715 File Offset: 0x000F8915
	// (set) Token: 0x06002F5A RID: 12122 RVA: 0x000FA73F File Offset: 0x000F893F
	[Networked]
	[NetworkedWeaved(0, 4)]
	private unsafe MagicCauldron.MagicCauldronData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MagicCauldron.MagicCauldronData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MagicCauldron.MagicCauldronData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x000FA76A File Offset: 0x000F896A
	public override void WriteDataFusion()
	{
		this.Data = new MagicCauldron.MagicCauldronData(this.currentStateElapsedTime, this.currentRecipeIndex, this.currentState, this.ingredientIndex);
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x000FA790 File Offset: 0x000F8990
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.CurrentStateElapsedTime, this.Data.CurrentRecipeIndex, this.Data.CurrentState, this.Data.IngredientIndex);
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x000FA7DC File Offset: 0x000F89DC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.currentStateElapsedTime);
		stream.SendNext(this.currentRecipeIndex);
		stream.SendNext(this.currentState);
		stream.SendNext(this.ingredientIndex);
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x000FA83C File Offset: 0x000F8A3C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float stateElapsedTime = (float)stream.ReceiveNext();
		int recipeIndex = (int)stream.ReceiveNext();
		MagicCauldron.CauldronState state = (MagicCauldron.CauldronState)stream.ReceiveNext();
		int num = (int)stream.ReceiveNext();
		this.ReadDataShared(stateElapsedTime, recipeIndex, state, num);
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x000FA894 File Offset: 0x000F8A94
	private void ReadDataShared(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
	{
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = stateElapsedTime;
		this.currentRecipeIndex = recipeIndex;
		this.currentState = state;
		this.ingredientIndex = ingredientIndex;
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && ingredientIndex != ingredientIndex && ingredientIndex >= 0 && ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[ingredientIndex].color);
		}
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x000FA991 File Offset: 0x000F8B91
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06002F62 RID: 12130 RVA: 0x000FA9A9 File Offset: 0x000F8BA9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x000FA9C0 File Offset: 0x000F8BC0
	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnIngredientAdd@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((MagicCauldron)behaviour).RPC_OnIngredientAdd(num3, info);
	}

	// Token: 0x04003B40 RID: 15168
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x04003B41 RID: 15169
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x04003B42 RID: 15170
	public float summonWitchesDuration = 20f;

	// Token: 0x04003B43 RID: 15171
	public float recipeFailedDuration = 5f;

	// Token: 0x04003B44 RID: 15172
	public float cooldownDuration = 30f;

	// Token: 0x04003B45 RID: 15173
	public MagicIngredientType[] allIngredients;

	// Token: 0x04003B46 RID: 15174
	public GameObject flyingWitchesContainer;

	// Token: 0x04003B47 RID: 15175
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003B48 RID: 15176
	public AudioClip ingredientAddedAudio;

	// Token: 0x04003B49 RID: 15177
	public AudioClip recipeFailedAudio;

	// Token: 0x04003B4A RID: 15178
	public ParticleSystem bubblesParticle;

	// Token: 0x04003B4B RID: 15179
	public ParticleSystem successParticle;

	// Token: 0x04003B4C RID: 15180
	public ParticleSystem splashParticle;

	// Token: 0x04003B4D RID: 15181
	public Color CauldronActiveColor;

	// Token: 0x04003B4E RID: 15182
	public Color CauldronFailedColor;

	// Token: 0x04003B4F RID: 15183
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x04003B50 RID: 15184
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x04003B51 RID: 15185
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x04003B52 RID: 15186
	private float currentStateElapsedTime;

	// Token: 0x04003B53 RID: 15187
	private MagicCauldron.CauldronState currentState;

	// Token: 0x04003B54 RID: 15188
	[SerializeField]
	private Renderer rendr;

	// Token: 0x04003B55 RID: 15189
	private Color cauldronColor;

	// Token: 0x04003B56 RID: 15190
	private Color currentColor;

	// Token: 0x04003B57 RID: 15191
	private int currentRecipeIndex;

	// Token: 0x04003B58 RID: 15192
	private int ingredientIndex;

	// Token: 0x04003B59 RID: 15193
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x04003B5A RID: 15194
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x04003B5B RID: 15195
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x04003B5C RID: 15196
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x04003B5D RID: 15197
	public bool testLevitationAlwaysOn;

	// Token: 0x04003B5E RID: 15198
	public float levitationRadius;

	// Token: 0x04003B5F RID: 15199
	public float levitationSpellDuration;

	// Token: 0x04003B60 RID: 15200
	public float levitationStrength;

	// Token: 0x04003B61 RID: 15201
	public float levitationDuration;

	// Token: 0x04003B62 RID: 15202
	public float levitationBlendOutDuration;

	// Token: 0x04003B63 RID: 15203
	public float levitationBonusStrength;

	// Token: 0x04003B64 RID: 15204
	public float levitationBonusOffAtYSpeed;

	// Token: 0x04003B65 RID: 15205
	public float levitationBonusFullAtYSpeed;

	// Token: 0x04003B66 RID: 15206
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 4)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MagicCauldron.MagicCauldronData _Data;

	// Token: 0x02000761 RID: 1889
	private enum CauldronState
	{
		// Token: 0x04003B68 RID: 15208
		notReady,
		// Token: 0x04003B69 RID: 15209
		ready,
		// Token: 0x04003B6A RID: 15210
		recipeCollecting,
		// Token: 0x04003B6B RID: 15211
		recipeActivated,
		// Token: 0x04003B6C RID: 15212
		summoned,
		// Token: 0x04003B6D RID: 15213
		failed,
		// Token: 0x04003B6E RID: 15214
		cooldown
	}

	// Token: 0x02000762 RID: 1890
	[Serializable]
	public struct Recipe
	{
		// Token: 0x04003B6F RID: 15215
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x04003B70 RID: 15216
		public AudioClip successAudio;
	}

	// Token: 0x02000763 RID: 1891
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x04003B71 RID: 15217
		public int key;
	}

	// Token: 0x02000764 RID: 1892
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06002F65 RID: 12133 RVA: 0x000FAA2F File Offset: 0x000F8C2F
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x000FAA37 File Offset: 0x000F8C37
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x04003B72 RID: 15218
		public FXSystemSettings playerSettings;

		// Token: 0x04003B73 RID: 15219
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x02000765 RID: 1893
		// (Invoke) Token: 0x06002F69 RID: 12137
		public delegate void Callback(int key);
	}

	// Token: 0x02000766 RID: 1894
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	private struct MagicCauldronData : INetworkStruct
	{
		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06002F6C RID: 12140 RVA: 0x000FAA4A File Offset: 0x000F8C4A
		// (set) Token: 0x06002F6D RID: 12141 RVA: 0x000FAA52 File Offset: 0x000F8C52
		public float CurrentStateElapsedTime { readonly get; set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x000FAA5B File Offset: 0x000F8C5B
		// (set) Token: 0x06002F6F RID: 12143 RVA: 0x000FAA63 File Offset: 0x000F8C63
		public int CurrentRecipeIndex { readonly get; set; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06002F70 RID: 12144 RVA: 0x000FAA6C File Offset: 0x000F8C6C
		// (set) Token: 0x06002F71 RID: 12145 RVA: 0x000FAA74 File Offset: 0x000F8C74
		public MagicCauldron.CauldronState CurrentState { readonly get; set; }

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06002F72 RID: 12146 RVA: 0x000FAA7D File Offset: 0x000F8C7D
		// (set) Token: 0x06002F73 RID: 12147 RVA: 0x000FAA85 File Offset: 0x000F8C85
		public int IngredientIndex { readonly get; set; }

		// Token: 0x06002F74 RID: 12148 RVA: 0x000FAA8E File Offset: 0x000F8C8E
		public MagicCauldronData(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
		{
			this.CurrentStateElapsedTime = stateElapsedTime;
			this.CurrentRecipeIndex = recipeIndex;
			this.CurrentState = state;
			this.IngredientIndex = ingredientIndex;
		}
	}
}
