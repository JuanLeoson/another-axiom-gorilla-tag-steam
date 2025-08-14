using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class MirrorCameraScript : MonoBehaviour
{
	// Token: 0x06000C6B RID: 3179 RVA: 0x00043039 File Offset: 0x00041239
	private void Start()
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		GorillaTagger.Instance.MirrorCameraCullingMask.AddCallback(new Action<int>(this.MirrorCullingMaskChanged), true);
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x00043070 File Offset: 0x00041270
	private void OnDestroy()
	{
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.MirrorCameraCullingMask.RemoveCallback(new Action<int>(this.MirrorCullingMaskChanged));
		}
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0004309A File Offset: 0x0004129A
	private void MirrorCullingMaskChanged(int newMask)
	{
		this.mirrorCamera.cullingMask = newMask;
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x000430A8 File Offset: 0x000412A8
	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 right = base.transform.right;
		Vector4 plane = new Vector4(right.x, right.y, right.z, -Vector3.Dot(right, position));
		Matrix4x4 zero = Matrix4x4.zero;
		this.CalculateReflectionMatrix(ref zero, plane);
		this.mirrorCamera.worldToCameraMatrix = this.mainCamera.worldToCameraMatrix * zero;
		Vector4 clipPlane = this.CameraSpacePlane(this.mirrorCamera, position, right, 1f);
		this.mirrorCamera.projectionMatrix = this.mainCamera.CalculateObliqueMatrix(clipPlane);
		Debug.Log(string.Format("Main Camera position {0}", this.mainCamera.transform.position));
		this.mirrorCamera.transform.position = zero.MultiplyPoint(this.mainCamera.transform.position);
		Debug.Log(string.Format("Reflected Camera position {0}", this.mirrorCamera.transform.position));
		this.mirrorCamera.transform.rotation = this.mainCamera.transform.rotation * Quaternion.Inverse(base.transform.rotation);
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			foreach (Material material in componentsInChildren[i].sharedMaterials)
			{
				if (material.shader == Shader.Find("Reflection"))
				{
					material.SetTexture("_ReflectionTex", this.mirrorCamera.targetTexture);
				}
			}
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0004325C File Offset: 0x0004145C
	private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
	{
		reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		reflectionMatrix.m30 = 0f;
		reflectionMatrix.m31 = 0f;
		reflectionMatrix.m32 = 0f;
		reflectionMatrix.m33 = 1f;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00043404 File Offset: 0x00041604
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * 0.07f;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
	}

	// Token: 0x04000F6E RID: 3950
	public Camera mainCamera;

	// Token: 0x04000F6F RID: 3951
	public Camera mirrorCamera;
}
