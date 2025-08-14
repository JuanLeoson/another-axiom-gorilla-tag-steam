using System;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class GRShieldCollider : MonoBehaviour
{
	// Token: 0x170003BC RID: 956
	// (get) Token: 0x0600283F RID: 10303 RVA: 0x000D8F08 File Offset: 0x000D7108
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x06002840 RID: 10304 RVA: 0x000D8F10 File Offset: 0x000D7110
	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000D8F18 File Offset: 0x000D7118
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000D8F34 File Offset: 0x000D7134
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.BlockHittable(enemyPosition, enemyAttackDirection, hittable, this);
		}
	}

	// Token: 0x040033B9 RID: 13241
	[SerializeField]
	private float knockbackVelocity = 3f;

	// Token: 0x040033BA RID: 13242
	[SerializeField]
	private GRToolDirectionalShield shieldTool;
}
