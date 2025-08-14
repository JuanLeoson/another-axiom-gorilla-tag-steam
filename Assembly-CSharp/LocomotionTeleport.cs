using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200031C RID: 796
public class LocomotionTeleport : MonoBehaviour
{
	// Token: 0x06001305 RID: 4869 RVA: 0x0006822F File Offset: 0x0006642F
	public void EnableMovement(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableMovementDuringReady = ready;
		this.EnableMovementDuringAim = aim;
		this.EnableMovementDuringPreTeleport = pre;
		this.EnableMovementDuringPostTeleport = post;
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0006824E File Offset: 0x0006644E
	public void EnableRotation(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableRotationDuringReady = ready;
		this.EnableRotationDuringAim = aim;
		this.EnableRotationDuringPreTeleport = pre;
		this.EnableRotationDuringPostTeleport = post;
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06001307 RID: 4871 RVA: 0x0006826D File Offset: 0x0006646D
	// (set) Token: 0x06001308 RID: 4872 RVA: 0x00068275 File Offset: 0x00066475
	public LocomotionTeleport.States CurrentState { get; private set; }

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06001309 RID: 4873 RVA: 0x00068280 File Offset: 0x00066480
	// (remove) Token: 0x0600130A RID: 4874 RVA: 0x000682B8 File Offset: 0x000664B8
	public event Action<bool, Vector3?, Quaternion?, Quaternion?> UpdateTeleportDestination;

	// Token: 0x0600130B RID: 4875 RVA: 0x000682ED File Offset: 0x000664ED
	public void OnUpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		if (this.UpdateTeleportDestination != null)
		{
			this.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x0600130C RID: 4876 RVA: 0x00068307 File Offset: 0x00066507
	public Quaternion DestinationRotation
	{
		get
		{
			return this._teleportDestination.OrientationIndicator.rotation;
		}
	}

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x0600130D RID: 4877 RVA: 0x00068319 File Offset: 0x00066519
	// (set) Token: 0x0600130E RID: 4878 RVA: 0x00068321 File Offset: 0x00066521
	public LocomotionController LocomotionController { get; private set; }

	// Token: 0x0600130F RID: 4879 RVA: 0x0006832C File Offset: 0x0006652C
	public bool AimCollisionTest(Vector3 start, Vector3 end, LayerMask aimCollisionLayerMask, out RaycastHit hitInfo)
	{
		Vector3 a = end - start;
		float magnitude = a.magnitude;
		Vector3 direction = a / magnitude;
		switch (this.AimCollisionType)
		{
		case LocomotionTeleport.AimCollisionTypes.Point:
			return Physics.Raycast(start, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		case LocomotionTeleport.AimCollisionTypes.Sphere:
		{
			float radius;
			if (this.UseCharacterCollisionData)
			{
				radius = this.LocomotionController.CharacterController.radius;
			}
			else
			{
				radius = this.AimCollisionRadius;
			}
			return Physics.SphereCast(start, radius, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		case LocomotionTeleport.AimCollisionTypes.Capsule:
		{
			float num;
			float num2;
			if (this.UseCharacterCollisionData)
			{
				CapsuleCollider characterController = this.LocomotionController.CharacterController;
				num = characterController.height;
				num2 = characterController.radius;
			}
			else
			{
				num = this.AimCollisionHeight;
				num2 = this.AimCollisionRadius;
			}
			return Physics.CapsuleCast(start + new Vector3(0f, num2, 0f), start + new Vector3(0f, num + num2, 0f), num2, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		default:
			throw new Exception();
		}
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00068438 File Offset: 0x00066638
	[Conditional("DEBUG_TELEPORT_STATES")]
	protected void LogState(string msg)
	{
		Debug.Log(Time.frameCount.ToString() + ": " + msg);
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00068464 File Offset: 0x00066664
	protected void CreateNewTeleportDestination()
	{
		this.TeleportDestinationPrefab.gameObject.SetActive(false);
		TeleportDestination teleportDestination = Object.Instantiate<TeleportDestination>(this.TeleportDestinationPrefab);
		teleportDestination.LocomotionTeleport = this;
		teleportDestination.gameObject.layer = this.TeleportDestinationLayer;
		this._teleportDestination = teleportDestination;
		this._teleportDestination.LocomotionTeleport = this;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x000684B9 File Offset: 0x000666B9
	private void DeactivateDestination()
	{
		this._teleportDestination.OnDeactivated();
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x000684C6 File Offset: 0x000666C6
	public void RecycleTeleportDestination(TeleportDestination oldDestination)
	{
		if (oldDestination == this._teleportDestination)
		{
			this.CreateNewTeleportDestination();
		}
		Object.Destroy(oldDestination.gameObject);
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x000684E7 File Offset: 0x000666E7
	private void EnableMotion(bool enableLinear, bool enableRotation)
	{
		this.LocomotionController.PlayerController.EnableLinearMovement = enableLinear;
		this.LocomotionController.PlayerController.EnableRotation = enableRotation;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x0006850B File Offset: 0x0006670B
	private void Awake()
	{
		this.LocomotionController = base.GetComponent<LocomotionController>();
		this.CreateNewTeleportDestination();
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0006851F File Offset: 0x0006671F
	public virtual void OnEnable()
	{
		this.CurrentState = LocomotionTeleport.States.Ready;
		base.StartCoroutine(this.ReadyStateCoroutine());
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00004F01 File Offset: 0x00003101
	public virtual void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06001318 RID: 4888 RVA: 0x00068538 File Offset: 0x00066738
	// (remove) Token: 0x06001319 RID: 4889 RVA: 0x00068570 File Offset: 0x00066770
	public event Action EnterStateReady;

	// Token: 0x0600131A RID: 4890 RVA: 0x000685A5 File Offset: 0x000667A5
	protected IEnumerator ReadyStateCoroutine()
	{
		yield return null;
		this.CurrentState = LocomotionTeleport.States.Ready;
		this.EnableMotion(this.EnableMovementDuringReady, this.EnableRotationDuringReady);
		if (this.EnterStateReady != null)
		{
			this.EnterStateReady();
		}
		while (this.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.AimStateCoroutine());
		yield break;
	}

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x0600131B RID: 4891 RVA: 0x000685B4 File Offset: 0x000667B4
	// (remove) Token: 0x0600131C RID: 4892 RVA: 0x000685EC File Offset: 0x000667EC
	public event Action EnterStateAim;

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x0600131D RID: 4893 RVA: 0x00068624 File Offset: 0x00066824
	// (remove) Token: 0x0600131E RID: 4894 RVA: 0x0006865C File Offset: 0x0006685C
	public event Action<LocomotionTeleport.AimData> UpdateAimData;

	// Token: 0x0600131F RID: 4895 RVA: 0x00068691 File Offset: 0x00066891
	public void OnUpdateAimData(LocomotionTeleport.AimData aimData)
	{
		if (this.UpdateAimData != null)
		{
			this.UpdateAimData(aimData);
		}
	}

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06001320 RID: 4896 RVA: 0x000686A8 File Offset: 0x000668A8
	// (remove) Token: 0x06001321 RID: 4897 RVA: 0x000686E0 File Offset: 0x000668E0
	public event Action ExitStateAim;

	// Token: 0x06001322 RID: 4898 RVA: 0x00068715 File Offset: 0x00066915
	protected IEnumerator AimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Aim;
		this.EnableMotion(this.EnableMovementDuringAim, this.EnableRotationDuringAim);
		if (this.EnterStateAim != null)
		{
			this.EnterStateAim();
		}
		this._teleportDestination.gameObject.SetActive(true);
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		if (this.ExitStateAim != null)
		{
			this.ExitStateAim();
		}
		yield return null;
		if ((this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Teleport) && this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.PreTeleportStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelAimStateCoroutine());
		}
		yield break;
	}

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06001323 RID: 4899 RVA: 0x00068724 File Offset: 0x00066924
	// (remove) Token: 0x06001324 RID: 4900 RVA: 0x0006875C File Offset: 0x0006695C
	public event Action EnterStateCancelAim;

	// Token: 0x06001325 RID: 4901 RVA: 0x00068791 File Offset: 0x00066991
	protected IEnumerator CancelAimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelAim;
		if (this.EnterStateCancelAim != null)
		{
			this.EnterStateCancelAim();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06001326 RID: 4902 RVA: 0x000687A0 File Offset: 0x000669A0
	// (remove) Token: 0x06001327 RID: 4903 RVA: 0x000687D8 File Offset: 0x000669D8
	public event Action EnterStatePreTeleport;

	// Token: 0x06001328 RID: 4904 RVA: 0x0006880D File Offset: 0x00066A0D
	protected IEnumerator PreTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PreTeleport;
		this.EnableMotion(this.EnableMovementDuringPreTeleport, this.EnableRotationDuringPreTeleport);
		if (this.EnterStatePreTeleport != null)
		{
			this.EnterStatePreTeleport();
		}
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.IsPreTeleportRequested)
		{
			yield return null;
		}
		if (this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.TeleportingStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelTeleportStateCoroutine());
		}
		yield break;
	}

	// Token: 0x14000033 RID: 51
	// (add) Token: 0x06001329 RID: 4905 RVA: 0x0006881C File Offset: 0x00066A1C
	// (remove) Token: 0x0600132A RID: 4906 RVA: 0x00068854 File Offset: 0x00066A54
	public event Action EnterStateCancelTeleport;

	// Token: 0x0600132B RID: 4907 RVA: 0x00068889 File Offset: 0x00066A89
	protected IEnumerator CancelTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelTeleport;
		if (this.EnterStateCancelTeleport != null)
		{
			this.EnterStateCancelTeleport();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x0600132C RID: 4908 RVA: 0x00068898 File Offset: 0x00066A98
	// (remove) Token: 0x0600132D RID: 4909 RVA: 0x000688D0 File Offset: 0x00066AD0
	public event Action EnterStateTeleporting;

	// Token: 0x0600132E RID: 4910 RVA: 0x00068905 File Offset: 0x00066B05
	protected IEnumerator TeleportingStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Teleporting;
		this.EnableMotion(false, false);
		if (this.EnterStateTeleporting != null)
		{
			this.EnterStateTeleporting();
		}
		while (this.IsTransitioning)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.PostTeleportStateCoroutine());
		yield break;
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x0600132F RID: 4911 RVA: 0x00068914 File Offset: 0x00066B14
	// (remove) Token: 0x06001330 RID: 4912 RVA: 0x0006894C File Offset: 0x00066B4C
	public event Action EnterStatePostTeleport;

	// Token: 0x06001331 RID: 4913 RVA: 0x00068981 File Offset: 0x00066B81
	protected IEnumerator PostTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PostTeleport;
		this.EnableMotion(this.EnableMovementDuringPostTeleport, this.EnableRotationDuringPostTeleport);
		if (this.EnterStatePostTeleport != null)
		{
			this.EnterStatePostTeleport();
		}
		while (this.IsPostTeleportRequested)
		{
			yield return null;
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06001332 RID: 4914 RVA: 0x00068990 File Offset: 0x00066B90
	// (remove) Token: 0x06001333 RID: 4915 RVA: 0x000689C8 File Offset: 0x00066BC8
	public event Action<Transform, Vector3, Quaternion> Teleported;

	// Token: 0x06001334 RID: 4916 RVA: 0x00068A00 File Offset: 0x00066C00
	public void DoTeleport()
	{
		CapsuleCollider characterController = this.LocomotionController.CharacterController;
		Transform transform = characterController.transform;
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += characterController.height * 0.5f;
		Quaternion landingRotation = this._teleportDestination.LandingRotation;
		if (this.Teleported != null)
		{
			this.Teleported(transform, position, landingRotation);
		}
		transform.position = position;
		transform.rotation = landingRotation;
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x00068A78 File Offset: 0x00066C78
	public Vector3 GetCharacterPosition()
	{
		return this.LocomotionController.CharacterController.transform.position;
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x00068A90 File Offset: 0x00066C90
	public Quaternion GetHeadRotationY()
	{
		Quaternion result = Quaternion.identity;
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out result);
		}
		Vector3 eulerAngles = result.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		result = Quaternion.Euler(eulerAngles);
		return result;
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x00068AEC File Offset: 0x00066CEC
	public void DoWarp(Vector3 startPos, float positionPercent)
	{
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += this.LocomotionController.CharacterController.height / 2f;
		Transform transform = this.LocomotionController.CharacterController.transform;
		Vector3 position2 = Vector3.Lerp(startPos, position, positionPercent);
		transform.position = position2;
	}

	// Token: 0x04001AA1 RID: 6817
	[Tooltip("Allow linear movement prior to the teleport system being activated.")]
	public bool EnableMovementDuringReady = true;

	// Token: 0x04001AA2 RID: 6818
	[Tooltip("Allow linear movement while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableMovementDuringAim = true;

	// Token: 0x04001AA3 RID: 6819
	[Tooltip("Allow linear movement while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableMovementDuringPreTeleport = true;

	// Token: 0x04001AA4 RID: 6820
	[Tooltip("Allow linear movement after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableMovementDuringPostTeleport = true;

	// Token: 0x04001AA5 RID: 6821
	[Tooltip("Allow rotation prior to the teleport system being activated.")]
	public bool EnableRotationDuringReady = true;

	// Token: 0x04001AA6 RID: 6822
	[Tooltip("Allow rotation while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableRotationDuringAim = true;

	// Token: 0x04001AA7 RID: 6823
	[Tooltip("Allow rotation while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableRotationDuringPreTeleport = true;

	// Token: 0x04001AA8 RID: 6824
	[Tooltip("Allow rotation after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableRotationDuringPostTeleport = true;

	// Token: 0x04001AAA RID: 6826
	[NonSerialized]
	public TeleportAimHandler AimHandler;

	// Token: 0x04001AAB RID: 6827
	[Tooltip("This prefab will be instantiated as needed and updated to match the current aim target.")]
	public TeleportDestination TeleportDestinationPrefab;

	// Token: 0x04001AAC RID: 6828
	[Tooltip("TeleportDestinationPrefab will be instantiated into this layer.")]
	public int TeleportDestinationLayer;

	// Token: 0x04001AAE RID: 6830
	[NonSerialized]
	public TeleportInputHandler InputHandler;

	// Token: 0x04001AAF RID: 6831
	[NonSerialized]
	public LocomotionTeleport.TeleportIntentions CurrentIntention;

	// Token: 0x04001AB0 RID: 6832
	[NonSerialized]
	public bool IsPreTeleportRequested;

	// Token: 0x04001AB1 RID: 6833
	[NonSerialized]
	public bool IsTransitioning;

	// Token: 0x04001AB2 RID: 6834
	[NonSerialized]
	public bool IsPostTeleportRequested;

	// Token: 0x04001AB3 RID: 6835
	private TeleportDestination _teleportDestination;

	// Token: 0x04001AB5 RID: 6837
	[Tooltip("When aiming at possible destinations, the aim collision type determines which shape to use for collision tests.")]
	public LocomotionTeleport.AimCollisionTypes AimCollisionType;

	// Token: 0x04001AB6 RID: 6838
	[Tooltip("Use the character collision radius/height/skinwidth for sphere/capsule collision tests.")]
	public bool UseCharacterCollisionData;

	// Token: 0x04001AB7 RID: 6839
	[Tooltip("Radius of the sphere or capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionRadius;

	// Token: 0x04001AB8 RID: 6840
	[Tooltip("Height of the capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionHeight;

	// Token: 0x0200031D RID: 797
	public enum States
	{
		// Token: 0x04001AC4 RID: 6852
		Ready,
		// Token: 0x04001AC5 RID: 6853
		Aim,
		// Token: 0x04001AC6 RID: 6854
		CancelAim,
		// Token: 0x04001AC7 RID: 6855
		PreTeleport,
		// Token: 0x04001AC8 RID: 6856
		CancelTeleport,
		// Token: 0x04001AC9 RID: 6857
		Teleporting,
		// Token: 0x04001ACA RID: 6858
		PostTeleport
	}

	// Token: 0x0200031E RID: 798
	public enum TeleportIntentions
	{
		// Token: 0x04001ACC RID: 6860
		None,
		// Token: 0x04001ACD RID: 6861
		Aim,
		// Token: 0x04001ACE RID: 6862
		PreTeleport,
		// Token: 0x04001ACF RID: 6863
		Teleport
	}

	// Token: 0x0200031F RID: 799
	public enum AimCollisionTypes
	{
		// Token: 0x04001AD1 RID: 6865
		Point,
		// Token: 0x04001AD2 RID: 6866
		Sphere,
		// Token: 0x04001AD3 RID: 6867
		Capsule
	}

	// Token: 0x02000320 RID: 800
	public class AimData
	{
		// Token: 0x06001339 RID: 4921 RVA: 0x00068B8A File Offset: 0x00066D8A
		public AimData()
		{
			this.Points = new List<Vector3>();
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x00068B9D File Offset: 0x00066D9D
		// (set) Token: 0x0600133B RID: 4923 RVA: 0x00068BA5 File Offset: 0x00066DA5
		public List<Vector3> Points { get; private set; }

		// Token: 0x0600133C RID: 4924 RVA: 0x00068BAE File Offset: 0x00066DAE
		public void Reset()
		{
			this.Points.Clear();
			this.TargetValid = false;
			this.Destination = null;
		}

		// Token: 0x04001AD4 RID: 6868
		public RaycastHit TargetHitInfo;

		// Token: 0x04001AD5 RID: 6869
		public bool TargetValid;

		// Token: 0x04001AD6 RID: 6870
		public Vector3? Destination;

		// Token: 0x04001AD7 RID: 6871
		public float Radius;
	}
}
