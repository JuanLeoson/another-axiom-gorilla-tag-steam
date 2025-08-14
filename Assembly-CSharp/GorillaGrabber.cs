using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200076D RID: 1901
public class GorillaGrabber : MonoBehaviour
{
	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06002F89 RID: 12169 RVA: 0x000FAE34 File Offset: 0x000F9034
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06002F8A RID: 12170 RVA: 0x000FAE3C File Offset: 0x000F903C
	public bool IsLeftHand
	{
		get
		{
			return this.XrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x06002F8B RID: 12171 RVA: 0x000FAE47 File Offset: 0x000F9047
	public bool IsRightHand
	{
		get
		{
			return this.XrNode == XRNode.RightHand;
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x06002F8C RID: 12172 RVA: 0x000FAE52 File Offset: 0x000F9052
	public GTPlayer Player
	{
		get
		{
			return this.player;
		}
	}

	// Token: 0x06002F8D RID: 12173 RVA: 0x000FAE5C File Offset: 0x000F905C
	private void Start()
	{
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnControllerUpdate));
		this.hapticStrengthActual = this.hapticStrength;
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<GTPlayer>();
		if (!this.player)
		{
			Debug.LogWarning("Gorilla Grabber Component has no player in hierarchy. Disabling this Gorilla Grabber");
			base.GetComponent<GorillaGrabber>().enabled = false;
		}
	}

	// Token: 0x06002F8E RID: 12174 RVA: 0x000FAEC4 File Offset: 0x000F90C4
	private void OnControllerUpdate()
	{
		bool grab = ControllerInputPoller.GetGrab(this.xrNode);
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab(null);
		}
		if (grabMomentary)
		{
			this.grabTimeStamp = Time.time;
		}
		if (grab && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.TryGrab(Time.time - this.grabTimeStamp < this.coyoteTimeDuration);
		}
		if (this.currentGrabbable != null && this.hapticStrengthActual > 0f)
		{
			GorillaTagger.Instance.DoVibration(this.xrNode, this.hapticStrengthActual, Time.deltaTime);
			this.hapticStrengthActual -= this.hapticDecay * Time.deltaTime;
		}
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x000FAF8D File Offset: 0x000F918D
	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.TransformPoint(this.localGrabbedPosition)) > this.breakDistance;
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x000FAFC8 File Offset: 0x000F91C8
	internal void Ungrab(IGorillaGrabable specificGrabbable = null)
	{
		if (specificGrabbable != null && specificGrabbable != this.currentGrabbable)
		{
			return;
		}
		this.currentGrabbable.OnGrabReleased(this);
		PlayerGameEvents.DroppedObject(this.currentGrabbable.name);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
		this.hapticStrengthActual = this.hapticStrength;
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x000FB01C File Offset: 0x000F921C
	private IGorillaGrabable TryGrab(bool momentary)
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * (this.grabRadius * this.player.scale), Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius * this.player.scale, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(out gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.FindClosestPoint(this.grabCastResults[i], base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null && (!gorillaGrabable.MomentaryGrabOnly() || momentary) && gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable.OnGrabbed(this, out this.currentGrabbedTransform, out this.localGrabbedPosition);
			PlayerGameEvents.GrabbedObject(gorillaGrabable.name);
		}
		if (gorillaGrabable != null && !gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable = null;
		}
		return gorillaGrabable;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x000FB12F File Offset: 0x000F932F
	private Vector3 FindClosestPoint(Collider collider, Vector3 position)
	{
		if (collider is MeshCollider && !(collider as MeshCollider).convex)
		{
			return position;
		}
		return collider.ClosestPoint(position);
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x000FB150 File Offset: 0x000F9350
	public void Inject(Transform currentGrabbableTransform, Vector3 localGrabbedPosition)
	{
		if (this.currentGrabbable != null)
		{
			this.Ungrab(null);
		}
		if (currentGrabbableTransform != null)
		{
			this.currentGrabbable = currentGrabbableTransform.GetComponent<IGorillaGrabable>();
			this.currentGrabbedTransform = currentGrabbableTransform;
			this.localGrabbedPosition = localGrabbedPosition;
			this.currentGrabbable.OnGrabbed(this, out this.currentGrabbedTransform, out localGrabbedPosition);
		}
	}

	// Token: 0x04003B8E RID: 15246
	private GTPlayer player;

	// Token: 0x04003B8F RID: 15247
	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	// Token: 0x04003B90 RID: 15248
	private AudioSource audioSource;

	// Token: 0x04003B91 RID: 15249
	private Transform currentGrabbedTransform;

	// Token: 0x04003B92 RID: 15250
	private Vector3 localGrabbedPosition;

	// Token: 0x04003B93 RID: 15251
	private IGorillaGrabable currentGrabbable;

	// Token: 0x04003B94 RID: 15252
	[SerializeField]
	private float grabRadius = 0.015f;

	// Token: 0x04003B95 RID: 15253
	[SerializeField]
	private float breakDistance = 0.3f;

	// Token: 0x04003B96 RID: 15254
	[SerializeField]
	private float hapticStrength = 0.2f;

	// Token: 0x04003B97 RID: 15255
	private float hapticStrengthActual = 0.2f;

	// Token: 0x04003B98 RID: 15256
	[SerializeField]
	private float hapticDecay;

	// Token: 0x04003B99 RID: 15257
	[SerializeField]
	private ParticleSystem gripEffects;

	// Token: 0x04003B9A RID: 15258
	private Collider[] grabCastResults = new Collider[32];

	// Token: 0x04003B9B RID: 15259
	private float grabTimeStamp;

	// Token: 0x04003B9C RID: 15260
	[SerializeField]
	private float coyoteTimeDuration = 0.25f;
}
