using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D9 RID: 1497
public class GhostReactorLevelSectionConnector : MonoBehaviour
{
	// Token: 0x060024A3 RID: 9379 RVA: 0x000C5248 File Offset: 0x000C3448
	private void Awake()
	{
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
		{
			this.prePlacedGameEntities[i].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(this.renderers);
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000C52F0 File Offset: 0x000C34F0
	public void Init(GhostReactorManager grManager)
	{
		if (grManager.IsAuthority())
		{
			if (this.gateEntity != null)
			{
				grManager.gameEntityManager.RequestCreateItem(this.gateEntity.name.GetStaticHash(), this.gateSpawnPoint.position, this.gateSpawnPoint.rotation, 0L);
			}
			for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
			{
				int staticHash = this.prePlacedGameEntities[i].gameObject.name.GetStaticHash();
				if (!grManager.gameEntityManager.FactoryHasEntity(staticHash))
				{
					Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
					{
						this.prePlacedGameEntities[i].gameObject.name,
						staticHash
					});
				}
				else
				{
					GameEntityCreateData item = new GameEntityCreateData
					{
						entityTypeId = staticHash,
						localPosition = this.prePlacedGameEntities[i].transform.position,
						localRotation = this.prePlacedGameEntities[i].transform.rotation,
						createData = 0L
					};
					GhostReactorLevelSection.tempCreateEntitiesList.Add(item);
				}
			}
			grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000023F5 File Offset: 0x000005F5
	public void DeInit()
	{
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x000C5440 File Offset: 0x000C3640
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000C548C File Offset: 0x000C368C
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float sqrMagnitude = (this.boundingCollider.ClosestPoint(playerPos) - playerPos).sqrMagnitude;
		float num = 324f;
		float num2 = 484f;
		if (this.hidden && sqrMagnitude < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && sqrMagnitude > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x04002E6A RID: 11882
	public Transform hubAnchor;

	// Token: 0x04002E6B RID: 11883
	public Transform sectionAnchor;

	// Token: 0x04002E6C RID: 11884
	public Transform gateSpawnPoint;

	// Token: 0x04002E6D RID: 11885
	public GameEntity gateEntity;

	// Token: 0x04002E6E RID: 11886
	public GhostReactorLevelSectionConnector.Direction direction;

	// Token: 0x04002E6F RID: 11887
	public BoxCollider boundingCollider;

	// Token: 0x04002E70 RID: 11888
	public List<Transform> pathNodes;

	// Token: 0x04002E71 RID: 11889
	private const float SHOW_DIST = 18f;

	// Token: 0x04002E72 RID: 11890
	private const float HIDE_DIST = 22f;

	// Token: 0x04002E73 RID: 11891
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x04002E74 RID: 11892
	private List<Renderer> renderers;

	// Token: 0x04002E75 RID: 11893
	private bool hidden;

	// Token: 0x020005DA RID: 1498
	public enum Direction
	{
		// Token: 0x04002E77 RID: 11895
		Down = -1,
		// Token: 0x04002E78 RID: 11896
		Forward,
		// Token: 0x04002E79 RID: 11897
		Up
	}
}
