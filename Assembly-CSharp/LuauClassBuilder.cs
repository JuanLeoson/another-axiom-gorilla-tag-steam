using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020009C8 RID: 2504
public class LuauClassBuilder<[IsUnmanaged] T> where T : struct, ValueType
{
	// Token: 0x06003CCA RID: 15562 RVA: 0x00137270 File Offset: 0x00135470
	public LuauClassBuilder(string className)
	{
		this._className = className;
		this._classType = typeof(T);
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x001372E8 File Offset: 0x001354E8
	public LuauClassBuilder<T> AddField(string luaName, string fieldName = null)
	{
		if (fieldName == null)
		{
			fieldName = luaName;
		}
		FieldInfo field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
		if (field == null)
		{
			throw new ArgumentException(string.Concat(new string[]
			{
				"Property ",
				fieldName,
				" does not exist on type ",
				typeof(T).Name,
				"."
			}));
		}
		this._classFields.TryAdd(LuaHashing.ByteHash(luaName), field);
		return this;
	}

	// Token: 0x06003CCC RID: 15564 RVA: 0x0013736A File Offset: 0x0013556A
	public LuauClassBuilder<T> AddStaticFunction(string luaName, lua_CFunction function)
	{
		this._staticFunctions.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06003CCD RID: 15565 RVA: 0x0013737B File Offset: 0x0013557B
	public LuauClassBuilder<T> AddStaticFunction(string luaName, FunctionPointer<lua_CFunction> function)
	{
		this._staticFunctionPtrs.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06003CCE RID: 15566 RVA: 0x0013738C File Offset: 0x0013558C
	public LuauClassBuilder<T> AddProperty(string luaName, lua_CFunction function)
	{
		this._properties.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x0013739D File Offset: 0x0013559D
	public LuauClassBuilder<T> AddProperty(string luaName, FunctionPointer<lua_CFunction> function)
	{
		this._propertyPtrs.TryAdd(luaName, function);
		return this;
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x001373AE File Offset: 0x001355AE
	public LuauClassBuilder<T> AddFunction(string luaName, lua_CFunction function)
	{
		if (luaName.StartsWith("__"))
		{
			this._staticFunctions.TryAdd(luaName, function);
		}
		this._functions.TryAdd(LuaHashing.ByteHash(luaName), function);
		return this;
	}

	// Token: 0x06003CD1 RID: 15569 RVA: 0x001373DF File Offset: 0x001355DF
	public LuauClassBuilder<T> AddFunction(string luaName, FunctionPointer<lua_CFunction> function)
	{
		if (luaName.StartsWith("__"))
		{
			this._staticFunctionPtrs.TryAdd(luaName, function);
		}
		this._functionPtrs.TryAdd(LuaHashing.ByteHash(luaName), function);
		return this;
	}

	// Token: 0x06003CD2 RID: 15570 RVA: 0x00137410 File Offset: 0x00135610
	public unsafe LuauClassBuilder<T> Build(lua_State* L, bool global)
	{
		BurstClassInfo.NewClass<T>(this._className, this._classFields, this._functions, this._functionPtrs);
		Luau.luaL_newmetatable(L, this._className);
		FunctionPointer<lua_CFunction> fn = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.Index));
		Luau.lua_pushcfunction(L, fn, null);
		Luau.lua_setfield(L, -2, "__index");
		FunctionPointer<lua_CFunction> fn2 = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.NameCall));
		Luau.lua_pushcfunction(L, fn2, null);
		Luau.lua_setfield(L, -2, "__namecall");
		FunctionPointer<lua_CFunction> fn3 = BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(BurstClassInfo.NewIndex));
		Luau.lua_pushcfunction(L, fn3, null);
		Luau.lua_setfield(L, -2, "__newindex");
		foreach (KeyValuePair<string, lua_CFunction> keyValuePair in this._staticFunctions)
		{
			Luau.lua_pushcfunction(L, keyValuePair.Value, keyValuePair.Key);
			Luau.lua_setfield(L, -2, keyValuePair.Key);
		}
		foreach (KeyValuePair<string, FunctionPointer<lua_CFunction>> keyValuePair2 in this._staticFunctionPtrs)
		{
			Luau.lua_pushcfunction(L, keyValuePair2.Value, keyValuePair2.Key);
			Luau.lua_setfield(L, -2, keyValuePair2.Key);
		}
		FixedString32Bytes fixedString32Bytes = "metahash";
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2;
		Luau.lua_pushnumber(L, (double)LuaHashing.ByteHash(this._className));
		Luau.lua_setfield(L, -2, k);
		Luau.lua_setreadonly(L, -1, 1);
		Luau.lua_pop(L, 1);
		if (global)
		{
			Luau.lua_createtable(L, 0, 0);
			foreach (KeyValuePair<string, lua_CFunction> keyValuePair3 in this._staticFunctions)
			{
				Luau.lua_pushcfunction(L, keyValuePair3.Value, keyValuePair3.Key);
				Luau.lua_setfield(L, -2, keyValuePair3.Key);
			}
			foreach (KeyValuePair<string, FunctionPointer<lua_CFunction>> keyValuePair4 in this._staticFunctionPtrs)
			{
				Luau.lua_pushcfunction(L, keyValuePair4.Value, keyValuePair4.Key);
				Luau.lua_setfield(L, -2, keyValuePair4.Key);
			}
			Luau.lua_pushnumber(L, (double)LuaHashing.ByteHash(this._className));
			Luau.lua_setfield(L, -2, k);
			Luau.luaL_getmetatable(L, this._className);
			Luau.lua_setmetatable(L, -2);
			Luau.lua_setglobal(L, this._className);
		}
		return this;
	}

	// Token: 0x0400493D RID: 18749
	private string _className;

	// Token: 0x0400493E RID: 18750
	private Type _classType;

	// Token: 0x0400493F RID: 18751
	private Dictionary<string, lua_CFunction> _staticFunctions = new Dictionary<string, lua_CFunction>();

	// Token: 0x04004940 RID: 18752
	private Dictionary<string, FunctionPointer<lua_CFunction>> _staticFunctionPtrs = new Dictionary<string, FunctionPointer<lua_CFunction>>();

	// Token: 0x04004941 RID: 18753
	private Dictionary<int, FieldInfo> _classFields = new Dictionary<int, FieldInfo>();

	// Token: 0x04004942 RID: 18754
	private Dictionary<string, lua_CFunction> _properties = new Dictionary<string, lua_CFunction>();

	// Token: 0x04004943 RID: 18755
	private Dictionary<string, FunctionPointer<lua_CFunction>> _propertyPtrs = new Dictionary<string, FunctionPointer<lua_CFunction>>();

	// Token: 0x04004944 RID: 18756
	private Dictionary<int, lua_CFunction> _functions = new Dictionary<int, lua_CFunction>();

	// Token: 0x04004945 RID: 18757
	private Dictionary<int, FunctionPointer<lua_CFunction>> _functionPtrs = new Dictionary<int, FunctionPointer<lua_CFunction>>();
}
