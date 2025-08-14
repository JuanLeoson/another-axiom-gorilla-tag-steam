using System;
using UnityEngine;

// Token: 0x020003D7 RID: 983
internal interface ITetheredObjectBehavior
{
	// Token: 0x06001707 RID: 5895
	void DbgClear();

	// Token: 0x06001708 RID: 5896
	void EnableDistanceConstraints(bool v, float playerScale);

	// Token: 0x06001709 RID: 5897
	void EnableDynamics(bool enable, bool collider, bool kinematic);

	// Token: 0x0600170A RID: 5898
	bool IsEnabled();

	// Token: 0x0600170B RID: 5899
	void ReParent();

	// Token: 0x0600170C RID: 5900
	bool ReturnStep();

	// Token: 0x0600170D RID: 5901
	void TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership);
}
