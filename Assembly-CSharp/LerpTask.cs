using System;
using UnityEngine;

// Token: 0x0200075F RID: 1887
public class LerpTask<T>
{
	// Token: 0x06002F44 RID: 12100 RVA: 0x000F9D16 File Offset: 0x000F7F16
	public void Reset()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 0f);
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x000F9D46 File Offset: 0x000F7F46
	public void Start(T from, T to, float duration)
	{
		this.lerpFrom = from;
		this.lerpTo = to;
		this.duration = duration;
		this.elapsed = 0f;
		this.active = true;
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x000F9D70 File Offset: 0x000F7F70
	public void Finish()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 1f);
		Action action = this.onLerpEnd;
		if (action != null)
		{
			action();
		}
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x000F9DBC File Offset: 0x000F7FBC
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.elapsed < this.duration)
		{
			float arg = (this.elapsed + deltaTime >= this.duration) ? 1f : (this.elapsed / this.duration);
			this.onLerp(this.lerpFrom, this.lerpTo, arg);
			this.elapsed += deltaTime;
			return;
		}
		this.Finish();
	}

	// Token: 0x04003B39 RID: 15161
	public float elapsed;

	// Token: 0x04003B3A RID: 15162
	public float duration;

	// Token: 0x04003B3B RID: 15163
	public T lerpFrom;

	// Token: 0x04003B3C RID: 15164
	public T lerpTo;

	// Token: 0x04003B3D RID: 15165
	public Action<T, T, float> onLerp;

	// Token: 0x04003B3E RID: 15166
	public Action onLerpEnd;

	// Token: 0x04003B3F RID: 15167
	public bool active;
}
