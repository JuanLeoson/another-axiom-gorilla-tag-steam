using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F4B RID: 3915
	public class NearbyCosmeticsEffect : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x060060E8 RID: 24808 RVA: 0x001EDCDA File Offset: 0x001EBEDA
		// (set) Token: 0x060060E9 RID: 24809 RVA: 0x001EDCE2 File Offset: 0x001EBEE2
		public bool IsMatched { get; set; }

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x060060EA RID: 24810 RVA: 0x001EDCEB File Offset: 0x001EBEEB
		// (set) Token: 0x060060EB RID: 24811 RVA: 0x001EDCF3 File Offset: 0x001EBEF3
		public VRRig MyRig { get; private set; }

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x060060EC RID: 24812 RVA: 0x001EDCFC File Offset: 0x001EBEFC
		// (set) Token: 0x060060ED RID: 24813 RVA: 0x001EDD04 File Offset: 0x001EBF04
		public bool IsSpawned { get; set; }

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x060060EE RID: 24814 RVA: 0x001EDD0D File Offset: 0x001EBF0D
		// (set) Token: 0x060060EF RID: 24815 RVA: 0x001EDD15 File Offset: 0x001EBF15
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x060060F0 RID: 24816 RVA: 0x001EDD1E File Offset: 0x001EBF1E
		// (set) Token: 0x060060F1 RID: 24817 RVA: 0x001EDD26 File Offset: 0x001EBF26
		public bool TickRunning { get; set; }

		// Token: 0x060060F2 RID: 24818 RVA: 0x001EDD2F File Offset: 0x001EBF2F
		public void OnSpawn(VRRig rig)
		{
			if (this.MyRig == null)
			{
				this.MyRig = rig;
			}
		}

		// Token: 0x060060F3 RID: 24819 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDespawn()
		{
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x001EDD48 File Offset: 0x001EBF48
		private void Start()
		{
			this.canPlayEffects = true;
			this.IsMatched = false;
			this.MyRig = base.GetComponentInParent<VRRig>();
			this.timer = 0f;
			if (NearbyCosmeticsManager.Instance != null)
			{
				NearbyCosmeticsManager.Instance.Register(this);
			}
			if (this.OnBelowThresholdLocal.GetPersistentEventCount() == 0 && this.OnBelowThresholdShared.GetPersistentEventCount() == 0 && this.WhileBelowThresholdLocal.GetPersistentEventCount() == 0 && this.WhileBelowThresholdShared.GetPersistentEventCount() == 0 && this.OnAboveThresholdLocal.GetPersistentEventCount() == 0 && this.OnAboveThresholdShared.GetPersistentEventCount() == 0)
			{
				this.targetOnly = true;
			}
		}

		// Token: 0x060060F5 RID: 24821 RVA: 0x001EDDE7 File Offset: 0x001EBFE7
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			if (NearbyCosmeticsManager.Instance != null)
			{
				NearbyCosmeticsManager.Instance.Register(this);
			}
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x001EDE07 File Offset: 0x001EC007
		public void Tick()
		{
			if (!this.canPlayEffects && Time.time - this.timer >= this.cooldownTime)
			{
				this.canPlayEffects = true;
			}
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x001EDE2C File Offset: 0x001EC02C
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			if (NearbyCosmeticsManager.Instance)
			{
				NearbyCosmeticsManager.Instance.Unregister(this);
			}
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x001EDE4C File Offset: 0x001EC04C
		public void OnBelow(Vector3 contact)
		{
			if (!this.canPlayEffects)
			{
				return;
			}
			if (!this.wasBelowThreshold)
			{
				if (this.MyRig.isLocal)
				{
					UnityEvent<Vector3> onBelowThresholdLocal = this.OnBelowThresholdLocal;
					if (onBelowThresholdLocal != null)
					{
						onBelowThresholdLocal.Invoke(contact);
					}
				}
				UnityEvent<Vector3> onBelowThresholdShared = this.OnBelowThresholdShared;
				if (onBelowThresholdShared != null)
				{
					onBelowThresholdShared.Invoke(contact);
				}
				this.wasBelowThreshold = true;
				this.canPlayEffects = false;
				this.timer = Time.time;
			}
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x001EDEB4 File Offset: 0x001EC0B4
		public void OnAbove()
		{
			if (!this.canPlayEffects)
			{
				return;
			}
			if (this.wasBelowThreshold)
			{
				if (this.MyRig.isLocal)
				{
					UnityEvent onAboveThresholdLocal = this.OnAboveThresholdLocal;
					if (onAboveThresholdLocal != null)
					{
						onAboveThresholdLocal.Invoke();
					}
				}
				UnityEvent onAboveThresholdShared = this.OnAboveThresholdShared;
				if (onAboveThresholdShared != null)
				{
					onAboveThresholdShared.Invoke();
				}
				this.wasBelowThreshold = false;
				this.canPlayEffects = false;
				this.timer = Time.time;
			}
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x001EDF1A File Offset: 0x001EC11A
		public void WhileBelow(Vector3 contact)
		{
			UnityEvent<Vector3> whileBelowThresholdShared = this.WhileBelowThresholdShared;
			if (whileBelowThresholdShared != null)
			{
				whileBelowThresholdShared.Invoke(contact);
			}
			if (this.MyRig.isLocal)
			{
				UnityEvent<Vector3> whileBelowThresholdLocal = this.WhileBelowThresholdLocal;
				if (whileBelowThresholdLocal == null)
				{
					return;
				}
				whileBelowThresholdLocal.Invoke(contact);
			}
		}

		// Token: 0x04006CE5 RID: 27877
		public string cosmeticType;

		// Token: 0x04006CE6 RID: 27878
		[Tooltip("Should be the same value on all types")]
		public float proximityThreshold = 0.15f;

		// Token: 0x04006CE7 RID: 27879
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x04006CE8 RID: 27880
		[Tooltip("If collider is not assigned, we will use the position of this object to find the distance between two cosmetic/objects")]
		public Collider collider;

		// Token: 0x04006CE9 RID: 27881
		[Space]
		[Tooltip("Vector3: contact point")]
		public UnityEvent<Vector3> OnBelowThresholdLocal;

		// Token: 0x04006CEA RID: 27882
		public UnityEvent<Vector3> OnBelowThresholdShared;

		// Token: 0x04006CEB RID: 27883
		[Space]
		[Tooltip("Vector3: contact point")]
		public UnityEvent<Vector3> WhileBelowThresholdLocal;

		// Token: 0x04006CEC RID: 27884
		public UnityEvent<Vector3> WhileBelowThresholdShared;

		// Token: 0x04006CED RID: 27885
		[Space]
		public UnityEvent OnAboveThresholdLocal;

		// Token: 0x04006CEE RID: 27886
		public UnityEvent OnAboveThresholdShared;

		// Token: 0x04006CEF RID: 27887
		private float timer;

		// Token: 0x04006CF0 RID: 27888
		private bool canPlayEffects;

		// Token: 0x04006CF1 RID: 27889
		private bool wasBelowThreshold;

		// Token: 0x04006CF2 RID: 27890
		public bool targetOnly;

		// Token: 0x04006CF5 RID: 27893
		private RubberDuckEvents _events;
	}
}
