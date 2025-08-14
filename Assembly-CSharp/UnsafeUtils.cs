using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000AFF RID: 2815
public static class UnsafeUtils
{
	// Token: 0x060043D8 RID: 17368 RVA: 0x00154C0C File Offset: 0x00152E0C
	public unsafe static ref readonly T[] GetInternalArray<T>(this List<T> list)
	{
		if (list == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return ref Unsafe.As<List<T>, StrongBox<T[]>>(ref list)->Value;
	}

	// Token: 0x060043D9 RID: 17369 RVA: 0x00154C24 File Offset: 0x00152E24
	public unsafe static ref readonly T[] GetInvocationListUnsafe<T>(this T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return Unsafe.As<Delegate[], T[]>(ref Unsafe.As<T, UnsafeUtils._MultiDelegateFields>(ref @delegate)->delegates);
	}

	// Token: 0x02000B00 RID: 2816
	[StructLayout(LayoutKind.Sequential)]
	private class _MultiDelegateFields : UnsafeUtils._DelegateFields
	{
		// Token: 0x04004E45 RID: 20037
		public Delegate[] delegates;
	}

	// Token: 0x02000B01 RID: 2817
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateFields
	{
		// Token: 0x04004E46 RID: 20038
		public IntPtr method_ptr;

		// Token: 0x04004E47 RID: 20039
		public IntPtr invoke_impl;

		// Token: 0x04004E48 RID: 20040
		public object m_target;

		// Token: 0x04004E49 RID: 20041
		public IntPtr method;

		// Token: 0x04004E4A RID: 20042
		public IntPtr delegate_trampoline;

		// Token: 0x04004E4B RID: 20043
		public IntPtr extra_arg;

		// Token: 0x04004E4C RID: 20044
		public IntPtr method_code;

		// Token: 0x04004E4D RID: 20045
		public IntPtr interp_method;

		// Token: 0x04004E4E RID: 20046
		public IntPtr interp_invoke_impl;

		// Token: 0x04004E4F RID: 20047
		public MethodInfo method_info;

		// Token: 0x04004E50 RID: 20048
		public MethodInfo original_method_info;

		// Token: 0x04004E51 RID: 20049
		public UnsafeUtils._DelegateData data;

		// Token: 0x04004E52 RID: 20050
		public bool method_is_virtual;
	}

	// Token: 0x02000B02 RID: 2818
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateData
	{
		// Token: 0x04004E53 RID: 20051
		public Type target_type;

		// Token: 0x04004E54 RID: 20052
		public string method_name;

		// Token: 0x04004E55 RID: 20053
		public bool curried_first_arg;
	}
}
