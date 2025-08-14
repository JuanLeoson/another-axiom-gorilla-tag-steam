using System;
using Newtonsoft.Json;

// Token: 0x02000ABD RID: 2749
public static class JsonUtils
{
	// Token: 0x0600424B RID: 16971 RVA: 0x0014DB5F File Offset: 0x0014BD5F
	public static string ToJson<T>(this T obj, bool indent = true)
	{
		return JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x0014DB73 File Offset: 0x0014BD73
	public static T FromJson<T>(this string s)
	{
		return JsonConvert.DeserializeObject<T>(s);
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x0014DB7C File Offset: 0x0014BD7C
	public static string JsonSerializeEventData<T>(this T obj)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			CheckAdditionalContent = true,
			Formatting = Formatting.None
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
	}

	// Token: 0x0600424E RID: 16974 RVA: 0x0014DBC0 File Offset: 0x0014BDC0
	public static T JsonDeserializeEventData<T>(this string s)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.DeserializeObject<T>(s, jsonSerializerSettings);
	}
}
