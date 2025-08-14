using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020004D6 RID: 1238
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x06001E59 RID: 7769 RVA: 0x000A12E1 File Offset: 0x0009F4E1
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x000A12F4 File Offset: 0x0009F4F4
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x040026DB RID: 9947
	public GameObject handToStickTo;

	// Token: 0x040026DC RID: 9948
	public bool isLeftHand;

	// Token: 0x040026DD RID: 9949
	public float hapticStrength;

	// Token: 0x040026DE RID: 9950
	private Rigidbody thisRigidbody;

	// Token: 0x040026DF RID: 9951
	private XRController controllerReference;
}
