using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200015D RID: 349
public class PropHuntHandFollower : MonoBehaviour, ICallBack
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000948 RID: 2376 RVA: 0x000327E8 File Offset: 0x000309E8
	// (set) Token: 0x06000949 RID: 2377 RVA: 0x000327F0 File Offset: 0x000309F0
	public bool hasProp
	{
		get
		{
			return this._hasProp;
		}
		private set
		{
			this._hasProp = value;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x0600094A RID: 2378 RVA: 0x000327F9 File Offset: 0x000309F9
	// (set) Token: 0x0600094B RID: 2379 RVA: 0x00032801 File Offset: 0x00030A01
	public bool IsInstantiatingAsync { get; private set; }

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x0003280A File Offset: 0x00030A0A
	// (set) Token: 0x0600094D RID: 2381 RVA: 0x00032812 File Offset: 0x00030A12
	public VRRig attachedToRig { get; private set; }

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x0600094E RID: 2382 RVA: 0x0003281B File Offset: 0x00030A1B
	public bool IsLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00032823 File Offset: 0x00030A23
	public void Awake()
	{
		this.attachedToRig = base.GetComponent<VRRig>();
		this.attachedToRig.propHuntHandFollower = this;
		this._isLocal = this.attachedToRig.isOfflineVRRig;
		this.raycastHits = new RaycastHit[20];
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0003285B File Offset: 0x00030A5B
	public void Start()
	{
		this.attachedToRig.AddLateUpdateCallback(this);
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00032869 File Offset: 0x00030A69
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropHandFollower(this);
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00032871 File Offset: 0x00030A71
	private void OnDisable()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		this.DestroyProp();
		GorillaPropHuntGameManager.UnregisterPropHandFollower(this);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00032888 File Offset: 0x00030A88
	public void DestroyProp()
	{
		if (!this.hasProp || this._prop == null)
		{
			return;
		}
		PropHuntGrabbableProp prop;
		PropHuntTaggableProp prop2;
		if (this._prop.TryGetComponent<PropHuntGrabbableProp>(out prop))
		{
			PropHuntPools.ReturnGrabbableProp(prop);
		}
		else if (this._prop.TryGetComponent<PropHuntTaggableProp>(out prop2))
		{
			PropHuntPools.ReturnTaggableProp(prop2);
		}
		this._prop = null;
		this.hasProp = false;
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000328E8 File Offset: 0x00030AE8
	public static void DestroyProp_NoPool(List<MeshCollider> _colliders, ref bool hasProp, ref GameObject _prop)
	{
		foreach (MeshCollider meshCollider in _colliders)
		{
			if (!(meshCollider == null))
			{
				meshCollider.gameObject.transform.parent = null;
				meshCollider.gameObject.SetActive(false);
			}
		}
		if (hasProp)
		{
			Object.Destroy(_prop);
		}
		_prop = null;
		hasProp = false;
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnRoundStart()
	{
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00032968 File Offset: 0x00030B68
	public void CreateProp()
	{
		if (this.hasProp)
		{
			this.DestroyProp();
		}
		this._isLeftHand = false;
		int num = GorillaPropHuntGameManager.instance.GetSeed();
		if (NetworkSystem.Instance.InRoom)
		{
			num += this.attachedToRig.OwningNetPlayer.ActorNumber;
		}
		SRand srand = new SRand(num);
		string cosmeticId = GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt());
		PropHuntTaggableProp propHuntTaggableProp;
		if (this._isLocal)
		{
			PropHuntGrabbableProp propHuntGrabbableProp;
			if (PropHuntPools.TryGetGrabbableProp(cosmeticId, out propHuntGrabbableProp))
			{
				this._grabbableProp = propHuntGrabbableProp;
				this._taggableProp = null;
				this._prop = propHuntGrabbableProp.gameObject;
				this._propOffset = this._grabbableProp.offset;
				propHuntGrabbableProp.handFollower = this;
				this.hasProp = true;
				for (int i = 0; i < propHuntGrabbableProp.interactionPoints.Count; i++)
				{
					propHuntGrabbableProp.interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
		}
		else if (PropHuntPools.TryGetTaggableProp(cosmeticId, out propHuntTaggableProp))
		{
			this._taggableProp = propHuntTaggableProp;
			this._grabbableProp = null;
			this._prop = propHuntTaggableProp.gameObject;
			this._propOffset = propHuntTaggableProp.offset;
			propHuntTaggableProp.ownerRig = this.attachedToRig;
			this.hasProp = true;
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00032A98 File Offset: 0x00030C98
	public void OnPropLoaded(AsyncOperationHandle<GameObject> handle)
	{
		this.IsInstantiatingAsync = false;
		CosmeticSO debugCosmeticSO = null;
		if (PropHuntHandFollower.TryPrepPropTemplate(handle.Result, this._isLocal, debugCosmeticSO, this._colliders, this._interactionPoints, out this._grabbableProp, out this._taggableProp))
		{
			this._prop = handle.Result;
			this.hasProp = (this._prop != null);
			this._prop.SetActive(true);
			if (this._isLocal)
			{
				this._propOffset = this._grabbableProp.offset;
				this._grabbableProp.handFollower = this;
				for (int i = 0; i < this._interactionPoints.Count; i++)
				{
					this._interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
			this._propOffset = this._taggableProp.offset;
			this._taggableProp.ownerRig = this.attachedToRig;
		}
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00032B80 File Offset: 0x00030D80
	public static bool TryPrepPropTemplate(GameObject _prop, bool _isLocal, CosmeticSO debugCosmeticSO, List<MeshCollider> _colliders, List<InteractionPoint> ref_interactionPoints, out PropHuntGrabbableProp grabbableProp, out PropHuntTaggableProp taggableProp)
	{
		if (_isLocal)
		{
			grabbableProp = _prop.AddComponent<PropHuntGrabbableProp>();
			taggableProp = null;
			grabbableProp.interactionPoints = ref_interactionPoints;
		}
		else
		{
			taggableProp = _prop.AddComponent<PropHuntTaggableProp>();
			grabbableProp = null;
		}
		bool flag = false;
		bool flag2 = true;
		Bounds bounds = default(Bounds);
		int num = 0;
		foreach (MeshRenderer meshRenderer in _prop.GetComponentsInChildren<MeshRenderer>())
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if (!(component == null))
			{
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					if (flag2)
					{
						bounds = meshRenderer.bounds;
					}
					else
					{
						bounds.Encapsulate(meshRenderer.bounds);
					}
					MeshCollider meshCollider;
					if (num >= _colliders.Count)
					{
						GameObject gameObject = new GameObject("PropHuntTaggable");
						gameObject.layer = 14;
						meshCollider = gameObject.AddComponent<MeshCollider>();
						meshCollider.convex = true;
						meshCollider.isTrigger = true;
						if (_isLocal)
						{
							ref_interactionPoints.Add(gameObject.AddComponent<InteractionPoint>());
						}
						_colliders.Add(meshCollider);
					}
					else
					{
						meshCollider = _colliders[num];
						meshCollider.gameObject.SetActive(true);
					}
					meshCollider.transform.parent = _prop.transform;
					meshCollider.transform.position = meshRenderer.transform.position;
					meshCollider.transform.rotation = meshRenderer.transform.rotation;
					meshCollider.sharedMesh = sharedMesh;
					num++;
					flag2 = false;
				}
			}
		}
		if (!flag)
		{
			bool flag3 = true;
			PropHuntHandFollower.DestroyProp_NoPool(_colliders, ref flag3, ref _prop);
			return false;
		}
		Vector3 offset = _prop.transform.InverseTransformPoint(bounds.center);
		if (_isLocal)
		{
			grabbableProp.interactionPoints = ref_interactionPoints;
			grabbableProp.offset = offset;
		}
		else
		{
			taggableProp.offset = offset;
		}
		return true;
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00032D44 File Offset: 0x00030F44
	void ICallBack.CallBack()
	{
		if (!this.hasProp || this._prop.IsNull())
		{
			return;
		}
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		Vector3 sourcePos = transform.position;
		if (this.attachedToRig.isLocal)
		{
			sourcePos = (this._isLeftHand ? this.attachedToRig.leftHand.overrideTarget.position : this.attachedToRig.rightHand.overrideTarget.position);
		}
		if ((this._isLeftHand ? Mathf.Max(this.attachedToRig.leftIndex.calcT, this.attachedToRig.leftMiddle.calcT) : Mathf.Max(this.attachedToRig.rightIndex.calcT, this.attachedToRig.rightMiddle.calcT)) > 0.5f)
		{
			this._prop.transform.rotation = transform.TransformRotation(this._lastRelativeAngle);
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, transform.TransformPoint(this._lastRelativePos) + this._prop.transform.TransformVector(this._propOffset)) - this._prop.transform.TransformVector(this._propOffset);
			this._networkLastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
			this._networkLastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
			return;
		}
		Vector3 v = transform.transform.position - this._prop.transform.TransformPoint(this._propOffset);
		if (v.IsLongerThan(GorillaPropHuntGameManager.instance.HandFollowDistance))
		{
			float d = v.magnitude - GorillaPropHuntGameManager.instance.HandFollowDistance;
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, this._prop.transform.position + this._prop.transform.TransformVector(this._propOffset) + v.normalized * d) - this._prop.transform.TransformVector(this._propOffset);
		}
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
		this._networkLastRelativePos = this._lastRelativePos;
		this._networkLastRelativeAngle = this._lastRelativeAngle;
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00032FF8 File Offset: 0x000311F8
	public Vector3 GeoCollisionPoint(Vector3 sourcePos, Vector3 targetPos)
	{
		Vector3 vector = targetPos - sourcePos;
		int num = Physics.RaycastNonAlloc(sourcePos, vector.normalized, this.raycastHits, vector.magnitude, this.collisionLayers, QueryTriggerInteraction.Ignore);
		if (num > 0)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			Vector3 result = targetPos;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector2 = this.raycastHits[i].point - sourcePos;
				if (vector2.sqrMagnitude < sqrMagnitude)
				{
					result = this.raycastHits[i].point;
					sqrMagnitude = vector2.sqrMagnitude;
				}
			}
			return result;
		}
		return targetPos;
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00033094 File Offset: 0x00031294
	public void SwitchHand(bool newIsLeftHand)
	{
		if (this._isLeftHand == newIsLeftHand)
		{
			return;
		}
		this._isLeftHand = newIsLeftHand;
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00033115 File Offset: 0x00031315
	public void SetProp(bool isLeftHand, Vector3 propPos, Quaternion propRot)
	{
		this._isLeftHand = isLeftHand;
		this._lastRelativePos = propPos;
		this._lastRelativeAngle = propRot;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0003312C File Offset: 0x0003132C
	public long GetRelativePosRotLong()
	{
		if (this._prop.IsNull())
		{
			return BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity);
		}
		return BitPackUtils.PackHandPosRotForNetwork(this._lastRelativePos, this._lastRelativeAngle);
	}

	// Token: 0x04000AE8 RID: 2792
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x04000AE9 RID: 2793
	private const bool _k_isBetaOrEditor = false;

	// Token: 0x04000AEA RID: 2794
	private const float HandFollowDistance = 0.1f;

	// Token: 0x04000AEB RID: 2795
	private bool _hasProp;

	// Token: 0x04000AEE RID: 2798
	private bool _isLocal;

	// Token: 0x04000AEF RID: 2799
	private GameObject _prop;

	// Token: 0x04000AF0 RID: 2800
	private bool _isLeftHand;

	// Token: 0x04000AF1 RID: 2801
	private Vector3 _propOffset;

	// Token: 0x04000AF2 RID: 2802
	private readonly List<MeshCollider> _colliders = new List<MeshCollider>(4);

	// Token: 0x04000AF3 RID: 2803
	private readonly List<InteractionPoint> _interactionPoints = new List<InteractionPoint>(4);

	// Token: 0x04000AF4 RID: 2804
	private Vector3 _lastRelativePos;

	// Token: 0x04000AF5 RID: 2805
	private Quaternion _lastRelativeAngle;

	// Token: 0x04000AF6 RID: 2806
	private Vector3 _networkLastRelativePos;

	// Token: 0x04000AF7 RID: 2807
	private Quaternion _networkLastRelativeAngle;

	// Token: 0x04000AF8 RID: 2808
	public LayerMask collisionLayers;

	// Token: 0x04000AF9 RID: 2809
	private Vector3 targetPoint;

	// Token: 0x04000AFA RID: 2810
	private RaycastHit[] raycastHits;

	// Token: 0x04000AFB RID: 2811
	private PropHuntGrabbableProp _grabbableProp;

	// Token: 0x04000AFC RID: 2812
	private PropHuntTaggableProp _taggableProp;
}
