using System;
using GorillaLocomotion.Gameplay;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x0200028F RID: 655
public class LckDirectGrabbable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000F00 RID: 3840 RVA: 0x00059CCC File Offset: 0x00057ECC
	// (remove) Token: 0x06000F01 RID: 3841 RVA: 0x00059D04 File Offset: 0x00057F04
	public event Action onGrabbed;

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000F02 RID: 3842 RVA: 0x00059D3C File Offset: 0x00057F3C
	// (remove) Token: 0x06000F03 RID: 3843 RVA: 0x00059D74 File Offset: 0x00057F74
	public event Action onReleased;

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000F04 RID: 3844 RVA: 0x00059DA9 File Offset: 0x00057FA9
	public GorillaGrabber grabber
	{
		get
		{
			return this._grabber;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000F05 RID: 3845 RVA: 0x00059DB1 File Offset: 0x00057FB1
	public bool isGrabbed
	{
		get
		{
			return this._grabber != null;
		}
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x00059DBF File Offset: 0x00057FBF
	public Vector3 GetLocalGrabbedPosition(GorillaGrabber grabber)
	{
		if (grabber == null)
		{
			return Vector3.zero;
		}
		return base.transform.InverseTransformPoint(grabber.transform.position);
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x00059DE6 File Offset: 0x00057FE6
	public bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return this._grabber == null || grabber == this._grabber;
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x00059E04 File Offset: 0x00058004
	public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		if (!base.isActiveAndEnabled)
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		if (this._grabber != null && this._grabber != grabber)
		{
			this.ForceRelease();
		}
		bool flag;
		bool flag2;
		if (this._precise && this.IsSlingshotHeldInHand(out flag, out flag2) && ((grabber.XrNode == XRNode.LeftHand && flag) || (grabber.XrNode == XRNode.RightHand && flag2)))
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		this._grabber = grabber;
		GtColliderTriggerProcessor.CurrentGrabbedHand = grabber.XrNode;
		GtColliderTriggerProcessor.IsGrabbingTablet = true;
		grabbedTransform = base.transform;
		localGrabbedPosition = this.GetLocalGrabbedPosition(this._grabber);
		this.target.SetParent(grabber.transform, true);
		Action action = this.onGrabbed;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletGrabbed = this.OnTabletGrabbed;
		if (onTabletGrabbed == null)
		{
			return;
		}
		onTabletGrabbed.Invoke();
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x00059F04 File Offset: 0x00058104
	public void OnGrabReleased(GorillaGrabber grabber)
	{
		this.target.transform.SetParent(this._originalTargetParent, true);
		this._grabber = null;
		GtColliderTriggerProcessor.IsGrabbingTablet = false;
		Action action = this.onReleased;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletReleased = this.OnTabletReleased;
		if (onTabletReleased == null)
		{
			return;
		}
		onTabletReleased.Invoke();
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x00059F56 File Offset: 0x00058156
	public void ForceGrab(GorillaGrabber grabber)
	{
		grabber.Inject(base.transform, this.GetLocalGrabbedPosition(grabber));
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x00059F6B File Offset: 0x0005816B
	public void ForceRelease()
	{
		if (this._grabber == null)
		{
			return;
		}
		this._grabber.Inject(null, Vector3.zero);
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x00059F90 File Offset: 0x00058190
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		if (rig == null || rig.projectileWeapon == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = rig.projectileWeapon.InLeftHand();
		rightHand = rig.projectileWeapon.InRightHand();
		return rig.projectileWeapon.InHand();
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00059FF1 File Offset: 0x000581F1
	public void SetOriginalTargetParent(Transform parent)
	{
		this._originalTargetParent = parent;
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool MomentaryGrabOnly()
	{
		return true;
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x000139A7 File Offset: 0x00011BA7
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x040017DB RID: 6107
	public UnityEvent OnTabletGrabbed = new UnityEvent();

	// Token: 0x040017DC RID: 6108
	public UnityEvent OnTabletReleased = new UnityEvent();

	// Token: 0x040017DD RID: 6109
	[SerializeField]
	private Transform _originalTargetParent;

	// Token: 0x040017DE RID: 6110
	public Transform target;

	// Token: 0x040017DF RID: 6111
	[SerializeField]
	private bool _precise;

	// Token: 0x040017E0 RID: 6112
	private GorillaGrabber _grabber;
}
