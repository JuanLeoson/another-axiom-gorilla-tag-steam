using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007BD RID: 1981
[DisallowMultipleComponent]
public class TappableGuardianIdol : Tappable
{
	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x060031AD RID: 12717 RVA: 0x00102ADF File Offset: 0x00100CDF
	// (set) Token: 0x060031AE RID: 12718 RVA: 0x00102AE7 File Offset: 0x00100CE7
	public bool isChangingPositions { get; private set; }

	// Token: 0x060031AF RID: 12719 RVA: 0x00102AF0 File Offset: 0x00100CF0
	protected override void OnEnable()
	{
		base.OnEnable();
		this._colliderBaseRadius = this.tapCollision.radius;
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x00102B09 File Offset: 0x00100D09
	protected override void OnDisable()
	{
		base.OnDisable();
		this.isChangingPositions = false;
		this._activationState = -1;
		this.isActivationReady = true;
		this.tapCollision.radius = this._colliderBaseRadius;
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x00102B37 File Offset: 0x00100D37
	public void OnZoneActiveStateChanged(bool zoneActive)
	{
		GTDev.Log<string>(string.Format("OnZoneActiveStateChanged({0}->{1})", this._zoneIsActive, zoneActive), this, null);
		this._zoneIsActive = zoneActive;
		this.idolVisualRoot.SetActive(this._zoneIsActive);
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x00102B74 File Offset: 0x00100D74
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (info.Sender.IsLocal)
		{
			this.zoneManager.SetScaleCenterPoint(base.transform);
		}
		if (!this.isChangingPositions)
		{
			if (!this.zoneManager.IsZoneValid())
			{
				return;
			}
			RigContainer rigContainer;
			if (PhotonNetwork.LocalPlayer.IsMasterClient && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (Vector3.Magnitude(rigContainer.Rig.transform.position - base.transform.position) > this.requiredTapDistance + Mathf.Epsilon)
				{
					return;
				}
				this.zoneManager.IdolWasTapped(info.Sender);
			}
			if (!this.zoneManager.IsPlayerGuardian(info.Sender))
			{
				this.tapFX.Play();
			}
		}
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x00102C40 File Offset: 0x00100E40
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position + new Vector3(0f, this.activeHeight, 0f);
		this.UpdateStageActivatedObjects();
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		base.StartCoroutine(this.<SetPosition>g__Unshrink|49_0());
	}

	// Token: 0x060031B4 RID: 12724 RVA: 0x00102CA2 File Offset: 0x00100EA2
	public void MovePositions(Vector3 finalPosition)
	{
		if (this.isChangingPositions)
		{
			return;
		}
		this.transitionPos = finalPosition + this.fallStartOffset;
		this.finalPos = finalPosition;
		base.StartCoroutine(this.TransitionToNextIdol());
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x00102CD4 File Offset: 0x00100ED4
	public void UpdateActivationProgress(float rawProgress, bool progressing)
	{
		this.isActivationReady = !progressing;
		if (rawProgress <= 0f && !progressing)
		{
			if (this._activationState >= 0)
			{
				if (this._activationRoutine != null)
				{
					base.StopCoroutine(this._activationRoutine);
					this._activationRoutine = null;
				}
				this.idolMeshRoot.transform.localScale = Vector3.one;
			}
			this._activationState = -1;
			this.UpdateStageActivatedObjects();
			this._audio.GTStop();
			return;
		}
		int num = (int)rawProgress;
		progressing &= (this._activationStageSounds.Length > num);
		if (this._activationState == num || !progressing)
		{
			return;
		}
		if (this._activationRoutine != null)
		{
			base.StopCoroutine(this._activationRoutine);
		}
		this._activationRoutine = base.StartCoroutine(this.ShowActivationEffect());
		this._activationState = num;
		this.UpdateStageActivatedObjects();
		TappableGuardianIdol.IdolActivationSound idolActivationSound = this._activationStageSounds[num];
		this._audio.GTPlayOneShot(idolActivationSound.activation, this._audio.volume);
		this._audio.clip = idolActivationSound.loop;
		this._audio.loop = true;
		this._audio.GTPlay();
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x00102DEB File Offset: 0x00100FEB
	public void StartLookingAround()
	{
		if (this._lookRoutine != null)
		{
			base.StopCoroutine(this._lookRoutine);
		}
		this._lookRoutine = base.StartCoroutine(this.DoLookingAround());
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x00102E13 File Offset: 0x00101013
	public void StopLookingAround()
	{
		if (this._lookRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this._lookRoutine);
		this._lookRoot.localRotation = Quaternion.identity;
		this._lookRoutine = null;
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x00102E41 File Offset: 0x00101041
	private IEnumerator DoLookingAround()
	{
		TappableGuardianIdol.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.nextLookTime = Time.time;
		CS$<>8__locals1._lookDirection = this._lookRoot.rotation;
		yield return null;
		for (;;)
		{
			if (Time.time >= CS$<>8__locals1.nextLookTime)
			{
				this.<DoLookingAround>g__PickLookTarget|54_0(ref CS$<>8__locals1);
			}
			this._lookRoot.rotation = Quaternion.Slerp(this._lookRoot.rotation, CS$<>8__locals1._lookDirection, Time.deltaTime * Mathf.Max(1f, (float)this._activationState * this._baseLookRate));
			yield return null;
		}
		yield break;
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x00102E50 File Offset: 0x00101050
	private void UpdateStageActivatedObjects()
	{
		foreach (TappableGuardianIdol.StageActivatedObject stageActivatedObject in this._stageActivatedObjects)
		{
			stageActivatedObject.UpdateActiveState(this._activationState);
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x00102E87 File Offset: 0x00101087
	private IEnumerator ShowActivationEffect()
	{
		float bulgeDuration = 1f;
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / bulgeDuration;
			float num = Mathf.Lerp(1f, this.bulgeScale, this.bulgeCurve.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		this._activationRoutine = null;
		yield break;
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x00102E96 File Offset: 0x00101096
	private IEnumerator TransitionToNextIdol()
	{
		this.isChangingPositions = true;
		this._audio.GTStop();
		if (this.knockbackOnTrigger)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		if (this.explodeFX)
		{
			ObjectPools.instance.Instantiate(this.explodeFX, base.transform.position, true);
		}
		this.UpdateActivationProgress(-1f, false);
		this.idolMeshRoot.SetActive(false);
		this.tapCollision.enabled = false;
		base.transform.position = this.transitionPos;
		yield return new WaitForSeconds(this.floatDuration);
		this.idolMeshRoot.SetActive(true);
		this.tapCollision.enabled = true;
		if (this.startFallFX)
		{
			ObjectPools.instance.Instantiate(this.startFallFX, this.transitionPos, true);
		}
		this._audio.GTPlayOneShot(this._descentSound, 1f);
		this.trailFX.Play();
		float fall = 0f;
		Vector3 startPos = this.transitionPos;
		Vector3 destinationPos = this.finalPos;
		while (fall < this.fallDuration)
		{
			fall += Time.deltaTime;
			base.transform.position = Vector3.Lerp(startPos, destinationPos, fall / this.fallDuration);
			yield return null;
		}
		base.transform.position = destinationPos;
		this.trailFX.Stop();
		if (this.landedFX)
		{
			ObjectPools.instance.Instantiate(this.landedFX, destinationPos, true);
		}
		if (this.knockbackOnLand)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		yield return new WaitForSeconds(this.inactiveDuration);
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		float activateLerp = 0f;
		startPos = this.finalPos;
		destinationPos = this.finalPos + new Vector3(0f, this.activeHeight, 0f);
		AnimationCurve animCurve = AnimationCurves.EaseInOutQuad;
		while (activateLerp < 1f)
		{
			activateLerp = Mathf.Clamp01(activateLerp + Time.deltaTime / this.activationDuration);
			base.transform.position = Vector3.Lerp(startPos, destinationPos, animCurve.Evaluate(activateLerp));
			yield return null;
		}
		if (this.activatedFX)
		{
			ObjectPools.instance.Instantiate(this.activatedFX, base.transform.position, true);
		}
		if (this.knockbackOnActivate)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		this.isChangingPositions = false;
		yield break;
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x00102EA5 File Offset: 0x001010A5
	private float EaseInOut(float input)
	{
		if (input >= 0.5f)
		{
			return 1f - Mathf.Pow(-2f * input + 2f, 3f) / 2f;
		}
		return 4f * input * input * input;
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x00102FDC File Offset: 0x001011DC
	[CompilerGenerated]
	private IEnumerator <SetPosition>g__Unshrink|49_0()
	{
		float lerpVal = 0f;
		float growDuration = 0.5f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / growDuration;
			float num = Mathf.Lerp(0f, 1f, AnimationCurves.EaseOutQuad.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x00102FEC File Offset: 0x001011EC
	[CompilerGenerated]
	private void <DoLookingAround>g__PickLookTarget|54_0(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		Transform transform = this.<DoLookingAround>g__GetClosestPlayerPosition|54_2(ref A_1);
		A_1._lookDirection = (transform ? Quaternion.LookRotation(transform.position - this._lookRoot.position) : Quaternion.Euler((float)Random.Range(-15, 15), this._lookRoot.rotation.eulerAngles.y + (float)Random.Range(-45, 45), 0f));
		this.<DoLookingAround>g__SetLookTime|54_1(ref A_1);
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x0010306A File Offset: 0x0010126A
	[CompilerGenerated]
	private void <DoLookingAround>g__SetLookTime|54_1(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		A_1.nextLookTime = Time.time + this._lookInterval / (float)this._activationState * 0.5f + Random.value;
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x00103094 File Offset: 0x00101294
	[CompilerGenerated]
	private Transform <DoLookingAround>g__GetClosestPlayerPosition|54_2(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		if (Random.value < this._randomLookChance)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform result = null;
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (!(vrrig == null))
			{
				bool flag = vrrig.OwningNetPlayer == this.zoneManager.CurrentGuardian;
				float num2 = Vector3.SqrMagnitude(vrrig.transform.position - position) * (float)(flag ? 100 : 1);
				if (num2 < num)
				{
					num = num2;
					result = vrrig.transform;
				}
			}
		}
		return result;
	}

	// Token: 0x04003D63 RID: 15715
	[SerializeField]
	private GorillaGuardianZoneManager zoneManager;

	// Token: 0x04003D64 RID: 15716
	[SerializeField]
	private float floatDuration = 2f;

	// Token: 0x04003D65 RID: 15717
	[SerializeField]
	private float fallDuration = 1.5f;

	// Token: 0x04003D66 RID: 15718
	[SerializeField]
	private float inactiveDuration = 2f;

	// Token: 0x04003D67 RID: 15719
	[SerializeField]
	private float activationDuration = 1f;

	// Token: 0x04003D68 RID: 15720
	[SerializeField]
	private float activeHeight = 1f;

	// Token: 0x04003D69 RID: 15721
	[SerializeField]
	private bool knockbackOnTrigger;

	// Token: 0x04003D6A RID: 15722
	[SerializeField]
	private bool knockbackOnLand = true;

	// Token: 0x04003D6B RID: 15723
	[SerializeField]
	private bool knockbackOnActivate;

	// Token: 0x04003D6C RID: 15724
	[SerializeField]
	private Vector3 fallStartOffset = new Vector3(3f, 20f, 3f);

	// Token: 0x04003D6D RID: 15725
	[SerializeField]
	private ParticleSystem trailFX;

	// Token: 0x04003D6E RID: 15726
	[SerializeField]
	private ParticleSystem tapFX;

	// Token: 0x04003D6F RID: 15727
	[SerializeField]
	private GameObject explodeFX;

	// Token: 0x04003D70 RID: 15728
	[SerializeField]
	private GameObject startFallFX;

	// Token: 0x04003D71 RID: 15729
	[SerializeField]
	private GameObject landedFX;

	// Token: 0x04003D72 RID: 15730
	[SerializeField]
	private GameObject activatedFX;

	// Token: 0x04003D73 RID: 15731
	[SerializeField]
	private SphereCollider tapCollision;

	// Token: 0x04003D74 RID: 15732
	[SerializeField]
	private GameObject idolVisualRoot;

	// Token: 0x04003D75 RID: 15733
	[SerializeField]
	private GameObject idolMeshRoot;

	// Token: 0x04003D76 RID: 15734
	[SerializeField]
	private AnimationCurve bulgeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04003D77 RID: 15735
	[SerializeField]
	private float bulgeScale = 1.1f;

	// Token: 0x04003D78 RID: 15736
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x04003D79 RID: 15737
	[SerializeField]
	private AudioClip[] _descentSound;

	// Token: 0x04003D7A RID: 15738
	[SerializeField]
	private AudioClip[] _activateSound;

	// Token: 0x04003D7B RID: 15739
	[SerializeField]
	private TappableGuardianIdol.IdolActivationSound[] _activationStageSounds;

	// Token: 0x04003D7C RID: 15740
	[SerializeField]
	private TappableGuardianIdol.StageActivatedObject[] _stageActivatedObjects;

	// Token: 0x04003D7D RID: 15741
	[Header("Look Around")]
	[SerializeField]
	private Transform _lookRoot;

	// Token: 0x04003D7E RID: 15742
	[SerializeField]
	private float _lookInterval = 10f;

	// Token: 0x04003D7F RID: 15743
	[SerializeField]
	private float _baseLookRate = 1f;

	// Token: 0x04003D80 RID: 15744
	[SerializeField]
	private float _randomLookChance = 0.25f;

	// Token: 0x04003D81 RID: 15745
	private Coroutine _lookRoutine;

	// Token: 0x04003D83 RID: 15747
	private Vector3 transitionPos;

	// Token: 0x04003D84 RID: 15748
	private Vector3 finalPos;

	// Token: 0x04003D85 RID: 15749
	private int _activationState;

	// Token: 0x04003D86 RID: 15750
	private Coroutine _activationRoutine;

	// Token: 0x04003D87 RID: 15751
	private float _colliderBaseRadius;

	// Token: 0x04003D88 RID: 15752
	private bool _zoneIsActive = true;

	// Token: 0x04003D89 RID: 15753
	public bool isActivationReady;

	// Token: 0x04003D8A RID: 15754
	private float requiredTapDistance = 3f;

	// Token: 0x020007BE RID: 1982
	[Serializable]
	public struct IdolActivationSound
	{
		// Token: 0x04003D8B RID: 15755
		public AudioClip activation;

		// Token: 0x04003D8C RID: 15756
		public AudioClip loop;
	}

	// Token: 0x020007BF RID: 1983
	[Serializable]
	public struct StageActivatedObject
	{
		// Token: 0x060031C2 RID: 12738 RVA: 0x00103160 File Offset: 0x00101360
		public void UpdateActiveState(int stage)
		{
			bool active = stage >= this.min && stage <= this.max;
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(active);
			}
		}

		// Token: 0x04003D8D RID: 15757
		public GameObject[] objects;

		// Token: 0x04003D8E RID: 15758
		public int min;

		// Token: 0x04003D8F RID: 15759
		public int max;
	}
}
