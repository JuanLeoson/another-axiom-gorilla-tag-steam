using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000B3A RID: 2874
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x17000683 RID: 1667
	// (get) Token: 0x06004520 RID: 17696 RVA: 0x00158BA3 File Offset: 0x00156DA3
	// (set) Token: 0x06004521 RID: 17697 RVA: 0x00158BAB File Offset: 0x00156DAB
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06004522 RID: 17698 RVA: 0x00158BB4 File Offset: 0x00156DB4
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x00158BC6 File Offset: 0x00156DC6
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x06004524 RID: 17700 RVA: 0x00158BDA File Offset: 0x00156DDA
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x00158C0F File Offset: 0x00156E0F
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x00158C26 File Offset: 0x00156E26
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x00158C40 File Offset: 0x00156E40
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x00158C53 File Offset: 0x00156E53
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x00158C70 File Offset: 0x00156E70
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x00158C80 File Offset: 0x00156E80
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x00158CDF File Offset: 0x00156EDF
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x04004F58 RID: 20312
	public float respawnTimerDuration;

	// Token: 0x04004F5A RID: 20314
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04004F5B RID: 20315
	private float _respawnTimestamp;

	// Token: 0x04004F5C RID: 20316
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x04004F5D RID: 20317
	private NetworkView netView;

	// Token: 0x04004F5E RID: 20318
	private Vector3 respawnAtPos;

	// Token: 0x04004F5F RID: 20319
	private Quaternion respawnAtRot;

	// Token: 0x04004F60 RID: 20320
	private Coroutine respawnTimer;
}
