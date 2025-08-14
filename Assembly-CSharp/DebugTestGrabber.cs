using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class DebugTestGrabber : MonoBehaviour
{
	// Token: 0x060002E6 RID: 742 RVA: 0x000123ED File Offset: 0x000105ED
	private void Awake()
	{
		if (this.grabber == null)
		{
			this.grabber = base.GetComponentInChildren<CrittersGrabber>();
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0001240C File Offset: 0x0001060C
	private void LateUpdate()
	{
		if (this.transformToFollow != null)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position;
		}
		if (this.grabber == null)
		{
			return;
		}
		if (!this.isGrabbing && this.setIsGrabbing)
		{
			this.setIsGrabbing = false;
			this.isGrabbing = true;
			this.remainingGrabDuration = this.grabDuration;
		}
		else if (this.isGrabbing && this.setRelease)
		{
			this.setRelease = false;
			this.isGrabbing = false;
			this.DoRelease();
		}
		if (this.isGrabbing && this.remainingGrabDuration > 0f)
		{
			this.remainingGrabDuration -= Time.deltaTime;
			this.DoGrab();
		}
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x000124E0 File Offset: 0x000106E0
	private void DoGrab()
	{
		this.grabber.grabbing = true;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.colliders, LayerMask.GetMask(new string[]
		{
			"GorillaInteractable"
		}));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CrittersActor componentInParent = this.colliders[i].GetComponentInParent<CrittersActor>();
				if (!(componentInParent == null) && componentInParent.usesRB && componentInParent.CanBeGrabbed(this.grabber))
				{
					this.isHandGrabbingDisabled = true;
					if (componentInParent.equipmentStorable)
					{
						componentInParent.localCanStore = true;
					}
					componentInParent.GrabbedBy(this.grabber, false, default(Quaternion), default(Vector3), false);
					this.grabber.grabbedActors.Add(componentInParent);
					this.remainingGrabDuration = 0f;
					return;
				}
			}
		}
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000125C4 File Offset: 0x000107C4
	private void DoRelease()
	{
		this.grabber.grabbing = false;
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			crittersActor.Released(true, crittersActor.transform.rotation, crittersActor.transform.position, this.estimator.linearVelocity, default(Vector3));
			if (i < this.grabber.grabbedActors.Count)
			{
				this.grabber.grabbedActors.RemoveAt(i);
			}
		}
		if (this.isHandGrabbingDisabled)
		{
			this.isHandGrabbingDisabled = false;
		}
	}

	// Token: 0x0400038C RID: 908
	public bool isGrabbing;

	// Token: 0x0400038D RID: 909
	public bool setIsGrabbing;

	// Token: 0x0400038E RID: 910
	public bool setRelease;

	// Token: 0x0400038F RID: 911
	public Collider[] colliders = new Collider[50];

	// Token: 0x04000390 RID: 912
	public bool isLeft;

	// Token: 0x04000391 RID: 913
	public float grabRadius = 0.05f;

	// Token: 0x04000392 RID: 914
	public Transform transformToFollow;

	// Token: 0x04000393 RID: 915
	public GorillaVelocityEstimator estimator;

	// Token: 0x04000394 RID: 916
	public CrittersGrabber grabber;

	// Token: 0x04000395 RID: 917
	public CrittersActorGrabber otherHand;

	// Token: 0x04000396 RID: 918
	private bool isHandGrabbingDisabled;

	// Token: 0x04000397 RID: 919
	private float grabDuration = 0.3f;

	// Token: 0x04000398 RID: 920
	private float remainingGrabDuration;
}
