using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class PropHuntPropZone : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x0600097A RID: 2426 RVA: 0x00034214 File Offset: 0x00032414
	private void Awake()
	{
		this.hasBoxCollider = base.TryGetComponent<BoxCollider>(out this.boxCollider);
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00034233 File Offset: 0x00032433
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropZone(this);
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0003423B File Offset: 0x0003243B
	private void OnDisable()
	{
		this.DestroyDecoys();
		GorillaPropHuntGameManager.UnregisterPropZone(this);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x0003424C File Offset: 0x0003244C
	public void DestroyDecoys()
	{
		foreach (PropPlacementRB propPlacementRB in this.propPlacementRBs)
		{
			if (propPlacementRB != null)
			{
				PropHuntPools.ReturnDecoyProp(propPlacementRB);
			}
		}
		this.propPlacementRBs.Clear();
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x000342B4 File Offset: 0x000324B4
	public void OnRoundStart()
	{
		if (!PropHuntPools.IsReady)
		{
			Debug.LogError("ERROR!!!  PropHuntPropZone: (this should never happen) props not ready to be spawned so aborting. you should only be calling this if `PropHuntPools.IsReady` is true or from the callback `PropHuntPools.OnReady`.");
		}
		this.CreateDecoys(GorillaPropHuntGameManager.instance.GetSeed());
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000342D8 File Offset: 0x000324D8
	public void CreateDecoys(int seed)
	{
		this.DestroyDecoys();
		SRand srand = new SRand(seed + this.seedOffset);
		for (int i = 0; i < this.numProps; i++)
		{
			PropPlacementRB propPlacementRB;
			if (!PropHuntPools.TryGetDecoyProp(GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt()), out propPlacementRB))
			{
				return;
			}
			Vector3 position2;
			if (this.hasBoxCollider)
			{
				Vector3 position = new Vector3(srand.NextFloat(-this.boxCollider.size.x, this.boxCollider.size.x) / 2f, srand.NextFloat(-this.boxCollider.size.y, this.boxCollider.size.y) / 2f, srand.NextFloat(-this.boxCollider.size.z, this.boxCollider.size.z) / 2f);
				position2 = base.transform.TransformPoint(position);
			}
			else
			{
				position2 = base.transform.position + srand.NextPointInsideSphere(this.radius);
			}
			propPlacementRB.gameObject.SetActive(false);
			propPlacementRB.transform.SetParent(null, false);
			propPlacementRB.transform.position = position2;
			propPlacementRB.transform.rotation = Quaternion.Euler(srand.NextFloat(360f), srand.NextFloat(360f), srand.NextFloat(360f));
			propPlacementRB._placingProp.SetActive(false);
			propPlacementRB._placingProp.transform.SetParent(null, false);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		for (int j = 0; j < this.propPlacementRBs.Count; j++)
		{
			this.propPlacementRBs[j].gameObject.SetActive(true);
		}
		GTDelayedExec.Add(this, this.m_simDurationBeforeFreeze, 0);
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x000344BC File Offset: 0x000326BC
	public void OnDelayedAction(int contextId)
	{
		for (int i = 0; i < this.propPlacementRBs.Count; i++)
		{
			PropPlacementRB propPlacementRB = this.propPlacementRBs[i];
			propPlacementRB.gameObject.SetActive(false);
			Transform transform = propPlacementRB.transform;
			GameObject placingProp = propPlacementRB._placingProp;
			placingProp.transform.SetPositionAndRotation(transform.position, transform.rotation);
			placingProp.SetActive(true);
		}
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00034520 File Offset: 0x00032720
	private PropPlacementRB _GetOrCreatePropPlacementObj_NoPool()
	{
		PropPlacementRB propPlacementRB;
		if (this.nextUnusedPropPlacement < this.propPlacementRBs.Count)
		{
			propPlacementRB = this.propPlacementRBs[this.nextUnusedPropPlacement];
		}
		else
		{
			propPlacementRB = Object.Instantiate<PropPlacementRB>(this.propPlacementPrefab, base.transform);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		this.nextUnusedPropPlacement++;
		return propPlacementRB;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00034581 File Offset: 0x00032781
	private void SpawnProp_NoPool(GTAssetRef<GameObject> item, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		this._GetOrCreatePropPlacementObj_NoPool().PlaceProp_NoPool(this, item, pos, rot, debugCosmeticSO);
	}

	// Token: 0x04000B39 RID: 2873
	private const string preLog = "PropHuntPropZone: ";

	// Token: 0x04000B3A RID: 2874
	private const string preLogEd = "(editor only log) PropHuntPropZone: ";

	// Token: 0x04000B3B RID: 2875
	private const string preLogBeta = "(beta only log) PropHuntPropZone: ";

	// Token: 0x04000B3C RID: 2876
	private const string preErr = "ERROR!!!  PropHuntPropZone: ";

	// Token: 0x04000B3D RID: 2877
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPropZone: ";

	// Token: 0x04000B3E RID: 2878
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPropZone: ";

	// Token: 0x04000B3F RID: 2879
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x04000B40 RID: 2880
	[SerializeField]
	private PropPlacementRB propPlacementPrefab;

	// Token: 0x04000B41 RID: 2881
	[SerializeField]
	private int seedOffset;

	// Token: 0x04000B42 RID: 2882
	[SerializeField]
	private float radius = 1f;

	// Token: 0x04000B43 RID: 2883
	[SerializeField]
	private int numProps = 10;

	// Token: 0x04000B44 RID: 2884
	[SerializeField]
	private float m_simDurationBeforeFreeze = 2f;

	// Token: 0x04000B45 RID: 2885
	private BoxCollider boxCollider;

	// Token: 0x04000B46 RID: 2886
	private bool hasBoxCollider;

	// Token: 0x04000B47 RID: 2887
	private int nextUnusedPropPlacement;

	// Token: 0x04000B48 RID: 2888
	private readonly List<PropPlacementRB> propPlacementRBs = new List<PropPlacementRB>(64);
}
