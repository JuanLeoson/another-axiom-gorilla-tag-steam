using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000135 RID: 309
public class PlayerGameEventListener : MonoBehaviour
{
	// Token: 0x0600080B RID: 2059 RVA: 0x0002CECE File Offset: 0x0002B0CE
	private void OnEnable()
	{
		this.SubscribeToEvents();
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002CED6 File Offset: 0x0002B0D6
	private void OnDisable()
	{
		this.UnsubscribeFromEvents();
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0002CEE0 File Offset: 0x0002B0E0
	private void SubscribeToEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent += this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0002D014 File Offset: 0x0002B214
	private void UnsubscribeFromEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent -= this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0002D145 File Offset: 0x0002B345
	private void OnGameMoveEventTriggered(float distance, float speed)
	{
		Debug.LogError("Movement events not supported - please implement");
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0002D151 File Offset: 0x0002B351
	public void OnGameEventTriggered(string eventName)
	{
		this.OnGameEventTriggered(eventName, 1);
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0002D15C File Offset: 0x0002B35C
	public void OnGameEventTriggered(string eventName, int count)
	{
		if (!string.IsNullOrEmpty(this.filter) && !eventName.StartsWith(this.filter))
		{
			return;
		}
		if (this._cooldownEnd > Time.time)
		{
			return;
		}
		this._cooldownEnd = Time.time + this.cooldown;
		UnityEvent unityEvent = this.onGameEvent;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		UnityEvent<int> unityEvent2 = this.onGameEventCounted;
		if (unityEvent2 == null)
		{
			return;
		}
		unityEvent2.Invoke(count);
	}

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private PlayerGameEvents.EventType eventType;

	// Token: 0x040009B4 RID: 2484
	[Tooltip("Cooldown in seconds")]
	[SerializeField]
	private string filter;

	// Token: 0x040009B5 RID: 2485
	[SerializeField]
	private float cooldown = 1f;

	// Token: 0x040009B6 RID: 2486
	[SerializeField]
	private UnityEvent onGameEvent;

	// Token: 0x040009B7 RID: 2487
	[SerializeField]
	private UnityEvent<int> onGameEventCounted;

	// Token: 0x040009B8 RID: 2488
	private float _cooldownEnd;
}
