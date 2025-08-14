using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class EqualizerAnim : MonoBehaviour
{
	// Token: 0x06000A32 RID: 2610 RVA: 0x00037A65 File Offset: 0x00035C65
	private void Start()
	{
		this.inputColorHash = Shader.PropertyToID(this.inputColorProperty);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00037A78 File Offset: 0x00035C78
	private void Update()
	{
		if (EqualizerAnim.thisFrame == Time.frameCount)
		{
			if (EqualizerAnim.materialsUpdatedThisFrame.Contains(this.material))
			{
				return;
			}
		}
		else
		{
			EqualizerAnim.thisFrame = Time.frameCount;
			EqualizerAnim.materialsUpdatedThisFrame.Clear();
		}
		float time = Time.time % this.loopDuration;
		this.material.SetColor(this.inputColorHash, new Color(this.redCurve.Evaluate(time), this.greenCurve.Evaluate(time), this.blueCurve.Evaluate(time)));
		EqualizerAnim.materialsUpdatedThisFrame.Add(this.material);
	}

	// Token: 0x04000C53 RID: 3155
	[SerializeField]
	private AnimationCurve redCurve;

	// Token: 0x04000C54 RID: 3156
	[SerializeField]
	private AnimationCurve greenCurve;

	// Token: 0x04000C55 RID: 3157
	[SerializeField]
	private AnimationCurve blueCurve;

	// Token: 0x04000C56 RID: 3158
	[SerializeField]
	private float loopDuration;

	// Token: 0x04000C57 RID: 3159
	[SerializeField]
	private Material material;

	// Token: 0x04000C58 RID: 3160
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04000C59 RID: 3161
	private int inputColorHash;

	// Token: 0x04000C5A RID: 3162
	private static int thisFrame;

	// Token: 0x04000C5B RID: 3163
	private static HashSet<Material> materialsUpdatedThisFrame = new HashSet<Material>();
}
