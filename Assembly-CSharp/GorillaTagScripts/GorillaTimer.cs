using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C3C RID: 3132
	public class GorillaTimer : MonoBehaviourPun
	{
		// Token: 0x06004D65 RID: 19813 RVA: 0x00180DCD File Offset: 0x0017EFCD
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06004D66 RID: 19814 RVA: 0x00180DD5 File Offset: 0x0017EFD5
		public void StartTimer()
		{
			this.startTimer = true;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x00180DEF File Offset: 0x0017EFEF
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x00180E05 File Offset: 0x0017F005
		private void StopTimer()
		{
			this.startTimer = false;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStopped;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x00180E1F File Offset: 0x0017F01F
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x00180E2C File Offset: 0x0017F02C
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x00180E59 File Offset: 0x0017F059
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x00180E62 File Offset: 0x0017F062
		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.startTimer && this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x00180EA0 File Offset: 0x0017F0A0
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x00180EA8 File Offset: 0x0017F0A8
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x00180EB1 File Offset: 0x0017F0B1
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x00180EC0 File Offset: 0x0017F0C0
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x00180EC8 File Offset: 0x0017F0C8
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x0400565C RID: 22108
		[SerializeField]
		private float timerDuration;

		// Token: 0x0400565D RID: 22109
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x0400565E RID: 22110
		[SerializeField]
		private float randTimeMin;

		// Token: 0x0400565F RID: 22111
		[SerializeField]
		private float randTimeMax;

		// Token: 0x04005660 RID: 22112
		private float passedTime;

		// Token: 0x04005661 RID: 22113
		private bool startTimer;

		// Token: 0x04005662 RID: 22114
		private bool resetTimer;

		// Token: 0x04005663 RID: 22115
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x04005664 RID: 22116
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
