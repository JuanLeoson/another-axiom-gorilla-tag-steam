using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class ShadeJumpscare : MonoBehaviour
{
	// Token: 0x06000453 RID: 1107 RVA: 0x00019533 File Offset: 0x00017733
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00019541 File Offset: 0x00017741
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.GTPlay();
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00019580 File Offset: 0x00017780
	private void Update()
	{
		float num = Time.time - this.startTime;
		float time = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(time), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num2 = this.shadeScaleFunction.Evaluate(time);
		this.shadeTransform.localScale = new Vector3(num2, num2 * this.shadeYScaleMultFunction.Evaluate(time), num2);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(time);
	}

	// Token: 0x040004F5 RID: 1269
	[SerializeField]
	private Transform shadeTransform;

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private float animationTime;

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private float shadeRotationSpeed = 1f;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	// Token: 0x040004F9 RID: 1273
	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	// Token: 0x040004FA RID: 1274
	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	// Token: 0x040004FB RID: 1275
	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	// Token: 0x040004FC RID: 1276
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x040004FD RID: 1277
	private AudioSource audioSource;

	// Token: 0x040004FE RID: 1278
	private float startTime;

	// Token: 0x040004FF RID: 1279
	private float startAngle;
}
