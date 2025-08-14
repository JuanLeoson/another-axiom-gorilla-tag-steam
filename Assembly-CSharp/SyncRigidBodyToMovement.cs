using System;
using BoingKit;
using UnityEngine;

// Token: 0x020007BB RID: 1979
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x060031A4 RID: 12708 RVA: 0x0010287E File Offset: 0x00100A7E
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x001028B8 File Offset: 0x00100AB8
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x0010290C File Offset: 0x00100B0C
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x00102920 File Offset: 0x00100B20
	private void FixedUpdate()
	{
		this.targetRigidbody.velocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x04003D5A RID: 15706
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04003D5B RID: 15707
	private Transform targetParent;
}
