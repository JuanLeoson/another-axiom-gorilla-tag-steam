using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000744 RID: 1860
[RequireComponent(typeof(Collider))]
public class HandHold : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x1400005F RID: 95
	// (add) Token: 0x06002E90 RID: 11920 RVA: 0x000F6B58 File Offset: 0x000F4D58
	// (remove) Token: 0x06002E91 RID: 11921 RVA: 0x000F6B8C File Offset: 0x000F4D8C
	public static event HandHold.HandHoldPositionEvent HandPositionRequestOverride;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x06002E92 RID: 11922 RVA: 0x000F6BC0 File Offset: 0x000F4DC0
	// (remove) Token: 0x06002E93 RID: 11923 RVA: 0x000F6BF4 File Offset: 0x000F4DF4
	public static event HandHold.HandHoldEvent HandPositionReleaseOverride;

	// Token: 0x06002E94 RID: 11924 RVA: 0x000F6C28 File Offset: 0x000F4E28
	public void OnDisable()
	{
		for (int i = 0; i < this.currentGrabbers.Count; i++)
		{
			if (this.currentGrabbers[i].IsNotNull())
			{
				this.currentGrabbers[i].Ungrab(this);
			}
		}
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000F6C70 File Offset: 0x000F4E70
	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.myTappable = base.GetComponent<Tappable>();
		this.myCollider = base.GetComponent<Collider>();
		this.initialized = true;
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x000F6C9C File Offset: 0x000F4E9C
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		this.Initialize();
		grabbedTransform = base.transform;
		Vector3 position = g.transform.position;
		localGrabbedPosition = base.transform.InverseTransformPoint(position);
		Vector3 arg;
		g.Player.AddHandHold(base.transform, localGrabbedPosition, g, g.IsRightHand, this.rotatePlayerWhenHeld, out arg);
		this.currentGrabbers.AddIfNew(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionRequestOverride != null)
		{
			HandHold.HandPositionRequestOverride(this, g.IsRightHand, this.CalculateOffset(position));
		}
		UnityEvent<Vector3> onGrab = this.OnGrab;
		if (onGrab != null)
		{
			onGrab.Invoke(arg);
		}
		UnityEvent<HandHold> onGrabHandHold = this.OnGrabHandHold;
		if (onGrabHandHold != null)
		{
			onGrabHandHold.Invoke(this);
		}
		UnityEvent<bool> onGrabHanded = this.OnGrabHanded;
		if (onGrabHanded != null)
		{
			onGrabHanded.Invoke(g.IsRightHand);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnGrab();
		}
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x000F6D84 File Offset: 0x000F4F84
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		this.Initialize();
		g.Player.RemoveHandHold(g, g.IsRightHand);
		this.currentGrabbers.Remove(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionReleaseOverride != null)
		{
			HandHold.HandPositionReleaseOverride(this, g.IsRightHand);
		}
		UnityEvent onRelease = this.OnRelease;
		if (onRelease != null)
		{
			onRelease.Invoke();
		}
		UnityEvent<HandHold> onReleaseHandHold = this.OnReleaseHandHold;
		if (onReleaseHandHold != null)
		{
			onReleaseHandHold.Invoke(this);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnRelease();
		}
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000F6E14 File Offset: 0x000F5014
	private Vector3 CalculateOffset(Vector3 position)
	{
		switch (this.handSnapMethod)
		{
		case HandHold.HandSnapMethod.SnapToNearestEdge:
			if (this.myCollider == null)
			{
				this.myCollider = base.GetComponent<Collider>();
				if (this.myCollider is MeshCollider && !(this.myCollider as MeshCollider).convex)
				{
					this.handSnapMethod = HandHold.HandSnapMethod.None;
					return Vector3.zero;
				}
			}
			return base.transform.position - this.myCollider.ClosestPoint(position);
		case HandHold.HandSnapMethod.SnapToXAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.right * base.transform.InverseTransformPoint(position).x);
		case HandHold.HandSnapMethod.SnapToYAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.up * base.transform.InverseTransformPoint(position).y);
		case HandHold.HandSnapMethod.SnapToZAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.forward * base.transform.InverseTransformPoint(position).z);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000F6F52 File Offset: 0x000F5152
	public bool MomentaryGrabOnly()
	{
		return this.forceMomentary;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000F6F5A File Offset: 0x000F515A
	public void CopyProperties(HandHoldSettings handHoldSettings)
	{
		this.handSnapMethod = (HandHold.HandSnapMethod)handHoldSettings.handSnapMethod;
		this.rotatePlayerWhenHeld = handHoldSettings.rotatePlayerWhenHeld;
		this.forceMomentary = !handHoldSettings.allowPreGrab;
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000139A7 File Offset: 0x00011BA7
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04003A88 RID: 14984
	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	// Token: 0x04003A89 RID: 14985
	[SerializeField]
	private HandHold.HandSnapMethod handSnapMethod;

	// Token: 0x04003A8A RID: 14986
	[SerializeField]
	private bool rotatePlayerWhenHeld;

	// Token: 0x04003A8B RID: 14987
	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	// Token: 0x04003A8C RID: 14988
	[SerializeField]
	private UnityEvent<HandHold> OnGrabHandHold;

	// Token: 0x04003A8D RID: 14989
	[SerializeField]
	private UnityEvent<bool> OnGrabHanded;

	// Token: 0x04003A8E RID: 14990
	[SerializeField]
	private UnityEvent OnRelease;

	// Token: 0x04003A8F RID: 14991
	[SerializeField]
	private UnityEvent<HandHold> OnReleaseHandHold;

	// Token: 0x04003A90 RID: 14992
	private bool initialized;

	// Token: 0x04003A91 RID: 14993
	private Collider myCollider;

	// Token: 0x04003A92 RID: 14994
	private Tappable myTappable;

	// Token: 0x04003A93 RID: 14995
	[Tooltip("Turning this on disables \"pregrabbing\". Use pregrabbing to allow players to catch a handhold even if they have squeezed the trigger too soon. Useful if you're anticipating jumping players needed to grab while airborne")]
	[SerializeField]
	private bool forceMomentary = true;

	// Token: 0x04003A94 RID: 14996
	private List<GorillaGrabber> currentGrabbers = new List<GorillaGrabber>();

	// Token: 0x02000745 RID: 1861
	private enum HandSnapMethod
	{
		// Token: 0x04003A96 RID: 14998
		None,
		// Token: 0x04003A97 RID: 14999
		SnapToCenterPoint,
		// Token: 0x04003A98 RID: 15000
		SnapToNearestEdge,
		// Token: 0x04003A99 RID: 15001
		SnapToXAxisPoint,
		// Token: 0x04003A9A RID: 15002
		SnapToYAxisPoint,
		// Token: 0x04003A9B RID: 15003
		SnapToZAxisPoint
	}

	// Token: 0x02000746 RID: 1862
	// (Invoke) Token: 0x06002E9F RID: 11935
	public delegate void HandHoldPositionEvent(HandHold hh, bool rh, Vector3 pos);

	// Token: 0x02000747 RID: 1863
	// (Invoke) Token: 0x06002EA3 RID: 11939
	public delegate void HandHoldEvent(HandHold hh, bool rh);
}
