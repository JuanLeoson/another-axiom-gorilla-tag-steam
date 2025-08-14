using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class GameModeSpecificObjectRegistry : MonoBehaviour
{
	// Token: 0x060003AF RID: 943 RVA: 0x00016A12 File Offset: 0x00014C12
	private void OnEnable()
	{
		GameModeSpecificObject.OnAwake += this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed += this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode += this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00016A47 File Offset: 0x00014C47
	private void OnDisable()
	{
		GameModeSpecificObject.OnAwake -= this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed -= this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode -= this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00016A7C File Offset: 0x00014C7C
	private void GameModeSpecificObject_OnAwake(GameModeSpecificObject obj)
	{
		foreach (GameModeType key in obj.GameModes)
		{
			if (!this.gameModeSpecificObjects.ContainsKey(key))
			{
				this.gameModeSpecificObjects.Add(key, new List<GameModeSpecificObject>());
			}
			this.gameModeSpecificObjects[key].Add(obj);
		}
		if (GameMode.ActiveGameMode == null)
		{
			obj.gameObject.SetActive(obj.Validation == GameModeSpecificObject.ValidationMethod.Exclusion);
			return;
		}
		obj.gameObject.SetActive(obj.CheckValid(GameMode.ActiveGameMode.GameType()));
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00016B38 File Offset: 0x00014D38
	private void GameModeSpecificObject_OnDestroyed(GameModeSpecificObject obj)
	{
		foreach (GameModeType key in obj.GameModes)
		{
			if (this.gameModeSpecificObjects.ContainsKey(key))
			{
				this.gameModeSpecificObjects[key].Remove(obj);
			}
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00016BA8 File Offset: 0x00014DA8
	private void GameMode_OnStartGameMode(GameModeType newGameModeType)
	{
		if (this.currentGameType == newGameModeType)
		{
			return;
		}
		if (this.gameModeSpecificObjects.ContainsKey(this.currentGameType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject in this.gameModeSpecificObjects[this.currentGameType])
			{
				gameModeSpecificObject.gameObject.SetActive(gameModeSpecificObject.CheckValid(newGameModeType));
			}
		}
		if (this.gameModeSpecificObjects.ContainsKey(newGameModeType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject2 in this.gameModeSpecificObjects[newGameModeType])
			{
				gameModeSpecificObject2.gameObject.SetActive(gameModeSpecificObject2.CheckValid(newGameModeType));
			}
		}
		this.currentGameType = newGameModeType;
	}

	// Token: 0x0400042D RID: 1069
	private Dictionary<GameModeType, List<GameModeSpecificObject>> gameModeSpecificObjects = new Dictionary<GameModeType, List<GameModeSpecificObject>>();

	// Token: 0x0400042E RID: 1070
	private GameModeType currentGameType = GameModeType.Count;
}
