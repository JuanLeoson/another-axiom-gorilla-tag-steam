using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000BB0 RID: 2992
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x060047EF RID: 18415
		bool IsArray { get; }

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x060047F0 RID: 18416
		bool IsBoolean { get; }

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x060047F1 RID: 18417
		bool IsDouble { get; }

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x060047F2 RID: 18418
		bool IsInt { get; }

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060047F3 RID: 18419
		bool IsLong { get; }

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060047F4 RID: 18420
		bool IsObject { get; }

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x060047F5 RID: 18421
		bool IsString { get; }

		// Token: 0x060047F6 RID: 18422
		bool GetBoolean();

		// Token: 0x060047F7 RID: 18423
		double GetDouble();

		// Token: 0x060047F8 RID: 18424
		int GetInt();

		// Token: 0x060047F9 RID: 18425
		JsonType GetJsonType();

		// Token: 0x060047FA RID: 18426
		long GetLong();

		// Token: 0x060047FB RID: 18427
		string GetString();

		// Token: 0x060047FC RID: 18428
		void SetBoolean(bool val);

		// Token: 0x060047FD RID: 18429
		void SetDouble(double val);

		// Token: 0x060047FE RID: 18430
		void SetInt(int val);

		// Token: 0x060047FF RID: 18431
		void SetJsonType(JsonType type);

		// Token: 0x06004800 RID: 18432
		void SetLong(long val);

		// Token: 0x06004801 RID: 18433
		void SetString(string val);

		// Token: 0x06004802 RID: 18434
		string ToJson();

		// Token: 0x06004803 RID: 18435
		void ToJson(JsonWriter writer);
	}
}
