using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000488 RID: 1160
[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	// Token: 0x06001CCC RID: 7372 RVA: 0x0009B195 File Offset: 0x00099395
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x0009B1B5 File Offset: 0x000993B5
	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x0009B1D8 File Offset: 0x000993D8
	public static bool IsUserNearWardrobe(string userID)
	{
		int layerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		});
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			int num = Physics.OverlapSphereNonAlloc(sphereCollider.transform.position, sphereCollider.radius, CosmeticWardrobeProximityDetector.overlapColliders, layerMask);
			num = Mathf.Min(num, CosmeticWardrobeProximityDetector.overlapColliders.Length);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					Collider collider = CosmeticWardrobeProximityDetector.overlapColliders[i];
					if (!(collider == null))
					{
						GameObject gameObject = collider.attachedRigidbody.gameObject;
						VRRig component = gameObject.GetComponent<VRRig>();
						if (component == null || component.creator == null || component.creator.IsNull || string.IsNullOrEmpty(component.creator.UserId))
						{
							if (gameObject.GetComponent<GTPlayer>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_135;
							}
							if (userID == NetworkSystem.Instance.LocalPlayer.UserId)
							{
								return true;
							}
						}
						else if (userID == component.creator.UserId)
						{
							return true;
						}
						CosmeticWardrobeProximityDetector.overlapColliders[i] = null;
					}
					IL_135:;
				}
			}
		}
		return false;
	}

	// Token: 0x04002534 RID: 9524
	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	// Token: 0x04002535 RID: 9525
	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	// Token: 0x04002536 RID: 9526
	private static readonly Collider[] overlapColliders = new Collider[20];
}
