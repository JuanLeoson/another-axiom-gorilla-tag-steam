using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

// Token: 0x02000397 RID: 919
public class SceneManagerHelper
{
	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001573 RID: 5491 RVA: 0x00075330 File Offset: 0x00073530
	public GameObject AnchorGameObject { get; }

	// Token: 0x06001574 RID: 5492 RVA: 0x00075338 File Offset: 0x00073538
	public SceneManagerHelper(GameObject gameObject)
	{
		this.AnchorGameObject = gameObject;
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00075348 File Offset: 0x00073548
	public void SetLocation(OVRLocatable locatable, Camera camera = null)
	{
		OVRLocatable.TrackingSpacePose trackingSpacePose;
		if (!locatable.TryGetSceneAnchorPose(out trackingSpacePose))
		{
			return;
		}
		Camera camera2 = (camera == null) ? Camera.main : camera;
		Vector3? vector = trackingSpacePose.ComputeWorldPosition(camera2);
		Quaternion? quaternion = trackingSpacePose.ComputeWorldRotation(camera2);
		if (vector != null && quaternion != null)
		{
			this.AnchorGameObject.transform.SetPositionAndRotation(vector.Value, quaternion.Value);
		}
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000753B8 File Offset: 0x000735B8
	public void CreatePlane(OVRBounded2D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Plane";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x00075444 File Offset: 0x00073644
	public void UpdatePlane(OVRBounded2D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Plane");
		if (transform == null)
		{
			this.CreatePlane(bounds);
			return;
		}
		transform.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x000754B8 File Offset: 0x000736B8
	public void CreateVolume(OVRBounded3D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Volume";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		gameObject.transform.localScale = bounds.BoundingBox.size;
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x00075558 File Offset: 0x00073758
	public void UpdateVolume(OVRBounded3D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Volume");
		if (transform == null)
		{
			this.CreateVolume(bounds);
			return;
		}
		transform.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		transform.transform.localScale = bounds.BoundingBox.size;
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000755DC File Offset: 0x000737DC
	public void CreateMesh(OVRTriangleMesh mesh)
	{
		int length;
		int num;
		if (!mesh.TryGetCounts(out length, out num))
		{
			return;
		}
		using (NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			using (NativeArray<int> indices = new NativeArray<int>(num * 3, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				if (mesh.TryGetMesh(nativeArray, indices))
				{
					Mesh mesh2 = new Mesh();
					mesh2.indexFormat = IndexFormat.UInt32;
					mesh2.SetVertices<Vector3>(nativeArray);
					mesh2.SetTriangles(indices.ToArray(), 0);
					GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
					gameObject.name = "Mesh";
					gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
					gameObject.GetComponent<MeshFilter>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshCollider>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
				}
			}
		}
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000756D8 File Offset: 0x000738D8
	public void UpdateMesh(OVRTriangleMesh mesh)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Mesh");
		if (transform != null)
		{
			Object.Destroy(transform);
		}
		this.CreateMesh(mesh);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x00075714 File Offset: 0x00073914
	public static Task<bool> RequestSceneCapture()
	{
		SceneManagerHelper.<RequestSceneCapture>d__11 <RequestSceneCapture>d__;
		<RequestSceneCapture>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<RequestSceneCapture>d__.<>1__state = -1;
		<RequestSceneCapture>d__.<>t__builder.Start<SceneManagerHelper.<RequestSceneCapture>d__11>(ref <RequestSceneCapture>d__);
		return <RequestSceneCapture>d__.<>t__builder.Task;
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x0007574F File Offset: 0x0007394F
	public static void RequestScenePermission()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE"))
		{
			Permission.RequestUserPermission("com.oculus.permission.USE_SCENE");
		}
	}

	// Token: 0x04001D3C RID: 7484
	private static bool SceneCaptureRunning;
}
