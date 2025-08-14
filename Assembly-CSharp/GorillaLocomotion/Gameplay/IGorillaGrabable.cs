using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E29 RID: 3625
	internal interface IGorillaGrabable
	{
		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06005A18 RID: 23064
		string name { get; }

		// Token: 0x06005A19 RID: 23065
		bool MomentaryGrabOnly();

		// Token: 0x06005A1A RID: 23066
		bool CanBeGrabbed(GorillaGrabber grabber);

		// Token: 0x06005A1B RID: 23067
		void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition);

		// Token: 0x06005A1C RID: 23068
		void OnGrabReleased(GorillaGrabber grabber);
	}
}
