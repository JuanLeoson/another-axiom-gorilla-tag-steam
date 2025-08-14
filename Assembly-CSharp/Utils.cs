using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000B05 RID: 2821
public static class Utils
{
	// Token: 0x060043E1 RID: 17377 RVA: 0x00154C74 File Offset: 0x00152E74
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		PooledList<IPreDisable> pooledList = Utils.g_listPool.Take();
		List<IPreDisable> list = pooledList.List;
		target.GetComponents<IPreDisable>(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				list[i].PreDisable();
			}
			catch (Exception)
			{
			}
		}
		target.SetActive(false);
		Utils.g_listPool.Return(pooledList);
	}

	// Token: 0x060043E2 RID: 17378 RVA: 0x00154CEC File Offset: 0x00152EEC
	public static void AddIfNew<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
		}
	}

	// Token: 0x060043E3 RID: 17379 RVA: 0x00154CFE File Offset: 0x00152EFE
	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.AllNetPlayers.Contains(player);
	}

	// Token: 0x060043E4 RID: 17380 RVA: 0x00154D20 File Offset: 0x00152F20
	public static bool PlayerInRoom(int actorNumber)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (allNetPlayers[i].ActorNumber == actorNumber)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060043E5 RID: 17381 RVA: 0x00154D60 File Offset: 0x00152F60
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x060043E6 RID: 17382 RVA: 0x00154D7F File Offset: 0x00152F7F
	public static bool PlayerInRoom(int actorNumber, out NetPlayer player)
	{
		if (NetworkSystem.Instance == null)
		{
			player = null;
			return false;
		}
		player = NetworkSystem.Instance.GetPlayer(actorNumber);
		return NetworkSystem.Instance.InRoom && player != null;
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x00154DB4 File Offset: 0x00152FB4
	public static long PackVector3ToLong(Vector3 vector)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x00154E38 File Offset: 0x00153038
	public static Vector3 UnpackVector3FromLong(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = data >> 21 & 2097151L;
		long num3 = data >> 42 & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	// Token: 0x060043E9 RID: 17385 RVA: 0x00154E96 File Offset: 0x00153096
	public static bool IsASCIILetterOrDigit(char c)
	{
		return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z');
	}

	// Token: 0x060043EA RID: 17386 RVA: 0x000023F5 File Offset: 0x000005F5
	public static void Log(object message)
	{
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x000023F5 File Offset: 0x000005F5
	public static void Log(object message, Object context)
	{
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x00154EC0 File Offset: 0x001530C0
	public static bool ValidateServerTime(double time, double maximumLatency)
	{
		double simTime = NetworkSystem.Instance.SimTime;
		double num = 4294967.295 - maximumLatency;
		double num2;
		if (simTime > maximumLatency || time < maximumLatency)
		{
			if (time > simTime)
			{
				return false;
			}
			num2 = simTime - time;
		}
		else
		{
			double num3 = num + simTime;
			if (time > simTime && time < num3)
			{
				return false;
			}
			num2 = simTime + (4294967.295 - time);
		}
		return num2 <= maximumLatency;
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x00154F20 File Offset: 0x00153120
	public static double CalculateNetworkDeltaTime(double prevTime, double newTime)
	{
		if (newTime >= prevTime)
		{
			return newTime - prevTime;
		}
		double num = 4294967.295 - prevTime;
		return newTime + num;
	}

	// Token: 0x04004E57 RID: 20055
	private static ObjectPool<PooledList<IPreDisable>> g_listPool = new ObjectPool<PooledList<IPreDisable>>(2, 10);

	// Token: 0x04004E58 RID: 20056
	private static StringBuilder reusableSB = new StringBuilder();
}
