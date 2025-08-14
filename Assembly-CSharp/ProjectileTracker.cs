using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A6B RID: 2667
internal static class ProjectileTracker
{
	// Token: 0x0600411D RID: 16669 RVA: 0x0014A140 File Offset: 0x00148340
	static ProjectileTracker()
	{
		RoomSystem.LeftRoomEvent += new Action(ProjectileTracker.ClearProjectiles);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(ProjectileTracker.RemovePlayerProjectiles);
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x0014A1AC File Offset: 0x001483AC
	public static void RemovePlayerProjectiles(NetPlayer player)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_playerProjectiles.Remove(player);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x0014A1E8 File Offset: 0x001483E8
	private static void ClearProjectiles()
	{
		foreach (LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray in ProjectileTracker.m_playerProjectiles.Values)
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
		ProjectileTracker.m_playerProjectiles.Clear();
	}

	// Token: 0x06004120 RID: 16672 RVA: 0x0014A254 File Offset: 0x00148454
	private static void ResetPlayerProjectiles(LoopingArray<ProjectileTracker.ProjectileInfo> projectiles)
	{
		for (int i = 0; i < projectiles.Length; i++)
		{
			SlingshotProjectile projectileInstance = projectiles[i].projectileInstance;
			if (!projectileInstance.IsNull() && projectileInstance.projectileOwner != NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
			{
				projectileInstance.Deactivate();
			}
		}
	}

	// Token: 0x06004121 RID: 16673 RVA: 0x0014A2AC File Offset: 0x001484AC
	public static int AddAndIncrementLocalProjectile(SlingshotProjectile projectile, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		SlingshotProjectile projectileInstance = ProjectileTracker.m_localProjectiles[ProjectileTracker.m_localProjectiles.CurrentIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance != projectile && projectileInstance.projectileOwner == NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo projectileInfo = new ProjectileTracker.ProjectileInfo(PhotonNetwork.Time, intialVelocity, initialPosition, scale, projectile);
		return ProjectileTracker.m_localProjectiles.AddAndIncrement(projectileInfo);
	}

	// Token: 0x06004122 RID: 16674 RVA: 0x0014A328 File Offset: 0x00148528
	public static void AddRemotePlayerProjectile(NetPlayer player, SlingshotProjectile projectile, int projectileIndex, double timeShot, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (!ProjectileTracker.m_playerProjectiles.ContainsKey(player))
		{
			loopingArray = ProjectileTracker.m_projectileInfoPool.Take();
			ProjectileTracker.m_playerProjectiles[player] = loopingArray;
		}
		else
		{
			loopingArray = ProjectileTracker.m_playerProjectiles[player];
		}
		if (projectileIndex < 0 || projectileIndex >= loopingArray.Length)
		{
			GorillaNot.instance.SendReport("invlProj", player.UserId, player.NickName);
			return;
		}
		SlingshotProjectile projectileInstance = loopingArray[projectileIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance.projectileOwner == player && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo value = new ProjectileTracker.ProjectileInfo(timeShot, intialVelocity, initialPosition, scale, projectile);
		loopingArray[projectileIndex] = value;
	}

	// Token: 0x06004123 RID: 16675 RVA: 0x0014A3DA File Offset: 0x001485DA
	public static ProjectileTracker.ProjectileInfo GetLocalProjectile(int index)
	{
		return ProjectileTracker.m_localProjectiles[index];
	}

	// Token: 0x06004124 RID: 16676 RVA: 0x0014A3E8 File Offset: 0x001485E8
	public static ValueTuple<bool, ProjectileTracker.ProjectileInfo> GetAndRemoveRemotePlayerProjectile(NetPlayer player, int index)
	{
		ValueTuple<bool, ProjectileTracker.ProjectileInfo> result = new ValueTuple<bool, ProjectileTracker.ProjectileInfo>(false, default(ProjectileTracker.ProjectileInfo));
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (index < 0 || index >= ProjectileTracker.m_localProjectiles.Length || !ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			return result;
		}
		ProjectileTracker.ProjectileInfo projectileInfo = loopingArray[index];
		if (projectileInfo.projectileInstance.IsNotNull())
		{
			result.Item1 = true;
			result.Item2 = projectileInfo;
			loopingArray[index] = default(ProjectileTracker.ProjectileInfo);
		}
		return result;
	}

	// Token: 0x04004CB8 RID: 19640
	private static LoopingArray<ProjectileTracker.ProjectileInfo>.Pool m_projectileInfoPool = new LoopingArray<ProjectileTracker.ProjectileInfo>.Pool(50, 9);

	// Token: 0x04004CB9 RID: 19641
	private static LoopingArray<ProjectileTracker.ProjectileInfo> m_localProjectiles = new LoopingArray<ProjectileTracker.ProjectileInfo>(50);

	// Token: 0x04004CBA RID: 19642
	public static readonly Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>> m_playerProjectiles = new Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>>(9);

	// Token: 0x02000A6C RID: 2668
	public struct ProjectileInfo
	{
		// Token: 0x06004125 RID: 16677 RVA: 0x0014A45E File Offset: 0x0014865E
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, float newScale, SlingshotProjectile projectile)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.scale = newScale;
			this.projectileInstance = projectile;
			this.hasImpactOverride = projectile.playerImpactEffectPrefab.IsNotNull();
		}

		// Token: 0x04004CBB RID: 19643
		public double timeLaunched;

		// Token: 0x04004CBC RID: 19644
		public Vector3 shotVelocity;

		// Token: 0x04004CBD RID: 19645
		public Vector3 launchOrigin;

		// Token: 0x04004CBE RID: 19646
		public float scale;

		// Token: 0x04004CBF RID: 19647
		public SlingshotProjectile projectileInstance;

		// Token: 0x04004CC0 RID: 19648
		public bool hasImpactOverride;
	}
}
