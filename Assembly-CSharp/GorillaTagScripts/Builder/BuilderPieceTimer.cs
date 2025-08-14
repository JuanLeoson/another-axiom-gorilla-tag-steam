using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C94 RID: 3220
	public class BuilderPieceTimer : MonoBehaviour, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06004FCB RID: 20427 RVA: 0x0018DB15 File Offset: 0x0018BD15
		private void Awake()
		{
			this.buttonTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnButtonPressed));
		}

		// Token: 0x06004FCC RID: 20428 RVA: 0x0018DB33 File Offset: 0x0018BD33
		private void OnDestroy()
		{
			if (this.buttonTrigger != null)
			{
				this.buttonTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06004FCD RID: 20429 RVA: 0x0018DB60 File Offset: 0x0018BD60
		private void OnButtonPressed()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (Time.time > this.lastTriggeredTime + this.debounceTime)
			{
				this.lastTriggeredTime = Time.time;
				if (!this.isStart && this.stopSoundBank != null)
				{
					this.stopSoundBank.Play();
				}
				else if (this.activateSoundBank != null)
				{
					this.activateSoundBank.Play();
				}
				if (this.isBoth && this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: 00:00:0";
				}
				PlayerTimerManager.instance.RequestTimerToggle(this.isStart);
			}
		}

		// Token: 0x06004FCE RID: 20430 RVA: 0x0018DC18 File Offset: 0x0018BE18
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isStart && !this.isBoth)
			{
				return;
			}
			double num = timeDelta;
			this.latestTime = num / 1000.0;
			if (this.latestTime > 3599.989990234375)
			{
				this.latestTime = 3599.989990234375;
			}
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds(this.latestTime).ToString("mm\\:ss\\:ff");
			if (this.isBoth && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStart = true;
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x0018DCC7 File Offset: 0x0018BEC7
		private void OnLocalTimerStarted()
		{
			if (this.isBoth)
			{
				this.isStart = false;
			}
			if (this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0018DCF4 File Offset: 0x0018BEF4
		private void OnZoneChanged()
		{
			bool active = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.displayText != null)
			{
				this.displayText.gameObject.SetActive(active);
			}
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x0018DD3C File Offset: 0x0018BF3C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.latestTime = double.MaxValue;
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
				this.OnZoneChanged();
				this.displayText.text = "TIME: __:__:_";
			}
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x0018DDA2 File Offset: 0x0018BFA2
		public void OnPieceDestroy()
		{
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0018DDD8 File Offset: 0x0018BFD8
		public void OnPieceActivate()
		{
			this.lastTriggeredTime = 0f;
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBoth)
			{
				this.isStart = !PlayerTimerManager.instance.IsLocalTimerStarted();
				if (!this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: __:__:_";
				}
			}
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x0018DE84 File Offset: 0x0018C084
		public void OnPieceDeactivate()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.displayText != null)
			{
				this.displayText.text = "TIME: --:--:-";
			}
		}

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06004FD6 RID: 20438 RVA: 0x0018DF00 File Offset: 0x0018C100
		// (set) Token: 0x06004FD7 RID: 20439 RVA: 0x0018DF08 File Offset: 0x0018C108
		public bool TickRunning { get; set; }

		// Token: 0x06004FD8 RID: 20440 RVA: 0x0018DF14 File Offset: 0x0018C114
		public void Tick()
		{
			if (this.displayText != null)
			{
				float num = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
				num = Mathf.Clamp(num, 0f, 3599.99f);
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)num).ToString("mm\\:ss\\:f");
			}
		}

		// Token: 0x040058E0 RID: 22752
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040058E1 RID: 22753
		[SerializeField]
		private bool isStart;

		// Token: 0x040058E2 RID: 22754
		[SerializeField]
		private bool isBoth;

		// Token: 0x040058E3 RID: 22755
		[SerializeField]
		private BuilderSmallHandTrigger buttonTrigger;

		// Token: 0x040058E4 RID: 22756
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x040058E5 RID: 22757
		[SerializeField]
		private SoundBankPlayer stopSoundBank;

		// Token: 0x040058E6 RID: 22758
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x040058E7 RID: 22759
		private float lastTriggeredTime;

		// Token: 0x040058E8 RID: 22760
		private double latestTime = 3.4028234663852886E+38;

		// Token: 0x040058E9 RID: 22761
		[SerializeField]
		private TMP_Text displayText;
	}
}
