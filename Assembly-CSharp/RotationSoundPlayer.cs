using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class RotationSoundPlayer : MonoBehaviour
{
	// Token: 0x06000641 RID: 1601 RVA: 0x000242FC File Offset: 0x000224FC
	private void Awake()
	{
		List<Transform> list = new List<Transform>(this.transforms);
		list.RemoveAll((Transform xform) => xform == null);
		this.transforms = list.ToArray();
		this.initialUpAxis = new Vector3[this.transforms.Length];
		this.lastUpAxis = new Vector3[this.transforms.Length];
		this.lastRotationSpeeds = new float[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.initialUpAxis[i] = this.transforms[i].localRotation * Vector3.up;
			this.lastUpAxis[i] = this.initialUpAxis[i];
			this.lastRotationSpeeds[i] = 0f;
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000243DC File Offset: 0x000225DC
	private void Update()
	{
		this.cooldownTimer -= Time.deltaTime;
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Vector3 vector = this.transforms[i].localRotation * Vector3.up;
			float num = Vector3.Angle(vector, this.initialUpAxis[i]);
			float num2 = Vector3.Angle(vector, this.lastUpAxis[i]);
			float deltaTime = Time.deltaTime;
			float num3 = num2 / deltaTime;
			if (this.cooldownTimer <= 0f && num > this.rotationAmountThreshold && num3 > this.rotationSpeedThreshold && !this.soundBankPlayer.isPlaying)
			{
				this.cooldownTimer = this.cooldown;
				this.soundBankPlayer.Play();
			}
			this.lastUpAxis[i] = vector;
			this.lastRotationSpeeds[i] = num3;
		}
	}

	// Token: 0x0400077F RID: 1919
	[Tooltip("Transforms that will make a noise when they rotate.")]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x04000780 RID: 1920
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000781 RID: 1921
	[Tooltip("How much the transform must rotate from it's initial rotation before a sound is played.")]
	private float rotationAmountThreshold = 30f;

	// Token: 0x04000782 RID: 1922
	[Tooltip("How fast the transform must rotate before a sound is played.")]
	private float rotationSpeedThreshold = 45f;

	// Token: 0x04000783 RID: 1923
	private float cooldown = 0.6f;

	// Token: 0x04000784 RID: 1924
	private float cooldownTimer;

	// Token: 0x04000785 RID: 1925
	private Vector3[] initialUpAxis;

	// Token: 0x04000786 RID: 1926
	private Vector3[] lastUpAxis;

	// Token: 0x04000787 RID: 1927
	private float[] lastRotationSpeeds;
}
