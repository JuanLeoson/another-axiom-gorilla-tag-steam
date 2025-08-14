using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DF RID: 479
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x04000E89 RID: 3721
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04000E8A RID: 3722
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000E8B RID: 3723
	[SerializeField]
	private Text errorUIText;

	// Token: 0x04000E8C RID: 3724
	[SerializeField]
	private Font errorFont;

	// Token: 0x04000E8D RID: 3725
	public string displayedText;
}
