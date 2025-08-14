using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	// Token: 0x06000BAF RID: 2991 RVA: 0x00040682 File Offset: 0x0003E882
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0004068B File Offset: 0x0003E88B
	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00040694 File Offset: 0x0003E894
	private void SetAllActive(bool active)
	{
		foreach (GameObject gameObject in this.otherObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		foreach (XSceneRef xsceneRef in this.otherXSceneObjects)
		{
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2))
			{
				gameObject2.SetActive(active);
			}
		}
	}

	// Token: 0x04000EA3 RID: 3747
	public GameObject[] otherObjects;

	// Token: 0x04000EA4 RID: 3748
	public XSceneRef[] otherXSceneObjects;
}
