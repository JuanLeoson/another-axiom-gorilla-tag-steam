using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200050B RID: 1291
public class MonkeBallTeamSelector : MonoBehaviour
{
	// Token: 0x06001F81 RID: 8065 RVA: 0x000A690E File Offset: 0x000A4B0E
	public void Awake()
	{
		this._setTeamButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x000A692C File Offset: 0x000A4B2C
	public void OnDestroy()
	{
		this._setTeamButton.onPressButton.RemoveListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x000A694A File Offset: 0x000A4B4A
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestSetTeam(this.teamId);
	}

	// Token: 0x04002818 RID: 10264
	public int teamId;

	// Token: 0x04002819 RID: 10265
	[SerializeField]
	private GorillaPressableButton _setTeamButton;
}
