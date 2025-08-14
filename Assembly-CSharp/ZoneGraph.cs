using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B5F RID: 2911
[DefaultExecutionOrder(5555)]
public class ZoneGraph : MonoBehaviour
{
	// Token: 0x17000699 RID: 1689
	// (get) Token: 0x060045AF RID: 17839 RVA: 0x0015C014 File Offset: 0x0015A214
	public static ZoneGraph Instance
	{
		get
		{
			return ZoneGraph.gGraph;
		}
	}

	// Token: 0x060045B0 RID: 17840 RVA: 0x0015C01B File Offset: 0x0015A21B
	public static ZoneDef ColliderToZoneDef(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToZoneDef[collider];
		}
		return null;
	}

	// Token: 0x060045B1 RID: 17841 RVA: 0x0015C038 File Offset: 0x0015A238
	public static ZoneNode ColliderToNode(BoxCollider collider)
	{
		if (!(collider == null))
		{
			return ZoneGraph.gGraph._colliderToNode[collider];
		}
		return ZoneNode.Null;
	}

	// Token: 0x060045B2 RID: 17842 RVA: 0x0015C059 File Offset: 0x0015A259
	private void Awake()
	{
		if (ZoneGraph.gGraph != null && ZoneGraph.gGraph != this)
		{
			Object.Destroy(this);
		}
		else
		{
			ZoneGraph.gGraph = this;
		}
		this.CompileColliderMaps(this._zoneDefs);
	}

	// Token: 0x060045B3 RID: 17843 RVA: 0x0015C08F File Offset: 0x0015A28F
	public void CheckCompiledMaps()
	{
		if (this._compiledGraph)
		{
			return;
		}
		this.CompileColliderMaps(this._zoneDefs);
	}

	// Token: 0x060045B4 RID: 17844 RVA: 0x0015C0A8 File Offset: 0x0015A2A8
	private void CompileColliderMaps(ZoneDef[] zones)
	{
		foreach (ZoneDef zoneDef in zones)
		{
			for (int j = 0; j < zoneDef.colliders.Length; j++)
			{
				BoxCollider boxCollider = zoneDef.colliders[j];
				if (!(boxCollider == null))
				{
					this._colliderToZoneDef[boxCollider] = zoneDef;
				}
			}
		}
		for (int k = 0; k < this._colliders.Length; k++)
		{
			BoxCollider boxCollider2 = this._colliders[k];
			if (!(boxCollider2 == null))
			{
				this._colliderToNode[boxCollider2] = this._nodes[k];
			}
		}
		this._compiledGraph = true;
	}

	// Token: 0x060045B5 RID: 17845 RVA: 0x0015C148 File Offset: 0x0015A348
	public static int Compare(ZoneDef x, ZoneDef y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		int num = (int)x.zoneId;
		int num2 = num.CompareTo((int)y.zoneId);
		if (num2 == 0)
		{
			num = (int)x.subZoneId;
			num2 = num.CompareTo((int)y.subZoneId);
		}
		return num2;
	}

	// Token: 0x060045B6 RID: 17846 RVA: 0x0015C1AD File Offset: 0x0015A3AD
	public static void Register(ZoneEntity entity)
	{
		if (ZoneGraph.gGraph == null)
		{
			ZoneGraph.gGraph = Object.FindFirstObjectByType<ZoneGraph>();
		}
		if (!ZoneGraph.gGraph._entityList.Contains(entity))
		{
			ZoneGraph.gGraph._entityList.Add(entity);
		}
	}

	// Token: 0x060045B7 RID: 17847 RVA: 0x0015C1E8 File Offset: 0x0015A3E8
	public static void Unregister(ZoneEntity entity)
	{
		ZoneGraph.gGraph._entityList.Remove(entity);
	}

	// Token: 0x040050BF RID: 20671
	[SerializeField]
	private ZoneDef[] _zoneDefs = new ZoneDef[0];

	// Token: 0x040050C0 RID: 20672
	[SerializeField]
	private BoxCollider[] _colliders = new BoxCollider[0];

	// Token: 0x040050C1 RID: 20673
	[SerializeField]
	private ZoneNode[] _nodes = new ZoneNode[0];

	// Token: 0x040050C2 RID: 20674
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneDef> _colliderToZoneDef = new Dictionary<BoxCollider, ZoneDef>(64);

	// Token: 0x040050C3 RID: 20675
	[Space]
	[NonSerialized]
	private Dictionary<BoxCollider, ZoneNode> _colliderToNode = new Dictionary<BoxCollider, ZoneNode>(64);

	// Token: 0x040050C4 RID: 20676
	[Space]
	[NonSerialized]
	private List<ZoneEntity> _entityList = new List<ZoneEntity>(16);

	// Token: 0x040050C5 RID: 20677
	private static ZoneGraph gGraph;

	// Token: 0x040050C6 RID: 20678
	private bool _compiledGraph;
}
