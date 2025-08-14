using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class GravityOverrideVolume : MonoBehaviour
{
	// Token: 0x06001E7B RID: 7803 RVA: 0x000A1754 File Offset: 0x0009F954
	private void OnEnable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter += this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit += this.OnColliderExitedVolume;
		}
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000A1792 File Offset: 0x0009F992
	private void OnDisable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit -= this.OnColliderExitedVolume;
		}
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000A17D0 File Offset: 0x0009F9D0
	private void OnColliderEnteredVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000A1810 File Offset: 0x0009FA10
	private void OnColliderExitedVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000A1844 File Offset: 0x0009FA44
	public void GravityOverrideFunction(GTPlayer player)
	{
		GravityOverrideVolume.GravityType gravityType = this.gravityType;
		if (gravityType == GravityOverrideVolume.GravityType.Directional)
		{
			Vector3 forward = this.referenceTransform.forward;
			player.AddForce(forward * this.strength, ForceMode.Acceleration);
			return;
		}
		if (gravityType != GravityOverrideVolume.GravityType.Radial)
		{
			return;
		}
		Vector3 normalized = (this.referenceTransform.position - player.headCollider.transform.position).normalized;
		player.AddForce(normalized * this.strength, ForceMode.Acceleration);
	}

	// Token: 0x0400272E RID: 10030
	[SerializeField]
	private GravityOverrideVolume.GravityType gravityType;

	// Token: 0x0400272F RID: 10031
	[SerializeField]
	private float strength = 9.8f;

	// Token: 0x04002730 RID: 10032
	[SerializeField]
	[Tooltip("In Radial: the center point of gravity, In Directional: the forward vector of this transform defines the direction")]
	private Transform referenceTransform;

	// Token: 0x04002731 RID: 10033
	[SerializeField]
	private CompositeTriggerEvents triggerEvents;

	// Token: 0x020004E4 RID: 1252
	public enum GravityType
	{
		// Token: 0x04002733 RID: 10035
		Directional,
		// Token: 0x04002734 RID: 10036
		Radial
	}
}
