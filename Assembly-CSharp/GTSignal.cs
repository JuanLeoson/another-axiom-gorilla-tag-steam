using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200073A RID: 1850
public static class GTSignal
{
	// Token: 0x06002E3A RID: 11834 RVA: 0x000F4FE8 File Offset: 0x000F31E8
	private static void _Emit(GTSignal.EmitMode mode, int signalID, object[] data)
	{
		object[] eventContent = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, eventContent, GTSignal.gTargetsToOptions[mode], GTSignal.gSendOptions);
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000F5020 File Offset: 0x000F3220
	private static void _Emit(int[] targetActors, int signalID, object[] data)
	{
		if (targetActors.IsNullOrEmpty<int>())
		{
			return;
		}
		GTSignal.gCustomTargetOptions.TargetActors = targetActors;
		object[] eventContent = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, eventContent, GTSignal.gCustomTargetOptions, GTSignal.gSendOptions);
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x000F5064 File Offset: 0x000F3264
	private static object[] _ToEventContent(int signalID, double time, object[] data)
	{
		int num = data.Length;
		int num2 = num + 2;
		object[] array;
		if (!GTSignal.gLengthToContentArray.TryGetValue(num2, out array))
		{
			array = new object[num2];
			GTSignal.gLengthToContentArray.Add(num2, array);
		}
		array[0] = signalID;
		array[1] = time;
		for (int i = 0; i < num; i++)
		{
			array[i + 2] = data[i];
		}
		return array;
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000F50C2 File Offset: 0x000F32C2
	public static int ComputeID(string s)
	{
		if (!string.IsNullOrWhiteSpace(s))
		{
			return XXHash32.Compute(s.Trim(), 0U);
		}
		return 0;
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000F50DC File Offset: 0x000F32DC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GTSignal.gCustomTargetOptions = RaiseEventOptions.Default;
		GTSignal.gSendOptions = SendOptions.SendReliable;
		GTSignal.gSendOptions.Encrypt = true;
		GTSignal.gTargetsToOptions = new Dictionary<GTSignal.EmitMode, RaiseEventOptions>(3);
		RaiseEventOptions @default = RaiseEventOptions.Default;
		@default.Receivers = ReceiverGroup.All;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.All, @default);
		RaiseEventOptions default2 = RaiseEventOptions.Default;
		default2.Receivers = ReceiverGroup.Others;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Others, default2);
		RaiseEventOptions default3 = RaiseEventOptions.Default;
		default3.Receivers = ReceiverGroup.MasterClient;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Host, default3);
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000F515E File Offset: 0x000F335E
	public static void Emit(string signal, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000F516D File Offset: 0x000F336D
	public static void Emit(GTSignal.EmitMode mode, string signal, params object[] data)
	{
		GTSignal._Emit(mode, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000F517C File Offset: 0x000F337C
	public static void Emit(int signalID, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, signalID, data);
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000F5186 File Offset: 0x000F3386
	public static void Emit(GTSignal.EmitMode mode, int signalID, params object[] data)
	{
		GTSignal._Emit(mode, signalID, data);
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000F5190 File Offset: 0x000F3390
	public static void Emit(int target, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[1];
		array[0] = target;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000F51A8 File Offset: 0x000F33A8
	public static void Emit(int target1, int target2, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[2];
		array[0] = target1;
		array[1] = target2;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000F51C4 File Offset: 0x000F33C4
	public static void Emit(int target1, int target2, int target3, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[3];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000F51E5 File Offset: 0x000F33E5
	public static void Emit(int target1, int target2, int target3, int target4, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[4];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000F520B File Offset: 0x000F340B
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[5];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000F5236 File Offset: 0x000F3436
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[6];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000F5266 File Offset: 0x000F3466
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[7];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000F529B File Offset: 0x000F349B
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[8];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000F52D5 File Offset: 0x000F34D5
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[9];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000F5318 File Offset: 0x000F3518
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int target10, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[10];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		array[9] = target10;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000F536C File Offset: 0x000F356C
	// Note: this type is marked as 'beforefieldinit'.
	static GTSignal()
	{
		Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
		dictionary[1] = new object[1];
		dictionary[2] = new object[2];
		dictionary[3] = new object[3];
		dictionary[4] = new object[4];
		dictionary[5] = new object[5];
		dictionary[6] = new object[6];
		dictionary[7] = new object[7];
		dictionary[8] = new object[8];
		dictionary[9] = new object[9];
		dictionary[10] = new object[10];
		dictionary[11] = new object[11];
		dictionary[12] = new object[12];
		dictionary[13] = new object[13];
		dictionary[14] = new object[14];
		dictionary[15] = new object[15];
		dictionary[16] = new object[16];
		GTSignal.gLengthToContentArray = dictionary;
		Dictionary<int, int[]> dictionary2 = new Dictionary<int, int[]>();
		dictionary2[1] = new int[1];
		dictionary2[2] = new int[2];
		dictionary2[3] = new int[3];
		dictionary2[4] = new int[4];
		dictionary2[5] = new int[5];
		dictionary2[6] = new int[6];
		dictionary2[7] = new int[7];
		dictionary2[8] = new int[8];
		dictionary2[9] = new int[9];
		dictionary2[10] = new int[10];
		GTSignal.gLengthToTargetsArray = dictionary2;
	}

	// Token: 0x04003A17 RID: 14871
	public const byte PHOTON_CODE = 186;

	// Token: 0x04003A18 RID: 14872
	private static Dictionary<GTSignal.EmitMode, RaiseEventOptions> gTargetsToOptions;

	// Token: 0x04003A19 RID: 14873
	private static Dictionary<int, object[]> gLengthToContentArray;

	// Token: 0x04003A1A RID: 14874
	private static Dictionary<int, int[]> gLengthToTargetsArray;

	// Token: 0x04003A1B RID: 14875
	private static SendOptions gSendOptions;

	// Token: 0x04003A1C RID: 14876
	private static RaiseEventOptions gCustomTargetOptions;

	// Token: 0x0200073B RID: 1851
	public enum EmitMode
	{
		// Token: 0x04003A1E RID: 14878
		None = -1,
		// Token: 0x04003A1F RID: 14879
		Others,
		// Token: 0x04003A20 RID: 14880
		Targets,
		// Token: 0x04003A21 RID: 14881
		All,
		// Token: 0x04003A22 RID: 14882
		Host
	}
}
