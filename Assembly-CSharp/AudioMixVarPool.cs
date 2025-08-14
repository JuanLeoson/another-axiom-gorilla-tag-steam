using System;
using UnityEngine;

// Token: 0x020004EE RID: 1262
[CreateAssetMenu(fileName = "New AudioMixVarPool", menuName = "ScriptableObjects/AudioMixVarPool", order = 0)]
public class AudioMixVarPool : ScriptableObject
{
	// Token: 0x06001EA4 RID: 7844 RVA: 0x000A1E1C File Offset: 0x000A001C
	public bool Rent(out AudioMixVar mixVar)
	{
		for (int i = 0; i < this._vars.Length; i++)
		{
			if (!this._vars[i].taken)
			{
				this._vars[i].taken = true;
				mixVar = this._vars[i];
				return true;
			}
		}
		mixVar = null;
		return false;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000A1E6C File Offset: 0x000A006C
	public void Return(AudioMixVar mixVar)
	{
		if (mixVar == null)
		{
			return;
		}
		int num = this._vars.IndexOfRef(mixVar);
		if (num == -1)
		{
			return;
		}
		this._vars[num].taken = false;
	}

	// Token: 0x0400274B RID: 10059
	[SerializeField]
	private AudioMixVar[] _vars = new AudioMixVar[0];
}
