using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020001CD RID: 461
[Serializable]
public struct GTDateTimeSerializable : ISerializationCallbackReceiver
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0003FDEA File Offset: 0x0003DFEA
	// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0003FDF2 File Offset: 0x0003DFF2
	public DateTime dateTime
	{
		get
		{
			return this._dateTime;
		}
		set
		{
			this._dateTime = value;
			this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
		}
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003FE0C File Offset: 0x0003E00C
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0003FE20 File Offset: 0x0003E020
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DateTime dateTime;
		if (GTDateTimeSerializable.TryParseDateTime(this._dateTimeString, out dateTime))
		{
			this._dateTime = dateTime;
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0003FE44 File Offset: 0x0003E044
	public GTDateTimeSerializable(int dummyValue)
	{
		DateTime now = DateTime.Now;
		this._dateTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0003FE8C File Offset: 0x0003E08C
	private static string FormatDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003FE9C File Offset: 0x0003E09C
	private static bool TryParseDateTime(string value, out DateTime result)
	{
		if (DateTime.TryParseExact(value, new string[]
		{
			"yyyy-MM-dd HH:mm",
			"yyyy-MM-dd",
			"yyyy-MM"
		}, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
		{
			DateTime dateTime = result;
			if (dateTime.Hour == 0 && dateTime.Minute == 0)
			{
				result = result.AddHours(11.0);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04000E20 RID: 3616
	[HideInInspector]
	[SerializeField]
	private string _dateTimeString;

	// Token: 0x04000E21 RID: 3617
	private DateTime _dateTime;
}
