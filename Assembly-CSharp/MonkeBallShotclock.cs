using System;
using TMPro;
using UnityEngine;

// Token: 0x0200050A RID: 1290
public class MonkeBallShotclock : MonoBehaviour
{
	// Token: 0x06001F7C RID: 8060 RVA: 0x000A67F8 File Offset: 0x000A49F8
	private void Update()
	{
		if (this._time >= 0f)
		{
			this._time -= Time.deltaTime;
			this.UpdateTimeText(this._time);
			if (this._time < 0f)
			{
				this.SetBackboard(this.neutralMaterial);
			}
		}
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000A684C File Offset: 0x000A4A4C
	public void SetTime(int teamId, float time)
	{
		this._time = time;
		if (teamId == -1)
		{
			this._time = 0f;
			this.SetBackboard(this.neutralMaterial);
		}
		else if (teamId >= 0 && teamId < this.teamMaterials.Length)
		{
			this.SetBackboard(this.teamMaterials[teamId]);
		}
		this.UpdateTimeText(time);
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000A68A1 File Offset: 0x000A4AA1
	private void SetBackboard(Material teamMaterial)
	{
		if (this.backboard != null)
		{
			this.backboard.material = teamMaterial;
		}
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000A68C0 File Offset: 0x000A4AC0
	private void UpdateTimeText(float time)
	{
		int num = Mathf.CeilToInt(time);
		if (this._timeInt != num)
		{
			this._timeInt = num;
			this.timeRemainingLabel.text = this._timeInt.ToString("#00");
		}
	}

	// Token: 0x04002812 RID: 10258
	public Renderer backboard;

	// Token: 0x04002813 RID: 10259
	public Material[] teamMaterials;

	// Token: 0x04002814 RID: 10260
	public Material neutralMaterial;

	// Token: 0x04002815 RID: 10261
	public TextMeshPro timeRemainingLabel;

	// Token: 0x04002816 RID: 10262
	private float _time;

	// Token: 0x04002817 RID: 10263
	private int _timeInt = -1;
}
