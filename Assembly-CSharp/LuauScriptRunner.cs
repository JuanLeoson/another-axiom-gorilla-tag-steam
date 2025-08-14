using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020009D8 RID: 2520
public class LuauScriptRunner
{
	// Token: 0x06003D4C RID: 15692 RVA: 0x00138848 File Offset: 0x00136A48
	public unsafe static bool ErrorCheck(lua_State* L, int status)
	{
		if (status == 2)
		{
			sbyte* value = Luau.lua_tostring(L, -1);
			LuauHud.Instance.LuauLog(new string(value));
			sbyte* value2 = (sbyte*)Luau.lua_debugtrace(L);
			LuauHud.Instance.LuauLog(new string(value2));
			Luau.lua_close(L);
			return true;
		}
		return false;
	}

	// Token: 0x06003D4D RID: 15693 RVA: 0x00138894 File Offset: 0x00136A94
	public bool Tick(float deltaTime)
	{
		if (!this.ShouldTick)
		{
			return false;
		}
		Luau.lua_getfield(this.L, -10002, "tick");
		if (Luau.lua_type(this.L, -1) == 7)
		{
			this.preTickCallback(this.L);
			Luau.lua_pushnumber(this.L, (double)deltaTime);
			int status = Luau.lua_pcall(this.L, 1, 0, 0);
			this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
			if (this.ShouldTick)
			{
				this.postTickCallback(this.L);
				Luau.lua_gc(this.L, 5, 2);
			}
			return this.ShouldTick;
		}
		Luau.lua_pop(this.L, 1);
		return false;
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x00138950 File Offset: 0x00136B50
	public unsafe LuauScriptRunner(string script, string name, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction preTick = null, [CanBeNull] lua_CFunction postTick = null)
	{
		this.Script = script;
		this.ScriptName = name;
		this.L = Luau.luaL_newstate();
		LuauScriptRunner.ScriptRunners.Add(this);
		Luau.luaL_openlibs(this.L);
		Bindings.Vec3Builder(this.L);
		Bindings.QuatBuilder(this.L);
		if (bindings != null)
		{
			bindings(this.L);
		}
		this.postTickCallback = postTick;
		this.preTickCallback = preTick;
		UIntPtr size = (UIntPtr)((IntPtr)0);
		Luau.lua_register(this.L, new lua_CFunction(Luau.lua_print), "print");
		byte[] bytes = Encoding.UTF8.GetBytes(script);
		sbyte* data = Luau.luau_compile(script, (UIntPtr)((IntPtr)bytes.Length), null, &size);
		Luau.luau_load(this.L, name, data, size, 0);
		int status = Luau.lua_resume(this.L, null, 0);
		this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, status);
	}

	// Token: 0x06003D4F RID: 15695 RVA: 0x00138A37 File Offset: 0x00136C37
	public LuauScriptRunner FromFile(string filePath, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction tick = null)
	{
		return new LuauScriptRunner(File.ReadAllText(Path.Join(Application.persistentDataPath, "Scripts", filePath)), filePath, bindings, tick, null);
	}

	// Token: 0x06003D50 RID: 15696 RVA: 0x00138A68 File Offset: 0x00136C68
	~LuauScriptRunner()
	{
		LuauVm.ClassBuilders.Clear();
		Bindings.LuauPlayerList.Clear();
		Bindings.LuauGameObjectList.Clear();
		Bindings.LuauGameObjectStates.Clear();
		Bindings.LuauVRRigList.Clear();
		Bindings.LuauAIAgentList.Clear();
		ReflectionMetaNames.ReflectedNames.Clear();
		if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			BurstClassInfo.ClassList.InfoFields.Data.Clear();
		}
	}

	// Token: 0x0400497F RID: 18815
	public static List<LuauScriptRunner> ScriptRunners = new List<LuauScriptRunner>();

	// Token: 0x04004980 RID: 18816
	public bool ShouldTick;

	// Token: 0x04004981 RID: 18817
	private lua_CFunction postTickCallback;

	// Token: 0x04004982 RID: 18818
	private lua_CFunction preTickCallback;

	// Token: 0x04004983 RID: 18819
	public string ScriptName;

	// Token: 0x04004984 RID: 18820
	public string Script;

	// Token: 0x04004985 RID: 18821
	public unsafe lua_State* L;
}
