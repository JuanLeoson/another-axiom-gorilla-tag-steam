using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class PlayableBoundaryManager : MonoBehaviour
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600091A RID: 2330 RVA: 0x00031DFA File Offset: 0x0002FFFA
	// (set) Token: 0x0600091B RID: 2331 RVA: 0x00031E12 File Offset: 0x00030012
	public static bool ShouldRender
	{
		get
		{
			return Shader.GetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled) > 0f;
		}
		set
		{
			Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00031E2B File Offset: 0x0003002B
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00031E3C File Offset: 0x0003003C
	public void Setup()
	{
		Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
		Vector3 position = base.transform.position;
		SRand srand = new SRand(StaticHash.Compute(position.x, position.y, position.z));
		this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
		this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
		for (int i = 1; i < 8; i++)
		{
			Vector3 vector = position + srand.NextPointInsideSphere(this.m_bigCylinderRadius * this.radiusScale);
			this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
			this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00031F54 File Offset: 0x00030154
	private void OnEnable()
	{
		PlayableBoundaryManager.ShouldRender = true;
		this.Setup();
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00031F62 File Offset: 0x00030162
	private void OnDisable()
	{
		PlayableBoundaryManager.ShouldRender = false;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00031F6C File Offset: 0x0003016C
	public unsafe void UpdateSim()
	{
		if (Time.frameCount == this._lastFrameUpdated)
		{
			return;
		}
		this._lastFrameUpdated = Time.frameCount;
		Vector4[] array = this._cylinders_centers;
		if (array != null && array.Length == 8)
		{
			array = this._cylinders_radiusHeights;
			if (array != null && array.Length == 8)
			{
				if (this.m_smallCylindersMoveTimeScale > 0.0)
				{
					Vector3 position = base.transform.position;
					float d = (float)((double)(GTTime.TimeAsMilliseconds() % 86400000L) * this.m_smallCylindersMoveTimeScale / 1000.0);
					this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
					this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
					for (int i = 1; i < 8; i++)
					{
						float num = (float)i * 0.125f;
						Vector3 v = *PlayableBoundaryManager.Hash3(num * 1.17f) + *PlayableBoundaryManager.Hash3(num * 13.7f) * d;
						Vector3 vector = position + v.Sin() * this.m_bigCylinderRadius * this.radiusScale;
						this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
						this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
					}
				}
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_Centers, this._cylinders_centers);
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_RadiusHeights, this._cylinders_radiusHeights);
				for (int j = 0; j < this.tracked.Count; j++)
				{
					PlayableBoundaryTracker playableBoundaryTracker = this.tracked[j];
					if (playableBoundaryTracker)
					{
						playableBoundaryTracker.UpdateSignedDistanceToBoundary(this._GetSignedDistanceToBoundary(playableBoundaryTracker.transform.position, playableBoundaryTracker.radius), Time.deltaTime);
					}
				}
				Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
				return;
			}
		}
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x000321A8 File Offset: 0x000303A8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _GetSignedDistanceToBoundary(float3 tracked_center, float tracked_radius)
	{
		float num = float.MaxValue;
		float smoothFactor = this.GetSmoothFactor();
		for (int i = 0; i < 8; i++)
		{
			float3 @float = this._cylinders_centers[i].xyz - tracked_center;
			float x = this._cylinders_radiusHeights[i].x;
			float signedDist = math.length(@float.xz) - x;
			num = this.SDFSmoothMerge(num, signedDist, smoothFactor);
		}
		return num - tracked_radius;
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00032224 File Offset: 0x00030424
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float SDFSmoothMerge(float signedDist1, float signedDist2, float smoothRadius)
	{
		float num = -math.length(math.min(new float2(signedDist1 - smoothRadius, signedDist2 - smoothRadius), new float2(0f, 0f)));
		float num2 = math.max(math.min(signedDist1, signedDist2), smoothRadius);
		return num + num2;
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x00032268 File Offset: 0x00030468
	private static ref Vector3 Hash3(float n)
	{
		PlayableBoundaryManager.kHashVec.x = Mathf.Sin(n) * 43758.547f % 1f;
		PlayableBoundaryManager.kHashVec.y = Mathf.Sin(n + 1f) * 22578.146f % 1f;
		PlayableBoundaryManager.kHashVec.z = Mathf.Sin(n + 2f) * 19642.35f % 1f;
		return ref PlayableBoundaryManager.kHashVec;
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000322DC File Offset: 0x000304DC
	private float GetSmoothFactor()
	{
		float num = this.m_smoothFactor;
		if (this.m_bigCylinderRadius <= 1f)
		{
			num *= math.max(this.m_bigCylinderRadius, 0f);
		}
		return math.max(num, 1E-06f);
	}

	// Token: 0x04000AC5 RID: 2757
	public List<PlayableBoundaryTracker> tracked = new List<PlayableBoundaryTracker>(10);

	// Token: 0x04000AC6 RID: 2758
	[Space]
	[Range(0f, 128f)]
	public float m_bigCylinderRadius = 8f;

	// Token: 0x04000AC7 RID: 2759
	public float m_smoothFactor = 1.5f;

	// Token: 0x04000AC8 RID: 2760
	public float m_smallCylindersRadius = 3f;

	// Token: 0x04000AC9 RID: 2761
	[SerializeField]
	private double m_smallCylindersMoveTimeScale = 0.25;

	// Token: 0x04000ACA RID: 2762
	[Space]
	private readonly Vector4[] _cylinders_centers = new Vector4[8];

	// Token: 0x04000ACB RID: 2763
	private readonly Vector4[] _cylinders_radiusHeights = new Vector4[8];

	// Token: 0x04000ACC RID: 2764
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_Centers = "_GTGameModes_PlayableBoundary_Cylinders_Centers";

	// Token: 0x04000ACD RID: 2765
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_RadiusHeights = "_GTGameModes_PlayableBoundary_Cylinders_RadiusHeights";

	// Token: 0x04000ACE RID: 2766
	private static ShaderHashId _GTGameModes_PlayableBoundary_NonZeroSmoothRadius = "_GTGameModes_PlayableBoundary_NonZeroSmoothRadius";

	// Token: 0x04000ACF RID: 2767
	private static ShaderHashId _GTGameModes_PlayableBoundary_IsEnabled = "_GTGameModes_PlayableBoundary_IsEnabled";

	// Token: 0x04000AD0 RID: 2768
	private const int _k_cylinders_count = 8;

	// Token: 0x04000AD1 RID: 2769
	[NonSerialized]
	public float radiusScale = 1f;

	// Token: 0x04000AD2 RID: 2770
	private int _lastFrameUpdated = -1;

	// Token: 0x04000AD3 RID: 2771
	private static Vector3 kHashVec = Vector3.zero;
}
