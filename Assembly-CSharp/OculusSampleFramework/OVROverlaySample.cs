using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x02000D3D RID: 3389
	public class OVROverlaySample : MonoBehaviour
	{
		// Token: 0x060053E1 RID: 21473 RVA: 0x0019E8BC File Offset: 0x0019CABC
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("OVROverlay Sample", 0);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("Level Loading Example", 0);
			DebugUIBuilder.instance.AddButton("Simulate Level Load", new DebugUIBuilder.OnClick(this.TriggerLoad), -1, 0, false);
			DebugUIBuilder.instance.AddButton("Destroy Cubes", new DebugUIBuilder.OnClick(this.TriggerUnload), -1, 0, false);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("OVROverlay vs. Application Render Comparison", 0);
			DebugUIBuilder.instance.AddRadio("OVROverlay", "group", delegate(Toggle t)
			{
				this.RadioPressed("OVROverlayID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.applicationRadioButton = DebugUIBuilder.instance.AddRadio("Application", "group", delegate(Toggle t)
			{
				this.RadioPressed("ApplicationID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.noneRadioButton = DebugUIBuilder.instance.AddRadio("None", "group", delegate(Toggle t)
			{
				this.RadioPressed("NoneID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			DebugUIBuilder.instance.Show();
			this.CameraAndRenderTargetSetup();
			this.cameraRenderOverlay.enabled = true;
			this.cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			this.spawnedCubes.Capacity = this.numObjectsPerLevel * this.numLevels;
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x0019EA14 File Offset: 0x0019CC14
		private void Update()
		{
			if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
			{
				if (this.inMenu)
				{
					DebugUIBuilder.instance.Hide();
				}
				else
				{
					DebugUIBuilder.instance.Show();
				}
				this.inMenu = !this.inMenu;
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				this.TriggerLoad();
			}
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x0019EA7C File Offset: 0x0019CC7C
		private void ActivateWorldGeo()
		{
			this.worldspaceGeoParent.SetActive(true);
			this.uiGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.applicationLabelTexture;
			Debug.Log("Switched to ActivateWorldGeo");
		}

		// Token: 0x060053E4 RID: 21476 RVA: 0x0019EAE4 File Offset: 0x0019CCE4
		private void ActivateOVROverlay()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(true);
			this.cameraRenderOverlay.enabled = true;
			this.uiGeoParent.SetActive(true);
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.compositorLabelTexture;
			Debug.Log("Switched to ActivateOVROVerlay");
		}

		// Token: 0x060053E5 RID: 21477 RVA: 0x0019EB4C File Offset: 0x0019CD4C
		private void ActivateNone()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.uiGeoParent.SetActive(false);
			this.renderingLabelOverlay.enabled = false;
			Debug.Log("Switched to ActivateNone");
		}

		// Token: 0x060053E6 RID: 21478 RVA: 0x0019EB9F File Offset: 0x0019CD9F
		private void TriggerLoad()
		{
			base.StartCoroutine(this.WaitforOVROverlay());
		}

		// Token: 0x060053E7 RID: 21479 RVA: 0x0019EBAE File Offset: 0x0019CDAE
		private IEnumerator WaitforOVROverlay()
		{
			Transform transform = this.mainCamera.transform;
			Transform transform2 = this.loadingTextQuadOverlay.transform;
			Vector3 position = transform.position + transform.forward * this.distanceFromCamToLoadText;
			position.y = transform.position.y;
			transform2.position = position;
			this.cubemapOverlay.enabled = true;
			this.loadingTextQuadOverlay.enabled = true;
			this.noneRadioButton.isOn = true;
			yield return new WaitForSeconds(0.1f);
			this.ClearObjects();
			this.SimulateLevelLoad();
			this.cubemapOverlay.enabled = false;
			this.loadingTextQuadOverlay.enabled = false;
			yield return null;
			yield break;
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x0019EBBD File Offset: 0x0019CDBD
		private void TriggerUnload()
		{
			this.ClearObjects();
			this.applicationRadioButton.isOn = true;
		}

		// Token: 0x060053E9 RID: 21481 RVA: 0x0019EBD4 File Offset: 0x0019CDD4
		private void CameraAndRenderTargetSetup()
		{
			float x = this.cameraRenderOverlay.transform.localScale.x;
			float y = this.cameraRenderOverlay.transform.localScale.y;
			float z = this.cameraRenderOverlay.transform.localScale.z;
			float num = 2160f;
			float num2 = 1200f;
			float num3 = num * 0.5f;
			float num4 = num2;
			float num5 = this.mainCamera.GetComponent<Camera>().fieldOfView / 2f;
			float num6 = 2f * z * Mathf.Tan(0.017453292f * num5);
			float num7 = num4 / num6 * x;
			float num8 = num6 * this.mainCamera.GetComponent<Camera>().aspect;
			float num9 = num3 / num8 * x;
			float orthographicSize = y / 2f;
			float aspect = x / y;
			this.uiCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
			this.uiCamera.GetComponent<Camera>().aspect = aspect;
			if (this.uiCamera.GetComponent<Camera>().targetTexture != null)
			{
				this.uiCamera.GetComponent<Camera>().targetTexture.Release();
			}
			RenderTexture renderTexture = new RenderTexture((int)num9 * 2, (int)num7 * 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Debug.Log("Created RT of resolution w: " + num9.ToString() + " and h: " + num7.ToString());
			renderTexture.hideFlags = HideFlags.DontSave;
			renderTexture.useMipMap = true;
			renderTexture.filterMode = FilterMode.Trilinear;
			renderTexture.anisoLevel = 4;
			renderTexture.autoGenerateMips = true;
			this.uiCamera.GetComponent<Camera>().targetTexture = renderTexture;
			this.cameraRenderOverlay.textures[0] = renderTexture;
		}

		// Token: 0x060053EA RID: 21482 RVA: 0x0019ED70 File Offset: 0x0019CF70
		private void SimulateLevelLoad()
		{
			int num = 0;
			for (int i = 0; i < this.numLoopsTrigger; i++)
			{
				num++;
			}
			Debug.Log("Finished " + num.ToString() + " Loops");
			Vector3 position = this.mainCamera.transform.position;
			position.y = 0.5f;
			for (int j = 0; j < this.numLevels; j++)
			{
				for (int k = 0; k < this.numObjectsPerLevel; k++)
				{
					float f = (float)k * 3.1415927f * 2f / (float)this.numObjectsPerLevel;
					float d = (k % 2 == 0) ? 1.5f : 1f;
					Vector3 a = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * this.cubeSpawnRadius * d;
					a.y = (float)j * this.heightBetweenItems;
					GameObject gameObject = Object.Instantiate<GameObject>(this.prefabForLevelLoadSim, a + position, Quaternion.identity);
					Transform transform = gameObject.transform;
					transform.LookAt(position);
					Vector3 eulerAngles = transform.rotation.eulerAngles;
					eulerAngles.x = 0f;
					transform.rotation = Quaternion.Euler(eulerAngles);
					this.spawnedCubes.Add(gameObject);
				}
			}
		}

		// Token: 0x060053EB RID: 21483 RVA: 0x0019EEC8 File Offset: 0x0019D0C8
		private void ClearObjects()
		{
			for (int i = 0; i < this.spawnedCubes.Count; i++)
			{
				Object.DestroyImmediate(this.spawnedCubes[i]);
			}
			this.spawnedCubes.Clear();
			GC.Collect();
		}

		// Token: 0x060053EC RID: 21484 RVA: 0x0019EF0C File Offset: 0x0019D10C
		public void RadioPressed(string radioLabel, string group, Toggle t)
		{
			if (string.Compare(radioLabel, "OVROverlayID") == 0)
			{
				this.ActivateOVROverlay();
				return;
			}
			if (string.Compare(radioLabel, "ApplicationID") == 0)
			{
				this.ActivateWorldGeo();
				return;
			}
			if (string.Compare(radioLabel, "NoneID") == 0)
			{
				this.ActivateNone();
			}
		}

		// Token: 0x04005D4C RID: 23884
		private bool inMenu;

		// Token: 0x04005D4D RID: 23885
		private const string ovrOverlayID = "OVROverlayID";

		// Token: 0x04005D4E RID: 23886
		private const string applicationID = "ApplicationID";

		// Token: 0x04005D4F RID: 23887
		private const string noneID = "NoneID";

		// Token: 0x04005D50 RID: 23888
		private Toggle applicationRadioButton;

		// Token: 0x04005D51 RID: 23889
		private Toggle noneRadioButton;

		// Token: 0x04005D52 RID: 23890
		[Header("App vs Compositor Comparison Settings")]
		public GameObject mainCamera;

		// Token: 0x04005D53 RID: 23891
		public GameObject uiCamera;

		// Token: 0x04005D54 RID: 23892
		public GameObject uiGeoParent;

		// Token: 0x04005D55 RID: 23893
		public GameObject worldspaceGeoParent;

		// Token: 0x04005D56 RID: 23894
		public OVROverlay cameraRenderOverlay;

		// Token: 0x04005D57 RID: 23895
		public OVROverlay renderingLabelOverlay;

		// Token: 0x04005D58 RID: 23896
		public Texture applicationLabelTexture;

		// Token: 0x04005D59 RID: 23897
		public Texture compositorLabelTexture;

		// Token: 0x04005D5A RID: 23898
		[Header("Level Loading Sim Settings")]
		public GameObject prefabForLevelLoadSim;

		// Token: 0x04005D5B RID: 23899
		public OVROverlay cubemapOverlay;

		// Token: 0x04005D5C RID: 23900
		public OVROverlay loadingTextQuadOverlay;

		// Token: 0x04005D5D RID: 23901
		public float distanceFromCamToLoadText;

		// Token: 0x04005D5E RID: 23902
		public float cubeSpawnRadius;

		// Token: 0x04005D5F RID: 23903
		public float heightBetweenItems;

		// Token: 0x04005D60 RID: 23904
		public int numObjectsPerLevel;

		// Token: 0x04005D61 RID: 23905
		public int numLevels;

		// Token: 0x04005D62 RID: 23906
		public int numLoopsTrigger = 500000000;

		// Token: 0x04005D63 RID: 23907
		private List<GameObject> spawnedCubes = new List<GameObject>();
	}
}
