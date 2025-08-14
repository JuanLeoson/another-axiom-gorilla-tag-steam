using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005EE RID: 1518
[Serializable]
public class GRAbilityInterpolatedMovement
{
	// Token: 0x06002546 RID: 9542 RVA: 0x000C86E7 File Offset: 0x000C68E7
	public void Setup(Transform root)
	{
		this.root = root;
		this.rb = root.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x000C8704 File Offset: 0x000C6904
	public void InitFromVelocityAndDuration(Vector3 velocity, float duration)
	{
		this.velocity = velocity;
		this.duration = duration;
		float magnitude = velocity.magnitude;
		if (magnitude > this.maxVelocityMagnitude)
		{
			this.velocity = velocity / magnitude * this.maxVelocityMagnitude;
		}
	}

	// Token: 0x06002548 RID: 9544 RVA: 0x000C8748 File Offset: 0x000C6948
	public void Start()
	{
		this.startPos = this.root.position;
		this.endPos = this.startPos + this.velocity * this.duration;
		this.endTime = Time.timeAsDouble + (double)this.duration;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.endPos, out navMeshHit, 5f, -1))
		{
			this.endPos = navMeshHit.position;
		}
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Stop()
	{
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000C87BD File Offset: 0x000C69BD
	public bool IsDone()
	{
		return Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000C87D0 File Offset: 0x000C69D0
	public void Update(float dt)
	{
		Vector3 position = this.root.position;
		float num = Mathf.Clamp01(1f - (float)((this.endTime - Time.timeAsDouble) / (double)this.duration));
		GRAbilityInterpolatedMovement.InterpType interpType = this.interpolationType;
		Vector3 vector;
		if (interpType != GRAbilityInterpolatedMovement.InterpType.Linear && interpType == GRAbilityInterpolatedMovement.InterpType.EaseOut)
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, EaseFunctions.EaseOutPower(num, 2.5f));
		}
		else
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, num);
		}
		vector.y = Mathf.Lerp(this.startPos.y, this.endPos.y, num * num);
		NavMeshHit navMeshHit;
		if (NavMesh.Raycast(position, vector, out navMeshHit, -1))
		{
			vector = navMeshHit.position;
		}
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
	}

	// Token: 0x04002F24 RID: 12068
	public Vector3 velocity = Vector3.zero;

	// Token: 0x04002F25 RID: 12069
	private Vector3 startPos;

	// Token: 0x04002F26 RID: 12070
	private Vector3 endPos;

	// Token: 0x04002F27 RID: 12071
	public float duration;

	// Token: 0x04002F28 RID: 12072
	public double endTime;

	// Token: 0x04002F29 RID: 12073
	public float maxVelocityMagnitude = 2f;

	// Token: 0x04002F2A RID: 12074
	private Transform root;

	// Token: 0x04002F2B RID: 12075
	private Rigidbody rb;

	// Token: 0x04002F2C RID: 12076
	public GRAbilityInterpolatedMovement.InterpType interpolationType;

	// Token: 0x020005EF RID: 1519
	public enum InterpType
	{
		// Token: 0x04002F2E RID: 12078
		Linear,
		// Token: 0x04002F2F RID: 12079
		EaseOut
	}
}
