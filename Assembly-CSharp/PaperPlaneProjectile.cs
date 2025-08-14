using System;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200049C RID: 1180
public class PaperPlaneProjectile : MonoBehaviour
{
	// Token: 0x14000049 RID: 73
	// (add) Token: 0x06001D32 RID: 7474 RVA: 0x0009CA3C File Offset: 0x0009AC3C
	// (remove) Token: 0x06001D33 RID: 7475 RVA: 0x0009CA74 File Offset: 0x0009AC74
	public event PaperPlaneProjectile.PaperPlaneHit OnHit;

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001D34 RID: 7476 RVA: 0x0009CAA9 File Offset: 0x0009ACA9
	public new Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001D35 RID: 7477 RVA: 0x0009CAB1 File Offset: 0x0009ACB1
	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0009CAB9 File Offset: 0x0009ACB9
	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x0009CAD3 File Offset: 0x0009ACD3
	private void Start()
	{
		this.ResetProjectile();
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x0009CADB File Offset: 0x0009ACDB
	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x0009CB00 File Offset: 0x0009AD00
	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(vel.sqrMagnitude / 2f, this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0009CBC4 File Offset: 0x0009ADC4
	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x0009CDB6 File Offset: 0x0009AFB6
	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x0400259F RID: 9631
	private const float speedScaleRatio = 0.7f;

	// Token: 0x040025A0 RID: 9632
	public Vector3 startPos;

	// Token: 0x040025A1 RID: 9633
	public Vector3 endPos;

	// Token: 0x040025A2 RID: 9634
	[FormerlySerializedAs("_flyTimeOut")]
	[Range(1f, 128f)]
	public float flyTimeOut = 32f;

	// Token: 0x040025A4 RID: 9636
	[Space]
	public float curveTime = 0.4f;

	// Token: 0x040025A5 RID: 9637
	[Space]
	public Vector3 curveDirection;

	// Token: 0x040025A6 RID: 9638
	public float curveDistance = 9.8f;

	// Token: 0x040025A7 RID: 9639
	[Space]
	[NonSerialized]
	private float _timeElapsed;

	// Token: 0x040025A8 RID: 9640
	[NonSerialized]
	private float _speed;

	// Token: 0x040025A9 RID: 9641
	[NonSerialized]
	private Vector3 _direction;

	// Token: 0x040025AA RID: 9642
	[NonSerialized]
	private bool _stopped;

	// Token: 0x040025AB RID: 9643
	private Transform _tCached;

	// Token: 0x040025AC RID: 9644
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x040025AD RID: 9645
	private Vector3 nextPos;

	// Token: 0x040025AE RID: 9646
	private RaycastHit[] results = new RaycastHit[1];

	// Token: 0x040025AF RID: 9647
	[SerializeField]
	private float maxFlightTime = 7.5f;

	// Token: 0x040025B0 RID: 9648
	[SerializeField]
	private float minFlightTime = 0.5f;

	// Token: 0x040025B1 RID: 9649
	[SerializeField]
	private float maxSpeed = 10f;

	// Token: 0x040025B2 RID: 9650
	[SerializeField]
	private float minSpeed = 1f;

	// Token: 0x040025B3 RID: 9651
	[SerializeField]
	private bool enableRotation;

	// Token: 0x040025B4 RID: 9652
	[SerializeField]
	private GameObject flyingObject;

	// Token: 0x040025B5 RID: 9653
	[SerializeField]
	private GameObject crashingObject;

	// Token: 0x040025B6 RID: 9654
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x040025B7 RID: 9655
	private VRRig myRig;

	// Token: 0x040025B8 RID: 9656
	private float scaleFactor;

	// Token: 0x0200049D RID: 1181
	// (Invoke) Token: 0x06001D3E RID: 7486
	public delegate void PaperPlaneHit(Vector3 endPoint);
}
