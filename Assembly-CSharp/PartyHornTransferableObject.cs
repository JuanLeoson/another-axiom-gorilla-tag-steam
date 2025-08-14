using System;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000F1 RID: 241
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000619 RID: 1561 RVA: 0x00023676 File Offset: 0x00021876
	internal override void OnEnable()
	{
		base.OnEnable();
		this.localHead = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
		this.InitToDefault();
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000236A3 File Offset: 0x000218A3
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x000236AB File Offset: 0x000218AB
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000236BC File Offset: 0x000218BC
	protected Vector3 CalcMouthPiecePos()
	{
		if (!this.mouthPiece)
		{
			return base.transform.position + this.mouthPieceZOffset * base.transform.forward;
		}
		return this.mouthPiece.position;
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00023708 File Offset: 0x00021908
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 b = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius * GTPlayer.Instance.scale * GTPlayer.Instance.scale;
		bool flag = (this.localHead.TransformPoint(this.mouthOffset) - b).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = (myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : null;
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
		{
			VRRig vrrig = GorillaParent.instance.vrrigs[i];
			if (vrrig.head == null || vrrig.head.rigTarget == null || flag)
			{
				break;
			}
			flag = ((vrrig.head.rigTarget.transform.TransformPoint(this.mouthOffset) - b).sqrMagnitude < num);
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer rigContainer = vrrig.rigContainer;
					if (rigContainer == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = rigContainer.Voice;
						bool? flag3 = (voice != null) ? new bool?(voice.IsSpeaking) : null;
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x000238D8 File Offset: 0x00021AD8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			if (this.effectsGameObject)
			{
				this.effectsGameObject.SetActive(true);
			}
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00023960 File Offset: 0x00021B60
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.effectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04000735 RID: 1845
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x04000736 RID: 1846
	public float cooldown = 2f;

	// Token: 0x04000737 RID: 1847
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x04000738 RID: 1848
	public float mouthPieceRadius = 0.05f;

	// Token: 0x04000739 RID: 1849
	public Transform mouthPiece;

	// Token: 0x0400073A RID: 1850
	public Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x0400073B RID: 1851
	public bool soundActivated;

	// Token: 0x0400073C RID: 1852
	public UnityEvent OnCooldownStart;

	// Token: 0x0400073D RID: 1853
	public UnityEvent OnCooldownReset;

	// Token: 0x0400073E RID: 1854
	private float cooldownRemaining;

	// Token: 0x0400073F RID: 1855
	private Transform localHead;

	// Token: 0x04000740 RID: 1856
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x04000741 RID: 1857
	private bool localWasActivated;

	// Token: 0x020000F2 RID: 242
	private enum PartyHornState
	{
		// Token: 0x04000743 RID: 1859
		None = 1,
		// Token: 0x04000744 RID: 1860
		CoolingDown
	}
}
