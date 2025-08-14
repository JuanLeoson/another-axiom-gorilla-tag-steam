using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class CustomObjectProvider : NetworkObjectProviderDefault
{
	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0005C304 File Offset: 0x0005A504
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker result;
			if ((result = CustomObjectProvider.baker) == null)
			{
				result = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return result;
		}
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0005C31A File Offset: 0x0005A51A
	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, context, out instance);
		if (networkObjectAcquireResult == NetworkObjectAcquireResult.Success)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x0005C334 File Offset: 0x0005A534
	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			GorillaGameModes.GameMode.GetGameModeInstance(GorillaGameModes.GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0005C36A File Offset: 0x0005A56A
	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0005C391 File Offset: 0x0005A591
	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	// Token: 0x0400186B RID: 6251
	public const int GameModeFlag = 1;

	// Token: 0x0400186C RID: 6252
	public const int PlayerFlag = 2;

	// Token: 0x0400186D RID: 6253
	private static NetworkObjectBaker baker;

	// Token: 0x0400186E RID: 6254
	internal List<GameObject> SceneObjects;
}
