using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000885 RID: 2181
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x060036AE RID: 13998 RVA: 0x0011DD04 File Offset: 0x0011BF04
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x0011DD15 File Offset: 0x0011BF15
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x0011DD28 File Offset: 0x0011BF28
	public void SetObjectStates(bool active)
	{
		int count = this.gameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.gameObjects[i];
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
		int count2 = this.behaviours.Count;
		for (int j = 0; j < count2; j++)
		{
			Behaviour behaviour = this.behaviours[j];
			if (!(behaviour == null))
			{
				behaviour.enabled = active;
			}
		}
		int count3 = this.renderers.Count;
		for (int k = 0; k < count3; k++)
		{
			Renderer renderer = this.renderers[k];
			if (!(renderer == null))
			{
				renderer.enabled = active;
			}
		}
		int count4 = this.colliders.Count;
		for (int l = 0; l < count4; l++)
		{
			Collider collider = this.colliders[l];
			if (!(collider == null))
			{
				collider.enabled = active;
			}
		}
	}

	// Token: 0x040043A1 RID: 17313
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x040043A2 RID: 17314
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x040043A3 RID: 17315
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x040043A4 RID: 17316
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x040043A5 RID: 17317
	public bool syncWithGroupState = true;
}
