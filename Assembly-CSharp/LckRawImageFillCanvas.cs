using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000290 RID: 656
[ExecuteInEditMode]
public class LckRawImageFillCanvas : UIBehaviour
{
	// Token: 0x06000F11 RID: 3857 RVA: 0x0005A018 File Offset: 0x00058218
	private new void OnEnable()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0005A018 File Offset: 0x00058218
	private void Update()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0005A020 File Offset: 0x00058220
	private void UpdateSizeDelta()
	{
		if (this._rawImage == null || this._rawImage.texture == null)
		{
			return;
		}
		RectTransform rectTransform = this._rawImage.rectTransform;
		Vector2 sizeDelta = ((RectTransform)rectTransform.parent).sizeDelta;
		Vector2 vector = new Vector2((float)this._rawImage.texture.width, (float)this._rawImage.texture.height);
		float num = sizeDelta.x / sizeDelta.y;
		float num2 = vector.x / vector.y;
		float num3 = num / num2;
		Vector2 vector2 = new Vector2(sizeDelta.x, sizeDelta.x / num2);
		Vector2 vector3 = new Vector2(sizeDelta.y * num2, sizeDelta.y);
		switch (this._scaleType)
		{
		case LckRawImageFillCanvas.ScaleType.Fill:
			rectTransform.sizeDelta = ((num3 > 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Inset:
			rectTransform.sizeDelta = ((num3 < 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Stretch:
			rectTransform.sizeDelta = sizeDelta;
			return;
		default:
			return;
		}
	}

	// Token: 0x040017E1 RID: 6113
	[SerializeField]
	private RawImage _rawImage;

	// Token: 0x040017E2 RID: 6114
	[SerializeField]
	private LckRawImageFillCanvas.ScaleType _scaleType;

	// Token: 0x02000291 RID: 657
	private enum ScaleType
	{
		// Token: 0x040017E4 RID: 6116
		Fill,
		// Token: 0x040017E5 RID: 6117
		Inset,
		// Token: 0x040017E6 RID: 6118
		Stretch
	}
}
