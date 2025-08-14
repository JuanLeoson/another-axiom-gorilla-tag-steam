using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200077D RID: 1917
public class Tappable : MonoBehaviour
{
	// Token: 0x0600300B RID: 12299 RVA: 0x000FC9EE File Offset: 0x000FABEE
	public void Validate()
	{
		this.CalculateId(true);
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x000FC9F7 File Offset: 0x000FABF7
	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x000FCA0E File Offset: 0x000FAC0E
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanTap(bool isLeftHand)
	{
		return true;
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000FCA18 File Offset: 0x000FAC18
	public void OnTap(float tapStrength)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, new object[]
		{
			this.tappableId,
			tapStrength
		});
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000FCA74 File Offset: 0x000FAC74
	public void OnGrab()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnGrabRPC", RpcTarget.All, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000FCAC8 File Offset: 0x000FACC8
	public void OnRelease()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnReleaseRPC", RpcTarget.All, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x000FC9EE File Offset: 0x000FABEE
	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x000FCB1C File Offset: 0x000FAD1C
	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
		int staticHash = base.GetType().Name.GetStaticHash();
		int hashCode2 = transform.position.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Compute(num, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000FCBE6 File Offset: 0x000FADE6
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	// Token: 0x04003C1F RID: 15391
	public int tappableId;

	// Token: 0x04003C20 RID: 15392
	public string staticId;

	// Token: 0x04003C21 RID: 15393
	public bool useStaticId;

	// Token: 0x04003C22 RID: 15394
	[Tooltip("If true, tap cooldown will be ignored.  Tapping will be allowed/disallowed based on result of CanTap()")]
	public bool overrideTapCooldown;

	// Token: 0x04003C23 RID: 15395
	[Space]
	public TappableManager manager;

	// Token: 0x04003C24 RID: 15396
	public RpcTarget rpcTarget;
}
