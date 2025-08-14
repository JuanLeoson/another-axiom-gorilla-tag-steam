using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000A50 RID: 2640
public static class FXSystem
{
	// Token: 0x06004087 RID: 16519 RVA: 0x00146A6C File Offset: 0x00144C6C
	public static void PlayFXForRig(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX();
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX();
		}
	}

	// Token: 0x06004088 RID: 16520 RVA: 0x00146AA8 File Offset: 0x00144CA8
	public static void PlayFXForRigValidated(List<int> hashes, FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		for (int i = 0; i < hashes.Count; i++)
		{
			if (!ObjectPools.instance.DoesPoolExist(hashes[i]))
			{
				return;
			}
		}
		FXSystem.PlayFXForRig(fxType, context, info);
	}

	// Token: 0x06004089 RID: 16521 RVA: 0x00146AE4 File Offset: 0x00144CE4
	public static void PlayFX<T>(FXType fxType, IFXContextParems<T> context, T args, PhotonMessageInfoWrapped info) where T : FXSArgs
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX(args);
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX(args);
		}
	}

	// Token: 0x0600408A RID: 16522 RVA: 0x00146B20 File Offset: 0x00144D20
	public static void PlayFXForRig<T>(FXType fxType, IFXEffectContext<T> context, PhotonMessageInfoWrapped info) where T : IFXEffectContextObject
	{
		FXSystemSettings settings = context.settings;
		if (!settings.forLocalRig && !FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			return;
		}
		FXSystem.PlayFX(context.effectContext);
	}

	// Token: 0x0600408B RID: 16523 RVA: 0x00146B60 File Offset: 0x00144D60
	public static void PlayFX(IFXEffectContextObject effectContext)
	{
		effectContext.OnTriggerActions();
		int[] prefabPoolIds = effectContext.PrefabPoolIds;
		if (prefabPoolIds != null)
		{
			int num = prefabPoolIds.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = prefabPoolIds[i];
				if (num2 != -1)
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(num2, effectContext.Positon, effectContext.Rotation, false);
					gameObject.SetActive(true);
					effectContext.OnPlayVisualFX(num2, gameObject);
				}
			}
		}
		AudioSource soundSource = effectContext.SoundSource;
		if (soundSource.IsNull())
		{
			return;
		}
		AudioClip sound = effectContext.Sound;
		if (sound.IsNotNull())
		{
			soundSource.volume = effectContext.Volume;
			soundSource.pitch = effectContext.Pitch;
			soundSource.GTPlayOneShot(sound, 1f);
			effectContext.OnPlaySoundFX(soundSource);
		}
	}

	// Token: 0x0600408C RID: 16524 RVA: 0x00146C14 File Offset: 0x00144E14
	public static bool CheckCallSpam(FXSystemSettings settings, int index, double serverTime)
	{
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[index];
		if (!callLimitType.UseNetWorkTime)
		{
			return callLimitType.CallLimitSettings.CheckCallTime(Time.time);
		}
		return callLimitType.CallLimitSettings.CheckCallServerTime(serverTime);
	}
}
