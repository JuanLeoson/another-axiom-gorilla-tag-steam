using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007AF RID: 1967
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x0600315C RID: 12636 RVA: 0x00100E4F File Offset: 0x000FF04F
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x0600315D RID: 12637 RVA: 0x00100E7A File Offset: 0x000FF07A
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x0600315E RID: 12638 RVA: 0x00004F01 File Offset: 0x00003101
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x0600315F RID: 12639 RVA: 0x00100E89 File Offset: 0x000FF089
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x04003D00 RID: 15616
	private Camera myCamera;

	// Token: 0x04003D01 RID: 15617
	private float frameRate;

	// Token: 0x04003D02 RID: 15618
	private float timeToNextFrame;
}
