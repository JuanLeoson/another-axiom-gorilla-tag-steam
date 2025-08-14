using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000ABE RID: 2750
public class Vector3Converter : JsonConverter
{
	// Token: 0x0600424F RID: 16975 RVA: 0x0014DBF4 File Offset: 0x0014BDF4
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector3 vector = (Vector3)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector.y);
		writer.WritePropertyName("z");
		writer.WriteValue(vector.z);
		writer.WriteEndObject();
	}

	// Token: 0x06004250 RID: 16976 RVA: 0x0014DC5C File Offset: 0x0014BE5C
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jobject = JObject.Load(reader);
		return new Vector3((float)jobject["x"], (float)jobject["y"], (float)jobject["z"]);
	}

	// Token: 0x06004251 RID: 16977 RVA: 0x0014DCAD File Offset: 0x0014BEAD
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}
}
