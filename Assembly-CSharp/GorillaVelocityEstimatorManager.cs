using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class GorillaVelocityEstimatorManager : MonoBehaviour
{
	// Token: 0x060005C7 RID: 1479 RVA: 0x00021695 File Offset: 0x0001F895
	protected void Awake()
	{
		if (GorillaVelocityEstimatorManager.hasInstance && GorillaVelocityEstimatorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(this);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000216B8 File Offset: 0x0001F8B8
	protected void OnDestroy()
	{
		if (GorillaVelocityEstimatorManager.instance == this)
		{
			GorillaVelocityEstimatorManager.hasInstance = false;
			GorillaVelocityEstimatorManager.instance = null;
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x000216D4 File Offset: 0x0001F8D4
	protected void LateUpdate()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < GorillaVelocityEstimatorManager.estimators.Count; i++)
		{
			if (GorillaVelocityEstimatorManager.estimators[i] != null)
			{
				GorillaVelocityEstimatorManager.estimators[i].TriggeredLateUpdate();
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00021721 File Offset: 0x0001F921
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(new GameObject("GorillaVelocityEstimatorManager").AddComponent<GorillaVelocityEstimatorManager>());
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0002173F File Offset: 0x0001F93F
	private static void SetInstance(GorillaVelocityEstimatorManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.instance = manager;
		GorillaVelocityEstimatorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00021762 File Offset: 0x0001F962
	public static void Register(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (!GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Add(velEstimator);
		}
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00021790 File Offset: 0x0001F990
	public static void Unregister(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Remove(velEstimator);
		}
	}

	// Token: 0x040006ED RID: 1773
	public static GorillaVelocityEstimatorManager instance;

	// Token: 0x040006EE RID: 1774
	public static bool hasInstance = false;

	// Token: 0x040006EF RID: 1775
	public static readonly List<GorillaVelocityEstimator> estimators = new List<GorillaVelocityEstimator>(1024);
}
