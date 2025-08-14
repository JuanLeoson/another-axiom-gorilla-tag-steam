using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000C8 RID: 200
public class MetroSpotlight : MonoBehaviour
{
	// Token: 0x060004F8 RID: 1272 RVA: 0x0001CFC8 File Offset: 0x0001B1C8
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 yDir = Vector3.Cross(normalized, vector);
		Vector3 worldPosition = MetroSpotlight.Figure8(position, vector, yDir, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(worldPosition);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001D07C File Offset: 0x0001B27C
	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float d = scale * num * Mathf.Cos(t + offset);
		float d2 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 axis = Vector3.Cross(xDir, yDir);
		Quaternion rotation = Quaternion.AngleAxis(theta, axis);
		xDir = rotation * xDir;
		yDir = rotation * yDir;
		Vector3 b = xDir * d + yDir * d2;
		return origin + b;
	}

	// Token: 0x040005EE RID: 1518
	[SerializeField]
	private Transform _blimp;

	// Token: 0x040005EF RID: 1519
	[SerializeField]
	private Transform _light;

	// Token: 0x040005F0 RID: 1520
	[SerializeField]
	private Transform _target;

	// Token: 0x040005F1 RID: 1521
	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	// Token: 0x040005F2 RID: 1522
	[SerializeField]
	private float _offset;

	// Token: 0x040005F3 RID: 1523
	[SerializeField]
	private float _theta;

	// Token: 0x040005F4 RID: 1524
	public float speed = 16f;

	// Token: 0x040005F5 RID: 1525
	[Space]
	private float _time;
}
