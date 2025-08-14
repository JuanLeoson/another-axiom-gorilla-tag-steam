using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000772 RID: 1906
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002FBD RID: 12221 RVA: 0x000FBAC4 File Offset: 0x000F9CC4
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06002FBE RID: 12222 RVA: 0x000FBB04 File Offset: 0x000F9D04
	public SizeChanger.ChangerType MyType
	{
		get
		{
			return this.myType;
		}
	}

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06002FBF RID: 12223 RVA: 0x000FBB0C File Offset: 0x000F9D0C
	public float MaxScale
	{
		get
		{
			return this.maxScale;
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x000FBB14 File Offset: 0x000F9D14
	public float MinScale
	{
		get
		{
			return this.minScale;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x06002FC1 RID: 12225 RVA: 0x000FBB1C File Offset: 0x000F9D1C
	public Transform StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x06002FC2 RID: 12226 RVA: 0x000FBB24 File Offset: 0x000F9D24
	public Transform EndPos
	{
		get
		{
			return this.endPos;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06002FC3 RID: 12227 RVA: 0x000FBB2C File Offset: 0x000F9D2C
	public float StaticEasing
	{
		get
		{
			return this.staticEasing;
		}
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x000FBB34 File Offset: 0x000F9D34
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x000FBB58 File Offset: 0x000F9D58
	public void OnEnable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter += this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit += this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x000FBBD4 File Offset: 0x000F9DD4
	public void OnDisable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter -= this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit -= this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x000FBC4D File Offset: 0x000F9E4D
	public void AddEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerEnter;
		}
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x000FBC69 File Offset: 0x000F9E69
	public void RemoveEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerEnter;
		}
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x000FBC85 File Offset: 0x000F9E85
	public void AddExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x000FBCA1 File Offset: 0x000F9EA1
	public void RemoveExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x000FBCC0 File Offset: 0x000F9EC0
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.acceptRig(component);
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x000FBCFD File Offset: 0x000F9EFD
	public void acceptRig(VRRig rig)
	{
		if (!rig.sizeManager.touchingChangers.Contains(this))
		{
			rig.sizeManager.touchingChangers.Add(this);
		}
		UnityAction onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter();
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x000FBD34 File Offset: 0x000F9F34
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.unacceptRig(component);
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000FBD71 File Offset: 0x000F9F71
	public void unacceptRig(VRRig rig)
	{
		rig.sizeManager.touchingChangers.Remove(this);
		UnityAction onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit();
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x000FBD98 File Offset: 0x000F9F98
	public Vector3 ClosestPoint(Vector3 position)
	{
		if (this.enterTrigger && this.exitTrigger)
		{
			Vector3 vector = this.enterTrigger.ClosestPoint(position);
			Vector3 vector2 = this.exitTrigger.ClosestPoint(position);
			if (Vector3.Distance(position, vector) >= Vector3.Distance(position, vector2))
			{
				return vector2;
			}
			return vector;
		}
		else
		{
			if (this.myCollider)
			{
				return this.myCollider.ClosestPoint(position);
			}
			return position;
		}
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x000FBE08 File Offset: 0x000FA008
	public void SetScaleCenterPoint(Transform centerPoint)
	{
		this.scaleAwayFromPoint = centerPoint;
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x000FBE11 File Offset: 0x000FA011
	public bool TryGetScaleCenterPoint(out Vector3 centerPoint)
	{
		if (this.scaleAwayFromPoint != null)
		{
			centerPoint = this.scaleAwayFromPoint.position;
			return true;
		}
		centerPoint = Vector3.zero;
		return false;
	}

	// Token: 0x04003BD2 RID: 15314
	[SerializeField]
	private SizeChanger.ChangerType myType;

	// Token: 0x04003BD3 RID: 15315
	[SerializeField]
	private float staticEasing;

	// Token: 0x04003BD4 RID: 15316
	[SerializeField]
	private float maxScale;

	// Token: 0x04003BD5 RID: 15317
	[SerializeField]
	private float minScale;

	// Token: 0x04003BD6 RID: 15318
	private Collider myCollider;

	// Token: 0x04003BD7 RID: 15319
	[SerializeField]
	private Transform startPos;

	// Token: 0x04003BD8 RID: 15320
	[SerializeField]
	private Transform endPos;

	// Token: 0x04003BD9 RID: 15321
	[SerializeField]
	private SizeChangerTrigger enterTrigger;

	// Token: 0x04003BDA RID: 15322
	[SerializeField]
	private SizeChangerTrigger exitTrigger;

	// Token: 0x04003BDB RID: 15323
	[SerializeField]
	private Transform scaleAwayFromPoint;

	// Token: 0x04003BDC RID: 15324
	[SerializeField]
	private SizeChangerTrigger exitOnEnterTrigger;

	// Token: 0x04003BDD RID: 15325
	public bool alwaysControlWhenEntered;

	// Token: 0x04003BDE RID: 15326
	public int priority;

	// Token: 0x04003BDF RID: 15327
	public bool aprilFoolsEnabled;

	// Token: 0x04003BE0 RID: 15328
	public float startRadius;

	// Token: 0x04003BE1 RID: 15329
	public float endRadius;

	// Token: 0x04003BE2 RID: 15330
	public bool affectLayerA = true;

	// Token: 0x04003BE3 RID: 15331
	public bool affectLayerB = true;

	// Token: 0x04003BE4 RID: 15332
	public bool affectLayerC = true;

	// Token: 0x04003BE5 RID: 15333
	public bool affectLayerD = true;

	// Token: 0x04003BE6 RID: 15334
	public UnityAction OnExit;

	// Token: 0x04003BE7 RID: 15335
	public UnityAction OnEnter;

	// Token: 0x04003BE8 RID: 15336
	private HashSet<VRRig> unregisteredPresentRigs;

	// Token: 0x02000773 RID: 1907
	public enum ChangerType
	{
		// Token: 0x04003BEA RID: 15338
		Static,
		// Token: 0x04003BEB RID: 15339
		Continuous,
		// Token: 0x04003BEC RID: 15340
		Radius
	}
}
