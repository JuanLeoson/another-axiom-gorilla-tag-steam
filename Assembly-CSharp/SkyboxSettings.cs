using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
[ExecuteInEditMode]
public class SkyboxSettings : MonoBehaviour
{
	// Token: 0x06000DB1 RID: 3505 RVA: 0x00053E62 File Offset: 0x00052062
	private void OnEnable()
	{
		if (this._skyMaterial)
		{
			RenderSettings.skybox = this._skyMaterial;
		}
	}

	// Token: 0x04001583 RID: 5507
	[SerializeField]
	private Material _skyMaterial;
}
