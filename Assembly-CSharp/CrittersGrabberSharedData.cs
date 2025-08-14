using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000041 RID: 65
public static class CrittersGrabberSharedData
{
	// Token: 0x0600013F RID: 319 RVA: 0x00008CA9 File Offset: 0x00006EA9
	public static void Initialize()
	{
		if (CrittersGrabberSharedData.initialized)
		{
			return;
		}
		CrittersGrabberSharedData.initialized = true;
		CrittersGrabberSharedData.enteredCritterActor = new List<CrittersActor>();
		CrittersGrabberSharedData.triggerCollidersToCheck = new List<CapsuleCollider>();
		CrittersGrabberSharedData.heldActor = new List<CrittersActor>();
		CrittersGrabberSharedData.actorGrabbers = new List<CrittersActorGrabber>();
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00008CE1 File Offset: 0x00006EE1
	public static void AddEnteredActor(CrittersActor actor)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.enteredCritterActor.Contains(actor))
		{
			return;
		}
		CrittersGrabberSharedData.enteredCritterActor.Add(actor);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00008D01 File Offset: 0x00006F01
	public static void RemoveEnteredActor(CrittersActor actor)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.enteredCritterActor.Contains(actor))
		{
			return;
		}
		CrittersGrabberSharedData.enteredCritterActor.Remove(actor);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00008D22 File Offset: 0x00006F22
	public static void AddTrigger(CapsuleCollider trigger)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.triggerCollidersToCheck.Contains(trigger))
		{
			return;
		}
		CrittersGrabberSharedData.triggerCollidersToCheck.Add(trigger);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00008D42 File Offset: 0x00006F42
	public static void RemoveTrigger(CapsuleCollider trigger)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.triggerCollidersToCheck.Contains(trigger))
		{
			return;
		}
		CrittersGrabberSharedData.triggerCollidersToCheck.Remove(trigger);
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00008D63 File Offset: 0x00006F63
	public static void AddActorGrabber(CrittersActorGrabber grabber)
	{
		CrittersGrabberSharedData.Initialize();
		if (CrittersGrabberSharedData.actorGrabbers.Contains(grabber))
		{
			return;
		}
		CrittersGrabberSharedData.actorGrabbers.Add(grabber);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00008D83 File Offset: 0x00006F83
	public static void RemoveActorGrabber(CrittersActorGrabber grabber)
	{
		CrittersGrabberSharedData.Initialize();
		if (!CrittersGrabberSharedData.actorGrabbers.Contains(grabber))
		{
			return;
		}
		CrittersGrabberSharedData.actorGrabbers.Remove(grabber);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00008DA4 File Offset: 0x00006FA4
	public static void DisableEmptyGrabberJoints()
	{
		CrittersGrabberSharedData.Initialize();
		for (int i = 0; i < CrittersGrabberSharedData.actorGrabbers.Count; i++)
		{
			if (CrittersGrabberSharedData.actorGrabbers[i].grabber != null && CrittersGrabberSharedData.actorGrabbers[i].actorsStillPresent.Count == 0)
			{
				for (int j = 0; j < CrittersGrabberSharedData.actorGrabbers[i].grabber.grabbedActors.Count; j++)
				{
					CrittersGrabberSharedData.actorGrabbers[i].grabber.grabbedActors[j].DisconnectJoint();
				}
			}
		}
	}

	// Token: 0x0400016B RID: 363
	public static List<CrittersActor> enteredCritterActor;

	// Token: 0x0400016C RID: 364
	public static List<CapsuleCollider> triggerCollidersToCheck;

	// Token: 0x0400016D RID: 365
	public static List<CrittersActor> heldActor;

	// Token: 0x0400016E RID: 366
	public static List<CrittersActorGrabber> actorGrabbers;

	// Token: 0x0400016F RID: 367
	private static bool initialized;
}
