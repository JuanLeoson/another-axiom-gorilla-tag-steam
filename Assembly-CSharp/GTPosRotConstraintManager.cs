using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000202 RID: 514
[DefaultExecutionOrder(1300)]
public class GTPosRotConstraintManager : MonoBehaviour
{
	// Token: 0x06000C2C RID: 3116 RVA: 0x00041CB6 File Offset: 0x0003FEB6
	protected void Awake()
	{
		if (GTPosRotConstraintManager.hasInstance && GTPosRotConstraintManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GTPosRotConstraintManager.SetInstance(this);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00041CD9 File Offset: 0x0003FED9
	protected void OnDestroy()
	{
		if (GTPosRotConstraintManager.instance == this)
		{
			GTPosRotConstraintManager.hasInstance = false;
			GTPosRotConstraintManager.instance = null;
		}
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00041CF4 File Offset: 0x0003FEF4
	public void InvokeConstraint(GorillaPosRotConstraint constraint, int index)
	{
		Transform source = constraint.source;
		Transform follower = constraint.follower;
		Vector3 position = source.position + source.TransformVector(constraint.positionOffset);
		Quaternion rotation = source.rotation * constraint.rotationOffset;
		follower.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00041D40 File Offset: 0x0003FF40
	protected void LateUpdate()
	{
		if (this.constraintsToDisable.Count <= 0)
		{
			return;
		}
		for (int i = this.constraintsToDisable.Count - 1; i >= 0; i--)
		{
			for (int j = 0; j < this.constraintsToDisable[i].constraints.Length; j++)
			{
				Transform follower = this.constraintsToDisable[i].constraints[j].follower;
				if (this.originalParent.ContainsKey(follower))
				{
					follower.SetParent(this.originalParent[follower], true);
					follower.localRotation = this.originalRot[follower];
					follower.localPosition = this.originalOffset[follower];
					follower.localScale = this.originalScale[follower];
					this.InvokeConstraint(this.constraintsToDisable[i].constraints[j], i);
				}
			}
			this.constraintsToDisable.RemoveAt(i);
		}
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x00041E3C File Offset: 0x0004003C
	public static void CreateManager()
	{
		GTPosRotConstraintManager gtposRotConstraintManager = new GameObject("GTPosRotConstraintManager").AddComponent<GTPosRotConstraintManager>();
		GTPosRotConstraintManager.constraints.Clear();
		GTPosRotConstraintManager.componentRanges.Clear();
		GTPosRotConstraintManager.SetInstance(gtposRotConstraintManager);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x00041E68 File Offset: 0x00040068
	private static void SetInstance(GTPosRotConstraintManager manager)
	{
		GTPosRotConstraintManager.instance = manager;
		GTPosRotConstraintManager.hasInstance = true;
		GTPosRotConstraintManager.instance.originalParent = new Dictionary<Transform, Transform>();
		GTPosRotConstraintManager.instance.originalOffset = new Dictionary<Transform, Vector3>();
		GTPosRotConstraintManager.instance.originalScale = new Dictionary<Transform, Vector3>();
		GTPosRotConstraintManager.instance.originalRot = new Dictionary<Transform, Quaternion>();
		GTPosRotConstraintManager.instance.constraintsToDisable = new List<GTPosRotConstraints>();
		if (Application.isPlaying)
		{
			manager.transform.SetParent(null, false);
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00041EE8 File Offset: 0x000400E8
	public static void Register(GTPosRotConstraints component)
	{
		if (!GTPosRotConstraintManager.hasInstance)
		{
			GTPosRotConstraintManager.CreateManager();
		}
		int instanceID = component.GetInstanceID();
		if (GTPosRotConstraintManager.componentRanges.ContainsKey(instanceID))
		{
			return;
		}
		for (int i = 0; i < component.constraints.Length; i++)
		{
			if (!component.constraints[i].follower)
			{
				Debug.LogError("Cannot add constraints for GTPosRotConstraints component because the `follower` Transform is null " + string.Format("at index {0}. Path in scene: {1}", i, component.transform.GetPathQ()), component);
				return;
			}
			if (!component.constraints[i].source)
			{
				Debug.LogError("Cannot add constraints for GTPosRotConstraints component because the `source` Transform is null " + string.Format("at index {0}. Path in scene: {1}", i, component.transform.GetPathQ()), component);
				return;
			}
		}
		GTPosRotConstraintManager.Range value = new GTPosRotConstraintManager.Range
		{
			start = GTPosRotConstraintManager.constraints.Count,
			end = GTPosRotConstraintManager.constraints.Count + component.constraints.Length - 1
		};
		GTPosRotConstraintManager.componentRanges.Add(instanceID, value);
		GTPosRotConstraintManager.constraints.AddRange(component.constraints);
		if (GTPosRotConstraintManager.instance.constraintsToDisable.Contains(component))
		{
			GTPosRotConstraintManager.instance.constraintsToDisable.Remove(component);
		}
		for (int j = 0; j < component.constraints.Length; j++)
		{
			Transform follower = component.constraints[j].follower;
			if (GTPosRotConstraintManager.instance.originalParent.ContainsKey(follower))
			{
				component.constraints[j].follower.SetParent(GTPosRotConstraintManager.instance.originalParent[follower], true);
				follower.localRotation = GTPosRotConstraintManager.instance.originalRot[follower];
				follower.localPosition = GTPosRotConstraintManager.instance.originalOffset[follower];
				follower.localScale = GTPosRotConstraintManager.instance.originalScale[follower];
			}
			else
			{
				GTPosRotConstraintManager.instance.originalParent[follower] = follower.parent;
				GTPosRotConstraintManager.instance.originalRot[follower] = follower.localRotation;
				GTPosRotConstraintManager.instance.originalOffset[follower] = follower.localPosition;
				GTPosRotConstraintManager.instance.originalScale[follower] = follower.localScale;
			}
			GTPosRotConstraintManager.instance.InvokeConstraint(component.constraints[j], j);
			component.constraints[j].follower.SetParent(component.constraints[j].source);
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x00042188 File Offset: 0x00040388
	public static void Unregister(GTPosRotConstraints component)
	{
		int instanceID = component.GetInstanceID();
		GTPosRotConstraintManager.Range range;
		if (!GTPosRotConstraintManager.hasInstance || !GTPosRotConstraintManager.componentRanges.TryGetValue(instanceID, out range))
		{
			return;
		}
		GTPosRotConstraintManager.constraints.RemoveRange(range.start, 1 + range.end - range.start);
		GTPosRotConstraintManager.componentRanges.Remove(instanceID);
		foreach (int key in GTPosRotConstraintManager.componentRanges.Keys.ToArray<int>())
		{
			GTPosRotConstraintManager.Range range2 = GTPosRotConstraintManager.componentRanges[key];
			if (range2.start > range.end)
			{
				GTPosRotConstraintManager.componentRanges[key] = new GTPosRotConstraintManager.Range
				{
					start = range2.start - range.end + range.start - 1,
					end = range2.end - range.end + range.start - 1
				};
			}
		}
		if (!GTPosRotConstraintManager.instance.constraintsToDisable.Contains(component))
		{
			GTPosRotConstraintManager.instance.constraintsToDisable.Add(component);
		}
	}

	// Token: 0x04000F05 RID: 3845
	public static GTPosRotConstraintManager instance;

	// Token: 0x04000F06 RID: 3846
	public static bool hasInstance = false;

	// Token: 0x04000F07 RID: 3847
	private const int _kComponentsCapacity = 256;

	// Token: 0x04000F08 RID: 3848
	private const int _kConstraintsCapacity = 1024;

	// Token: 0x04000F09 RID: 3849
	[NonSerialized]
	public Dictionary<Transform, Transform> originalParent;

	// Token: 0x04000F0A RID: 3850
	[NonSerialized]
	public Dictionary<Transform, Vector3> originalOffset;

	// Token: 0x04000F0B RID: 3851
	[NonSerialized]
	public Dictionary<Transform, Vector3> originalScale;

	// Token: 0x04000F0C RID: 3852
	[NonSerialized]
	public Dictionary<Transform, Quaternion> originalRot;

	// Token: 0x04000F0D RID: 3853
	[NonSerialized]
	public List<GTPosRotConstraints> constraintsToDisable;

	// Token: 0x04000F0E RID: 3854
	[OnEnterPlay_Clear]
	private static readonly List<GorillaPosRotConstraint> constraints = new List<GorillaPosRotConstraint>(1024);

	// Token: 0x04000F0F RID: 3855
	[OnEnterPlay_Clear]
	public static readonly Dictionary<int, GTPosRotConstraintManager.Range> componentRanges = new Dictionary<int, GTPosRotConstraintManager.Range>(256);

	// Token: 0x02000203 RID: 515
	public struct Range
	{
		// Token: 0x04000F10 RID: 3856
		public int start;

		// Token: 0x04000F11 RID: 3857
		public int end;
	}
}
