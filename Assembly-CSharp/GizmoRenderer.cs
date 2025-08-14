using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200086B RID: 2155
public class GizmoRenderer : MonoBehaviour
{
	// Token: 0x0600361F RID: 13855 RVA: 0x0011C154 File Offset: 0x0011A354
	private void Update()
	{
		this.RenderGizmos();
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x0011C15C File Offset: 0x0011A35C
	private unsafe void RenderGizmos()
	{
		if (this.renderMode == GizmoRenderer.RenderMode.Never)
		{
			return;
		}
		if (this.gizmos == null)
		{
			return;
		}
		int num = this.gizmos.Length;
		if (num == 0)
		{
			return;
		}
		CommandBuilder arg = *Draw.ingame;
		Transform transform = base.transform;
		for (int i = 0; i < num; i++)
		{
			GizmoRenderer.GizmoInfo gizmoInfo = this.gizmos[i];
			if (gizmoInfo.render)
			{
				Transform transform2 = gizmoInfo.target ? gizmoInfo.target : transform;
				using (arg.InLocalSpace(transform2))
				{
					using (arg.WithLineWidth(gizmoInfo.lineWidth, false))
					{
						GizmoRenderer.gRenderFuncs[(int)gizmoInfo.type](arg, gizmoInfo);
					}
				}
			}
		}
	}

	// Token: 0x06003621 RID: 13857 RVA: 0x0011C248 File Offset: 0x0011A448
	private static void RenderPlaneWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WirePlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06003622 RID: 13858 RVA: 0x0011C26E File Offset: 0x0011A46E
	private static void RenderPlaneSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidPlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06003623 RID: 13859 RVA: 0x0011C294 File Offset: 0x0011A494
	private static void RenderGridWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireGrid(gizmo.center, gizmo.rotation, gizmo.gridCells, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x0011C2C0 File Offset: 0x0011A4C0
	private static void RenderBoxWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06003625 RID: 13861 RVA: 0x0011C2E1 File Offset: 0x0011A4E1
	private static void RenderBoxSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06003626 RID: 13862 RVA: 0x0011C302 File Offset: 0x0011A502
	private static void RenderSphereWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireSphere(gizmo.center, gizmo.radius * 0.5f, gizmo.color);
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x0011C324 File Offset: 0x0011A524
	private static void RenderSphereSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		Matrix4x4 matrix = Matrix4x4.TRS(gizmo.center, quaternion.identity, new float3(gizmo.radius));
		using (draw.WithMatrix(matrix))
		{
			draw.SolidMesh(GizmoRenderer.gSphereMesh, gizmo.color);
		}
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x0011C398 File Offset: 0x0011A598
	private static void RenderLabel3D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label3D(gizmo.center, gizmo.rotation, gizmo.text, gizmo.textSize * 0.1f, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x0011C3D5 File Offset: 0x0011A5D5
	private static void RenderLabel2D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label2D(gizmo.center, gizmo.text, gizmo.textSize * gizmo.textPPU, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x0011C40F File Offset: 0x0011A60F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GizmoRenderer.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
	}

	// Token: 0x0600362B RID: 13867 RVA: 0x0011C420 File Offset: 0x0011A620
	private static Color GetRandomColor()
	{
		Color result = Color.HSVToRGB((float)(DateTime.UtcNow.Ticks % 65536L) / 65535f, 1f, 1f, true);
		result.a = 1f;
		return result;
	}

	// Token: 0x040042F3 RID: 17139
	public GizmoRenderer.RenderMode renderMode = GizmoRenderer.RenderMode.Always;

	// Token: 0x040042F4 RID: 17140
	public bool includeInBuild;

	// Token: 0x040042F5 RID: 17141
	public GizmoRenderer.GizmoInfo[] gizmos = new GizmoRenderer.GizmoInfo[0];

	// Token: 0x040042F6 RID: 17142
	private static readonly Action<CommandBuilder, GizmoRenderer.GizmoInfo>[] gRenderFuncs = new Action<CommandBuilder, GizmoRenderer.GizmoInfo>[]
	{
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel3D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel2D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderGridWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneWire)
	};

	// Token: 0x040042F7 RID: 17143
	private static readonly LabelAlignment[] gLabelAligns = new LabelAlignment[]
	{
		LabelAlignment.Center,
		LabelAlignment.MiddleRight,
		LabelAlignment.MiddleLeft,
		LabelAlignment.BottomCenter,
		LabelAlignment.BottomRight,
		LabelAlignment.BottomLeft,
		LabelAlignment.TopRight,
		LabelAlignment.TopLeft,
		LabelAlignment.TopCenter
	};

	// Token: 0x040042F8 RID: 17144
	private static Mesh gSphereMesh;

	// Token: 0x0200086C RID: 2156
	[Serializable]
	public class GizmoInfo
	{
		// Token: 0x040042F9 RID: 17145
		public bool render = true;

		// Token: 0x040042FA RID: 17146
		public GizmoRenderer.GizmoType type;

		// Token: 0x040042FB RID: 17147
		public Color color = GizmoRenderer.GetRandomColor();

		// Token: 0x040042FC RID: 17148
		public uint lineWidth = 1U;

		// Token: 0x040042FD RID: 17149
		[Space]
		public Transform target;

		// Token: 0x040042FE RID: 17150
		[Space]
		public float3 center = float3.zero;

		// Token: 0x040042FF RID: 17151
		public float3 size = Vector3.one;

		// Token: 0x04004300 RID: 17152
		public float radius = 1f;

		// Token: 0x04004301 RID: 17153
		public quaternion rotation = quaternion.identity;

		// Token: 0x04004302 RID: 17154
		[Space]
		public string text = string.Empty;

		// Token: 0x04004303 RID: 17155
		public float textSize = 4f;

		// Token: 0x04004304 RID: 17156
		public GizmoRenderer.TextAlign textAlign;

		// Token: 0x04004305 RID: 17157
		public uint textPPU = 24U;

		// Token: 0x04004306 RID: 17158
		[Space]
		public int2 gridCells = new int2(4);
	}

	// Token: 0x0200086D RID: 2157
	[Flags]
	public enum RenderMode : uint
	{
		// Token: 0x04004308 RID: 17160
		Never = 0U,
		// Token: 0x04004309 RID: 17161
		InEditor = 1U,
		// Token: 0x0400430A RID: 17162
		InBuild = 2U,
		// Token: 0x0400430B RID: 17163
		Always = 3U
	}

	// Token: 0x0200086E RID: 2158
	public enum GizmoType : uint
	{
		// Token: 0x0400430D RID: 17165
		BoxWire,
		// Token: 0x0400430E RID: 17166
		BoxSolid,
		// Token: 0x0400430F RID: 17167
		SphereWire,
		// Token: 0x04004310 RID: 17168
		SphereSolid,
		// Token: 0x04004311 RID: 17169
		Label3D,
		// Token: 0x04004312 RID: 17170
		Label2D,
		// Token: 0x04004313 RID: 17171
		GridWire,
		// Token: 0x04004314 RID: 17172
		PlaneSolid,
		// Token: 0x04004315 RID: 17173
		PlaneWire
	}

	// Token: 0x0200086F RID: 2159
	public enum TextAlign : uint
	{
		// Token: 0x04004317 RID: 17175
		Center,
		// Token: 0x04004318 RID: 17176
		MiddleRight,
		// Token: 0x04004319 RID: 17177
		MiddleLeft,
		// Token: 0x0400431A RID: 17178
		BottomCenter,
		// Token: 0x0400431B RID: 17179
		BottomRight,
		// Token: 0x0400431C RID: 17180
		BottomLeft,
		// Token: 0x0400431D RID: 17181
		TopRight,
		// Token: 0x0400431E RID: 17182
		TopLeft,
		// Token: 0x0400431F RID: 17183
		TopCenter
	}
}
