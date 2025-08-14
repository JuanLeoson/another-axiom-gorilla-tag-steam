using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class GTPosRotConstraints : MonoBehaviour, ISpawnable
{
	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000C36 RID: 3126 RVA: 0x000422B8 File Offset: 0x000404B8
	// (set) Token: 0x06000C37 RID: 3127 RVA: 0x000422C0 File Offset: 0x000404C0
	public bool IsSpawned { get; set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000C38 RID: 3128 RVA: 0x000422C9 File Offset: 0x000404C9
	// (set) Token: 0x06000C39 RID: 3129 RVA: 0x000422D1 File Offset: 0x000404D1
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000C3A RID: 3130 RVA: 0x000422DC File Offset: 0x000404DC
	void ISpawnable.OnSpawn(VRRig rig)
	{
		Transform[] array = Array.Empty<Transform>();
		string str;
		if (rig != null && !GTHardCodedBones.TryGetBoneXforms(rig, out array, out str))
		{
			Debug.LogError("GTPosRotConstraints: Error getting bone Transforms: " + str, this);
			return;
		}
		for (int i = 0; i < this.constraints.Length; i++)
		{
			GorillaPosRotConstraint gorillaPosRotConstraint = this.constraints[i];
			if (Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.x, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.y, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.z, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.w, 0f))
			{
				gorillaPosRotConstraint.rotationOffset = Quaternion.identity;
			}
			if (!gorillaPosRotConstraint.follower)
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("{0}: Disabling component! At index {1}, Transform `follower` is ", "GTPosRotConstraints", i),
					"null. Affected component path: ",
					base.transform.GetPathQ(),
					"\n- Affected component path: ",
					base.transform.GetPathQ()
				}), this);
				base.enabled = false;
				return;
			}
			if (gorillaPosRotConstraint.sourceGorillaBone == GTHardCodedBones.EBone.None)
			{
				if (!gorillaPosRotConstraint.source)
				{
					if (string.IsNullOrEmpty(gorillaPosRotConstraint.sourceRelativePath))
					{
						Debug.LogError(string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i) + "null, not EBone, and `sourceRelativePath` is null or empty.\n- Affected component path: " + base.transform.GetPathQ(), this);
						base.enabled = false;
						return;
					}
					if (!base.transform.TryFindByPath(gorillaPosRotConstraint.sourceRelativePath, out gorillaPosRotConstraint.source, false))
					{
						Debug.LogError(string.Concat(new string[]
						{
							string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i),
							"null, not EBone, and could not find by path: \"",
							gorillaPosRotConstraint.sourceRelativePath,
							"\"\n- Affected component path: ",
							base.transform.GetPathQ()
						}), this);
						base.enabled = false;
						return;
					}
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
			else
			{
				if (rig == null)
				{
					Debug.LogError("GTPosRotConstraints: Disabling component! `VRRig` could not be found in parents, but " + string.Format("bone at index {0} is set to use EBone `{1}` but without `VRRig` it cannot ", i, gorillaPosRotConstraint.sourceGorillaBone) + "be resolved.\n- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				int boneIndex = GTHardCodedBones.GetBoneIndex(gorillaPosRotConstraint.sourceGorillaBone);
				if (boneIndex <= 0)
				{
					Debug.LogError(string.Format("{0}: (should never happen) Disabling component! At index {1}, could ", "GTPosRotConstraints", i) + string.Format("not find EBone `{0}`.\n", gorillaPosRotConstraint.sourceGorillaBone) + "- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				gorillaPosRotConstraint.source = array[boneIndex];
				if (!gorillaPosRotConstraint.source)
				{
					Debug.LogError(string.Concat(new string[]
					{
						string.Format("{0}: Disabling component! At index {1}, bone {2} was ", "GTPosRotConstraints", i, gorillaPosRotConstraint.sourceGorillaBone),
						"not present in `VRRig` path: ",
						rig.transform.GetPathQ(),
						"\n- Affected component path: ",
						base.transform.GetPathQ()
					}), this);
					base.enabled = false;
					return;
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
		}
		if (base.isActiveAndEnabled)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x0004264B File Offset: 0x0004084B
	protected void OnEnable()
	{
		if (this.IsSpawned)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0004265B File Offset: 0x0004085B
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x04000F12 RID: 3858
	public GorillaPosRotConstraint[] constraints;
}
