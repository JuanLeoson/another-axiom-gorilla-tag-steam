using System;
using System.Collections.Generic;
using Drawing;
using GorillaTag;
using UnityEngine;

// Token: 0x02000895 RID: 2197
public class VolumeCast : MonoBehaviourGizmos
{
	// Token: 0x06003767 RID: 14183 RVA: 0x0011F6CC File Offset: 0x0011D8CC
	public bool CheckOverlaps()
	{
		Transform transform = base.transform;
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		int num = (int)this.physicsMask;
		QueryTriggerInteraction queryTriggerInteraction = this.includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
		Vector3 vector;
		Vector3 vector2;
		float num2;
		VolumeCast.GetEndsAndRadius(transform, this.center, this.height, this.radius, out vector, out vector2, out num2);
		VolumeCast.VolumeShape volumeShape = this.shape;
		Vector3 vector3;
		Vector3 halfExtents;
		if (volumeShape != VolumeCast.VolumeShape.Box)
		{
			if (volumeShape != VolumeCast.VolumeShape.Cylinder)
			{
				return false;
			}
			vector3 = (vector + vector2) * 0.5f;
			halfExtents = new Vector3(num2, Vector3.Distance(vector, vector2) * 0.5f, num2);
		}
		else
		{
			vector3 = transform.TransformPoint(this.center);
			halfExtents = Vector3.Scale(lossyScale, this.size * 0.5f).Abs();
		}
		Array.Clear(this._boxOverlaps, 0, 8);
		this._boxHits = Physics.OverlapBoxNonAlloc(vector3, halfExtents, this._boxOverlaps, rotation, num, queryTriggerInteraction);
		if (this.shape != VolumeCast.VolumeShape.Cylinder)
		{
			return this._colliding = (this._boxHits > 0);
		}
		this._hits = 0;
		Array.Clear(this._capOverlaps, 0, 8);
		Array.Clear(this._overlaps, 0, 8);
		this._capHits = Physics.OverlapCapsuleNonAlloc(vector, vector2, num2, this._capOverlaps, num, queryTriggerInteraction);
		this._set.Clear();
		int num3 = Math.Max(this._capHits, this._boxHits);
		Collider[] array = (this._capHits < this._boxHits) ? this._capOverlaps : this._boxOverlaps;
		Collider[] array2 = (this._capHits < this._boxHits) ? this._boxOverlaps : this._capOverlaps;
		for (int i = 0; i < num3; i++)
		{
			Collider collider = array[i];
			if (collider && !this._set.Add(collider))
			{
				Collider[] overlaps = this._overlaps;
				int hits = this._hits;
				this._hits = hits + 1;
				overlaps[hits] = collider;
			}
			Collider collider2 = array2[i];
			if (collider2 && !this._set.Add(collider2))
			{
				Collider[] overlaps2 = this._overlaps;
				int hits = this._hits;
				this._hits = hits + 1;
				overlaps2[hits] = collider2;
			}
		}
		return this._colliding = (this._hits > 0);
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x0011F910 File Offset: 0x0011DB10
	private static void GetEndsAndRadius(Transform t, Vector3 center, float height, float radius, out Vector3 a, out Vector3 b, out float r)
	{
		float d = height * 0.5f;
		Vector3 lossyScale = t.lossyScale;
		a = t.TransformPoint(center + Vector3.down * d);
		b = t.TransformPoint(center + Vector3.up * d);
		r = Math.Max(Math.Abs(lossyScale.x), Math.Abs(lossyScale.z)) * radius;
	}

	// Token: 0x040043E6 RID: 17382
	public VolumeCast.VolumeShape shape;

	// Token: 0x040043E7 RID: 17383
	[Space]
	public Vector3 center;

	// Token: 0x040043E8 RID: 17384
	public Vector3 size = Vector3.one;

	// Token: 0x040043E9 RID: 17385
	public float height = 1f;

	// Token: 0x040043EA RID: 17386
	public float radius = 1f;

	// Token: 0x040043EB RID: 17387
	private const int MAX_HITS = 8;

	// Token: 0x040043EC RID: 17388
	[Space]
	public UnityLayerMask physicsMask = UnityLayerMask.Everything;

	// Token: 0x040043ED RID: 17389
	public bool includeTriggers;

	// Token: 0x040043EE RID: 17390
	[Space]
	[SerializeField]
	private bool _simulateInEditMode;

	// Token: 0x040043EF RID: 17391
	[DebugReadout]
	[NonSerialized]
	private int _capHits;

	// Token: 0x040043F0 RID: 17392
	[DebugReadout]
	[NonSerialized]
	private Collider[] _capOverlaps = new Collider[8];

	// Token: 0x040043F1 RID: 17393
	[DebugReadout]
	[NonSerialized]
	private int _boxHits;

	// Token: 0x040043F2 RID: 17394
	[DebugReadout]
	[NonSerialized]
	private Collider[] _boxOverlaps = new Collider[8];

	// Token: 0x040043F3 RID: 17395
	[DebugReadout]
	[NonSerialized]
	private int _hits;

	// Token: 0x040043F4 RID: 17396
	[DebugReadout]
	[NonSerialized]
	private Collider[] _overlaps = new Collider[8];

	// Token: 0x040043F5 RID: 17397
	[DebugReadout]
	[NonSerialized]
	private bool _colliding;

	// Token: 0x040043F6 RID: 17398
	[NonSerialized]
	private HashSet<Collider> _set = new HashSet<Collider>(8);

	// Token: 0x02000896 RID: 2198
	public enum VolumeShape
	{
		// Token: 0x040043F8 RID: 17400
		Box,
		// Token: 0x040043F9 RID: 17401
		Cylinder
	}
}
