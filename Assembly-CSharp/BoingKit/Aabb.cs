using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FF7 RID: 4087
	public struct Aabb
	{
		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06006606 RID: 26118 RVA: 0x0020796F File Offset: 0x00205B6F
		// (set) Token: 0x06006607 RID: 26119 RVA: 0x0020797C File Offset: 0x00205B7C
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06006608 RID: 26120 RVA: 0x0020798A File Offset: 0x00205B8A
		// (set) Token: 0x06006609 RID: 26121 RVA: 0x00207997 File Offset: 0x00205B97
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x0600660A RID: 26122 RVA: 0x002079A5 File Offset: 0x00205BA5
		// (set) Token: 0x0600660B RID: 26123 RVA: 0x002079B2 File Offset: 0x00205BB2
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x0600660C RID: 26124 RVA: 0x002079C0 File Offset: 0x00205BC0
		// (set) Token: 0x0600660D RID: 26125 RVA: 0x002079CD File Offset: 0x00205BCD
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x0600660E RID: 26126 RVA: 0x002079DB File Offset: 0x00205BDB
		// (set) Token: 0x0600660F RID: 26127 RVA: 0x002079E8 File Offset: 0x00205BE8
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06006610 RID: 26128 RVA: 0x002079F6 File Offset: 0x00205BF6
		// (set) Token: 0x06006611 RID: 26129 RVA: 0x00207A03 File Offset: 0x00205C03
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x06006612 RID: 26130 RVA: 0x00207A11 File Offset: 0x00205C11
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x06006613 RID: 26131 RVA: 0x00207A30 File Offset: 0x00205C30
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x06006614 RID: 26132 RVA: 0x00207A95 File Offset: 0x00205C95
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x00207AC4 File Offset: 0x00205CC4
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x00207AE0 File Offset: 0x00205CE0
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x00207B04 File Offset: 0x00205D04
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x00207B14 File Offset: 0x00205D14
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06006619 RID: 26137 RVA: 0x00207BAC File Offset: 0x00205DAC
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x0600661A RID: 26138 RVA: 0x00207C12 File Offset: 0x00205E12
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x00207C35 File Offset: 0x00205E35
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x00207C58 File Offset: 0x00205E58
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x0600661D RID: 26141 RVA: 0x00207C7C File Offset: 0x00205E7C
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x0600661E RID: 26142 RVA: 0x00207CE8 File Offset: 0x00205EE8
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x00207D48 File Offset: 0x00205F48
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x0400710E RID: 28942
		public Vector3 Min;

		// Token: 0x0400710F RID: 28943
		public Vector3 Max;
	}
}
