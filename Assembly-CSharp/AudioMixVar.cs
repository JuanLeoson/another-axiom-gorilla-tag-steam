using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020004EF RID: 1263
[Serializable]
public class AudioMixVar
{
	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001EA7 RID: 7847 RVA: 0x000A1EB4 File Offset: 0x000A00B4
	// (set) Token: 0x06001EA8 RID: 7848 RVA: 0x000A1F03 File Offset: 0x000A0103
	public float value
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			if (!this.mixer)
			{
				return 0f;
			}
			float result;
			if (!this.mixer.GetFloat(this.name, out result))
			{
				return 0f;
			}
			return result;
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.name, value);
			}
		}
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000A1F25 File Offset: 0x000A0125
	public void ReturnToPool()
	{
		if (this._pool != null)
		{
			this._pool.Return(this);
		}
	}

	// Token: 0x0400274C RID: 10060
	public AudioMixerGroup group;

	// Token: 0x0400274D RID: 10061
	public AudioMixer mixer;

	// Token: 0x0400274E RID: 10062
	public string name;

	// Token: 0x0400274F RID: 10063
	[NonSerialized]
	public bool taken;

	// Token: 0x04002750 RID: 10064
	[SerializeField]
	private AudioMixVarPool _pool;
}
