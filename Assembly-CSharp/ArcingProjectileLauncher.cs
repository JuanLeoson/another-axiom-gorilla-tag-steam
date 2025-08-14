using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class ArcingProjectileLauncher : ElfLauncher
{
	// Token: 0x060009C5 RID: 2501 RVA: 0x000354FC File Offset: 0x000336FC
	protected override void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		float num = Vector3.Dot(direction, Vector3.up);
		Vector3 vector;
		if ((double)num > 0.99999 || (double)num < -0.99999)
		{
			if (this.parentHoldable.myRig != null)
			{
				vector = this.parentHoldable.myRig.transform.forward;
			}
			else
			{
				vector = Vector3.forward;
			}
		}
		else
		{
			vector = Vector3.ProjectOnPlane(direction, Vector3.up);
		}
		vector.Normalize();
		Vector3 axis = Vector3.Cross(vector, Vector3.up);
		float num2 = Vector3.SignedAngle(vector, direction, axis);
		float num3 = this.angleVelocityMultiplier.Evaluate(num2);
		float num4 = Mathf.Clamp(num2, this.fireAngleLimits.x, this.fireAngleLimits.y);
		float d = Mathf.Sin(num4 * 0.017453292f);
		float d2 = Mathf.Cos(num4 * 0.017453292f);
		Vector3 a = d * Vector3.up + d2 * vector;
		a.Normalize();
		Vector3 velocity = a * (this.muzzleVelocity * lossyScale.x * num3);
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		IProjectile component = gameObject.GetComponent<IProjectile>();
		if (component != null)
		{
			component.Launch(origin, Quaternion.LookRotation(vector, Vector3.up), velocity, 1f, this.parentHoldable.myRig, -1);
			return;
		}
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
	}

	// Token: 0x04000B97 RID: 2967
	[SerializeField]
	private Vector2 fireAngleLimits = new Vector3(-75f, 75f);

	// Token: 0x04000B98 RID: 2968
	[SerializeField]
	private AnimationCurve angleVelocityMultiplier;
}
