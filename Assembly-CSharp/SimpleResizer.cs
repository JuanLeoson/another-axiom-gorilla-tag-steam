using System;
using UnityEngine;

// Token: 0x020003AC RID: 940
public class SimpleResizer
{
	// Token: 0x060015C9 RID: 5577 RVA: 0x00076FE8 File Offset: 0x000751E8
	public void CreateResizedObject(Vector3 newSize, GameObject parent, SimpleResizable sourcePrefab)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.name = sourcePrefab.name;
		SimpleResizable component = gameObject.GetComponent<SimpleResizable>();
		component.SetNewSize(newSize);
		if (component == null)
		{
			Debug.LogError("Resizable component missing.");
			return;
		}
		Mesh sharedMesh = SimpleResizer.ProcessVertices(component, newSize, false);
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		component2.sharedMesh = sharedMesh;
		component2.sharedMesh.RecalculateBounds();
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		Object.Destroy(component);
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x00077090 File Offset: 0x00075290
	internal static Mesh ProcessVertices(SimpleResizable resizable, Vector3 newSize, bool pivot = false)
	{
		Mesh originalMesh = resizable.OriginalMesh;
		Vector3 defaultSize = resizable.DefaultSize;
		SimpleResizable.Method resizeMethod = (defaultSize.x < newSize.x) ? resizable.ScalingX : SimpleResizable.Method.Scale;
		SimpleResizable.Method resizeMethod2 = (defaultSize.y < newSize.y) ? resizable.ScalingY : SimpleResizable.Method.Scale;
		SimpleResizable.Method resizeMethod3 = (defaultSize.z < newSize.z) ? resizable.ScalingZ : SimpleResizable.Method.Scale;
		Vector3[] vertices = originalMesh.vertices;
		Vector3 vector = resizable.transform.InverseTransformPoint(resizable.PivotPosition);
		float pivot2 = 1f / resizable.DefaultSize.x * vector.x;
		float pivot3 = 1f / resizable.DefaultSize.y * vector.y;
		float pivot4 = 1f / resizable.DefaultSize.z * vector.z;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector2 = vertices[i];
			vector2.x = SimpleResizer.CalculateNewVertexPosition(resizeMethod, vector2.x, defaultSize.x, newSize.x, resizable.PaddingX, resizable.PaddingXMax, pivot2);
			vector2.y = SimpleResizer.CalculateNewVertexPosition(resizeMethod2, vector2.y, defaultSize.y, newSize.y, resizable.PaddingY, resizable.PaddingYMax, pivot3);
			vector2.z = SimpleResizer.CalculateNewVertexPosition(resizeMethod3, vector2.z, defaultSize.z, newSize.z, resizable.PaddingZ, resizable.PaddingZMax, pivot4);
			if (pivot)
			{
				vector2 += vector;
			}
			vertices[i] = vector2;
		}
		Mesh mesh = Object.Instantiate<Mesh>(originalMesh);
		mesh.vertices = vertices;
		return mesh;
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x00077238 File Offset: 0x00075438
	private static float CalculateNewVertexPosition(SimpleResizable.Method resizeMethod, float currentPosition, float currentSize, float newSize, float padding, float paddingMax, float pivot)
	{
		float num = currentSize / 2f * (newSize / 2f * (1f / (currentSize / 2f))) - currentSize / 2f;
		switch (resizeMethod)
		{
		case SimpleResizable.Method.Adapt:
			if (Mathf.Abs(currentPosition) >= padding)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			if (currentPosition >= padding)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			if (currentPosition <= paddingMax)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			break;
		case SimpleResizable.Method.Scale:
			currentPosition = newSize / (currentSize / currentPosition);
			break;
		}
		float num2 = newSize * -pivot;
		currentPosition += num2;
		return currentPosition;
	}
}
