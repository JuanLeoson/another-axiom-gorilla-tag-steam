using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class CosmeticCritterShadeFleeing : CosmeticCritter
{
	// Token: 0x06000449 RID: 1097 RVA: 0x00019173 File Offset: 0x00017373
	public override void OnSpawn()
	{
		this.spawnFX.Play();
		this.spawnAudioSource.clip = this.spawnAudioClips.GetRandomItem<AudioClip>();
		this.spawnAudioSource.GTPlay();
		this.pullVector = Vector3.zero;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x000191AC File Offset: 0x000173AC
	public void SetFleePosition(Vector3 position, Vector3 fleeFrom)
	{
		this.origin = position;
		Vector3 vector = position - fleeFrom;
		this.fleeForward = vector.normalized;
		this.fleeRight = Vector3.Cross(this.fleeForward, Vector3.up);
		this.fleeUp = Vector3.Cross(this.fleeForward, this.fleeRight);
		this.trailingPosition = position + vector.normalized * 3f;
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00019220 File Offset: 0x00017420
	public override void SetRandomVariables()
	{
		float num = 0f;
		for (int i = 0; i < this.modelSwaps.Length; i++)
		{
			num += this.modelSwaps[i].relativeProbability;
			this.modelSwaps[i].gameObject.SetActive(false);
		}
		float num2 = Random.value * num;
		for (int j = 0; j < this.modelSwaps.Length; j++)
		{
			if (num2 < this.modelSwaps[j].relativeProbability)
			{
				this.modelSwaps[j].gameObject.SetActive(true);
				break;
			}
			num2 -= this.modelSwaps[j].relativeProbability;
		}
		this.fleeBobFrequencyXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.x, Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.y);
		this.fleeBobMagnitudeXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.x, Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.y);
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00019340 File Offset: 0x00017540
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		Vector3 vector = this.origin + num * this.fleeForward + this.pullVector + Mathf.Sin(this.fleeBobFrequencyXY.x * num) * this.fleeBobMagnitudeXY.x * this.fleeRight + Mathf.Sin(this.fleeBobFrequencyXY.y * num) * this.fleeBobMagnitudeXY.y * this.fleeUp;
		Quaternion rotation = Quaternion.LookRotation((vector - this.trailingPosition).normalized, Vector3.up);
		this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector, 0.05f);
		base.transform.SetPositionAndRotation(vector, rotation);
		this.animator.SetFloat(this.animatorProperty, Mathf.Sin(num * 3f) * 0.5f + 0.5f);
	}

	// Token: 0x040004D9 RID: 1241
	[Tooltip("Randomly selects one of these models when spawned, accounting for relative probabilities. For example, if one model has a probability of 1 and another a probability of 2, the second is twice as likely to be picked (and thus will be picked 67% of the time).")]
	[SerializeField]
	private CosmeticCritterShadeFleeing.ModelSwap[] modelSwaps;

	// Token: 0x040004DA RID: 1242
	[Space]
	[Tooltip("Despawn the Shade after it has fled (fleed?) this many meters.")]
	[SerializeField]
	private float fleeDistanceToDespawn = 10f;

	// Token: 0x040004DB RID: 1243
	[Tooltip("Flee away from the spotter at this many meters per second.")]
	[SerializeField]
	private float fleeSpeed;

	// Token: 0x040004DC RID: 1244
	[Tooltip("The maximum strength the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobMagnitudeXYMax;

	// Token: 0x040004DD RID: 1245
	[Tooltip("The maximum frequency the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobFrequencyXYMax;

	// Token: 0x040004DE RID: 1246
	[SerializeField]
	private Animator animator;

	// Token: 0x040004DF RID: 1247
	[SerializeField]
	private ParticleSystem spawnFX;

	// Token: 0x040004E0 RID: 1248
	[SerializeField]
	private AudioSource spawnAudioSource;

	// Token: 0x040004E1 RID: 1249
	[SerializeField]
	private AudioClip[] spawnAudioClips;

	// Token: 0x040004E2 RID: 1250
	[HideInInspector]
	public Vector3 pullVector;

	// Token: 0x040004E3 RID: 1251
	private Vector3 origin;

	// Token: 0x040004E4 RID: 1252
	private Vector3 fleeForward;

	// Token: 0x040004E5 RID: 1253
	private Vector3 fleeRight;

	// Token: 0x040004E6 RID: 1254
	private Vector3 fleeUp = Vector3.up;

	// Token: 0x040004E7 RID: 1255
	private Vector2 fleeBobFrequencyXY;

	// Token: 0x040004E8 RID: 1256
	private Vector2 fleeBobMagnitudeXY;

	// Token: 0x040004E9 RID: 1257
	private Vector3 trailingPosition;

	// Token: 0x040004EA RID: 1258
	private float closestCatcherDistance;

	// Token: 0x040004EB RID: 1259
	private int animatorProperty = Animator.StringToHash("Distance");

	// Token: 0x020000AE RID: 174
	[Serializable]
	private class ModelSwap
	{
		// Token: 0x040004EC RID: 1260
		public float relativeProbability;

		// Token: 0x040004ED RID: 1261
		public GameObject gameObject;
	}
}
