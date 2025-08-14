using System;
using UnityEngine;

// Token: 0x0200066A RID: 1642
public class GRSentientCore : MonoBehaviour
{
	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06002836 RID: 10294 RVA: 0x000D8D46 File Offset: 0x000D6F46
	public double LastDestinationUpdateTime
	{
		get
		{
			return this.lastDestinationUpdateTime;
		}
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x000D8D4E File Offset: 0x000D6F4E
	private void Start()
	{
		GhostReactor.instance.sentientCores.Add(this);
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		this.Sleep();
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000D8D7D File Offset: 0x000D6F7D
	private void OnDestroy()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.sentientCores.Remove(this);
		}
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000D8D9D File Offset: 0x000D6F9D
	public void WakeUp()
	{
		if (this.gameEntity.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
		}
		base.enabled = true;
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000D8DCB File Offset: 0x000D6FCB
	public void Sleep()
	{
		if (this.gameEntity.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, 0L);
		}
		base.enabled = false;
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000D8DF9 File Offset: 0x000D6FF9
	private void OnStateChanged(long prevState, long nextState)
	{
		if ((int)nextState == 0)
		{
			this.Sleep();
			return;
		}
		if ((int)nextState == 1)
		{
			this.WakeUp();
		}
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x000D8E14 File Offset: 0x000D7014
	public void ApplyDestination(Vector3 currentPosition, Vector3 destination)
	{
		this.targetPosition = destination;
		this.lastDestinationUpdateTime = Time.timeAsDouble;
		if (base.enabled)
		{
			float num = 3f;
			if ((base.transform.position - currentPosition).sqrMagnitude > num * num)
			{
				base.transform.position = currentPosition;
				return;
			}
		}
		else
		{
			base.transform.position = currentPosition;
		}
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x000D8E78 File Offset: 0x000D7078
	private void FixedUpdate()
	{
		Vector3 a = this.targetPosition - base.transform.position;
		if (a.sqrMagnitude > 1f)
		{
			a = a.normalized;
		}
		this.rb.AddForce(a * this.repelForce, ForceMode.Acceleration);
	}

	// Token: 0x040033AE RID: 13230
	public GameEntity gameEntity;

	// Token: 0x040033AF RID: 13231
	public Rigidbody rb;

	// Token: 0x040033B0 RID: 13232
	public Vector2 reactProximityMinMax = new Vector2(1f, 3f);

	// Token: 0x040033B1 RID: 13233
	public float retreatTargetDistance = 5f;

	// Token: 0x040033B2 RID: 13234
	public float repelForce = 100f;

	// Token: 0x040033B3 RID: 13235
	public float destinationUpdateFrequency = 1f;

	// Token: 0x040033B4 RID: 13236
	private double lastDestinationUpdateTime;

	// Token: 0x040033B5 RID: 13237
	private Vector3 targetPosition;

	// Token: 0x0200066B RID: 1643
	public enum SentientCoreState
	{
		// Token: 0x040033B7 RID: 13239
		Asleep,
		// Token: 0x040033B8 RID: 13240
		Awake
	}
}
