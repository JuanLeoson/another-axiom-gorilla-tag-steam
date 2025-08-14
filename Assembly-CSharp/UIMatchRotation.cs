using System;
using UnityEngine;

// Token: 0x02000AFA RID: 2810
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x060043B8 RID: 17336 RVA: 0x00154699 File Offset: 0x00152899
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x001546C8 File Offset: 0x001528C8
	private void Update()
	{
		Vector3 lhs = this.x0z(base.transform.forward);
		Vector3 vector = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(lhs, vector);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x0015476C File Offset: 0x0015296C
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x04004E3C RID: 20028
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x04004E3D RID: 20029
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x04004E3E RID: 20030
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x04004E3F RID: 20031
	private UIMatchRotation.State state;

	// Token: 0x02000AFB RID: 2811
	private enum State
	{
		// Token: 0x04004E41 RID: 20033
		Ready,
		// Token: 0x04004E42 RID: 20034
		Rotating
	}
}
