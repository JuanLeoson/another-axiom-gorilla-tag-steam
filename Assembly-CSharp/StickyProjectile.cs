using System;
using System.Collections;
using GorillaLocomotion.Swimming;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004A7 RID: 1191
public class StickyProjectile : MonoBehaviour, IProjectile
{
	// Token: 0x06001D66 RID: 7526 RVA: 0x0009D7EC File Offset: 0x0009B9EC
	private void Awake()
	{
		this.stickyPart.GetLocalPositionAndRotation(out this.stickyPartLocalPosition, out this.stickyPartLocalRotation);
		this.stickyPartLocalScale = this.stickyPart.localScale;
		this.rb = base.GetComponent<Rigidbody>();
		this.rbwi = base.GetComponent<RigidbodyWaterInteraction>();
		this.collider = base.GetComponent<Collider>();
		this.GetOrAddComponent(out this.events);
		this.playerMouthOffsetDirection = this.playerMouthOffset.normalized;
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0009D864 File Offset: 0x0009BA64
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		UnityEvent onLaunch = this.OnLaunch;
		if (onLaunch != null)
		{
			onLaunch.Invoke();
		}
		this.stickyPart.SetParent(base.transform, false);
		this.stickyPart.SetLocalPositionAndRotation(this.stickyPartLocalPosition, this.stickyPartLocalRotation);
		this.stickyPart.localScale = this.stickyPartLocalScale;
		base.transform.SetPositionAndRotation(startPosition, startRotation);
		base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		this.rb.isKinematic = false;
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.rb.velocity = velocity;
		this.rb.angularVelocity = Random.onUnitSphere * Random.Range(this.launchRandomSpinSpeedMinMax.x, this.launchRandomSpinSpeedMinMax.y);
		this.rbwi.enabled = true;
		this.collider.enabled = true;
		this.SetColor(Color.Lerp(Color.white, ownerRig.playerColor, this.playerColorInfluence));
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x0009D97C File Offset: 0x0009BB7C
	public void SetColor(Color color)
	{
		for (int i = 0; i < this.playerColorableRenderers.Length; i++)
		{
			this.playerColorableRenderers[i].material.color = color;
		}
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x0009D9B0 File Offset: 0x0009BBB0
	private void StickTo(Transform otherTransform, Vector3 position, Quaternion rotation)
	{
		this.stickyPart.parent = otherTransform;
		this.stickyPart.SetPositionAndRotation(position + rotation * this.stickyPartLocalPosition, rotation * this.stickyPartLocalRotation);
		this.rb.isKinematic = true;
		this.rbwi.enabled = false;
		this.collider.enabled = false;
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x0009DA18 File Offset: 0x0009BC18
	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.GetContact(0);
		if (this.alignToHitNormal)
		{
			this.StickTo(collision.transform, contact.point, Quaternion.LookRotation(contact.normal, Random.onUnitSphere));
		}
		else
		{
			this.StickTo(collision.transform, contact.point, base.transform.rotation);
		}
		UnityEvent onStickShared = this.OnStickShared;
		if (onStickShared != null)
		{
			onStickShared.Invoke();
		}
		UnityEvent onStickToSurface = this.OnStickToSurface;
		if (onStickToSurface == null)
		{
			return;
		}
		onStickToSurface.Invoke();
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x0009DA9C File Offset: 0x0009BC9C
	private void OnTriggerEnter(Collider other)
	{
		Vector3 vector = Time.fixedDeltaTime * 2f * this.rb.velocity;
		Vector3 vector2 = base.transform.position - vector;
		Vector3 vector3;
		Quaternion rotation;
		if (this.alignToHitNormal)
		{
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(vector2, vector / magnitude), out raycastHit, 2f * magnitude);
			vector3 = raycastHit.point;
			rotation = Quaternion.LookRotation(raycastHit.normal, Random.onUnitSphere);
		}
		else
		{
			vector3 = other.ClosestPoint(vector2);
			rotation = base.transform.rotation;
		}
		if (this.OnStickToPlayerMouth != null && other.name.Contains("Head"))
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent)
			{
				Transform transform = componentInParent.head.rigTarget.transform;
				Vector3 vector4 = transform.TransformPoint(this.playerMouthOffset);
				if ((vector3 - vector4).magnitude <= this.mouthCatchRadius * componentInParent.scaleFactor)
				{
					if (componentInParent.isOfflineVRRig)
					{
						this.StickTo(other.transform, transform.TransformPoint(this.positionWhenInMyMouth), transform.rotation);
						this.stickyPart.localScale *= this.scaleWhenInMyMouth;
					}
					else
					{
						this.StickTo(other.transform, vector4, transform.rotation);
					}
					UnityEvent onStickShared = this.OnStickShared;
					if (onStickShared != null)
					{
						onStickShared.Invoke();
					}
					this.OnStickToPlayerMouth.Invoke();
					if (this.OnEat != null)
					{
						base.StartCoroutine(this.EatDelayCoroutine());
					}
					return;
				}
				if (componentInParent.isOfflineVRRig)
				{
					this.stickyPart.localScale *= this.scaleWhenOnMyHead;
				}
			}
		}
		this.StickTo(other.transform, vector3, rotation);
		UnityEvent onStickShared2 = this.OnStickShared;
		if (onStickShared2 != null)
		{
			onStickShared2.Invoke();
		}
		UnityEvent onStickToSurface = this.OnStickToSurface;
		if (onStickToSurface == null)
		{
			return;
		}
		onStickToSurface.Invoke();
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x0009DC93 File Offset: 0x0009BE93
	private IEnumerator EatDelayCoroutine()
	{
		yield return new WaitForSeconds(this.eatDelay);
		this.OnEat.Invoke();
		yield break;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x0009DCA2 File Offset: 0x0009BEA2
	private void OnEnable()
	{
		this.stickyPart.gameObject.SetActive(true);
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x0009DCB5 File Offset: 0x0009BEB5
	private void OnDisable()
	{
		this.stickyPart.gameObject.SetActive(false);
	}

	// Token: 0x040025E0 RID: 9696
	[SerializeField]
	private Transform stickyPart;

	// Token: 0x040025E1 RID: 9697
	[SerializeField]
	private Renderer[] playerColorableRenderers;

	// Token: 0x040025E2 RID: 9698
	[SerializeField]
	[Range(0f, 1f)]
	private float playerColorInfluence = 1f;

	// Token: 0x040025E3 RID: 9699
	[SerializeField]
	private Vector2 launchRandomSpinSpeedMinMax = new Vector2(90f, 360f);

	// Token: 0x040025E4 RID: 9700
	[SerializeField]
	private bool alignToHitNormal = true;

	// Token: 0x040025E5 RID: 9701
	[Header("Mouth catch settings")]
	[SerializeField]
	private Vector3 playerMouthOffset = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x040025E6 RID: 9702
	[SerializeField]
	private float mouthCatchRadius = 0.15f;

	// Token: 0x040025E7 RID: 9703
	[SerializeField]
	private float scaleWhenOnMyHead = 0.7f;

	// Token: 0x040025E8 RID: 9704
	[SerializeField]
	private float scaleWhenInMyMouth = 1f;

	// Token: 0x040025E9 RID: 9705
	[SerializeField]
	private Vector3 positionWhenInMyMouth = new Vector3(0f, 0.05f, 0.2f);

	// Token: 0x040025EA RID: 9706
	[SerializeField]
	private float eatDelay = 0.5f;

	// Token: 0x040025EB RID: 9707
	[Header("Events")]
	public UnityEvent OnLaunch;

	// Token: 0x040025EC RID: 9708
	public UnityEvent OnStickShared;

	// Token: 0x040025ED RID: 9709
	public UnityEvent OnStickToSurface;

	// Token: 0x040025EE RID: 9710
	public UnityEvent OnStickToPlayerMouth;

	// Token: 0x040025EF RID: 9711
	public UnityEvent OnEat;

	// Token: 0x040025F0 RID: 9712
	private Vector3 stickyPartLocalPosition;

	// Token: 0x040025F1 RID: 9713
	private Quaternion stickyPartLocalRotation;

	// Token: 0x040025F2 RID: 9714
	private Vector3 stickyPartLocalScale;

	// Token: 0x040025F3 RID: 9715
	private Rigidbody rb;

	// Token: 0x040025F4 RID: 9716
	private RigidbodyWaterInteraction rbwi;

	// Token: 0x040025F5 RID: 9717
	private Collider collider;

	// Token: 0x040025F6 RID: 9718
	private RubberDuckEvents events;

	// Token: 0x040025F7 RID: 9719
	private Vector3 playerMouthOffsetDirection;
}
