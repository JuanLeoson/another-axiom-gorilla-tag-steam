using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200050D RID: 1293
public class Breakable : MonoBehaviour
{
	// Token: 0x06001F87 RID: 8071 RVA: 0x000A69A0 File Offset: 0x000A4BA0
	private void Awake()
	{
		this._breakSignal.OnSignal += this.BreakRPC;
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000A69BC File Offset: 0x000A4BBC
	private void BreakRPC(int owner, PhotonSignalInfo info)
	{
		VRRig vrrig = base.GetComponent<OwnerRig>();
		if (vrrig == null)
		{
			return;
		}
		if (vrrig.OwningNetPlayer.ActorNumber != owner)
		{
			return;
		}
		this.OnBreak(true, false);
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000A69F8 File Offset: 0x000A4BF8
	private void Setup()
	{
		if (this._collider == null)
		{
			SphereCollider collider;
			this.GetOrAddComponent(out collider);
			this._collider = collider;
		}
		this._collider.enabled = true;
		if (this._rigidbody == null)
		{
			this.GetOrAddComponent(out this._rigidbody);
		}
		this._rigidbody.isKinematic = false;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.UpdatePhysMasks();
		if (this.rendererRoot == null)
		{
			this._renderers = base.GetComponentsInChildren<Renderer>();
			return;
		}
		this._renderers = this.rendererRoot.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000A6A9F File Offset: 0x000A4C9F
	private void OnCollisionEnter(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x000A6A9F File Offset: 0x000A4C9F
	private void OnCollisionStay(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000A6A9F File Offset: 0x000A4C9F
	private void OnTriggerEnter(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000A6A9F File Offset: 0x000A4C9F
	private void OnTriggerStay(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x000A6AA9 File Offset: 0x000A4CA9
	private void OnEnable()
	{
		this._breakSignal.Enable();
		this._broken = false;
		this.OnSpawn(true);
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x000A6AC4 File Offset: 0x000A4CC4
	private void OnDisable()
	{
		this._breakSignal.Disable();
		this._broken = false;
		this.OnReset(false);
		this.ShowRenderers(false);
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x000A6A9F File Offset: 0x000A4C9F
	public void Break()
	{
		this.OnBreak(true, true);
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000A6AE6 File Offset: 0x000A4CE6
	public void Reset()
	{
		this.OnReset(true);
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000A6AF0 File Offset: 0x000A4CF0
	protected virtual void ShowRenderers(bool visible)
	{
		if (this._renderers.IsNullOrEmpty<Renderer>())
		{
			return;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			Renderer renderer = this._renderers[i];
			if (renderer)
			{
				renderer.forceRenderingOff = !visible;
			}
		}
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000A6B3C File Offset: 0x000A4D3C
	protected virtual void OnReset(bool callback = true)
	{
		if (this._breakEffect && this._breakEffect.isPlaying)
		{
			this._breakEffect.Stop();
		}
		this.ShowRenderers(true);
		this._broken = false;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onReset;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000A6B90 File Offset: 0x000A4D90
	protected virtual void OnSpawn(bool callback = true)
	{
		this.startTime = Time.time;
		this.endTime = this.startTime + this.canBreakDelay;
		this.ShowRenderers(true);
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onSpawn;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x000A6BCC File Offset: 0x000A4DCC
	protected virtual void OnBreak(bool callback = true, bool signal = true)
	{
		if (this._broken)
		{
			return;
		}
		if (Time.time < this.endTime)
		{
			return;
		}
		if (this._breakEffect)
		{
			if (this._breakEffect.isPlaying)
			{
				this._breakEffect.Stop();
			}
			this._breakEffect.Play();
		}
		if (signal && PhotonNetwork.InRoom)
		{
			VRRig vrrig = base.GetComponent<OwnerRig>();
			if (vrrig != null)
			{
				this._breakSignal.Raise(vrrig.OwningNetPlayer.ActorNumber);
			}
		}
		this.ShowRenderers(false);
		this._broken = true;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onBreak;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x000A6C78 File Offset: 0x000A4E78
	private void UpdatePhysMasks()
	{
		int physicsMask = (int)this._physicsMask;
		if (this._collider)
		{
			this._collider.includeLayers = physicsMask;
			this._collider.excludeLayers = ~physicsMask;
		}
		if (this._rigidbody)
		{
			this._rigidbody.includeLayers = physicsMask;
			this._rigidbody.excludeLayers = ~physicsMask;
		}
	}

	// Token: 0x0400281B RID: 10267
	[SerializeField]
	private Collider _collider;

	// Token: 0x0400281C RID: 10268
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x0400281D RID: 10269
	[SerializeField]
	private GameObject rendererRoot;

	// Token: 0x0400281E RID: 10270
	[SerializeField]
	private Renderer[] _renderers = new Renderer[0];

	// Token: 0x0400281F RID: 10271
	[Space]
	[SerializeField]
	private ParticleSystem _breakEffect;

	// Token: 0x04002820 RID: 10272
	[SerializeField]
	private UnityLayerMask _physicsMask = UnityLayerMask.GorillaHand;

	// Token: 0x04002821 RID: 10273
	public UnityEvent<Breakable> onSpawn;

	// Token: 0x04002822 RID: 10274
	public UnityEvent<Breakable> onBreak;

	// Token: 0x04002823 RID: 10275
	public UnityEvent<Breakable> onReset;

	// Token: 0x04002824 RID: 10276
	public float canBreakDelay = 1f;

	// Token: 0x04002825 RID: 10277
	[SerializeField]
	private PhotonSignal<int> _breakSignal = "_breakSignal";

	// Token: 0x04002826 RID: 10278
	[Space]
	[NonSerialized]
	private bool _broken;

	// Token: 0x04002827 RID: 10279
	private float startTime;

	// Token: 0x04002828 RID: 10280
	private float endTime;
}
