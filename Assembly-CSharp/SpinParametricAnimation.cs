using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200024D RID: 589
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x06000DBC RID: 3516 RVA: 0x000540A9 File Offset: 0x000522A9
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000540BC File Offset: 0x000522BC
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float angle = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(angle, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(angle, this.axis) * transform.localRotation;
	}

	// Token: 0x0400158F RID: 5519
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x04001590 RID: 5520
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x04001591 RID: 5521
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x04001592 RID: 5522
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x04001593 RID: 5523
	private float _animationProgress;

	// Token: 0x04001594 RID: 5524
	private float _oldAngle;
}
