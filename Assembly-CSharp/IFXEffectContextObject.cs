using System;
using UnityEngine;

// Token: 0x02000A4E RID: 2638
public interface IFXEffectContextObject
{
	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x0600407B RID: 16507
	int[] PrefabPoolIds { get; }

	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x0600407C RID: 16508
	Vector3 Positon { get; }

	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x0600407D RID: 16509
	Quaternion Rotation { get; }

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x0600407E RID: 16510
	AudioSource SoundSource { get; }

	// Token: 0x17000619 RID: 1561
	// (get) Token: 0x0600407F RID: 16511
	AudioClip Sound { get; }

	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x06004080 RID: 16512
	float Volume { get; }

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x06004081 RID: 16513
	float Pitch { get; }

	// Token: 0x06004082 RID: 16514
	void OnTriggerActions();

	// Token: 0x06004083 RID: 16515
	void OnPlayVisualFX(int effectID, GameObject effect);

	// Token: 0x06004084 RID: 16516
	void OnPlaySoundFX(AudioSource audioSource);
}
