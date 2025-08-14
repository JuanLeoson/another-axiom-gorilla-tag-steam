using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F4C RID: 3916
	public class NearbyCosmeticsManager : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x060060FC RID: 24828 RVA: 0x001EDF6A File Offset: 0x001EC16A
		public static NearbyCosmeticsManager Instance
		{
			get
			{
				return NearbyCosmeticsManager._instance;
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x060060FD RID: 24829 RVA: 0x001EDF71 File Offset: 0x001EC171
		// (set) Token: 0x060060FE RID: 24830 RVA: 0x001EDF79 File Offset: 0x001EC179
		public bool TickRunning { get; set; }

		// Token: 0x060060FF RID: 24831 RVA: 0x001EDF82 File Offset: 0x001EC182
		private void Awake()
		{
			if (NearbyCosmeticsManager._instance != null && NearbyCosmeticsManager._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			NearbyCosmeticsManager._instance = this;
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x0001D447 File Offset: 0x0001B647
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x0001D44F File Offset: 0x0001B64F
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x001EDFB0 File Offset: 0x001EC1B0
		private void OnDestroy()
		{
			if (NearbyCosmeticsManager._instance == this)
			{
				NearbyCosmeticsManager._instance = null;
			}
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x001EDFC8 File Offset: 0x001EC1C8
		public void Register(NearbyCosmeticsEffect cosmetic)
		{
			if (!this.cosmetics.Contains(cosmetic))
			{
				this.cosmetics.Add(cosmetic);
				this.cosmetics.Sort((NearbyCosmeticsEffect x, NearbyCosmeticsEffect y) => x.targetOnly.CompareTo(y.targetOnly));
			}
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x001EE019 File Offset: 0x001EC219
		public void Unregister(NearbyCosmeticsEffect cosmetic)
		{
			this.cosmetics.Remove(cosmetic);
		}

		// Token: 0x06006105 RID: 24837 RVA: 0x001EE028 File Offset: 0x001EC228
		public void Tick()
		{
			if (this.cosmetics.Count == 0)
			{
				return;
			}
			this.CheckProximity();
			this.BreakTheBound();
		}

		// Token: 0x06006106 RID: 24838 RVA: 0x001EE044 File Offset: 0x001EC244
		private void CheckProximity()
		{
			for (int i = 0; i < this.cosmetics.Count; i++)
			{
				NearbyCosmeticsEffect nearbyCosmeticsEffect = this.cosmetics[i];
				if (!nearbyCosmeticsEffect.targetOnly)
				{
					for (int j = i + 1; j < this.cosmetics.Count; j++)
					{
						NearbyCosmeticsEffect nearbyCosmeticsEffect2 = this.cosmetics[j];
						if (nearbyCosmeticsEffect.MyRig != null)
						{
							nearbyCosmeticsEffect.MyRig == nearbyCosmeticsEffect2.MyRig;
						}
						if (!nearbyCosmeticsEffect.IsMatched && !nearbyCosmeticsEffect2.IsMatched)
						{
							float threshold = Mathf.Min(nearbyCosmeticsEffect.proximityThreshold, nearbyCosmeticsEffect2.proximityThreshold);
							Vector3 contact;
							if (NearbyCosmeticsManager.AreCollidersWithinThreshold(nearbyCosmeticsEffect, nearbyCosmeticsEffect2, threshold, out contact) && !string.IsNullOrEmpty(nearbyCosmeticsEffect.cosmeticType) && string.Equals(nearbyCosmeticsEffect.cosmeticType, nearbyCosmeticsEffect2.cosmeticType))
							{
								nearbyCosmeticsEffect.OnBelow(contact);
								nearbyCosmeticsEffect2.OnBelow(contact);
								nearbyCosmeticsEffect.IsMatched = true;
								nearbyCosmeticsEffect2.IsMatched = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x001EE140 File Offset: 0x001EC340
		private void BreakTheBound()
		{
			for (int i = 0; i < this.cosmetics.Count; i++)
			{
				NearbyCosmeticsEffect nearbyCosmeticsEffect = this.cosmetics[i];
				if (!nearbyCosmeticsEffect.targetOnly)
				{
					bool isMatched = false;
					if (nearbyCosmeticsEffect.IsMatched)
					{
						for (int j = i + 1; j < this.cosmetics.Count; j++)
						{
							NearbyCosmeticsEffect nearbyCosmeticsEffect2 = this.cosmetics[j];
							float threshold = Mathf.Min(nearbyCosmeticsEffect.proximityThreshold, nearbyCosmeticsEffect2.proximityThreshold);
							Vector3 contact;
							if (NearbyCosmeticsManager.AreCollidersWithinThreshold(nearbyCosmeticsEffect, nearbyCosmeticsEffect2, threshold, out contact) && !string.IsNullOrEmpty(nearbyCosmeticsEffect.cosmeticType) && string.Equals(nearbyCosmeticsEffect.cosmeticType, nearbyCosmeticsEffect2.cosmeticType))
							{
								isMatched = true;
								nearbyCosmeticsEffect.WhileBelow(contact);
								nearbyCosmeticsEffect2.WhileBelow(contact);
								break;
							}
						}
						nearbyCosmeticsEffect.IsMatched = isMatched;
						if (!nearbyCosmeticsEffect.IsMatched)
						{
							nearbyCosmeticsEffect.OnAbove();
						}
					}
				}
			}
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x001EE224 File Offset: 0x001EC424
		private static bool AreCollidersWithinThreshold(NearbyCosmeticsEffect a, NearbyCosmeticsEffect b, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = (b.collider == null) ? b.transform.position : b.collider.ClosestPoint(a.transform.position);
			Vector3 a2 = (a.collider == null) ? a.transform.position : a.collider.ClosestPoint(vector);
			contactPoint = (a2 + vector) * 0.5f;
			return Vector3.Distance(a2, vector) <= threshold;
		}

		// Token: 0x04006CF9 RID: 27897
		private List<NearbyCosmeticsEffect> cosmetics = new List<NearbyCosmeticsEffect>();

		// Token: 0x04006CFA RID: 27898
		private static NearbyCosmeticsManager _instance;
	}
}
