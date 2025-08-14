using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F9A RID: 3994
	public class GameObjectScheduler : MonoBehaviour
	{
		// Token: 0x060063D1 RID: 25553 RVA: 0x001F68EC File Offset: 0x001F4AEC
		private void Start()
		{
			this.schedule.Validate();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).gameObject);
			}
			this.scheduledGameObject = list.ToArray();
			for (int j = 0; j < this.scheduledGameObject.Length; j++)
			{
				this.scheduledGameObject[j].SetActive(false);
			}
			this.dispatcher = base.GetComponent<GameObjectSchedulerEventDispatcher>();
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}

		// Token: 0x060063D2 RID: 25554 RVA: 0x001F6982 File Offset: 0x001F4B82
		private void OnEnable()
		{
			if (this.monitor == null && this.scheduledGameObject != null)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x060063D3 RID: 25555 RVA: 0x001F69A6 File Offset: 0x001F4BA6
		private void OnDisable()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x060063D4 RID: 25556 RVA: 0x001F69C3 File Offset: 0x001F4BC3
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			bool previousState = this.getActiveState();
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(previousState);
			}
			for (;;)
			{
				yield return new WaitForSeconds(60f);
				bool activeState = this.getActiveState();
				if (previousState != activeState)
				{
					this.changeActiveState(activeState);
					previousState = activeState;
				}
			}
			yield break;
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x001F69D4 File Offset: 0x001F4BD4
		private bool getActiveState()
		{
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(this.getServerTime(), 0);
			bool result;
			if (this.currentNodeIndex == -1)
			{
				result = this.schedule.InitialState;
			}
			else if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				result = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
			}
			else
			{
				result = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			}
			return result;
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x001B51C7 File Offset: 0x001B33C7
		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x001F6A64 File Offset: 0x001F4C64
		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		// Token: 0x04006EAF RID: 28335
		[SerializeField]
		private GameObjectSchedule schedule;

		// Token: 0x04006EB0 RID: 28336
		private GameObject[] scheduledGameObject;

		// Token: 0x04006EB1 RID: 28337
		private GameObjectSchedulerEventDispatcher dispatcher;

		// Token: 0x04006EB2 RID: 28338
		private int currentNodeIndex = -1;

		// Token: 0x04006EB3 RID: 28339
		private Coroutine monitor;
	}
}
