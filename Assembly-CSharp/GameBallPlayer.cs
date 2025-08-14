using System;
using UnityEngine;

// Token: 0x020004F6 RID: 1270
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x06001EE2 RID: 7906 RVA: 0x000A36A4 File Offset: 0x000A18A4
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000A36D8 File Offset: 0x000A18D8
	public void CleanupPlayer()
	{
		MonkeBallPlayer component = base.GetComponent<MonkeBallPlayer>();
		if (component != null)
		{
			component.currGoalZone = null;
			for (int i = 0; i < MonkeBallGame.Instance.goalZones.Count; i++)
			{
				MonkeBallGame.Instance.goalZones[i].CleanupPlayer(component);
			}
		}
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000A372C File Offset: 0x000A192C
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GameBallPlayer.HandData handData = this.hands[handIndex];
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000A376C File Offset: 0x000A196C
	public void ClearGrabbedIfHeld(GameBallId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000A37A5 File Offset: 0x000A19A5
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000A37B4 File Offset: 0x000A19B4
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000A37DB File Offset: 0x000A19DB
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000A3800 File Offset: 0x000A1A00
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000A381B File Offset: 0x000A1A1B
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000A3830 File Offset: 0x000A1A30
	public int FindHandIndex(GameBallId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000A386C File Offset: 0x000A1A6C
	public GameBallId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId.IsValid())
			{
				return this.hands[i].grabbedGameBallId;
			}
		}
		return GameBallId.Invalid;
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000A38BB File Offset: 0x000A1ABB
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000A38E3 File Offset: 0x000A1AE3
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000A38E9 File Offset: 0x000A1AE9
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x000A38F4 File Offset: 0x000A1AF4
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || player.IsNull || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x000A3930 File Offset: 0x000A1B30
	public static GameBallPlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		VRRig vrrig = GameBallPlayer.GetRig(actorNumber);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.GetComponent<GameBallPlayer>();
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000A395C File Offset: 0x000A1B5C
	public static GameBallPlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameBallPlayer component = transform.GetComponent<GameBallPlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0400278A RID: 10122
	public VRRig rig;

	// Token: 0x0400278B RID: 10123
	public int teamId;

	// Token: 0x0400278C RID: 10124
	private GameBallPlayer.HandData[] hands;

	// Token: 0x0400278D RID: 10125
	public const int MAX_HANDS = 2;

	// Token: 0x0400278E RID: 10126
	public const int LEFT_HAND = 0;

	// Token: 0x0400278F RID: 10127
	public const int RIGHT_HAND = 1;

	// Token: 0x04002790 RID: 10128
	private int inGoalZone;

	// Token: 0x020004F7 RID: 1271
	private struct HandData
	{
		// Token: 0x04002791 RID: 10129
		public GameBallId grabbedGameBallId;
	}
}
