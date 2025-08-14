using System;
using System.Collections.Generic;
using System.IO;
using AOT;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000971 RID: 2417
public sealed class CustomGameMode : GorillaGameManager
{
	// Token: 0x06003B19 RID: 15129 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(object obj)
	{
	}

	// Token: 0x06003B1B RID: 15131 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06003B1C RID: 15132 RVA: 0x00058615 File Offset: 0x00056815
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06003B1D RID: 15133 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void AddFusionDataBehaviour(NetworkObject obj)
	{
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x001320EA File Offset: 0x001302EA
	public override GameModeType GameType()
	{
		return GameModeType.Custom;
	}

	// Token: 0x06003B1F RID: 15135 RVA: 0x001320ED File Offset: 0x001302ED
	public override string GameModeName()
	{
		return "CUSTOM";
	}

	// Token: 0x06003B20 RID: 15136 RVA: 0x001320F4 File Offset: 0x001302F4
	public unsafe override int MyMatIndex(NetPlayer forPlayer)
	{
		IntPtr value;
		if (Bindings.LuauPlayerList.TryGetValue(forPlayer.ActorNumber, out value))
		{
			return ((Bindings.LuauPlayer*)((void*)value))->PlayerMaterial;
		}
		return 0;
	}

	// Token: 0x06003B21 RID: 15137 RVA: 0x00132124 File Offset: 0x00130324
	public unsafe override void OnPlayerEnteredRoom(NetPlayer player)
	{
		try
		{
			if (CustomGameMode.gameScriptRunner != null)
			{
				if (CustomGameMode.gameScriptRunner.ShouldTick)
				{
					if (!Bindings.LuauPlayerList.ContainsKey(player.ActorNumber))
					{
						lua_State* l = CustomGameMode.gameScriptRunner.L;
						Luau.lua_getglobal(l, "Players");
						int num = Luau.lua_objlen(l, -1);
						Bindings.LuauPlayer* ptr = Luau.lua_class_push<Bindings.LuauPlayer>(l);
						ptr->PlayerID = player.ActorNumber;
						ptr->PlayerMaterial = 0;
						ptr->IsMasterClient = player.IsMasterClient;
						VRRig vrrig = this.FindPlayerVRRig(player);
						ptr->PlayerName = vrrig.playerNameVisible;
						Bindings.LuauVRRigList[player.ActorNumber] = vrrig;
						Bindings.PlayerFunctions.UpdatePlayer(l, vrrig, ptr);
						Bindings.LuauPlayerList[player.ActorNumber] = (IntPtr)((void*)ptr);
						Luau.lua_rawseti(CustomGameMode.gameScriptRunner.L, -2, num + 1);
						ptr->PlayerName = vrrig.playerNameVisible;
						if (player.IsLocal)
						{
							Luau.lua_rawgeti(l, -1, num + 1);
							Luau.lua_setglobal(l, "LocalPlayer");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	// Token: 0x06003B22 RID: 15138 RVA: 0x00132260 File Offset: 0x00130460
	public unsafe override void OnPlayerLeftRoom(NetPlayer player)
	{
		try
		{
			if (CustomGameMode.gameScriptRunner != null)
			{
				if (CustomGameMode.gameScriptRunner.ShouldTick)
				{
					lua_State* l = CustomGameMode.gameScriptRunner.L;
					Bindings.LuauPlayerList.Remove(player.ActorNumber);
					Luau.lua_getglobal(l, "Players");
					int num = Luau.lua_objlen(l, -1);
					for (int i = 1; i <= num; i++)
					{
						Luau.lua_rawgeti(l, -1, i);
						Bindings.LuauPlayer* ptr = (Bindings.LuauPlayer*)Luau.lua_touserdata(l, -1);
						Luau.lua_pop(l, 1);
						if (ptr != null && ptr->PlayerID == player.ActorNumber)
						{
							for (int j = i; j < num; j++)
							{
								Luau.lua_rawgeti(l, -1, j + 1);
								Luau.lua_rawseti(l, -2, j);
							}
							Luau.lua_pushnil(l);
							Luau.lua_rawseti(l, -2, num);
							break;
						}
					}
					Luau.lua_pop(l, 1);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	// Token: 0x06003B23 RID: 15139 RVA: 0x0013234C File Offset: 0x0013054C
	public unsafe override void OnMasterClientSwitched(NetPlayer newMasterClient)
	{
		try
		{
			if (CustomGameMode.gameScriptRunner != null)
			{
				if (CustomGameMode.gameScriptRunner.ShouldTick)
				{
					foreach (KeyValuePair<int, IntPtr> keyValuePair in Bindings.LuauPlayerList)
					{
						Bindings.LuauPlayer* ptr = (Bindings.LuauPlayer*)((void*)keyValuePair.Value);
						ptr->IsMasterClient = false;
					}
					IntPtr value;
					Bindings.LuauPlayerList.TryGetValue(newMasterClient.ActorNumber, out value);
					Bindings.LuauPlayer* ptr2 = (Bindings.LuauPlayer*)((void*)value);
					ptr2->IsMasterClient = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	// Token: 0x06003B24 RID: 15140 RVA: 0x00132404 File Offset: 0x00130604
	public unsafe static void OnGameEntityRemoved(GameEntity entity)
	{
		if (CustomGameMode.gameScriptRunner == null)
		{
			return;
		}
		if (!CustomGameMode.gameScriptRunner.ShouldTick)
		{
			return;
		}
		lua_State* l = CustomGameMode.gameScriptRunner.L;
		if (Bindings.LuauAIAgentList.ContainsKey(entity.GetNetId()))
		{
			Bindings.LuauAIAgentList[entity.GetNetId()] = IntPtr.Zero;
			Luau.lua_getglobal(l, "AIAgents");
			int num = Luau.lua_objlen(l, -1);
			for (int i = 1; i <= num; i++)
			{
				Luau.lua_rawgeti(l, -1, i);
				Bindings.LuauAIAgent* ptr = (Bindings.LuauAIAgent*)Luau.lua_touserdata(l, -1);
				Luau.lua_pop(l, 1);
				if (ptr != null && ptr->EntityID == entity.GetNetId())
				{
					Luau.lua_pushnil(l);
					Luau.lua_rawseti(l, -2, i);
					break;
				}
			}
			Luau.lua_pop(l, 1);
			Luau.lua_getfield(l, -10002, "onEvent");
			if (Luau.lua_type(l, -1) == 7)
			{
				Luau.lua_pushstring(l, "agentDestroyed");
				Luau.lua_pushnumber(l, (double)entity.id.index);
				Luau.lua_pcall(l, 2, 0, 0);
				return;
			}
			Luau.lua_pop(l, 1);
		}
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x00132508 File Offset: 0x00130708
	public override void StartPlaying()
	{
		base.StartPlaying();
		try
		{
			PhotonNetwork.AddCallbackTarget(this);
			CustomGameMode.GameModeInitialized = true;
			if (CustomGameMode.LuaScript != "")
			{
				CustomGameMode.LuaStart();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	// Token: 0x06003B26 RID: 15142 RVA: 0x0013255C File Offset: 0x0013075C
	public unsafe static void LuaStart()
	{
		if (CustomGameMode.LuaScript == "")
		{
			return;
		}
		CustomGameMode.RunGamemodeScript(CustomGameMode.LuaScript);
		if (CustomGameMode.gameScriptRunner.ShouldTick)
		{
			lua_State* l = CustomGameMode.gameScriptRunner.L;
			Bindings.LuauPlayerList.Clear();
			Luau.lua_getglobal(l, "Players");
			Player[] playerList = PhotonNetwork.PlayerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				NetPlayer netPlayer = playerList[i];
				if (netPlayer != null)
				{
					Bindings.LuauPlayer* ptr = Luau.lua_class_push<Bindings.LuauPlayer>(l);
					ptr->PlayerID = netPlayer.ActorNumber;
					ptr->PlayerMaterial = 0;
					ptr->IsMasterClient = netPlayer.IsMasterClient;
					Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr);
					RigContainer rigContainer;
					VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer);
					VRRig rig = rigContainer.Rig;
					ptr->PlayerName = rig.playerNameVisible;
					Bindings.LuauVRRigList[netPlayer.ActorNumber] = rig;
					Bindings.PlayerFunctions.UpdatePlayer(l, rig, ptr);
					ptr->PlayerName = rig.playerNameVisible;
					Luau.lua_rawseti(l, -2, i + 1);
					if (netPlayer.IsLocal)
					{
						Luau.lua_rawgeti(l, -1, i + 1);
						Luau.lua_setglobal(l, "LocalPlayer");
					}
				}
				else
				{
					Luau.lua_pushnil(l);
					Luau.lua_rawseti(l, -2, i + 1);
				}
			}
			for (int j = playerList.Length; j <= 10; j++)
			{
				Luau.lua_pushnil(l);
				Luau.lua_rawseti(l, -2, j + 1);
			}
			Bindings.LuauAIAgentList.Clear();
			Luau.lua_getglobal(l, "AIAgents");
			List<GameAgent> agents = CustomMapsGameManager.instance.gameAgentManager.GetAgents();
			for (int k = 0; k < agents.Count; k++)
			{
				GameAgent gameAgent = agents[k];
				if (!gameAgent.IsNull() && !gameAgent.entity.IsNull())
				{
					Bindings.LuauAIAgent* ptr2 = Luau.lua_class_push<Bindings.LuauAIAgent>(l);
					Bindings.AIAgentFunctions.UpdateAIAgent(gameAgent, ptr2);
					Bindings.LuauAIAgentList[gameAgent.entity.GetNetId()] = (IntPtr)((void*)ptr2);
					Luau.lua_rawseti(l, -2, Bindings.LuauAIAgentList.Count);
					if (Bindings.LuauAIAgentList.Count == Constants.aiAgentLimit)
					{
						Debug.Log("[CustomGameMode::LuaStart] Custom Map AI Agent limit has been reached!");
						return;
					}
				}
			}
		}
	}

	// Token: 0x06003B27 RID: 15143 RVA: 0x0013279C File Offset: 0x0013099C
	public override void StopPlaying()
	{
		base.StopPlaying();
		try
		{
			CustomGameMode.GameModeInitialized = false;
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	// Token: 0x06003B28 RID: 15144 RVA: 0x001327E0 File Offset: 0x001309E0
	public static void StopScript()
	{
		if (CustomGameMode.gameScriptRunner.ShouldTick)
		{
			Luau.lua_close(CustomGameMode.gameScriptRunner.L);
		}
		LuauScriptRunner.ScriptRunners.Remove(CustomGameMode.gameScriptRunner);
		CustomGameMode.gameScriptRunner.ShouldTick = false;
		CustomGameMode.gameScriptRunner = null;
		foreach (KeyValuePair<GameObject, Bindings.LuauGameObjectInitialState> keyValuePair in Bindings.LuauGameObjectStates)
		{
			Bindings.LuauGameObjectInitialState value = keyValuePair.Value;
			GameObject key = keyValuePair.Key;
			if (key.IsNotNull())
			{
				key.SetActive(true);
				key.transform.localPosition = value.Position;
				key.transform.localRotation = value.Rotation;
				key.transform.localScale = value.Scale;
				MeshRenderer component = key.GetComponent<MeshRenderer>();
				Collider component2 = key.GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = value.Visible;
				}
				if (component2 != null)
				{
					component2.enabled = value.Collidable;
				}
			}
		}
		Bindings.LuauGameObjectStates.Clear();
		LuauVm.ClassBuilders.Clear();
		Bindings.LuauPlayerList.Clear();
		Bindings.LuauGameObjectList.Clear();
		Bindings.LuauVRRigList.Clear();
		ReflectionMetaNames.ReflectedNames.Clear();
		if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			BurstClassInfo.ClassList.InfoFields.Data.Clear();
		}
	}

	// Token: 0x06003B29 RID: 15145 RVA: 0x0013295C File Offset: 0x00130B5C
	public unsafe static void TouchPlayer(NetPlayer touchedPlayer)
	{
		if (CustomGameMode.gameScriptRunner == null)
		{
			return;
		}
		if (!CustomGameMode.gameScriptRunner.ShouldTick)
		{
			return;
		}
		lua_State* l = CustomGameMode.gameScriptRunner.L;
		Luau.lua_getfield(l, -10002, "onEvent");
		if (Luau.lua_type(l, -1) == 7)
		{
			IntPtr ptr;
			if (Bindings.LuauPlayerList.TryGetValue(touchedPlayer.ActorNumber, out ptr))
			{
				Luau.lua_pushstring(l, "touchedPlayer");
				Luau.lua_class_push(l, "Player", ptr);
				Luau.lua_pcall(l, 2, 0, 0);
				return;
			}
		}
		else
		{
			Luau.lua_pop(l, 1);
		}
	}

	// Token: 0x06003B2A RID: 15146 RVA: 0x001329E8 File Offset: 0x00130BE8
	public unsafe static void TaggedByEnvironment()
	{
		if (CustomGameMode.gameScriptRunner == null)
		{
			return;
		}
		if (!CustomGameMode.gameScriptRunner.ShouldTick)
		{
			return;
		}
		lua_State* l = CustomGameMode.gameScriptRunner.L;
		Luau.lua_getfield(l, -10002, "onEvent");
		if (Luau.lua_type(l, -1) == 7)
		{
			Luau.lua_pushstring(l, "taggedByEnvironment");
			Luau.lua_pushnil(l);
			Luau.lua_pcall(l, 2, 0, 0);
			return;
		}
		Luau.lua_pop(l, 1);
	}

	// Token: 0x06003B2B RID: 15147 RVA: 0x00132A54 File Offset: 0x00130C54
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int GameModeBindings(lua_State* L)
	{
		Bindings.GorillaLocomotionSettingsBuilder(L);
		Bindings.PlayerInputBuilder(L);
		Bindings.PlayerBuilder(L);
		Bindings.GameObjectBuilder(L);
		Bindings.AIAgentBuilder(L);
		Luau.lua_createtable(L, 10, 0);
		Luau.lua_setglobal(L, "Players");
		Luau.lua_createtable(L, Constants.aiAgentLimit, 0);
		Luau.lua_setglobal(L, "AIAgents");
		Luau.lua_register(L, new lua_CFunction(Bindings.LuaEmit.Emit), "emitEvent");
		Luau.lua_register(L, new lua_CFunction(Bindings.LuaStartVibration), "startVibration");
		Luau.lua_register(L, new lua_CFunction(Bindings.LuaPlaySound), "playSound");
		Luau.lua_register(L, new lua_CFunction(Bindings.JSON.DataSave), "dataSave");
		Luau.lua_register(L, new lua_CFunction(Bindings.JSON.DataLoad), "dataLoad");
		return 0;
	}

	// Token: 0x06003B2C RID: 15148 RVA: 0x00132B20 File Offset: 0x00130D20
	public unsafe override float[] LocalPlayerSpeed()
	{
		if (Bindings.LocomotionSettings == null || CustomGameMode.gameScriptRunner == null || !CustomGameMode.gameScriptRunner.ShouldTick)
		{
			this.playerSpeed[0] = 6.5f;
			this.playerSpeed[1] = 1.1f;
		}
		else
		{
			this.playerSpeed[0] = Bindings.LocomotionSettings->maxJumpSpeed.ClampSafe(0f, 100f);
			this.playerSpeed[1] = Bindings.LocomotionSettings->jumpMultiplier.ClampSafe(0f, 100f);
		}
		return this.playerSpeed;
	}

	// Token: 0x06003B2D RID: 15149 RVA: 0x00132BB0 File Offset: 0x00130DB0
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int AfterTickGamemode(lua_State* L)
	{
		try
		{
			foreach (KeyValuePair<GameObject, IntPtr> keyValuePair in Bindings.LuauGameObjectList)
			{
				GameObject key = keyValuePair.Key;
				if (key.IsNotNull())
				{
					Transform transform = key.transform;
					Bindings.LuauGameObject* ptr = (Bindings.LuauGameObject*)((void*)keyValuePair.Value);
					Vector3 position = ptr->Position;
					position = new Vector3((float)Math.Round((double)position.x, 4), (float)Math.Round((double)position.y, 4), (float)Math.Round((double)position.z, 4));
					transform.SetPositionAndRotation(position, ptr->Rotation);
					transform.localScale = ptr->Scale;
				}
			}
		}
		catch (Exception)
		{
		}
		return 0;
	}

	// Token: 0x06003B2E RID: 15150 RVA: 0x00132C8C File Offset: 0x00130E8C
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int PreTickGamemode(lua_State* L)
	{
		try
		{
			Luau.lua_pushboolean(L, (PhotonNetwork.InRoom && CustomGameMode.WasInRoom) ? 1 : 0);
			Luau.lua_setglobal(L, "InRoom");
			foreach (KeyValuePair<int, IntPtr> keyValuePair in Bindings.LuauPlayerList)
			{
				Bindings.LuauPlayer* ptr = (Bindings.LuauPlayer*)((void*)keyValuePair.Value);
				VRRig vrrig;
				Bindings.LuauVRRigList.TryGetValue(keyValuePair.Key, out vrrig);
				if (!vrrig.IsNotNull())
				{
					LuauHud.Instance.LuauLog("Unknown Rig for player");
				}
				else
				{
					if (keyValuePair.Key == PhotonNetwork.LocalPlayer.ActorNumber)
					{
						ptr->IsMasterClient = PhotonNetwork.LocalPlayer.IsMasterClient;
					}
					Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr);
				}
			}
			Luau.lua_getglobal(L, "AIAgents");
			List<GameAgent> agents = CustomMapsGameManager.instance.gameAgentManager.GetAgents();
			for (int i = 0; i < agents.Count; i++)
			{
				GameAgent gameAgent = agents[i];
				if (!gameAgent.IsNull() && !gameAgent.entity.IsNull())
				{
					IntPtr value;
					if (Bindings.LuauAIAgentList.TryGetValue(gameAgent.entity.GetNetId(), out value))
					{
						Bindings.AIAgentFunctions.UpdateAIAgent(gameAgent, (Bindings.LuauAIAgent*)((void*)value));
					}
					else if (Bindings.LuauAIAgentList.Count == Constants.aiAgentLimit)
					{
						Debug.Log("[CustomGameMode::PreTick] Custom Map AI Agent limit has been reached!");
					}
					else
					{
						Bindings.LuauAIAgent* ptr2 = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
						Bindings.AIAgentFunctions.UpdateAIAgent(gameAgent, ptr2);
						Bindings.LuauAIAgentList[gameAgent.entity.GetNetId()] = (IntPtr)((void*)ptr2);
						Luau.lua_rawseti(L, -2, Bindings.LuauAIAgentList.Count);
					}
				}
			}
			Luau.lua_pop(L, 1);
			foreach (KeyValuePair<GameObject, IntPtr> keyValuePair2 in Bindings.LuauGameObjectList)
			{
				GameObject key = keyValuePair2.Key;
				if (key.IsNotNull())
				{
					Transform transform = key.transform;
					Bindings.LuauGameObject* ptr3 = (Bindings.LuauGameObject*)((void*)keyValuePair2.Value);
					Vector3 position = transform.position;
					position = new Vector3((float)Math.Round((double)position.x, 4), (float)Math.Round((double)position.y, 4), (float)Math.Round((double)position.z, 4));
					ptr3->Position = position;
					ptr3->Rotation = transform.rotation;
					ptr3->Scale = transform.localScale;
				}
			}
			Bindings.UpdateInputs();
			CustomGameMode.WasInRoom = PhotonNetwork.InRoom;
		}
		catch (Exception)
		{
		}
		return 0;
	}

	// Token: 0x06003B2F RID: 15151 RVA: 0x00132F60 File Offset: 0x00131160
	private static void RunGamemodeScript(string script)
	{
		CustomGameMode.gameScriptRunner = new LuauScriptRunner(script, "GameMode", new lua_CFunction(CustomGameMode.GameModeBindings), new lua_CFunction(CustomGameMode.PreTickGamemode), new lua_CFunction(CustomGameMode.AfterTickGamemode));
	}

	// Token: 0x06003B30 RID: 15152 RVA: 0x00132F96 File Offset: 0x00131196
	private static void RunGamemodeScriptFromFile(string filename)
	{
		CustomGameMode.RunGamemodeScript(File.ReadAllText(Path.Join(Application.persistentDataPath, "Scripts", filename)));
	}

	// Token: 0x040048B1 RID: 18609
	public static LuauScriptRunner gameScriptRunner;

	// Token: 0x040048B2 RID: 18610
	public static string LuaScript = "";

	// Token: 0x040048B3 RID: 18611
	private static bool WasInRoom = false;

	// Token: 0x040048B4 RID: 18612
	public static bool GameModeInitialized;
}
