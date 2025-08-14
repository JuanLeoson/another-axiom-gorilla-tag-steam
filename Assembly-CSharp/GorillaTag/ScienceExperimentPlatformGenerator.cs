using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000E82 RID: 3714
	public class ScienceExperimentPlatformGenerator : MonoBehaviourPun, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06005D02 RID: 23810 RVA: 0x001D5FF3 File Offset: 0x001D41F3
		private void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
			this.scienceExperimentManager = base.GetComponent<ScienceExperimentManager>();
		}

		// Token: 0x06005D03 RID: 23811 RVA: 0x001D6007 File Offset: 0x001D4207
		private void OnEnable()
		{
			if (((IGuidedRefReceiverMono)this).GuidedRefsWaitingToResolveCount > 0)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005D04 RID: 23812 RVA: 0x00100D37 File Offset: 0x000FEF37
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06005D05 RID: 23813 RVA: 0x001D6019 File Offset: 0x001D4219
		// (set) Token: 0x06005D06 RID: 23814 RVA: 0x001D6021 File Offset: 0x001D4221
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005D07 RID: 23815 RVA: 0x001D602C File Offset: 0x001D422C
		void ITickSystemPost.PostTick()
		{
			double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
			this.UpdateTrails(currentTime);
			this.RemoveExpiredBubbles(currentTime);
			this.SpawnNewBubbles(currentTime);
			this.UpdateActiveBubbles(currentTime);
		}

		// Token: 0x06005D08 RID: 23816 RVA: 0x001D606C File Offset: 0x001D426C
		private void RemoveExpiredBubbles(double currentTime)
		{
			for (int i = this.activeBubbles.Count - 1; i >= 0; i--)
			{
				if (Mathf.Clamp01((float)(currentTime - this.activeBubbles[i].spawnTime) / this.activeBubbles[i].lifetime) >= 1f)
				{
					this.activeBubbles[i].bubble.Pop();
					this.activeBubbles.RemoveAt(i);
				}
			}
		}

		// Token: 0x06005D09 RID: 23817 RVA: 0x001D60E8 File Offset: 0x001D42E8
		private void SpawnNewBubbles(double currentTime)
		{
			if (base.photonView.IsMine && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				int num = Mathf.Min((int)(this.rockCountVsLavaProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.bubbleCountMultiplier), this.maxBubbleCount) - this.activeBubbles.Count;
				if (this.activeBubbles.Count < this.maxBubbleCount)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnRockAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
			}
		}

		// Token: 0x06005D0A RID: 23818 RVA: 0x001D6178 File Offset: 0x001D4378
		private void UpdateActiveBubbles(double currentTime)
		{
			if (this.liquidSurfacePlane == null)
			{
				return;
			}
			float y = this.liquidSurfacePlane.transform.position.y;
			float num = this.bubblePopWobbleAmplitude * Mathf.Sin(this.bubblePopWobbleFrequency * 0.5f * 3.1415927f * Time.time);
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = this.activeBubbles[i];
				float time = Mathf.Clamp01((float)(currentTime - bubbleData.spawnTime) / bubbleData.lifetime);
				float d = bubbleData.spawnSize * this.rockSizeVsLifetime.Evaluate(time) * this.scaleFactor;
				bubbleData.position.y = y;
				bubbleData.bubble.body.gameObject.transform.localScale = Vector3.one * d;
				bubbleData.bubble.body.MovePosition(bubbleData.position);
				float num2 = (float)((double)bubbleData.lifetime + bubbleData.spawnTime - currentTime);
				if (num2 < this.bubblePopAnticipationTime)
				{
					float num3 = Mathf.Clamp01(1f - num2 / this.bubblePopAnticipationTime);
					bubbleData.bubble.bubbleMesh.transform.localScale = Vector3.one * (1f + num3 * num);
				}
				this.activeBubbles[i] = bubbleData;
			}
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x001D62E0 File Offset: 0x001D44E0
		private void UpdateTrails(double currentTime)
		{
			if (base.photonView.IsMine)
			{
				int num = (int)(this.trailCountVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailCountMultiplier) - this.trailHeads.Count;
				if (num > 0 && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnTrailAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
				else if (num < 0)
				{
					for (int j = 0; j > num; j--)
					{
						this.trailHeads.RemoveAt(0);
					}
				}
				float num2 = this.trailSpawnRateVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailSpawnRateMultiplier;
				float num3 = this.trailBubbleBoundaryRadiusVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.surfaceRadiusSpawnRange.y;
				for (int k = this.trailHeads.Count - 1; k >= 0; k--)
				{
					if ((float)(currentTime - this.trailHeads[k].spawnTime) > num2)
					{
						float num4 = -this.trailMaxTurnAngle;
						float num5 = this.trailMaxTurnAngle;
						float num6 = Vector3.SignedAngle(this.trailHeads[k].direction, this.trailHeads[k].position - this.liquidSurfacePlane.transform.position, Vector3.up);
						float num7 = num3 - Vector3.Distance(this.trailHeads[k].position, this.liquidSurfacePlane.transform.position);
						if (num7 < this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor)
						{
							float num8 = Mathf.InverseLerp(this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor, this.trailEdgeAvoidanceSpawnsMinMax.y * this.trailDistanceBetweenSpawns * this.scaleFactor, num7);
							if (num6 > 0f)
							{
								float b = num6 - 90f * num8;
								num5 = Mathf.Min(num5, b);
								num4 = Mathf.Min(num4, num5 - this.trailMaxTurnAngle);
							}
							else
							{
								float b2 = num6 + 90f * num8;
								num4 = Mathf.Max(num4, b2);
								num5 = Mathf.Max(num5, num4 + this.trailMaxTurnAngle);
							}
						}
						Vector3 vector = Quaternion.AngleAxis(Random.Range(num4, num5), Vector3.up) * this.trailHeads[k].direction;
						Vector3 vector2 = this.trailHeads[k].position + vector * this.trailDistanceBetweenSpawns * this.scaleFactor - this.liquidSurfacePlane.transform.position;
						if (vector2.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
						{
							vector2 = vector2.normalized * this.surfaceRadiusSpawnRange.y;
						}
						Vector2 vector3 = new Vector2(vector2.x, vector2.z);
						float num9 = this.trailBubbleSize;
						float num10 = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
						this.trailHeads.RemoveAt(k);
						base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[]
						{
							vector3,
							num9,
							num10,
							currentTime
						});
						this.SpawnSodaBubbleLocal(vector3, num9, num10, currentTime, true, vector);
					}
				}
			}
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x001D6678 File Offset: 0x001D4878
		private void SpawnRockAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num2 = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num3 = Random.Range(this.lifetimeRange.x, this.lifetimeRange.y) * num;
				float num4 = Random.Range(this.sizeRange.x, this.sizeRange.y * num2);
				float d = this.spawnRadiusMultiplierVsLavaProgress.Evaluate(lavaProgress);
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y) * d;
				vector = this.GetSpawnPositionWithClearance(vector, num4 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[]
				{
					vector,
					num4,
					num3,
					currentTime
				});
				this.SpawnSodaBubbleLocal(vector, num4, num3, currentTime, false, default(Vector3));
			}
		}

		// Token: 0x06005D0D RID: 23821 RVA: 0x001D67B0 File Offset: 0x001D49B0
		private void SpawnTrailAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
				float num2 = this.trailBubbleSize;
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y);
				vector = this.GetSpawnPositionWithClearance(vector, num2 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				Vector3 direction = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward;
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[]
				{
					vector,
					num2,
					num,
					currentTime
				});
				this.SpawnSodaBubbleLocal(vector, num2, num, currentTime, true, direction);
			}
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x001D68B8 File Offset: 0x001D4AB8
		private void SpawnSodaBubbleLocal(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, bool addAsTrail = false, Vector3 direction = default(Vector3))
		{
			if (this.activeBubbles.Count < this.maxBubbleCount)
			{
				Vector3 position = this.liquidSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0f, surfacePosLocal.y);
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = new ScienceExperimentPlatformGenerator.BubbleData
				{
					position = position,
					spawnSize = spawnSize,
					lifetime = lifetime,
					spawnTime = spawnTime,
					isTrail = false
				};
				bubbleData.bubble = ObjectPools.instance.Instantiate(this.spawnedPrefab, bubbleData.position, Quaternion.identity, 0f, true).GetComponent<SodaBubble>();
				if (base.photonView.IsMine && addAsTrail)
				{
					bubbleData.direction = direction;
					bubbleData.isTrail = true;
					this.trailHeads.Add(bubbleData);
				}
				this.activeBubbles.Add(bubbleData);
			}
		}

		// Token: 0x06005D0F RID: 23823 RVA: 0x001D69A0 File Offset: 0x001D4BA0
		[PunRPC]
		public void SpawnSodaBubbleRPC(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnSodaBubbleRPC");
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				if (!float.IsFinite(spawnSize) || !float.IsFinite(lifetime) || !double.IsFinite(spawnTime))
				{
					return;
				}
				float time = Mathf.Clamp01(this.scienceExperimentManager.RiseProgressLinear);
				ref surfacePosLocal.ClampThisMagnitudeSafe(this.surfaceRadiusSpawnRange.y);
				spawnSize = Mathf.Clamp(spawnSize, this.sizeRange.x, this.sizeRange.y * this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(time));
				lifetime = Mathf.Clamp(lifetime, this.lifetimeRange.x, this.lifetimeRange.y * this.rockLifetimeMultiplierVsLavaProgress.Evaluate(time));
				double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
				spawnTime = ((Mathf.Abs((float)(spawnTime - num)) < 10f) ? spawnTime : num);
				this.SpawnSodaBubbleLocal(surfacePosLocal, spawnSize, lifetime, spawnTime, false, default(Vector3));
			}
		}

		// Token: 0x06005D10 RID: 23824 RVA: 0x001D6AA0 File Offset: 0x001D4CA0
		private Vector2 GetSpawnPositionWithClearance(Vector2 inputPosition, float inputSize, float maxDistance, Vector3 lavaSurfaceOrigin)
		{
			Vector2 vector = inputPosition;
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				Vector3 vector2 = this.activeBubbles[i].position - lavaSurfaceOrigin;
				Vector2 b = new Vector2(vector2.x, vector2.z);
				Vector2 a = vector - b;
				float num = (inputSize + this.activeBubbles[i].spawnSize * this.scaleFactor) * 0.5f;
				if (a.sqrMagnitude < num * num)
				{
					float magnitude = a.magnitude;
					if (magnitude > 0.001f)
					{
						Vector2 a2 = a / magnitude;
						vector += a2 * (num - magnitude);
						if (vector.sqrMagnitude > maxDistance * maxDistance)
						{
							vector = vector.normalized * maxDistance;
						}
					}
				}
			}
			if (vector.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
			{
				vector = vector.normalized * this.surfaceRadiusSpawnRange.y;
			}
			return vector;
		}

		// Token: 0x06005D11 RID: 23825 RVA: 0x001D6BB3 File Offset: 0x001D4DB3
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<ScienceExperimentPlatformGenerator>(this, "liquidSurfacePlane", ref this.liquidSurfacePlane_gRef);
			GuidedRefHub.ReceiverFullyRegistered<ScienceExperimentPlatformGenerator>(this);
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x06005D12 RID: 23826 RVA: 0x001D6BCC File Offset: 0x001D4DCC
		// (set) Token: 0x06005D13 RID: 23827 RVA: 0x001D6BD4 File Offset: 0x001D4DD4
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06005D14 RID: 23828 RVA: 0x001D6BDD File Offset: 0x001D4DDD
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<ScienceExperimentPlatformGenerator, Transform>(this, ref this.liquidSurfacePlane, this.liquidSurfacePlane_gRef, target);
		}

		// Token: 0x06005D15 RID: 23829 RVA: 0x001D6BF2 File Offset: 0x001D4DF2
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			if (!base.enabled)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005D16 RID: 23830 RVA: 0x00100D37 File Offset: 0x000FEF37
		void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005D18 RID: 23832 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040066E5 RID: 26341
		[SerializeField]
		private GameObject spawnedPrefab;

		// Token: 0x040066E6 RID: 26342
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x040066E7 RID: 26343
		[Header("Random Bubbles")]
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x040066E8 RID: 26344
		[SerializeField]
		private Vector2 lifetimeRange = new Vector2(5f, 10f);

		// Token: 0x040066E9 RID: 26345
		[SerializeField]
		private Vector2 sizeRange = new Vector2(0.5f, 2f);

		// Token: 0x040066EA RID: 26346
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066EB RID: 26347
		[SerializeField]
		[FormerlySerializedAs("rockCountMultiplier")]
		private float bubbleCountMultiplier = 80f;

		// Token: 0x040066EC RID: 26348
		[SerializeField]
		private int maxBubbleCount = 100;

		// Token: 0x040066ED RID: 26349
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040066EE RID: 26350
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040066EF RID: 26351
		[SerializeField]
		private AnimationCurve spawnRadiusMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040066F0 RID: 26352
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066F1 RID: 26353
		[Header("Bubble Trails")]
		[SerializeField]
		private AnimationCurve trailSpawnRateVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066F2 RID: 26354
		[SerializeField]
		private float trailSpawnRateMultiplier = 1f;

		// Token: 0x040066F3 RID: 26355
		[SerializeField]
		private AnimationCurve trailBubbleLifetimeVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066F4 RID: 26356
		[SerializeField]
		private AnimationCurve trailBubbleBoundaryRadiusVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066F5 RID: 26357
		[SerializeField]
		private float trailBubbleLifetimeMultiplier = 6f;

		// Token: 0x040066F6 RID: 26358
		[SerializeField]
		private float trailDistanceBetweenSpawns = 3f;

		// Token: 0x040066F7 RID: 26359
		[SerializeField]
		private float trailMaxTurnAngle = 55f;

		// Token: 0x040066F8 RID: 26360
		[SerializeField]
		private float trailBubbleSize = 1.5f;

		// Token: 0x040066F9 RID: 26361
		[SerializeField]
		private AnimationCurve trailCountVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040066FA RID: 26362
		[SerializeField]
		private float trailCountMultiplier = 12f;

		// Token: 0x040066FB RID: 26363
		[SerializeField]
		private Vector2 trailEdgeAvoidanceSpawnsMinMax = new Vector2(3f, 1f);

		// Token: 0x040066FC RID: 26364
		[Header("Feedback Effects")]
		[SerializeField]
		private float bubblePopAnticipationTime = 2f;

		// Token: 0x040066FD RID: 26365
		[SerializeField]
		private float bubblePopWobbleFrequency = 25f;

		// Token: 0x040066FE RID: 26366
		[SerializeField]
		private float bubblePopWobbleAmplitude = 0.01f;

		// Token: 0x040066FF RID: 26367
		[SerializeField]
		private Transform liquidSurfacePlane;

		// Token: 0x04006700 RID: 26368
		[SerializeField]
		private GuidedRefReceiverFieldInfo liquidSurfacePlane_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04006701 RID: 26369
		private List<ScienceExperimentPlatformGenerator.BubbleData> activeBubbles = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04006702 RID: 26370
		private List<ScienceExperimentPlatformGenerator.BubbleData> trailHeads = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04006703 RID: 26371
		private List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug> bubbleSpawnDebug = new List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug>();

		// Token: 0x04006704 RID: 26372
		private ScienceExperimentManager scienceExperimentManager;

		// Token: 0x02000E83 RID: 3715
		private struct BubbleData
		{
			// Token: 0x04006707 RID: 26375
			public Vector3 position;

			// Token: 0x04006708 RID: 26376
			public Vector3 direction;

			// Token: 0x04006709 RID: 26377
			public float spawnSize;

			// Token: 0x0400670A RID: 26378
			public float lifetime;

			// Token: 0x0400670B RID: 26379
			public double spawnTime;

			// Token: 0x0400670C RID: 26380
			public bool isTrail;

			// Token: 0x0400670D RID: 26381
			public SodaBubble bubble;
		}

		// Token: 0x02000E84 RID: 3716
		private struct BubbleSpawnDebug
		{
			// Token: 0x0400670E RID: 26382
			public Vector3 initialPosition;

			// Token: 0x0400670F RID: 26383
			public Vector3 initialDirection;

			// Token: 0x04006710 RID: 26384
			public Vector3 spawnPosition;

			// Token: 0x04006711 RID: 26385
			public float minAngle;

			// Token: 0x04006712 RID: 26386
			public float maxAngle;

			// Token: 0x04006713 RID: 26387
			public float edgeCorrectionAngle;

			// Token: 0x04006714 RID: 26388
			public double spawnTime;
		}
	}
}
