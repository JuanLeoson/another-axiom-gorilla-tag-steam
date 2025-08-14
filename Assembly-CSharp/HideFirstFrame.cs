using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class HideFirstFrame : MonoBehaviour
{
	// Token: 0x0600008B RID: 139 RVA: 0x00004C92 File Offset: 0x00002E92
	private void Awake()
	{
		this._cam = base.GetComponent<Camera>();
		this._farClipPlane = this._cam.farClipPlane;
		this._cam.farClipPlane = this._cam.nearClipPlane + 0.1f;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004CCD File Offset: 0x00002ECD
	public IEnumerator Start()
	{
		int num;
		for (int i = 0; i < this._frameDelay; i = num + 1)
		{
			yield return null;
			num = i;
		}
		this._cam.farClipPlane = this._farClipPlane;
		yield break;
	}

	// Token: 0x040000A8 RID: 168
	[SerializeField]
	private int _frameDelay = 1;

	// Token: 0x040000A9 RID: 169
	private Camera _cam;

	// Token: 0x040000AA RID: 170
	private float _farClipPlane;
}
