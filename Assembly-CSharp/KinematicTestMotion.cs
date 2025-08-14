using System;
using UnityEngine;

// Token: 0x02000A86 RID: 2694
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06004184 RID: 16772 RVA: 0x0014AF06 File Offset: 0x00149106
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x0014AF1D File Offset: 0x0014911D
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004186 RID: 16774 RVA: 0x0014AF33 File Offset: 0x00149133
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06004187 RID: 16775 RVA: 0x0014AF4C File Offset: 0x0014914C
	private void UpdatePosition(float time)
	{
		float t = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 position = Vector3.Lerp(this.start.position, this.end.position, t);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = position;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(position);
		}
	}

	// Token: 0x04004D0D RID: 19725
	public Transform start;

	// Token: 0x04004D0E RID: 19726
	public Transform end;

	// Token: 0x04004D0F RID: 19727
	public Rigidbody rigidbody;

	// Token: 0x04004D10 RID: 19728
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04004D11 RID: 19729
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04004D12 RID: 19730
	public float period = 4f;

	// Token: 0x02000A87 RID: 2695
	public enum UpdateType
	{
		// Token: 0x04004D14 RID: 19732
		Update,
		// Token: 0x04004D15 RID: 19733
		LateUpdate,
		// Token: 0x04004D16 RID: 19734
		FixedUpdate
	}

	// Token: 0x02000A88 RID: 2696
	public enum MoveType
	{
		// Token: 0x04004D18 RID: 19736
		TransformPosition,
		// Token: 0x04004D19 RID: 19737
		RigidbodyMovePosition
	}
}
