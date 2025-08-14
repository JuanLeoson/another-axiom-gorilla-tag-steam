using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AF7 RID: 2807
public static class TransformUtils
{
	// Token: 0x060043A9 RID: 17321 RVA: 0x0015444C File Offset: 0x0015264C
	public static int ComputePathHashByInstance(Transform t)
	{
		if (t == null)
		{
			return 0;
		}
		int num = 0;
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.GetHashCode());
			transform = transform.parent;
		}
		return num;
	}

	// Token: 0x060043AA RID: 17322 RVA: 0x00154488 File Offset: 0x00152688
	public static Hash128 ComputePathHash(Transform t)
	{
		if (t == null)
		{
			return default(Hash128);
		}
		Hash128 result = default(Hash128);
		Transform transform = t;
		while (transform != null)
		{
			Hash128 hash = Hash128.Compute(transform.name);
			HashUtilities.AppendHash(ref hash, ref result);
			transform = transform.parent;
		}
		return result;
	}

	// Token: 0x060043AB RID: 17323 RVA: 0x001544DC File Offset: 0x001526DC
	public static string GetScenePath(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	// Token: 0x060043AC RID: 17324 RVA: 0x00154528 File Offset: 0x00152728
	public static string GetScenePathReverse(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		Queue<string> queue = new Queue<string>(16);
		while (parent != null)
		{
			queue.Enqueue(parent.name);
			parent = parent.parent;
		}
		while (queue.Count > 0)
		{
			text = text + "/" + queue.Dequeue();
		}
		return text;
	}

	// Token: 0x04004E38 RID: 20024
	private const string kFwdSlash = "/";
}
