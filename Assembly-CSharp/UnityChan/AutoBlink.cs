using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x02000FB7 RID: 4023
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x0600649E RID: 25758 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Awake()
		{
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x001FE4E2 File Offset: 0x001FC6E2
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x001FE4F6 File Offset: 0x001FC6F6
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x001FE50C File Offset: 0x001FC70C
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x001FE580 File Offset: 0x001FC780
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x001FE5D2 File Offset: 0x001FC7D2
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x060064A4 RID: 25764 RVA: 0x001FE5F8 File Offset: 0x001FC7F8
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x001FE61E File Offset: 0x001FC81E
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x060064A6 RID: 25766 RVA: 0x001FE644 File Offset: 0x001FC844
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x04006F34 RID: 28468
		public bool isActive = true;

		// Token: 0x04006F35 RID: 28469
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x04006F36 RID: 28470
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x04006F37 RID: 28471
		public float ratio_Close = 85f;

		// Token: 0x04006F38 RID: 28472
		public float ratio_HalfClose = 20f;

		// Token: 0x04006F39 RID: 28473
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x04006F3A RID: 28474
		private bool timerStarted;

		// Token: 0x04006F3B RID: 28475
		private bool isBlink;

		// Token: 0x04006F3C RID: 28476
		public float timeBlink = 0.4f;

		// Token: 0x04006F3D RID: 28477
		private float timeRemining;

		// Token: 0x04006F3E RID: 28478
		public float threshold = 0.3f;

		// Token: 0x04006F3F RID: 28479
		public float interval = 3f;

		// Token: 0x04006F40 RID: 28480
		private AutoBlink.Status eyeStatus;

		// Token: 0x02000FB8 RID: 4024
		private enum Status
		{
			// Token: 0x04006F42 RID: 28482
			Close,
			// Token: 0x04006F43 RID: 28483
			HalfClose,
			// Token: 0x04006F44 RID: 28484
			Open
		}
	}
}
