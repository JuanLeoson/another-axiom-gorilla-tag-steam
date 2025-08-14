using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000363 RID: 867
public class BrushController : MonoBehaviour
{
	// Token: 0x06001489 RID: 5257 RVA: 0x0006EDA0 File Offset: 0x0006CFA0
	private void Start()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		if (!this.brush.lineContainer)
		{
			this.brush.lineContainer = new GameObject("LineContainer");
		}
		this.backgroundSphere.material.renderQueue = 3998;
		this.backgroundSphere.transform.parent = null;
		this.backgroundSphere.enabled = false;
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0006EE6D File Offset: 0x0006D06D
	private void Update()
	{
		this.backgroundSphere.transform.position = Camera.main.transform.position;
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x0006EE90 File Offset: 0x0006D090
	public void Grab(OVRInput.Controller grabHand)
	{
		this.brush.controllerHand = grabHand;
		this.brush.lineContainer.SetActive(true);
		this.backgroundSphere.enabled = true;
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.grabRoutine = this.FadeSphere(Color.grey, 0.25f, false);
		base.StartCoroutine(this.grabRoutine);
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x0006EF14 File Offset: 0x0006D114
	public void Release()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		this.brush.lineContainer.SetActive(false);
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.releaseRoutine = this.FadeSphere(new Color(0.5f, 0.5f, 0.5f, 0f), 0.25f, true);
		base.StartCoroutine(this.releaseRoutine);
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x0006EF9E File Offset: 0x0006D19E
	private IEnumerator FadeCameraClearColor(Color newColor, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0006EFB4 File Offset: 0x0006D1B4
	private IEnumerator FadeSphere(Color newColor, float fadeTime, bool disableOnFinish = false)
	{
		float timer = 0f;
		Color currentColor = this.backgroundSphere.material.GetColor("_Color");
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			this.backgroundSphere.material.SetColor("_Color", Color.Lerp(currentColor, newColor, t));
			if (disableOnFinish && timer >= fadeTime)
			{
				this.backgroundSphere.enabled = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04001C11 RID: 7185
	public PassthroughBrush brush;

	// Token: 0x04001C12 RID: 7186
	public MeshRenderer backgroundSphere;

	// Token: 0x04001C13 RID: 7187
	private IEnumerator grabRoutine;

	// Token: 0x04001C14 RID: 7188
	private IEnumerator releaseRoutine;
}
