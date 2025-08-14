using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000357 RID: 855
public class CustomDebugUI : MonoBehaviour
{
	// Token: 0x06001444 RID: 5188 RVA: 0x0006D0FE File Offset: 0x0006B2FE
	private void Awake()
	{
		CustomDebugUI.instance = this;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x0006D108 File Offset: 0x0006B308
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		DebugUIBuilder obj = DebugUIBuilder.instance;
		typeof(DebugUIBuilder).GetMethod("AddRect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, new object[]
		{
			component,
			targetCanvas
		});
		return component;
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0006D16C File Offset: 0x0006B36C
	public void RemoveFromCanvas(RectTransform element, int targetCanvas = 0)
	{
		DebugUIBuilder obj = DebugUIBuilder.instance;
		FieldInfo field = typeof(DebugUIBuilder).GetField("insertedElements", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method = typeof(DebugUIBuilder).GetMethod("Relayout", BindingFlags.Instance | BindingFlags.NonPublic);
		List<RectTransform>[] array = (List<RectTransform>[])field.GetValue(obj);
		if (targetCanvas > -1 && targetCanvas < array.Length - 1)
		{
			array[targetCanvas].Remove(element);
			element.SetParent(null);
			method.Invoke(obj, new object[0]);
		}
	}

	// Token: 0x04001BD9 RID: 7129
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x04001BDA RID: 7130
	public static CustomDebugUI instance;

	// Token: 0x04001BDB RID: 7131
	private const BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.NonPublic;
}
