using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FBE RID: 4030
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x060064C2 RID: 25794 RVA: 0x001FF23B File Offset: 0x001FD43B
		// (set) Token: 0x060064C3 RID: 25795 RVA: 0x001FF24D File Offset: 0x001FD44D
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x060064C4 RID: 25796 RVA: 0x001FF267 File Offset: 0x001FD467
		// (set) Token: 0x060064C5 RID: 25797 RVA: 0x001FF279 File Offset: 0x001FD479
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x060064C6 RID: 25798 RVA: 0x001FF293 File Offset: 0x001FD493
		// (set) Token: 0x060064C7 RID: 25799 RVA: 0x001FF2A5 File Offset: 0x001FD4A5
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x060064C8 RID: 25800 RVA: 0x001FF2BF File Offset: 0x001FD4BF
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x060064C9 RID: 25801 RVA: 0x001FF2E8 File Offset: 0x001FD4E8
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x060064CA RID: 25802 RVA: 0x001FF3B1 File Offset: 0x001FD5B1
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x001FF3C7 File Offset: 0x001FD5C7
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x001FF3D0 File Offset: 0x001FD5D0
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x060064CD RID: 25805 RVA: 0x001FF3D8 File Offset: 0x001FD5D8
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x001FF3E0 File Offset: 0x001FD5E0
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x001FF3E8 File Offset: 0x001FD5E8
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x060064D0 RID: 25808 RVA: 0x001FF4E7 File Offset: 0x001FD6E7
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x001FF4F0 File Offset: 0x001FD6F0
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x001FF566 File Offset: 0x001FD766
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x060064D3 RID: 25811 RVA: 0x001FF574 File Offset: 0x001FD774
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x060064D4 RID: 25812 RVA: 0x001FF584 File Offset: 0x001FD784
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x001FF650 File Offset: 0x001FD850
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x001FF748 File Offset: 0x001FD948
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x04006F73 RID: 28531
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04006F74 RID: 28532
		public bool TwoDDistanceCheck;

		// Token: 0x04006F75 RID: 28533
		public bool TwoDPositionInfluence;

		// Token: 0x04006F76 RID: 28534
		public bool TwoDRotationInfluence;

		// Token: 0x04006F77 RID: 28535
		public bool EnablePositionEffect = true;

		// Token: 0x04006F78 RID: 28536
		public bool EnableRotationEffect = true;

		// Token: 0x04006F79 RID: 28537
		public bool EnableScaleEffect;

		// Token: 0x04006F7A RID: 28538
		public bool GlobalReactionUpVector;

		// Token: 0x04006F7B RID: 28539
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x04006F7C RID: 28540
		public bool LockTranslationX;

		// Token: 0x04006F7D RID: 28541
		public bool LockTranslationY;

		// Token: 0x04006F7E RID: 28542
		public bool LockTranslationZ;

		// Token: 0x04006F7F RID: 28543
		public BoingWork.Params Params;

		// Token: 0x04006F80 RID: 28544
		public SharedBoingParams SharedParams;

		// Token: 0x04006F81 RID: 28545
		internal bool PositionSpringDirty;

		// Token: 0x04006F82 RID: 28546
		internal bool RotationSpringDirty;

		// Token: 0x04006F83 RID: 28547
		internal bool ScaleSpringDirty;

		// Token: 0x04006F84 RID: 28548
		internal bool CachedTransformValid;

		// Token: 0x04006F85 RID: 28549
		internal Vector3 CachedPositionLs;

		// Token: 0x04006F86 RID: 28550
		internal Vector3 CachedPositionWs;

		// Token: 0x04006F87 RID: 28551
		internal Vector3 RenderPositionWs;

		// Token: 0x04006F88 RID: 28552
		internal Quaternion CachedRotationLs;

		// Token: 0x04006F89 RID: 28553
		internal Quaternion CachedRotationWs;

		// Token: 0x04006F8A RID: 28554
		internal Quaternion RenderRotationWs;

		// Token: 0x04006F8B RID: 28555
		internal Vector3 CachedScaleLs;

		// Token: 0x04006F8C RID: 28556
		internal Vector3 RenderScaleLs;

		// Token: 0x04006F8D RID: 28557
		internal bool InitRebooted;
	}
}
