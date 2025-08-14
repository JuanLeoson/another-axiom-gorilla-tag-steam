using System;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class GorillaGeoHideShowTrigger : GorillaTriggerBox, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x06002B18 RID: 11032 RVA: 0x000E43D0 File Offset: 0x000E25D0
	protected void Awake()
	{
		((IGuidedRefObject)this).GuidedRefInitialize();
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000E43D8 File Offset: 0x000E25D8
	public override void OnBoxTriggered()
	{
		if (!this._guidedRefsAreFullyResolved)
		{
			return;
		}
		if (this.makeSureThisIsDisabled != null)
		{
			foreach (GameObject gameObject in this.makeSureThisIsDisabled)
			{
				if (gameObject == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject.SetActive(false);
			}
		}
		if (this.makeSureThisIsEnabled != null)
		{
			foreach (GameObject gameObject2 in this.makeSureThisIsEnabled)
			{
				if (gameObject2 == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject2.SetActive(true);
			}
		}
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x000E4492 File Offset: 0x000E2692
	void IGuidedRefObject.GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsDisabled", ref this.makeSureThisIsDisabled, ref this.makeSureThisIsDisabled_gRefs);
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsEnabled", ref this.makeSureThisIsEnabled, ref this.makeSureThisIsEnabled_gRefs);
		GuidedRefHub.ReceiverFullyRegistered<GorillaGeoHideShowTrigger>(this);
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x000E44C8 File Offset: 0x000E26C8
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		return GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsDisabled_gRefs, target) || GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsEnabled_gRefs, target);
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x000E44F9 File Offset: 0x000E26F9
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
		this._guidedRefsAreFullyResolved = true;
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x000E4502 File Offset: 0x000E2702
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
		this._guidedRefsAreFullyResolved = false;
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x06002B1E RID: 11038 RVA: 0x000E450B File Offset: 0x000E270B
	// (set) Token: 0x06002B1F RID: 11039 RVA: 0x000E4513 File Offset: 0x000E2713
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x06002B21 RID: 11041 RVA: 0x0005860D File Offset: 0x0005680D
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x0001745D File Offset: 0x0001565D
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x04003685 RID: 13957
	[SerializeField]
	private GameObject[] makeSureThisIsDisabled;

	// Token: 0x04003686 RID: 13958
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsDisabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x04003687 RID: 13959
	[SerializeField]
	private GameObject[] makeSureThisIsEnabled;

	// Token: 0x04003688 RID: 13960
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsEnabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x04003689 RID: 13961
	private bool _guidedRefsAreFullyResolved;
}
