using System;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class TestScreen : ArcadeGame
{
	// Token: 0x06000FC3 RID: 4035 RVA: 0x00058615 File Offset: 0x00056815
	public override byte[] GetNetworkState()
	{
		return null;
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void SetNetworkState(byte[] b)
	{
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0005C07C File Offset: 0x0005A27C
	private int buttonToLightIndex(int player, ArcadeButtons button)
	{
		int num = 0;
		if (button <= ArcadeButtons.RIGHT)
		{
			switch (button)
			{
			case ArcadeButtons.GRAB:
				num = 0;
				break;
			case ArcadeButtons.UP:
				num = 1;
				break;
			case ArcadeButtons.GRAB | ArcadeButtons.UP:
				break;
			case ArcadeButtons.DOWN:
				num = 2;
				break;
			default:
				if (button != ArcadeButtons.LEFT)
				{
					if (button == ArcadeButtons.RIGHT)
					{
						num = 4;
					}
				}
				else
				{
					num = 3;
				}
				break;
			}
		}
		else if (button != ArcadeButtons.B0)
		{
			if (button != ArcadeButtons.B1)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					num = 7;
				}
			}
			else
			{
				num = 6;
			}
		}
		else
		{
			num = 5;
		}
		return (player * 8 + num) % this.lights.Length;
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x0005C0F3 File Offset: 0x0005A2F3
	protected override void ButtonUp(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.red;
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0005C10E File Offset: 0x0005A30E
	protected override void ButtonDown(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.green;
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnTimeout()
	{
	}

	// Token: 0x04001854 RID: 6228
	[SerializeField]
	private SpriteRenderer[] lights;

	// Token: 0x04001855 RID: 6229
	[SerializeField]
	private Transform dot;
}
