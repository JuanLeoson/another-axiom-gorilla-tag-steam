using System;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class SyncToPlayerColor : MonoBehaviour
{
	// Token: 0x06001DB5 RID: 7605 RVA: 0x0009F67D File Offset: 0x0009D87D
	protected virtual void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._colorFunc = new Action<Color>(this.UpdateColor);
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x0009F69E File Offset: 0x0009D89E
	protected virtual void Start()
	{
		this.UpdateColor(this.rig.playerColor);
		this.rig.OnColorInitialized(this._colorFunc);
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x0009F6C2 File Offset: 0x0009D8C2
	protected virtual void OnEnable()
	{
		this.rig.OnColorChanged += this._colorFunc;
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x0009F6D5 File Offset: 0x0009D8D5
	protected virtual void OnDisable()
	{
		this.rig.OnColorChanged -= this._colorFunc;
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x0009F6E8 File Offset: 0x0009D8E8
	public virtual void UpdateColor(Color color)
	{
		if (!this.target)
		{
			return;
		}
		if (this.colorPropertiesToSync == null)
		{
			return;
		}
		for (int i = 0; i < this.colorPropertiesToSync.Length; i++)
		{
			ShaderHashId h = this.colorPropertiesToSync[i];
			this.target.SetColor(h, color);
		}
	}

	// Token: 0x0400264F RID: 9807
	public VRRig rig;

	// Token: 0x04002650 RID: 9808
	public Material target;

	// Token: 0x04002651 RID: 9809
	public ShaderHashId[] colorPropertiesToSync = new ShaderHashId[]
	{
		"_BaseColor"
	};

	// Token: 0x04002652 RID: 9810
	private Action<Color> _colorFunc;
}
