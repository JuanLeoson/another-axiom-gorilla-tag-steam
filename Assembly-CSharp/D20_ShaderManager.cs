using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x060006E7 RID: 1767 RVA: 0x000274D8 File Offset: 0x000256D8
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x0002753D File Offset: 0x0002573D
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x0400084B RID: 2123
	private Rigidbody rb;

	// Token: 0x0400084C RID: 2124
	private Vector3 lastPosition;

	// Token: 0x0400084D RID: 2125
	public float updateInterval = 0.1f;

	// Token: 0x0400084E RID: 2126
	public Vector3 velocity;

	// Token: 0x0400084F RID: 2127
	private Material material;
}
