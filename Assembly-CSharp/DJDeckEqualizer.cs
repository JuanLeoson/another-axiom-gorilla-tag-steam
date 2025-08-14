using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class DJDeckEqualizer : MonoBehaviour
{
	// Token: 0x06000A09 RID: 2569 RVA: 0x00036E65 File Offset: 0x00035065
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
		this.material = this.display.material;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00036E8C File Offset: 0x0003508C
	private void Update()
	{
		Color value = default(Color);
		value.r = 0.25f;
		value.g = 0.25f;
		value.b = 0.5f;
		for (int i = 0; i < this.redTracks.Length; i++)
		{
			AudioSource audioSource = this.redTracks[i];
			if (audioSource.isPlaying)
			{
				value.r = Mathf.Lerp(0.25f, 1f, this.redTrackCurves[i].Evaluate(audioSource.time));
				break;
			}
		}
		for (int j = 0; j < this.greenTracks.Length; j++)
		{
			AudioSource audioSource2 = this.greenTracks[j];
			if (audioSource2.isPlaying)
			{
				value.g = Mathf.Lerp(0.25f, 1f, this.greenTrackCurves[j].Evaluate(audioSource2.time));
				break;
			}
		}
		this.material.SetColor(this.inputColorHash, value);
	}

	// Token: 0x04000C13 RID: 3091
	[SerializeField]
	private MeshRenderer display;

	// Token: 0x04000C14 RID: 3092
	[SerializeField]
	private AnimationCurve[] redTrackCurves;

	// Token: 0x04000C15 RID: 3093
	[SerializeField]
	private AnimationCurve[] greenTrackCurves;

	// Token: 0x04000C16 RID: 3094
	[SerializeField]
	private AudioSource[] redTracks;

	// Token: 0x04000C17 RID: 3095
	[SerializeField]
	private AudioSource[] greenTracks;

	// Token: 0x04000C18 RID: 3096
	private Material material;

	// Token: 0x04000C19 RID: 3097
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04000C1A RID: 3098
	private ShaderHashId inputColorHash;
}
