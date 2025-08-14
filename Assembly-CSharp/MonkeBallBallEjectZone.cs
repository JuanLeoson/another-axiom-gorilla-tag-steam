using System;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class MonkeBallBallEjectZone : MonoBehaviour
{
	// Token: 0x06001F1E RID: 7966 RVA: 0x000A480C File Offset: 0x000A2A0C
	private void OnCollisionEnter(Collision collision)
	{
		GameBall component = collision.gameObject.GetComponent<GameBall>();
		if (component != null && collision.contacts.Length != 0)
		{
			component.SetVelocity(collision.contacts[0].impulse.normalized * this.ejectVelocity);
		}
	}

	// Token: 0x040027BA RID: 10170
	public Transform target;

	// Token: 0x040027BB RID: 10171
	public float ejectVelocity = 15f;
}
