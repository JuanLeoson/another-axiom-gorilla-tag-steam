using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FAA RID: 4010
	public class GizmosUtil
	{
		// Token: 0x06006423 RID: 25635 RVA: 0x001F8003 File Offset: 0x001F6203
		public static void DrawLine(Vector3 v0, Vector3 v1, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(v0, v1);
		}

		// Token: 0x06006424 RID: 25636 RVA: 0x001F8014 File Offset: 0x001F6214
		public static void DrawLines(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i += 2)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x06006425 RID: 25637 RVA: 0x001F804C File Offset: 0x001F624C
		public static void DrawLineStrip(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i++)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x001F8084 File Offset: 0x001F6284
		public static void DrawBox(Vector3 center, Quaternion rotation, Vector3 dimensions, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (dimensions.x < MathUtil.Epsilon || dimensions.y < MathUtil.Epsilon || dimensions.z < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			if (style != GizmosUtil.Style.Wireframe)
			{
				if (style - GizmosUtil.Style.FlatShaded <= 1)
				{
					mesh = PrimitiveMeshFactory.BoxFlatShaded();
				}
			}
			else
			{
				mesh = PrimitiveMeshFactory.BoxWireframe();
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, dimensions);
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, dimensions);
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x001F8100 File Offset: 0x001F6300
		public static void DrawCylinder(Vector3 center, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CylinderWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CylinderFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CylinderSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x001F8188 File Offset: 0x001F6388
		public static void DrawCylinder(Vector3 point0, Vector3 point1, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCylinder(center, rotation, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x001F820C File Offset: 0x001F640C
		public static void DrawSphere(Vector3 center, Quaternion rotation, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.SphereWireframe(latSegments, longSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.SphereFlatShaded(latSegments, longSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.SphereSmoothShaded(latSegments, longSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x001F828E File Offset: 0x001F648E
		public static void DrawSphere(Vector3 center, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			GizmosUtil.DrawSphere(center, Quaternion.identity, radius, latSegments, longSegments, color, style);
		}

		// Token: 0x0600642B RID: 25643 RVA: 0x001F82A4 File Offset: 0x001F64A4
		public static void DrawCapsule(Vector3 center, Quaternion rotation, float height, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			Mesh mesh2 = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			}
			if (mesh == null || mesh2 == null)
			{
				return;
			}
			Vector3 vector = rotation * Vector3.up;
			Vector3 b = 0.5f * (height - radius) * vector;
			Vector3 position = center + b;
			Vector3 position2 = center - b;
			Quaternion rotation2 = Quaternion.AngleAxis(180f, vector) * rotation;
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, position, rotation, new Vector3(radius, radius, radius));
				Gizmos.DrawWireMesh(mesh, position2, rotation2, new Vector3(-radius, -radius, radius));
				Gizmos.DrawWireMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, position, rotation, new Vector3(radius, radius, radius));
			Gizmos.DrawMesh(mesh, position2, rotation2, new Vector3(-radius, -radius, radius));
			Gizmos.DrawMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x001F83F8 File Offset: 0x001F65F8
		public static void DrawCapsule(Vector3 point0, Vector3 point1, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCapsule(center, rotation, magnitude, radius, latSegmentsPerCap, longSegmentsPerCap, color, style);
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x001F847C File Offset: 0x001F667C
		public static void DrawCone(Vector3 baseCenter, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.ConeWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.ConeFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.ConeSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x001F8504 File Offset: 0x001F6704
		public static void DrawCone(Vector3 baseCenter, Vector3 top, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = top - baseCenter;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(baseCenter, rotation, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x001F8570 File Offset: 0x001F6770
		public static void DrawArrow(Vector3 from, Vector3 to, float coneRadius, float coneHeight, int numSegments, float stemThickness, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = to - from;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(to - coneHeight * vector, rotation, coneHeight, coneRadius, numSegments, color, style);
			if (stemThickness <= 0f)
			{
				if (style != GizmosUtil.Style.Wireframe)
				{
					to -= coneHeight * vector;
				}
				GizmosUtil.DrawLine(from, to, color);
				return;
			}
			if (coneHeight < magnitude)
			{
				to -= coneHeight * vector;
				GizmosUtil.DrawCylinder(from, to, 0.5f * stemThickness, numSegments, color, style);
			}
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x001F8636 File Offset: 0x001F6836
		public static void DrawArrow(Vector3 from, Vector3 to, float size, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			GizmosUtil.DrawArrow(from, to, 0.5f * size, size, 8, 0f, color, style);
		}

		// Token: 0x02000FAB RID: 4011
		public enum Style
		{
			// Token: 0x04006EEB RID: 28395
			Wireframe,
			// Token: 0x04006EEC RID: 28396
			FlatShaded,
			// Token: 0x04006EED RID: 28397
			SmoothShaded
		}
	}
}
