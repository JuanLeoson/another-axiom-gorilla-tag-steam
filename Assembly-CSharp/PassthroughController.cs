using System;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class PassthroughController : MonoBehaviour
{
	// Token: 0x060014E9 RID: 5353 RVA: 0x00072508 File Offset: 0x00070708
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x00072558 File Offset: 0x00070758
	private void Update()
	{
		Color edgeColor = Color.HSVToRGB(Time.time * 0.1f % 1f, 1f, 1f);
		this.passthroughLayer.edgeColor = edgeColor;
		float contrast = Mathf.Sin(Time.time);
		this.passthroughLayer.SetColorMapControls(contrast, 0f, 0f, null, OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor);
		base.transform.position = Camera.main.transform.position;
		base.transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized);
	}

	// Token: 0x04001C87 RID: 7303
	private OVRPassthroughLayer passthroughLayer;
}
