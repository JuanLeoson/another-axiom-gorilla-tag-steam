using System;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006C9 RID: 1737
public class GorillaGuardianEjectWatch : MonoBehaviour
{
	// Token: 0x06002B23 RID: 11043 RVA: 0x000E453C File Offset: 0x000E273C
	private void Start()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.AddListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000E4568 File Offset: 0x000E2768
	private void OnDestroy()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.RemoveListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000E4594 File Offset: 0x000E2794
	private void OnEjectButtonPressed()
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x0400368B RID: 13963
	[SerializeField]
	private HeldButton ejectButton;
}
