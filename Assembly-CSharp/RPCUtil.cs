using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006AD RID: 1709
internal class RPCUtil
{
	// Token: 0x06002A15 RID: 10773 RVA: 0x000E1124 File Offset: 0x000DF324
	public static bool NotSpam(string id, PhotonMessageInfoWrapped info, float delay)
	{
		RPCUtil.RPCCallID key = new RPCUtil.RPCCallID(id, info.senderID);
		if (!RPCUtil.RPCCallLog.ContainsKey(key))
		{
			RPCUtil.RPCCallLog.Add(key, Time.time);
			return true;
		}
		if (Time.time - RPCUtil.RPCCallLog[key] > delay)
		{
			RPCUtil.RPCCallLog[key] = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x000E1185 File Offset: 0x000DF385
	public static bool SafeValue(float v)
	{
		return !float.IsNaN(v) && float.IsFinite(v);
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000E1197 File Offset: 0x000DF397
	public static bool SafeValue(float v, float min, float max)
	{
		return RPCUtil.SafeValue(v) && v <= max && v >= min;
	}

	// Token: 0x040035D4 RID: 13780
	private static Dictionary<RPCUtil.RPCCallID, float> RPCCallLog = new Dictionary<RPCUtil.RPCCallID, float>();

	// Token: 0x020006AE RID: 1710
	private struct RPCCallID : IEquatable<RPCUtil.RPCCallID>
	{
		// Token: 0x06002A1A RID: 10778 RVA: 0x000E11BC File Offset: 0x000DF3BC
		public RPCCallID(string nameOfFunction, int senderId)
		{
			this._senderID = senderId;
			this._nameOfFunction = nameOfFunction;
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06002A1B RID: 10779 RVA: 0x000E11CC File Offset: 0x000DF3CC
		public readonly int SenderID
		{
			get
			{
				return this._senderID;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06002A1C RID: 10780 RVA: 0x000E11D4 File Offset: 0x000DF3D4
		public readonly string NameOfFunction
		{
			get
			{
				return this._nameOfFunction;
			}
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x000E11DC File Offset: 0x000DF3DC
		bool IEquatable<RPCUtil.RPCCallID>.Equals(RPCUtil.RPCCallID other)
		{
			return other.NameOfFunction.Equals(this.NameOfFunction) && other.SenderID.Equals(this.SenderID);
		}

		// Token: 0x040035D5 RID: 13781
		private int _senderID;

		// Token: 0x040035D6 RID: 13782
		private string _nameOfFunction;
	}
}
