using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200061A RID: 1562
public class GRDropZone : MonoBehaviour
{
	// Token: 0x06002640 RID: 9792 RVA: 0x000CC7FE File Offset: 0x000CA9FE
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000CC81C File Offset: 0x000CAA1C
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.manager.ghostReactorManager != null)
		{
			GhostReactorManager.Get(component).EntityEnteredDropZone(component);
		}
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000CC865 File Offset: 0x000CAA65
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000CC870 File Offset: 0x000CAA70
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x000CC8E9 File Offset: 0x000CAAE9
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x0400308C RID: 12428
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x0400308D RID: 12429
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x0400308E RID: 12430
	public float effectDuration = 1f;

	// Token: 0x0400308F RID: 12431
	private bool playingEffect;

	// Token: 0x04003090 RID: 12432
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x04003091 RID: 12433
	private Vector3 repelDirectionWorld = Vector3.up;
}
