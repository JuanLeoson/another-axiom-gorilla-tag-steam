using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class PassthroughStyler : MonoBehaviour
{
	// Token: 0x060014EF RID: 5359 RVA: 0x00072740 File Offset: 0x00070940
	private void Start()
	{
		GrabObject grabObject;
		if (base.TryGetComponent<GrabObject>(out grabObject))
		{
			GrabObject grabObject2 = grabObject;
			grabObject2.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(grabObject2.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject grabObject3 = grabObject;
			grabObject3.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(grabObject3.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
			GrabObject grabObject4 = grabObject;
			grabObject4.CursorPositionDelegate = (GrabObject.SetCursorPosition)Delegate.Combine(grabObject4.CursorPositionDelegate, new GrabObject.SetCursorPosition(this.Cursor));
		}
		this._savedColor = new Color(1f, 1f, 1f, 0f);
		this.ShowFullMenu(false);
		this._mainCanvas.interactable = false;
		this._passthroughColorLut = new OVRPassthroughColorLut(this._colorLutTexture, true);
		if (!OVRManager.GetPassthroughCapabilities().SupportsColorPassthrough && this._objectsToHideForColorPassthrough != null)
		{
			for (int i = 0; i < this._objectsToHideForColorPassthrough.Length; i++)
			{
				this._objectsToHideForColorPassthrough[i].SetActive(false);
			}
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x00072836 File Offset: 0x00070A36
	private void Update()
	{
		if (this._controllerHand == OVRInput.Controller.None)
		{
			return;
		}
		if (this._settingColor)
		{
			this.GetColorFromWheel();
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x0007284F File Offset: 0x00070A4F
	public void OnBrightnessChanged(float newValue)
	{
		this._savedBrightness = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0007285E File Offset: 0x00070A5E
	public void OnContrastChanged(float newValue)
	{
		this._savedContrast = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0007286D File Offset: 0x00070A6D
	public void OnSaturationChanged(float newValue)
	{
		this._savedSaturation = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0007287C File Offset: 0x00070A7C
	public void OnAlphaChanged(float newValue)
	{
		this._savedColor = new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, newValue);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000728BC File Offset: 0x00070ABC
	public void OnBlendChange(float newValue)
	{
		this._savedBlend = newValue;
		this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000728DC File Offset: 0x00070ADC
	public void DoColorDrag(bool doDrag)
	{
		this._settingColor = doDrag;
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000728E5 File Offset: 0x00070AE5
	public void SetPassthroughStyleToColorAdjustment(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment);
		}
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000728F1 File Offset: 0x00070AF1
	public void SetPassthroughStyleToColorLut(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorLut);
		}
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x00072900 File Offset: 0x00070B00
	private void Grab(OVRInput.Controller grabHand)
	{
		this._controllerHand = grabHand;
		this.ShowFullMenu(true);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = true;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToCurrentStyle(0.2f);
		base.StartCoroutine(this._fade);
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x00072968 File Offset: 0x00070B68
	private void Release()
	{
		this._controllerHand = OVRInput.Controller.None;
		this.ShowFullMenu(false);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = false;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToDefaultPassthrough(0.2f);
		base.StartCoroutine(this._fade);
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000729CE File Offset: 0x00070BCE
	private IEnumerator FadeToCurrentStyle(float fadeTime)
	{
		this._passthroughLayer.edgeRenderingEnabled = true;
		yield return this.FadeTo(1f, fadeTime);
		yield break;
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000729E4 File Offset: 0x00070BE4
	private IEnumerator FadeToDefaultPassthrough(float fadeTime)
	{
		yield return this.FadeTo(0f, fadeTime);
		this._passthroughLayer.edgeRenderingEnabled = false;
		yield break;
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000729FA File Offset: 0x00070BFA
	private IEnumerator FadeTo(float styleValueMultiplier, float duration)
	{
		float timer = 0f;
		float brightness = this._passthroughLayer.colorMapEditorBrightness;
		float contrast = this._passthroughLayer.colorMapEditorContrast;
		float saturation = this._passthroughLayer.colorMapEditorSaturation;
		Color edgeCol = this._passthroughLayer.edgeColor;
		float blend = this._savedBlend;
		while (timer <= duration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / duration);
			if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
			{
				this._passthroughLayer.SetColorLut(this._passthroughColorLut, Mathf.Lerp(blend, this._savedBlend * styleValueMultiplier, t));
			}
			else
			{
				this._passthroughLayer.SetBrightnessContrastSaturation(Mathf.Lerp(brightness, this._savedBrightness * styleValueMultiplier, t), Mathf.Lerp(contrast, this._savedContrast * styleValueMultiplier, t), Mathf.Lerp(saturation, this._savedSaturation * styleValueMultiplier, t));
			}
			this._passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, this._savedColor.a * styleValueMultiplier), t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x00072A17 File Offset: 0x00070C17
	private void UpdateBrighnessContrastSaturation()
	{
		this._passthroughLayer.SetBrightnessContrastSaturation(this._savedBrightness, this._savedContrast, this._savedSaturation);
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x00072A38 File Offset: 0x00070C38
	private void ShowFullMenu(bool doShow)
	{
		GameObject[] compactObjects = this._compactObjects;
		for (int i = 0; i < compactObjects.Length; i++)
		{
			compactObjects[i].SetActive(doShow);
		}
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x00072A63 File Offset: 0x00070C63
	private void Cursor(Vector3 cP)
	{
		this._cursorPosition = cP;
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x00072A6C File Offset: 0x00070C6C
	private void GetColorFromWheel()
	{
		Vector3 vector = this._colorWheel.transform.InverseTransformPoint(this._cursorPosition);
		Vector2 vector2 = new Vector2(vector.x / this._colorWheel.sizeDelta.x + 0.5f, vector.y / this._colorWheel.sizeDelta.y + 0.5f);
		Debug.Log("Sanctuary: " + vector2.x.ToString() + ", " + vector2.y.ToString());
		Color color = Color.black;
		if ((double)vector2.x < 1.0 && vector2.x > 0f && (double)vector2.y < 1.0 && vector2.y > 0f)
		{
			int x = Mathf.RoundToInt(vector2.x * (float)this._colorTexture.width);
			int y = Mathf.RoundToInt(vector2.y * (float)this._colorTexture.height);
			color = this._colorTexture.GetPixel(x, y);
		}
		this._savedColor = new Color(color.r, color.g, color.b, this._savedColor.a);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x00072BBC File Offset: 0x00070DBC
	private void SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType passthroughStyle)
	{
		this._currentStyle = passthroughStyle;
		if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
		{
			this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
			return;
		}
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x04001C8B RID: 7307
	private const float FadeDuration = 0.2f;

	// Token: 0x04001C8C RID: 7308
	[SerializeField]
	private OVRInput.Controller _controllerHand;

	// Token: 0x04001C8D RID: 7309
	[SerializeField]
	private OVRPassthroughLayer _passthroughLayer;

	// Token: 0x04001C8E RID: 7310
	[SerializeField]
	private RectTransform _colorWheel;

	// Token: 0x04001C8F RID: 7311
	[SerializeField]
	private Texture2D _colorTexture;

	// Token: 0x04001C90 RID: 7312
	[SerializeField]
	private Texture2D _colorLutTexture;

	// Token: 0x04001C91 RID: 7313
	[SerializeField]
	private CanvasGroup _mainCanvas;

	// Token: 0x04001C92 RID: 7314
	[SerializeField]
	private GameObject[] _compactObjects;

	// Token: 0x04001C93 RID: 7315
	[SerializeField]
	private GameObject[] _objectsToHideForColorPassthrough;

	// Token: 0x04001C94 RID: 7316
	private Vector3 _cursorPosition = Vector3.zero;

	// Token: 0x04001C95 RID: 7317
	private bool _settingColor;

	// Token: 0x04001C96 RID: 7318
	private Color _savedColor = Color.white;

	// Token: 0x04001C97 RID: 7319
	private float _savedBrightness;

	// Token: 0x04001C98 RID: 7320
	private float _savedContrast;

	// Token: 0x04001C99 RID: 7321
	private float _savedSaturation;

	// Token: 0x04001C9A RID: 7322
	private OVRPassthroughLayer.ColorMapEditorType _currentStyle = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;

	// Token: 0x04001C9B RID: 7323
	private float _savedBlend = 1f;

	// Token: 0x04001C9C RID: 7324
	private OVRPassthroughColorLut _passthroughColorLut;

	// Token: 0x04001C9D RID: 7325
	private IEnumerator _fade;
}
