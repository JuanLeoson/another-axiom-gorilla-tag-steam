using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class MenagerieDepositBox : MonoBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x000139B8 File Offset: 0x00011BB8
	public void OnTriggerEnter(Collider other)
	{
		MenagerieCritter component = other.transform.parent.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Combine(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x06000339 RID: 825 RVA: 0x00013A00 File Offset: 0x00011C00
	public void OnTriggerExit(Collider other)
	{
		MenagerieCritter component = other.transform.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Remove(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x040003CB RID: 971
	public Action<MenagerieCritter> OnCritterInserted;
}
