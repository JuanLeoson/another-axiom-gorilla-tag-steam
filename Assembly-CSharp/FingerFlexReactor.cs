using System;
using UnityEngine;

// Token: 0x02000583 RID: 1411
public class FingerFlexReactor : MonoBehaviour
{
	// Token: 0x06002269 RID: 8809 RVA: 0x000B9B6C File Offset: 0x000B7D6C
	private void Setup()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._fingers = new VRMap[]
		{
			this._rig.leftThumb,
			this._rig.leftIndex,
			this._rig.leftMiddle,
			this._rig.rightThumb,
			this._rig.rightIndex,
			this._rig.rightMiddle
		};
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000B9BF3 File Offset: 0x000B7DF3
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000B9BFB File Offset: 0x000B7DFB
	private void FixedUpdate()
	{
		this.UpdateBlendShapes();
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000B9C04 File Offset: 0x000B7E04
	public void UpdateBlendShapes()
	{
		if (!this._rig)
		{
			return;
		}
		if (this._blendShapeTargets == null || this._fingers == null)
		{
			return;
		}
		if (this._blendShapeTargets.Length == 0 || this._fingers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._blendShapeTargets.Length; i++)
		{
			FingerFlexReactor.BlendShapeTarget blendShapeTarget = this._blendShapeTargets[i];
			if (blendShapeTarget != null)
			{
				int sourceFinger = (int)blendShapeTarget.sourceFinger;
				if (sourceFinger != -1)
				{
					SkinnedMeshRenderer targetRenderer = blendShapeTarget.targetRenderer;
					if (targetRenderer)
					{
						float lerpValue = FingerFlexReactor.GetLerpValue(this._fingers[sourceFinger]);
						Vector2 inputRange = blendShapeTarget.inputRange;
						Vector2 outputRange = blendShapeTarget.outputRange;
						float num = MathUtils.Linear(lerpValue, inputRange.x, inputRange.y, outputRange.x, outputRange.y);
						blendShapeTarget.currentValue = num;
						targetRenderer.SetBlendShapeWeight(blendShapeTarget.blendShapeIndex, num);
					}
				}
			}
		}
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000B9CD8 File Offset: 0x000B7ED8
	private static float GetLerpValue(VRMap map)
	{
		VRMapThumb vrmapThumb = map as VRMapThumb;
		float result;
		if (vrmapThumb == null)
		{
			VRMapIndex vrmapIndex = map as VRMapIndex;
			if (vrmapIndex == null)
			{
				VRMapMiddle vrmapMiddle = map as VRMapMiddle;
				if (vrmapMiddle == null)
				{
					result = 0f;
				}
				else
				{
					result = vrmapMiddle.calcT;
				}
			}
			else
			{
				result = vrmapIndex.calcT;
			}
		}
		else
		{
			result = ((vrmapThumb.calcT > 0.1f) ? 1f : 0f);
		}
		return result;
	}

	// Token: 0x04002BDF RID: 11231
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04002BE0 RID: 11232
	[SerializeField]
	private VRMap[] _fingers = new VRMap[0];

	// Token: 0x04002BE1 RID: 11233
	[SerializeField]
	private FingerFlexReactor.BlendShapeTarget[] _blendShapeTargets = new FingerFlexReactor.BlendShapeTarget[0];

	// Token: 0x02000584 RID: 1412
	[Serializable]
	public class BlendShapeTarget
	{
		// Token: 0x04002BE2 RID: 11234
		public FingerFlexReactor.FingerMap sourceFinger;

		// Token: 0x04002BE3 RID: 11235
		public SkinnedMeshRenderer targetRenderer;

		// Token: 0x04002BE4 RID: 11236
		public int blendShapeIndex;

		// Token: 0x04002BE5 RID: 11237
		public Vector2 inputRange = new Vector2(0f, 1f);

		// Token: 0x04002BE6 RID: 11238
		public Vector2 outputRange = new Vector2(0f, 1f);

		// Token: 0x04002BE7 RID: 11239
		[NonSerialized]
		public float currentValue;
	}

	// Token: 0x02000585 RID: 1413
	public enum FingerMap
	{
		// Token: 0x04002BE9 RID: 11241
		None = -1,
		// Token: 0x04002BEA RID: 11242
		LeftThumb,
		// Token: 0x04002BEB RID: 11243
		LeftIndex,
		// Token: 0x04002BEC RID: 11244
		LeftMiddle,
		// Token: 0x04002BED RID: 11245
		RightThumb,
		// Token: 0x04002BEE RID: 11246
		RightIndex,
		// Token: 0x04002BEF RID: 11247
		RightMiddle
	}
}
