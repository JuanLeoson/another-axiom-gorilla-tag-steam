using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class ShadeRevealer : ProjectileWeapon
{
	// Token: 0x06000457 RID: 1111 RVA: 0x00019650 File Offset: 0x00017850
	private float GetDistanceToBeamRay(Vector3 toPosition)
	{
		return Vector3.Cross(this.beamForward.forward, toPosition).magnitude;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00019678 File Offset: 0x00017878
	public ShadeRevealer.State GetBeamStateForPosition(Vector3 toPosition, float tolerance)
	{
		if (toPosition.magnitude <= this.beamLength + tolerance && Vector3.Dot(toPosition.normalized, this.beamForward.forward) > 0f)
		{
			float num = this.GetDistanceToBeamRay(toPosition) - tolerance;
			if (num <= this.lockThreshold)
			{
				return ShadeRevealer.State.LOCKED;
			}
			if (num <= this.trackThreshold)
			{
				return ShadeRevealer.State.TRACKING;
			}
		}
		return ShadeRevealer.State.SCANNING;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x000196D5 File Offset: 0x000178D5
	public ShadeRevealer.State GetBeamStateForCritter(CosmeticCritter critter, float tolerance)
	{
		return this.GetBeamStateForPosition(critter.transform.position - this.beamForward.position, tolerance);
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x000196F9 File Offset: 0x000178F9
	public bool CritterWithinBeamThreshold(CosmeticCritter critter, ShadeRevealer.State criteria, float tolerance)
	{
		return this.GetBeamStateForCritter(critter, tolerance) >= criteria;
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00019709 File Offset: 0x00017909
	public void SetBestBeamState(ShadeRevealer.State state)
	{
		if (state > this.pendingBeamState)
		{
			this.pendingBeamState = state;
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001971C File Offset: 0x0001791C
	private void SetObjectsEnabledFromState(ShadeRevealer.State state)
	{
		for (int i = 0; i < this.enableWhenScanning.Length; i++)
		{
			this.enableWhenScanning[i].SetActive(false);
		}
		for (int j = 0; j < this.enableWhenTracking.Length; j++)
		{
			this.enableWhenTracking[j].SetActive(false);
		}
		for (int k = 0; k < this.enableWhenLocked.Length; k++)
		{
			this.enableWhenLocked[k].SetActive(false);
		}
		for (int l = 0; l < this.enableWhenPrimed.Length; l++)
		{
			this.enableWhenPrimed[l].SetActive(false);
		}
		GameObject[] array;
		switch (state)
		{
		case ShadeRevealer.State.SCANNING:
			array = this.enableWhenScanning;
			break;
		case ShadeRevealer.State.TRACKING:
			array = this.enableWhenTracking;
			break;
		case ShadeRevealer.State.LOCKED:
			array = this.enableWhenLocked;
			break;
		case ShadeRevealer.State.PRIMED:
			array = this.enableWhenPrimed;
			break;
		default:
			return;
		}
		for (int m = 0; m < array.Length; m++)
		{
			array[m].SetActive(true);
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00019809 File Offset: 0x00017A09
	protected override Vector3 GetLaunchPosition()
	{
		return this.beamForward.position;
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00019816 File Offset: 0x00017A16
	protected override Vector3 GetLaunchVelocity()
	{
		return this.beamForward.forward * this.shootVelocity;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00019830 File Offset: 0x00017A30
	internal override SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			return base.LaunchNetworkedProjectile(location, velocity, projectileSource, projectileCounter, scale, shouldOverrideColor, color, info);
		}
		return null;
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001985C File Offset: 0x00017A5C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.currentBeamState != this.pendingBeamState)
		{
			this.currentBeamState = this.pendingBeamState;
			this.SetObjectsEnabledFromState(this.currentBeamState);
		}
		this.beamSFX.pitch = 1f + this.shadeCatcher.GetActionTimeFrac() * 2f;
		if (this.isScanning)
		{
			this.pendingBeamState = ShadeRevealer.State.SCANNING;
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000198C6 File Offset: 0x00017AC6
	public void StartScanning()
	{
		this.shadeCatcher.enabled = true;
		this.initialActivationSFX.GTPlay();
		this.beamSFX.GTPlay();
		this.isScanning = true;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.SCANNING;
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00019900 File Offset: 0x00017B00
	public void StopScanning()
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			base.LaunchProjectile();
			this.shootFX.Play();
		}
		this.shadeCatcher.enabled = false;
		this.initialActivationSFX.GTStop();
		this.beamSFX.GTStop();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.OFF;
		this.SetObjectsEnabledFromState(ShadeRevealer.State.OFF);
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00019968 File Offset: 0x00017B68
	public void ShadeCaught()
	{
		this.shadeCatcher.enabled = false;
		this.beamSFX.GTStop();
		this.catchSFX.GTPlay();
		this.catchFX.Play();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.PRIMED;
	}

	// Token: 0x04000500 RID: 1280
	[SerializeField]
	private AudioSource initialActivationSFX;

	// Token: 0x04000501 RID: 1281
	[SerializeField]
	private AudioSource beamSFX;

	// Token: 0x04000502 RID: 1282
	[SerializeField]
	private AudioSource catchSFX;

	// Token: 0x04000503 RID: 1283
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x04000504 RID: 1284
	[SerializeField]
	private ParticleSystem shootFX;

	// Token: 0x04000505 RID: 1285
	[Space]
	[SerializeField]
	private CosmeticCritterCatcherShade shadeCatcher;

	// Token: 0x04000506 RID: 1286
	[Space]
	[Tooltip("The transform that represents the origin of the revealer beam.")]
	[SerializeField]
	private Transform beamForward;

	// Token: 0x04000507 RID: 1287
	[Tooltip("The maximum length of the beam.")]
	[SerializeField]
	private float beamLength;

	// Token: 0x04000508 RID: 1288
	[Tooltip("If the Shade is this close to the beam, set it to flee and have all Revealers enter Tracking mode.")]
	[SerializeField]
	private float trackThreshold;

	// Token: 0x04000509 RID: 1289
	[Tooltip("If the Shade is this close to the beam, slow it down.")]
	[SerializeField]
	private float lockThreshold;

	// Token: 0x0400050A RID: 1290
	[Tooltip("Editor-only object to help test the thresholds.")]
	[SerializeField]
	private Transform thresholdTester;

	// Token: 0x0400050B RID: 1291
	[Tooltip("Whether to draw the tester or not.")]
	[SerializeField]
	private bool drawThresholdTesterInEditor = true;

	// Token: 0x0400050C RID: 1292
	[Tooltip("The velocity to shoot a captured Shade with.")]
	[SerializeField]
	private float shootVelocity = 5f;

	// Token: 0x0400050D RID: 1293
	[Space]
	[Tooltip("Enable these objects while the beam is in Scanning mode.")]
	[SerializeField]
	private GameObject[] enableWhenScanning;

	// Token: 0x0400050E RID: 1294
	[Tooltip("Enable these objects while the beam is in Tracking mode.")]
	[SerializeField]
	private GameObject[] enableWhenTracking;

	// Token: 0x0400050F RID: 1295
	[Tooltip("Enable these objects while the beam is in Locked mode.")]
	[SerializeField]
	private GameObject[] enableWhenLocked;

	// Token: 0x04000510 RID: 1296
	[Tooltip("Enable these objects while ready to fire.")]
	[SerializeField]
	private GameObject[] enableWhenPrimed;

	// Token: 0x04000511 RID: 1297
	private bool isScanning;

	// Token: 0x04000512 RID: 1298
	private ShadeRevealer.State currentBeamState;

	// Token: 0x04000513 RID: 1299
	private ShadeRevealer.State pendingBeamState;

	// Token: 0x020000B2 RID: 178
	public enum State
	{
		// Token: 0x04000515 RID: 1301
		OFF,
		// Token: 0x04000516 RID: 1302
		SCANNING,
		// Token: 0x04000517 RID: 1303
		TRACKING,
		// Token: 0x04000518 RID: 1304
		LOCKED,
		// Token: 0x04000519 RID: 1305
		PRIMED
	}
}
