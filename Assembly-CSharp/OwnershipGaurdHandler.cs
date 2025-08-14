using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A0D RID: 2573
internal class OwnershipGaurdHandler : IPunOwnershipCallbacks
{
	// Token: 0x06003ECD RID: 16077 RVA: 0x0013FA79 File Offset: 0x0013DC79
	static OwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x06003ECE RID: 16078 RVA: 0x0013FA99 File Offset: 0x0013DC99
	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Add(view);
	}

	// Token: 0x06003ECF RID: 16079 RVA: 0x0013FAC0 File Offset: 0x0013DCC0
	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RegisterView(photonViews[i]);
		}
	}

	// Token: 0x06003ED0 RID: 16080 RVA: 0x0013FAE5 File Offset: 0x0013DCE5
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Remove(view);
	}

	// Token: 0x06003ED1 RID: 16081 RVA: 0x0013FB00 File Offset: 0x0013DD00
	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RemoveView(photonViews[i]);
		}
	}

	// Token: 0x06003ED2 RID: 16082 RVA: 0x0013FB28 File Offset: 0x0013DD28
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGaurdHandler.gaurdedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	// Token: 0x06003ED3 RID: 16083 RVA: 0x000023F5 File Offset: 0x000005F5
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06003ED4 RID: 16084 RVA: 0x000023F5 File Offset: 0x000005F5
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x04004ADA RID: 19162
	private static HashSet<PhotonView> gaurdedViews = new HashSet<PhotonView>();

	// Token: 0x04004ADB RID: 19163
	private static readonly OwnershipGaurdHandler callbackInstance = new OwnershipGaurdHandler();
}
