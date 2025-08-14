using System;
using UnityEngine;

namespace com.AnotherAxiom.SpaceFight
{
	// Token: 0x02000DF1 RID: 3569
	public class SpaceFight : ArcadeGame
	{
		// Token: 0x0600586B RID: 22635 RVA: 0x001B7514 File Offset: 0x001B5714
		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.getButtonState(i, ArcadeButtons.UP))
				{
					this.move(this.player[i], 0.15f);
					this.clamp(this.player[i]);
				}
				if (base.getButtonState(i, ArcadeButtons.RIGHT))
				{
					this.turn(this.player[i], true);
				}
				if (base.getButtonState(i, ArcadeButtons.LEFT))
				{
					this.turn(this.player[i], false);
				}
				if (this.projectilesFired[i])
				{
					this.move(this.projectile[i], 0.5f);
					if (Vector2.Distance(this.player[1 - i].localPosition, this.projectile[i].localPosition) < 0.25f)
					{
						base.PlaySound(1, 2);
						this.player[1 - i].Rotate(0f, 0f, 180f);
						this.projectilesFired[i] = false;
					}
					if (Mathf.Abs(this.projectile[i].localPosition.x) > this.tableSize.x || Mathf.Abs(this.projectile[i].localPosition.y) > this.tableSize.y)
					{
						this.projectilesFired[i] = false;
					}
				}
				if (!this.projectilesFired[i])
				{
					this.projectile[i].position = this.player[i].position;
					this.projectile[i].rotation = this.player[i].rotation;
				}
			}
		}

		// Token: 0x0600586C RID: 22636 RVA: 0x001B76A4 File Offset: 0x001B58A4
		private void clamp(Transform tr)
		{
			tr.localPosition = new Vector2(Mathf.Clamp(tr.localPosition.x, -this.tableSize.x, this.tableSize.x), Mathf.Clamp(tr.localPosition.y, -this.tableSize.y, this.tableSize.y));
		}

		// Token: 0x0600586D RID: 22637 RVA: 0x001B770F File Offset: 0x001B590F
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.TRIGGER)
			{
				if (!this.projectilesFired[player])
				{
					base.PlaySound(0, 3);
				}
				this.projectilesFired[player] = true;
			}
		}

		// Token: 0x0600586E RID: 22638 RVA: 0x001B7734 File Offset: 0x001B5934
		private void move(Transform p, float speed)
		{
			p.Translate(p.up * Time.deltaTime * speed, Space.World);
		}

		// Token: 0x0600586F RID: 22639 RVA: 0x001B7753 File Offset: 0x001B5953
		private void turn(Transform p, bool cw)
		{
			p.Rotate(0f, 0f, (float)(cw ? 180 : -180) * Time.deltaTime);
		}

		// Token: 0x06005870 RID: 22640 RVA: 0x001B777C File Offset: 0x001B597C
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P1LocX = this.player[0].localPosition.x;
			this.netStateCur.P1LocY = this.player[0].localPosition.y;
			this.netStateCur.P1Rot = this.player[0].localRotation.eulerAngles.z;
			this.netStateCur.P2LocX = this.player[1].localPosition.x;
			this.netStateCur.P2LocY = this.player[1].localPosition.y;
			this.netStateCur.P2Rot = this.player[1].localRotation.eulerAngles.z;
			this.netStateCur.P1PrLocX = this.projectile[0].localPosition.x;
			this.netStateCur.P1PrLocY = this.projectile[0].localPosition.y;
			this.netStateCur.P2PrLocX = this.projectile[1].localPosition.x;
			this.netStateCur.P2PrLocY = this.projectile[1].localPosition.y;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06005871 RID: 22641 RVA: 0x001B78FC File Offset: 0x001B5AFC
		public override void SetNetworkState(byte[] b)
		{
			SpaceFight.SpaceFlightNetState spaceFlightNetState = (SpaceFight.SpaceFlightNetState)ArcadeGame.UnwrapNetState(b);
			this.player[0].localPosition = new Vector2(spaceFlightNetState.P1LocX, spaceFlightNetState.P1LocY);
			this.player[0].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P1Rot);
			this.player[1].localPosition = new Vector2(spaceFlightNetState.P2LocX, spaceFlightNetState.P2LocY);
			this.player[1].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P2Rot);
			this.projectile[0].localPosition = new Vector2(spaceFlightNetState.P1PrLocX, spaceFlightNetState.P1PrLocY);
			this.projectile[1].localPosition = new Vector2(spaceFlightNetState.P2PrLocX, spaceFlightNetState.P2PrLocY);
		}

		// Token: 0x06005872 RID: 22642 RVA: 0x000023F5 File Offset: 0x000005F5
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06005873 RID: 22643 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnTimeout()
		{
		}

		// Token: 0x04006220 RID: 25120
		[SerializeField]
		private Transform[] player;

		// Token: 0x04006221 RID: 25121
		[SerializeField]
		private Transform[] projectile;

		// Token: 0x04006222 RID: 25122
		[SerializeField]
		private Vector2 tableSize;

		// Token: 0x04006223 RID: 25123
		private bool[] projectilesFired = new bool[2];

		// Token: 0x04006224 RID: 25124
		private SpaceFight.SpaceFlightNetState netStateLast;

		// Token: 0x04006225 RID: 25125
		private SpaceFight.SpaceFlightNetState netStateCur;

		// Token: 0x02000DF2 RID: 3570
		[Serializable]
		private struct SpaceFlightNetState : IEquatable<SpaceFight.SpaceFlightNetState>
		{
			// Token: 0x06005875 RID: 22645 RVA: 0x001B79FC File Offset: 0x001B5BFC
			public bool Equals(SpaceFight.SpaceFlightNetState other)
			{
				return this.P1LocX.Approx(other.P1LocX, 1E-06f) && this.P1LocY.Approx(other.P1LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P2LocX.Approx(other.P2LocX, 1E-06f) && this.P2LocY.Approx(other.P2LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P1PrLocX.Approx(other.P1PrLocX, 1E-06f) && this.P1PrLocY.Approx(other.P1PrLocY, 1E-06f) && this.P2PrLocX.Approx(other.P2PrLocX, 1E-06f) && this.P2PrLocY.Approx(other.P2PrLocY, 1E-06f);
			}

			// Token: 0x04006226 RID: 25126
			public float P1LocX;

			// Token: 0x04006227 RID: 25127
			public float P1LocY;

			// Token: 0x04006228 RID: 25128
			public float P1Rot;

			// Token: 0x04006229 RID: 25129
			public float P2LocX;

			// Token: 0x0400622A RID: 25130
			public float P2LocY;

			// Token: 0x0400622B RID: 25131
			public float P2Rot;

			// Token: 0x0400622C RID: 25132
			public float P1PrLocX;

			// Token: 0x0400622D RID: 25133
			public float P1PrLocY;

			// Token: 0x0400622E RID: 25134
			public float P2PrLocX;

			// Token: 0x0400622F RID: 25135
			public float P2PrLocY;
		}
	}
}
