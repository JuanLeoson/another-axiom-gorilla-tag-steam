using System;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
public class GorillaPawn : MonoBehaviour
{
	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06002C81 RID: 11393 RVA: 0x000EB6A9 File Offset: 0x000E98A9
	public VRRig rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002C82 RID: 11394 RVA: 0x000EB6B1 File Offset: 0x000E98B1
	public ZoneEntity zoneEntity
	{
		get
		{
			return this._zoneEntity;
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06002C83 RID: 11395 RVA: 0x000EB6B9 File Offset: 0x000E98B9
	public new Transform transform
	{
		get
		{
			return this._transform;
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06002C84 RID: 11396 RVA: 0x000EB6C1 File Offset: 0x000E98C1
	public XformNode handLeft
	{
		get
		{
			return this._handLeftXform;
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06002C85 RID: 11397 RVA: 0x000EB6C9 File Offset: 0x000E98C9
	public XformNode handRight
	{
		get
		{
			return this._handRightXform;
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06002C86 RID: 11398 RVA: 0x000EB6D1 File Offset: 0x000E98D1
	public XformNode body
	{
		get
		{
			return this._bodyXform;
		}
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x06002C87 RID: 11399 RVA: 0x000EB6D9 File Offset: 0x000E98D9
	public XformNode head
	{
		get
		{
			return this._headXform;
		}
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x000EB6E1 File Offset: 0x000E98E1
	private void Awake()
	{
		this.Setup(false);
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x000EB6EC File Offset: 0x000E98EC
	private void Setup(bool force)
	{
		this._transform = base.transform;
		this._rig = base.GetComponentInChildren<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._zoneEntity = this._rig.zoneEntity;
		if (this._zoneEntity)
		{
			if (this._bodyXform == null)
			{
				this._bodyXform = new XformNode();
			}
			this._bodyXform.localPosition = this._zoneEntity.collider.center;
			this._bodyXform.radius = this._zoneEntity.collider.radius;
			this._bodyXform.parent = this._transform;
		}
		bool flag = force || this._handLeft.AsNull<Transform>() == null;
		bool flag2 = force || this._handRight.AsNull<Transform>() == null;
		bool flag3 = force || this._head.AsNull<Transform>() == null;
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (flag3 && name.StartsWith("head", StringComparison.OrdinalIgnoreCase))
			{
				this._head = transform;
				this._headXform = new XformNode();
				this._headXform.localPosition = new Vector3(0f, 0.13f, 0.015f);
				this._headXform.radius = 0.12f;
				this._headXform.parent = transform;
			}
			else if (flag && name.StartsWith("hand.L", StringComparison.OrdinalIgnoreCase))
			{
				this._handLeft = transform;
				this._handLeftXform = new XformNode();
				this._handLeftXform.localPosition = new Vector3(-0.014f, 0.034f, 0f);
				this._handLeftXform.radius = 0.044f;
				this._handLeftXform.parent = transform;
			}
			else if (flag2 && name.StartsWith("hand.R", StringComparison.OrdinalIgnoreCase))
			{
				this._handRight = transform;
				this._handRightXform = new XformNode();
				this._handRightXform.localPosition = new Vector3(0.014f, 0.034f, 0f);
				this._handRightXform.radius = 0.044f;
				this._handRightXform.parent = transform;
			}
		}
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x000EB95F File Offset: 0x000E9B5F
	private bool CanRun()
	{
		if (GorillaPawn._gPawnActiveCount > 10)
		{
			Debug.LogError(string.Format("Cannot register more than {0} pawns.", 10));
			return false;
		}
		return true;
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x000EB984 File Offset: 0x000E9B84
	private void OnEnable()
	{
		if (!this.CanRun())
		{
			return;
		}
		this._id = -1;
		if (this._rig && this._rig.OwningNetPlayer != null)
		{
			this._id = this._rig.OwningNetPlayer.ActorNumber;
		}
		this._index = GorillaPawn._gPawnActiveCount++;
		GorillaPawn._gPawns[this._index] = this;
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000EB9F4 File Offset: 0x000E9BF4
	private void OnDisable()
	{
		this._id = -1;
		if (!this.CanRun())
		{
			return;
		}
		if (this._index < 0 || this._index >= GorillaPawn._gPawnActiveCount - 1)
		{
			return;
		}
		int num = --GorillaPawn._gPawnActiveCount;
		GorillaPawn._gPawns.Swap(this._index, num);
		this._index = num;
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x000EBA50 File Offset: 0x000E9C50
	private void OnDestroy()
	{
		int num = GorillaPawn._gPawns.IndexOfRef(this);
		GorillaPawn._gPawns[num] = null;
		Array.Sort<GorillaPawn>(GorillaPawn._gPawns, new Comparison<GorillaPawn>(GorillaPawn.ComparePawns));
		int num2 = 0;
		while (num2 < GorillaPawn._gPawns.Length && GorillaPawn._gPawns[num2])
		{
			num2++;
		}
		GorillaPawn._gPawnActiveCount = num2;
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x000EBAB0 File Offset: 0x000E9CB0
	private static int ComparePawns(GorillaPawn x, GorillaPawn y)
	{
		bool flag = x.AsNull<GorillaPawn>() == null;
		bool flag2 = y.AsNull<GorillaPawn>() == null;
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return x._index.CompareTo(y._index);
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x06002C8F RID: 11407 RVA: 0x000EBAF9 File Offset: 0x000E9CF9
	public static GorillaPawn[] AllPawns
	{
		get
		{
			return GorillaPawn._gPawns;
		}
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06002C90 RID: 11408 RVA: 0x000EBB00 File Offset: 0x000E9D00
	public static int ActiveCount
	{
		get
		{
			return GorillaPawn._gPawnActiveCount;
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06002C91 RID: 11409 RVA: 0x000EBB07 File Offset: 0x000E9D07
	public static Matrix4x4[] ShaderData
	{
		get
		{
			return GorillaPawn._gShaderData;
		}
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000EBB10 File Offset: 0x000E9D10
	public static void SyncPawnData()
	{
		Matrix4x4[] gShaderData = GorillaPawn._gShaderData;
		m4x4 m4x = default(m4x4);
		for (int i = 0; i < GorillaPawn._gPawnActiveCount; i++)
		{
			GorillaPawn gorillaPawn = GorillaPawn._gPawns[i];
			Vector4 worldPosition = gorillaPawn._headXform.worldPosition;
			Vector4 worldPosition2 = gorillaPawn._bodyXform.worldPosition;
			Vector4 worldPosition3 = gorillaPawn._handLeftXform.worldPosition;
			Vector4 worldPosition4 = gorillaPawn._handRightXform.worldPosition;
			m4x.SetRow0(ref worldPosition);
			m4x.SetRow1(ref worldPosition2);
			m4x.SetRow2(ref worldPosition3);
			m4x.SetRow3(ref worldPosition4);
			m4x.Push(ref gShaderData[i]);
		}
		for (int j = GorillaPawn._gPawnActiveCount; j < 10; j++)
		{
			MatrixUtils.Clear(ref gShaderData[j]);
		}
	}

	// Token: 0x040037EB RID: 14315
	[SerializeField]
	private Transform _transform;

	// Token: 0x040037EC RID: 14316
	[SerializeField]
	private Transform _handLeft;

	// Token: 0x040037ED RID: 14317
	[SerializeField]
	private Transform _handRight;

	// Token: 0x040037EE RID: 14318
	[SerializeField]
	private Transform _head;

	// Token: 0x040037EF RID: 14319
	[Space]
	[SerializeField]
	private VRRig _rig;

	// Token: 0x040037F0 RID: 14320
	[SerializeField]
	private ZoneEntity _zoneEntity;

	// Token: 0x040037F1 RID: 14321
	[Space]
	[SerializeField]
	private XformNode _handLeftXform;

	// Token: 0x040037F2 RID: 14322
	[SerializeField]
	private XformNode _handRightXform;

	// Token: 0x040037F3 RID: 14323
	[SerializeField]
	private XformNode _bodyXform;

	// Token: 0x040037F4 RID: 14324
	[SerializeField]
	private XformNode _headXform;

	// Token: 0x040037F5 RID: 14325
	[Space]
	private int _id;

	// Token: 0x040037F6 RID: 14326
	private int _index;

	// Token: 0x040037F7 RID: 14327
	private bool _invalid;

	// Token: 0x040037F8 RID: 14328
	public const int MAX_PAWNS = 10;

	// Token: 0x040037F9 RID: 14329
	private static GorillaPawn[] _gPawns = new GorillaPawn[10];

	// Token: 0x040037FA RID: 14330
	private static int _gPawnActiveCount = 0;

	// Token: 0x040037FB RID: 14331
	private static Matrix4x4[] _gShaderData = new Matrix4x4[10];
}
