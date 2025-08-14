using System;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class PushableSlider : MonoBehaviour
{
	// Token: 0x06001CDB RID: 7387 RVA: 0x0009B965 File Offset: 0x00099B65
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x0009B96D File Offset: 0x00099B6D
	private void Initialize()
	{
		if (this._initialized)
		{
			return;
		}
		this._initialized = true;
		this._localSpace = base.transform.worldToLocalMatrix;
		this._startingPos = base.transform.localPosition;
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0009B9A4 File Offset: 0x00099BA4
	private void OnTriggerStay(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 b = this._localSpace.MultiplyPoint3x4(other.transform.position);
		Vector3 vector = base.transform.localPosition - this._startingPos - b;
		float num = Mathf.Abs(vector.x);
		if (num < this.farPushDist)
		{
			Vector3 currentVelocity = componentInParent.currentVelocity;
			if (Mathf.Sign(vector.x) != Mathf.Sign((this._localSpace.rotation * currentVelocity).x))
			{
				return;
			}
			vector.x = Mathf.Sign(vector.x) * (this.farPushDist - num);
			vector.y = 0f;
			vector.z = 0f;
			Vector3 vector2 = base.transform.localPosition - this._startingPos + vector;
			vector2.x = Mathf.Clamp(vector2.x, this.minXOffset, this.maxXOffset);
			base.transform.localPosition = this.GetXOffsetVector(vector2.x + this._startingPos.x);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0009BB07 File Offset: 0x00099D07
	private Vector3 GetXOffsetVector(float x)
	{
		return new Vector3(x, this._startingPos.y, this._startingPos.z);
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x0009BB28 File Offset: 0x00099D28
	public void SetProgress(float value)
	{
		this.Initialize();
		value = Mathf.Clamp(value, 0f, 1f);
		float num = Mathf.Lerp(this.minXOffset, this.maxXOffset, value);
		base.transform.localPosition = this.GetXOffsetVector(this._startingPos.x + num);
		this._previousLocalPosition = new Vector3(num, 0f, 0f);
		this._cachedProgress = value;
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x0009BB9C File Offset: 0x00099D9C
	public float GetProgress()
	{
		this.Initialize();
		Vector3 vector = base.transform.localPosition - this._startingPos;
		if (vector == this._previousLocalPosition)
		{
			return this._cachedProgress;
		}
		this._previousLocalPosition = vector;
		this._cachedProgress = (vector.x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
		return this._cachedProgress;
	}

	// Token: 0x04002547 RID: 9543
	[SerializeField]
	private float farPushDist = 0.015f;

	// Token: 0x04002548 RID: 9544
	[SerializeField]
	private float maxXOffset;

	// Token: 0x04002549 RID: 9545
	[SerializeField]
	private float minXOffset;

	// Token: 0x0400254A RID: 9546
	private Matrix4x4 _localSpace;

	// Token: 0x0400254B RID: 9547
	private Vector3 _startingPos;

	// Token: 0x0400254C RID: 9548
	private Vector3 _previousLocalPosition;

	// Token: 0x0400254D RID: 9549
	private float _cachedProgress;

	// Token: 0x0400254E RID: 9550
	private bool _initialized;
}
