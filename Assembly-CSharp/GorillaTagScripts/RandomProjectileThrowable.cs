using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTagScripts
{
	// Token: 0x02000C4E RID: 3150
	public class RandomProjectileThrowable : MonoBehaviour
	{
		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06004DEE RID: 19950 RVA: 0x001837FC File Offset: 0x001819FC
		// (set) Token: 0x06004DEF RID: 19951 RVA: 0x00183804 File Offset: 0x00181A04
		public float TimeEnabled { get; private set; }

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06004DF0 RID: 19952 RVA: 0x0018380D File Offset: 0x00181A0D
		// (set) Token: 0x06004DF1 RID: 19953 RVA: 0x00183815 File Offset: 0x00181A15
		public bool ForceDestroy { get; set; }

		// Token: 0x06004DF2 RID: 19954 RVA: 0x0018381E File Offset: 0x00181A1E
		private void OnEnable()
		{
			this.TimeEnabled = Time.time;
			this.currentProjectile = this.projectilePrefab;
		}

		// Token: 0x06004DF3 RID: 19955 RVA: 0x00183837 File Offset: 0x00181A37
		private void OnDisable()
		{
			this.ForceDestroy = false;
		}

		// Token: 0x06004DF4 RID: 19956 RVA: 0x00183840 File Offset: 0x00181A40
		public void ForceDestroyThrowable()
		{
			this.ForceDestroy = true;
		}

		// Token: 0x06004DF5 RID: 19957 RVA: 0x00183849 File Offset: 0x00181A49
		public void UpdateProjectilePrefab()
		{
			this.currentProjectile = this.alternativeProjectilePrefab;
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x00183857 File Offset: 0x00181A57
		public GameObject GetProjectilePrefab()
		{
			return this.currentProjectile;
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x00183860 File Offset: 0x00181A60
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Gorilla Head"))
			{
				if (this.audioSource && this.triggerClip)
				{
					this.audioSource.GTPlayOneShot(this.triggerClip, 1f);
				}
				base.Invoke("TriggerEvent", 0.25f);
			}
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x001838C4 File Offset: 0x00181AC4
		private void TriggerEvent()
		{
			UnityAction<bool> onTriggerEntered = this.OnTriggerEntered;
			if (onTriggerEntered == null)
			{
				return;
			}
			onTriggerEntered(false);
		}

		// Token: 0x040056EC RID: 22252
		public GameObject projectilePrefab;

		// Token: 0x040056ED RID: 22253
		[Tooltip("Use for a different/updated version of the projectile if needed.")]
		public GameObject alternativeProjectilePrefab;

		// Token: 0x040056EE RID: 22254
		[FormerlySerializedAs("weightedChance")]
		[Range(0f, 1f)]
		public float spawnChance = 1f;

		// Token: 0x040056EF RID: 22255
		public AudioSource audioSource;

		// Token: 0x040056F0 RID: 22256
		public AudioClip triggerClip;

		// Token: 0x040056F1 RID: 22257
		[Tooltip("Immediately destroys after the release")]
		public bool destroyAfterRelease;

		// Token: 0x040056F2 RID: 22258
		[Tooltip("Set a timer to destroy after X seconds is passed and the object is not thrown yet")]
		[FormerlySerializedAs("destroyAfterSeconds")]
		public float autoDestroyAfterSeconds = -1f;

		// Token: 0x040056F3 RID: 22259
		[Tooltip("If checked, any amount of passed time will be deducted from the lifetime of the slingshot projectile when thrownShould be less than or equal to lifetime of the slingshot projectile")]
		public bool moveOverPassedLifeTime;

		// Token: 0x040056F6 RID: 22262
		public UnityAction<bool> OnTriggerEntered;

		// Token: 0x040056F7 RID: 22263
		private GameObject currentProjectile;
	}
}
