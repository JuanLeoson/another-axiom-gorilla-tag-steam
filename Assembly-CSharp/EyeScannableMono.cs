using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class EyeScannableMono : MonoBehaviour, IEyeScannable
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x060003D6 RID: 982 RVA: 0x000173F0 File Offset: 0x000155F0
	// (remove) Token: 0x060003D7 RID: 983 RVA: 0x00017428 File Offset: 0x00015628
	public event Action OnDataChange;

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x0001745D File Offset: 0x0001565D
	int IEyeScannable.scannableId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060003D9 RID: 985 RVA: 0x00017465 File Offset: 0x00015665
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position - this._initialPosition + this._bounds.center;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060003DA RID: 986 RVA: 0x0001748D File Offset: 0x0001568D
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this._bounds;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060003DB RID: 987 RVA: 0x00017495 File Offset: 0x00015695
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.data.Entries;
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x000174A2 File Offset: 0x000156A2
	private void Awake()
	{
		this.RecalculateBounds();
	}

	// Token: 0x060003DD RID: 989 RVA: 0x000174AA File Offset: 0x000156AA
	public void OnEnable()
	{
		this.RecalculateBoundsLater();
		EyeScannerMono.Register(this);
	}

	// Token: 0x060003DE RID: 990 RVA: 0x000137E5 File Offset: 0x000119E5
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x000174B8 File Offset: 0x000156B8
	private void RecalculateBoundsLater()
	{
		EyeScannableMono.<RecalculateBoundsLater>d__17 <RecalculateBoundsLater>d__;
		<RecalculateBoundsLater>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RecalculateBoundsLater>d__.<>4__this = this;
		<RecalculateBoundsLater>d__.<>1__state = -1;
		<RecalculateBoundsLater>d__.<>t__builder.Start<EyeScannableMono.<RecalculateBoundsLater>d__17>(ref <RecalculateBoundsLater>d__);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x000174F0 File Offset: 0x000156F0
	private void RecalculateBounds()
	{
		this._initialPosition = base.transform.position;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		this._bounds = default(Bounds);
		if (componentsInChildren.Length == 0)
		{
			this._bounds.center = base.transform.position;
			this._bounds.Expand(1f);
			return;
		}
		this._bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			this._bounds.Encapsulate(componentsInChildren[i].bounds);
		}
	}

	// Token: 0x0400045A RID: 1114
	[SerializeField]
	private KeyValuePairSet data;

	// Token: 0x0400045B RID: 1115
	private Bounds _bounds;

	// Token: 0x0400045C RID: 1116
	private Vector3 _initialPosition;
}
