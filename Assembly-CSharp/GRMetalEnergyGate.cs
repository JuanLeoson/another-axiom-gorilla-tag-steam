using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000642 RID: 1602
public class GRMetalEnergyGate : MonoBehaviour
{
	// Token: 0x0600276A RID: 10090 RVA: 0x000D4A51 File Offset: 0x000D2C51
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000D4A84 File Offset: 0x000D2C84
	private void OnDisable()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange -= this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000D4ADC File Offset: 0x000D2CDC
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(chargingEntityId);
		GRPlayer grplayer = null;
		if (gameEntity != null)
		{
			grplayer = GRPlayer.Get(gameEntity.heldByActorNumber);
		}
		if (grplayer != null)
		{
			grplayer.coresSpentOnGates += energyChange;
		}
		if (this.state == GRMetalEnergyGate.State.Closed && tool.energy >= tool.GetEnergyMax())
		{
			if (grplayer != null)
			{
				grplayer.gatesUnlocked++;
			}
			this.SetState(GRMetalEnergyGate.State.Open);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000D4B82 File Offset: 0x000D2D82
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRMetalEnergyGate.State)nextState);
		}
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000D4B9C File Offset: 0x000D2D9C
	public void SetState(GRMetalEnergyGate.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.audioSource.PlayOneShot(this.doorOpenClip);
					for (int i = 0; i < this.enableObjectsOnOpen.Count; i++)
					{
						this.enableObjectsOnOpen[i].gameObject.SetActive(true);
					}
					for (int j = 0; j < this.disableObjectsOnOpen.Count; j++)
					{
						this.disableObjectsOnOpen[j].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.audioSource.PlayOneShot(this.doorCloseClip);
				for (int k = 0; k < this.enableObjectsOnOpen.Count; k++)
				{
					this.enableObjectsOnOpen[k].gameObject.SetActive(false);
				}
				for (int l = 0; l < this.disableObjectsOnOpen.Count; l++)
				{
					this.disableObjectsOnOpen[l].gameObject.SetActive(true);
				}
			}
			if (this.doorAnimationCoroutine == null)
			{
				this.doorAnimationCoroutine = base.StartCoroutine(this.UpdateDoorAnimation());
			}
		}
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000D4CC4 File Offset: 0x000D2EC4
	public void OpenGate()
	{
		this.SetState(GRMetalEnergyGate.State.Open);
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000D4CCD File Offset: 0x000D2ECD
	public void CloseGate()
	{
		this.SetState(GRMetalEnergyGate.State.Closed);
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000D4CD6 File Offset: 0x000D2ED6
	private IEnumerator UpdateDoorAnimation()
	{
		while ((this.state == GRMetalEnergyGate.State.Open && this.openProgress < 1f) || (this.state == GRMetalEnergyGate.State.Closed && this.openProgress > 0f))
		{
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.openProgress = Mathf.MoveTowards(this.openProgress, 1f, Time.deltaTime / this.doorOpenTime);
					float t = this.doorOpenCurve.Evaluate(this.openProgress);
					this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, t);
					this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, t);
				}
			}
			else
			{
				this.openProgress = Mathf.MoveTowards(this.openProgress, 0f, Time.deltaTime / this.doorOpenTime);
				float t2 = this.doorCloseCurve.Evaluate(this.openProgress);
				this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, t2);
				this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, t2);
			}
			yield return null;
		}
		this.doorAnimationCoroutine = null;
		yield break;
	}

	// Token: 0x04003294 RID: 12948
	[SerializeField]
	public GRMetalEnergyGate.DoorParams upperDoor;

	// Token: 0x04003295 RID: 12949
	[SerializeField]
	public GRMetalEnergyGate.DoorParams lowerDoor;

	// Token: 0x04003296 RID: 12950
	[SerializeField]
	private float doorOpenTime = 1.5f;

	// Token: 0x04003297 RID: 12951
	[SerializeField]
	private float doorCloseTime = 1.5f;

	// Token: 0x04003298 RID: 12952
	[SerializeField]
	private AnimationCurve doorOpenCurve;

	// Token: 0x04003299 RID: 12953
	[SerializeField]
	private AnimationCurve doorCloseCurve;

	// Token: 0x0400329A RID: 12954
	[SerializeField]
	private AudioClip doorOpenClip;

	// Token: 0x0400329B RID: 12955
	[SerializeField]
	private AudioClip doorCloseClip;

	// Token: 0x0400329C RID: 12956
	[SerializeField]
	private List<Transform> enableObjectsOnOpen = new List<Transform>();

	// Token: 0x0400329D RID: 12957
	[SerializeField]
	private List<Transform> disableObjectsOnOpen = new List<Transform>();

	// Token: 0x0400329E RID: 12958
	[SerializeField]
	private GRTool tool;

	// Token: 0x0400329F RID: 12959
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x040032A0 RID: 12960
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040032A1 RID: 12961
	public GRMetalEnergyGate.State state;

	// Token: 0x040032A2 RID: 12962
	private float openProgress;

	// Token: 0x040032A3 RID: 12963
	private Coroutine doorAnimationCoroutine;

	// Token: 0x02000643 RID: 1603
	public enum State
	{
		// Token: 0x040032A5 RID: 12965
		Closed,
		// Token: 0x040032A6 RID: 12966
		Open
	}

	// Token: 0x02000644 RID: 1604
	[Serializable]
	public struct DoorParams
	{
		// Token: 0x040032A7 RID: 12967
		public Transform doorTransform;

		// Token: 0x040032A8 RID: 12968
		public Transform doorClosedPosition;

		// Token: 0x040032A9 RID: 12969
		public Transform doorOpenPosition;
	}
}
