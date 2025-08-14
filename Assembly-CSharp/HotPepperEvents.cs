using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000496 RID: 1174
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x06001D13 RID: 7443 RVA: 0x0009C39E File Offset: 0x0009A59E
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x0009C3D8 File Offset: 0x0009A5D8
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0009C412 File Offset: 0x0009A612
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0009C41D File Offset: 0x0009A61D
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0009C428 File Offset: 0x0009A628
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		rig.transform.Find("RigAnchor/rig/body/head/gorillaface/spicy").gameObject.GetComponent<HotPepperFace>().PlayFX(1f);
	}

	// Token: 0x04002577 RID: 9591
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x02000497 RID: 1175
	public enum EdibleState
	{
		// Token: 0x04002579 RID: 9593
		A = 1,
		// Token: 0x0400257A RID: 9594
		B,
		// Token: 0x0400257B RID: 9595
		C = 4,
		// Token: 0x0400257C RID: 9596
		D = 8
	}
}
