using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class GameModeSelectorButtonLayout : MonoBehaviour
{
	// Token: 0x06000396 RID: 918 RVA: 0x00016453 File Offset: 0x00014653
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += this.SetupButtons;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0001647C File Offset: 0x0001467C
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.SetupButtons;
	}

	// Token: 0x06000398 RID: 920 RVA: 0x000164A0 File Offset: 0x000146A0
	private void SetupButtons()
	{
		GameModeSelectorButtonLayout.<SetupButtons>d__6 <SetupButtons>d__;
		<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetupButtons>d__.<>4__this = this;
		<SetupButtons>d__.<>1__state = -1;
		<SetupButtons>d__.<>t__builder.Start<GameModeSelectorButtonLayout.<SetupButtons>d__6>(ref <SetupButtons>d__);
	}

	// Token: 0x04000413 RID: 1043
	[SerializeField]
	private ModeSelectButton pf_button;

	// Token: 0x04000414 RID: 1044
	[SerializeField]
	private GTZone zone;

	// Token: 0x04000415 RID: 1045
	[SerializeField]
	private PartyGameModeWarning warningScreen;

	// Token: 0x04000416 RID: 1046
	private List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
