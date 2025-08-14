using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000507 RID: 1287
public class MonkeBallResetGame : MonoBehaviour
{
	// Token: 0x06001F6A RID: 8042 RVA: 0x000A6504 File Offset: 0x000A4704
	private void Awake()
	{
		this._resetButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
		if (this._resetButton == null)
		{
			this._buttonOrigin = this._resetButton.transform.position;
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000A6551 File Offset: 0x000A4751
	private void Update()
	{
		if (this._cooldown)
		{
			this._cooldownTimer -= Time.deltaTime;
			if (this._cooldownTimer <= 0f)
			{
				this.ToggleButton(false, -1);
				this._cooldown = false;
			}
		}
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000A658C File Offset: 0x000A478C
	public void ToggleReset(bool toggle, int teamId, bool force = false)
	{
		if (teamId < -1 || teamId >= this.teamMaterials.Length)
		{
			return;
		}
		if (toggle)
		{
			this.ToggleButton(true, teamId);
			this._cooldown = false;
			return;
		}
		if (force)
		{
			this.ToggleButton(false, -1);
			return;
		}
		this._cooldown = true;
		this._cooldownTimer = 3f;
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000A65DC File Offset: 0x000A47DC
	private void ToggleButton(bool toggle, int teamId)
	{
		this._resetButton.enabled = toggle;
		this.allowedTeamId = teamId;
		if (!toggle || teamId == -1)
		{
			this.button.sharedMaterial = this.neutralMaterial;
			return;
		}
		this.button.sharedMaterial = this.teamMaterials[teamId];
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000A6628 File Offset: 0x000A4828
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestResetGame();
	}

	// Token: 0x040027F8 RID: 10232
	[SerializeField]
	private GorillaPressableButton _resetButton;

	// Token: 0x040027F9 RID: 10233
	public Renderer button;

	// Token: 0x040027FA RID: 10234
	public Vector3 buttonPressOffset;

	// Token: 0x040027FB RID: 10235
	private Vector3 _buttonOrigin = Vector3.zero;

	// Token: 0x040027FC RID: 10236
	[Space]
	public Material[] teamMaterials;

	// Token: 0x040027FD RID: 10237
	public Material neutralMaterial;

	// Token: 0x040027FE RID: 10238
	public int allowedTeamId = -1;

	// Token: 0x040027FF RID: 10239
	[SerializeField]
	private TextMeshPro _resetLabel;

	// Token: 0x04002800 RID: 10240
	private bool _cooldown;

	// Token: 0x04002801 RID: 10241
	private float _cooldownTimer;
}
