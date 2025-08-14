using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033D RID: 829
public abstract class TeleportTargetHandler : TeleportSupport
{
	// Token: 0x060013CF RID: 5071 RVA: 0x0006A5BE File Offset: 0x000687BE
	protected TeleportTargetHandler()
	{
		this._startAimAction = delegate()
		{
			base.StartCoroutine(this.TargetAimCoroutine());
		};
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0006A5EE File Offset: 0x000687EE
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0006A607 File Offset: 0x00068807
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0006A620 File Offset: 0x00068820
	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			this.ResetAimData();
			Vector3 start = base.LocomotionTeleport.transform.position;
			this._aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(this._aimPoints);
			for (int i = 0; i < this._aimPoints.Count; i++)
			{
				Vector3 vector = this._aimPoints[i];
				this.AimData.TargetValid = this.ConsiderTeleport(start, ref vector);
				this.AimData.Points.Add(vector);
				if (this.AimData.TargetValid)
				{
					this.AimData.Destination = this.ConsiderDestination(vector);
					this.AimData.TargetValid = (this.AimData.Destination != null);
					break;
				}
				start = this._aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(this.AimData);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0006A62F File Offset: 0x0006882F
	protected virtual void ResetAimData()
	{
		this.AimData.Reset();
	}

	// Token: 0x060013D4 RID: 5076
	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	// Token: 0x060013D5 RID: 5077 RVA: 0x0006A63C File Offset: 0x0006883C
	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 end = vector;
		end.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, end, num, this.AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return new Vector3?(location);
	}

	// Token: 0x04001B4C RID: 6988
	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	// Token: 0x04001B4D RID: 6989
	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	// Token: 0x04001B4E RID: 6990
	private readonly Action _startAimAction;

	// Token: 0x04001B4F RID: 6991
	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	// Token: 0x04001B50 RID: 6992
	private const float ERROR_MARGIN = 0.1f;
}
