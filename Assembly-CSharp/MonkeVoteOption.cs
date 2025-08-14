using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200012B RID: 299
public class MonkeVoteOption : MonoBehaviour
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x060007C3 RID: 1987 RVA: 0x0002C024 File Offset: 0x0002A224
	// (remove) Token: 0x060007C4 RID: 1988 RVA: 0x0002C05C File Offset: 0x0002A25C
	public event Action<MonkeVoteOption, Collider> OnVote;

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060007C5 RID: 1989 RVA: 0x0002C091 File Offset: 0x0002A291
	// (set) Token: 0x060007C6 RID: 1990 RVA: 0x0002C09C File Offset: 0x0002A29C
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060007C7 RID: 1991 RVA: 0x0002C0BE File Offset: 0x0002A2BE
	// (set) Token: 0x060007C8 RID: 1992 RVA: 0x0002C0C8 File Offset: 0x0002A2C8
	public bool CanVote
	{
		get
		{
			return this._canVote;
		}
		set
		{
			Collider trigger = this._trigger;
			this._canVote = value;
			trigger.enabled = value;
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0002C0EA File Offset: 0x0002A2EA
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0002C0F4 File Offset: 0x0002A2F4
	private void Configure()
	{
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			if (collider.isTrigger)
			{
				this._trigger = collider;
				break;
			}
		}
		if (!this._optionText)
		{
			this._optionText = base.GetComponentInChildren<TMP_Text>();
		}
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002C144 File Offset: 0x0002A344
	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsValidVotingRock(other))
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0002C164 File Offset: 0x0002A364
	private bool IsValidVotingRock(Collider other)
	{
		SlingshotProjectile component = other.GetComponent<SlingshotProjectile>();
		return component && component.projectileOwner.IsLocal;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0002C18D File Offset: 0x0002A38D
	public void ResetState()
	{
		this.OnVote = null;
		this.ShowIndicators(false, false, true);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002C19F File Offset: 0x0002A39F
	public void ShowIndicators(bool showVote, bool showPrediction, bool instant = true)
	{
		this._voteIndicator.SetVisible(showVote, instant);
		this._guessIndicator.SetVisible(showPrediction, instant);
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0002C1BB File Offset: 0x0002A3BB
	private void Vote()
	{
		this.SendVote(null);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002C1C4 File Offset: 0x0002A3C4
	private void SendVote(Collider other)
	{
		if (!this._canVote)
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002C1E1 File Offset: 0x0002A3E1
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._voteIndicator.SetVisible(visible, true);
		this._guessIndicator.SetVisible(visible, true);
	}

	// Token: 0x04000969 RID: 2409
	[SerializeField]
	private Collider _trigger;

	// Token: 0x0400096A RID: 2410
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x0400096B RID: 2411
	[SerializeField]
	private VotingCard _voteIndicator;

	// Token: 0x0400096C RID: 2412
	[FormerlySerializedAs("_predictionIndicator")]
	[SerializeField]
	private VotingCard _guessIndicator;

	// Token: 0x0400096E RID: 2414
	private string _text = string.Empty;

	// Token: 0x0400096F RID: 2415
	private bool _canVote;
}
