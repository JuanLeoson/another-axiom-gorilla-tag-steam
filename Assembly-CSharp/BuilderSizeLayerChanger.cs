using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000519 RID: 1305
public class BuilderSizeLayerChanger : MonoBehaviour
{
	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06001FBC RID: 8124 RVA: 0x000A7944 File Offset: 0x000A5B44
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000A7984 File Offset: 0x000A5B84
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000A799C File Offset: 0x000A5B9C
	public void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000A7A28 File Offset: 0x000A5C28
	public void OnTriggerExit(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04002860 RID: 10336
	public float maxScale;

	// Token: 0x04002861 RID: 10337
	public float minScale;

	// Token: 0x04002862 RID: 10338
	public bool isAssurance;

	// Token: 0x04002863 RID: 10339
	public bool affectLayerA = true;

	// Token: 0x04002864 RID: 10340
	public bool affectLayerB = true;

	// Token: 0x04002865 RID: 10341
	public bool affectLayerC = true;

	// Token: 0x04002866 RID: 10342
	public bool affectLayerD = true;

	// Token: 0x04002867 RID: 10343
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04002868 RID: 10344
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04002869 RID: 10345
	[SerializeField]
	private GameObject fxForLayerChange;
}
