using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D31 RID: 3377
	public class TrainCrossingController : MonoBehaviour
	{
		// Token: 0x06005399 RID: 21401 RVA: 0x0019D952 File Offset: 0x0019BB52
		private void Awake()
		{
			this._lightsSide1Mat = this._lightSide1Renderer.material;
			this._lightsSide2Mat = this._lightSide2Renderer.material;
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x0019D976 File Offset: 0x0019BB76
		private void OnDestroy()
		{
			if (this._lightsSide1Mat != null)
			{
				Object.Destroy(this._lightsSide1Mat);
			}
			if (this._lightsSide2Mat != null)
			{
				Object.Destroy(this._lightsSide2Mat);
			}
		}

		// Token: 0x0600539B RID: 21403 RVA: 0x0019D9AA File Offset: 0x0019BBAA
		public void CrossingButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this.ActivateTrainCrossing();
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x0600539C RID: 21404 RVA: 0x0019D9D8 File Offset: 0x0019BBD8
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x0600539D RID: 21405 RVA: 0x0019DA2C File Offset: 0x0019BC2C
		private void ActivateTrainCrossing()
		{
			int num = this._crossingSounds.Length - 1;
			AudioClip audioClip = this._crossingSounds[(int)(Random.value * (float)num)];
			this._audioSource.clip = audioClip;
			this._audioSource.timeSamples = 0;
			this._audioSource.Play();
			if (this._xingAnimationCr != null)
			{
				base.StopCoroutine(this._xingAnimationCr);
			}
			this._xingAnimationCr = base.StartCoroutine(this.AnimateCrossing(audioClip.length * 0.75f));
		}

		// Token: 0x0600539E RID: 21406 RVA: 0x0019DAAA File Offset: 0x0019BCAA
		private IEnumerator AnimateCrossing(float animationLength)
		{
			this.ToggleLightObjects(true);
			float animationEndTime = Time.time + animationLength;
			float lightBlinkDuration = animationLength * 0.1f;
			float lightBlinkStartTime = Time.time;
			float lightBlinkEndTime = Time.time + lightBlinkDuration;
			Material lightToBlinkOn = this._lightsSide1Mat;
			Material lightToBlinkOff = this._lightsSide2Mat;
			Color onColor = new Color(1f, 1f, 1f, 1f);
			Color offColor = new Color(1f, 1f, 1f, 0f);
			while (Time.time < animationEndTime)
			{
				float t = (Time.time - lightBlinkStartTime) / lightBlinkDuration;
				lightToBlinkOn.SetColor(this._colorId, Color.Lerp(offColor, onColor, t));
				lightToBlinkOff.SetColor(this._colorId, Color.Lerp(onColor, offColor, t));
				if (Time.time > lightBlinkEndTime)
				{
					Material material = lightToBlinkOn;
					lightToBlinkOn = lightToBlinkOff;
					lightToBlinkOff = material;
					lightBlinkStartTime = Time.time;
					lightBlinkEndTime = Time.time + lightBlinkDuration;
				}
				yield return null;
			}
			this.ToggleLightObjects(false);
			yield break;
		}

		// Token: 0x0600539F RID: 21407 RVA: 0x0019DAC0 File Offset: 0x0019BCC0
		private void AffectMaterials(Material[] materials, Color newColor)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetColor(this._colorId, newColor);
			}
		}

		// Token: 0x060053A0 RID: 21408 RVA: 0x0019DAEC File Offset: 0x0019BCEC
		private void ToggleLightObjects(bool enableState)
		{
			this._lightSide1Renderer.gameObject.SetActive(enableState);
			this._lightSide2Renderer.gameObject.SetActive(enableState);
		}

		// Token: 0x04005CE2 RID: 23778
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04005CE3 RID: 23779
		[SerializeField]
		private AudioClip[] _crossingSounds;

		// Token: 0x04005CE4 RID: 23780
		[SerializeField]
		private MeshRenderer _lightSide1Renderer;

		// Token: 0x04005CE5 RID: 23781
		[SerializeField]
		private MeshRenderer _lightSide2Renderer;

		// Token: 0x04005CE6 RID: 23782
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04005CE7 RID: 23783
		private Material _lightsSide1Mat;

		// Token: 0x04005CE8 RID: 23784
		private Material _lightsSide2Mat;

		// Token: 0x04005CE9 RID: 23785
		private int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x04005CEA RID: 23786
		private Coroutine _xingAnimationCr;

		// Token: 0x04005CEB RID: 23787
		private InteractableTool _toolInteractingWithMe;
	}
}
