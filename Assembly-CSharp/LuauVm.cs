using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x020009D7 RID: 2519
public class LuauVm : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x06003D45 RID: 15685 RVA: 0x00138174 File Offset: 0x00136374
	private void Update()
	{
		foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
		{
			if (!luauScriptRunner.Tick(Time.deltaTime))
			{
				LuauHud.Instance.LuauLog(luauScriptRunner.ScriptName + " errored out");
				LuauScriptRunner.ScriptRunners.Remove(luauScriptRunner);
				break;
			}
		}
	}

	// Token: 0x06003D46 RID: 15686 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x001381F4 File Offset: 0x001363F4
	public unsafe void OnEvent(EventData eventData)
	{
		if (eventData.Code != 180)
		{
			return;
		}
		float num = 0f;
		LuauVm.callTimers.TryGetValue(eventData.Sender, out num);
		if (num < Time.time - 1f)
		{
			num = Time.time - 1f;
		}
		num += 1f / LuauVm.callCount;
		LuauVm.callTimers[eventData.Sender] = num;
		if (num > Time.time)
		{
			return;
		}
		try
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				foreach (LuauScriptRunner luauScriptRunner in LuauScriptRunner.ScriptRunners)
				{
					if (luauScriptRunner.ShouldTick)
					{
						lua_State* l = luauScriptRunner.L;
						Luau.lua_getfield(l, -10002, "onEvent");
						if (Luau.lua_type(l, -1) == 7)
						{
							object[] array = (object[])eventData.CustomData;
							if (array.Length > 20)
							{
								Luau.lua_pop(l, 1);
								break;
							}
							string text = array[0] as string;
							if (text == null)
							{
								Luau.lua_pop(l, 1);
								break;
							}
							if (string.IsNullOrEmpty(text))
							{
								Luau.lua_pop(l, 1);
								break;
							}
							if (text.Length > 30)
							{
								Luau.lua_pop(l, 1);
								break;
							}
							Luau.lua_pushstring(l, (string)array[0]);
							Luau.lua_createtable(l, array.Length, 0);
							for (int i = 1; i < array.Length; i++)
							{
								if (!luauScriptRunner.ShouldTick)
								{
									return;
								}
								object obj = array[i];
								if (obj.IsType<double>())
								{
									if (double.IsFinite((double)obj))
									{
										Luau.lua_pushnumber(l, (double)obj);
										Luau.lua_rawseti(l, -2, i);
									}
								}
								else if (obj.IsType<bool>())
								{
									Luau.lua_pushboolean(l, (int)obj);
									Luau.lua_rawseti(l, -2, i);
								}
								else if (obj.IsType<Vector3>())
								{
									Vector3 vector = (Vector3)obj;
									vector.ClampMagnitudeSafe(10000000f);
									*Luau.lua_class_push<Vector3>(l, "Vec3") = vector;
									Luau.lua_rawseti(l, -2, i);
								}
								else if (obj.IsType<Quaternion>())
								{
									Quaternion quaternion = (Quaternion)obj;
									if (float.IsFinite(quaternion.x) && float.IsFinite(quaternion.y) && float.IsFinite(quaternion.z) && float.IsFinite(quaternion.w))
									{
										*Luau.lua_class_push<Quaternion>(l, "Quat") = quaternion;
										Luau.lua_rawseti(l, -2, i);
									}
								}
								else if (obj.IsType<Player>())
								{
									int actorNumber = ((Player)obj).ActorNumber;
									IntPtr ptr;
									if (Bindings.LuauPlayerList.TryGetValue(actorNumber, out ptr))
									{
										Luau.lua_class_push(l, "Player", ptr);
										Luau.lua_rawseti(l, -2, i);
									}
									else
									{
										NetPlayer netPlayer = (NetPlayer)obj;
										if (netPlayer == null)
										{
											Luau.lua_pushnil(l);
											Luau.lua_rawseti(l, -2, i);
										}
										else
										{
											Bindings.LuauPlayer* ptr2 = Luau.lua_class_push<Bindings.LuauPlayer>(l);
											ptr2->PlayerID = netPlayer.ActorNumber;
											ptr2->PlayerName = netPlayer.SanitizedNickName;
											ptr2->PlayerMaterial = 0;
											ptr2->IsMasterClient = netPlayer.IsMasterClient;
											RigContainer rigContainer;
											VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer);
											VRRig rig = rigContainer.Rig;
											Bindings.LuauVRRigList[netPlayer.ActorNumber] = rig;
											Bindings.PlayerFunctions.UpdatePlayer(l, rig, ptr2);
											Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr2);
											Luau.lua_rawseti(l, -2, i);
										}
									}
								}
								else if (obj.IsType<Bindings.LuauAIAgent>())
								{
									int entityID = ((Bindings.LuauAIAgent)obj).EntityID;
									IntPtr value;
									if (Bindings.LuauAIAgentList.TryGetValue(entityID, out value))
									{
										Luau.lua_class_push<Bindings.LuauAIAgent>(l, (Bindings.LuauAIAgent*)((void*)value));
										Luau.lua_rawseti(l, -2, i);
									}
									else
									{
										bool flag = false;
										if (Bindings.LuauAIAgentList.Count == Constants.aiAgentLimit)
										{
											Debug.Log("[LuauVM::OnEvent] Custom Map AI Agent limit has already been reached!");
										}
										else
										{
											GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
											if (entityManager.IsNotNull())
											{
												GameEntityId entityIdFromNetId = entityManager.GetEntityIdFromNetId(entityID);
												GameEntity gameEntity = entityManager.GetGameEntity(entityIdFromNetId);
												if (gameEntity.IsNotNull() && gameEntity.gameObject.IsNotNull())
												{
													GameAgent component = gameEntity.gameObject.GetComponent<GameAgent>();
													if (component != null)
													{
														Bindings.LuauAIAgent* ptr3 = Luau.lua_class_push<Bindings.LuauAIAgent>(l);
														Bindings.AIAgentFunctions.UpdateAIAgent(component, ptr3);
														Bindings.LuauAIAgentList[entityID] = (IntPtr)((void*)ptr3);
														Luau.lua_rawseti(l, -2, i);
														flag = true;
													}
												}
											}
										}
										if (!flag)
										{
											Luau.lua_pushnil(l);
											Luau.lua_rawseti(l, -2, i);
										}
									}
								}
								else if (obj == null)
								{
									Luau.lua_pushnil(l);
									Luau.lua_rawseti(l, -2, i);
								}
							}
							Luau.lua_pcall(l, 2, 0, 0);
						}
						else
						{
							Luau.lua_pop(l, 1);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x0013871C File Offset: 0x0013691C
	protected override void Finalize()
	{
		try
		{
			foreach (GCHandle gchandle in LuauVm.Handles)
			{
				gchandle.Free();
			}
			if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
			{
				foreach (KVPair<int, BurstClassInfo.ClassInfo> kvpair in BurstClassInfo.ClassList.InfoFields.Data)
				{
					if (kvpair.Value.FieldList.IsCreated)
					{
						kvpair.Value.FieldList.Dispose();
					}
				}
				BurstClassInfo.ClassList.InfoFields.Data.Dispose();
			}
		}
		catch (ObjectDisposedException message)
		{
			Debug.Log(message);
		}
		finally
		{
			base.Finalize();
		}
	}

	// Token: 0x0400497B RID: 18811
	public static List<object> ClassBuilders = new List<object>();

	// Token: 0x0400497C RID: 18812
	public static List<GCHandle> Handles = new List<GCHandle>();

	// Token: 0x0400497D RID: 18813
	private static Dictionary<int, float> callTimers = new Dictionary<int, float>();

	// Token: 0x0400497E RID: 18814
	private static float callCount = 25f;
}
