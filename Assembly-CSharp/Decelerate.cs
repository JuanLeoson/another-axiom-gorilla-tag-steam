using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057D RID: 1405
public class Decelerate : MonoBehaviour
{
	// Token: 0x06002258 RID: 8792 RVA: 0x000B97C8 File Offset: 0x000B79C8
	public void Restart()
	{
		base.enabled = true;
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000B97D4 File Offset: 0x000B79D4
	private void Update()
	{
		if (!this._rigidbody)
		{
			return;
		}
		Vector3 vector = this._rigidbody.velocity;
		vector *= this._friction;
		if (vector.Approx0(0.001f))
		{
			this._rigidbody.velocity = Vector3.zero;
			UnityEvent unityEvent = this.onStop;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			base.enabled = false;
		}
		else
		{
			this._rigidbody.velocity = vector;
		}
		if (this._resetOrientationOnRelease && !this._rigidbody.rotation.Approx(Quaternion.identity, 1E-06f))
		{
			this._rigidbody.rotation = Quaternion.identity;
		}
	}

	// Token: 0x04002BC0 RID: 11200
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04002BC1 RID: 11201
	[SerializeField]
	private float _friction = 0.875f;

	// Token: 0x04002BC2 RID: 11202
	[SerializeField]
	private bool _resetOrientationOnRelease;

	// Token: 0x04002BC3 RID: 11203
	public UnityEvent onStop;
}
