using System;
using System.Globalization;
using System.Reflection;

// Token: 0x02000B13 RID: 2835
public class ProxyType : Type
{
	// Token: 0x0600443A RID: 17466 RVA: 0x00155C1C File Offset: 0x00153E1C
	public ProxyType()
	{
	}

	// Token: 0x0600443B RID: 17467 RVA: 0x00155C34 File Offset: 0x00153E34
	public ProxyType(string typeName)
	{
		this._typeName = typeName;
	}

	// Token: 0x1700066E RID: 1646
	// (get) Token: 0x0600443C RID: 17468 RVA: 0x00155C53 File Offset: 0x00153E53
	public override string Name
	{
		get
		{
			return this._typeName;
		}
	}

	// Token: 0x1700066F RID: 1647
	// (get) Token: 0x0600443D RID: 17469 RVA: 0x00155C5B File Offset: 0x00153E5B
	public override string FullName
	{
		get
		{
			return ProxyType.kPrefix + this._typeName;
		}
	}

	// Token: 0x0600443E RID: 17470 RVA: 0x00155C70 File Offset: 0x00153E70
	public static ProxyType Parse(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.Trim();
		if (!input.Contains(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (!input.StartsWith(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (input.Contains(','))
		{
			input = input.Split(',', StringSplitOptions.None)[0];
		}
		string text = input.Split('.', StringSplitOptions.None)[1].Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return ProxyType.kInvalidType;
		}
		return new ProxyType(text);
	}

	// Token: 0x0600443F RID: 17471 RVA: 0x00155CFC File Offset: 0x00153EFC
	public override string ToString()
	{
		return base.ToString() + "." + this._typeName;
	}

	// Token: 0x06004440 RID: 17472 RVA: 0x00155D14 File Offset: 0x00153F14
	public override object[] GetCustomAttributes(bool inherit)
	{
		return this._self.GetCustomAttributes(inherit);
	}

	// Token: 0x06004441 RID: 17473 RVA: 0x00155D22 File Offset: 0x00153F22
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return this._self.GetCustomAttributes(attributeType, inherit);
	}

	// Token: 0x06004442 RID: 17474 RVA: 0x00155D31 File Offset: 0x00153F31
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return this._self.IsDefined(attributeType, inherit);
	}

	// Token: 0x17000670 RID: 1648
	// (get) Token: 0x06004443 RID: 17475 RVA: 0x00155D40 File Offset: 0x00153F40
	public override Module Module
	{
		get
		{
			return this._self.Module;
		}
	}

	// Token: 0x17000671 RID: 1649
	// (get) Token: 0x06004444 RID: 17476 RVA: 0x00155D4D File Offset: 0x00153F4D
	public override string Namespace
	{
		get
		{
			return this._self.Namespace;
		}
	}

	// Token: 0x06004445 RID: 17477 RVA: 0x00002076 File Offset: 0x00000276
	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return TypeAttributes.NotPublic;
	}

