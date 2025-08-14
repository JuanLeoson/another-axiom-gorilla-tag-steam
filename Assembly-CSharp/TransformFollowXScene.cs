using System;
using UnityEngine;

// Token: 0x020007C8 RID: 1992
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x060031E6 RID: 12774 RVA: 0x00103907 File Offset: 0x00101B07
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x0010391A File Offset: 0x00101B1A
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x060031E8 RID: 12776 RVA: 0x00103930 File Offset: 0x00101B30
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04003DB0 RID: 15792
	public XSceneRef refToFollow;

	// Token: 0x04003DB1 RID: 15793
	private Transform transformToFollow;

	// Token: 0x04003DB2 RID: 15794
	public Vector3 offset;

	// Token: 0x04003DB3 RID: 15795
	public Vector3 prevPos;
}
