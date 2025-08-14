using System;
using UnityEngine;

// Token: 0x02000AE1 RID: 2785
[CreateAssetMenu(menuName = "Gorilla Tag/SoundBankSO")]
public class SoundBankSO : ScriptableObject
{
	// Token: 0x04004DE7 RID: 19943
	public AudioClip[] sounds;

	// Token: 0x04004DE8 RID: 19944
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x04004DE9 RID: 19945
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
