using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class BootscreenPositioner : MonoBehaviour
{
	// Token: 0x06000088 RID: 136 RVA: 0x00004B84 File Offset: 0x00002D84
	private void Awake()
	{
		base.transform.position = this._pov.position;
		base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00004BDC File Offset: 0x00002DDC
	private void LateUpdate()
	{
		if (Vector3.Distance(base.transform.position, this._pov.position) > this._distanceThreshold)
		{
			base.transform.position = this._pov.position;
		}
		if (Mathf.Abs(this._pov.rotation.eulerAngles.y - base.transform.rotation.eulerAngles.y) > this._rotationThreshold)
		{
			base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private Transform _pov;

	// Token: 0x040000A6 RID: 166
	[SerializeField]
	private float _distanceThreshold;

	// Token: 0x040000A7 RID: 167
	[SerializeField]
	private float _rotationThreshold;
}
