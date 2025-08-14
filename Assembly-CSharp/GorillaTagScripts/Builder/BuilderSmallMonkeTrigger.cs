using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA1 RID: 3233
	public class BuilderSmallMonkeTrigger : MonoBehaviour
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06005041 RID: 20545 RVA: 0x001902E2 File Offset: 0x0018E4E2
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06005042 RID: 20546 RVA: 0x001902EF File Offset: 0x0018E4EF
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x06005043 RID: 20547 RVA: 0x00190300 File Offset: 0x0018E500
		// (remove) Token: 0x06005044 RID: 20548 RVA: 0x00190338 File Offset: 0x0018E538
		public event Action<int> onPlayerEnteredTrigger;

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x06005045 RID: 20549 RVA: 0x00190370 File Offset: 0x0018E570
		// (remove) Token: 0x06005046 RID: 20550 RVA: 0x001903A8 File Offset: 0x0018E5A8
		public event Action onTriggerFirstEntered;

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x06005047 RID: 20551 RVA: 0x001903E0 File Offset: 0x0018E5E0
		// (remove) Token: 0x06005048 RID: 20552 RVA: 0x00190418 File Offset: 0x0018E618
		public event Action onTriggerLastExited;

		// Token: 0x06005049 RID: 20553 RVA: 0x00190450 File Offset: 0x0018E650
		public void ValidateOverlappingColliders()
		{
			for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
			{
				if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
				{
					this.overlappingColliders.RemoveAt(i);
				}
				else
				{
					VRRig vrrig = this.overlappingColliders[i].attachedRigidbody.gameObject.GetComponent<VRRig>();
					if (vrrig == null)
					{
						if (GTPlayer.Instance.bodyCollider == this.overlappingColliders[i] || GTPlayer.Instance.headCollider == this.overlappingColliders[i])
						{
							vrrig = GorillaTagger.Instance.offlineVRRig;
						}
						else
						{
							this.overlappingColliders.RemoveAt(i);
						}
					}
					if (!this.ignoreScale && vrrig != null && (double)vrrig.scaleFactor > 0.99)
					{
						this.overlappingColliders.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x0600504A RID: 20554 RVA: 0x00190574 File Offset: 0x0018E774
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other) && !(GTPlayer.Instance.headCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(vrrig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (!this.ignoreScale && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			if (vrrig != null)
			{
				Action<int> action = this.onPlayerEnteredTrigger;
				if (action != null)
				{
					action(vrrig.OwningNetPlayer.ActorNumber);
				}
			}
			bool flag = this.overlappingColliders.Count == 0;
			if (!this.overlappingColliders.Contains(other))
			{
				this.overlappingColliders.Add(other);
			}
			this.lastTriggeredFrame = Time.frameCount;
			if (flag)
			{
				Action action2 = this.onTriggerFirstEntered;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x0600504B RID: 20555 RVA: 0x00190693 File Offset: 0x0018E893
		private void OnTriggerExit(Collider other)
		{
			if (this.overlappingColliders.Remove(other) && this.overlappingColliders.Count == 0)
			{
				Action action = this.onTriggerLastExited;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x0400597C RID: 22908
		private int lastTriggeredFrame = -1;

		// Token: 0x0400597D RID: 22909
		private List<Collider> overlappingColliders = new List<Collider>(20);

		// Token: 0x04005981 RID: 22913
		private bool hasCheckedZone;

		// Token: 0x04005982 RID: 22914
		private bool ignoreScale;
	}
}
