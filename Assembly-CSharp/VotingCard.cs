using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class VotingCard : MonoBehaviour
{
	// Token: 0x060007E4 RID: 2020 RVA: 0x0002C52A File Offset: 0x0002A72A
	private void MoveToOffPosition()
	{
		this._card.transform.position = this._offPosition.position;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002C547 File Offset: 0x0002A747
	private void MoveToOnPosition()
	{
		this._card.transform.position = this._onPosition.position;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002C564 File Offset: 0x0002A764
	public void SetVisible(bool showVote, bool instant)
	{
		if (this._isVisible != showVote)
		{
			base.StopAllCoroutines();
		}
		if (instant)
		{
			this._card.transform.position = (showVote ? this._onPosition.position : this._offPosition.position);
			this._card.SetActive(showVote);
		}
		else if (showVote)
		{
			if (this._isVisible != showVote)
			{
				base.StartCoroutine(this.DoActivate());
			}
		}
		else
		{
			this._card.SetActive(false);
			this._card.transform.position = this._offPosition.position;
		}
		this._isVisible = showVote;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002C605 File Offset: 0x0002A805
	private IEnumerator DoActivate()
	{
		Vector3 from = this._offPosition.position;
		Vector3 to = this._onPosition.position;
		this._card.transform.position = from;
		this._card.SetActive(true);
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / this.activationTime;
			this._card.transform.position = Vector3.Lerp(from, to, lerpVal);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000985 RID: 2437
	[SerializeField]
	private GameObject _card;

	// Token: 0x04000986 RID: 2438
	[SerializeField]
	private Transform _offPosition;

	// Token: 0x04000987 RID: 2439
	[SerializeField]
	private Transform _onPosition;

	// Token: 0x04000988 RID: 2440
	[SerializeField]
	private float activationTime = 0.5f;

	// Token: 0x04000989 RID: 2441
	private bool _isVisible;
}
