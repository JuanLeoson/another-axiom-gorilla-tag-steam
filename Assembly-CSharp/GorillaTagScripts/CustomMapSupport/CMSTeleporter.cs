using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C56 RID: 3158
	public class CMSTeleporter : CMSTrigger
	{
		// Token: 0x06004E25 RID: 20005 RVA: 0x001847E4 File Offset: 0x001829E4
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(TeleporterSettings))
			{
				TeleporterSettings teleporterSettings = (TeleporterSettings)settings;
				this.TeleportPoints = teleporterSettings.TeleportPoints;
				this.matchTeleportPointRotation = teleporterSettings.matchTeleportPointRotation;
				this.maintainVelocity = teleporterSettings.maintainVelocity;
			}
			for (int i = this.TeleportPoints.Count - 1; i >= 0; i--)
			{
				if (this.TeleportPoints[i] == null)
				{
					this.TeleportPoints.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06004E26 RID: 20006 RVA: 0x00184874 File Offset: 0x00182A74
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				if (this.TeleportPoints.Count != 0)
				{
					Transform transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
					if (transform != null)
					{
						instance.TeleportTo(transform, this.matchTeleportPointRotation, this.maintainVelocity);
					}
				}
			}
		}

		// Token: 0x04005711 RID: 22289
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x04005712 RID: 22290
		public bool matchTeleportPointRotation;

		// Token: 0x04005713 RID: 22291
		public bool maintainVelocity;
	}
}
