using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D5 RID: 213
public class IDCardScanner : MonoBehaviour
{
	// Token: 0x06000538 RID: 1336 RVA: 0x0001E070 File Offset: 0x0001C270
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity component = other.GetComponent<GameEntity>();
			if (component == null && other.attachedRigidbody != null)
			{
				component = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (component != null && component.heldByActorNumber != -1)
			{
				UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke(component.heldByActorNumber);
			}
		}
	}

	// Token: 0x04000626 RID: 1574
	public UnityEvent onCardSwiped;

	// Token: 0x04000627 RID: 1575
	public UnityEvent<int> onCardSwipedByPlayer;

	// Token: 0x04000628 RID: 1576
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	// Token: 0x04000629 RID: 1577
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;
}
