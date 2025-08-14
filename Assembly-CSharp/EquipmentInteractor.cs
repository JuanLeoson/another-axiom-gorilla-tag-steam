using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003D0 RID: 976
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x17000274 RID: 628
	// (get) Token: 0x060016B6 RID: 5814 RVA: 0x0007BA0B File Offset: 0x00079C0B
	public GorillaHandClimber BodyClimber
	{
		get
		{
			return this.bodyClimber;
		}
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x060016B7 RID: 5815 RVA: 0x0007BA13 File Offset: 0x00079C13
	public GorillaHandClimber LeftClimber
	{
		get
		{
			return this.leftClimber;
		}
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x060016B8 RID: 5816 RVA: 0x0007BA1B File Offset: 0x00079C1B
	public GorillaHandClimber RightClimber
	{
		get
		{
			return this.rightClimber;
		}
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0007BA24 File Offset: 0x00079C24
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0007BA78 File Offset: 0x00079C78
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0007BA97 File Offset: 0x00079C97
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0007BAD6 File Offset: 0x00079CD6
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0007BB15 File Offset: 0x00079D15
	public void ForceStopClimbing()
	{
		this.bodyClimber.ForceStopClimbing(false, false);
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0007BB3E File Offset: 0x00079D3E
	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment != null;
		}
		return this.rightHandHeldEquipment != null;
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0007BB58 File Offset: 0x00079D58
	public void InteractionPointDisabled(InteractionPoint interactionPoint)
	{
		if (this.iteratingInteractionPoints)
		{
			this.interactionPointsToRemove.Add(interactionPoint);
			return;
		}
		if (this.overlapInteractionPointsLeft != null)
		{
			this.overlapInteractionPointsLeft.Remove(interactionPoint);
		}
		if (this.overlapInteractionPointsRight != null)
		{
			this.overlapInteractionPointsRight.Remove(interactionPoint);
		}
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x0007BBA4 File Offset: 0x00079DA4
	public bool CanGrabLeft()
	{
		return !this.disableLeftGrab && this.leftHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[0] == null;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0007BBCF File Offset: 0x00079DCF
	public bool CanGrabRight()
	{
		return !this.disableRightGrab && this.rightHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[1] == null;
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0007BBFC File Offset: 0x00079DFC
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.CheckInputValue(true);
		this.isLeftGrabbing = ((this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.leftClimber && this.leftClimber.isClimbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = ((this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.rightClimber && this.rightClimber.isClimbing)
		{
			this.isRightGrabbing = false;
		}
		BuilderPiece pieceInHand = this.builderPieceInteractor.heldPiece[0];
		BuilderPiece pieceInHand2 = this.builderPieceInteractor.heldPiece[1];
		this.FireHandInteractions(this.leftHand, true, pieceInHand);
		this.FireHandInteractions(this.rightHand, false, pieceInHand2);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.builderPieceInteractor.OnLateUpdate();
		if (GameBallPlayerLocal.instance != null)
		{
			GameBallPlayerLocal.instance.OnUpdateInteract();
		}
		if (GamePlayerLocal.instance != null)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0007BDB8 File Offset: 0x00079FB8
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand, BuilderPiece pieceInHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = ((this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft));
			this.justReleased = (this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed);
		}
		else
		{
			this.justGrabbed = ((this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight));
			this.justReleased = (this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed);
		}
		List<InteractionPoint> list = isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight;
		bool flag = isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null);
		bool flag2 = pieceInHand != null;
		bool flag3 = isLeftHand ? this.disableLeftGrab : this.disableRightGrab;
		bool flag4 = !flag && !flag2 && !flag3;
		this.iteratingInteractionPoints = true;
		foreach (InteractionPoint interactionPoint in list)
		{
			if (flag4 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.Holdable.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.Holdable.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
		this.iteratingInteractionPoints = false;
		foreach (InteractionPoint item in this.interactionPointsToRemove)
		{
			if (this.overlapInteractionPointsLeft != null)
			{
				this.overlapInteractionPointsLeft.Remove(item);
			}
			if (this.overlapInteractionPointsRight != null)
			{
				this.overlapInteractionPointsRight.Remove(item);
			}
		}
		this.interactionPointsToRemove.Clear();
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0007C010 File Offset: 0x0007A210
	public void UpdateHandEquipment(IHoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment != null && newEquipment == this.rightHandHeldEquipment && !newEquipment.TwoHanded)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment != null && newEquipment == this.leftHandHeldEquipment && !newEquipment.TwoHanded)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0007C09C File Offset: 0x0007A29C
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x0007C0F5 File Offset: 0x0007A2F5
	public void ForceDropEquipment(IHoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x0007C117 File Offset: 0x0007A317
	public void ForceDropAnyEquipment()
	{
		this.rightHandHeldEquipment = null;
		this.leftHandHeldEquipment = null;
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0007C128 File Offset: 0x0007A328
	public void ForceDropManipulatableObject(HoldableObject manipulatableObject)
	{
		if ((HoldableObject)this.rightHandHeldEquipment == manipulatableObject)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
			this.rightHandHeldEquipment = null;
			this.autoGrabRight = false;
		}
		if ((HoldableObject)this.leftHandHeldEquipment == manipulatableObject)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
			this.leftHandHeldEquipment = null;
			this.autoGrabLeft = false;
		}
	}

	// Token: 0x04001E86 RID: 7814
	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	// Token: 0x04001E87 RID: 7815
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04001E88 RID: 7816
	public IHoldableObject leftHandHeldEquipment;

	// Token: 0x04001E89 RID: 7817
	public IHoldableObject rightHandHeldEquipment;

	// Token: 0x04001E8A RID: 7818
	public BuilderPieceInteractor builderPieceInteractor;

	// Token: 0x04001E8B RID: 7819
	public GameObject rightHand;

	// Token: 0x04001E8C RID: 7820
	public GameObject leftHand;

	// Token: 0x04001E8D RID: 7821
	public InputDevice leftHandDevice;

	// Token: 0x04001E8E RID: 7822
	public InputDevice rightHandDevice;

	// Token: 0x04001E8F RID: 7823
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x04001E90 RID: 7824
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x04001E91 RID: 7825
	public float grabRadius;

	// Token: 0x04001E92 RID: 7826
	public float grabThreshold = 0.7f;

	// Token: 0x04001E93 RID: 7827
	public float grabHysteresis = 0.05f;

	// Token: 0x04001E94 RID: 7828
	public bool wasLeftGrabPressed;

	// Token: 0x04001E95 RID: 7829
	public bool wasRightGrabPressed;

	// Token: 0x04001E96 RID: 7830
	public bool isLeftGrabbing;

	// Token: 0x04001E97 RID: 7831
	public bool isRightGrabbing;

	// Token: 0x04001E98 RID: 7832
	public bool justReleased;

	// Token: 0x04001E99 RID: 7833
	public bool justGrabbed;

	// Token: 0x04001E9A RID: 7834
	public bool disableLeftGrab;

	// Token: 0x04001E9B RID: 7835
	public bool disableRightGrab;

	// Token: 0x04001E9C RID: 7836
	public bool autoGrabLeft;

	// Token: 0x04001E9D RID: 7837
	public bool autoGrabRight;

	// Token: 0x04001E9E RID: 7838
	private float grabValue;

	// Token: 0x04001E9F RID: 7839
	private float tempValue;

	// Token: 0x04001EA0 RID: 7840
	private DropZone tempZone;

	// Token: 0x04001EA1 RID: 7841
	private bool iteratingInteractionPoints;

	// Token: 0x04001EA2 RID: 7842
	private List<InteractionPoint> interactionPointsToRemove = new List<InteractionPoint>();

	// Token: 0x04001EA3 RID: 7843
	[SerializeField]
	private GorillaHandClimber bodyClimber;

	// Token: 0x04001EA4 RID: 7844
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x04001EA5 RID: 7845
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
