using System;
using UnityEngine;

// Token: 0x0200041C RID: 1052
public static class JSonHelper
{
	// Token: 0x0600198E RID: 6542 RVA: 0x000899B6 File Offset: 0x00087BB6
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000899C3 File Offset: 0x00087BC3
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000899D6 File Offset: 0x00087BD6
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x0200041D RID: 1053
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x040021E6 RID: 8678
		public T[] Items;
	}
}
