using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class HoseSimulator : MonoBehaviour, ISpawnable
{
	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000A9D RID: 2717 RVA: 0x00039257 File Offset: 0x00037457
	// (set) Token: 0x06000A9E RID: 2718 RVA: 0x0003925F File Offset: 0x0003745F
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00039268 File Offset: 0x00037468
	// (set) Token: 0x06000AA0 RID: 2720 RVA: 0x00039270 File Offset: 0x00037470
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000AA1 RID: 2721 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0003927C File Offset: 0x0003747C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchors = rig.cosmeticReferences.Get(this.startAnchorRef).GetComponent<HoseSimulatorAnchors>();
		if (this.skinnedMeshRenderer != null)
		{
			Bounds localBounds = this.skinnedMeshRenderer.localBounds;
			localBounds.extents = this.localBoundsOverride;
			this.skinnedMeshRenderer.localBounds = localBounds;
		}
		this.hoseSectionLengths = new float[this.hoseBones.Length - 1];
		this.hoseBonePositions = new Vector3[this.hoseBones.Length];
		this.hoseBoneVelocities = new Vector3[this.hoseBones.Length];
		for (int i = 0; i < this.hoseSectionLengths.Length; i++)
		{
			float num = 1f;
			this.hoseSectionLengths[i] = num;
			this.totalHoseLength += num;
		}
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00039344 File Offset: 0x00037544
	private void LateUpdate()
	{
		if (this.myHoldable.InLeftHand())
		{
			this.isLeftHanded = true;
		}
		else if (this.myHoldable.InRightHand())
		{
			this.isLeftHanded = false;
		}
		for (int i = 0; i < this.miscBones.Length; i++)
		{
			Transform transform = this.isLeftHanded ? this.anchors.miscAnchorsLeft[i] : this.anchors.miscAnchorsRight[i];
			this.miscBones[i].transform.position = transform.position;
			this.miscBones[i].transform.rotation = transform.rotation;
		}
		this.startAnchor = (this.isLeftHanded ? this.anchors.leftAnchorPoint : this.anchors.rightAnchorPoint);
		float x = this.myHoldable.transform.lossyScale.x;
		float num = 0f;
		Vector3 position = this.startAnchor.position;
		Vector3 ctrl = position + this.startAnchor.forward * this.startStiffness * x;
		Vector3 position2 = this.endAnchor.position;
		Vector3 ctrl2 = position2 - this.endAnchor.forward * this.endStiffness * x;
		for (int j = 0; j < this.hoseBones.Length; j++)
		{
			float num2 = num / this.totalHoseLength;
			Vector3 vector = BezierUtils.BezierSolve(num2, position, ctrl, ctrl2, position2);
			Vector3 a = BezierUtils.BezierSolve(num2 + 0.1f, position, ctrl, ctrl2, position2);
			if (this.firstUpdate)
			{
				this.hoseBones[j].transform.position = vector;
				this.hoseBonePositions[j] = vector;
				this.hoseBoneVelocities[j] = Vector3.zero;
			}
			else
			{
				this.hoseBoneVelocities[j] *= this.damping;
				this.hoseBonePositions[j] += this.hoseBoneVelocities[j] * Time.deltaTime;
				float num3 = this.hoseBoneMaxDisplacement[j] * x;
				if ((vector - this.hoseBonePositions[j]).IsLongerThan(num3))
				{
					Vector3 vector2 = vector + (this.hoseBonePositions[j] - vector).normalized * num3;
					this.hoseBoneVelocities[j] += (vector2 - this.hoseBonePositions[j]) / Time.deltaTime;
					this.hoseBonePositions[j] = vector2;
				}
				this.hoseBones[j].transform.position = this.hoseBonePositions[j];
			}
			this.hoseBones[j].transform.rotation = Quaternion.LookRotation(a - vector, this.endAnchor.transform.up);
			if (j < this.hoseSectionLengths.Length)
			{
				num += this.hoseSectionLengths[j];
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00039682 File Offset: 0x00037882
	private void OnDrawGizmosSelected()
	{
		if (this.hoseBonePositions != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLineStrip(this.hoseBonePositions, false);
		}
	}

	// Token: 0x04000CF6 RID: 3318
	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000CF7 RID: 3319
	[SerializeField]
	private Vector3 localBoundsOverride;

	// Token: 0x04000CF8 RID: 3320
	[SerializeField]
	private Transform[] miscBones;

	// Token: 0x04000CF9 RID: 3321
	[SerializeField]
	private Transform[] hoseBones;

	// Token: 0x04000CFA RID: 3322
	[SerializeField]
	private float[] hoseBoneMaxDisplacement;

	// Token: 0x04000CFB RID: 3323
	[SerializeField]
	private CosmeticRefID startAnchorRef;

	// Token: 0x04000CFC RID: 3324
	private Transform startAnchor;

	// Token: 0x04000CFD RID: 3325
	[SerializeField]
	private float startStiffness = 0.5f;

	// Token: 0x04000CFE RID: 3326
	[SerializeField]
	private Transform endAnchor;

	// Token: 0x04000CFF RID: 3327
	[SerializeField]
	private float endStiffness = 0.5f;

	// Token: 0x04000D00 RID: 3328
	private Vector3[] hoseBonePositions;

	// Token: 0x04000D01 RID: 3329
	private Vector3[] hoseBoneVelocities;

	// Token: 0x04000D02 RID: 3330
	[SerializeField]
	private float damping = 0.97f;

	// Token: 0x04000D03 RID: 3331
	private float[] hoseSectionLengths;

	// Token: 0x04000D04 RID: 3332
	private float totalHoseLength;

	// Token: 0x04000D05 RID: 3333
	private bool firstUpdate = true;

	// Token: 0x04000D06 RID: 3334
	private HoseSimulatorAnchors anchors;

	// Token: 0x04000D07 RID: 3335
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04000D08 RID: 3336
	private bool isLeftHanded;
}
