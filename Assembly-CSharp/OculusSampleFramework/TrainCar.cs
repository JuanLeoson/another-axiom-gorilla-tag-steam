using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D2F RID: 3375
	public class TrainCar : TrainCarBase
	{
		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x0600538A RID: 21386 RVA: 0x0019D6D5 File Offset: 0x0019B8D5
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		// Token: 0x0600538B RID: 21387 RVA: 0x0019D6E4 File Offset: 0x0019B8E4
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x0600538C RID: 21388 RVA: 0x0019D6EC File Offset: 0x0019B8EC
		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		// Token: 0x04005CD5 RID: 23765
		[SerializeField]
		private TrainCarBase _parentLocomotive;

		// Token: 0x04005CD6 RID: 23766
		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
