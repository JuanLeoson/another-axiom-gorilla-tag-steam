using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000850 RID: 2128
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x06003588 RID: 13704 RVA: 0x00118A30 File Offset: 0x00116C30
	public void NotifyPlayerTeleporting(short teleportVFXIdx, AudioSource localPlayerTeleporterAudioSource)
	{
		if ((int)teleportVFXIdx >= this.teleporterVFX.Count)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				false,
				teleportVFXIdx
			});
		}
		this.ActivateTeleportVFXLocal(teleportVFXIdx, true);
		if (localPlayerTeleporterAudioSource.IsNotNull() && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
		{
			localPlayerTeleporterAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
			localPlayerTeleporterAudioSource.Play();
		}
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x00118ABC File Offset: 0x00116CBC
	public void NotifyPlayerReturning(short teleportVFXIdx)
	{
		if ((int)teleportVFXIdx >= this.returnVFX.Count)
		{
			return;
		}
		Debug.Log(string.Format("[VRTeleporterSerializer::NotifyPlayerReturning] Sending RPC to activate VFX at idx: {0}", teleportVFXIdx));
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				true,
				teleportVFXIdx
			});
		}
		this.ActivateReturnVFXLocal(teleportVFXIdx, true);
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x00118B20 File Offset: 0x00116D20
	[PunRPC]
	private void ActivateTeleportVFX(bool returning, short teleportVFXIdx, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ActivateTeleportVFX");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[13].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (returning)
		{
			this.ActivateReturnVFXLocal(teleportVFXIdx, false);
			return;
		}
		this.ActivateTeleportVFXLocal(teleportVFXIdx, false);
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x00118B94 File Offset: 0x00116D94
	private void ActivateTeleportVFXLocal(short teleportVFXIdx, bool isTeleporter = false)
	{
		if ((int)teleportVFXIdx >= this.teleporterVFX.Count)
		{
			return;
		}
		ParticleSystem particleSystem = this.teleporterVFX[(int)teleportVFXIdx];
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
		if (isTeleporter)
		{
			return;
		}
		AudioSource audioSource = this.teleportAudioSource[(int)teleportVFXIdx];
		if (audioSource.IsNotNull())
		{
			audioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
			audioSource.Play();
		}
	}

	// Token: 0x0600358C RID: 13708 RVA: 0x00118C0C File Offset: 0x00116E0C
	private void ActivateReturnVFXLocal(short teleportVFXIdx, bool isTeleporter = false)
	{
		if ((int)teleportVFXIdx >= this.returnVFX.Count)
		{
			return;
		}
		ParticleSystem particleSystem = this.returnVFX[(int)teleportVFXIdx];
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
		AudioSource audioSource = this.teleportAudioSource[(int)teleportVFXIdx];
		if (audioSource.IsNotNull())
		{
			audioSource.clip = (isTeleporter ? this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)] : this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)]);
			audioSource.Play();
		}
	}

	// Token: 0x0400426B RID: 17003
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x0400426C RID: 17004
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x0400426D RID: 17005
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x0400426E RID: 17006
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x0400426F RID: 17007
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
