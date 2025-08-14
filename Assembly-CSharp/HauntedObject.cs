using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200074B RID: 1867
public class HauntedObject : MonoBehaviour
{
	// Token: 0x06002EC8 RID: 11976 RVA: 0x000F7E48 File Offset: 0x000F6048
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000F7F04 File Offset: 0x000F6104
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x000F7F8F File Offset: 0x000F618F
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x000F7FB8 File Offset: 0x000F61B8
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine("Shake");
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.GTPlayOneShot(this.hauntedSound, 1f);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position, true);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine("TurnOff");
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger("Haunted");
			}
		}
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x000F8088 File Offset: 0x000F6288
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000F8097 File Offset: 0x000F6297
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04003ABC RID: 15036
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04003ABD RID: 15037
	public float speed = 60f;

	// Token: 0x04003ABE RID: 15038
	public float amount = 0.01f;

	// Token: 0x04003ABF RID: 15039
	public float duration = 1f;

	// Token: 0x04003AC0 RID: 15040
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04003AC1 RID: 15041
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04003AC2 RID: 15042
	public float TurnOffDuration = 2f;

	// Token: 0x04003AC3 RID: 15043
	private Vector3 initialPos;

	// Token: 0x04003AC4 RID: 15044
	private float passedTime;

	// Token: 0x04003AC5 RID: 15045
	private float lightPassedTime;

	// Token: 0x04003AC6 RID: 15046
	private GameObject lurkerGhost;

	// Token: 0x04003AC7 RID: 15047
	private GameObject wanderingGhost;

	// Token: 0x04003AC8 RID: 15048
	private Animator[] animators;

	// Token: 0x04003AC9 RID: 15049
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003ACA RID: 15050
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
