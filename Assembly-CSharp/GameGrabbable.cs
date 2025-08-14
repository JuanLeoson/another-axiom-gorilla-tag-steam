using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B5 RID: 1461
public class GameGrabbable : MonoBehaviour
{
	// Token: 0x060023FB RID: 9211 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000C105C File Offset: 0x000BF25C
	public void GetBestGrabPoint(Vector3 handPos, Quaternion handRot, int handIndex, out GameGrab grab)
	{
		float num = 0.15f;
		grab = default(GameGrab);
		grab.position = base.transform.position;
		grab.rotation = base.transform.rotation;
		bool flag = GamePlayer.IsLeftHand(handIndex);
		if (this.snapGrabPoints != null)
		{
			for (int i = 0; i < this.snapGrabPoints.Count; i++)
			{
				GameGrabbable.SnapGrabPoints snapGrabPoints = this.snapGrabPoints[i];
				if (snapGrabPoints.isLeftHand == flag && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_UP, handRot * GameGrabbable.GRAB_UP) >= 0f && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_PALM, handRot * GameGrabbable.GRAB_PALM) >= 0f && (double)(handPos - snapGrabPoints.handTransform.position).sqrMagnitude <= 0.0225)
				{
					grab.position = handPos + handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation) * -snapGrabPoints.handTransform.localPosition;
					grab.rotation = handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation);
				}
			}
		}
		Vector3 vector = grab.position - handPos;
		if (vector.sqrMagnitude > num * num)
		{
			grab.position = handPos + vector.normalized * num;
		}
	}

	// Token: 0x04002D66 RID: 11622
	public GameEntity gameEntity;

	// Token: 0x04002D67 RID: 11623
	public List<GameGrabbable.SnapGrabPoints> snapGrabPoints;

	// Token: 0x04002D68 RID: 11624
	private static Vector3 GRAB_UP = new Vector3(0f, 0f, 1f);

	// Token: 0x04002D69 RID: 11625
	private static Vector3 GRAB_PALM = new Vector3(1f, 0f, 0f);

	// Token: 0x020005B6 RID: 1462
	[Serializable]
	public class SnapGrabPoints
	{
		// Token: 0x04002D6A RID: 11626
		public bool isLeftHand;

		// Token: 0x04002D6B RID: 11627
		public Transform handTransform;
	}
}
