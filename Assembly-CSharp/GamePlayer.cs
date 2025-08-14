using System;
using System.IO;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005BF RID: 1471
public class GamePlayer : MonoBehaviour
{
	// Token: 0x06002421 RID: 9249 RVA: 0x000C18C8 File Offset: 0x000BFAC8
	private void Awake()
	{
		this.handTransforms = new Transform[2];
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		this.hands = new GamePlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000C1990 File Offset: 0x000BFB90
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GamePlayer.HandData handData = this.hands[handIndex];
		handData.grabbedEntityId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000C19D0 File Offset: 0x000BFBD0
	public void Clear()
	{
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ClearZone()
	{
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000C19F0 File Offset: 0x000BFBF0
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000C1A29 File Offset: 0x000BFC29
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000C1A37 File Offset: 0x000BFC37
	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000C1A3F File Offset: 0x000BFC3F
	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000C1A48 File Offset: 0x000BFC48
	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000C1A5C File Offset: 0x000BFC5C
	public bool IsHoldingEntity(GameEntityManager gameEntityManager, bool isLeftHand)
	{
		return gameEntityManager.GetGameEntity(this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000C1A76 File Offset: 0x000BFC76
	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000C1A84 File Offset: 0x000BFC84
	public GameEntityId GetGameEntityId(int handIndex)
	{
		return this.hands[handIndex].grabbedEntityId;
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000C1A98 File Offset: 0x000BFC98
	public int FindHandIndex(GameEntityId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000C1AD4 File Offset: 0x000BFCD4
	public GameEntityId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId.IsValid())
			{
				return this.hands[i].grabbedEntityId;
			}
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000A38E3 File Offset: 0x000A1AE3
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000A38E9 File Offset: 0x000A1AE9
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000C1B24 File Offset: 0x000BFD24
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x000C1B59 File Offset: 0x000BFD59
	public static GamePlayer GetGamePlayer(Player player)
	{
		if (player == null)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(player.ActorNumber);
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x000C1B6B File Offset: 0x000BFD6B
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(GamePlayer.GetRig(actorNumber));
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000C1B7E File Offset: 0x000BFD7E
	public static GamePlayer GetGamePlayer(VRRig rig)
	{
		if (rig == null)
		{
			return null;
		}
		return rig.GamePlayerRef;
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000C1B94 File Offset: 0x000BFD94
	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
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

	// Token: 0x06002436 RID: 9270 RVA: 0x000C1BD0 File Offset: 0x000BFDD0
	public static Transform GetHandTransform(VRRig rig, int handIndex)
	{
		if (handIndex < 0 || handIndex >= 2)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(rig).handTransforms[handIndex];
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000C1BE9 File Offset: 0x000BFDE9
	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000C1C10 File Offset: 0x000BFE10
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player, GameEntityManager manager)
	{
		for (int i = 0; i < 2; i++)
		{
			int netIdFromEntityId = manager.GetNetIdFromEntityId(this.hands[i].grabbedEntityId);
			writer.Write(netIdFromEntityId);
			if (netIdFromEntityId != -1)
			{
				long value = 0L;
				GameEntity gameEntity = manager.GetGameEntity(this.hands[i].grabbedEntityId);
				if (gameEntity != null)
				{
					value = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
				}
				writer.Write(value);
			}
		}
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000C1C94 File Offset: 0x000BFE94
	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer, GameEntityManager manager)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = reader.ReadInt32();
			if (num != -1)
			{
				long num2 = reader.ReadInt64();
				GameEntityId entityIdFromNetId = manager.GetEntityIdFromNetId(num);
				if (entityIdFromNetId.IsValid())
				{
					GameEntity gameEntity = manager.GetGameEntity(entityIdFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out localPosition, out localRotation);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							manager.GrabEntityOnCreate(entityIdFromNetId, GamePlayer.IsLeftHand(i), localPosition, localRotation, gamePlayer.rig.OwningNetPlayer);
						}
					}
				}
			}
		}
	}

	// Token: 0x04002D91 RID: 11665
	public VRRig rig;

	// Token: 0x04002D92 RID: 11666
	public Transform leftHand;

	// Token: 0x04002D93 RID: 11667
	public Transform rightHand;

	// Token: 0x04002D94 RID: 11668
	private Transform[] handTransforms;

	// Token: 0x04002D95 RID: 11669
	private GamePlayer.HandData[] hands;

	// Token: 0x04002D96 RID: 11670
	public const int MAX_HANDS = 2;

	// Token: 0x04002D97 RID: 11671
	public const int LEFT_HAND = 0;

	// Token: 0x04002D98 RID: 11672
	public const int RIGHT_HAND = 1;

	// Token: 0x04002D99 RID: 11673
	public CallLimiter newJoinZoneLimiter;

	// Token: 0x04002D9A RID: 11674
	public CallLimiter netImpulseLimiter;

	// Token: 0x04002D9B RID: 11675
	public CallLimiter netGrabLimiter;

	// Token: 0x04002D9C RID: 11676
	public CallLimiter netThrowLimiter;

	// Token: 0x04002D9D RID: 11677
	public CallLimiter netStateLimiter;

	// Token: 0x04002D9E RID: 11678
	private bool grabbingDisabled;

	// Token: 0x020005C0 RID: 1472
	private struct HandData
	{
		// Token: 0x04002D9F RID: 11679
		public GameEntityId grabbedEntityId;
	}

	// Token: 0x020005C1 RID: 1473
	public enum ZoneState
	{
		// Token: 0x04002DA1 RID: 11681
		NotInZone,
		// Token: 0x04002DA2 RID: 11682
		WaitingForState,
		// Token: 0x04002DA3 RID: 11683
		Active
	}
}
