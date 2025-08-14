using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class ElfLauncher : MonoBehaviour
{
	// Token: 0x06000A1F RID: 2591 RVA: 0x0003754C File Offset: 0x0003574C
	private void OnEnable()
	{
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (this.parentHoldable.myOnlineRig != null) ? this.parentHoldable.myOnlineRig.creator : ((this.parentHoldable.myRig != null) ? ((this.parentHoldable.myRig.creator != null) ? this.parentHoldable.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this.m_player = netPlayer;
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.ShootShared;
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00037638 File Offset: 0x00035838
	private void OnDisable()
	{
		if (this._events != null)
		{
			this._events.Activate -= this.ShootShared;
			this._events.Dispose();
			this._events = null;
			this.m_player = null;
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00037690 File Offset: 0x00035890
	private void Awake()
	{
		this._events = base.GetComponent<RubberDuckEvents>();
		this.elfProjectileHash = PoolUtils.GameObjHashCode(this.elfProjectilePrefab);
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCranked));
		}
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x000376E4 File Offset: 0x000358E4
	private void OnCranked(float deltaAngle)
	{
		this.currentShootCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentShootCrankAmount) > this.crankShootThreshold)
		{
			this.currentShootCrankAmount = 0f;
			this.Shoot();
		}
		this.currentClickCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentClickCrankAmount) > this.crankClickThreshold)
		{
			this.currentClickCrankAmount = 0f;
			this.crankClickAudio.Play();
		}
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0003775C File Offset: 0x0003595C
	private void Shoot()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			GorillaTagger.Instance.StartVibration(true, this.shootHapticStrength, this.shootHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.shootHapticStrength, this.shootHapticDuration);
			if (PhotonNetwork.InRoom)
			{
				this._events.Activate.RaiseAll(new object[]
				{
					this.muzzle.transform.position,
					this.muzzle.transform.forward
				});
				return;
			}
			this.ShootShared(this.muzzle.transform.position, this.muzzle.transform.forward);
		}
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0003781C File Offset: 0x00035A1C
	private void ShootShared(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (args.Length != 2)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		VRRig ownerRig = this.parentHoldable.ownerRig;
		if (info.senderID != ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length == 2)
		{
			object obj = args[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				obj = args[1];
				if (obj is Vector3)
				{
					Vector3 direction = (Vector3)obj;
					float num = 10000f;
					if (vector.IsValid(num))
					{
						float num2 = 10000f;
						if (direction.IsValid(num2))
						{
							if (!FXSystem.CheckCallSpam(ownerRig.fxSettings, 4, info.SentServerTime) || !ownerRig.IsPositionInRange(vector, 6f))
							{
								return;
							}
							this.ShootShared(vector, direction);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x000378D4 File Offset: 0x00035AD4
	protected virtual void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().velocity = direction * this.muzzleVelocity * lossyScale.x;
	}

	// Token: 0x04000C3B RID: 3131
	[SerializeField]
	protected TransferrableObject parentHoldable;

	// Token: 0x04000C3C RID: 3132
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x04000C3D RID: 3133
	[SerializeField]
	private float crankShootThreshold = 360f;

	// Token: 0x04000C3E RID: 3134
	[SerializeField]
	private float crankClickThreshold = 30f;

	// Token: 0x04000C3F RID: 3135
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04000C40 RID: 3136
	[SerializeField]
	private GameObject elfProjectilePrefab;

	// Token: 0x04000C41 RID: 3137
	protected int elfProjectileHash;

	// Token: 0x04000C42 RID: 3138
	[SerializeField]
	protected float muzzleVelocity = 10f;

	// Token: 0x04000C43 RID: 3139
	[SerializeField]
	private SoundBankPlayer crankClickAudio;

	// Token: 0x04000C44 RID: 3140
	[SerializeField]
	protected SoundBankPlayer shootAudio;

	// Token: 0x04000C45 RID: 3141
	[SerializeField]
	private float shootHapticStrength;

	// Token: 0x04000C46 RID: 3142
	[SerializeField]
	private float shootHapticDuration;

	// Token: 0x04000C47 RID: 3143
	private RubberDuckEvents _events;

	// Token: 0x04000C48 RID: 3144
	private float currentShootCrankAmount;

	// Token: 0x04000C49 RID: 3145
	private float currentClickCrankAmount;

	// Token: 0x04000C4A RID: 3146
	private NetPlayer m_player;
}
