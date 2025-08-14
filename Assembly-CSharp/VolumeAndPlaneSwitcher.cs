using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AE RID: 942
[RequireComponent(typeof(OVRSceneAnchor))]
public class VolumeAndPlaneSwitcher : MonoBehaviour
{
	// Token: 0x060015D1 RID: 5585 RVA: 0x000773A0 File Offset: 0x000755A0
	private void ReplaceAnchor(OVRSceneAnchor prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
	{
		OVRSceneAnchor ovrsceneAnchor = Object.Instantiate<OVRSceneAnchor>(prefab, base.transform.parent);
		ovrsceneAnchor.enabled = false;
		ovrsceneAnchor.InitializeFrom(base.GetComponent<OVRSceneAnchor>());
		ovrsceneAnchor.transform.SetPositionAndRotation(position, rotation);
		foreach (object obj in ovrsceneAnchor.transform)
		{
			((Transform)obj).localScale = localScale;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x00077434 File Offset: 0x00075634
	private void Start()
	{
		OVRSemanticClassification component = base.GetComponent<OVRSemanticClassification>();
		if (!component)
		{
			return;
		}
		foreach (VolumeAndPlaneSwitcher.LabelGeometryPair labelGeometryPair in this.desiredSwitches)
		{
			if (component.Contains(labelGeometryPair.label))
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				Vector3 zero2 = Vector3.zero;
				VolumeAndPlaneSwitcher.GeometryType desiredGeometryType = labelGeometryPair.desiredGeometryType;
				if (desiredGeometryType != VolumeAndPlaneSwitcher.GeometryType.Plane)
				{
					if (desiredGeometryType == VolumeAndPlaneSwitcher.GeometryType.Volume)
					{
						OVRScenePlane component2 = base.GetComponent<OVRScenePlane>();
						if (!component2)
						{
							Debug.LogWarning("Ignoring desired plane to volume switch for " + labelGeometryPair.label + " because it is not a plane.");
						}
						else
						{
							Debug.Log(string.Format("IN Plane Position {0}, Dimensions: {1}", base.transform.position, component2.Dimensions));
							this.GetVolumeFromTopPlane(base.transform, component2.Dimensions, base.transform.position.y, out zero, out identity, out zero2);
							Debug.Log(string.Format("OUT Volume Position {0}, Dimensions: {1}", zero, zero2));
							this.ReplaceAnchor(this.volumePrefab, zero, identity, zero2);
						}
					}
				}
				else
				{
					OVRSceneVolume component3 = base.GetComponent<OVRSceneVolume>();
					if (!component3)
					{
						Debug.LogWarning("Ignoring desired volume to plane switch for " + labelGeometryPair.label + " because it is not a volume.");
					}
					else
					{
						Debug.Log(string.Format("IN Volume Position {0}, Dimensions: {1}", base.transform.position, component3.Dimensions));
						this.GetTopPlaneFromVolume(base.transform, component3.Dimensions, out zero, out identity, out zero2);
						Debug.Log(string.Format("OUT Plane Position {0}, Dimensions: {1}", zero, zero2));
						this.ReplaceAnchor(this.planePrefab, zero, identity, zero2);
					}
				}
			}
		}
		Object.Destroy(this);
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x00077640 File Offset: 0x00075840
	private void GetVolumeFromTopPlane(Transform plane, Vector2 dimensions, float height, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		position = plane.position;
		rotation = plane.rotation;
		localScale = new Vector3(dimensions.x, dimensions.y, height);
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x00077678 File Offset: 0x00075878
	private void GetTopPlaneFromVolume(Transform volume, Vector3 dimensions, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		float d = dimensions.y / 2f;
		position = volume.position + Vector3.up * d;
		rotation = Quaternion.LookRotation(Vector3.up, -volume.forward);
		localScale = new Vector3(dimensions.x, dimensions.z, dimensions.y);
	}

	// Token: 0x04001D93 RID: 7571
	public OVRSceneAnchor planePrefab;

	// Token: 0x04001D94 RID: 7572
	public OVRSceneAnchor volumePrefab;

	// Token: 0x04001D95 RID: 7573
	public List<VolumeAndPlaneSwitcher.LabelGeometryPair> desiredSwitches;

	// Token: 0x020003AF RID: 943
	public enum GeometryType
	{
		// Token: 0x04001D97 RID: 7575
		Plane,
		// Token: 0x04001D98 RID: 7576
		Volume
	}

	// Token: 0x020003B0 RID: 944
	[Serializable]
	public struct LabelGeometryPair
	{
		// Token: 0x04001D99 RID: 7577
		public string label;

		// Token: 0x04001D9A RID: 7578
		public VolumeAndPlaneSwitcher.GeometryType desiredGeometryType;
	}
}