	// Token: 0x06004446 RID: 17478 RVA: 0x00058615 File Offset: 0x00056815
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06004447 RID: 17479 RVA: 0x00155D5A File Offset: 0x00153F5A
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return this._self.GetConstructors(bindingAttr);
	}

	// Token: 0x06004448 RID: 17480 RVA: 0x00155D68 File Offset: 0x00153F68
	public override Type GetElementType()
	{
		return this._self.GetElementType();
	}

	// Token: 0x06004449 RID: 17481 RVA: 0x00155D75 File Offset: 0x00153F75
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return this._self.GetEvent(name, bindingAttr);
	}

	// Token: 0x0600444A RID: 17482 RVA: 0x00155D84 File Offset: 0x00153F84
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return this._self.GetEvents(bindingAttr);
	}

	// Token: 0x0600444B RID: 17483 RVA: 0x00155D92 File Offset: 0x00153F92
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return this._self.GetField(name, bindingAttr);
	}

	// Token: 0x0600444C RID: 17484 RVA: 0x00155DA1 File Offset: 0x00153FA1
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return this._self.GetFields(bindingAttr);
	}

	// Token: 0x0600444D RID: 17485 RVA: 0x00155DAF File Offset: 0x00153FAF
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		return this._self.GetMembers(bindingAttr);
	}

	// Token: 0x0600444E RID: 17486 RVA: 0x00058615 File Offset: 0x00056815
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x0600444F RID: 17487 RVA: 0x00155DBD File Offset: 0x00153FBD
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return this._self.GetMethods(bindingAttr);
	}

	// Token: 0x06004450 RID: 17488 RVA: 0x00155DCB File Offset: 0x00153FCB
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return this._self.GetProperties(bindingAttr);
	}

	// Token: 0x06004451 RID: 17489 RVA: 0x00155DDC File Offset: 0x00153FDC
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return this._self.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	// Token: 0x17000672 RID: 1650
	// (get) Token: 0x06004452 RID: 17490 RVA: 0x00155E01 File Offset: 0x00154001
	public override Type UnderlyingSystemType
	{
		get
		{
			return this._self.UnderlyingSystemType;
		}
	}

	// Token: 0x06004453 RID: 17491 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsArrayImpl()
	{
		return false;
	}

	// Token: 0x06004454 RID: 17492 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsByRefImpl()
	{
		return false;
	}

	// Token: 0x06004455 RID: 17493 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsCOMObjectImpl()
	{
		return false;
	}

	// Token: 0x06004456 RID: 17494 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPointerImpl()
	{
		return false;
	}

	// Token: 0x06004457 RID: 17495 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	// Token: 0x17000673 RID: 1651
	// (get) Token: 0x06004458 RID: 17496 RVA: 0x00155E0E File Offset: 0x0015400E
	public override Assembly Assembly
	{
		get
		{
			return this._self.Assembly;
		}
	}

	// Token: 0x17000674 RID: 1652
	// (get) Token: 0x06004459 RID: 17497 RVA: 0x00155E1B File Offset: 0x0015401B
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName.Replace("ProxyType", this.FullName);
		}
	}

	// Token: 0x17000675 RID: 1653
	// (get) Token: 0x0600445A RID: 17498 RVA: 0x00155E38 File Offset: 0x00154038
	public override Type BaseType
	{
		get
		{
			return this._self.BaseType;
		}
	}

	// Token: 0x17000676 RID: 1654
	// (get) Token: 0x0600445B RID: 17499 RVA: 0x00155E45 File Offset: 0x00154045
	public override Guid GUID
	{
		get
		{
			return this._self.GUID;
		}
	}

	// Token: 0x0600445C RID: 17500 RVA: 0x00058615 File Offset: 0x00056815
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x0600445D RID: 17501 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	// Token: 0x0600445E RID: 17502 RVA: 0x00155E52 File Offset: 0x00154052
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		return this._self.GetNestedType(name, bindingAttr);
	}

	// Token: 0x0600445F RID: 17503 RVA: 0x00155E61 File Offset: 0x00154061
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return this._self.GetNestedTypes(bindingAttr);
	}

	// Token: 0x06004460 RID: 17504 RVA: 0x00155E6F File Offset: 0x0015406F
	public override Type GetInterface(string name, bool ignoreCase)
	{
		return this._self.GetInterface(name, ignoreCase);
	}

	// Token: 0x06004461 RID: 17505 RVA: 0x00155E7E File Offset: 0x0015407E
	public override Type[] GetInterfaces()
	{
		return this._self.GetInterfaces();
	}

	// Token: 0x04004E75 RID: 20085
	private Type _self = typeof(ProxyType);

	// Token: 0x04004E76 RID: 20086
	private readonly string _typeName;

	// Token: 0x04004E77 RID: 20087
	private static readonly string kPrefix = "ProxyType.";

	// Token: 0x04004E78 RID: 20088
	private static InvalidType kInvalidType = new InvalidType();
}
