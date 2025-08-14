using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000371 RID: 881
public class ObjectManipulator : MonoBehaviour
{
	// Token: 0x060014CD RID: 5325 RVA: 0x00071330 File Offset: 0x0006F530
	private void Start()
	{
		if (this.passthrough)
		{
			this.passthrough.colorMapEditorBrightness = -1f;
			this.passthrough.colorMapEditorContrast = -1f;
		}
		base.StartCoroutine(this.StartDemo());
		if (this.objectNameLabel)
		{
			this.objectNameLabel.font.material.renderQueue = 4600;
		}
		if (this.objectInstructionsLabel)
		{
			this.objectInstructionsLabel.font.material.renderQueue = 4600;
		}
		if (this.objectInfoBG)
		{
			this.objectInfoBG.materialForRendering.renderQueue = 4599;
		}
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000713E8 File Offset: 0x0006F5E8
	private void Update()
	{
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(this.controller);
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(this.controller);
		this.FindHoverObject(localControllerPosition, localControllerRotation);
		if (this.hoverObject && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, this.controller))
		{
			this.grabObject = this.hoverObject;
			this.GrabHoverObject(this.grabObject, localControllerPosition, localControllerRotation);
		}
		if (this.grabObject)
		{
			this.grabTime += Time.deltaTime * 5f;
			this.grabTime = Mathf.Clamp01(this.grabTime);
			this.ManipulateObject(this.grabObject, localControllerPosition, localControllerRotation);
			if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, this.controller))
			{
				this.ReleaseObject();
				return;
			}
		}
		else
		{
			this.grabTime -= Time.deltaTime * 5f;
			this.grabTime = Mathf.Clamp01(this.grabTime);
		}
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000714D4 File Offset: 0x0006F6D4
	private void GrabHoverObject(GameObject grbObj, Vector3 controllerPos, Quaternion controllerRot)
	{
		this.localGrabOffset = Quaternion.Inverse(controllerRot) * (this.grabObject.transform.position - controllerPos);
		this.localGrabRotation = Quaternion.Inverse(controllerRot) * this.grabObject.transform.rotation;
		this.rotationOffset = 0f;
		if (this.grabObject.GetComponent<GrabObject>())
		{
			this.grabObject.GetComponent<GrabObject>().Grab(this.controller);
			this.grabObject.GetComponent<GrabObject>().grabbedRotation = this.grabObject.transform.rotation;
			this.AssignInstructions(this.grabObject.GetComponent<GrabObject>());
		}
		this.handGrabPosition = controllerPos;
		this.handGrabRotation = controllerRot;
		this.camGrabPosition = Camera.main.transform.position;
		this.camGrabRotation = Camera.main.transform.rotation;
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x000715C8 File Offset: 0x0006F7C8
	private void ReleaseObject()
	{
		if (this.grabObject.GetComponent<GrabObject>())
		{
			if (this.grabObject.GetComponent<GrabObject>().objectManipulationType == GrabObject.ManipulationType.Menu)
			{
				this.grabObject.transform.position = this.handGrabPosition + this.handGrabRotation * this.localGrabOffset;
				this.grabObject.transform.rotation = this.handGrabRotation * this.localGrabRotation;
			}
			this.grabObject.GetComponent<GrabObject>().Release();
		}
		this.grabObject = null;
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x0007165E File Offset: 0x0006F85E
	private IEnumerator StartDemo()
	{
		this.demoObjects.SetActive(false);
		float timer = 0f;
		float fadeTime = 1f;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			if (this.passthrough)
			{
				this.passthrough.colorMapEditorBrightness = Mathf.Lerp(-1f, 0f, t);
				this.passthrough.colorMapEditorContrast = Mathf.Lerp(-1f, 0f, t);
			}
			yield return null;
		}
		this.demoObjects.SetActive(true);
		Vector3 normalized = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
		this.demoObjects.transform.position = Camera.main.transform.position + normalized;
		this.demoObjects.transform.rotation = Quaternion.LookRotation(normalized);
		yield break;
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x00071670 File Offset: 0x0006F870
	private void FindHoverObject(Vector3 controllerPos, Quaternion controllerRot)
	{
		RaycastHit[] array = Physics.RaycastAll(controllerPos, controllerRot * Vector3.forward);
		float num = float.PositiveInfinity;
		float d = 2f;
		bool enabled = true;
		Vector3 vector = Vector3.zero;
		foreach (RaycastHit raycastHit in array)
		{
			float num2 = Vector3.Distance(raycastHit.point, controllerPos);
			if (num2 < num)
			{
				this.hoverObject = raycastHit.collider.gameObject;
				num = num2;
				d = (this.grabObject ? num2 : (num2 - 0.1f));
				vector = raycastHit.point;
			}
		}
		if (array.Length == 0)
		{
			this.hoverObject = null;
		}
		foreach (Collider collider in Physics.OverlapSphere(controllerPos, 0.05f))
		{
			this.hoverObject = collider.gameObject;
			enabled = false;
			vector = collider.ClosestPoint(controllerPos);
			vector += (Camera.main.transform.position - vector).normalized * 0.05f;
		}
		if (this.objectInfo && this.objectInstructionsLabel)
		{
			bool active = this.hoverObject || this.grabObject;
			this.objectInfo.gameObject.SetActive(active);
			Vector3 forward = vector - Camera.main.transform.position;
			if (this.hoverObject && !this.grabObject)
			{
				Vector3 a = vector - forward.normalized * 0.05f;
				this.objectInfo.position = Vector3.Lerp(a, this.objectInfo.position, this.grabTime);
				this.objectInfo.rotation = Quaternion.LookRotation(forward);
				this.objectInfo.localScale = Vector3.one * forward.magnitude * 2f;
				if (this.hoverObject.GetComponent<GrabObject>())
				{
					this.AssignInstructions(this.hoverObject.GetComponent<GrabObject>());
				}
			}
			else if (this.grabObject)
			{
				Vector3 b = controllerPos + (Camera.main.transform.position - controllerPos).normalized * 0.1f;
				this.objectInfo.position = Vector3.Lerp(this.objectInfo.position, b, this.grabTime);
				this.objectInfo.rotation = Quaternion.LookRotation(this.objectInfo.position - Camera.main.transform.position);
				this.objectInfo.localScale = Vector3.one;
				if (this.grabObject.GetComponent<GrabObject>())
				{
					enabled = this.grabObject.GetComponent<GrabObject>().showLaserWhileGrabbed;
				}
			}
		}
		if (this.laser)
		{
			this.laser.positionCount = 2;
			Vector3 position = controllerPos + controllerRot * (Vector3.forward * 0.05f);
			this.cursorPosition = controllerPos + controllerRot * (Vector3.forward * d);
			this.laser.SetPosition(0, position);
			this.laser.SetPosition(1, this.cursorPosition);
			this.laser.enabled = enabled;
			if (this.grabObject && this.grabObject.GetComponent<GrabObject>())
			{
				this.grabObject.GetComponent<GrabObject>().CursorPos(this.cursorPosition);
			}
		}
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x00071A34 File Offset: 0x0006FC34
	private void ManipulateObject(GameObject obj, Vector3 controllerPos, Quaternion controllerRot)
	{
		bool flag = true;
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.controller);
		if (obj.GetComponent<GrabObject>())
		{
			flag = false;
			switch (obj.GetComponent<GrabObject>().objectManipulationType)
			{
			case GrabObject.ManipulationType.Default:
				flag = true;
				break;
			case GrabObject.ManipulationType.ForcedHand:
				obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, this.grabTime);
				break;
			case GrabObject.ManipulationType.DollyHand:
			{
				float num = this.localGrabOffset.z + vector.y * 0.01f;
				num = Mathf.Clamp(num, 0.1f, 2f);
				this.localGrabOffset = Vector3.forward * num;
				obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos + controllerRot * this.localGrabOffset, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, this.grabTime);
				break;
			}
			case GrabObject.ManipulationType.DollyAttached:
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				obj.transform.rotation = controllerRot * this.localGrabRotation;
				this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				break;
			case GrabObject.ManipulationType.HorizontalScaled:
			{
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, this.controller))
				{
					obj.transform.localScale = this.ClampScale(obj.transform.localScale, vector);
				}
				else
				{
					this.rotationOffset -= vector.x;
					this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				}
				Vector3 vector2 = obj.GetComponent<GrabObject>().grabbedRotation * Vector3.forward;
				vector2 = new Vector3(vector2.x, 0f, vector2.z);
				obj.transform.rotation = Quaternion.Euler(0f, this.rotationOffset, 0f) * Quaternion.LookRotation(vector2);
				break;
			}
			case GrabObject.ManipulationType.VerticalScaled:
			{
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, this.controller))
				{
					obj.transform.localScale = this.ClampScale(obj.transform.localScale, vector);
				}
				else
				{
					this.rotationOffset -= vector.x;
					this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				}
				Vector3 vector3 = obj.GetComponent<GrabObject>().grabbedRotation * Vector3.up;
				vector3 = new Vector3(vector3.x, 0f, vector3.z);
				obj.transform.rotation = Quaternion.LookRotation(Vector3.up, Quaternion.Euler(0f, this.rotationOffset, 0f) * vector3);
				break;
			}
			case GrabObject.ManipulationType.Menu:
			{
				Vector3 vector4 = this.handGrabPosition + this.handGrabRotation * Vector3.forward * 0.4f;
				Quaternion b = Quaternion.LookRotation(vector4 - this.camGrabPosition);
				obj.transform.position = Vector3.Lerp(obj.transform.position, vector4, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, b, this.grabTime);
				break;
			}
			default:
				flag = true;
				break;
			}
		}
		if (flag)
		{
			obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
			obj.transform.Rotate(Vector3.up * -vector.x);
			this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
		}
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x00071E48 File Offset: 0x00070048
	private void ClampGrabOffset(ref Vector3 localOffset, float thumbY)
	{
		Vector3 vector = localOffset + Vector3.forward * thumbY * 0.01f;
		if (vector.z > 0.1f)
		{
			localOffset = vector;
		}
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x00071E8C File Offset: 0x0007008C
	private Vector3 ClampScale(Vector3 localScale, Vector2 thumb)
	{
		float num = localScale.x + thumb.x * 0.01f;
		if (num <= 0.1f)
		{
			num = 0.1f;
		}
		float num2 = localScale.z + thumb.y * 0.01f;
		if (num2 <= 0.1f)
		{
			num2 = 0.1f;
		}
		return new Vector3(num, 0f, num2);
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x00071EEC File Offset: 0x000700EC
	private void CheckForDominantHand()
	{
		if (this.hoverObject || this.grabObject)
		{
			return;
		}
		if (this.controller == OVRInput.Controller.RTouch)
		{
			if (OVRInput.Get(OVRInput.RawButton.LHandTrigger, OVRInput.Controller.Active))
			{
				this.controller = OVRInput.Controller.LTouch;
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.RawButton.RHandTrigger, OVRInput.Controller.Active))
		{
			this.controller = OVRInput.Controller.RTouch;
		}
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x00071F50 File Offset: 0x00070150
	private void AssignInstructions(GrabObject targetObject)
	{
		if (this.objectNameLabel)
		{
			this.objectNameLabel.text = targetObject.ObjectName;
		}
		if (this.objectInstructionsLabel)
		{
			if (this.grabObject)
			{
				this.objectInstructionsLabel.text = targetObject.ObjectInstructions;
				return;
			}
			this.objectInstructionsLabel.text = "Grip Trigger to grab";
		}
	}

	// Token: 0x04001C61 RID: 7265
	private OVRInput.Controller controller = OVRInput.Controller.RTouch;

	// Token: 0x04001C62 RID: 7266
	private GameObject hoverObject;

	// Token: 0x04001C63 RID: 7267
	private GameObject grabObject;

	// Token: 0x04001C64 RID: 7268
	private float grabTime;

	// Token: 0x04001C65 RID: 7269
	private Vector3 localGrabOffset = Vector3.zero;

	// Token: 0x04001C66 RID: 7270
	private Quaternion localGrabRotation = Quaternion.identity;

	// Token: 0x04001C67 RID: 7271
	private Vector3 camGrabPosition = Vector3.zero;

	// Token: 0x04001C68 RID: 7272
	private Quaternion camGrabRotation = Quaternion.identity;

	// Token: 0x04001C69 RID: 7273
	private Vector3 handGrabPosition = Vector3.zero;

	// Token: 0x04001C6A RID: 7274
	private Quaternion handGrabRotation = Quaternion.identity;

	// Token: 0x04001C6B RID: 7275
	private Vector3 cursorPosition = Vector3.zero;

	// Token: 0x04001C6C RID: 7276
	private float rotationOffset;

	// Token: 0x04001C6D RID: 7277
	public LineRenderer laser;

	// Token: 0x04001C6E RID: 7278
	public Transform objectInfo;

	// Token: 0x04001C6F RID: 7279
	public TextMesh objectNameLabel;

	// Token: 0x04001C70 RID: 7280
	public TextMesh objectInstructionsLabel;

	// Token: 0x04001C71 RID: 7281
	public Image objectInfoBG;

	// Token: 0x04001C72 RID: 7282
	public GameObject demoObjects;

	// Token: 0x04001C73 RID: 7283
	public OVRPassthroughLayer passthrough;
}
