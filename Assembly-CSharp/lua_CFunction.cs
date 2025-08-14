using System;
using System.Runtime.InteropServices;

// Token: 0x020009CB RID: 2507
// (Invoke) Token: 0x06003CD4 RID: 15572
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int lua_CFunction(lua_State* L);
