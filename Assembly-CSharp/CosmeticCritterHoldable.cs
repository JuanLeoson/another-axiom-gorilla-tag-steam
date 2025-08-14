using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002212 RID: 8722 RVA: 0x000B89F7 File Offset: 0x000B6BF7
	// (set) Token: 0x06002213 RID: 8723 RVA: 0x000B89FF File Offset: 0x000B6BFF
	public int OwnerID { get; private set; }

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06002214 RID: 8724 RVA: 0x000B8A08 File Offset: 0x000B6C08
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000B8A15 File Offset: 0x000B6C15
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000B8A30 File Offset: 0x000B6C30
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000B8A43 File Offset: 0x000B6C43
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000B8A50 File Offset: 0x000B6C50
	private void TrySetID()
	{
		if (this.IsLocal)
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.transferrableObject.targetRig != null && this.transferrableObject.targetRig.creator != null)
		{
			string userId = this.transferrableObject.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000B8AFE File Offset: 0x000B6CFE
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000B8B2B File Offset: 0x000B6D2B
	protected virtual void OnEnable()
	{
		this.TrySetID();
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnDisable()
	{
	}

	// Token: 0x04002B94 RID: 11156
	protected TransferrableObject transferrableObject;

	// Token: 0x04002B96 RID: 11158
	protected CallLimiter callLimiter;
}
