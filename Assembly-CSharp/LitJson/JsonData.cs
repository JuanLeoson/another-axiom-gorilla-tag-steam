using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000BB1 RID: 2993
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06004804 RID: 18436 RVA: 0x001601CA File Offset: 0x0015E3CA
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06004805 RID: 18437 RVA: 0x001601D7 File Offset: 0x0015E3D7
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06004806 RID: 18438 RVA: 0x001601E2 File Offset: 0x0015E3E2
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06004807 RID: 18439 RVA: 0x001601ED File Offset: 0x0015E3ED
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06004808 RID: 18440 RVA: 0x001601F8 File Offset: 0x0015E3F8
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06004809 RID: 18441 RVA: 0x00160203 File Offset: 0x0015E403
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x0600480A RID: 18442 RVA: 0x0016020E File Offset: 0x0015E40E
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x0600480B RID: 18443 RVA: 0x00160219 File Offset: 0x0015E419
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x0600480C RID: 18444 RVA: 0x00160224 File Offset: 0x0015E424
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600480D RID: 18445 RVA: 0x0016022C File Offset: 0x0015E42C
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x0600480E RID: 18446 RVA: 0x00160239 File Offset: 0x0015E439
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x0600480F RID: 18447 RVA: 0x00160246 File Offset: 0x0015E446
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06004810 RID: 18448 RVA: 0x00160253 File Offset: 0x0015E453
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06004811 RID: 18449 RVA: 0x00160260 File Offset: 0x0015E460
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06004812 RID: 18450 RVA: 0x001602C8 File Offset: 0x0015E4C8
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06004813 RID: 18451 RVA: 0x00160330 File Offset: 0x0015E530
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06004814 RID: 18452 RVA: 0x00160338 File Offset: 0x0015E538
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06004815 RID: 18453 RVA: 0x00160340 File Offset: 0x0015E540
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06004816 RID: 18454 RVA: 0x00160348 File Offset: 0x0015E548
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06004817 RID: 18455 RVA: 0x00160350 File Offset: 0x0015E550
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06004818 RID: 18456 RVA: 0x00160358 File Offset: 0x0015E558
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06004819 RID: 18457 RVA: 0x00160360 File Offset: 0x0015E560
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x0600481A RID: 18458 RVA: 0x00160368 File Offset: 0x0015E568
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x0600481B RID: 18459 RVA: 0x00160375 File Offset: 0x0015E575
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x170006F5 RID: 1781
		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = this.ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		// Token: 0x170006F6 RID: 1782
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData value2 = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = value2;
				KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
				this.object_list[idx] = value3;
			}
		}

		// Token: 0x170006F7 RID: 1783
		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData value2 = this.ToJsonData(value);
				this[index] = value2;
			}
		}

		// Token: 0x170006F8 RID: 1784
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x170006F9 RID: 1785
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = value2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x06004826 RID: 18470 RVA: 0x00002050 File Offset: 0x00000250
		public JsonData()
		{
		}

		// Token: 0x06004827 RID: 18471 RVA: 0x001605D3 File Offset: 0x0015E7D3
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x06004828 RID: 18472 RVA: 0x001605E9 File Offset: 0x0015E7E9
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x06004829 RID: 18473 RVA: 0x001605FF File Offset: 0x0015E7FF
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x0600482A RID: 18474 RVA: 0x00160615 File Offset: 0x0015E815
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x0600482B RID: 18475 RVA: 0x0016062C File Offset: 0x0015E82C
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x001606D5 File Offset: 0x0015E8D5
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x0600482D RID: 18477 RVA: 0x001606EB File Offset: 0x0015E8EB
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x0600482E RID: 18478 RVA: 0x001606F3 File Offset: 0x0015E8F3
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x0600482F RID: 18479 RVA: 0x001606FB File Offset: 0x0015E8FB
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06004830 RID: 18480 RVA: 0x00160703 File Offset: 0x0015E903
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06004831 RID: 18481 RVA: 0x0016070B File Offset: 0x0015E90B
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06004832 RID: 18482 RVA: 0x00160713 File Offset: 0x0015E913
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06004833 RID: 18483 RVA: 0x0016072F File Offset: 0x0015E92F
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x0016074B File Offset: 0x0015E94B
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x00160767 File Offset: 0x0015E967
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06004836 RID: 18486 RVA: 0x00160783 File Offset: 0x0015E983
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06004837 RID: 18487 RVA: 0x0016079F File Offset: 0x0015E99F
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x06004838 RID: 18488 RVA: 0x001607B0 File Offset: 0x0015E9B0
		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			this.object_list.Add(item);
			this.json = null;
		}

		// Token: 0x06004839 RID: 18489 RVA: 0x001607F3 File Offset: 0x0015E9F3
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x0600483A RID: 18490 RVA: 0x00160812 File Offset: 0x0015EA12
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x0600483B RID: 18491 RVA: 0x00160820 File Offset: 0x0015EA20
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x0600483C RID: 18492 RVA: 0x00160828 File Offset: 0x0015EA28
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x0600483D RID: 18493 RVA: 0x0016088D File Offset: 0x0015EA8D
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x0016089A File Offset: 0x0015EA9A
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x001608B6 File Offset: 0x0015EAB6
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x001608D2 File Offset: 0x0015EAD2
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x001608EE File Offset: 0x0015EAEE
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06004842 RID: 18498 RVA: 0x0016090A File Offset: 0x0015EB0A
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06004843 RID: 18499 RVA: 0x00160926 File Offset: 0x0015EB26
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06004844 RID: 18500 RVA: 0x0016093D File Offset: 0x0015EB3D
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06004845 RID: 18501 RVA: 0x00160954 File Offset: 0x0015EB54
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06004846 RID: 18502 RVA: 0x0016096B File Offset: 0x0015EB6B
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06004847 RID: 18503 RVA: 0x00160982 File Offset: 0x0015EB82
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x06004848 RID: 18504 RVA: 0x00160999 File Offset: 0x0015EB99
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x06004849 RID: 18505 RVA: 0x001609A1 File Offset: 0x0015EBA1
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x0600484A RID: 18506 RVA: 0x001609AA File Offset: 0x0015EBAA
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x0600484B RID: 18507 RVA: 0x001609B3 File Offset: 0x0015EBB3
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x0600484C RID: 18508 RVA: 0x001609C7 File Offset: 0x0015EBC7
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x0600484D RID: 18509 RVA: 0x001609D5 File Offset: 0x0015EBD5
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x0600484E RID: 18510 RVA: 0x001609E3 File Offset: 0x0015EBE3
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x0600484F RID: 18511 RVA: 0x001609F9 File Offset: 0x0015EBF9
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06004850 RID: 18512 RVA: 0x00160A0E File Offset: 0x0015EC0E
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06004851 RID: 18513 RVA: 0x00160A23 File Offset: 0x0015EC23
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06004852 RID: 18514 RVA: 0x00160A3C File Offset: 0x0015EC3C
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this.ToJsonData(value);
			this[text] = value2;
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			this.object_list.Insert(idx, item);
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x00160A78 File Offset: 0x0015EC78
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06004854 RID: 18516 RVA: 0x00160AB8 File Offset: 0x0015ECB8
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x06004855 RID: 18517 RVA: 0x00160AF0 File Offset: 0x0015ECF0
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x06004856 RID: 18518 RVA: 0x00160B50 File Offset: 0x0015ED50
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x06004857 RID: 18519 RVA: 0x00160BA2 File Offset: 0x0015EDA2
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x06004858 RID: 18520 RVA: 0x00160BC0 File Offset: 0x0015EDC0
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x06004859 RID: 18521 RVA: 0x00160D08 File Offset: 0x0015EF08
		public int Add(object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(value2);
		}

		// Token: 0x0600485A RID: 18522 RVA: 0x00160D30 File Offset: 0x0015EF30
		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		// Token: 0x0600485B RID: 18523 RVA: 0x00160D50 File Offset: 0x0015EF50
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x0600485C RID: 18524 RVA: 0x00160E25 File Offset: 0x0015F025
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x0600485D RID: 18525 RVA: 0x00160E30 File Offset: 0x0015F030
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x0600485E RID: 18526 RVA: 0x00160ED0 File Offset: 0x0015F0D0
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x0600485F RID: 18527 RVA: 0x00160F1C File Offset: 0x0015F11C
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06004860 RID: 18528 RVA: 0x00160F48 File Offset: 0x0015F148
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x040051C0 RID: 20928
		private IList<JsonData> inst_array;

		// Token: 0x040051C1 RID: 20929
		private bool inst_boolean;

		// Token: 0x040051C2 RID: 20930
		private double inst_double;

		// Token: 0x040051C3 RID: 20931
		private int inst_int;

		// Token: 0x040051C4 RID: 20932
		private long inst_long;

		// Token: 0x040051C5 RID: 20933
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x040051C6 RID: 20934
		private string inst_string;

		// Token: 0x040051C7 RID: 20935
		private string json;

		// Token: 0x040051C8 RID: 20936
		private JsonType type;

		// Token: 0x040051C9 RID: 20937
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}
