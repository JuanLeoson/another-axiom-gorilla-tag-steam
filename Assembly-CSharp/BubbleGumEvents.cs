using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000490 RID: 1168
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x06001CF3 RID: 7411 RVA: 0x0009BD75 File Offset: 0x00099F75
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x0009BDAF File Offset: 0x00099FAF
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x0009BDE9 File Offset: 0x00099FE9
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0009BDF4 File Offset: 0x00099FF4
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x0009BE00 File Offset: 0x0009A000
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, out this._bubble))
		{
			this._bubble = gameObject.GetComponentsInChildren<GumBubble>(true).FirstOrDefault((GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x0400255A RID: 9562
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x0400255B RID: 9563
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x0400255C RID: 9564
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x02000491 RID: 1169
	public enum EdibleState
	{
		// Token: 0x0400255E RID: 9566
		A = 1,
		// Token: 0x0400255F RID: 9567
		B,
		// Token: 0x04002560 RID: 9568
		C = 4,
		// Token: 0x04002561 RID: 9569
		D = 8
	}
}
