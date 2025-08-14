using System;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000618 RID: 1560
public class GRDistillery : MonoBehaviour
{
	// Token: 0x0600262E RID: 9774 RVA: 0x000CC02C File Offset: 0x000CA22C
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		this.researchManager = reactor.GetComponent<GRResearchManager>();
		this.sentientCoreDeposit.Init(reactor);
		this.cores = PlayerPrefs.GetInt("_grDistilleryCore", -1);
		if (this.cores == -1)
		{
			this.cores = 0;
		}
		this.RestoreStartTime();
		this.InitializeGauges();
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000CC088 File Offset: 0x000CA288
	private void SaveStartTime(DateTime time)
	{
		string value = time.ToString("O");
		PlayerPrefs.SetString("_grDistilleryStartTime", value);
		PlayerPrefs.Save();
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000CC0B4 File Offset: 0x000CA2B4
	private void RestoreStartTime()
	{
		string @string = PlayerPrefs.GetString("_grDistilleryStartTime", string.Empty);
		if (@string != string.Empty)
		{
			this.startTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000CC0F9 File Offset: 0x000CA2F9
	public void StartResearch()
	{
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime();
			this.SaveStartTime(this.startTime);
			this.bProcessing = true;
			this.InitializeGauges();
		}
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000CC130 File Offset: 0x000CA330
	public double CalculateRemaining()
	{
		return (double)this.secondsToResearchACore - (GorillaComputer.instance.GetServerTime() - this.startTime).TotalSeconds;
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000CC164 File Offset: 0x000CA364
	private void FirstUpdate()
	{
		double num = this.CalculateRemaining();
		while (this.cores > 0 && num < (double)(-(double)this.secondsToResearchACore))
		{
			if (num < (double)(-(double)this.secondsToResearchACore))
			{
				this.CompleteResearchingCore();
				num += (double)this.secondsToResearchACore;
			}
		}
		if (this.cores > 0 && num < 0.0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(num);
			num = this.CalculateRemaining();
			this.SaveStartTime(this.startTime);
		}
		if (this.cores > 0)
		{
			this.bProcessing = true;
			this.currentGaugeCore = this.cores - 1;
		}
		else
		{
			this.currentGaugeCore = 0;
		}
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = this.depositClosePosition.position;
		}
		else
		{
			this.depositDoor.transform.position = this.depositOpenPosition.position;
		}
		this.UpdateGauges();
		this.currentResearchPoints.text = this.researchManager.playerResearchPoints.ToString();
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000CC278 File Offset: 0x000CA478
	public void Update()
	{
		if (!this.firstUpdate)
		{
			this.FirstUpdate();
			this.firstUpdate = true;
		}
		this.UpdateDoorPosition();
		this.UpdateGauges();
		if (!this.bProcessing)
		{
			return;
		}
		this.remaingTime = this.CalculateRemaining();
		if (this.remaingTime <= 0.0)
		{
			this.CompleteResearchingCore();
		}
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000CC2D4 File Offset: 0x000CA4D4
	private void UpdateDoorPosition()
	{
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositClosePosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
			return;
		}
		this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositOpenPosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000CC370 File Offset: 0x000CA570
	private void CompleteResearchingCore()
	{
		this.researchManager.CompletedResearch();
		this.cores = Math.Max(this.cores - 1, 0);
		this.currentGaugeCore = Math.Max(this.cores - 1, 0);
		PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
		PlayerPrefs.Save();
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(this.remaingTime);
			this.SaveStartTime(this.startTime);
			this.remaingTime = this.CalculateRemaining();
		}
		if (this.cores == 0)
		{
			this.bProcessing = false;
		}
		this.UpdateGauges();
		this.currentResearchPoints.text = this.researchManager.playerResearchPoints.ToString();
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000CC43C File Offset: 0x000CA63C
	public void DepositCore()
	{
		if (this.cores < this.maxCores)
		{
			this.cores++;
			if (!this.bFillingGauge)
			{
				this.bFillingGauge = true;
				this.fillTime = 0f;
			}
			PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
			PlayerPrefs.Save();
			if (this.cores == 1)
			{
				this.StartResearch();
			}
		}
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000023F5 File Offset: 0x000005F5
	public void DebugFinishDistill()
	{
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000CC4A4 File Offset: 0x000CA6A4
	private void OnEnable()
	{
		if (this._applyMaterialgauge1)
		{
			this._applyMaterialgauge1.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge2)
		{
			this._applyMaterialgauge2.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge3)
		{
			this._applyMaterialgauge3.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge4)
		{
			this._applyMaterialgauge4.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		this.InitializeGauges();
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000CC51C File Offset: 0x000CA71C
	private void InitializeGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length - 1; i++)
		{
			this.gaugesFill[i] = ((this.cores >= i + 1) ? this.gaugeFullFillAmount : this.gaugeEmptyFillAmount);
		}
		this.researchGaugeFill = this.gaugesFill[0];
		this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000CC57C File Offset: 0x000CA77C
	private void UpdateGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length; i++)
		{
			if (i + 1 > this.cores)
			{
				this.gaugesFill[i] = this.gaugeEmptyFillAmount;
			}
		}
		if (this.bFillingGauge)
		{
			this.fillTime += Time.deltaTime;
			float num = this.fillTime / this.gaugeDrainTime;
			if (this.currentGaugeCore == this.cores - 1)
			{
				if (num > 1f)
				{
					this.bFillingGauge = false;
				}
				else
				{
					this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore), num);
				}
			}
			else
			{
				this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, this.gaugeFullFillAmount, num);
			}
			if (this.bFillingGauge && num > 1f)
			{
				this.currentGaugeCore++;
				this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
				this.fillTime = 0f;
			}
		}
		else if (this.bProcessing)
		{
			this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore);
			this.currentGaugeFillAmount = this.gaugesFill[this.currentGaugeCore];
		}
		this._applyMaterialgauge1.SetFloat("_LiquidFill", this.gaugesFill[0]);
		this._applyMaterialgauge1.Apply();
		this._applyMaterialgauge2.SetFloat("_LiquidFill", this.gaugesFill[1]);
		this._applyMaterialgauge2.Apply();
		this._applyMaterialgauge3.SetFloat("_LiquidFill", this.gaugesFill[2]);
		this._applyMaterialgauge3.Apply();
		this._applyMaterialgauge4.SetFloat("_LiquidFill", this.gaugesFill[3]);
		this._applyMaterialgauge4.Apply();
		this._applyMaterialCurrentResearch.SetFloat("_LiquidFill", this.researchGaugeFill);
		this._applyMaterialCurrentResearch.Apply();
	}

	// Token: 0x04003067 RID: 12391
	[SerializeField]
	private GRCurrencyDepositor sentientCoreDeposit;

	// Token: 0x04003068 RID: 12392
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge1;

	// Token: 0x04003069 RID: 12393
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge2;

	// Token: 0x0400306A RID: 12394
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge3;

	// Token: 0x0400306B RID: 12395
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge4;

	// Token: 0x0400306C RID: 12396
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialCurrentResearch;

	// Token: 0x0400306D RID: 12397
	[FormerlySerializedAs("emptyFillAmount")]
	public float gaugeEmptyFillAmount = 0.44f;

	// Token: 0x0400306E RID: 12398
	[FormerlySerializedAs("fullFillAmount")]
	public float gaugeFullFillAmount = 0.56f;

	// Token: 0x0400306F RID: 12399
	[SerializeField]
	private Transform depositClosePosition;

	// Token: 0x04003070 RID: 12400
	[SerializeField]
	private Transform depositOpenPosition;

	// Token: 0x04003071 RID: 12401
	[SerializeField]
	private GameObject depositDoor;

	// Token: 0x04003072 RID: 12402
	[SerializeField]
	private float depositDoorCloseSpeed = 0.5f;

	// Token: 0x04003073 RID: 12403
	[SerializeField]
	private TextMeshPro currentResearchPoints;

	// Token: 0x04003074 RID: 12404
	public float researchGaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003075 RID: 12405
	public float researchGaugeFullFillAmount = 0.56f;

	// Token: 0x04003076 RID: 12406
	public int secondsToResearchACore;

	// Token: 0x04003077 RID: 12407
	public float gaugeDrainTime = 2f;

	// Token: 0x04003078 RID: 12408
	public int maxCores = 4;

	// Token: 0x04003079 RID: 12409
	public AudioSource feedbackSound;

	// Token: 0x0400307A RID: 12410
	private DateTime startTime;

	// Token: 0x0400307B RID: 12411
	private bool bProcessing;

	// Token: 0x0400307C RID: 12412
	private int cores;

	// Token: 0x0400307D RID: 12413
	private bool bFillingGauge;

	// Token: 0x0400307E RID: 12414
	private int currentGaugeCore;

	// Token: 0x0400307F RID: 12415
	private float currentGaugeFillAmount;

	// Token: 0x04003080 RID: 12416
	private double remaingTime;

	// Token: 0x04003081 RID: 12417
	private float fillTime;

	// Token: 0x04003082 RID: 12418
	private float[] gaugesFill = new float[4];

	// Token: 0x04003083 RID: 12419
	private float researchGaugeFill;

	// Token: 0x04003084 RID: 12420
	private bool firstUpdate;

	// Token: 0x04003085 RID: 12421
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x04003086 RID: 12422
	[NonSerialized]
	public GRResearchManager researchManager;

	// Token: 0x04003087 RID: 12423
	private const string grDistilleryCorePrefsKey = "_grDistilleryCore";

	// Token: 0x04003088 RID: 12424
	private const string grDistilleryStartTimePrefsKey = "_grDistilleryStartTime";
}
