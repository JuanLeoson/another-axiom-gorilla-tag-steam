using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000DF8 RID: 3576
	public class JoustPlayer : MonoBehaviour
	{
		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06005890 RID: 22672 RVA: 0x001B8A09 File Offset: 0x001B6C09
		// (set) Token: 0x06005891 RID: 22673 RVA: 0x001B8A11 File Offset: 0x001B6C11
		public float HorizontalSpeed
		{
			get
			{
				return this.HSpeed;
			}
			set
			{
				this.HSpeed = value;
			}
		}

		// Token: 0x06005892 RID: 22674 RVA: 0x001B8A1C File Offset: 0x001B6C1C
		private void LateUpdate()
		{
			this.velocity.x = this.HSpeed * 0.001f;
			if (this.flap)
			{
				this.velocity.y = Mathf.Min(this.velocity.y + 0.0005f, 0.0005f);
				this.flap = false;
			}
			else
			{
				this.velocity.y = Mathf.Max(this.velocity.y - Time.deltaTime * 0.0001f, -0.001f);
				int i = 0;
				while (i < Physics2D.RaycastNonAlloc(base.transform.position, this.velocity.normalized, this.raycastHitResults, this.velocity.magnitude))
				{
					JoustTerrain joustTerrain;
					if (this.raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(out joustTerrain))
					{
						this.velocity.y = 0f;
						if (joustTerrain.transform.localPosition.y < base.transform.localPosition.y)
						{
							base.transform.localPosition = new Vector2(base.transform.localPosition.x, joustTerrain.transform.localPosition.y + this.raycastHitResults[i].collider.bounds.size.y);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.transform.Translate(this.velocity);
			if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
			}
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x001B8C02 File Offset: 0x001B6E02
		public void Flap()
		{
			this.flap = true;
		}

		// Token: 0x04006266 RID: 25190
		private Vector2 velocity;

		// Token: 0x04006267 RID: 25191
		private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

		// Token: 0x04006268 RID: 25192
		private float HSpeed;

		// Token: 0x04006269 RID: 25193
		private bool flap;
	}
}
