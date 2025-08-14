using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FAC RID: 4012
	public class PrimitiveMeshFactory
	{
		// Token: 0x06006432 RID: 25650 RVA: 0x001F8650 File Offset: 0x001F6850
		private static Mesh GetPooledLineMesh()
		{
			if (PrimitiveMeshFactory.s_lineMeshPool == null)
			{
				PrimitiveMeshFactory.s_lineMeshPool = new List<Mesh>();
				PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
			}
			if (PrimitiveMeshFactory.s_lastDrawLineFrame != Time.frameCount)
			{
				PrimitiveMeshFactory.s_iPooledMesh = 0;
				PrimitiveMeshFactory.s_lastDrawLineFrame = Time.frameCount;
			}
			if (PrimitiveMeshFactory.s_iPooledMesh == PrimitiveMeshFactory.s_lineMeshPool.Count)
			{
				PrimitiveMeshFactory.s_lineMeshPool.Capacity *= 2;
				for (int i = PrimitiveMeshFactory.s_iPooledMesh; i < PrimitiveMeshFactory.s_lineMeshPool.Capacity; i++)
				{
					PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
				}
			}
			Mesh mesh = PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh++];
			if (mesh == null)
			{
				mesh = (PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh - 1] = new Mesh());
			}
			return mesh;
		}

		// Token: 0x06006433 RID: 25651 RVA: 0x001F8720 File Offset: 0x001F6920
		public static Mesh Line(Vector3 v0, Vector3 v1)
		{
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			Vector3[] vertices = new Vector3[]
			{
				v0,
				v1
			};
			int[] indices = new int[]
			{
				0,
				1
			};
			pooledLineMesh.vertices = vertices;
			pooledLineMesh.SetIndices(indices, MeshTopology.Lines, 0);
			pooledLineMesh.RecalculateBounds();
			return pooledLineMesh;
		}

		// Token: 0x06006434 RID: 25652 RVA: 0x001F8778 File Offset: 0x001F6978
		public static Mesh Lines(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.Lines, 0);
			return pooledLineMesh;
		}

		// Token: 0x06006435 RID: 25653 RVA: 0x001F87CC File Offset: 0x001F69CC
		public static Mesh LineStrip(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.LineStrip, 0);
			return pooledLineMesh;
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x001F8820 File Offset: 0x001F6A20
		public static Mesh BoxWireframe()
		{
			if (PrimitiveMeshFactory.s_boxWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_boxWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0,
					2,
					6,
					6,
					7,
					7,
					3,
					7,
					4,
					4,
					5,
					5,
					6,
					5,
					1,
					1,
					0,
					0,
					4
				};
				PrimitiveMeshFactory.s_boxWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.SetIndices(indices, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_boxWireframeMesh;
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x001F8964 File Offset: 0x001F6B64
		public static Mesh BoxSolidColor()
		{
			if (PrimitiveMeshFactory.s_boxSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_boxSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					3,
					2,
					6,
					3,
					6,
					7,
					7,
					6,
					5,
					7,
					5,
					4,
					4,
					5,
					1,
					4,
					1,
					0,
					1,
					5,
					6,
					1,
					6,
					2,
					0,
					3,
					7,
					0,
					7,
					4
				};
				PrimitiveMeshFactory.s_boxSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_boxSolidColorMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxSolidColorMesh;
		}

		// Token: 0x06006438 RID: 25656 RVA: 0x001F8A9C File Offset: 0x001F6C9C
		public static Mesh BoxFlatShaded()
		{
			if (PrimitiveMeshFactory.s_boxFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_boxFlatShadedMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				Vector3[] array2 = new Vector3[]
				{
					array[0],
					array[1],
					array[2],
					array[0],
					array[2],
					array[3],
					array[3],
					array[2],
					array[6],
					array[3],
					array[6],
					array[7],
					array[7],
					array[6],
					array[5],
					array[7],
					array[5],
					array[4],
					array[4],
					array[5],
					array[1],
					array[4],
					array[1],
					array[0],
					array[1],
					array[5],
					array[6],
					array[1],
					array[6],
					array[2],
					array[0],
					array[3],
					array[7],
					array[0],
					array[7],
					array[4]
				};
				Vector3[] array3 = new Vector3[]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 0f, 1f),
					new Vector3(-1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				Vector3[] normals = new Vector3[]
				{
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5]
				};
				int[] array4 = new int[array2.Length];
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i] = i;
				}
				PrimitiveMeshFactory.s_boxFlatShadedMesh.vertices = array2;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.SetIndices(array4, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxFlatShadedMesh;
		}

		// Token: 0x06006439 RID: 25657 RVA: 0x001F90D0 File Offset: 0x001F72D0
		public static Mesh RectWireframe()
		{
			if (PrimitiveMeshFactory.s_rectWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_rectWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0
				};
				PrimitiveMeshFactory.s_rectWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.SetIndices(indices, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_rectWireframeMesh;
		}

		// Token: 0x0600643A RID: 25658 RVA: 0x001F91A4 File Offset: 0x001F73A4
		public static Mesh RectSolidColor()
		{
			if (PrimitiveMeshFactory.s_rectSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_rectSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					0,
					2,
					1,
					0,
					3,
					2
				};
				PrimitiveMeshFactory.s_rectSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectSolidColorMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectSolidColorMesh;
		}

		// Token: 0x0600643B RID: 25659 RVA: 0x001F9270 File Offset: 0x001F7470
		public static Mesh RectFlatShaded()
		{
			if (PrimitiveMeshFactory.s_rectFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_rectFlatShadedMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				Vector3[] normals = new Vector3[]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					4,
					6,
					5,
					4,
					7,
					6
				};
				PrimitiveMeshFactory.s_rectFlatShadedMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectFlatShadedMesh;
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x001F9490 File Offset: 0x001F7690
		public static Mesh CircleWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments];
				int[] array2 = new int[numSegments + 1];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					array2[i] = i;
					num2 += num;
				}
				array2[numSegments] = 0;
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_circleWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600643D RID: 25661 RVA: 0x001F9584 File Offset: 0x001F7784
		public static Mesh CircleSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
				}
				array[numSegments] = Vector3.zero;
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600643E RID: 25662 RVA: 0x001F96B4 File Offset: 0x001F78B4
		public static Mesh CircleFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(numSegments + 1) * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[i] = new Vector3(0f, 1f, 0f);
					array3[num++] = numSegments;
					array3[num++] = (i + 1) % numSegments;
					array3[num++] = i;
				}
				array[numSegments] = Vector3.zero;
				array2[numSegments] = new Vector3(0f, 1f, 0f);
				num3 = 0f;
				for (int j = 0; j < numSegments; j++)
				{
					array[j + numSegments + 1] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 -= num2;
					array2[j + numSegments + 1] = new Vector3(0f, -1f, 0f);
					array3[num++] = numSegments * 2 + 1;
					array3[num++] = (j + 1) % numSegments + numSegments + 1;
					array3[num++] = j + (numSegments + 1);
				}
				array[numSegments * 2 + 1] = Vector3.zero;
				array2[numSegments * 2 + 1] = new Vector3(0f, -1f, 0f);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600643F RID: 25663 RVA: 0x001F98F0 File Offset: 0x001F7AF0
		public static Mesh CylinderWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2];
				int[] array2 = new int[numSegments * 6];
				Vector3 a = new Vector3(0f, -0.5f, 0f);
				Vector3 a2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array[i] = a + b;
					array[numSegments + i] = a2 + b;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + (i + 1) % numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006440 RID: 25664 RVA: 0x001F9A80 File Offset: 0x001F7C80
		public static Mesh CylinderSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 2];
				int[] array2 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 2 + 1;
				array[num2] = vector;
				array[num3] = vector2;
				int num4 = 0;
				float num5 = 6.2831855f / (float)numSegments;
				float num6 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num6) * Vector3.right + Mathf.Sin(num6) * Vector3.forward;
					array[num + i] = vector + b;
					array[numSegments + i] = vector2 + b;
					array2[num4++] = num2;
					array2[num4++] = num + i;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num3;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = numSegments + i;
					num6 += num5;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006441 RID: 25665 RVA: 0x001F9C98 File Offset: 0x001F7E98
		public static Mesh CylinderFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 6 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 6;
				int num4 = numSegments * 6 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + b;
					array[numSegments + i] = vector2 + b;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num2 + i * 4] = vector + b;
					array[num2 + i * 4 + 1] = vector2 + b;
					array[num2 + i * 4 + 2] = vector + vector3;
					array[num2 + i * 4 + 3] = vector2 + vector3;
					Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - b).normalized;
					array2[num2 + i * 4] = normalized;
					array2[num2 + i * 4 + 1] = normalized;
					array2[num2 + i * 4 + 2] = normalized;
					array2[num2 + i * 4 + 3] = normalized;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 3;
					array3[num5++] = num2 + i * 4 + 2;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 1;
					array3[num5++] = num2 + i * 4 + 3;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006442 RID: 25666 RVA: 0x001FA030 File Offset: 0x001F8230
		public static Mesh CylinderSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 4 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 4;
				int num4 = numSegments * 4 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					array[num2 + i * 2] = vector + vector3;
					array[num2 + i * 2 + 1] = vector2 + vector3;
					array2[num2 + i * 2] = vector3;
					array2[num2 + i * 2 + 1] = vector3;
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 2) % (numSegments * 2);
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 1) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006443 RID: 25667 RVA: 0x001FA33C File Offset: 0x001F853C
		public static Mesh SphereWireframe(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereWireframeMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments * 2 - 1) * 2];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num9 = array5[k];
					float num10 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num11 = array3[l];
						float y = array4[l];
						array[num7] = new Vector3(num10 * num11, y, num9 * num11);
						if (l == 0)
						{
							array2[num8++] = num;
							array2[num8++] = num7;
						}
						else
						{
							array2[num8++] = num7 - 1;
							array2[num8++] = num7;
						}
						array2[num8++] = num7;
						array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
						if (l == latSegments - 2)
						{
							array2[num8++] = num7;
							array2[num8++] = num2;
						}
						num7++;
					}
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_sphereWireframeMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereWireframeMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereWireframeMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006444 RID: 25668 RVA: 0x001FA5B4 File Offset: 0x001F87B4
		public static Mesh SphereSolidColor(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSolidColorMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments - 1) * 2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num9 = array5[k];
					float num10 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num11 = array3[l];
						float y = array4[l];
						array[num7] = new Vector3(num10 * num11, y, num9 * num11);
						if (l == 0)
						{
							array2[num8++] = num;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num7;
						}
						if (l < latSegments - 2)
						{
							array2[num8++] = num7 + 1;
							array2[num8++] = num7;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num7 + 1;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = (num7 + latSegments) % (longSegments * (latSegments - 1));
						}
						if (l == latSegments - 2)
						{
							array2[num8++] = num7;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num2;
						}
						num7++;
					}
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x001FA880 File Offset: 0x001F8A80
		public static Mesh SphereFlatShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (latSegments - 2) * 4 + 6;
				int num2 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array6[k];
					float num11 = array7[k];
					float num12 = array6[(k + 1) % longSegments];
					float num13 = array7[(k + 1) % longSegments];
					int num14 = num7;
					array[num7++] = vector;
					array[num7++] = new Vector3(num11 * array4[0], array5[0], num10 * array4[0]);
					array[num7++] = new Vector3(num13 * array4[0], array5[0], num12 * array4[0]);
					int num15 = num7;
					array[num7++] = vector2;
					array[num7++] = new Vector3(num11 * array4[latSegments - 2], array5[latSegments - 2], num10 * array4[latSegments - 2]);
					array[num7++] = new Vector3(num13 * array4[latSegments - 2], array5[latSegments - 2], num12 * array4[latSegments - 2]);
					Vector3 normalized = Vector3.Cross(array[num14 + 2] - array[num14], array[num14 + 1] - array[num14]).normalized;
					array2[num8++] = normalized;
					array2[num8++] = normalized;
					array2[num8++] = normalized;
					Vector3 normalized2 = Vector3.Cross(array[num15 + 1] - array[num15], array[num15 + 2] - array[num15]).normalized;
					array2[num8++] = normalized2;
					array2[num8++] = normalized2;
					array2[num8++] = normalized2;
					array3[num9++] = num14;
					array3[num9++] = num14 + 2;
					array3[num9++] = num14 + 1;
					array3[num9++] = num15;
					array3[num9++] = num15 + 1;
					array3[num9++] = num15 + 2;
					for (int l = 0; l < latSegments - 2; l++)
					{
						float num16 = array4[l];
						float y = array5[l];
						float num17 = array4[l + 1];
						float y2 = array5[l + 1];
						int num18 = num7;
						array[num7++] = new Vector3(num11 * num16, y, num10 * num16);
						array[num7++] = new Vector3(num13 * num16, y, num12 * num16);
						array[num7++] = new Vector3(num13 * num17, y2, num12 * num17);
						array[num7++] = new Vector3(num11 * num17, y2, num10 * num17);
						Vector3 normalized3 = Vector3.Cross(array[num18 + 1] - array[num18], array[num18 + 2] - array[num18]).normalized;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array3[num9++] = num18;
						array3[num9++] = num18 + 1;
						array3[num9++] = num18 + 2;
						array3[num9++] = num18;
						array3[num9++] = num18 + 2;
						array3[num9++] = num18 + 3;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x001FADD8 File Offset: 0x001F8FD8
		public static Mesh SphereSmoothShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = latSegments - 1;
				int num2 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num3 = longSegments * num;
				int num4 = num3 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, 1f, 0f);
				array2[num4] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num5 = 3.1415927f / (float)latSegments;
				float num6 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num6 += num5;
					array4[i] = Mathf.Sin(num6);
					array5[i] = Mathf.Cos(num6);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num7 = 6.2831855f / (float)longSegments;
				float num8 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num8 += num7;
					array6[j] = Mathf.Sin(num8);
					array7[j] = Mathf.Cos(num8);
				}
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num12 = array6[k];
					float num13 = array7[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num14 = array4[l];
						float y = array5[l];
						Vector3 vector3 = new Vector3(num13 * num14, y, num12 * num14);
						array[num9++] = vector3;
						array2[num10++] = vector3;
						int num15 = num9 - 1;
						int num16 = num15 + 1;
						int num17 = (num15 + num) % (longSegments * num);
						int num18 = (num15 + num + 1) % (longSegments * num);
						if (latSegments == 2)
						{
							array3[num11++] = num3;
							array3[num11++] = num17;
							array3[num11++] = num15;
							array3[num11++] = num4;
							array3[num11++] = num16;
							array3[num11++] = num18;
						}
						else if (l < latSegments - 2)
						{
							if (l == 0)
							{
								array3[num11++] = num3;
								array3[num11++] = num17;
								array3[num11++] = num15;
							}
							else if (l == latSegments - 3)
							{
								array3[num11++] = num4;
								array3[num11++] = num16;
								array3[num11++] = num18;
							}
							array3[num11++] = num15;
							array3[num11++] = num18;
							array3[num11++] = num16;
							array3[num11++] = num15;
							array3[num11++] = num17;
							array3[num11++] = num18;
						}
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x001FB160 File Offset: 0x001F9360
		public static Mesh CapsuleWireframe(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleWireframeMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4 + 1) * 2];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num8 = array5[k];
					float num9 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num10 = array3[l];
						float num11 = array4[l];
						array[num7] = new Vector3(num9 * num10, num11 + 0.5f, num8 * num10);
						array[num7 + 1] = new Vector3(num9 * num10, -num11 - 0.5f, num8 * num10);
						if (caps)
						{
							if (l == 0)
							{
								array2[newSize++] = num;
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num2;
									array2[newSize++] = num7 + 1;
								}
							}
							else
							{
								array2[newSize++] = num7 - 2;
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num7 - 1;
									array2[newSize++] = num7 + 1;
								}
							}
						}
						if (caps || l == latSegmentsPerCap - 1)
						{
							array2[newSize++] = num7;
							array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							if (!topCapOnly)
							{
								array2[newSize++] = num7 + 1;
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						if (sides && l == latSegmentsPerCap - 1)
						{
							array2[newSize++] = num7;
							array2[newSize++] = num7 + 1;
						}
						num7 += 2;
					}
				}
				Array.Resize<int>(ref array2, newSize);
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006448 RID: 25672 RVA: 0x001FB4A8 File Offset: 0x001F96A8
		public static Mesh CapsuleSolidColor(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num8 = array5[k];
					float num9 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num10 = array3[l];
						float num11 = array4[l];
						array[num7] = new Vector3(num9 * num10, num11 + 0.5f, num8 * num10);
						array[num7 + 1] = new Vector3(num9 * num10, -num11 - 0.5f, num8 * num10);
						if (l == 0)
						{
							if (caps)
							{
								array2[newSize++] = num;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num2;
									array2[newSize++] = num7 + 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
						}
						else
						{
							if (caps)
							{
								array2[newSize++] = num7 - 2;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7;
								array2[newSize++] = num7 - 2;
								array2[newSize++] = (num7 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array2[newSize++] = num7 - 1;
									array2[newSize++] = num7 + 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[newSize++] = num7 - 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[newSize++] = (num7 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array2[newSize++] = num7;
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7 + 1;
								array2[newSize++] = num7;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num7 += 2;
					}
				}
				Array.Resize<int>(ref array2, newSize);
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x06006449 RID: 25673 RVA: 0x001FB8C0 File Offset: 0x001F9AC0
		public static Mesh CapsuleFlatShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * (latSegmentsPerCap - 1) * 8 + longSegmentsPerCap * 10];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				array2[num] = new Vector3(0f, 1f, 0f);
				array2[num2] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num9 = array6[k];
					float num10 = array7[k];
					float num11 = array6[(k + 1) % longSegmentsPerCap];
					float num12 = array7[(k + 1) % longSegmentsPerCap];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num13 = array4[l];
						float num14 = array5[l];
						if (caps && l < latSegmentsPerCap - 1)
						{
							if (l == 0)
							{
								int num15 = num7;
								array[num7++] = vector;
								array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
								array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
								Vector3 vector3 = Vector3.Cross(array[num15 + 2] - array[num15], array[num15 + 1] - array[num15]);
								array2[num8++] = vector3;
								array2[num8++] = vector3;
								array2[num8++] = vector3;
								array3[newSize++] = num15;
								array3[newSize++] = num15 + 2;
								array3[newSize++] = num15 + 1;
								if (!topCapOnly)
								{
									int num16 = num7;
									array[num7++] = vector2;
									array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
									array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
									Vector3 normalized = Vector3.Cross(array[num16 + 1] - array[num16], array[num16 + 2] - array[num16]).normalized;
									array2[num8++] = normalized;
									array2[num8++] = normalized;
									array2[num8++] = normalized;
									array3[newSize++] = num16;
									array3[newSize++] = num16 + 1;
									array3[newSize++] = num16 + 2;
								}
							}
							float num17 = array4[l + 1];
							float num18 = array5[l + 1];
							if (caps)
							{
								int num19 = num7;
								array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
								array[num7++] = new Vector3(num10 * num17, num18 + 0.5f, num9 * num17);
								array[num7++] = new Vector3(num12 * num17, num18 + 0.5f, num11 * num17);
								array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
								Vector3 vector4 = Vector3.Cross(array[num19 + 3] - array[num19], array[num19 + 1] - array[num19]);
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array3[newSize++] = num19;
								array3[newSize++] = num19 + 2;
								array3[newSize++] = num19 + 1;
								array3[newSize++] = num19;
								array3[newSize++] = num19 + 3;
								array3[newSize++] = num19 + 2;
								if (!topCapOnly)
								{
									int num20 = num7;
									array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
									array[num7++] = new Vector3(num10 * num17, -num18 - 0.5f, num9 * num17);
									array[num7++] = new Vector3(num12 * num17, -num18 - 0.5f, num11 * num17);
									array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
									Vector3 vector5 = Vector3.Cross(array[num20 + 1] - array[num20], array[num20 + 3] - array[num20]);
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array3[newSize++] = num20;
									array3[newSize++] = num20 + 1;
									array3[newSize++] = num20 + 2;
									array3[newSize++] = num20;
									array3[newSize++] = num20 + 2;
									array3[newSize++] = num20 + 3;
								}
							}
						}
						else if (sides && l == latSegmentsPerCap - 1)
						{
							int num21 = num7;
							array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
							array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
							array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
							array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
							Vector3 normalized2 = Vector3.Cross(array[num21 + 3] - array[num21], array[num21 + 1] - array[num21]).normalized;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array3[newSize++] = num21;
							array3[newSize++] = num21 + 2;
							array3[newSize++] = num21 + 1;
							array3[newSize++] = num21;
							array3[newSize++] = num21 + 3;
							array3[newSize++] = num21 + 2;
						}
					}
				}
				Array.Resize<int>(ref array3, newSize);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x001FC170 File Offset: 0x001FA370
		public static Mesh CapsuleSmoothShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				array2[num] = new Vector3(0f, 1f, 0f);
				array2[num2] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num9 = array6[k];
					float num10 = array7[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num11 = array4[l];
						float num12 = array5[l];
						array[num7] = new Vector3(num10 * num11, num12 + 0.5f, num9 * num11);
						array[num7 + 1] = new Vector3(num10 * num11, -num12 - 0.5f, num9 * num11);
						array2[num8] = new Vector3(num10 * num11, num12, num9 * num11);
						array2[num8 + 1] = new Vector3(num10 * num11, -num12, num9 * num11);
						if (caps && l == 0)
						{
							array3[newSize++] = num;
							array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							array3[newSize++] = num7;
							if (!topCapOnly)
							{
								array3[newSize++] = num2;
								array3[newSize++] = num7 + 1;
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						else
						{
							if (caps)
							{
								array3[newSize++] = num7 - 2;
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = num7;
								array3[newSize++] = num7 - 2;
								array3[newSize++] = (num7 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array3[newSize++] = num7 - 1;
									array3[newSize++] = num7 + 1;
									array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[newSize++] = num7 - 1;
									array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[newSize++] = (num7 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array3[newSize++] = num7;
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = num7 + 1;
								array3[newSize++] = num7;
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num7 += 2;
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array3, newSize);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x001FC624 File Offset: 0x001FA824
		public static Mesh Capsule2DWireframe(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 4];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 0; k < array.Length - 1; k++)
				{
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x001FC7D4 File Offset: 0x001FA9D4
		public static Mesh Capsule2DSolidColor(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 12];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 1; k < array.Length; k++)
				{
					array2[num2++] = 0;
					array2[num2++] = (k + 1) % array.Length;
					array2[num2++] = k;
					array2[num2++] = 0;
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x001FC9AC File Offset: 0x001FABAC
		public static Mesh Capsule2DFlatShaded(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (capSegments + 1) * 2;
				Vector3[] array = new Vector3[num * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[num * 6];
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				float num5 = 3.1415927f / (float)capSegments;
				float num6 = 0f;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < capSegments; j++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
					for (int k = 0; k < capSegments; k++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
					Vector3 vector = new Vector3(0f, 0f, (i == 0) ? -1f : 1f);
					for (int l = 0; l < num; l++)
					{
						array2[num3++] = vector;
					}
				}
				for (int m = 1; m < num; m++)
				{
					array3[num4++] = 0;
					array3[num4++] = (m + 1) % num;
					array3[num4++] = m;
					array3[num4++] = num;
					array3[num4++] = num + m;
					array3[num4++] = num + (m + 1) % num;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x001FCC00 File Offset: 0x001FAE00
		public static Mesh ConeWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 4];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_coneWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x001FCD3C File Offset: 0x001FAF3C
		public static Mesh ConeSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 3 + (numSegments - 2) * 3];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					if (i >= 2)
					{
						array2[num++] = 0;
						array2[num++] = i - 1;
						array2[num++] = i;
					}
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x001FCE98 File Offset: 0x001FB098
		public static Mesh ConeFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 3 + numSegments];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3[] array4 = new Vector3[numSegments];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array4[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					num2 += num;
				}
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int j = 0; j < numSegments; j++)
				{
					int num6 = num3;
					array[num3++] = vector;
					array[num3++] = array4[j];
					array[num3++] = array4[(j + 1) % numSegments];
					Vector3 normalized = Vector3.Cross(array[num6 + 2] - array[num6], array[num6 + 1] - array[num6]).normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array3[num4++] = num6;
					array3[num4++] = num6 + 2;
					array3[num4++] = num6 + 1;
				}
				int num7 = num3;
				for (int k = 0; k < numSegments; k++)
				{
					array[num3++] = array4[k];
					array2[num5++] = new Vector3(0f, -1f, 0f);
					if (k >= 2)
					{
						array3[num4++] = num7;
						array3[num4++] = num7 + k - 1;
						array3[num4++] = num7 + k;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x001FD120 File Offset: 0x001FB320
		public static Mesh ConeSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 1];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				int num = array.Length - 1;
				array[num] = new Vector3(0f, 1f, 0f);
				array2[num] = new Vector3(0f, 0f, 0f);
				float num2 = Mathf.Sqrt(0.5f);
				int num3 = 0;
				float num4 = 6.2831855f / (float)numSegments;
				float num5 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					float num6 = Mathf.Cos(num5);
					float num7 = Mathf.Sin(num5);
					Vector3 vector = num6 * Vector3.right + num7 * Vector3.forward;
					array[i] = vector;
					array[numSegments + i] = vector;
					array2[i] = new Vector3(num6 * num2, num2, num7 * num2);
					array2[numSegments + i] = new Vector3(0f, -1f, 0f);
					array3[num3++] = num;
					array3[num3++] = (i + 1) % numSegments;
					array3[num3++] = i;
					if (i >= 2)
					{
						array3[num3++] = numSegments;
						array3[num3++] = numSegments + i - 1;
						array3[num3++] = numSegments + i;
					}
					num5 += num4;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x04006EEE RID: 28398
		private static int s_lastDrawLineFrame = -1;

		// Token: 0x04006EEF RID: 28399
		private static int s_iPooledMesh = 0;

		// Token: 0x04006EF0 RID: 28400
		private static List<Mesh> s_lineMeshPool;

		// Token: 0x04006EF1 RID: 28401
		private static Mesh s_boxWireframeMesh;

		// Token: 0x04006EF2 RID: 28402
		private static Mesh s_boxSolidColorMesh;

		// Token: 0x04006EF3 RID: 28403
		private static Mesh s_boxFlatShadedMesh;

		// Token: 0x04006EF4 RID: 28404
		private static Mesh s_rectWireframeMesh;

		// Token: 0x04006EF5 RID: 28405
		private static Mesh s_rectSolidColorMesh;

		// Token: 0x04006EF6 RID: 28406
		private static Mesh s_rectFlatShadedMesh;

		// Token: 0x04006EF7 RID: 28407
		private static Dictionary<int, Mesh> s_circleWireframeMeshPool;

		// Token: 0x04006EF8 RID: 28408
		private static Dictionary<int, Mesh> s_circleSolidColorMeshPool;

		// Token: 0x04006EF9 RID: 28409
		private static Dictionary<int, Mesh> s_circleFlatShadedMeshPool;

		// Token: 0x04006EFA RID: 28410
		private static Dictionary<int, Mesh> s_cylinderWireframeMeshPool;

		// Token: 0x04006EFB RID: 28411
		private static Dictionary<int, Mesh> s_cylinderSolidColorMeshPool;

		// Token: 0x04006EFC RID: 28412
		private static Dictionary<int, Mesh> s_cylinderFlatShadedMeshPool;

		// Token: 0x04006EFD RID: 28413
		private static Dictionary<int, Mesh> s_cylinderSmoothShadedMeshPool;

		// Token: 0x04006EFE RID: 28414
		private static Dictionary<int, Mesh> s_sphereWireframeMeshPool;

		// Token: 0x04006EFF RID: 28415
		private static Dictionary<int, Mesh> s_sphereSolidColorMeshPool;

		// Token: 0x04006F00 RID: 28416
		private static Dictionary<int, Mesh> s_sphereFlatShadedMeshPool;

		// Token: 0x04006F01 RID: 28417
		private static Dictionary<int, Mesh> s_sphereSmoothShadedMeshPool;

		// Token: 0x04006F02 RID: 28418
		private static Dictionary<int, Mesh> s_capsuleWireframeMeshPool;

		// Token: 0x04006F03 RID: 28419
		private static Dictionary<int, Mesh> s_capsuleSolidColorMeshPool;

		// Token: 0x04006F04 RID: 28420
		private static Dictionary<int, Mesh> s_capsuleFlatShadedMeshPool;

		// Token: 0x04006F05 RID: 28421
		private static Dictionary<int, Mesh> s_capsuleSmoothShadedMeshPool;

		// Token: 0x04006F06 RID: 28422
		private static Dictionary<int, Mesh> s_capsule2dWireframeMeshPool;

		// Token: 0x04006F07 RID: 28423
		private static Dictionary<int, Mesh> s_capsule2dSolidColorMeshPool;

		// Token: 0x04006F08 RID: 28424
		private static Dictionary<int, Mesh> s_capsule2dFlatShadedMeshPool;

		// Token: 0x04006F09 RID: 28425
		private static Dictionary<int, Mesh> s_coneWireframeMeshPool;

		// Token: 0x04006F0A RID: 28426
		private static Dictionary<int, Mesh> s_coneSolidColorMeshPool;

		// Token: 0x04006F0B RID: 28427
		private static Dictionary<int, Mesh> s_coneFlatShadedMeshPool;

		// Token: 0x04006F0C RID: 28428
		private static Dictionary<int, Mesh> s_coneSmoothhadedMeshPool;
	}
}
