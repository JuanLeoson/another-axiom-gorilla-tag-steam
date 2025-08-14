using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D60 RID: 3424
	public class FriendshipBracelet : MonoBehaviour
	{
		// Token: 0x060054FD RID: 21757 RVA: 0x001A5B42 File Offset: 0x001A3D42
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x060054FE RID: 21758 RVA: 0x001A5B50 File Offset: 0x001A3D50
		private AudioSource GetAudioSource()
		{
			if (!this.isLeftHand)
			{
				return this.ownerRig.rightHandPlayer;
			}
			return this.ownerRig.leftHandPlayer;
		}

		// Token: 0x060054FF RID: 21759 RVA: 0x001A5B71 File Offset: 0x001A3D71
		private void OnEnable()
		{
			this.PlayAppearEffects();
		}

		// Token: 0x06005500 RID: 21760 RVA: 0x001A5B79 File Offset: 0x001A3D79
		public void PlayAppearEffects()
		{
			this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound, 1f);
			if (this.braceletFormedParticle)
			{
				this.braceletFormedParticle.Play();
			}
		}

		// Token: 0x06005501 RID: 21761 RVA: 0x001A5BAC File Offset: 0x001A3DAC
		private void OnDisable()
		{
			if (!this.ownerRig.gameObject.activeInHierarchy)
			{
				return;
			}
			this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound, 1f);
			if (this.braceletBrokenParticle)
			{
				this.braceletBrokenParticle.Play();
			}
		}

		// Token: 0x06005502 RID: 21762 RVA: 0x001A5BFC File Offset: 0x001A3DFC
		public void UpdateBeads(List<Color> colors, int selfIndex)
		{
			int num = colors.Count - 1;
			int num2 = (this.braceletBeads.Length - num) / 2;
			for (int i = 0; i < this.braceletBeads.Length; i++)
			{
				int num3 = i - num2;
				if (num3 >= 0 && num3 < num)
				{
					this.braceletBeads[i].enabled = true;
					this.braceletBeads[i].material.color = colors[num3];
					this.braceletBananas[i].gameObject.SetActive(num3 == selfIndex);
				}
				else
				{
					this.braceletBeads[i].enabled = false;
					this.braceletBananas[i].gameObject.SetActive(false);
				}
			}
			SkinnedMeshRenderer[] array = this.braceletStrings;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].material.color = colors[colors.Count - 1];
			}
		}

		// Token: 0x04005E8F RID: 24207
		[SerializeField]
		private SkinnedMeshRenderer[] braceletStrings;

		// Token: 0x04005E90 RID: 24208
		[SerializeField]
		private MeshRenderer[] braceletBeads;

		// Token: 0x04005E91 RID: 24209
		[SerializeField]
		private MeshRenderer[] braceletBananas;

		// Token: 0x04005E92 RID: 24210
		[SerializeField]
		private bool isLeftHand;

		// Token: 0x04005E93 RID: 24211
		[SerializeField]
		private AudioClip braceletFormedSound;

		// Token: 0x04005E94 RID: 24212
		[SerializeField]
		private AudioClip braceletBrokenSound;

		// Token: 0x04005E95 RID: 24213
		[SerializeField]
		private ParticleSystem braceletFormedParticle;

		// Token: 0x04005E96 RID: 24214
		[SerializeField]
		private ParticleSystem braceletBrokenParticle;

		// Token: 0x04005E97 RID: 24215
		private VRRig ownerRig;
	}
}
