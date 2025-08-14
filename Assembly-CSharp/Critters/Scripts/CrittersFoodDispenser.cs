using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x02000F8C RID: 3980
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x0600638F RID: 25487 RVA: 0x001F5DFE File Offset: 0x001F3FFE
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x001F5E0D File Offset: 0x001F400D
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x001F5E28 File Offset: 0x001F4028
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06006392 RID: 25490 RVA: 0x001F5E3D File Offset: 0x001F403D
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x001F5E53 File Offset: 0x001F4053
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x04006E84 RID: 28292
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
