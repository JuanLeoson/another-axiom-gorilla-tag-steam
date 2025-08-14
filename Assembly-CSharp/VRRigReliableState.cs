using System;
using System.Collections.Generic;
using Fusion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class VRRigReliableState : MonoBehaviour, IWrappedSerializable, INetworkStruct
{
	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001966 RID: 6502 RVA: 0x00088946 File Offset: 0x00086B46
	public bool HasBracelet
	{
		get
		{
			return this.braceletBeadColors.Count > 0;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001967 RID: 6503 RVA: 0x00088956 File Offset: 0x00086B56
	// (set) Token: 0x06001968 RID: 6504 RVA: 0x0008895E File Offset: 0x00086B5E
	public bool isDirty { get; private set; } = true;

	// Token: 0x06001969 RID: 6505 RVA: 0x00088967 File Offset: 0x00086B67
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
		RoomSystem.JoinedRoomEvent += new Action(this.SetIsDirty);
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000889A4 File Offset: 0x00086BA4
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000889C6 File Offset: 0x00086BC6
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000889CF File Offset: 0x00086BCF
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000889D8 File Offset: 0x00086BD8
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[5];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[5];
		this.transferrableItemStates = new TransferrableObject.ItemStates[5];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[5];
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x00088A40 File Offset: 0x00086C40
	void IWrappedSerializable.OnSerializeRead(object newData)
	{
		this.Data = (ReliableStateData)newData;
		long header = this.Data.Header;
		int num;
		this.SetHeader(header, out num);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L)
			{
				long num2 = this.Data.TransferrableStates[i];
				this.activeTransferrableObjectIndex[i] = (int)num2;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num2 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num2 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num2 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = this.Data.WearablesPackedState;
		this.lThrowableProjectileIndex = this.Data.LThrowableProjectileIndex;
		this.rThrowableProjectileIndex = this.Data.RThrowableProjectileIndex;
		this.sizeLayerMask = this.Data.SizeLayerMask;
		this.randomThrowableIndex = this.Data.RandomThrowableIndex;
		this.braceletBeadColors.Clear();
		if (num > 0)
		{
			if (num <= 3)
			{
				int num3 = (int)this.Data.PackedBeads;
				this.braceletSelfIndex = num3 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num3, 0, num, this.braceletBeadColors);
			}
			else
			{
				long packedBeads = this.Data.PackedBeads;
				this.braceletSelfIndex = (int)(packedBeads >> 60);
				if (num <= 6)
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, num, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors(this.Data.PackedBeadsMoreThan6, 6, num, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x00088C1C File Offset: 0x00086E1C
	object IWrappedSerializable.OnSerializeWrite()
	{
		this.isDirty = false;
		ReliableStateData reliableStateData = default(ReliableStateData);
		long header = this.GetHeader();
		reliableStateData.Header = header;
		long[] array = this.GetTransferrableStates(header).ToArray();
		reliableStateData.TransferrableStates.CopyFrom(array, 0, array.Length);
		reliableStateData.WearablesPackedState = this.wearablesPackedStates;
		reliableStateData.LThrowableProjectileIndex = this.lThrowableProjectileIndex;
		reliableStateData.RThrowableProjectileIndex = this.rThrowableProjectileIndex;
		reliableStateData.SizeLayerMask = this.sizeLayerMask;
		reliableStateData.RandomThrowableIndex = this.randomThrowableIndex;
		if (this.braceletBeadColors.Count > 0)
		{
			long num = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num |= (long)this.braceletSelfIndex << 30;
				reliableStateData.PackedBeads = num;
			}
			else
			{
				num |= (long)this.braceletSelfIndex << 60;
				reliableStateData.PackedBeads = num;
				if (this.braceletBeadColors.Count > 6)
				{
					reliableStateData.PackedBeadsMoreThan6 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6);
				}
			}
		}
		this.Data = reliableStateData;
		return reliableStateData;
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x00088D34 File Offset: 0x00086F34
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		long header = this.GetHeader();
		stream.SendNext(header);
		foreach (long num in this.GetTransferrableStates(header))
		{
			stream.SendNext(num);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.sizeLayerMask);
		stream.SendNext(this.randomThrowableIndex);
		if (this.braceletBeadColors.Count > 0)
		{
			long num2 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num2 |= (long)this.braceletSelfIndex << 30;
				stream.SendNext((int)num2);
				return;
			}
			num2 |= (long)this.braceletSelfIndex << 60;
			stream.SendNext(num2);
			if (this.braceletBeadColors.Count > 6)
			{
				stream.SendNext(VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6));
			}
		}
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x00088E88 File Offset: 0x00087088
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		long num = (long)stream.ReceiveNext();
		this.isMicEnabled = ((num & 32L) != 0L);
		this.isBraceletLeftHanded = ((num & 64L) != 0L);
		this.isBuilderWatchEnabled = ((num & 128L) != 0L);
		int num2 = (int)(num >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(num >> 16);
		this.lThrowableProjectileColor.g = (byte)(num >> 24);
		this.lThrowableProjectileColor.b = (byte)(num >> 32);
		this.rThrowableProjectileColor.r = (byte)(num >> 40);
		this.rThrowableProjectileColor.g = (byte)(num >> 48);
		this.rThrowableProjectileColor.b = (byte)(num >> 56);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((num & 1L << (i & 31)) != 0L)
			{
				long num3 = (long)stream.ReceiveNext();
				this.activeTransferrableObjectIndex[i] = (int)num3;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num3 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num3 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num3 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		this.braceletBeadColors.Clear();
		if (num2 > 0)
		{
			if (num2 <= 3)
			{
				int num4 = (int)stream.ReceiveNext();
				this.braceletSelfIndex = num4 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num4, 0, num2, this.braceletBeadColors);
			}
			else
			{
				long num5 = (long)stream.ReceiveNext();
				this.braceletSelfIndex = (int)(num5 >> 60);
				if (num2 <= 6)
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, num2, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors((long)stream.ReceiveNext(), 6, num2, this.braceletBeadColors);
				}
			}
		}
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.bDock.RefreshTransferrableItems();
		}
		this.bDock.myRig.UpdateFriendshipBracelet();
		this.bDock.myRig.EnableBuilderResizeWatch(this.isBuilderWatchEnabled);
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000890F8 File Offset: 0x000872F8
	private long GetHeader()
	{
		long num = 0L;
		if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
			{
				if (this.activeTransferrableObjectIndex[i] != -1 && (this.transferrablePosStates[i] == TransferrableObject.PositionState.InLeftHand || this.transferrablePosStates[i] == TransferrableObject.PositionState.InRightHand))
				{
					num |= (long)((ulong)((byte)(1 << i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < this.activeTransferrableObjectIndex.Length; j++)
			{
				if (this.activeTransferrableObjectIndex[j] != -1)
				{
					num |= (long)((ulong)((byte)(1 << j)));
				}
			}
		}
		if (this.isBraceletLeftHanded)
		{
			num |= 64L;
		}
		if (this.isMicEnabled)
		{
			num |= 32L;
		}
		if (this.isBuilderWatchEnabled && !CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			num |= 128L;
		}
		num |= ((long)this.braceletBeadColors.Count & 15L) << 12;
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.r) << 16);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.g) << 24);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.b) << 32);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.r) << 40);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.g) << 48);
		return num | (long)((long)((ulong)this.rThrowableProjectileColor.b) << 56);
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x00089240 File Offset: 0x00087440
	private void SetHeader(long header, out int numBeadsToRead)
	{
		this.isMicEnabled = ((header & 32L) != 0L);
		this.isBraceletLeftHanded = ((header & 64L) != 0L);
		numBeadsToRead = ((int)(header >> 12) & 15);
		this.lThrowableProjectileColor.r = (byte)(header >> 16);
		this.lThrowableProjectileColor.g = (byte)(header >> 24);
		this.lThrowableProjectileColor.b = (byte)(header >> 32);
		this.rThrowableProjectileColor.r = (byte)(header >> 40);
		this.rThrowableProjectileColor.g = (byte)(header >> 48);
		this.rThrowableProjectileColor.b = (byte)(header >> 56);
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000892D8 File Offset: 0x000874D8
	private List<long> GetTransferrableStates(long header)
	{
		List<long> list = new List<long>();
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L && this.activeTransferrableObjectIndex[i] != -1)
			{
				long num = (long)((ulong)this.activeTransferrableObjectIndex[i]);
				num |= (long)this.transferrablePosStates[i] << 32;
				num |= (long)this.transferrableItemStates[i] << 40;
				num |= (long)this.transferableDockPositions[i] << 48;
				list.Add(num);
			}
		}
		return list;
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x00089354 File Offset: 0x00087554
	private static long PackBeadColors(List<Color> beadColors, int fromIndex)
	{
		long num = 0L;
		int num2 = Mathf.Min(fromIndex + 6, beadColors.Count);
		int num3 = 0;
		for (int i = fromIndex; i < num2; i++)
		{
			long num4 = (long)FriendshipGroupDetection.PackColor(beadColors[i]);
			num |= num4 << num3;
			num3 += 10;
		}
		return num;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000893A0 File Offset: 0x000875A0
	private static void UnpackBeadColors(long packed, int startIndex, int endIndex, List<Color> beadColorsResult)
	{
		int num = Mathf.Min(startIndex + 6, endIndex);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			short data = (short)(packed >> num2 & 1023L);
			beadColorsResult.Add(FriendshipGroupDetection.UnpackColor(data));
			num2 += 10;
		}
	}

	// Token: 0x040021B5 RID: 8629
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x040021B6 RID: 8630
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x040021B7 RID: 8631
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x040021B8 RID: 8632
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x040021B9 RID: 8633
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x040021BA RID: 8634
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x040021BB RID: 8635
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x040021BC RID: 8636
	[NonSerialized]
	public Color32 lThrowableProjectileColor = Color.white;

	// Token: 0x040021BD RID: 8637
	[NonSerialized]
	public Color32 rThrowableProjectileColor = Color.white;

	// Token: 0x040021BE RID: 8638
	[NonSerialized]
	public int randomThrowableIndex;

	// Token: 0x040021BF RID: 8639
	[NonSerialized]
	public bool isMicEnabled;

	// Token: 0x040021C0 RID: 8640
	private bool isOfflineVRRig;

	// Token: 0x040021C1 RID: 8641
	private BodyDockPositions bDock;

	// Token: 0x040021C2 RID: 8642
	[NonSerialized]
	public int sizeLayerMask = 1;

	// Token: 0x040021C3 RID: 8643
	private const long IS_MIC_ENABLED_BIT = 32L;

	// Token: 0x040021C4 RID: 8644
	private const long BRACELET_LEFTHAND_BIT = 64L;

	// Token: 0x040021C5 RID: 8645
	private const long BUILDER_WATCH_ENABLED_BIT = 128L;

	// Token: 0x040021C6 RID: 8646
	private const int BRACELET_NUM_BEADS_SHIFT = 12;

	// Token: 0x040021C7 RID: 8647
	private const int LPROJECTILECOLOR_R_SHIFT = 16;

	// Token: 0x040021C8 RID: 8648
	private const int LPROJECTILECOLOR_G_SHIFT = 24;

	// Token: 0x040021C9 RID: 8649
	private const int LPROJECTILECOLOR_B_SHIFT = 32;

	// Token: 0x040021CA RID: 8650
	private const int RPROJECTILECOLOR_R_SHIFT = 40;

	// Token: 0x040021CB RID: 8651
	private const int RPROJECTILECOLOR_G_SHIFT = 48;

	// Token: 0x040021CC RID: 8652
	private const int RPROJECTILECOLOR_B_SHIFT = 56;

	// Token: 0x040021CD RID: 8653
	private const int POS_STATES_SHIFT = 32;

	// Token: 0x040021CE RID: 8654
	private const int ITEM_STATES_SHIFT = 40;

	// Token: 0x040021CF RID: 8655
	private const int DOCK_POSITIONS_SHIFT = 48;

	// Token: 0x040021D0 RID: 8656
	private const int BRACELET_SELF_INDEX_SHIFT = 60;

	// Token: 0x040021D1 RID: 8657
	[NonSerialized]
	public bool isBraceletLeftHanded;

	// Token: 0x040021D2 RID: 8658
	[NonSerialized]
	public int braceletSelfIndex;

	// Token: 0x040021D3 RID: 8659
	[NonSerialized]
	public List<Color> braceletBeadColors = new List<Color>(10);

	// Token: 0x040021D4 RID: 8660
	[NonSerialized]
	public bool isBuilderWatchEnabled;

	// Token: 0x040021D6 RID: 8662
	private ReliableStateData Data;
}
