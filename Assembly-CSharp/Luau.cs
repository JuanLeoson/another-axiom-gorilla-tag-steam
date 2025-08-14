using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020009CD RID: 2509
public class Luau
{
	// Token: 0x06003CDB RID: 15579
	[DllImport("luau")]
	public unsafe static extern lua_State* luaL_newstate();

	// Token: 0x06003CDC RID: 15580
	[DllImport("luau")]
	public unsafe static extern void luaL_openlibs(lua_State* L);

	// Token: 0x06003CDD RID: 15581
	[DllImport("luau")]
	public unsafe static extern sbyte* luau_compile([MarshalAs(UnmanagedType.LPStr)] string source, [NativeInteger] UIntPtr size, lua_CompileOptions* options, [NativeInteger] UIntPtr* outsize);

	// Token: 0x06003CDE RID: 15582
	[DllImport("luau")]
	public unsafe static extern int luau_load(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string chunkname, sbyte* data, [NativeInteger] UIntPtr size, int env);

	// Token: 0x06003CDF RID: 15583
	[DllImport("luau")]
	public unsafe static extern void lua_pushvalue(lua_State* L, int idx);

	// Token: 0x06003CE0 RID: 15584
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06003CE1 RID: 15585
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06003CE2 RID: 15586
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, byte* debugname, int nup, int* cont);

	// Token: 0x06003CE3 RID: 15587 RVA: 0x001376D0 File Offset: 0x001358D0
	public unsafe static void lua_pushcfunction(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06003CE4 RID: 15588 RVA: 0x001376DC File Offset: 0x001358DC
	public unsafe static void lua_pushcfunction(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06003CE5 RID: 15589
	[DllImport("luau")]
	public unsafe static extern void lua_settop(lua_State* L, int idx);

	// Token: 0x06003CE6 RID: 15590
	[DllImport("luau")]
	public unsafe static extern int lua_gettop(lua_State* L);

	// Token: 0x06003CE7 RID: 15591
	[DllImport("luau")]
	public unsafe static extern sbyte* lua_tolstring(lua_State* L, int idx, int* len);

	// Token: 0x06003CE8 RID: 15592
	[DllImport("luau")]
	public unsafe static extern int lua_resume(lua_State* L, lua_State* from, int nargs);

	// Token: 0x06003CE9 RID: 15593
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06003CEA RID: 15594
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, byte* k);

	// Token: 0x06003CEB RID: 15595 RVA: 0x001376E8 File Offset: 0x001358E8
	public unsafe static void lua_setglobal(lua_State* L, string s)
	{
		Luau.lua_setfield(L, -10002, s);
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x001376F8 File Offset: 0x001358F8
	public unsafe static void lua_register(lua_State* L, lua_CFunction f, string n)
	{
		lua_Continuation cont = null;
		Luau.lua_pushcclosurek(L, f, n, 0, cont);
		Luau.lua_setglobal(L, n);
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x00137718 File Offset: 0x00135918
	public unsafe static void lua_pop(lua_State* L, int n)
	{
		Luau.lua_settop(L, -n - 1);
	}

	// Token: 0x06003CEE RID: 15598 RVA: 0x00137724 File Offset: 0x00135924
	public unsafe static sbyte* lua_tostring(lua_State* L, int idx)
	{
		return Luau.lua_tolstring(L, idx, null);
	}

	// Token: 0x06003CEF RID: 15599
	[DllImport("luau")]
	public unsafe static extern int lua_isstring(lua_State* L, int index);

	// Token: 0x06003CF0 RID: 15600
	[DllImport("luau")]
	public unsafe static extern int lua_type(lua_State* L, int index);

	// Token: 0x06003CF1 RID: 15601
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string s);

	// Token: 0x06003CF2 RID: 15602
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, byte* s);

	// Token: 0x06003CF3 RID: 15603
	[DllImport("luau")]
	public unsafe static extern int lua_error(lua_State* L);

	// Token: 0x06003CF4 RID: 15604
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string fmt, [MarshalAs(UnmanagedType.LPStr)] params string[] a);

	// Token: 0x06003CF5 RID: 15605
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, sbyte* fmt);

	// Token: 0x06003CF6 RID: 15606
	[DllImport("luau")]
	public unsafe static extern int lua_toboolean(lua_State* L, int index);

	// Token: 0x06003CF7 RID: 15607
	[DllImport("luau")]
	public unsafe static extern byte* lua_debugtrace(lua_State* L);

	// Token: 0x06003CF8 RID: 15608
	[DllImport("luau")]
	public unsafe static extern void lua_close(lua_State* L);

	// Token: 0x06003CF9 RID: 15609
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdatatagged(lua_State* L, int idx, int tag);

	// Token: 0x06003CFA RID: 15610
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdata(lua_State* L, int index);

	// Token: 0x06003CFB RID: 15611
	[DllImport("luau")]
	public unsafe static extern void* lua_newuserdatatagged(lua_State* L, int sz, int tag);

	// Token: 0x06003CFC RID: 15612
	[DllImport("luau")]
	public unsafe static extern void lua_getuserdatametatable(lua_State* L, int tag);

	// Token: 0x06003CFD RID: 15613
	[DllImport("luau")]
	public unsafe static extern void lua_setuserdatametatable(lua_State* L, int tag, int idx);

	// Token: 0x06003CFE RID: 15614
	[DllImport("luau")]
	public unsafe static extern int lua_setmetatable(lua_State* L, int objindex);

	// Token: 0x06003CFF RID: 15615
	[DllImport("luau")]
	public unsafe static extern int luaL_newmetatable(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x06003D00 RID: 15616
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06003D01 RID: 15617
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, byte* k);

	// Token: 0x06003D02 RID: 15618
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, byte* k);

	// Token: 0x06003D03 RID: 15619
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06003D04 RID: 15620 RVA: 0x0013772F File Offset: 0x0013592F
	public unsafe static void luaL_getmetatable(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06003D05 RID: 15621 RVA: 0x0013773E File Offset: 0x0013593E
	public unsafe static void luaL_getmetatable(lua_State* L, byte* n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06003D06 RID: 15622 RVA: 0x0013774D File Offset: 0x0013594D
	public unsafe static void lua_getglobal(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10002, n);
	}

	// Token: 0x06003D07 RID: 15623
	[DllImport("luau")]
	public unsafe static extern int lua_getmetatable(lua_State* L, int objindex);

	// Token: 0x06003D08 RID: 15624
	[DllImport("luau")]
	public unsafe static extern byte* lua_namecallatom(lua_State* L, int* atom);

	// Token: 0x06003D09 RID: 15625
	[DllImport("luau")]
	public unsafe static extern byte* luaL_checklstring(lua_State* L, int numArg, int* l);

	// Token: 0x06003D0A RID: 15626 RVA: 0x0013775C File Offset: 0x0013595C
	public unsafe static byte* luaL_checkstring(lua_State* L, int n)
	{
		return Luau.luaL_checklstring(L, n, null);
	}

	// Token: 0x06003D0B RID: 15627
	[DllImport("luau")]
	public unsafe static extern void lua_pushnumber(lua_State* L, double n);

	// Token: 0x06003D0C RID: 15628
	[DllImport("luau")]
	public unsafe static extern double luaL_checknumber(lua_State* L, int numArg);

	// Token: 0x06003D0D RID: 15629
	[DllImport("luau")]
	public unsafe static extern void lua_setreadonly(lua_State* L, int idx, int enabled);

	// Token: 0x06003D0E RID: 15630
	[DllImport("luau")]
	public unsafe static extern double lua_tonumberx(lua_State* L, int index, int* isnum);

	// Token: 0x06003D0F RID: 15631
	[DllImport("luau")]
	public unsafe static extern int lua_gc(lua_State* L, int what, int data);

	// Token: 0x06003D10 RID: 15632
	[DllImport("luau")]
	public unsafe static extern void lua_call(lua_State* L, int nargs, int nresults);

	// Token: 0x06003D11 RID: 15633
	[DllImport("luau")]
	public unsafe static extern int lua_pcall(lua_State* L, int nargs, int nresults, int fn);

	// Token: 0x06003D12 RID: 15634
	[DllImport("luau")]
	public unsafe static extern int lua_status(lua_State* L);

	// Token: 0x06003D13 RID: 15635
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x06003D14 RID: 15636
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, byte* tname);

	// Token: 0x06003D15 RID: 15637
	[DllImport("luau")]
	public unsafe static extern int lua_objlen(lua_State* L, int index);

	// Token: 0x06003D16 RID: 15638
	[DllImport("luau")]
	public unsafe static extern double luaL_optnumber(lua_State* L, int narg, double d);

	// Token: 0x06003D17 RID: 15639
	[DllImport("luau")]
	public unsafe static extern void lua_createtable(lua_State* L, int narr, int nrec);

	// Token: 0x06003D18 RID: 15640
	[DllImport("luau")]
	public unsafe static extern void lua_pushlightuserdatatagged(lua_State* L, void* p, int tag);

	// Token: 0x06003D19 RID: 15641
	[DllImport("luau")]
	public unsafe static extern void lua_pushnil(lua_State* L);

	// Token: 0x06003D1A RID: 15642
	[DllImport("luau")]
	public unsafe static extern int lua_next(lua_State* L, int index);

	// Token: 0x06003D1B RID: 15643 RVA: 0x00137767 File Offset: 0x00135967
	public unsafe static void lua_pushlightuserdata(lua_State* L, void* p)
	{
		Luau.lua_pushlightuserdatatagged(L, p, 0);
	}

	// Token: 0x06003D1C RID: 15644
	[DllImport("luau")]
	public unsafe static extern void lua_rawseti(lua_State* L, int idx, int n);

	// Token: 0x06003D1D RID: 15645
	[DllImport("luau")]
	public unsafe static extern void lua_rawgeti(lua_State* L, int index, int n);

	// Token: 0x06003D1E RID: 15646
	[DllImport("luau")]
	public unsafe static extern void lua_rawget(lua_State* L, int index);

	// Token: 0x06003D1F RID: 15647
	[DllImport("luau")]
	public unsafe static extern void lua_rawset(lua_State* L, int index);

	// Token: 0x06003D20 RID: 15648
	[DllImport("luau")]
	public unsafe static extern void lua_remove(lua_State* L, int index);

	// Token: 0x06003D21 RID: 15649
	[DllImport("luau")]
	public unsafe static extern void lua_pushboolean(lua_State* L, int b);

	// Token: 0x06003D22 RID: 15650
	[DllImport("luau")]
	public unsafe static extern int lua_rawequal(lua_State* L, int a, int b);

	// Token: 0x06003D23 RID: 15651 RVA: 0x00137771 File Offset: 0x00135971
	public unsafe static void* lua_newuserdata(lua_State* L, int size)
	{
		return Luau.lua_newuserdatatagged(L, size, 0);
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x0013777B File Offset: 0x0013597B
	public unsafe static double lua_tonumber(lua_State* L, int index)
	{
		return Luau.lua_tonumberx(L, index, null);
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x00137788 File Offset: 0x00135988
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L) where T : struct, ValueType
	{
		T* result = (T*)Luau.lua_newuserdata(L, sizeof(T));
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return result;
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x001377C4 File Offset: 0x001359C4
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L, FixedString32Bytes name) where T : struct, ValueType
	{
		T* result = (T*)Luau.lua_newuserdata(L, sizeof(T));
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return result;
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x001377F8 File Offset: 0x001359F8
	public unsafe static void lua_class_push(lua_State* L, FixedString32Bytes name, IntPtr ptr)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdata(L, (void*)ptr);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x06003D28 RID: 15656 RVA: 0x00137850 File Offset: 0x00135A50
	public unsafe static void lua_class_push<[IsUnmanaged] T>(lua_State* L, T* ptr) where T : struct, ValueType
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdata(L, (void*)ptr);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x001378A8 File Offset: 0x00135AA8
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 2);
			if (flag)
			{
				Luau.lua_getfield(L, idx, "__ptr");
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
		return null;
	}

	// Token: 0x06003D2A RID: 15658 RVA: 0x00137960 File Offset: 0x00135B60
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx, FixedString32Bytes name) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x00137A20 File Offset: 0x00135C20
	public unsafe static byte* lua_class_get(lua_State* L, int idx, FixedString32Bytes name)
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			byte* ptr2 = (byte*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					byte* ptr3 = (byte*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x00137AE0 File Offset: 0x00135CE0
	public unsafe static IntPtr lua_light_ptr(lua_State* L, int idx)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		if (Luau.lua_type(L, -1) == 2)
		{
			void* ptr = Luau.lua_touserdata(L, -1);
			Luau.lua_pop(L, 1);
			if (ptr != null)
			{
				return (IntPtr)ptr;
			}
		}
		return IntPtr.Zero;
	}

	// Token: 0x06003D2D RID: 15661 RVA: 0x00137B33 File Offset: 0x00135D33
	public unsafe static bool lua_class_check<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		return Luau.lua_objlen(L, idx) == sizeof(T);
	}

	// Token: 0x06003D2E RID: 15662 RVA: 0x00137B44 File Offset: 0x00135D44
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int lua_print(lua_State* L)
	{
		string text = "";
		int num = Luau.lua_gettop(L);
		for (int i = 1; i <= num; i++)
		{
			int num2 = Luau.lua_type(L, i);
			if (num2 == 5 || num2 == 3)
			{
				sbyte* value = Luau.lua_tostring(L, i);
				text += Marshal.PtrToStringAnsi((IntPtr)((void*)value));
			}
			else
			{
				if (num2 != 1)
				{
					Luau.luaL_errorL(L, "Invalid String", Array.Empty<string>());
					return 0;
				}
				int num3 = Luau.lua_toboolean(L, i);
				text += ((num3 == 1) ? "true" : "false");
			}
		}
		LuauHud.Instance.LuauLog(text);
		return 0;
	}

	// Token: 0x04004946 RID: 18758
	public const int LUA_GLOBALSINDEX = -10002;

	// Token: 0x04004947 RID: 18759
	public const int LUA_REGISTRYINDEX = -10000;

	// Token: 0x020009CE RID: 2510
	public enum lua_Types
	{
		// Token: 0x04004949 RID: 18761
		LUA_TNIL,
		// Token: 0x0400494A RID: 18762
		LUA_TBOOLEAN,
		// Token: 0x0400494B RID: 18763
		LUA_TLIGHTUSERDATA,
		// Token: 0x0400494C RID: 18764
		LUA_TNUMBER,
		// Token: 0x0400494D RID: 18765
		LUA_TVECTOR,
		// Token: 0x0400494E RID: 18766
		LUA_TSTRING,
		// Token: 0x0400494F RID: 18767
		LUA_TTABLE,
		// Token: 0x04004950 RID: 18768
		LUA_TFUNCTION,
		// Token: 0x04004951 RID: 18769
		LUA_TUSERDATA,
		// Token: 0x04004952 RID: 18770
		LUA_TTHREAD,
		// Token: 0x04004953 RID: 18771
		LUA_TBUFFER,
		// Token: 0x04004954 RID: 18772
		LUA_TPROTO,
		// Token: 0x04004955 RID: 18773
		LUA_TUPVAL,
		// Token: 0x04004956 RID: 18774
		LUA_TDEADKEY,
		// Token: 0x04004957 RID: 18775
		LUA_T_COUNT = 11
	}

	// Token: 0x020009CF RID: 2511
	public enum lua_Status
	{
		// Token: 0x04004959 RID: 18777
		LUA_OK,
		// Token: 0x0400495A RID: 18778
		LUA_YIELD,
		// Token: 0x0400495B RID: 18779
		LUA_ERRRUN,
		// Token: 0x0400495C RID: 18780
		LUA_ERRSYNTAX,
		// Token: 0x0400495D RID: 18781
		LUA_ERRMEM,
		// Token: 0x0400495E RID: 18782
		LUA_ERRERR,
		// Token: 0x0400495F RID: 18783
		LUA_BREAK
	}

	// Token: 0x020009D0 RID: 2512
	public enum gc_status
	{
		// Token: 0x04004961 RID: 18785
		LUA_GCSTOP,
		// Token: 0x04004962 RID: 18786
		LUA_GCRESTART,
		// Token: 0x04004963 RID: 18787
		LUA_GCCOLLECT,
		// Token: 0x04004964 RID: 18788
		LUA_GCCOUNT,
		// Token: 0x04004965 RID: 18789
		LUA_GCISRUNNING,
		// Token: 0x04004966 RID: 18790
		LUA_GCSTEP,
		// Token: 0x04004967 RID: 18791
		LUA_GCSETGOAL,
		// Token: 0x04004968 RID: 18792
		LUA_GCSETSTEPMUL,
		// Token: 0x04004969 RID: 18793
		LUA_GCSETSTEPSIZE
	}

	// Token: 0x020009D1 RID: 2513
	public static class lua_TypeID
	{
		// Token: 0x06003D30 RID: 15664 RVA: 0x00137BE0 File Offset: 0x00135DE0
		public static string get(Type t)
		{
			string result;
			if (Luau.lua_TypeID.names.TryGetValue(t, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x06003D31 RID: 15665 RVA: 0x00137C03 File Offset: 0x00135E03
		public static void push(Type t, string name)
		{
			Luau.lua_TypeID.names.TryAdd(t, name);
		}

		// Token: 0x0400496A RID: 18794
		private static Dictionary<Type, string> names = new Dictionary<Type, string>();
	}

	// Token: 0x020009D2 RID: 2514
	public static class lua_ClassFields<T>
	{
		// Token: 0x06003D33 RID: 15667 RVA: 0x00137C20 File Offset: 0x00135E20
		public static FieldInfo Get(string name)
		{
			Dictionary<int, FieldInfo> dictionary;
			FieldInfo result;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary) && dictionary.TryGetValue(name.GetHashCode(), out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06003D34 RID: 15668 RVA: 0x00137C60 File Offset: 0x00135E60
		public static void Add(string name, FieldInfo field)
		{
			Dictionary<int, FieldInfo> dictionary;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary))
			{
				dictionary.TryAdd(name.GetHashCode(), field);
				return;
			}
			Dictionary<int, FieldInfo> dictionary2 = new Dictionary<int, FieldInfo>();
			dictionary2.TryAdd(name.GetHashCode(), field);
			Luau.lua_ClassFields<T>.classDictionarys.TryAdd(typeof(T).GetHashCode(), dictionary2);
		}

		// Token: 0x0400496B RID: 18795
		private static Dictionary<int, Dictionary<int, FieldInfo>> classDictionarys = new Dictionary<int, Dictionary<int, FieldInfo>>();
	}

	// Token: 0x020009D3 RID: 2515
	public static class lua_ClassProperties<T>
	{
		// Token: 0x06003D36 RID: 15670 RVA: 0x00137CD4 File Offset: 0x00135ED4
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction result;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x00137D08 File Offset: 0x00135F08
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassProperties<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x0400496C RID: 18796
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}

	// Token: 0x020009D4 RID: 2516
	public static class lua_ClassFunctions<T>
	{
		// Token: 0x06003D39 RID: 15673 RVA: 0x00137D68 File Offset: 0x00135F68
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction result;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06003D3A RID: 15674 RVA: 0x00137D9C File Offset: 0x00135F9C
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassFunctions<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x0400496D RID: 18797
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}
}
