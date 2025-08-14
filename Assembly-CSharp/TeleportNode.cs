using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000B1F RID: 2847
public class TeleportNode : GorillaTriggerBox
{
	// Token: 0x06004492 RID: 17554 RVA: 0x001569C0 File Offset: 0x00154BC0
	public override void OnBoxTriggered()
	{
		if (Time.time - this.teleportTime < 0.1f)
		{
			return;
		}
		base.OnBoxTriggered();
		Transform transform;
		Transform transform2;
		if (this.teleportFromRef.TryResolve<Transform>(out transform) && this.teleportToRef.TryResolve<Transform>(out transform2))
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 position = transform2.TransformPoint(transform.InverseTransformPoint(instance.transform.position));
			instance.TeleportTo(position, instance.transform.rotation, true);
			this.teleportTime = Time.time;
		}
	}

	// Token: 0x04004EB8 RID: 20152
	[SerializeField]
	private XSceneRef teleportFromRef;

	// Token: 0x04004EB9 RID: 20153
	[SerializeField]
	private XSceneRef teleportToRef;

	// Token: 0x04004EBA RID: 20154
	private float teleportTime;
}
