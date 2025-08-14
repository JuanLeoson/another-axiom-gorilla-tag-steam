using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020007ED RID: 2029
[Serializable]
public sealed class ComponentFunctionReference<TResult>
{
	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x060032D0 RID: 13008 RVA: 0x001089BC File Offset: 0x00106BBC
	public bool IsValid
	{
		get
		{
			return this._selection.component || !string.IsNullOrEmpty(this._selection.methodName);
		}
	}

	// Token: 0x060032D1 RID: 13009 RVA: 0x001089E5 File Offset: 0x00106BE5
	private IEnumerable<ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>> GetMethodOptions()
	{
		if (this._target == null)
		{
			yield break;
		}
		yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>("NONE", default(ComponentFunctionReference<TResult>.MethodRef));
		Type type = typeof(GameObject);
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (MethodInfo methodInfo in type.GetMethods(flags))
		{
			if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(TResult))
			{
				string text = type.Name + "/" + methodInfo.Name;
				yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text, new ComponentFunctionReference<TResult>.MethodRef(this._target, methodInfo));
			}
		}
		MethodInfo[] array = null;
		foreach (Component comp in this._target.GetComponents<Component>())
		{
			type = comp.GetType();
			foreach (MethodInfo methodInfo2 in type.GetMethods(flags))
			{
				if (methodInfo2.GetParameters().Length == 0 && methodInfo2.ReturnType == typeof(TResult))
				{
					string text2 = type.Name + "/" + methodInfo2.Name;
					yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text2, new ComponentFunctionReference<TResult>.MethodRef(comp, methodInfo2));
				}
			}
			array = null;
			comp = null;
		}
		Component[] array2 = null;
		yield break;
	}

	// Token: 0x060032D2 RID: 13010 RVA: 0x001089F8 File Offset: 0x00106BF8
	public TResult Invoke()
	{
		if (this._cached == null)
		{
			this.Cache();
		}
		if (this._cached == null)
		{
			return default(TResult);
		}
		return this._cached();
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x00108A30 File Offset: 0x00106C30
	public void Cache()
	{
		this._cached = null;
		if (this._selection.component == null || string.IsNullOrEmpty(this._selection.methodName))
		{
			return;
		}
		MethodInfo method = this._selection.component.GetType().GetMethod(this._selection.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		if (method != null)
		{
			this._cached = (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), this._selection.component, method);
		}
	}

	// Token: 0x04003FBD RID: 16317
	[SerializeField]
	private GameObject _target;

	// Token: 0x04003FBE RID: 16318
	[SerializeField]
	private ComponentFunctionReference<TResult>.MethodRef _selection;

	// Token: 0x04003FBF RID: 16319
	private Func<TResult> _cached;

	// Token: 0x020007EE RID: 2030
	[Serializable]
	private struct MethodRef
	{
		// Token: 0x060032D5 RID: 13013 RVA: 0x00108AC3 File Offset: 0x00106CC3
		public MethodRef(Object obj, MethodInfo m)
		{
			this.component = obj;
			this.methodName = m.Name;
		}

		// Token: 0x04003FC0 RID: 16320
		public Object component;

		// Token: 0x04003FC1 RID: 16321
		public string methodName;
	}
}
