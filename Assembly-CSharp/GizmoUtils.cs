using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drawing;
using UnityEngine;

// Token: 0x02000ABA RID: 2746
public static class GizmoUtils
{
	// Token: 0x06004244 RID: 16964 RVA: 0x0014D9D0 File Offset: 0x0014BBD0
	[Conditional("UNITY_EDITOR")]
	public unsafe static void DrawGizmo(this Collider c, Color color = default(Color))
	{
		if (c == null)
		{
			return;
		}
		if (color == default(Color) && !GizmoUtils.gColliderToColor.TryGetValue(c, out color))
		{
			color = new SRand(c.GetHashCode()).NextColor();
			GizmoUtils.gColliderToColor.Add(c, color);
		}
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider == null)
					{
						MeshCollider meshCollider = c as MeshCollider;
						if (meshCollider != null)
						{
							commandBuilder.WireMesh(meshCollider.sharedMesh, color);
						}
					}
					else
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius, color);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius, color);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size, color);
			}
		}
	}

	// Token: 0x06004245 RID: 16965 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	public static void DrawWireCubeTRS(Vector3 t, Quaternion r, Vector3 s)
	{
	}

	// Token: 0x04004D94 RID: 19860
	private static readonly Dictionary<Collider, Color> gColliderToColor = new Dictionary<Collider, Color>(64);
}
