using System;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class Example : MonoBehaviour
{
	// Token: 0x0600038C RID: 908 RVA: 0x00016050 File Offset: 0x00014250
	private void OnDrawGizmos()
	{
		if (this.debugPoint)
		{
			DebugExtension.DrawPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale);
		}
		if (this.debugBounds)
		{
			DebugExtension.DrawBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color);
		}
		if (this.debugCircle)
		{
			DebugExtension.DrawCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius);
		}
		if (this.debugWireSphere)
		{
			Gizmos.color = this.debugWireSphere_Color;
			Gizmos.DrawWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Radius);
		}
		if (this.debugCylinder)
		{
			DebugExtension.DrawCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius);
		}
		if (this.debugCone)
		{
			DebugExtension.DrawCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle);
		}
		if (this.debugArrow)
		{
			DebugExtension.DrawArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color);
		}
		if (this.debugCapsule)
		{
			DebugExtension.DrawCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius);
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x000161DC File Offset: 0x000143DC
	private void Update()
	{
		DebugExtension.DebugPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale, 0f, true);
		DebugExtension.DebugBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color, 0f, true);
		DebugExtension.DebugCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius, 0f, true);
		DebugExtension.DebugWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Color, this.debugWireSphere_Radius, 0f, true);
		DebugExtension.DebugCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius, 0f, true);
		DebugExtension.DebugCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle, 0f, true);
		DebugExtension.DebugArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color, 0f, true);
		DebugExtension.DebugCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius, 0f, true);
	}

	// Token: 0x040003F0 RID: 1008
	public bool debugPoint;

	// Token: 0x040003F1 RID: 1009
	public Vector3 debugPoint_Position;

	// Token: 0x040003F2 RID: 1010
	public float debugPoint_Scale;

	// Token: 0x040003F3 RID: 1011
	public Color debugPoint_Color;

	// Token: 0x040003F4 RID: 1012
	public bool debugBounds;

	// Token: 0x040003F5 RID: 1013
	public Vector3 debugBounds_Position;

	// Token: 0x040003F6 RID: 1014
	public Vector3 debugBounds_Size;

	// Token: 0x040003F7 RID: 1015
	public Color debugBounds_Color;

	// Token: 0x040003F8 RID: 1016
	public bool debugCircle;

	// Token: 0x040003F9 RID: 1017
	public Vector3 debugCircle_Up;

	// Token: 0x040003FA RID: 1018
	public float debugCircle_Radius;

	// Token: 0x040003FB RID: 1019
	public Color debugCircle_Color;

	// Token: 0x040003FC RID: 1020
	public bool debugWireSphere;

	// Token: 0x040003FD RID: 1021
	public float debugWireSphere_Radius;

	// Token: 0x040003FE RID: 1022
	public Color debugWireSphere_Color;

	// Token: 0x040003FF RID: 1023
	public bool debugCylinder;

	// Token: 0x04000400 RID: 1024
	public Vector3 debugCylinder_End;

	// Token: 0x04000401 RID: 1025
	public float debugCylinder_Radius;

	// Token: 0x04000402 RID: 1026
	public Color debugCylinder_Color;

	// Token: 0x04000403 RID: 1027
	public bool debugCone;

	// Token: 0x04000404 RID: 1028
	public Vector3 debugCone_Direction;

	// Token: 0x04000405 RID: 1029
	public float debugCone_Angle;

	// Token: 0x04000406 RID: 1030
	public Color debugCone_Color;

	// Token: 0x04000407 RID: 1031
	public bool debugArrow;

	// Token: 0x04000408 RID: 1032
	public Vector3 debugArrow_Direction;

	// Token: 0x04000409 RID: 1033
	public Color debugArrow_Color;

	// Token: 0x0400040A RID: 1034
	public bool debugCapsule;

	// Token: 0x0400040B RID: 1035
	public Vector3 debugCapsule_End;

	// Token: 0x0400040C RID: 1036
	public float debugCapsule_Radius;

	// Token: 0x0400040D RID: 1037
	public Color debugCapsule_Color;
}
