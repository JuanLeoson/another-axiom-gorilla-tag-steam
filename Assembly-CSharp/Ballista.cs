using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Ballista : MonoBehaviourPun
{
	// Token: 0x06000581 RID: 1409 RVA: 0x00020135 File Offset: 0x0001E335
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00020148 File Offset: 0x0001E348
	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000583 RID: 1411 RVA: 0x0002015B File Offset: 0x0001E35B
	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0002017C File Offset: 0x0001E37C
	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0002022C File Offset: 0x0001E42C
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 b = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 b2 = playerBodyCenterPosition - b;
					Vector3 a = Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (a - b2);
					this.playerReadyToFire = (a.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist);
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float b3 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num2 = 0.25f / this.launchRampDistance;
			float num3 = Mathf.Max(num + num2, b3);
			float d = num3 * this.launchRampDistance;
			Vector3 a2 = this.launchDirection * d + this.launchStart.position;
			instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num3 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00020521 File Offset: 0x0001E721
	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00020550 File Offset: 0x0001E750
	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x000205CC File Offset: 0x0001E7CC
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00020600 File Offset: 0x0001E800
	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00020631 File Offset: 0x0001E831
	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0002063C File Offset: 0x0001E83C
	private void UpdatePredictionLine()
	{
		float d = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 a = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += a * d;
			a += Vector3.down * 9.8f * d;
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x000206D6 File Offset: 0x0001E8D6
	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000206EC File Offset: 0x0001E8EC
	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00020750 File Offset: 0x0001E950
	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = (this.currentSpeedIndex == 0);
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = (this.currentSpeedIndex == 1);
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = (this.currentSpeedIndex == 2);
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = (this.currentSpeedIndex == 3);
		this.speedThreeButton.UpdateColor();
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x000207D9 File Offset: 0x0001E9D9
	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	// Token: 0x04000680 RID: 1664
	public Animator animator;

	// Token: 0x04000681 RID: 1665
	public Transform launchStart;

	// Token: 0x04000682 RID: 1666
	public Transform launchEnd;

	// Token: 0x04000683 RID: 1667
	public Transform launchBone;

	// Token: 0x04000684 RID: 1668
	public float reloadDelay = 1f;

	// Token: 0x04000685 RID: 1669
	public float loadTime = 1.933f;

	// Token: 0x04000686 RID: 1670
	public float playerMagnetismStrength = 3f;

	// Token: 0x04000687 RID: 1671
	public float launchSpeed = 20f;

	// Token: 0x04000688 RID: 1672
	[Range(0f, 1f)]
	public float pitch;

	// Token: 0x04000689 RID: 1673
	private bool useSpeedOptions;

	// Token: 0x0400068A RID: 1674
	public float[] speedOptions = new float[]
	{
		10f,
		15f,
		20f,
		25f
	};

	// Token: 0x0400068B RID: 1675
	public int currentSpeedIndex;

	// Token: 0x0400068C RID: 1676
	public GorillaPressableButton speedZeroButton;

	// Token: 0x0400068D RID: 1677
	public GorillaPressableButton speedOneButton;

	// Token: 0x0400068E RID: 1678
	public GorillaPressableButton speedTwoButton;

	// Token: 0x0400068F RID: 1679
	public GorillaPressableButton speedThreeButton;

	// Token: 0x04000690 RID: 1680
	private bool debugDrawTrajectoryOnLaunch;

	// Token: 0x04000691 RID: 1681
	private int loadTriggerHash = Animator.StringToHash("Load");

	// Token: 0x04000692 RID: 1682
	private int fireTriggerHash = Animator.StringToHash("Fire");

	// Token: 0x04000693 RID: 1683
	private int pitchParamHash = Animator.StringToHash("Pitch");

	// Token: 0x04000694 RID: 1684
	private int idleStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000695 RID: 1685
	private int loadStateHash = Animator.StringToHash("Load");

	// Token: 0x04000696 RID: 1686
	private int fireStateHash = Animator.StringToHash("Fire");

	// Token: 0x04000697 RID: 1687
	private int prevStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000698 RID: 1688
	private float fireCompleteTime;

	// Token: 0x04000699 RID: 1689
	private float loadStartTime;

	// Token: 0x0400069A RID: 1690
	private bool playerInTrigger;

	// Token: 0x0400069B RID: 1691
	private bool playerReadyToFire;

	// Token: 0x0400069C RID: 1692
	private bool playerLaunched;

	// Token: 0x0400069D RID: 1693
	private float playerReadyToFireDist = 0.1f;

	// Token: 0x0400069E RID: 1694
	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	// Token: 0x0400069F RID: 1695
	private Vector3 launchDirection;

	// Token: 0x040006A0 RID: 1696
	private float launchRampDistance;

	// Token: 0x040006A1 RID: 1697
	private int collidingLayer;

	// Token: 0x040006A2 RID: 1698
	private int notCollidingLayer;

	// Token: 0x040006A3 RID: 1699
	private float playerPullInRate;

	// Token: 0x040006A4 RID: 1700
	private float appliedAnimatorPitch;

	// Token: 0x040006A5 RID: 1701
	private const int predictionLineSamples = 240;

	// Token: 0x040006A6 RID: 1702
	private Vector3[] predictionLinePoints = new Vector3[240];
}
