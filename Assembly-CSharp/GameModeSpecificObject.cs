using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class GameModeSpecificObject : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600039F RID: 927 RVA: 0x000167D4 File Offset: 0x000149D4
	// (remove) Token: 0x060003A0 RID: 928 RVA: 0x00016808 File Offset: 0x00014A08
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060003A1 RID: 929 RVA: 0x0001683C File Offset: 0x00014A3C
	// (remove) Token: 0x060003A2 RID: 930 RVA: 0x00016870 File Offset: 0x00014A70
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060003A3 RID: 931 RVA: 0x000168A3 File Offset: 0x00014AA3
	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060003A4 RID: 932 RVA: 0x000168AB File Offset: 0x00014AAB
	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000168B4 File Offset: 0x00014AB4
	private void Awake()
	{
		GameModeSpecificObject.<Awake>d__15 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameModeSpecificObject.<Awake>d__15>(ref <Awake>d__);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x000168EB File Offset: 0x00014AEB
	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000168FF File Offset: 0x00014AFF
	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	// Token: 0x04000423 RID: 1059
	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	// Token: 0x04000424 RID: 1060
	[SerializeField]
	private GameModeType[] _gameModes;

	// Token: 0x04000425 RID: 1061
	private List<GameModeType> gameModes;

	// Token: 0x02000091 RID: 145
	// (Invoke) Token: 0x060003AA RID: 938
	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	// Token: 0x02000092 RID: 146
	[Serializable]
	public enum ValidationMethod
	{
		// Token: 0x04000427 RID: 1063
		Inclusion,
		// Token: 0x04000428 RID: 1064
		Exclusion
	}
}
