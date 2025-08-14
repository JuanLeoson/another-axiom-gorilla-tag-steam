using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007AD RID: 1965
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06003153 RID: 12627 RVA: 0x00100D6F File Offset: 0x000FEF6F
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x00100D7E File Offset: 0x000FEF7E
	private IEnumerator PopCoroutine()
	{
		this.audioSource.GTPlay();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04003CF9 RID: 15609
	public MeshRenderer bubbleMesh;

	// Token: 0x04003CFA RID: 15610
	public Rigidbody body;

	// Token: 0x04003CFB RID: 15611
	public MeshCollider bubbleCollider;

	// Token: 0x04003CFC RID: 15612
	public AudioSource audioSource;
}
