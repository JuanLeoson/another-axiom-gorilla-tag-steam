using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000368 RID: 872
public class FlashlightController : MonoBehaviour
{
	// Token: 0x060014A2 RID: 5282 RVA: 0x0006F29C File Offset: 0x0006D49C
	private void Start()
	{
		this.localRotation = this.flashlightRoot.localRotation;
		this.localPosition = this.flashlightRoot.localPosition;
		this.skeletons = new OVRSkeleton[2];
		this.hands = new OVRHand[2];
		this.externalController = base.GetComponent<GrabObject>();
		if (this.externalController)
		{
			GrabObject grabObject = this.externalController;
			grabObject.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(grabObject.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject grabObject2 = this.externalController;
			grabObject2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(grabObject2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(false);
		}
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x0006F364 File Offset: 0x0006D564
	private void LateUpdate()
	{
		if (!this.externalController)
		{
			this.FindHands();
			if (OVRInput.GetActiveController() != OVRInput.Controller.RTouch && OVRInput.GetActiveController() != OVRInput.Controller.LTouch && OVRInput.GetActiveController() != OVRInput.Controller.Touch)
			{
				if (this.handIndex >= 0)
				{
					this.AlignWithHand(this.hands[this.handIndex], this.skeletons[this.handIndex]);
				}
				if (this.infoText)
				{
					this.infoText.text = "Pinch to toggle flashlight";
					return;
				}
			}
			else
			{
				this.AlignWithController(OVRInput.Controller.RTouch);
				if (OVRInput.GetUp(OVRInput.RawButton.A, OVRInput.Controller.Active) && base.GetComponent<Flashlight>())
				{
					base.GetComponent<Flashlight>().ToggleFlashlight();
				}
				if (this.infoText)
				{
					this.infoText.text = "Press A to toggle flashlight";
				}
			}
		}
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x0006F434 File Offset: 0x0006D634
	private void FindHands()
	{
		if (this.skeletons[0] == null || this.skeletons[1] == null)
		{
			OVRSkeleton[] array = Object.FindObjectsOfType<OVRSkeleton>();
			if (array[0])
			{
				this.skeletons[0] = array[0];
				this.hands[0] = this.skeletons[0].GetComponent<OVRHand>();
				this.handIndex = 0;
			}
			if (array[1])
			{
				this.skeletons[1] = array[1];
				this.hands[1] = this.skeletons[1].GetComponent<OVRHand>();
				this.handIndex = 1;
				return;
			}
		}
		else if (this.handIndex == 0)
		{
			if (this.hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
			{
				this.handIndex = 1;
				return;
			}
		}
		else if (this.hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index))
		{
			this.handIndex = 0;
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0006F504 File Offset: 0x0006D704
	private void AlignWithHand(OVRHand hand, OVRSkeleton skeleton)
	{
		if (this.pinching)
		{
			if (hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.8f)
			{
				this.pinching = false;
			}
		}
		else if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
		{
			if (base.GetComponent<Flashlight>())
			{
				base.GetComponent<Flashlight>().ToggleFlashlight();
			}
			this.pinching = true;
		}
		this.flashlightRoot.position = skeleton.Bones[6].Transform.position;
		this.flashlightRoot.rotation = Quaternion.LookRotation(skeleton.Bones[6].Transform.position - skeleton.Bones[0].Transform.position);
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0006F5BC File Offset: 0x0006D7BC
	private void AlignWithController(OVRInput.Controller controller)
	{
		base.transform.position = OVRInput.GetLocalControllerPosition(controller);
		base.transform.rotation = OVRInput.GetLocalControllerRotation(controller);
		this.flashlightRoot.localRotation = this.localRotation;
		this.flashlightRoot.localPosition = this.localPosition;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x0006F610 File Offset: 0x0006D810
	public void Grab(OVRInput.Controller grabHand)
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(true);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(new Color(0f, 0f, 0f, 0.95f), 0f, 0.25f));
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0006F66C File Offset: 0x0006D86C
	public void Release()
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(false);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(Color.clear, 1f, 0.25f));
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x0006F6A9 File Offset: 0x0006D8A9
	private IEnumerator FadeLighting(Color newColor, float sceneLightIntensity, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		float currentLight = this.sceneLight ? this.sceneLight.intensity : 0f;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, t);
			if (this.sceneLight)
			{
				this.sceneLight.intensity = Mathf.Lerp(currentLight, sceneLightIntensity, t);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04001C26 RID: 7206
	public Light sceneLight;

	// Token: 0x04001C27 RID: 7207
	public Transform flashlightRoot;

	// Token: 0x04001C28 RID: 7208
	private Vector3 localPosition = Vector3.zero;

	// Token: 0x04001C29 RID: 7209
	private Quaternion localRotation = Quaternion.identity;

	// Token: 0x04001C2A RID: 7210
	public TextMesh infoText;

	// Token: 0x04001C2B RID: 7211
	private GrabObject externalController;

	// Token: 0x04001C2C RID: 7212
	private OVRSkeleton[] skeletons;

	// Token: 0x04001C2D RID: 7213
	private OVRHand[] hands;

	// Token: 0x04001C2E RID: 7214
	private int handIndex = -1;

	// Token: 0x04001C2F RID: 7215
	private bool pinching;
}
