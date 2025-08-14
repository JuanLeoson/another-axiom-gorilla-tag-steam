using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007B6 RID: 1974
public class StopwatchFace : MonoBehaviour
{
	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06003183 RID: 12675 RVA: 0x001015D5 File Offset: 0x000FF7D5
	public bool watchActive
	{
		get
		{
			return this._watchActive;
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06003184 RID: 12676 RVA: 0x001015DD File Offset: 0x000FF7DD
	public int millisElapsed
	{
		get
		{
			return this._millisElapsed;
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06003185 RID: 12677 RVA: 0x001015E5 File Offset: 0x000FF7E5
	public Vector3Int digitsMmSsMs
	{
		get
		{
			return StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		}
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x001015F8 File Offset: 0x000FF7F8
	public void SetMillisElapsed(int millis, bool updateFace = true)
	{
		this._millisElapsed = millis;
		if (!updateFace)
		{
			return;
		}
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x00101611 File Offset: 0x000FF811
	private void Awake()
	{
		this._lerpToZero = new LerpTask<int>();
		this._lerpToZero.onLerp = new Action<int, int, float>(this.OnLerpToZero);
		this._lerpToZero.onLerpEnd = new Action(this.OnLerpEnd);
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x0010164C File Offset: 0x000FF84C
	private void OnLerpToZero(int a, int b, float t)
	{
		this._millisElapsed = Mathf.FloorToInt(Mathf.Lerp((float)a, (float)b, t * t));
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x00101671 File Offset: 0x000FF871
	private void OnLerpEnd()
	{
		this.WatchReset(false);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x00101671 File Offset: 0x000FF871
	private void OnEnable()
	{
		this.WatchReset(false);
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x00101671 File Offset: 0x000FF871
	private void OnDisable()
	{
		this.WatchReset(false);
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x0010167C File Offset: 0x000FF87C
	private void Update()
	{
		if (this._lerpToZero.active)
		{
			this._lerpToZero.Update();
			return;
		}
		if (this._watchActive)
		{
			this._millisElapsed += Mathf.FloorToInt(Time.deltaTime * 1000f);
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x001016D4 File Offset: 0x000FF8D4
	private static Vector3Int ParseDigits(TimeSpan time)
	{
		int num = (int)time.TotalMinutes % 100;
		double num2 = 60.0 * (time.TotalMinutes - (double)num);
		int num3 = (int)num2;
		int num4 = (int)(100.0 * (num2 - (double)num3));
		num = Math.Clamp(num, 0, 99);
		num3 = Math.Clamp(num3, 0, 59);
		num4 = Math.Clamp(num4, 0, 99);
		return new Vector3Int(num, num3, num4);
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x0010173C File Offset: 0x000FF93C
	private void UpdateText()
	{
		Vector3Int vector3Int = StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		string text = vector3Int.x.ToString("D2");
		string text2 = vector3Int.y.ToString("D2");
		string text3 = vector3Int.z.ToString("D2");
		this._text.text = string.Concat(new string[]
		{
			text,
			":",
			text2,
			":",
			text3
		});
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x001017D0 File Offset: 0x000FF9D0
	private void UpdateHand()
	{
		float z = (float)(this._millisElapsed % 60000) / 60000f * 360f;
		this._hand.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x00101812 File Offset: 0x000FFA12
	public void WatchToggle()
	{
		if (!this._watchActive)
		{
			this.WatchStart();
			return;
		}
		this.WatchStop();
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x00101829 File Offset: 0x000FFA29
	public void WatchStart()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = true;
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x00101840 File Offset: 0x000FFA40
	public void WatchStop()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = false;
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x00101857 File Offset: 0x000FFA57
	public void WatchReset()
	{
		this.WatchReset(true);
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x00101860 File Offset: 0x000FFA60
	public void WatchReset(bool doLerp)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (doLerp)
		{
			if (!this._lerpToZero.active)
			{
				this._lerpToZero.Start(this._millisElapsed % 60000, 0, 0.36f);
				return;
			}
		}
		else
		{
			this._watchActive = false;
			this._millisElapsed = 0;
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x04003D30 RID: 15664
	[SerializeField]
	private Transform _hand;

	// Token: 0x04003D31 RID: 15665
	[SerializeField]
	private Text _text;

	// Token: 0x04003D32 RID: 15666
	[Space]
	[SerializeField]
	private StopwatchCosmetic _cosmetic;

	// Token: 0x04003D33 RID: 15667
	[Space]
	[SerializeField]
	private AudioClip _audioClick;

	// Token: 0x04003D34 RID: 15668
	[SerializeField]
	private AudioClip _audioReset;

	// Token: 0x04003D35 RID: 15669
	[SerializeField]
	private AudioClip _audioTick;

	// Token: 0x04003D36 RID: 15670
	[Space]
	[NonSerialized]
	private int _millisElapsed;

	// Token: 0x04003D37 RID: 15671
	[NonSerialized]
	private bool _watchActive;

	// Token: 0x04003D38 RID: 15672
	[NonSerialized]
	private LerpTask<int> _lerpToZero;
}
