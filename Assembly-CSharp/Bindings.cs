using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000972 RID: 2418
[BurstCompile]
public static class Bindings
{
	// Token: 0x06003B33 RID: 15155 RVA: 0x00132FD4 File Offset: 0x001311D4
	public unsafe static void GameObjectBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGameObject>("GameObject").AddField("position", "Position").AddField("rotation", "Rotation").AddField("scale", "Scale").AddStaticFunction("findGameObject", new lua_CFunction(Bindings.GameObjectFunctions.FindGameObject)).AddFunction("setCollision", new lua_CFunction(Bindings.GameObjectFunctions.SetCollision)).AddFunction("setVisibility", new lua_CFunction(Bindings.GameObjectFunctions.SetVisibility)).AddFunction("setActive", new lua_CFunction(Bindings.GameObjectFunctions.SetActive)).AddFunction("setText", new lua_CFunction(Bindings.GameObjectFunctions.SetText)).Build(L, true));
	}

	// Token: 0x06003B34 RID: 15156 RVA: 0x00133098 File Offset: 0x00131298
	public unsafe static void GorillaLocomotionSettingsBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.GorillaLocomotionSettings>("PSettings").AddField("velocityLimit", null).AddField("slideVelocityLimit", null).AddField("maxJumpSpeed", null).AddField("jumpMultiplier", null).Build(L, false));
		Bindings.LocomotionSettings = Luau.lua_class_push<Bindings.GorillaLocomotionSettings>(L);
		Bindings.LocomotionSettings->velocityLimit = GTPlayer.Instance.velocityLimit;
		Bindings.LocomotionSettings->slideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
		Bindings.LocomotionSettings->maxJumpSpeed = 6.5f;
		Bindings.LocomotionSettings->jumpMultiplier = 1.1f;
		Luau.lua_setglobal(L, "PlayerSettings");
	}

	// Token: 0x06003B35 RID: 15157 RVA: 0x0013314C File Offset: 0x0013134C
	public unsafe static void PlayerInputBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.PlayerInput>("PInput").AddField("leftXAxis", null).AddField("rightXAxis", null).AddField("leftYAxis", null).AddField("rightYAxis", null).AddField("leftTrigger", null).AddField("rightTrigger", null).AddField("leftGrip", null).AddField("rightGrip", null).AddField("leftPrimaryButton", null).AddField("rightPrimaryButton", null).AddField("leftSecondaryButton", null).AddField("rightSecondaryButton", null).Build(L, false));
		Bindings.LocalPlayerInput = Luau.lua_class_push<Bindings.PlayerInput>(L);
		Bindings.UpdateInputs();
		Luau.lua_setglobal(L, "PlayerInput");
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x00133214 File Offset: 0x00131414
	public unsafe static void UpdateInputs()
	{
		if (Bindings.LocalPlayerInput != null)
		{
			Bindings.LocalPlayerInput->leftPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightPrimaryButton = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightSecondaryButton = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftGrip = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightGrip = ControllerInputPoller.GripFloat(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
			Bindings.LocalPlayerInput->rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
			Vector2 vector = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand);
			Vector2 vector2 = ControllerInputPoller.Primary2DAxis(XRNode.RightHand);
			Bindings.LocalPlayerInput->leftXAxis = vector.x;
			Bindings.LocalPlayerInput->leftYAxis = vector.y;
			Bindings.LocalPlayerInput->rightXAxis = vector2.x;
			Bindings.LocalPlayerInput->rightYAxis = vector2.y;
		}
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x001332FC File Offset: 0x001314FC
	public unsafe static void Vec3Builder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Vector3>("Vec3").AddField("x", null).AddField("y", null).AddField("z", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.New))).AddFunction("__add", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Add))).AddFunction("__sub", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Sub))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Mul))).AddFunction("__div", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Div))).AddFunction("__unm", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Unm))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.Vec3Functions.ToString)).AddFunction("dot", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Dot))).AddFunction("cross", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Cross))).AddFunction("projectOnTo", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Project))).AddFunction("length", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Length))).AddFunction("normalize", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Normalize))).AddFunction("getSafeNormal", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.SafeNormal))).AddStaticFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate))).AddStaticFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance))).AddStaticFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp))).AddProperty("zeroVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.ZeroVector))).AddProperty("oneVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.OneVector))).AddStaticFunction("nearlyEqual", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.NearlyEqual))).Build(L, true));
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x001335C4 File Offset: 0x001317C4
	public unsafe static void QuatBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Quaternion>("Quat").AddField("x", null).AddField("y", null).AddField("z", null).AddField("w", null).AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.New))).AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Mul))).AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Eq))).AddFunction("__tostring", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.QuatFunctions.ToString)).AddStaticFunction("fromEuler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromEuler))).AddStaticFunction("fromDirection", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromDirection))).AddFunction("getUpVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.GetUpVector))).AddFunction("euler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Euler))).Build(L, true));
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x00133704 File Offset: 0x00131904
	public unsafe static void PlayerBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauPlayer>("Player").AddField("playerID", "PlayerID").AddField("playerName", "PlayerName").AddField("playerMaterial", "PlayerMaterial").AddField("isMasterClient", "IsMasterClient").AddField("bodyPosition", "BodyPosition").AddField("leftHandPosition", "LeftHandPosition").AddField("rightHandPosition", "RightHandPosition").AddField("headRotation", "HeadRotation").AddField("leftHandRotation", "LeftHandRotation").AddField("rightHandRotation", "RightHandRotation").AddField("isInVStump", "IsInVStump").AddField("isEntityAuthority", "IsEntityAuthority").AddStaticFunction("getPlayerByID", new lua_CFunction(Bindings.PlayerFunctions.GetPlayerByID)).Build(L, true));
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x001337F8 File Offset: 0x001319F8
	public unsafe static void AIAgentBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauAIAgent>("AIAgent").AddField("entityID", "EntityID").AddField("agentPosition", "AgentPosition").AddField("agentRotation", "AgentRotation").AddFunction("__tostring", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("toString", new lua_CFunction(Bindings.AIAgentFunctions.ToString)).AddFunction("setDestination", new lua_CFunction(Bindings.AIAgentFunctions.SetDestination)).AddFunction("destroyAgent", new lua_CFunction(Bindings.AIAgentFunctions.DestroyAgent)).AddFunction("playAgentAnimation", new lua_CFunction(Bindings.AIAgentFunctions.PlayAgentAnimation)).AddStaticFunction("findPrePlacedAIAgentByID", new lua_CFunction(Bindings.AIAgentFunctions.FindPrePlacedAIAgentByID)).AddStaticFunction("getAIAgentByEntityID", new lua_CFunction(Bindings.AIAgentFunctions.GetAIAgentByEntityID)).AddStaticFunction("spawnAIAgent", new lua_CFunction(Bindings.AIAgentFunctions.SpawnAIAgent)).Build(L, true));
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x00133900 File Offset: 0x00131B00
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaStartVibration(lua_State* L)
	{
		bool forLeftController = Luau.lua_toboolean(L, 1) == 1;
		float amplitude = (float)Luau.luaL_checknumber(L, 2);
		float duration = (float)Luau.luaL_checknumber(L, 3);
		GorillaTagger.Instance.StartVibration(forLeftController, amplitude, duration);
		return 0;
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x00133938 File Offset: 0x00131B38
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaPlaySound(lua_State* L)
	{
		int num = (int)Luau.luaL_checknumber(L, 1);
		Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
		float volume = (float)Luau.luaL_checknumber(L, 3);
		if (num < 0 || num >= VRRig.LocalRig.clipToPlay.Length)
		{
			return 0;
		}
		AudioSource.PlayClipAtPoint(VRRig.LocalRig.clipToPlay[num], position, volume);
		return 0;
	}

	// Token: 0x040048B5 RID: 18613
	public static Dictionary<GameObject, IntPtr> LuauGameObjectList = new Dictionary<GameObject, IntPtr>();

	// Token: 0x040048B6 RID: 18614
	public static Dictionary<GameObject, Bindings.LuauGameObjectInitialState> LuauGameObjectStates = new Dictionary<GameObject, Bindings.LuauGameObjectInitialState>();

	// Token: 0x040048B7 RID: 18615
	public static Dictionary<int, IntPtr> LuauPlayerList = new Dictionary<int, IntPtr>();

	// Token: 0x040048B8 RID: 18616
	public static Dictionary<int, VRRig> LuauVRRigList = new Dictionary<int, VRRig>();

	// Token: 0x040048B9 RID: 18617
	public unsafe static Bindings.GorillaLocomotionSettings* LocomotionSettings;

	// Token: 0x040048BA RID: 18618
	public unsafe static Bindings.PlayerInput* LocalPlayerInput;

	// Token: 0x040048BB RID: 18619
	public static Dictionary<int, IntPtr> LuauAIAgentList = new Dictionary<int, IntPtr>();

	// Token: 0x02000973 RID: 2419
	public static class LuaEmit
	{
		// Token: 0x06003B3E RID: 15166 RVA: 0x001339CC File Offset: 0x00131BCC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Emit(lua_State* L)
		{
			if (Bindings.LuaEmit.callTime < Time.time - 1f)
			{
				Bindings.LuaEmit.callTime = Time.time - 1f;
			}
			Bindings.LuaEmit.callTime += 1f / Bindings.LuaEmit.callCount;
			if (Bindings.LuaEmit.callTime > Time.time)
			{
				LuauHud.Instance.LuauLog("Emit rate limit reached, event not sent");
				return 0;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others
			};
			if (Luau.lua_type(L, 2) != 6)
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				return 0;
			}
			Luau.lua_pushnil(L);
			int num = 0;
			List<object> list = new List<object>();
			list.Add(Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1))));
			while (Luau.lua_next(L, 2) != 0 && num++ < 10)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				if (lua_Types <= Luau.lua_Types.LUA_TNUMBER)
				{
					if (lua_Types == Luau.lua_Types.LUA_TBOOLEAN)
					{
						list.Add(Luau.lua_toboolean(L, -1) == 1);
						Luau.lua_pop(L, 1);
						continue;
					}
					if (lua_Types == Luau.lua_Types.LUA_TNUMBER)
					{
						list.Add(Luau.luaL_checknumber(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
				}
				else if (lua_Types == Luau.lua_Types.LUA_TTABLE || lua_Types == Luau.lua_Types.LUA_TUSERDATA)
				{
					Luau.luaL_getmetafield(L, -1, "metahash");
					BurstClassInfo.ClassInfo classInfo;
					if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
					{
						FixedString64Bytes fixedString64Bytes = "\"Internal Class Info Error No Metatable Found\"";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return 0;
					}
					Luau.lua_pop(L, 1);
					FixedString32Bytes fixedString32Bytes = "Vec3";
					if (classInfo.Name == fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Vector3>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Quat";
					if (classInfo.Name == fixedString32Bytes)
					{
						list.Add(*Luau.lua_class_get<Quaternion>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Player";
					if (classInfo.Name == fixedString32Bytes)
					{
						int playerID = Luau.lua_class_get<Bindings.LuauPlayer>(L, -1)->PlayerID;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in RoomSystem.PlayersInRoom)
						{
							if (netPlayer2.ActorNumber == playerID)
							{
								netPlayer = netPlayer2;
							}
						}
						if (netPlayer == null)
						{
							list.Add(null);
						}
						else
						{
							list.Add(netPlayer.GetPlayerRef());
						}
						Luau.lua_pop(L, 1);
						continue;
					}
					FixedString32Bytes fixedString32Bytes2 = "\"Unknown Type in table\"";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
					continue;
				}
				FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type in table\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return 0;
			}
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent(180, list.ToArray(), raiseEventOptions, SendOptions.SendReliable);
			}
			return 0;
		}

		// Token: 0x040048BC RID: 18620
		private static float callTime = 0f;

		// Token: 0x040048BD RID: 18621
		private static float callCount = 20f;
	}

	// Token: 0x02000974 RID: 2420
	[BurstCompile]
	public struct LuauGameObject
	{
		// Token: 0x040048BE RID: 18622
		public Vector3 Position;

		// Token: 0x040048BF RID: 18623
		public Quaternion Rotation;

		// Token: 0x040048C0 RID: 18624
		public Vector3 Scale;
	}

	// Token: 0x02000975 RID: 2421
	[BurstCompile]
	public struct LuauGameObjectInitialState
	{
		// Token: 0x040048C1 RID: 18625
		public Vector3 Position;

		// Token: 0x040048C2 RID: 18626
		public Quaternion Rotation;

		// Token: 0x040048C3 RID: 18627
		public Vector3 Scale;

		// Token: 0x040048C4 RID: 18628
		public bool Visible;

		// Token: 0x040048C5 RID: 18629
		public bool Collidable;
	}

	// Token: 0x02000976 RID: 2422
	[BurstCompile]
	public static class GameObjectFunctions
	{
		// Token: 0x06003B40 RID: 15168 RVA: 0x00133CD8 File Offset: 0x00131ED8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
			ptr->Position = gameObject.transform.position;
			ptr->Rotation = gameObject.transform.rotation;
			ptr->Scale = gameObject.transform.localScale;
			Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			return 1;
		}

		// Token: 0x06003B41 RID: 15169 RVA: 0x00133D3C File Offset: 0x00131F3C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindGameObject(lua_State* L)
		{
			GameObject gameObject = GameObject.Find(new string((sbyte*)Luau.luaL_checkstring(L, 1)));
			if (!(gameObject != null))
			{
				return 0;
			}
			if (!CustomMapLoader.IsCustomScene(gameObject.scene.name))
			{
				return 0;
			}
			IntPtr ptr;
			if (Bindings.LuauGameObjectList.TryGetValue(gameObject, out ptr))
			{
				Luau.lua_class_push(L, "GameObject", ptr);
			}
			else
			{
				Bindings.LuauGameObject* ptr2 = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr2->Position = gameObject.transform.position;
				ptr2->Rotation = gameObject.transform.rotation;
				ptr2->Scale = gameObject.transform.localScale;
				Bindings.LuauGameObjectInitialState value = default(Bindings.LuauGameObjectInitialState);
				value.Position = gameObject.transform.localPosition;
				value.Rotation = gameObject.transform.localRotation;
				value.Scale = gameObject.transform.localScale;
				value.Visible = true;
				value.Collidable = true;
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				Collider component2 = gameObject.GetComponent<Collider>();
				if (component2.IsNotNull())
				{
					value.Collidable = component2.enabled;
				}
				if (component.IsNotNull())
				{
					value.Visible = component.enabled;
				}
				Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr2));
				Bindings.LuauGameObjectStates.TryAdd(gameObject, value);
			}
			return 1;
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x00133E8C File Offset: 0x0013208C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetCollision(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Collider component = Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.GetComponent<Collider>();
			if (component.IsNotNull())
			{
				component.enabled = (Luau.lua_toboolean(L, 2) == 1);
			}
			return 0;
		}

		// Token: 0x06003B43 RID: 15171 RVA: 0x00133EF4 File Offset: 0x001320F4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVisibility(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			MeshRenderer component = Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.GetComponent<MeshRenderer>();
			if (component.IsNotNull())
			{
				component.enabled = (Luau.lua_toboolean(L, 2) == 1);
			}
			return 0;
		}

		// Token: 0x06003B44 RID: 15172 RVA: 0x00133F5C File Offset: 0x0013215C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetActive(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.SetActive(Luau.lua_toboolean(L, 2) == 1);
			return 0;
		}

		// Token: 0x06003B45 RID: 15173 RVA: 0x00133FB4 File Offset: 0x001321B4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetText(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject key = Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key;
			string text = new string(Luau.lua_tostring(L, 2));
			TextMeshPro component = key.GetComponent<TextMeshPro>();
			if (component.IsNotNull())
			{
				component.text = text;
			}
			else
			{
				TextMesh component2 = key.GetComponent<TextMesh>();
				if (component2.IsNotNull())
				{
					component2.text = text;
				}
			}
			return 0;
		}
	}

	// Token: 0x0200097B RID: 2427
	[BurstCompile]
	public struct LuauPlayer
	{
		// Token: 0x040048CA RID: 18634
		public int PlayerID;

		// Token: 0x040048CB RID: 18635
		public FixedString32Bytes PlayerName;

		// Token: 0x040048CC RID: 18636
		public int PlayerMaterial;

		// Token: 0x040048CD RID: 18637
		[MarshalAs(UnmanagedType.U1)]
		public bool IsMasterClient;

		// Token: 0x040048CE RID: 18638
		public Vector3 BodyPosition;

		// Token: 0x040048CF RID: 18639
		public Vector3 LeftHandPosition;

		// Token: 0x040048D0 RID: 18640
		public Vector3 RightHandPosition;

		// Token: 0x040048D1 RID: 18641
		[MarshalAs(UnmanagedType.U1)]
		public bool IsEntityAuthority;

		// Token: 0x040048D2 RID: 18642
		public Quaternion HeadRotation;

		// Token: 0x040048D3 RID: 18643
		public Quaternion LeftHandRotation;

		// Token: 0x040048D4 RID: 18644
		public Quaternion RightHandRotation;

		// Token: 0x040048D5 RID: 18645
		[MarshalAs(UnmanagedType.U1)]
		public bool IsInVStump;
	}

	// Token: 0x0200097C RID: 2428
	[BurstCompile]
	public static class PlayerFunctions
	{
		// Token: 0x06003B4E RID: 15182 RVA: 0x001340A4 File Offset: 0x001322A4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetPlayerByID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (netPlayer.ActorNumber == num)
				{
					IntPtr ptr;
					if (Bindings.LuauPlayerList.TryGetValue(netPlayer.ActorNumber, out ptr))
					{
						Luau.lua_class_push(L, "Player", ptr);
					}
					else
					{
						Bindings.LuauPlayer* ptr2 = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr2->PlayerID = netPlayer.ActorNumber;
						ptr2->PlayerMaterial = 0;
						ptr2->IsMasterClient = netPlayer.IsMasterClient;
						Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						GorillaGameManager instance = GorillaGameManager.instance;
						VRRig vrrig = (instance != null) ? instance.FindPlayerVRRig(netPlayer) : null;
						if (vrrig != null)
						{
							ptr2->PlayerName = vrrig.playerNameVisible;
							Bindings.LuauVRRigList[netPlayer.ActorNumber] = vrrig;
							Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr2);
							Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06003B4F RID: 15183 RVA: 0x001341DC File Offset: 0x001323DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdatePlayer(lua_State* L, VRRig p, Bindings.LuauPlayer* data)
		{
			data->BodyPosition = p.transform.position;
			data->LeftHandPosition = p.leftHandTransform.position;
			data->RightHandPosition = p.rightHandTransform.position;
			data->HeadRotation = p.transform.rotation;
			data->LeftHandRotation = p.leftHandTransform.rotation;
			data->RightHandRotation = p.rightHandTransform.rotation;
			if (p.isLocal)
			{
				data->IsInVStump = CustomMapManager.IsLocalPlayerInVirtualStump();
			}
			else if (p.creator != null)
			{
				data->IsInVStump = CustomMapManager.IsRemotePlayerInVirtualStump(p.creator.UserId);
			}
			else
			{
				data->IsInVStump = false;
			}
			data->IsEntityAuthority = (CustomMapsGameManager.instance.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsNotNull() && CustomMapsGameManager.instance.gameEntityManager.IsZoneAuthority());
		}
	}

	// Token: 0x0200097D RID: 2429
	[BurstCompile]
	public struct LuauAIAgent
	{
		// Token: 0x040048D6 RID: 18646
		public int EntityID;

		// Token: 0x040048D7 RID: 18647
		public Vector3 AgentPosition;

		// Token: 0x040048D8 RID: 18648
		public Quaternion AgentRotation;
	}

	// Token: 0x0200097E RID: 2430
	[BurstCompile]
	public static class AIAgentFunctions
	{
		// Token: 0x06003B50 RID: 15184 RVA: 0x001342C0 File Offset: 0x001324C0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			string s = "NULL";
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				s = string.Concat(new string[]
				{
					"ID: ",
					ptr->EntityID.ToString(),
					" | Pos: ",
					ptr->AgentPosition.ToString(),
					" | Rot: ",
					ptr->AgentRotation.ToString()
				});
			}
			Luau.lua_pushstring(L, s);
			return 1;
		}

		// Token: 0x06003B51 RID: 15185 RVA: 0x00134344 File Offset: 0x00132544
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetAIAgentByEntityID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			Debug.Log(string.Format("[LuauBindings::GetAIAgentByEntityID] ID: {0}", num));
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(num);
				GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
				if (gameEntity.IsNotNull())
				{
					if (gameEntity.gameObject.IsNull())
					{
						return 0;
					}
					GameAgent component = gameEntity.gameObject.GetComponent<GameAgent>();
					if (component.IsNotNull())
					{
						Debug.Log("[LuauBindings::GetAIAgentByEntityID] Found agent: " + gameEntity.gameObject.name);
						IntPtr value;
						if (Bindings.LuauAIAgentList.TryGetValue(num, out value))
						{
							Bindings.AIAgentFunctions.UpdateAIAgent(component, (Bindings.LuauAIAgent*)((void*)value));
							Luau.lua_class_push<Bindings.LuauAIAgent>(L, (Bindings.LuauAIAgent*)((void*)value));
						}
						else
						{
							Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
							Bindings.AIAgentFunctions.UpdateAIAgent(component, ptr);
							Bindings.LuauAIAgentList[num] = (IntPtr)((void*)ptr);
						}
					}
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06003B52 RID: 15186 RVA: 0x00134434 File Offset: 0x00132634
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindPrePlacedAIAgentByID(lua_State* L)
		{
			short num = (short)Luau.luaL_checknumber(L, 1);
			GameAgentManager gameAgentManager = CustomMapsGameManager.instance.gameAgentManager;
			if (gameAgentManager.IsNotNull())
			{
				List<GameAgent> agents = gameAgentManager.GetAgents();
				for (int i = 0; i < agents.Count; i++)
				{
					if (!agents[i].gameObject.IsNull())
					{
						CustomMapsAIBehaviourController component = agents[i].gameObject.GetComponent<CustomMapsAIBehaviourController>();
						if (!component.IsNull() && component.luaAgentID == num)
						{
							IntPtr value;
							if (Bindings.LuauAIAgentList.TryGetValue(agents[i].entity.GetNetId(), out value))
							{
								Bindings.AIAgentFunctions.UpdateAIAgent(agents[i], (Bindings.LuauAIAgent*)((void*)value));
								Luau.lua_class_push<Bindings.LuauAIAgent>(L, (Bindings.LuauAIAgent*)((void*)value));
							}
							else
							{
								Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
								Bindings.AIAgentFunctions.UpdateAIAgent(agents[i], ptr);
								Bindings.LuauAIAgentList[agents[i].entity.GetNetId()] = (IntPtr)((void*)ptr);
							}
							return 1;
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06003B53 RID: 15187 RVA: 0x0013453C File Offset: 0x0013273C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SpawnAIAgent(lua_State* L)
		{
			CustomMapsGameManager instance = CustomMapsGameManager.instance;
			GameEntityManager gameEntityManager = instance.IsNotNull() ? instance.gameEntityManager : null;
			if (gameEntityManager.IsNull())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. EntityManager is null.");
				return 0;
			}
			if (!gameEntityManager.IsZoneAuthority())
			{
				LuauHud.Instance.LuauLog("SpawnAIAgent failed. Local Player doesn't have Entity Authority.");
				return 0;
			}
			if (Bindings.LuauAIAgentList.Count == Constants.aiAgentLimit)
			{
				LuauHud.Instance.LuauLog(string.Format("SpawnAIAgent failed, AIAgentLimit of {0}", Constants.aiAgentLimit) + " has already been reached.");
				return 0;
			}
			int enemyTypeId = (int)Luau.luaL_checknumber(L, 1);
			Vector3 position = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 3, "Quat");
			GameEntityId id = instance.SpawnEnemyAtLocation(enemyTypeId, position, rotation);
			if (id.IsValid())
			{
				GameEntity gameEntity = gameEntityManager.GetGameEntity(id);
				GameAgent gameAgent = gameEntity.IsNotNull() ? gameEntity.gameObject.GetComponent<GameAgent>() : null;
				if (gameAgent.IsNotNull())
				{
					IntPtr value;
					if (Bindings.LuauAIAgentList.TryGetValue(gameEntity.GetNetId(), out value))
					{
						Bindings.AIAgentFunctions.UpdateAIAgent(gameAgent, (Bindings.LuauAIAgent*)((void*)value));
						Luau.lua_class_push<Bindings.LuauAIAgent>(L, (Bindings.LuauAIAgent*)((void*)value));
						return 1;
					}
					Luau.lua_getglobal(L, "AIAgents");
					Bindings.LuauAIAgent* ptr = Luau.lua_class_push<Bindings.LuauAIAgent>(L);
					Bindings.AIAgentFunctions.UpdateAIAgent(gameAgent, ptr);
					Bindings.LuauAIAgentList[gameEntity.GetNetId()] = (IntPtr)((void*)ptr);
					Luau.lua_rawseti(L, -2, Bindings.LuauAIAgentList.Count);
					Luau.lua_pop(L, 1);
					Luau.lua_class_push<Bindings.LuauAIAgent>(L, ptr);
					return 1;
				}
			}
			LuauHud.Instance.LuauLog("SpawnAIAgent failed to create entity.");
			return 0;
		}

		// Token: 0x06003B54 RID: 15188 RVA: 0x001346E8 File Offset: 0x001328E8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetDestination(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			Vector3* ptr2 = Luau.lua_class_get<Vector3>(L, 2);
			GameEntityManager gameEntityManager = CustomMapsGameManager.instance.gameEntityManager;
			if (gameEntityManager.IsNotNull())
			{
				CustomMapsAIBehaviourController component = gameEntityManager.GetGameEntity(gameEntityManager.GetEntityIdFromNetId(ptr->EntityID)).gameObject.GetComponent<CustomMapsAIBehaviourController>();
				if (component.IsNotNull())
				{
					component.RequestDestination(*ptr2);
				}
			}
			return 0;
		}

		// Token: 0x06003B55 RID: 15189 RVA: 0x0013474A File Offset: 0x0013294A
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdateAIAgent(GameAgent agent, Bindings.LuauAIAgent* luaAgent)
		{
			luaAgent->EntityID = agent.entity.GetNetId();
			luaAgent->AgentPosition = agent.transform.position;
			luaAgent->AgentRotation = agent.transform.rotation;
		}

		// Token: 0x06003B56 RID: 15190 RVA: 0x00134780 File Offset: 0x00132980
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DestroyAgent(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(ptr->EntityID);
					entityManager.RequestDestroyItem(entityIdFromNetId);
				}
			}
			return 0;
		}

		// Token: 0x06003B57 RID: 15191 RVA: 0x001347C0 File Offset: 0x001329C0
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int PlayAgentAnimation(lua_State* L)
		{
			Bindings.LuauAIAgent* ptr = Luau.lua_class_get<Bindings.LuauAIAgent>(L, 1);
			string stateName = Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 2)));
			if (ptr != null)
			{
				GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
				if (entityManager.IsNotNull())
				{
					CustomMapsAIBehaviourController behaviorControllerForEntity = CustomMapsGameManager.GetBehaviorControllerForEntity(entityManager.GetEntityIdFromNetId(ptr->EntityID));
					if (behaviorControllerForEntity.IsNotNull())
					{
						behaviorControllerForEntity.PlayAnimation(stateName);
					}
				}
			}
			return 0;
		}
	}

	// Token: 0x0200097F RID: 2431
	[BurstCompile]
	public static class Vec3Functions
	{
		// Token: 0x06003B58 RID: 15192 RVA: 0x0013481C File Offset: 0x00132A1C
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B59 RID: 15193 RVA: 0x00134824 File Offset: 0x00132A24
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Add(lua_State* L)
		{
			return Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5A RID: 15194 RVA: 0x0013482C File Offset: 0x00132A2C
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Sub(lua_State* L)
		{
			return Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5B RID: 15195 RVA: 0x00134834 File Offset: 0x00132A34
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5C RID: 15196 RVA: 0x0013483C File Offset: 0x00132A3C
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Div(lua_State* L)
		{
			return Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x00134844 File Offset: 0x00132A44
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Unm(lua_State* L)
		{
			return Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5E RID: 15198 RVA: 0x0013484C File Offset: 0x00132A4C
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B5F RID: 15199 RVA: 0x00134854 File Offset: 0x00132A54
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushstring(L, vector.ToString());
			return 1;
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x0013488D File Offset: 0x00132A8D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Dot(lua_State* L)
		{
			return Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x00134895 File Offset: 0x00132A95
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Cross(lua_State* L)
		{
			return Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0013489D File Offset: 0x00132A9D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Project(lua_State* L)
		{
			return Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B63 RID: 15203 RVA: 0x001348A5 File Offset: 0x00132AA5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Length(lua_State* L)
		{
			return Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B64 RID: 15204 RVA: 0x001348AD File Offset: 0x00132AAD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Normalize(lua_State* L)
		{
			return Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B65 RID: 15205 RVA: 0x001348B5 File Offset: 0x00132AB5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SafeNormal(lua_State* L)
		{
			return Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x001348BD File Offset: 0x00132ABD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Distance(lua_State* L)
		{
			return Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B67 RID: 15207 RVA: 0x001348C5 File Offset: 0x00132AC5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Lerp(lua_State* L)
		{
			return Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B68 RID: 15208 RVA: 0x001348CD File Offset: 0x00132ACD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Rotate(lua_State* L)
		{
			return Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B69 RID: 15209 RVA: 0x001348D5 File Offset: 0x00132AD5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ZeroVector(lua_State* L)
		{
			return Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B6A RID: 15210 RVA: 0x001348DD File Offset: 0x00132ADD
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OneVector(lua_State* L)
		{
			return Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B6B RID: 15211 RVA: 0x001348E5 File Offset: 0x00132AE5
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int NearlyEqual(lua_State* L)
		{
			return Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003B6C RID: 15212 RVA: 0x001348F0 File Offset: 0x00132AF0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int New$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			return 1;
		}

		// Token: 0x06003B6D RID: 15213 RVA: 0x00134954 File Offset: 0x00132B54
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Add$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a + b;
			return 1;
		}

		// Token: 0x06003B6E RID: 15214 RVA: 0x001349AC File Offset: 0x00132BAC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Sub$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a - b;
			return 1;
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x00134A04 File Offset: 0x00132C04
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float d = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a * d;
			return 1;
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x00134A50 File Offset: 0x00132C50
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Div$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float d = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = a / d;
			return 1;
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x00134A9C File Offset: 0x00132C9C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Unm$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = -a;
			return 1;
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x00134ADC File Offset: 0x00132CDC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			int num = (lhs == rhs) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06003B73 RID: 15219 RVA: 0x00134B2C File Offset: 0x00132D2C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Dot$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double n = (double)Vector3.Dot(lhs, rhs);
			Luau.lua_pushnumber(L, n);
			return 1;
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x00134B78 File Offset: 0x00132D78
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Cross$BurstManaged(lua_State* L)
		{
			Vector3 lhs = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 rhs = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Cross(lhs, rhs);
			return 1;
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x00134BD0 File Offset: 0x00132DD0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Project$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 onNormal = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Project(vector, onNormal);
			return 1;
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x00134C28 File Offset: 0x00132E28
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Length$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Magnitude(vector));
			return 1;
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x00134C5A File Offset: 0x00132E5A
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Normalize$BurstManaged(lua_State* L)
		{
			Luau.lua_class_get<Vector3>(L, 1, "Vec3")->Normalize();
			return 0;
		}

		// Token: 0x06003B78 RID: 15224 RVA: 0x00134C74 File Offset: 0x00132E74
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int SafeNormal$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector.normalized;
			return 1;
		}

		// Token: 0x06003B79 RID: 15225 RVA: 0x00134CB8 File Offset: 0x00132EB8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Distance$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Distance(a, b));
			return 1;
		}

		// Token: 0x06003B7A RID: 15226 RVA: 0x00134D04 File Offset: 0x00132F04
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Lerp$BurstManaged(lua_State* L)
		{
			Vector3 a = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 b = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = Luau.luaL_checknumber(L, 3);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Lerp(a, b, (float)num);
			return 1;
		}

		// Token: 0x06003B7B RID: 15227 RVA: 0x00134D68 File Offset: 0x00132F68
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Rotate$BurstManaged(lua_State* L)
		{
			Vector3 point = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = rotation * point;
			return 1;
		}

		// Token: 0x06003B7C RID: 15228 RVA: 0x00134DC0 File Offset: 0x00132FC0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int ZeroVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 0f;
			ptr->y = 0f;
			ptr->z = 0f;
			return 1;
		}

		// Token: 0x06003B7D RID: 15229 RVA: 0x00134DF3 File Offset: 0x00132FF3
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int OneVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 1f;
			ptr->y = 1f;
			ptr->z = 1f;
			return 1;
		}

		// Token: 0x06003B7E RID: 15230 RVA: 0x00134E28 File Offset: 0x00133028
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int NearlyEqual$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			float num = (float)Luau.luaL_optnumber(L, 3, 0.0001);
			bool flag = Math.Abs(vector.x - vector2.x) <= num;
			if (flag && Math.Abs(vector.y - vector2.y) > num)
			{
				flag = false;
			}
			if (flag && Math.Abs(vector.z - vector2.z) > num)
			{
				flag = false;
			}
			Luau.lua_pushboolean(L, flag ? 1 : 0);
			return 1;
		}

		// Token: 0x02000980 RID: 2432
		// (Invoke) Token: 0x06003B80 RID: 15232
		public unsafe delegate int New_00003B58$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000981 RID: 2433
		internal static class New_00003B58$BurstDirectCall
		{
			// Token: 0x06003B83 RID: 15235 RVA: 0x00134ED0 File Offset: 0x001330D0
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.New_00003B58$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.New$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.New_00003B58$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Pointer;
			}

			// Token: 0x06003B84 RID: 15236 RVA: 0x00134EFC File Offset: 0x001330FC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.New_00003B58$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003B85 RID: 15237 RVA: 0x00134F14 File Offset: 0x00133114
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.New_00003B58$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.New(lua_State*)).MethodHandle);
			}

			// Token: 0x06003B86 RID: 15238 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003B87 RID: 15239 RVA: 0x00134F25 File Offset: 0x00133125
			// Note: this type is marked as 'beforefieldinit'.
			static New_00003B58$BurstDirectCall()
			{
				Bindings.Vec3Functions.New_00003B58$BurstDirectCall.Constructor();
			}

			// Token: 0x06003B88 RID: 15240 RVA: 0x00134F2C File Offset: 0x0013312C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.New_00003B58$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.New$BurstManaged(L);
			}

			// Token: 0x040048D9 RID: 18649
			private static IntPtr Pointer;

			// Token: 0x040048DA RID: 18650
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000982 RID: 2434
		// (Invoke) Token: 0x06003B8A RID: 15242
		public unsafe delegate int Add_00003B59$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000983 RID: 2435
		internal static class Add_00003B59$BurstDirectCall
		{
			// Token: 0x06003B8D RID: 15245 RVA: 0x00134F5D File Offset: 0x0013315D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Add$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Add_00003B59$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Pointer;
			}

			// Token: 0x06003B8E RID: 15246 RVA: 0x00134F8C File Offset: 0x0013318C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003B8F RID: 15247 RVA: 0x00134FA4 File Offset: 0x001331A4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Add(lua_State*)).MethodHandle);
			}

			// Token: 0x06003B90 RID: 15248 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003B91 RID: 15249 RVA: 0x00134FB5 File Offset: 0x001331B5
			// Note: this type is marked as 'beforefieldinit'.
			static Add_00003B59$BurstDirectCall()
			{
				Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.Constructor();
			}

			// Token: 0x06003B92 RID: 15250 RVA: 0x00134FBC File Offset: 0x001331BC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Add_00003B59$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Add$BurstManaged(L);
			}

			// Token: 0x040048DB RID: 18651
			private static IntPtr Pointer;

			// Token: 0x040048DC RID: 18652
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000984 RID: 2436
		// (Invoke) Token: 0x06003B94 RID: 15252
		public unsafe delegate int Sub_00003B5A$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000985 RID: 2437
		internal static class Sub_00003B5A$BurstDirectCall
		{
			// Token: 0x06003B97 RID: 15255 RVA: 0x00134FED File Offset: 0x001331ED
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Sub$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Sub_00003B5A$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Pointer;
			}

			// Token: 0x06003B98 RID: 15256 RVA: 0x0013501C File Offset: 0x0013321C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003B99 RID: 15257 RVA: 0x00135034 File Offset: 0x00133234
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Sub(lua_State*)).MethodHandle);
			}

			// Token: 0x06003B9A RID: 15258 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003B9B RID: 15259 RVA: 0x00135045 File Offset: 0x00133245
			// Note: this type is marked as 'beforefieldinit'.
			static Sub_00003B5A$BurstDirectCall()
			{
				Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.Constructor();
			}

			// Token: 0x06003B9C RID: 15260 RVA: 0x0013504C File Offset: 0x0013324C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Sub_00003B5A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Sub$BurstManaged(L);
			}

			// Token: 0x040048DD RID: 18653
			private static IntPtr Pointer;

			// Token: 0x040048DE RID: 18654
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000986 RID: 2438
		// (Invoke) Token: 0x06003B9E RID: 15262
		public unsafe delegate int Mul_00003B5B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000987 RID: 2439
		internal static class Mul_00003B5B$BurstDirectCall
		{
			// Token: 0x06003BA1 RID: 15265 RVA: 0x0013507D File Offset: 0x0013327D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Mul$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Mul_00003B5B$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BA2 RID: 15266 RVA: 0x001350AC File Offset: 0x001332AC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BA3 RID: 15267 RVA: 0x001350C4 File Offset: 0x001332C4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Mul(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BA4 RID: 15268 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BA5 RID: 15269 RVA: 0x001350D5 File Offset: 0x001332D5
			// Note: this type is marked as 'beforefieldinit'.
			static Mul_00003B5B$BurstDirectCall()
			{
				Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BA6 RID: 15270 RVA: 0x001350DC File Offset: 0x001332DC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Mul_00003B5B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Mul$BurstManaged(L);
			}

			// Token: 0x040048DF RID: 18655
			private static IntPtr Pointer;

			// Token: 0x040048E0 RID: 18656
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000988 RID: 2440
		// (Invoke) Token: 0x06003BA8 RID: 15272
		public unsafe delegate int Div_00003B5C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000989 RID: 2441
		internal static class Div_00003B5C$BurstDirectCall
		{
			// Token: 0x06003BAB RID: 15275 RVA: 0x0013510D File Offset: 0x0013330D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Div$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Div_00003B5C$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BAC RID: 15276 RVA: 0x0013513C File Offset: 0x0013333C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BAD RID: 15277 RVA: 0x00135154 File Offset: 0x00133354
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Div(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BAE RID: 15278 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BAF RID: 15279 RVA: 0x00135165 File Offset: 0x00133365
			// Note: this type is marked as 'beforefieldinit'.
			static Div_00003B5C$BurstDirectCall()
			{
				Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BB0 RID: 15280 RVA: 0x0013516C File Offset: 0x0013336C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Div_00003B5C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Div$BurstManaged(L);
			}

			// Token: 0x040048E1 RID: 18657
			private static IntPtr Pointer;

			// Token: 0x040048E2 RID: 18658
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200098A RID: 2442
		// (Invoke) Token: 0x06003BB2 RID: 15282
		public unsafe delegate int Unm_00003B5D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200098B RID: 2443
		internal static class Unm_00003B5D$BurstDirectCall
		{
			// Token: 0x06003BB5 RID: 15285 RVA: 0x0013519D File Offset: 0x0013339D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Unm$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Unm_00003B5D$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BB6 RID: 15286 RVA: 0x001351CC File Offset: 0x001333CC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BB7 RID: 15287 RVA: 0x001351E4 File Offset: 0x001333E4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Unm(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BB8 RID: 15288 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BB9 RID: 15289 RVA: 0x001351F5 File Offset: 0x001333F5
			// Note: this type is marked as 'beforefieldinit'.
			static Unm_00003B5D$BurstDirectCall()
			{
				Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BBA RID: 15290 RVA: 0x001351FC File Offset: 0x001333FC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Unm_00003B5D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Unm$BurstManaged(L);
			}

			// Token: 0x040048E3 RID: 18659
			private static IntPtr Pointer;

			// Token: 0x040048E4 RID: 18660
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200098C RID: 2444
		// (Invoke) Token: 0x06003BBC RID: 15292
		public unsafe delegate int Eq_00003B5E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200098D RID: 2445
		internal static class Eq_00003B5E$BurstDirectCall
		{
			// Token: 0x06003BBF RID: 15295 RVA: 0x0013522D File Offset: 0x0013342D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Eq$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Eq_00003B5E$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BC0 RID: 15296 RVA: 0x0013525C File Offset: 0x0013345C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BC1 RID: 15297 RVA: 0x00135274 File Offset: 0x00133474
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Eq(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BC2 RID: 15298 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BC3 RID: 15299 RVA: 0x00135285 File Offset: 0x00133485
			// Note: this type is marked as 'beforefieldinit'.
			static Eq_00003B5E$BurstDirectCall()
			{
				Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BC4 RID: 15300 RVA: 0x0013528C File Offset: 0x0013348C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Eq_00003B5E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Eq$BurstManaged(L);
			}

			// Token: 0x040048E5 RID: 18661
			private static IntPtr Pointer;

			// Token: 0x040048E6 RID: 18662
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200098E RID: 2446
		// (Invoke) Token: 0x06003BC6 RID: 15302
		public unsafe delegate int Dot_00003B60$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200098F RID: 2447
		internal static class Dot_00003B60$BurstDirectCall
		{
			// Token: 0x06003BC9 RID: 15305 RVA: 0x001352BD File Offset: 0x001334BD
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Dot$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Dot_00003B60$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BCA RID: 15306 RVA: 0x001352EC File Offset: 0x001334EC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BCB RID: 15307 RVA: 0x00135304 File Offset: 0x00133504
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Dot(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BCC RID: 15308 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BCD RID: 15309 RVA: 0x00135315 File Offset: 0x00133515
			// Note: this type is marked as 'beforefieldinit'.
			static Dot_00003B60$BurstDirectCall()
			{
				Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BCE RID: 15310 RVA: 0x0013531C File Offset: 0x0013351C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Dot_00003B60$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Dot$BurstManaged(L);
			}

			// Token: 0x040048E7 RID: 18663
			private static IntPtr Pointer;

			// Token: 0x040048E8 RID: 18664
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000990 RID: 2448
		// (Invoke) Token: 0x06003BD0 RID: 15312
		public unsafe delegate int Cross_00003B61$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000991 RID: 2449
		internal static class Cross_00003B61$BurstDirectCall
		{
			// Token: 0x06003BD3 RID: 15315 RVA: 0x0013534D File Offset: 0x0013354D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Cross$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Cross_00003B61$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BD4 RID: 15316 RVA: 0x0013537C File Offset: 0x0013357C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BD5 RID: 15317 RVA: 0x00135394 File Offset: 0x00133594
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Cross(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BD6 RID: 15318 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BD7 RID: 15319 RVA: 0x001353A5 File Offset: 0x001335A5
			// Note: this type is marked as 'beforefieldinit'.
			static Cross_00003B61$BurstDirectCall()
			{
				Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BD8 RID: 15320 RVA: 0x001353AC File Offset: 0x001335AC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Cross_00003B61$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Cross$BurstManaged(L);
			}

			// Token: 0x040048E9 RID: 18665
			private static IntPtr Pointer;

			// Token: 0x040048EA RID: 18666
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000992 RID: 2450
		// (Invoke) Token: 0x06003BDA RID: 15322
		public unsafe delegate int Project_00003B62$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000993 RID: 2451
		internal static class Project_00003B62$BurstDirectCall
		{
			// Token: 0x06003BDD RID: 15325 RVA: 0x001353DD File Offset: 0x001335DD
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Project$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Project_00003B62$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BDE RID: 15326 RVA: 0x0013540C File Offset: 0x0013360C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BDF RID: 15327 RVA: 0x00135424 File Offset: 0x00133624
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Project(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BE0 RID: 15328 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BE1 RID: 15329 RVA: 0x00135435 File Offset: 0x00133635
			// Note: this type is marked as 'beforefieldinit'.
			static Project_00003B62$BurstDirectCall()
			{
				Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BE2 RID: 15330 RVA: 0x0013543C File Offset: 0x0013363C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Project_00003B62$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Project$BurstManaged(L);
			}

			// Token: 0x040048EB RID: 18667
			private static IntPtr Pointer;

			// Token: 0x040048EC RID: 18668
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000994 RID: 2452
		// (Invoke) Token: 0x06003BE4 RID: 15332
		public unsafe delegate int Length_00003B63$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000995 RID: 2453
		internal static class Length_00003B63$BurstDirectCall
		{
			// Token: 0x06003BE7 RID: 15335 RVA: 0x0013546D File Offset: 0x0013366D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Length$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Length_00003B63$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BE8 RID: 15336 RVA: 0x0013549C File Offset: 0x0013369C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BE9 RID: 15337 RVA: 0x001354B4 File Offset: 0x001336B4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Length(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BEA RID: 15338 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BEB RID: 15339 RVA: 0x001354C5 File Offset: 0x001336C5
			// Note: this type is marked as 'beforefieldinit'.
			static Length_00003B63$BurstDirectCall()
			{
				Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BEC RID: 15340 RVA: 0x001354CC File Offset: 0x001336CC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Length_00003B63$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Length$BurstManaged(L);
			}

			// Token: 0x040048ED RID: 18669
			private static IntPtr Pointer;

			// Token: 0x040048EE RID: 18670
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000996 RID: 2454
		// (Invoke) Token: 0x06003BEE RID: 15342
		public unsafe delegate int Normalize_00003B64$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000997 RID: 2455
		internal static class Normalize_00003B64$BurstDirectCall
		{
			// Token: 0x06003BF1 RID: 15345 RVA: 0x001354FD File Offset: 0x001336FD
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Normalize$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Normalize_00003B64$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BF2 RID: 15346 RVA: 0x0013552C File Offset: 0x0013372C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BF3 RID: 15347 RVA: 0x00135544 File Offset: 0x00133744
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Normalize(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BF4 RID: 15348 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BF5 RID: 15349 RVA: 0x00135555 File Offset: 0x00133755
			// Note: this type is marked as 'beforefieldinit'.
			static Normalize_00003B64$BurstDirectCall()
			{
				Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.Constructor();
			}

			// Token: 0x06003BF6 RID: 15350 RVA: 0x0013555C File Offset: 0x0013375C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Normalize_00003B64$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Normalize$BurstManaged(L);
			}

			// Token: 0x040048EF RID: 18671
			private static IntPtr Pointer;

			// Token: 0x040048F0 RID: 18672
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000998 RID: 2456
		// (Invoke) Token: 0x06003BF8 RID: 15352
		public unsafe delegate int SafeNormal_00003B65$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000999 RID: 2457
		internal static class SafeNormal_00003B65$BurstDirectCall
		{
			// Token: 0x06003BFB RID: 15355 RVA: 0x0013558D File Offset: 0x0013378D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.SafeNormal$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.SafeNormal_00003B65$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Pointer;
			}

			// Token: 0x06003BFC RID: 15356 RVA: 0x001355BC File Offset: 0x001337BC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003BFD RID: 15357 RVA: 0x001355D4 File Offset: 0x001337D4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.SafeNormal(lua_State*)).MethodHandle);
			}

			// Token: 0x06003BFE RID: 15358 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003BFF RID: 15359 RVA: 0x001355E5 File Offset: 0x001337E5
			// Note: this type is marked as 'beforefieldinit'.
			static SafeNormal_00003B65$BurstDirectCall()
			{
				Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C00 RID: 15360 RVA: 0x001355EC File Offset: 0x001337EC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.SafeNormal_00003B65$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.SafeNormal$BurstManaged(L);
			}

			// Token: 0x040048F1 RID: 18673
			private static IntPtr Pointer;

			// Token: 0x040048F2 RID: 18674
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200099A RID: 2458
		// (Invoke) Token: 0x06003C02 RID: 15362
		public unsafe delegate int Distance_00003B66$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200099B RID: 2459
		internal static class Distance_00003B66$BurstDirectCall
		{
			// Token: 0x06003C05 RID: 15365 RVA: 0x0013561D File Offset: 0x0013381D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Distance$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Distance_00003B66$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C06 RID: 15366 RVA: 0x0013564C File Offset: 0x0013384C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C07 RID: 15367 RVA: 0x00135664 File Offset: 0x00133864
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Distance(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C08 RID: 15368 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C09 RID: 15369 RVA: 0x00135675 File Offset: 0x00133875
			// Note: this type is marked as 'beforefieldinit'.
			static Distance_00003B66$BurstDirectCall()
			{
				Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C0A RID: 15370 RVA: 0x0013567C File Offset: 0x0013387C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Distance_00003B66$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Distance$BurstManaged(L);
			}

			// Token: 0x040048F3 RID: 18675
			private static IntPtr Pointer;

			// Token: 0x040048F4 RID: 18676
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200099C RID: 2460
		// (Invoke) Token: 0x06003C0C RID: 15372
		public unsafe delegate int Lerp_00003B67$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200099D RID: 2461
		internal static class Lerp_00003B67$BurstDirectCall
		{
			// Token: 0x06003C0F RID: 15375 RVA: 0x001356AD File Offset: 0x001338AD
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Lerp$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Lerp_00003B67$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C10 RID: 15376 RVA: 0x001356DC File Offset: 0x001338DC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C11 RID: 15377 RVA: 0x001356F4 File Offset: 0x001338F4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Lerp(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C12 RID: 15378 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C13 RID: 15379 RVA: 0x00135705 File Offset: 0x00133905
			// Note: this type is marked as 'beforefieldinit'.
			static Lerp_00003B67$BurstDirectCall()
			{
				Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C14 RID: 15380 RVA: 0x0013570C File Offset: 0x0013390C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Lerp_00003B67$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Lerp$BurstManaged(L);
			}

			// Token: 0x040048F5 RID: 18677
			private static IntPtr Pointer;

			// Token: 0x040048F6 RID: 18678
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200099E RID: 2462
		// (Invoke) Token: 0x06003C16 RID: 15382
		public unsafe delegate int Rotate_00003B68$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200099F RID: 2463
		internal static class Rotate_00003B68$BurstDirectCall
		{
			// Token: 0x06003C19 RID: 15385 RVA: 0x0013573D File Offset: 0x0013393D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Rotate$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Rotate_00003B68$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C1A RID: 15386 RVA: 0x0013576C File Offset: 0x0013396C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C1B RID: 15387 RVA: 0x00135784 File Offset: 0x00133984
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Rotate(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C1C RID: 15388 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C1D RID: 15389 RVA: 0x00135795 File Offset: 0x00133995
			// Note: this type is marked as 'beforefieldinit'.
			static Rotate_00003B68$BurstDirectCall()
			{
				Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C1E RID: 15390 RVA: 0x0013579C File Offset: 0x0013399C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Rotate_00003B68$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Rotate$BurstManaged(L);
			}

			// Token: 0x040048F7 RID: 18679
			private static IntPtr Pointer;

			// Token: 0x040048F8 RID: 18680
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009A0 RID: 2464
		// (Invoke) Token: 0x06003C20 RID: 15392
		public unsafe delegate int ZeroVector_00003B69$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009A1 RID: 2465
		internal static class ZeroVector_00003B69$BurstDirectCall
		{
			// Token: 0x06003C23 RID: 15395 RVA: 0x001357CD File Offset: 0x001339CD
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.ZeroVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.ZeroVector_00003B69$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C24 RID: 15396 RVA: 0x001357FC File Offset: 0x001339FC
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C25 RID: 15397 RVA: 0x00135814 File Offset: 0x00133A14
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.ZeroVector(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C26 RID: 15398 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C27 RID: 15399 RVA: 0x00135825 File Offset: 0x00133A25
			// Note: this type is marked as 'beforefieldinit'.
			static ZeroVector_00003B69$BurstDirectCall()
			{
				Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C28 RID: 15400 RVA: 0x0013582C File Offset: 0x00133A2C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.ZeroVector_00003B69$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.ZeroVector$BurstManaged(L);
			}

			// Token: 0x040048F9 RID: 18681
			private static IntPtr Pointer;

			// Token: 0x040048FA RID: 18682
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009A2 RID: 2466
		// (Invoke) Token: 0x06003C2A RID: 15402
		public unsafe delegate int OneVector_00003B6A$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009A3 RID: 2467
		internal static class OneVector_00003B6A$BurstDirectCall
		{
			// Token: 0x06003C2D RID: 15405 RVA: 0x0013585D File Offset: 0x00133A5D
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.OneVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.OneVector_00003B6A$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C2E RID: 15406 RVA: 0x0013588C File Offset: 0x00133A8C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C2F RID: 15407 RVA: 0x001358A4 File Offset: 0x00133AA4
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.OneVector(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C30 RID: 15408 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C31 RID: 15409 RVA: 0x001358B5 File Offset: 0x00133AB5
			// Note: this type is marked as 'beforefieldinit'.
			static OneVector_00003B6A$BurstDirectCall()
			{
				Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C32 RID: 15410 RVA: 0x001358BC File Offset: 0x00133ABC
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.OneVector_00003B6A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.OneVector$BurstManaged(L);
			}

			// Token: 0x040048FB RID: 18683
			private static IntPtr Pointer;

			// Token: 0x040048FC RID: 18684
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009A4 RID: 2468
		// (Invoke) Token: 0x06003C34 RID: 15412
		public unsafe delegate int NearlyEqual_00003B6B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009A5 RID: 2469
		internal static class NearlyEqual_00003B6B$BurstDirectCall
		{
			// Token: 0x06003C37 RID: 15415 RVA: 0x001358ED File Offset: 0x00133AED
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.NearlyEqual$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.NearlyEqual_00003B6B$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C38 RID: 15416 RVA: 0x0013591C File Offset: 0x00133B1C
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C39 RID: 15417 RVA: 0x00135934 File Offset: 0x00133B34
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.NearlyEqual(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C3A RID: 15418 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C3B RID: 15419 RVA: 0x00135945 File Offset: 0x00133B45
			// Note: this type is marked as 'beforefieldinit'.
			static NearlyEqual_00003B6B$BurstDirectCall()
			{
				Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C3C RID: 15420 RVA: 0x0013594C File Offset: 0x00133B4C
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.NearlyEqual_00003B6B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.NearlyEqual$BurstManaged(L);
			}

			// Token: 0x040048FD RID: 18685
			private static IntPtr Pointer;

			// Token: 0x040048FE RID: 18686
			private static IntPtr DeferredCompilation;
		}
	}

	// Token: 0x020009A6 RID: 2470
	[BurstCompile]
	public static class QuatFunctions
	{
		// Token: 0x06003C3D RID: 15421 RVA: 0x0013597D File Offset: 0x00133B7D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x00135985 File Offset: 0x00133B85
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x0013598D File Offset: 0x00133B8D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x00135998 File Offset: 0x00133B98
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Luau.lua_pushstring(L, quaternion.ToString());
			return 1;
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x001359D1 File Offset: 0x00133BD1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromEuler(lua_State* L)
		{
			return Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x001359D9 File Offset: 0x00133BD9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromDirection(lua_State* L)
		{
			return Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x001359E1 File Offset: 0x00133BE1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetUpVector(lua_State* L)
		{
			return Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x001359E9 File Offset: 0x00133BE9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Euler(lua_State* L)
		{
			return Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x001359F4 File Offset: 0x00133BF4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int New$BurstManaged(lua_State* L)
		{
			Quaternion* ptr = Luau.lua_class_push<Quaternion>(L, "Quat");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			ptr->w = (float)Luau.luaL_optnumber(L, 4, 0.0);
			return 1;
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x00135A70 File Offset: 0x00133C70
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Quaternion lhs = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion rhs = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Quaternion>(L, "Quat") = lhs * rhs;
			return 1;
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x00135AC8 File Offset: 0x00133CC8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Quaternion lhs = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion rhs = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			int num = (lhs == rhs) ? 1 : 0;
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x00135B18 File Offset: 0x00133D18
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int FromEuler$BurstManaged(lua_State* L)
		{
			float x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			float y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			float z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			Luau.lua_class_push<Quaternion>(L, "Quat")->eulerAngles = new Vector3(x, y, z);
			return 1;
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x00135B7C File Offset: 0x00133D7C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int FromDirection$BurstManaged(lua_State* L)
		{
			Vector3 lookRotation = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_class_push<Quaternion>(L, "Quat")->SetLookRotation(lookRotation);
			return 1;
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x00135BB8 File Offset: 0x00133DB8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetUpVector$BurstManaged(lua_State* L)
		{
			Quaternion rotation = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = rotation * Vector3.up;
			return 1;
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x00135C00 File Offset: 0x00133E00
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Euler$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion.eulerAngles;
			return 1;
		}

		// Token: 0x020009A7 RID: 2471
		// (Invoke) Token: 0x06003C4D RID: 15437
		public unsafe delegate int New_00003B6C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009A8 RID: 2472
		internal static class New_00003B6C$BurstDirectCall
		{
			// Token: 0x06003C50 RID: 15440 RVA: 0x00135C41 File Offset: 0x00133E41
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.New$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.New_00003B6C$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C51 RID: 15441 RVA: 0x00135C70 File Offset: 0x00133E70
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C52 RID: 15442 RVA: 0x00135C88 File Offset: 0x00133E88
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.New(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C53 RID: 15443 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C54 RID: 15444 RVA: 0x00135C99 File Offset: 0x00133E99
			// Note: this type is marked as 'beforefieldinit'.
			static New_00003B6C$BurstDirectCall()
			{
				Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C55 RID: 15445 RVA: 0x00135CA0 File Offset: 0x00133EA0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.New_00003B6C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.New$BurstManaged(L);
			}

			// Token: 0x040048FF RID: 18687
			private static IntPtr Pointer;

			// Token: 0x04004900 RID: 18688
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009A9 RID: 2473
		// (Invoke) Token: 0x06003C57 RID: 15447
		public unsafe delegate int Mul_00003B6D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009AA RID: 2474
		internal static class Mul_00003B6D$BurstDirectCall
		{
			// Token: 0x06003C5A RID: 15450 RVA: 0x00135CD1 File Offset: 0x00133ED1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Mul$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Mul_00003B6D$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C5B RID: 15451 RVA: 0x00135D00 File Offset: 0x00133F00
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C5C RID: 15452 RVA: 0x00135D18 File Offset: 0x00133F18
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Mul(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C5D RID: 15453 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C5E RID: 15454 RVA: 0x00135D29 File Offset: 0x00133F29
			// Note: this type is marked as 'beforefieldinit'.
			static Mul_00003B6D$BurstDirectCall()
			{
				Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C5F RID: 15455 RVA: 0x00135D30 File Offset: 0x00133F30
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Mul_00003B6D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Mul$BurstManaged(L);
			}

			// Token: 0x04004901 RID: 18689
			private static IntPtr Pointer;

			// Token: 0x04004902 RID: 18690
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009AB RID: 2475
		// (Invoke) Token: 0x06003C61 RID: 15457
		public unsafe delegate int Eq_00003B6E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009AC RID: 2476
		internal static class Eq_00003B6E$BurstDirectCall
		{
			// Token: 0x06003C64 RID: 15460 RVA: 0x00135D61 File Offset: 0x00133F61
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Eq$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Eq_00003B6E$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C65 RID: 15461 RVA: 0x00135D90 File Offset: 0x00133F90
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C66 RID: 15462 RVA: 0x00135DA8 File Offset: 0x00133FA8
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Eq(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C67 RID: 15463 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C68 RID: 15464 RVA: 0x00135DB9 File Offset: 0x00133FB9
			// Note: this type is marked as 'beforefieldinit'.
			static Eq_00003B6E$BurstDirectCall()
			{
				Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C69 RID: 15465 RVA: 0x00135DC0 File Offset: 0x00133FC0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Eq_00003B6E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Eq$BurstManaged(L);
			}

			// Token: 0x04004903 RID: 18691
			private static IntPtr Pointer;

			// Token: 0x04004904 RID: 18692
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009AD RID: 2477
		// (Invoke) Token: 0x06003C6B RID: 15467
		public unsafe delegate int FromEuler_00003B70$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009AE RID: 2478
		internal static class FromEuler_00003B70$BurstDirectCall
		{
			// Token: 0x06003C6E RID: 15470 RVA: 0x00135DF1 File Offset: 0x00133FF1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.FromEuler$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.FromEuler_00003B70$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C6F RID: 15471 RVA: 0x00135E20 File Offset: 0x00134020
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C70 RID: 15472 RVA: 0x00135E38 File Offset: 0x00134038
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.FromEuler(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C71 RID: 15473 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C72 RID: 15474 RVA: 0x00135E49 File Offset: 0x00134049
			// Note: this type is marked as 'beforefieldinit'.
			static FromEuler_00003B70$BurstDirectCall()
			{
				Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C73 RID: 15475 RVA: 0x00135E50 File Offset: 0x00134050
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromEuler_00003B70$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromEuler$BurstManaged(L);
			}

			// Token: 0x04004905 RID: 18693
			private static IntPtr Pointer;

			// Token: 0x04004906 RID: 18694
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009AF RID: 2479
		// (Invoke) Token: 0x06003C75 RID: 15477
		public unsafe delegate int FromDirection_00003B71$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009B0 RID: 2480
		internal static class FromDirection_00003B71$BurstDirectCall
		{
			// Token: 0x06003C78 RID: 15480 RVA: 0x00135E81 File Offset: 0x00134081
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.FromDirection$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.FromDirection_00003B71$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C79 RID: 15481 RVA: 0x00135EB0 File Offset: 0x001340B0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C7A RID: 15482 RVA: 0x00135EC8 File Offset: 0x001340C8
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.FromDirection(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C7B RID: 15483 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C7C RID: 15484 RVA: 0x00135ED9 File Offset: 0x001340D9
			// Note: this type is marked as 'beforefieldinit'.
			static FromDirection_00003B71$BurstDirectCall()
			{
				Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C7D RID: 15485 RVA: 0x00135EE0 File Offset: 0x001340E0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromDirection_00003B71$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromDirection$BurstManaged(L);
			}

			// Token: 0x04004907 RID: 18695
			private static IntPtr Pointer;

			// Token: 0x04004908 RID: 18696
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009B1 RID: 2481
		// (Invoke) Token: 0x06003C7F RID: 15487
		public unsafe delegate int GetUpVector_00003B72$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009B2 RID: 2482
		internal static class GetUpVector_00003B72$BurstDirectCall
		{
			// Token: 0x06003C82 RID: 15490 RVA: 0x00135F11 File Offset: 0x00134111
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.GetUpVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.GetUpVector_00003B72$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C83 RID: 15491 RVA: 0x00135F40 File Offset: 0x00134140
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C84 RID: 15492 RVA: 0x00135F58 File Offset: 0x00134158
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.GetUpVector(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C85 RID: 15493 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C86 RID: 15494 RVA: 0x00135F69 File Offset: 0x00134169
			// Note: this type is marked as 'beforefieldinit'.
			static GetUpVector_00003B72$BurstDirectCall()
			{
				Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C87 RID: 15495 RVA: 0x00135F70 File Offset: 0x00134170
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.GetUpVector_00003B72$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.GetUpVector$BurstManaged(L);
			}

			// Token: 0x04004909 RID: 18697
			private static IntPtr Pointer;

			// Token: 0x0400490A RID: 18698
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x020009B3 RID: 2483
		// (Invoke) Token: 0x06003C89 RID: 15497
		public unsafe delegate int Euler_00003B73$PostfixBurstDelegate(lua_State* L);

		// Token: 0x020009B4 RID: 2484
		internal static class Euler_00003B73$BurstDirectCall
		{
			// Token: 0x06003C8C RID: 15500 RVA: 0x00135FA1 File Offset: 0x001341A1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Euler$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Euler_00003B73$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Pointer;
			}

			// Token: 0x06003C8D RID: 15501 RVA: 0x00135FD0 File Offset: 0x001341D0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x06003C8E RID: 15502 RVA: 0x00135FE8 File Offset: 0x001341E8
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Euler(lua_State*)).MethodHandle);
			}

			// Token: 0x06003C8F RID: 15503 RVA: 0x000023F5 File Offset: 0x000005F5
			public static void Initialize()
			{
			}

			// Token: 0x06003C90 RID: 15504 RVA: 0x00135FF9 File Offset: 0x001341F9
			// Note: this type is marked as 'beforefieldinit'.
			static Euler_00003B73$BurstDirectCall()
			{
				Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.Constructor();
			}

			// Token: 0x06003C91 RID: 15505 RVA: 0x00136000 File Offset: 0x00134200
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Euler_00003B73$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Euler$BurstManaged(L);
			}

			// Token: 0x0400490B RID: 18699
			private static IntPtr Pointer;

			// Token: 0x0400490C RID: 18700
			private static IntPtr DeferredCompilation;
		}
	}

	// Token: 0x020009B5 RID: 2485
	public struct GorillaLocomotionSettings
	{
		// Token: 0x0400490D RID: 18701
		public float velocityLimit;

		// Token: 0x0400490E RID: 18702
		public float slideVelocityLimit;

		// Token: 0x0400490F RID: 18703
		public float maxJumpSpeed;

		// Token: 0x04004910 RID: 18704
		public float jumpMultiplier;
	}

	// Token: 0x020009B6 RID: 2486
	[BurstCompile]
	public struct PlayerInput
	{
		// Token: 0x04004911 RID: 18705
		public float leftXAxis;

		// Token: 0x04004912 RID: 18706
		[MarshalAs(UnmanagedType.U1)]
		public bool leftPrimaryButton;

		// Token: 0x04004913 RID: 18707
		public float rightXAxis;

		// Token: 0x04004914 RID: 18708
		[MarshalAs(UnmanagedType.U1)]
		public bool rightPrimaryButton;

		// Token: 0x04004915 RID: 18709
		public float leftYAxis;

		// Token: 0x04004916 RID: 18710
		[MarshalAs(UnmanagedType.U1)]
		public bool leftSecondaryButton;

		// Token: 0x04004917 RID: 18711
		public float rightYAxis;

		// Token: 0x04004918 RID: 18712
		[MarshalAs(UnmanagedType.U1)]
		public bool rightSecondaryButton;

		// Token: 0x04004919 RID: 18713
		public float leftTrigger;

		// Token: 0x0400491A RID: 18714
		public float rightTrigger;

		// Token: 0x0400491B RID: 18715
		public float leftGrip;

		// Token: 0x0400491C RID: 18716
		public float rightGrip;
	}

	// Token: 0x020009B7 RID: 2487
	public static class JSON
	{
		// Token: 0x06003C92 RID: 15506 RVA: 0x00136034 File Offset: 0x00134234
		public unsafe static Dictionary<object, object> ConsumeTable(lua_State* L, int tableIndex)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			Luau.lua_pushnil(L);
			if (tableIndex < 0)
			{
				tableIndex--;
			}
			while (Luau.lua_next(L, tableIndex) != 0)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				Luau.lua_Types lua_Types2 = (Luau.lua_Types)Luau.lua_type(L, -2);
				object key;
				if (lua_Types2 == Luau.lua_Types.LUA_TSTRING)
				{
					key = new string(Luau.lua_tostring(L, -2));
				}
				else
				{
					if (lua_Types2 != Luau.lua_Types.LUA_TNUMBER)
					{
						FixedString64Bytes fixedString64Bytes = "Invalid key in table, key must be a string or a number";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return null;
					}
					key = Luau.lua_tonumber(L, -2);
				}
				switch (lua_Types)
				{
				case Luau.lua_Types.LUA_TBOOLEAN:
					dictionary.Add(key, Luau.lua_toboolean(L, -1) == 1);
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TNUMBER:
					dictionary.Add(key, Luau.luaL_checknumber(L, -1));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TSTRING:
					dictionary.Add(key, new string(Luau.lua_tostring(L, -1)));
					Luau.lua_pop(L, 1);
					continue;
				case Luau.lua_Types.LUA_TTABLE:
				case Luau.lua_Types.LUA_TUSERDATA:
					if (Luau.luaL_getmetafield(L, -1, "metahash") == 1)
					{
						BurstClassInfo.ClassInfo classInfo;
						if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
						{
							FixedString64Bytes fixedString64Bytes2 = "\"Internal Class Info Error No Metatable Found\"";
							Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes2) + 2));
							return null;
						}
						Luau.lua_pop(L, 1);
						FixedString32Bytes fixedString32Bytes = "Vec3";
						if (classInfo.Name == fixedString32Bytes)
						{
							dictionary.Add(key, *Luau.lua_class_get<Vector3>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						fixedString32Bytes = "Quat";
						if (classInfo.Name == fixedString32Bytes)
						{
							dictionary.Add(key, *Luau.lua_class_get<Quaternion>(L, -1));
							Luau.lua_pop(L, 1);
							continue;
						}
						FixedString32Bytes fixedString32Bytes2 = "Invalid type in table";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
						return null;
					}
					else
					{
						object obj = Bindings.JSON.ConsumeTable(L, -1);
						Luau.lua_pop(L, 1);
						if (obj != null)
						{
							dictionary.Add(key, obj);
							continue;
						}
						return null;
					}
					break;
				}
				FixedString32Bytes fixedString32Bytes3 = "Unknown type in table";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return null;
			}
			return dictionary;
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x0013627C File Offset: 0x0013447C
		private static int ParseStrictInt(string input)
		{
			if (string.IsNullOrEmpty(input) || input != input.Trim())
			{
				return -1;
			}
			int result;
			if (!int.TryParse(input, out result))
			{
				return -1;
			}
			return result;
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x001362B0 File Offset: 0x001344B0
		private static bool CompareKeys(JObject obj, HashSet<string> set)
		{
			HashSet<string> equals = new HashSet<string>(from p in obj.Properties()
			select p.Name);
			return set.SetEquals(equals);
		}

		// Token: 0x06003C95 RID: 15509 RVA: 0x001362F4 File Offset: 0x001344F4
		public unsafe static bool PushTable(lua_State* L, JObject table)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, JToken> keyValuePair in table)
			{
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					int num = Bindings.JSON.ParseStrictInt(keyValuePair.Key);
					if (num == -1)
					{
						Luau.lua_pushstring(L, keyValuePair.Key);
					}
					if (keyValuePair.Value is JObject)
					{
						if (Bindings.JSON.CompareKeys((JObject)keyValuePair.Value, new HashSet<string>
						{
							"x",
							"y",
							"z"
						}))
						{
							JObject jobject = keyValuePair.Value as JObject;
							float x = jobject["x"].ToObject<float>();
							float y = jobject["y"].ToObject<float>();
							float z = jobject["z"].ToObject<float>();
							Vector3 vector = new Vector3(x, y, z);
							*Luau.lua_class_push<Vector3>(L) = vector;
						}
						else if (Bindings.JSON.CompareKeys((JObject)keyValuePair.Value, new HashSet<string>
						{
							"x",
							"y",
							"z",
							"w"
						}))
						{
							JObject jobject2 = keyValuePair.Value as JObject;
							float x2 = jobject2["x"].ToObject<float>();
							float y2 = jobject2["y"].ToObject<float>();
							float z2 = jobject2["z"].ToObject<float>();
							float w = jobject2["w"].ToObject<float>();
							Quaternion quaternion = new Quaternion(x2, y2, z2, w);
							*Luau.lua_class_push<Quaternion>(L) = quaternion;
						}
						else
						{
							Bindings.JSON.PushTable(L, (JObject)keyValuePair.Value);
						}
					}
					else if (keyValuePair.Value is JValue)
					{
						JTokenType type = keyValuePair.Value.Type;
						if (type == JTokenType.Integer)
						{
							Luau.lua_pushnumber(L, (double)keyValuePair.Value.ToObject<int>());
						}
						else if (type == JTokenType.Boolean)
						{
							Luau.lua_pushboolean(L, keyValuePair.Value.ToObject<bool>() ? 1 : 0);
						}
						else if (type == JTokenType.Float)
						{
							Luau.lua_pushnumber(L, keyValuePair.Value.ToObject<double>());
						}
						else
						{
							if (type != JTokenType.String)
							{
								continue;
							}
							Luau.lua_pushstring(L, keyValuePair.Value.ToString());
						}
					}
					if (num == -1)
					{
						Luau.lua_rawset(L, -3);
					}
					else
					{
						Luau.lua_rawseti(L, -2, num);
					}
				}
			}
			return true;
		}

		// Token: 0x06003C96 RID: 15510 RVA: 0x001365A4 File Offset: 0x001347A4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataSave(lua_State* L)
		{
			int result;
			try
			{
				string text = JsonConvert.SerializeObject(Bindings.JSON.ConsumeTable(L, 1), Formatting.Indented);
				if (text.Length > 10000)
				{
					Luau.luaL_errorL(L, "Save exceeds 10000 bytes", Array.Empty<string>());
					result = 0;
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					File.WriteAllText(Path.Join(directoryInfo.FullName, "luau.json"), text);
					result = 0;
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x0013666C File Offset: 0x0013486C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int DataLoad(lua_State* L)
		{
			int result;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(Bindings.JSON.ModIODirectory, "saves", CustomMapLoader.LoadedMapModId.ToString()));
				if (!directoryInfo.Exists)
				{
					Luau.lua_createtable(L, 0, 0);
					result = 1;
				}
				else
				{
					FileInfo[] files = directoryInfo.GetFiles("luau.json");
					if (files.Length == 0)
					{
						Luau.lua_createtable(L, 0, 0);
						result = 1;
					}
					else
					{
						JObject table = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(files[0].FullName));
						if (Bindings.JSON.PushTable(L, table))
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
					}
				}
			}
			catch
			{
				Luau.luaL_errorL(L, "Error while loading data", Array.Empty<string>());
				result = 0;
			}
			return result;
		}

		// Token: 0x0400491D RID: 18717
		private static string ModIODirectory = Path.Join(Path.Join(Application.persistentDataPath, "mod.io", "06657"), "data");
	}
}
