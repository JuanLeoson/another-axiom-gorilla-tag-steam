using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A3 RID: 931
[RequireComponent(typeof(OVRSceneAnchor))]
[DefaultExecutionOrder(30)]
public class FurnitureSpawner : MonoBehaviour
{
	// Token: 0x0600159F RID: 5535 RVA: 0x000763C7 File Offset: 0x000745C7
	private void Start()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		this._classification = base.GetComponent<OVRSemanticClassification>();
		this.AddRoomLight();
		this.SpawnSpawnable();
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x000763F0 File Offset: 0x000745F0
	private void SpawnSpawnable()
	{
		SimpleResizable sourcePrefab;
		if (!this.FindValidSpawnable(out sourcePrefab))
		{
			return;
		}
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 localScale = base.transform.localScale;
		OVRScenePlane component = this._sceneAnchor.GetComponent<OVRScenePlane>();
		OVRSceneVolume component2 = this._sceneAnchor.GetComponent<OVRSceneVolume>();
		Vector3 newSize = component2 ? component2.Dimensions : Vector3.one;
		if (this._classification && component)
		{
			newSize = component.Dimensions;
			newSize.z = 1f;
			if (this._classification.Contains("TABLE") || this._classification.Contains("COUCH"))
			{
				this.GetVolumeFromTopPlane(base.transform, component.Dimensions, base.transform.position.y, out position, out rotation, out localScale);
				newSize = localScale;
				position.y += localScale.y / 2f;
			}
			if (this._classification.Contains("WALL_FACE") || this._classification.Contains("CEILING") || this._classification.Contains("FLOOR"))
			{
				newSize.z = 0.01f;
			}
		}
		GameObject gameObject = new GameObject("Root");
		gameObject.transform.parent = base.transform;
		gameObject.transform.SetPositionAndRotation(position, rotation);
		new SimpleResizer().CreateResizedObject(newSize, gameObject, sourcePrefab);
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0007657C File Offset: 0x0007477C
	private bool FindValidSpawnable(out SimpleResizable currentSpawnable)
	{
		currentSpawnable = null;
		if (!this._classification)
		{
			return false;
		}
		if (!Object.FindObjectOfType<OVRSceneManager>())
		{
			return false;
		}
		foreach (Spawnable spawnable in this.SpawnablePrefabs)
		{
			if (this._classification.Contains(spawnable.ClassificationLabel))
			{
				currentSpawnable = spawnable.ResizablePrefab;
				return true;
			}
		}
		if (this.FallbackPrefab != null)
		{
			currentSpawnable = this.FallbackPrefab;
			return true;
		}
		return false;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x00076624 File Offset: 0x00074824
	private void AddRoomLight()
	{
		if (!this.RoomLightPrefab)
		{
			return;
		}
		if (this._classification && this._classification.Contains("CEILING") && !FurnitureSpawner._roomLightRef)
		{
			FurnitureSpawner._roomLightRef = Object.Instantiate<GameObject>(this.RoomLightPrefab, this._sceneAnchor.transform);
		}
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x00076688 File Offset: 0x00074888
	private void GetVolumeFromTopPlane(Transform plane, Vector2 dimensions, float height, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		float num = height / 2f;
		position = plane.position - Vector3.up * num;
		rotation = Quaternion.LookRotation(-plane.up, Vector3.up);
		localScale = new Vector3(dimensions.x, num * 2f, dimensions.y);
	}

	// Token: 0x04001D6A RID: 7530
	[Tooltip("Add a point at ceiling.")]
	public GameObject RoomLightPrefab;

	// Token: 0x04001D6B RID: 7531
	[Tooltip("This prefab will be used if the label is not in the SpawnablesPrefabs")]
	public SimpleResizable FallbackPrefab;

	// Token: 0x04001D6C RID: 7532
	public List<Spawnable> SpawnablePrefabs;

	// Token: 0x04001D6D RID: 7533
	private OVRSceneAnchor _sceneAnchor;

	// Token: 0x04001D6E RID: 7534
	private OVRSemanticClassification _classification;

	// Token: 0x04001D6F RID: 7535
	private static GameObject _roomLightRef;

	// Token: 0x04001D70 RID: 7536
	private int _frameCounter;
}
