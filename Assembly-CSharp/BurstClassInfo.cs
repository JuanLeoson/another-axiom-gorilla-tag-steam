using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020009BA RID: 2490
[BurstCompile]
public static class BurstClassInfo
{
	// Token: 0x06003C9D RID: 15517 RVA: 0x0013678C File Offset: 0x0013498C
	public unsafe static void NewClass<T>(string className, Dictionary<int, FieldInfo> fieldList, Dictionary<int, lua_CFunction> functionList, Dictionary<int, FunctionPointer<lua_CFunction>> functionPtrList)
	{
		if (!BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			*BurstClassInfo.ClassList.InfoFields.Data = new NativeHashMap<int, BurstClassInfo.ClassInfo>(20, Allocator.Persistent);
		}
		BurstClassInfo.ClassList.MetatableNames<T>.Name = className;
		ReflectionMetaNames.ReflectedNames.TryAdd(typeof(T), className);
		BurstClassInfo.ClassInfo classInfo = default(BurstClassInfo.ClassInfo);
		classInfo.NameHash = LuaHashing.ByteHash(className);
		if (className.Length > 30)
		{
			throw new Exception("Name to long");
		}
		classInfo.Name = className;
		classInfo.FieldList = new NativeHashMap<int, BurstClassInfo.BurstFieldInfo>(fieldList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, FieldInfo> keyValuePair in fieldList)
		{
			BurstClassInfo.BurstFieldInfo item = default(BurstClassInfo.BurstFieldInfo);
			item.NameHash = keyValuePair.Key;
			item.Name = keyValuePair.Value.Name;
			item.Offset = (int)Marshal.OffsetOf<T>(keyValuePair.Value.Name);
			Type fieldType = keyValuePair.Value.FieldType;
			if (fieldType == typeof(float))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Float;
			}
			else if (fieldType == typeof(int))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Int;
			}
			else if (fieldType == typeof(double))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Double;
			}
			else if (fieldType == typeof(bool))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Bool;
			}
			else if (fieldType == typeof(FixedString32Bytes))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.String;
			}
			else if (!fieldType.IsPrimitive)
			{
				item.FieldType = BurstClassInfo.EFieldTypes.LightUserData;
				ReflectionMetaNames.ReflectedNames.TryGetValue(fieldType, out item.MetatableName);
			}
			item.Size = Marshal.SizeOf(fieldType);
			classInfo.FieldList.TryAdd(keyValuePair.Key, item);
		}
		classInfo.FunctionList = new NativeHashMap<int, IntPtr>(functionList.Count + functionPtrList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, lua_CFunction> keyValuePair2 in functionList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair2.Key, Marshal.GetFunctionPointerForDelegate<lua_CFunction>(keyValuePair2.Value));
		}
		foreach (KeyValuePair<int, FunctionPointer<lua_CFunction>> keyValuePair3 in functionPtrList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair3.Key, keyValuePair3.Value.Value);
		}
		BurstClassInfo.ClassList.InfoFields.Data.Add(classInfo.NameHash, classInfo);
	}

	// Token: 0x06003C9E RID: 15518 RVA: 0x00136AC0 File Offset: 0x00134CC0
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int Index(lua_State* L)
	{
		return BurstClassInfo.Index_00003B80$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003C9F RID: 15519 RVA: 0x00136AC8 File Offset: 0x00134CC8
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int NewIndex(lua_State* L)
	{
		return BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x00136AD0 File Offset: 0x00134CD0
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int NameCall(lua_State* L)
	{
		return BurstClassInfo.NameCall_00003B82$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x00136AEC File Offset: 0x00134CEC
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Index$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __index\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr intPtr = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				Luau.lua_pushnumber(L, (double)(*(float*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Int:
				Luau.lua_pushnumber(L, (double)(*(int*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Double:
				Luau.lua_pushnumber(L, *(double*)((void*)intPtr));
				return 1;
			case BurstClassInfo.EFieldTypes.Bool:
				Luau.lua_pushboolean(L, (*(byte*)((void*)intPtr) != 0) ? 1 : 0);
				return 1;
			case BurstClassInfo.EFieldTypes.String:
				Luau.lua_pushstring(L, (byte*)((void*)intPtr) + 2);
				return 1;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Luau.lua_class_push(L, burstFieldInfo.MetatableName, intPtr);
				return 1;
			}
		}
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> fn = new FunctionPointer<lua_CFunction>(ptr);
			FixedString32Bytes fixedString32Bytes3 = "";
			Luau.lua_pushcclosurek(L, fn, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2, 0, null);
			return 1;
		}
		FixedString32Bytes fixedString32Bytes4 = "\"Unknown Type?\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes4) + 2));
		return 0;
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x00136CE0 File Offset: 0x00134EE0
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int NewIndex$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __newindex\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr value = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				*(float*)((void*)value) = (float)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Int:
				*(int*)((void*)value) = (int)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Double:
				*(double*)((void*)value) = Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Bool:
				*(byte*)((void*)value) = ((Luau.lua_toboolean(L, 3) != 0) ? 1 : 0);
				return 0;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Buffer.MemoryCopy((void*)((IntPtr)((void*)Luau.lua_class_get(L, 3, burstFieldInfo.MetatableName))), (void*)value, (long)burstFieldInfo.Size, (long)burstFieldInfo.Size);
				return 0;
			}
		}
		FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
		return 0;
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x00136EB0 File Offset: 0x001350B0
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int NameCall$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		int key = LuaHashing.ByteHash(Luau.lua_namecallatom(L, null));
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(ptr);
			return functionPointer.Invoke(L);
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Function not found in function list\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return 0;
	}

	// Token: 0x04004921 RID: 18721
	private static readonly FixedString32Bytes _k_metatableLookup = "metahash";

	// Token: 0x020009BB RID: 2491
	public enum EFieldTypes
	{
		// Token: 0x04004923 RID: 18723
		Float,
		// Token: 0x04004924 RID: 18724
		Int,
		// Token: 0x04004925 RID: 18725
		Double,
		// Token: 0x04004926 RID: 18726
		Bool,
		// Token: 0x04004927 RID: 18727
		String,
		// Token: 0x04004928 RID: 18728
		LightUserData
	}

	// Token: 0x020009BC RID: 2492
	[BurstCompile]
	public struct BurstFieldInfo
	{
		// Token: 0x04004929 RID: 18729
		public int NameHash;

		// Token: 0x0400492A RID: 18730
		public FixedString32Bytes Name;

		// Token: 0x0400492B RID: 18731
		public FixedString32Bytes MetatableName;

		// Token: 0x0400492C RID: 18732
		public int Offset;

		// Token: 0x0400492D RID: 18733
		public BurstClassInfo.EFieldTypes FieldType;

		// Token: 0x0400492E RID: 18734
		public int Size;
	}

	// Token: 0x020009BD RID: 2493
	[BurstCompile]
	public struct ClassInfo
	{
		// Token: 0x0400492F RID: 18735
		public int NameHash;

		// Token: 0x04004930 RID: 18736
		public FixedString32Bytes Name;

		// Token: 0x04004931 RID: 18737
		public NativeHashMap<int, BurstClassInfo.BurstFieldInfo> FieldList;

		// Token: 0x04004932 RID: 18738
		public NativeHashMap<int, IntPtr> FunctionList;
	}

	// Token: 0x020009BE RID: 2494
	public abstract class ClassList
	{
		// Token: 0x04004933 RID: 18739
		public static readonly SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>> InfoFields = SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>>.GetOrCreateUnsafe(0U, -7258312696341931442L, -7445903157129162016L);

		// Token: 0x020009BF RID: 2495
		private class FieldKey
		{
		}

		// Token: 0x020009C0 RID: 2496
		public static class MetatableNames<T>
		{
			// Token: 0x04004934 RID: 18740
			public static FixedString32Bytes Name;
		}
	}

	// Token: 0x020009C1 RID: 2497
	// (Invoke) Token: 0x06003CA9 RID: 15529
	public unsafe delegate int Index_00003B80$PostfixBurstDelegate(lua_State* L);

	// Token: 0x020009C2 RID: 2498
	internal static class Index_00003B80$BurstDirectCall
	{
		// Token: 0x06003CAC RID: 15532 RVA: 0x00136F86 File Offset: 0x00135186
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.Index_00003B80$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.Index_00003B80$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.Index_00003B80$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.Index$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.Index_00003B80$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.Index_00003B80$BurstDirectCall.Pointer;
		}

		// Token: 0x06003CAD RID: 15533 RVA: 0x00136FB4 File Offset: 0x001351B4
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.Index_00003B80$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x00136FCC File Offset: 0x001351CC
		public unsafe static void Constructor()
		{
			BurstClassInfo.Index_00003B80$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.Index(lua_State*)).MethodHandle);
		}

		// Token: 0x06003CAF RID: 15535 RVA: 0x000023F5 File Offset: 0x000005F5
		public static void Initialize()
		{
		}

		// Token: 0x06003CB0 RID: 15536 RVA: 0x00136FDD File Offset: 0x001351DD
		// Note: this type is marked as 'beforefieldinit'.
		static Index_00003B80$BurstDirectCall()
		{
			BurstClassInfo.Index_00003B80$BurstDirectCall.Constructor();
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x00136FE4 File Offset: 0x001351E4
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.Index_00003B80$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.Index$BurstManaged(L);
		}

		// Token: 0x04004935 RID: 18741
		private static IntPtr Pointer;

		// Token: 0x04004936 RID: 18742
		private static IntPtr DeferredCompilation;
	}

	// Token: 0x020009C3 RID: 2499
	// (Invoke) Token: 0x06003CB3 RID: 15539
	public unsafe delegate int NewIndex_00003B81$PostfixBurstDelegate(lua_State* L);

	// Token: 0x020009C4 RID: 2500
	internal static class NewIndex_00003B81$BurstDirectCall
	{
		// Token: 0x06003CB6 RID: 15542 RVA: 0x00137015 File Offset: 0x00135215
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.NewIndex_00003B81$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.NewIndex$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.NewIndex_00003B81$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Pointer;
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x00137044 File Offset: 0x00135244
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NewIndex_00003B81$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x0013705C File Offset: 0x0013525C
		public unsafe static void Constructor()
		{
			BurstClassInfo.NewIndex_00003B81$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.NewIndex(lua_State*)).MethodHandle);
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x000023F5 File Offset: 0x000005F5
		public static void Initialize()
		{
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x0013706D File Offset: 0x0013526D
		// Note: this type is marked as 'beforefieldinit'.
		static NewIndex_00003B81$BurstDirectCall()
		{
			BurstClassInfo.NewIndex_00003B81$BurstDirectCall.Constructor();
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x00137074 File Offset: 0x00135274
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NewIndex_00003B81$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NewIndex$BurstManaged(L);
		}

		// Token: 0x04004937 RID: 18743
		private static IntPtr Pointer;

		// Token: 0x04004938 RID: 18744
		private static IntPtr DeferredCompilation;
	}

	// Token: 0x020009C5 RID: 2501
	// (Invoke) Token: 0x06003CBD RID: 15549
	public unsafe delegate int NameCall_00003B82$PostfixBurstDelegate(lua_State* L);

	// Token: 0x020009C6 RID: 2502
	internal static class NameCall_00003B82$BurstDirectCall
	{
		// Token: 0x06003CC0 RID: 15552 RVA: 0x001370A5 File Offset: 0x001352A5
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NameCall_00003B82$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NameCall_00003B82$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.NameCall_00003B82$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.NameCall$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.NameCall_00003B82$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.NameCall_00003B82$BurstDirectCall.Pointer;
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x001370D4 File Offset: 0x001352D4
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NameCall_00003B82$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x001370EC File Offset: 0x001352EC
		public unsafe static void Constructor()
		{
			BurstClassInfo.NameCall_00003B82$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.NameCall(lua_State*)).MethodHandle);
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x000023F5 File Offset: 0x000005F5
		public static void Initialize()
		{
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x001370FD File Offset: 0x001352FD
		// Note: this type is marked as 'beforefieldinit'.
		static NameCall_00003B82$BurstDirectCall()
		{
			BurstClassInfo.NameCall_00003B82$BurstDirectCall.Constructor();
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x00137104 File Offset: 0x00135304
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NameCall_00003B82$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NameCall$BurstManaged(L);
		}

		// Token: 0x04004939 RID: 18745
		private static IntPtr Pointer;

		// Token: 0x0400493A RID: 18746
		private static IntPtr DeferredCompilation;
	}
}
