using System;
using UnityEngine;

// Token: 0x02000893 RID: 2195
[Serializable]
public class VelocityHelper
{
	// Token: 0x0600375E RID: 14174 RVA: 0x0011F545 File Offset: 0x0011D745
	public VelocityHelper(int historySize = 12)
	{
		this._size = historySize;
		this._samples = new float[historySize * 4];
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0011F564 File Offset: 0x0011D764
	public void SamplePosition(Transform target, float dt)
	{
		Vector3 position = target.position;
		if (!this._initialized)
		{
			this._InitSamples(position, dt);
		}
		this._SetSample(this._latest, position, dt);
		this._latest = (this._latest + 1) % this._size;
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0011F5AC File Offset: 0x0011D7AC
	private void _InitSamples(Vector3 position, float dt)
	{
		for (int i = 0; i < this._size; i++)
		{
			this._SetSample(i, position, dt);
		}
		this._initialized = true;
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x0011F5DA File Offset: 0x0011D7DA
	private void _SetSample(int i, Vector3 position, float dt)
	{
		this._samples[i] = position.x;
		this._samples[i + 1] = position.y;
		this._samples[i + 2] = position.z;
		this._samples[i + 3] = dt;
	}

	// Token: 0x040043DD RID: 17373
	private float[] _samples;

	// Token: 0x040043DE RID: 17374
	private int _latest;

	// Token: 0x040043DF RID: 17375
	private int _size;

	// Token: 0x040043E0 RID: 17376
	private bool _initialized;
}
