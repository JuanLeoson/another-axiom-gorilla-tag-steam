using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001BA RID: 442
public abstract class ProjectileWeapon : TransferrableObject
{
	// Token: 0x06000AF9 RID: 2809
	protected abstract Vector3 GetLaunchPosition();

	// Token: 0x06000AFA RID: 2810
	protected abstract Vector3 GetLaunchVelocity();

	// Token: 0x06000AFB RID: 2811 RVA: 0x0003A97F File Offset: 0x00038B7F
	internal override void OnEnable()
	{
		base.OnEnable();
		if (base.myOnlineRig != null)
		{
			base.myOnlineRig.projectileWeapon = this;
		}
		if (base.myRig != null)
		{
			base.myRig.projectileWeapon = this;
		}
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0003A9BC File Offset: 0x00038BBC
	protected void LaunchProjectile()
	{
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		int trailHash = PoolUtils.GameObjHashCode(this.projectileTrail);
		GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
		float num = Mathf.Abs(base.transform.lossyScale.x);
		gameObject.transform.localScale = Vector3.one * num;
		Vector3 launchPosition = this.GetLaunchPosition();
		Vector3 launchVelocity = this.GetLaunchVelocity();
		bool blueTeam;
		bool orangeTeam;
		this.GetIsOnTeams(out blueTeam, out orangeTeam);
		this.AttachTrail(trailHash, gameObject, launchPosition, blueTeam, orangeTeam);
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		if (NetworkSystem.Instance.InRoom)
		{
			int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, launchVelocity, launchPosition, num);
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, projectileCount, num, false, default(Color));
			TransferrableObject.PositionState currentState = this.currentState;
			RoomSystem.SendLaunchProjectile(launchPosition, launchVelocity, RoomSystem.ProjectileSource.ProjectileWeapon, projectileCount, false, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			this.PlayLaunchSfx();
		}
		else
		{
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, 0, num, false, default(Color));
			this.PlayLaunchSfx();
		}
		PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003AAF8 File Offset: 0x00038CF8
	internal virtual SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		GameObject gameObject = null;
		SlingshotProjectile slingshotProjectile = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		try
		{
			int hash = -1;
			int num = -1;
			if (projectileSource == RoomSystem.ProjectileSource.ProjectileWeapon)
			{
				if (this.currentState == TransferrableObject.PositionState.OnChest || this.currentState == TransferrableObject.PositionState.None)
				{
					return null;
				}
				hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				num = PoolUtils.GameObjHashCode(this.projectileTrail);
			}
			gameObject = ObjectPools.instance.Instantiate(hash, true);
			slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
			bool blueTeam;
			bool orangeTeam;
			this.GetIsOnTeams(out blueTeam, out orangeTeam);
			if (num != -1)
			{
				this.AttachTrail(num, slingshotProjectile.gameObject, location, blueTeam, orangeTeam);
			}
			slingshotProjectile.Launch(location, velocity, player, blueTeam, orangeTeam, projectileCounter, scale, shouldOverrideColor, color);
			this.PlayLaunchSfx();
		}
		catch
		{
			GorillaNot.instance.SendReport("projectile error", player.UserId, player.NickName);
			if (slingshotProjectile != null && slingshotProjectile)
			{
				slingshotProjectile.transform.position = Vector3.zero;
				slingshotProjectile.Deactivate();
				slingshotProjectile = null;
			}
			else if (gameObject.IsNotNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
		}
		return slingshotProjectile;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003AC18 File Offset: 0x00038E18
	protected void GetIsOnTeams(out bool blueTeam, out bool orangeTeam)
	{
		NetPlayer player = base.OwningPlayer();
		blueTeam = false;
		orangeTeam = false;
		if (GorillaGameManager.instance != null)
		{
			GorillaPaintbrawlManager component = GorillaGameManager.instance.GetComponent<GorillaPaintbrawlManager>();
			if (component != null)
			{
				blueTeam = component.OnBlueTeam(player);
				orangeTeam = component.OnRedTeam(player);
			}
		}
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003AC68 File Offset: 0x00038E68
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0003ACB4 File Offset: 0x00038EB4
	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.GTPlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)], 1f);
		}
	}

	// Token: 0x04000D66 RID: 3430
	[SerializeField]
	protected GameObject projectilePrefab;

	// Token: 0x04000D67 RID: 3431
	[SerializeField]
	private GameObject projectileTrail;

	// Token: 0x04000D68 RID: 3432
	public AudioClip[] shootSfxClips;

	// Token: 0x04000D69 RID: 3433
	public AudioSource shootSfx;
}
