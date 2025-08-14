using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA5 RID: 3237
	public class KnockbackTrigger : MonoBehaviour
	{
		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x0600505F RID: 20575 RVA: 0x0019107A File Offset: 0x0018F27A
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06005060 RID: 20576 RVA: 0x0019108C File Offset: 0x0018F28C
		private void CheckZone()
		{
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x001910DC File Offset: 0x0018F2DC
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.CheckZone();
			if (!this.ignoreScale && this.onlySmallMonke && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			this.collidersEntered.Add(other);
			if (this.collidersEntered.Count > 1)
			{
				return;
			}
			Vector3 vector = this.triggerVolume.ClosestPoint(GorillaTagger.Instance.headCollider.transform.position);
			Vector3 vector2 = vector - base.transform.TransformPoint(this.triggerVolume.center);
			vector2 -= Vector3.Project(vector2, base.transform.TransformDirection(this.localAxis));
			float magnitude = vector2.magnitude;
			Vector3 direction = Vector3.up;
			if (magnitude >= 0.01f)
			{
				direction = vector2 / magnitude;
			}
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			GTPlayer.Instance.ApplyKnockback(direction, this.knockbackVelocity * VRRigCache.Instance.localRig.Rig.scaleFactor, false);
			if (this.impactFX != null)
			{
				ObjectPools.instance.Instantiate(this.impactFX, vector, true);
			}
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			this.lastTriggeredFrame = Time.frameCount;
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x0019127E File Offset: 0x0018F47E
		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.collidersEntered.Remove(other);
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x001912BA File Offset: 0x0018F4BA
		private void OnDisable()
		{
			this.collidersEntered.Clear();
		}

		// Token: 0x040059AF RID: 22959
		[SerializeField]
		private BoxCollider triggerVolume;

		// Token: 0x040059B0 RID: 22960
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x040059B1 RID: 22961
		[SerializeField]
		private Vector3 localAxis;

		// Token: 0x040059B2 RID: 22962
		[SerializeField]
		private GameObject impactFX;

		// Token: 0x040059B3 RID: 22963
		[SerializeField]
		private bool onlySmallMonke;

		// Token: 0x040059B4 RID: 22964
		private bool hasCheckedZone;

		// Token: 0x040059B5 RID: 22965
		private bool ignoreScale;

		// Token: 0x040059B6 RID: 22966
		private int lastTriggeredFrame = -1;

		// Token: 0x040059B7 RID: 22967
		private List<Collider> collidersEntered = new List<Collider>(4);
	}
}
