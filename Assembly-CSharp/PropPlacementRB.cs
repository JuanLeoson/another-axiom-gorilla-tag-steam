using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

// Token: 0x02000165 RID: 357
public class PropPlacementRB : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06000985 RID: 2437 RVA: 0x000345C7 File Offset: 0x000327C7
	protected void OnDestroy()
	{
		if (this._placingProp != null)
		{
			Object.Destroy(this._placingProp);
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x000345E4 File Offset: 0x000327E4
	public void PlaceProp_NoPool(PropHuntPropZone parentZone, GTAssetRef<GameObject> propRef, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		if (this._isInstantiatingAsync)
		{
			Debug.LogError("ERROR!!!  PropPlacementRB: Tried to place (spawn) prop while one was already being placed.");
			return;
		}
		this._parentZone = parentZone;
		MeshCollider[] colliders = this._colliders;
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].gameObject.SetActive(false);
		}
		base.transform.position = pos;
		base.transform.rotation = rot;
		base.gameObject.SetActive(false);
		this._isInstantiatingAsync = true;
		propRef.InstantiateAsync(null, false).Completed += this.OnPropLoaded_NoPool;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00034678 File Offset: 0x00032878
	public void OnPropLoaded_NoPool(AsyncOperationHandle<GameObject> handle)
	{
		this._isInstantiatingAsync = false;
		this._placingProp = handle.Result;
		this._placingProp.transform.position = base.transform.position;
		this._placingProp.transform.rotation = base.transform.rotation;
		this.m_rb.velocity = Vector3.zero;
		this.m_rb.angularVelocity = Vector3.zero;
		CosmeticSO debugCosmeticSO = null;
		if (!PropPlacementRB.TryPrepPropTemplate(this, this._placingProp, debugCosmeticSO))
		{
			this.DestroyProp_NoPool();
			return;
		}
		this._placingProp.SetActive(false);
		base.gameObject.SetActive(true);
		GTDelayedExec.Add(this, 2f, 0);
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x0003472C File Offset: 0x0003292C
	public static bool TryPrepPropTemplate(PropPlacementRB rb, GameObject rendererGobj, CosmeticSO _debugCosmeticSO)
	{
		rb._isInstantiatingAsync = false;
		rb._placingProp = rendererGobj;
		rb._placingProp.transform.position = rb.transform.position;
		rb._placingProp.transform.rotation = rb.transform.rotation;
		rb.m_rb.velocity = Vector3.zero;
		rb.m_rb.angularVelocity = Vector3.zero;
		bool flag = false;
		MeshFilter[] componentsInChildren = rendererGobj.GetComponentsInChildren<MeshFilter>(true);
		List<MeshCollider> list;
		bool result;
		using (ListPool<MeshCollider>.Get(out list))
		{
			list.Capacity = math.max(list.Capacity, 8);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					MeshCollider meshCollider = new GameObject(meshFilter.name + "__PropHuntDecoy_Collider")
					{
						transform = 
						{
							parent = rb.transform
						},
						layer = 30
					}.AddComponent<MeshCollider>();
					meshCollider.convex = true;
					meshCollider.transform.position = meshFilter.transform.position;
					meshCollider.transform.rotation = meshFilter.transform.rotation;
					meshCollider.sharedMesh = meshFilter.sharedMesh;
					list.Add(meshCollider);
				}
			}
			rb._colliders = list.ToArray();
			if (!flag)
			{
				result = false;
			}
			else
			{
				Transform[] componentsInChildren2 = rendererGobj.GetComponentsInChildren<Transform>(true);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].gameObject.isStatic = true;
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x000348F8 File Offset: 0x00032AF8
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		this.OnPropFell();
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00034900 File Offset: 0x00032B00
	private void OnPropFell()
	{
		if (this._placingProp == null)
		{
			return;
		}
		this._placingProp.transform.position = base.transform.position;
		this._placingProp.transform.rotation = base.transform.rotation;
		this._placingProp.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00034964 File Offset: 0x00032B64
	public void DestroyProp_NoPool()
	{
		if (this._placingProp != null)
		{
			Object.Destroy(this._placingProp);
			this._placingProp = null;
		}
	}

	// Token: 0x04000B4B RID: 2891
	[FormerlySerializedAs("rb")]
	[SerializeField]
	private Rigidbody m_rb;

	// Token: 0x04000B4C RID: 2892
	[FormerlySerializedAs("simDurationBeforeFreeze")]
	[SerializeField]
	private float m_simDurationBeforeFreeze;

	// Token: 0x04000B4D RID: 2893
	private PropHuntPropZone _parentZone;

	// Token: 0x04000B4E RID: 2894
	[SerializeField]
	internal GameObject _placingProp;

	// Token: 0x04000B4F RID: 2895
	[SerializeField]
	private MeshCollider[] _colliders;

	// Token: 0x04000B50 RID: 2896
	private bool _isInstantiatingAsync;
}
