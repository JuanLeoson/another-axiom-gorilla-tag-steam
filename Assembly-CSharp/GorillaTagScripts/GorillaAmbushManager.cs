using System;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C34 RID: 3124
	public sealed class GorillaAmbushManager : GorillaTagManager
	{
		// Token: 0x06004D1A RID: 19738 RVA: 0x0017F96B File Offset: 0x0017DB6B
		public override GameModeType GameType()
		{
			if (!this.isGhostTag)
			{
				return GameModeType.Ambush;
			}
			return GameModeType.Ghost;
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06004D1B RID: 19739 RVA: 0x0017F978 File Offset: 0x0017DB78
		public static int HandEffectHash
		{
			get
			{
				return GorillaAmbushManager.handTapHash;
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06004D1C RID: 19740 RVA: 0x0017F97F File Offset: 0x0017DB7F
		// (set) Token: 0x06004D1D RID: 19741 RVA: 0x0017F986 File Offset: 0x0017DB86
		public static float HandFXScaleModifier { get; private set; }

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06004D1E RID: 19742 RVA: 0x0017F98E File Offset: 0x0017DB8E
		// (set) Token: 0x06004D1F RID: 19743 RVA: 0x0017F996 File Offset: 0x0017DB96
		public bool isGhostTag { get; private set; }

		// Token: 0x06004D20 RID: 19744 RVA: 0x0017F99F File Offset: 0x0017DB9F
		public override void Awake()
		{
			base.Awake();
			if (this.handTapFX != null)
			{
				GorillaAmbushManager.handTapHash = PoolUtils.GameObjHashCode(this.handTapFX);
			}
			GorillaAmbushManager.HandFXScaleModifier = this.handTapScaleFactor;
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x0017F9D0 File Offset: 0x0017DBD0
		private void Start()
		{
			this.hasScryingPlane = this.scryingPlaneRef.TryResolve<MeshRenderer>(out this.scryingPlane);
			this.hasScryingPlane3p = this.scryingPlane3pRef.TryResolve<MeshRenderer>(out this.scryingPlane3p);
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0017FA00 File Offset: 0x0017DC00
		public override string GameModeName()
		{
			if (!this.isGhostTag)
			{
				return "AMBUSH";
			}
			return "GHOST";
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x0017FA18 File Offset: 0x0017DC18
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			int materialIndex = this.MyMatIndex(rig.creator);
			rig.ChangeMaterialLocal(materialIndex);
			bool flag = base.IsInfected(rig.Creator);
			bool flag2 = base.IsInfected(NetworkSystem.Instance.LocalPlayer);
			rig.bodyRenderer.SetGameModeBodyType(flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default);
			rig.SetInvisibleToLocalPlayer(flag && !flag2);
			if (this.isGhostTag && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(flag);
				if (this.hasScryingPlane)
				{
					this.scryingPlane.enabled = flag2;
				}
				if (this.hasScryingPlane3p)
				{
					this.scryingPlane3p.enabled = flag2;
				}
			}
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x0017FABE File Offset: 0x0017DCBE
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (!base.IsInfected(forPlayer))
			{
				return 0;
			}
			return 13;
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x0017FAD0 File Offset: 0x0017DCD0
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				GorillaSkin.ApplyToRig(vrrig, null, GorillaSkin.SkinType.gameMode);
				vrrig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
				vrrig.SetInvisibleToLocalPlayer(false);
			}
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (this.hasScryingPlane)
			{
				this.scryingPlane.enabled = false;
			}
			if (this.hasScryingPlane3p)
			{
				this.scryingPlane3p.enabled = false;
			}
		}

		// Token: 0x04005614 RID: 22036
		public GameObject handTapFX;

		// Token: 0x04005615 RID: 22037
		public GorillaSkin ambushSkin;

		// Token: 0x04005616 RID: 22038
		[SerializeField]
		private AudioClip[] firstPersonTaggedSounds;

		// Token: 0x04005617 RID: 22039
		[SerializeField]
		private float firstPersonTaggedSoundVolume;

		// Token: 0x04005618 RID: 22040
		private static int handTapHash = -1;

		// Token: 0x04005619 RID: 22041
		public float handTapScaleFactor = 0.5f;

		// Token: 0x0400561B RID: 22043
		public float crawlingSpeedForMaxVolume;

		// Token: 0x0400561D RID: 22045
		[SerializeField]
		private XSceneRef scryingPlaneRef;

		// Token: 0x0400561E RID: 22046
		[SerializeField]
		private XSceneRef scryingPlane3pRef;

		// Token: 0x0400561F RID: 22047
		private const int STEALTH_MATERIAL_INDEX = 13;

		// Token: 0x04005620 RID: 22048
		private MeshRenderer scryingPlane;

		// Token: 0x04005621 RID: 22049
		private bool hasScryingPlane;

		// Token: 0x04005622 RID: 22050
		private MeshRenderer scryingPlane3p;

		// Token: 0x04005623 RID: 22051
		private bool hasScryingPlane3p;
	}
}
