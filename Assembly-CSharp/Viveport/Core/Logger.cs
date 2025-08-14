using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x02000BAE RID: 2990
	public class Logger
	{
		// Token: 0x060047E9 RID: 18409 RVA: 0x001600BC File Offset: 0x0015E2BC
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x060047EA RID: 18410 RVA: 0x001600D9 File Offset: 0x0015E2D9
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x060047EB RID: 18411 RVA: 0x001600E8 File Offset: 0x0015E2E8
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[]
				{
					typeof(string)
				}).Invoke(null, new object[]
				{
					message
				});
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x060047EC RID: 18412 RVA: 0x00160174 File Offset: 0x0015E374
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x040051B3 RID: 20915
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x040051B4 RID: 20916
		private static bool _hasDetected;

		// Token: 0x040051B5 RID: 20917
		private static bool _usingUnityLog = true;

		// Token: 0x040051B6 RID: 20918
		private static Type _unityLogType;
	}
}
