using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000B5D RID: 2909
public class ZoneEntity : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000692 RID: 1682
	// (get) Token: 0x06004597 RID: 17815 RVA: 0x0015B940 File Offset: 0x00159B40
	public string entityTag
	{
		get
		{
			return this._entityTag;
		}
	}

	// Token: 0x17000693 RID: 1683
	// (get) Token: 0x06004598 RID: 17816 RVA: 0x0015B948 File Offset: 0x00159B48
	public int entityID
	{
		get
		{
			int value = this._entityID.GetValueOrDefault();
			if (this._entityID == null)
			{
				value = base.GetInstanceID();
				this._entityID = new int?(value);
			}
			return this._entityID.Value;
		}
	}

	// Token: 0x17000694 RID: 1684
	// (get) Token: 0x06004599 RID: 17817 RVA: 0x0015B98C File Offset: 0x00159B8C
	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	// Token: 0x17000695 RID: 1685
	// (get) Token: 0x0600459A RID: 17818 RVA: 0x0015B994 File Offset: 0x00159B94
	public SphereCollider collider
	{
		get
		{
			return this._collider;
		}
	}

	// Token: 0x17000696 RID: 1686
	// (get) Token: 0x0600459B RID: 17819 RVA: 0x0015B99C File Offset: 0x00159B9C
	public GroupJoinZoneAB GroupZone
	{
		get
		{
			return (this.currentGroupZone & ~this.currentExcludeGroupZone) | this.previousGroupZone;
		}
	}

	// Token: 0x0600459C RID: 17820 RVA: 0x0015B9BF File Offset: 0x00159BBF
	public virtual void OnEnable()
	{
		this.insideBoxes.Clear();
		ZoneGraph.Register(this);
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x0600459D RID: 17821 RVA: 0x0015B9D9 File Offset: 0x00159BD9
	public virtual void OnDisable()
	{
		this.insideBoxes.Clear();
		ZoneGraph.Unregister(this);
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x0600459E RID: 17822 RVA: 0x0015B9F4 File Offset: 0x00159BF4
	public void SliceUpdate()
	{
		if (this.layerMask == -1)
		{
			this.layerMask = LayerMask.GetMask(new string[]
			{
				"Zone"
			});
		}
		int num = Physics.OverlapSphereNonAlloc(base.transform.TransformPoint(this.collider.center), this.collider.radius * base.transform.lossyScale.x, this.colliders, this.layerMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < num; i++)
		{
			this.OnTriggerStayManualInvoke(this.colliders[i]);
		}
	}

	// Token: 0x0600459F RID: 17823 RVA: 0x0015BA91 File Offset: 0x00159C91
	public void EnableZoneChanges()
	{
		this._collider.enabled = true;
		if (this.disabledZoneChangesOnTriggerStayCoroutine != null)
		{
			base.StopCoroutine(this.disabledZoneChangesOnTriggerStayCoroutine);
			this.disabledZoneChangesOnTriggerStayCoroutine = null;
		}
	}

	// Token: 0x060045A0 RID: 17824 RVA: 0x0015BABA File Offset: 0x00159CBA
	public void DisableZoneChanges()
	{
		this._collider.enabled = false;
		if (this.insideBoxes.Count > 0 && this.disabledZoneChangesOnTriggerStayCoroutine == null)
		{
			this.disabledZoneChangesOnTriggerStayCoroutine = base.StartCoroutine(this.DisabledZoneCollider_OnTriggerStay());
		}
	}

	// Token: 0x060045A1 RID: 17825 RVA: 0x0015BAF0 File Offset: 0x00159CF0
	private IEnumerator DisabledZoneCollider_OnTriggerStay()
	{
		ZoneGraph instance = ZoneGraph.Instance;
		if (instance != null)
		{
			instance.CheckCompiledMaps();
		}
		for (;;)
		{
			foreach (BoxCollider c in this.insideBoxes)
			{
				this.OnTriggerStayManualInvoke(c);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060045A2 RID: 17826 RVA: 0x0015BAFF File Offset: 0x00159CFF
	protected virtual void OnTriggerEnter(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_enter, c);
	}

	// Token: 0x060045A3 RID: 17827 RVA: 0x0015BB09 File Offset: 0x00159D09
	protected virtual void OnTriggerExit(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_exit, c);
	}

	// Token: 0x060045A4 RID: 17828 RVA: 0x0015BB14 File Offset: 0x00159D14
	protected virtual void OnTriggerStayManualInvoke(Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		if (Time.time >= this.groupZoneClearAtTimestamp)
		{
			this.previousGroupZone = (this.currentGroupZone & ~this.currentExcludeGroupZone);
			this.currentGroupZone = zoneDef.groupZoneAB;
			this.currentExcludeGroupZone = zoneDef.excludeGroupZoneAB;
			this.groupZoneClearAtTimestamp = Time.time + this.groupZoneClearInterval;
		}
		else
		{
			this.currentGroupZone |= zoneDef.groupZoneAB;
			this.currentExcludeGroupZone |= zoneDef.excludeGroupZoneAB;
		}
		if (!this.gLastStayPoll.HasElapsed(1f, true))
		{
			return;
		}
		this.OnZoneTrigger(GTZoneEventType.zone_stay, boxCollider);
	}

	// Token: 0x060045A5 RID: 17829 RVA: 0x0015BBDC File Offset: 0x00159DDC
	protected virtual void OnZoneTrigger(GTZoneEventType zoneEvent, Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zone = ZoneGraph.ColliderToZoneDef(boxCollider);
		this.OnZoneTrigger(zoneEvent, zone, boxCollider);
	}

	// Token: 0x060045A6 RID: 17830 RVA: 0x0015BC0C File Offset: 0x00159E0C
	private void OnZoneTrigger(GTZoneEventType zoneEvent, ZoneDef zone, BoxCollider box)
	{
		bool flag = false;
		switch (zoneEvent)
		{
		case GTZoneEventType.zone_enter:
		{
			if (zone.zoneId != this.lastEnteredNode.zoneId)
			{
				this.sinceZoneEntered = 0;
			}
			this.lastEnteredNode = ZoneGraph.ColliderToNode(box);
			ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(box);
			this.insideBoxes.Add(box);
			if (zoneDef.priority > this.currentZonePriority)
			{
				this.currentZone = zone.zoneId;
				this.currentSubZone = zone.subZoneId;
				this.currentZonePriority = zoneDef.priority;
			}
			if (zone.subZoneId == GTSubZone.store_register)
			{
				GorillaTelemetry.PostShopEvent(this._entityRig, GTShopEventType.register_visit, CosmeticsController.instance.currentCart);
			}
			flag = zone.trackEnter;
			break;
		}
		case GTZoneEventType.zone_exit:
			this.lastExitedNode = ZoneGraph.ColliderToNode(box);
			this.insideBoxes.Remove(box);
			if (this.currentZone == this.lastExitedNode.zoneId)
			{
				int num = 0;
				ZoneDef zoneDef2 = null;
				foreach (BoxCollider collider in this.insideBoxes)
				{
					ZoneDef zoneDef3 = ZoneGraph.ColliderToZoneDef(collider);
					if (zoneDef3.priority > num)
					{
						zoneDef2 = zoneDef3;
						num = zoneDef3.priority;
					}
				}
				if (zoneDef2 != null)
				{
					this.currentZone = zoneDef2.zoneId;
					this.currentSubZone = zoneDef2.subZoneId;
					this.currentZonePriority = zoneDef2.priority;
				}
				else
				{
					this.currentZone = GTZone.none;
					this.currentSubZone = GTSubZone.none;
					this.currentZonePriority = 0;
				}
			}
			if (this.currentZone == GTZone.forest && this.currentSubZone == GTSubZone.tree_room)
			{
				zone.subZoneId = GTSubZone.none;
			}
			flag = zone.trackExit;
			break;
		case GTZoneEventType.zone_stay:
		{
			bool flag2 = this.sinceZoneEntered.secondsElapsedInt >= this._zoneStayEventInterval;
			if (flag2)
			{
				this.sinceZoneEntered = 0;
			}
			flag = (zone.trackStay && flag2);
			break;
		}
		}
		GorillaTelemetry.CurrentZone = zone.zoneId;
		GorillaTelemetry.CurrentSubZone = zone.subZoneId;
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!flag)
		{
			return;
		}
		if (!this._entityRig.isOfflineVRRig)
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zone.zoneId, zone.subZoneId, zoneEvent);
	}

	// Token: 0x060045A7 RID: 17831 RVA: 0x0015BE3C File Offset: 0x0015A03C
	public static int Compare<T>(T x, T y) where T : ZoneEntity
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		return x.entityID.CompareTo(y.entityID);
	}

	// Token: 0x040050A3 RID: 20643
	[Space]
	[NonSerialized]
	private int? _entityID;

	// Token: 0x040050A4 RID: 20644
	[SerializeField]
	private string _entityTag;

	// Token: 0x040050A5 RID: 20645
	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	// Token: 0x040050A6 RID: 20646
	[SerializeField]
	private int _zoneStayEventInterval = 300;

	// Token: 0x040050A7 RID: 20647
	[Space]
	[SerializeField]
	private VRRig _entityRig;

	// Token: 0x040050A8 RID: 20648
	[SerializeField]
	private SphereCollider _collider;

	// Token: 0x040050A9 RID: 20649
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040050AA RID: 20650
	[Space]
	[NonSerialized]
	public GTZone currentZone = GTZone.none;

	// Token: 0x040050AB RID: 20651
	[NonSerialized]
	public GTSubZone currentSubZone;

	// Token: 0x040050AC RID: 20652
	[NonSerialized]
	private GroupJoinZoneAB currentGroupZone = 0;

	// Token: 0x040050AD RID: 20653
	[NonSerialized]
	private GroupJoinZoneAB previousGroupZone = 0;

	// Token: 0x040050AE RID: 20654
	[NonSerialized]
	private GroupJoinZoneAB currentExcludeGroupZone = 0;

	// Token: 0x040050AF RID: 20655
	private HashSet<BoxCollider> insideBoxes = new HashSet<BoxCollider>();

	// Token: 0x040050B0 RID: 20656
	private int currentZonePriority;

	// Token: 0x040050B1 RID: 20657
	private float groupZoneClearAtTimestamp;

	// Token: 0x040050B2 RID: 20658
	private float groupZoneClearInterval = 0.1f;

	// Token: 0x040050B3 RID: 20659
	private Coroutine disabledZoneChangesOnTriggerStayCoroutine;

	// Token: 0x040050B4 RID: 20660
	[Space]
	[NonSerialized]
	public ZoneNode currentNode = ZoneNode.Null;

	// Token: 0x040050B5 RID: 20661
	[NonSerialized]
	public ZoneNode lastEnteredNode = ZoneNode.Null;

	// Token: 0x040050B6 RID: 20662
	[NonSerialized]
	public ZoneNode lastExitedNode = ZoneNode.Null;

	// Token: 0x040050B7 RID: 20663
	[Space]
	[NonSerialized]
	private TimeSince sinceZoneEntered = 0;

	// Token: 0x040050B8 RID: 20664
	private Collider[] colliders = new Collider[20];

	// Token: 0x040050B9 RID: 20665
	public const string ZONE_LAYER = "Zone";

	// Token: 0x040050BA RID: 20666
	private LayerMask layerMask = -1;

	// Token: 0x040050BB RID: 20667
	private TimeSince gLastStayPoll = 0;
}
