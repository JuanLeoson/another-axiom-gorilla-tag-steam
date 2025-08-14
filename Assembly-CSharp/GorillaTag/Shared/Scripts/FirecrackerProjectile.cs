using System;
using System.Collections;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts
{
	// Token: 0x02000EC6 RID: 3782
	public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
	{
		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06005E4D RID: 24141 RVA: 0x001DB781 File Offset: 0x001D9981
		// (set) Token: 0x06005E4E RID: 24142 RVA: 0x001DB789 File Offset: 0x001D9989
		public bool TickRunning { get; set; }

		// Token: 0x06005E4F RID: 24143 RVA: 0x001DB792 File Offset: 0x001D9992
		public void Tick()
		{
			if (Time.time - this.timeCreated > this.forceBackToPoolAfterSec || Time.time - this.timeExploded > this.explosionTime)
			{
				UnityEvent<FirecrackerProjectile> onDetonationComplete = this.OnDetonationComplete;
				if (onDetonationComplete == null)
				{
					return;
				}
				onDetonationComplete.Invoke(this);
			}
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x001DB7D0 File Offset: 0x001D99D0
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.m_timer.Start();
			this.timeExploded = float.PositiveInfinity;
			this.timeCreated = float.PositiveInfinity;
			this.collisionEntered = false;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(true);
			}
			UnityEvent onEnableObject = this.OnEnableObject;
			if (onEnableObject == null)
			{
				return;
			}
			onEnableObject.Invoke();
		}

		// Token: 0x06005E51 RID: 24145 RVA: 0x001DB834 File Offset: 0x001D9A34
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.m_timer.Stop();
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x001DB847 File Offset: 0x001D9A47
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.m_timer.callback = new Action(this.Detonate);
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x001DB878 File Offset: 0x001D9A78
		private void Detonate()
		{
			this.m_timer.Stop();
			this.timeExploded = Time.time;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x06005E54 RID: 24148 RVA: 0x001DB8B0 File Offset: 0x001D9AB0
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rb.velocity = velocity;
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x001DB900 File Offset: 0x001D9B00
		private void OnCollisionEnter(Collision other)
		{
			if (this.collisionEntered)
			{
				return;
			}
			Vector3 point = other.contacts[0].point;
			Vector3 normal = other.contacts[0].normal;
			UnityEvent<FirecrackerProjectile, Vector3> onCollisionEntered = this.OnCollisionEntered;
			if (onCollisionEntered != null)
			{
				onCollisionEntered.Invoke(this, normal);
			}
			if (this.sizzleDuration > 0f)
			{
				base.StartCoroutine(this.Sizzle(point, normal));
			}
			else
			{
				UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
				if (onDetonationStart != null)
				{
					onDetonationStart.Invoke(this, point);
				}
				this.Detonate(point, normal);
			}
			this.collisionEntered = true;
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x001DB98D File Offset: 0x001D9B8D
		private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
		{
			if (this.audioSource && this.sizzleAudioClip != null)
			{
				this.audioSource.GTPlayOneShot(this.sizzleAudioClip, 1f);
			}
			yield return new WaitForSeconds(this.sizzleDuration);
			UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
			if (onDetonationStart != null)
			{
				onDetonationStart.Invoke(this, contactPoint);
			}
			this.Detonate(contactPoint, normal);
			yield break;
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x001DB9AC File Offset: 0x001D9BAC
		private void Detonate(Vector3 contactPoint, Vector3 normal)
		{
			this.timeExploded = Time.time;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer soundBankPlayer;
			if (gameObject.TryGetComponent<SoundBankPlayer>(out soundBankPlayer) && soundBankPlayer.soundBank)
			{
				soundBankPlayer.Play();
			}
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x0400681E RID: 26654
		[SerializeField]
		private GameObject explosionEffect;

		// Token: 0x0400681F RID: 26655
		[SerializeField]
		private float forceBackToPoolAfterSec = 20f;

		// Token: 0x04006820 RID: 26656
		[SerializeField]
		private float explosionTime = 5f;

		// Token: 0x04006821 RID: 26657
		[SerializeField]
		private GameObject disableWhenHit;

		// Token: 0x04006822 RID: 26658
		[SerializeField]
		private float sizzleDuration;

		// Token: 0x04006823 RID: 26659
		[SerializeField]
		private AudioClip sizzleAudioClip;

		// Token: 0x04006824 RID: 26660
		[Space]
		public UnityEvent OnEnableObject;

		// Token: 0x04006825 RID: 26661
		public UnityEvent<FirecrackerProjectile, Vector3> OnCollisionEntered;

		// Token: 0x04006826 RID: 26662
		public UnityEvent<FirecrackerProjectile, Vector3> OnDetonationStart;

		// Token: 0x04006827 RID: 26663
		public UnityEvent<FirecrackerProjectile> OnDetonationComplete;

		// Token: 0x04006828 RID: 26664
		private Rigidbody rb;

		// Token: 0x04006829 RID: 26665
		private float timeCreated = float.PositiveInfinity;

		// Token: 0x0400682A RID: 26666
		private float timeExploded = float.PositiveInfinity;

		// Token: 0x0400682B RID: 26667
		private AudioSource audioSource;

		// Token: 0x0400682C RID: 26668
		private TickSystemTimer m_timer = new TickSystemTimer(40f);

		// Token: 0x0400682D RID: 26669
		private bool collisionEntered;
	}
}
