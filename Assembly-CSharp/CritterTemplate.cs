using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000072 RID: 114
public class CritterTemplate : ScriptableObject
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060002CB RID: 715 RVA: 0x00011084 File Offset: 0x0000F284
	private string HapticsBlurb
	{
		get
		{
			float num = this.grabbedStruggleHaptics.GetPeakMagnitude() * this.grabbedStruggleHapticsStrength;
			float num2 = this.grabbedStruggleHaptics.GetRMSMagnitude() * this.grabbedStruggleHapticsStrength;
			return string.Format("Peak Strength: {0:0.##} Mean Strength: {1:0.##}", num, num2);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x000110D0 File Offset: 0x0000F2D0
	private void SetMaxStrength(float maxStrength = 1f)
	{
		float peakMagnitude = this.grabbedStruggleHaptics.GetPeakMagnitude();
		Debug.Log(string.Format("Clip {0} max strength: {1}", this.grabbedStruggleHaptics, peakMagnitude));
		if (peakMagnitude > 0f)
		{
			this.grabbedStruggleHapticsStrength = maxStrength / peakMagnitude;
		}
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00011118 File Offset: 0x0000F318
	private void SetMeanStrength(float meanStrength = 1f)
	{
		float rmsmagnitude = this.grabbedStruggleHaptics.GetRMSMagnitude();
		Debug.Log(string.Format("Clip {0} mean strength: {1}", this.grabbedStruggleHaptics, rmsmagnitude));
		if (meanStrength > 0f)
		{
			this.grabbedStruggleHapticsStrength = meanStrength / rmsmagnitude;
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0001115D File Offset: 0x0000F35D
	private void OnValidate()
	{
		this.modifiedValues.Clear();
		this.RegisterModifiedBehaviour();
		this.RegisterModifiedVisual();
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00011176 File Offset: 0x0000F376
	private void OnEnable()
	{
		this.OnValidate();
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00011180 File Offset: 0x0000F380
	private void RegisterModifiedBehaviour()
	{
		if (this.maxJumpVel != 0f)
		{
			this.modifiedValues.Add("maxJumpVel", this.maxJumpVel);
		}
		if (this.jumpCooldown != 0f)
		{
			this.modifiedValues.Add("jumpCooldown", this.jumpCooldown);
		}
		if (this.scaredJumpCooldown != 0f)
		{
			this.modifiedValues.Add("scaredJumpCooldown", this.scaredJumpCooldown);
		}
		if (this.jumpVariabilityTime != 0f)
		{
			this.modifiedValues.Add("jumpVariabilityTime", this.jumpVariabilityTime);
		}
		if (this.visionConeAngle != 0f)
		{
			this.modifiedValues.Add("visionConeAngle", this.visionConeAngle);
		}
		if (this.sensoryRange != 0f)
		{
			this.modifiedValues.Add("sensoryRange", this.sensoryRange);
		}
		if (this.maxHunger != 0f)
		{
			this.modifiedValues.Add("maxHunger", this.maxHunger);
		}
		if (this.hungryThreshold != 0f)
		{
			this.modifiedValues.Add("hungryThreshold", this.hungryThreshold);
		}
		if (this.satiatedThreshold != 0f)
		{
			this.modifiedValues.Add("satiatedThreshold", this.satiatedThreshold);
		}
		if (this.hungerLostPerSecond != 0f)
		{
			this.modifiedValues.Add("hungerLostPerSecond", this.hungerLostPerSecond);
		}
		if (this.hungerGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("hungerGainedPerSecond", this.hungerGainedPerSecond);
		}
		if (this.maxFear != 0f)
		{
			this.modifiedValues.Add("maxFear", this.maxFear);
		}
		if (this.scaredThreshold != 0f)
		{
			this.modifiedValues.Add("scaredThreshold", this.scaredThreshold);
		}
		if (this.calmThreshold != 0f)
		{
			this.modifiedValues.Add("calmThreshold", this.calmThreshold);
		}
		if (this.fearLostPerSecond != 0f)
		{
			this.modifiedValues.Add("fearLostPerSecond", this.fearLostPerSecond);
		}
		if (this.maxAttraction != 0f)
		{
			this.modifiedValues.Add("maxAttraction", this.maxAttraction);
		}
		if (this.attractedThreshold != 0f)
		{
			this.modifiedValues.Add("attractedThreshold", this.attractedThreshold);
		}
		if (this.unattractedThreshold != 0f)
		{
			this.modifiedValues.Add("unattractedThreshold", this.unattractedThreshold);
		}
		if (this.attractionLostPerSecond != 0f)
		{
			this.modifiedValues.Add("attractionLostPerSecond", this.attractionLostPerSecond);
		}
		if (this.maxSleepiness != 0f)
		{
			this.modifiedValues.Add("maxSleepiness", this.maxSleepiness);
		}
		if (this.tiredThreshold != 0f)
		{
			this.modifiedValues.Add("tiredThreshold", this.tiredThreshold);
		}
		if (this.awakeThreshold != 0f)
		{
			this.modifiedValues.Add("awakeThreshold", this.awakeThreshold);
		}
		if (this.sleepinessGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("sleepinessGainedPerSecond", this.sleepinessGainedPerSecond);
		}
		if (this.sleepinessLostPerSecond != 0f)
		{
			this.modifiedValues.Add("sleepinessLostPerSecond", this.sleepinessLostPerSecond);
		}
		if (this.maxStruggle != 0f)
		{
			this.modifiedValues.Add("maxStruggle", this.maxStruggle);
		}
		if (this.escapeThreshold != 0f)
		{
			this.modifiedValues.Add("escapeThreshold", this.escapeThreshold);
		}
		if (this.catchableThreshold != 0f)
		{
			this.modifiedValues.Add("catchableThreshold", this.catchableThreshold);
		}
		if (this.struggleGainedPerSecond != 0f)
		{
			this.modifiedValues.Add("struggleGainedPerSecond", this.struggleGainedPerSecond);
		}
		if (this.struggleLostPerSecond != 0f)
		{
			this.modifiedValues.Add("struggleLostPerSecond", this.struggleLostPerSecond);
		}
		if (this.afraidOfList != null)
		{
			this.modifiedValues.Add("afraidOfList", this.afraidOfList);
		}
		if (this.attractedToList != null)
		{
			this.modifiedValues.Add("attractedToList", this.attractedToList);
		}
		if (this.lifeTime != 0f)
		{
			this.modifiedValues.Add("lifeTime", this.lifeTime);
		}
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0001167C File Offset: 0x0000F87C
	private void RegisterModifiedVisual()
	{
		if (this.hatChance != 0f)
		{
			this.modifiedValues.Add("hatChance", this.hatChance);
		}
		if (this.hats != null && this.hats.Length != 0)
		{
			this.modifiedValues.Add("hats", this.hats);
		}
		if (this.minSize != 0f)
		{
			this.modifiedValues.Add("minSize", this.minSize);
		}
		if (this.maxSize != 0f)
		{
			this.modifiedValues.Add("maxSize", this.maxSize);
		}
		if (this.eatingStartFX != null)
		{
			this.modifiedValues.Add("eatingStartFX", this.eatingStartFX);
		}
		if (this.eatingOngoingFX != null)
		{
			this.modifiedValues.Add("eatingOngoingFX", this.eatingOngoingFX);
		}
		if (CrittersAnim.IsModified(this.eatingAnim))
		{
			this.modifiedValues.Add("eatingAnim", this.eatingAnim);
		}
		if (this.fearStartFX != null)
		{
			this.modifiedValues.Add("fearStartFX", this.fearStartFX);
		}
		if (this.fearOngoingFX != null)
		{
			this.modifiedValues.Add("fearOngoingFX", this.fearOngoingFX);
		}
		if (CrittersAnim.IsModified(this.fearAnim))
		{
			this.modifiedValues.Add("fearAnim", this.fearAnim);
		}
		if (this.attractionStartFX != null)
		{
			this.modifiedValues.Add("attractionStartFX", this.attractionStartFX);
		}
		if (this.attractionOngoingFX != null)
		{
			this.modifiedValues.Add("attractionOngoingFX", this.attractionOngoingFX);
		}
		if (CrittersAnim.IsModified(this.attractionAnim))
		{
			this.modifiedValues.Add("attractionAnim", this.attractionAnim);
		}
		if (this.sleepStartFX != null)
		{
			this.modifiedValues.Add("sleepStartFX", this.sleepStartFX);
		}
		if (this.sleepOngoingFX != null)
		{
			this.modifiedValues.Add("sleepOngoingFX", this.sleepOngoingFX);
		}
		if (CrittersAnim.IsModified(this.sleepAnim))
		{
			this.modifiedValues.Add("sleepAnim", this.sleepAnim);
		}
		if (this.grabbedStartFX != null)
		{
			this.modifiedValues.Add("grabbedStartFX", this.grabbedStartFX);
		}
		if (this.grabbedOngoingFX != null)
		{
			this.modifiedValues.Add("grabbedOngoingFX", this.grabbedOngoingFX);
		}
		if (this.grabbedStopFX != null)
		{
			this.modifiedValues.Add("grabbedStopFX", this.grabbedStopFX);
		}
		if (CrittersAnim.IsModified(this.grabbedAnim))
		{
			this.modifiedValues.Add("grabbedAnim", this.grabbedAnim);
		}
		if (this.hungryStartFX != null)
		{
			this.modifiedValues.Add("hungryStartFX", this.hungryStartFX);
		}
		if (this.hungryOngoingFX != null)
		{
			this.modifiedValues.Add("hungryOngoingFX", this.hungryOngoingFX);
		}
		if (CrittersAnim.IsModified(this.hungryAnim))
		{
			this.modifiedValues.Add("hungryAnim", this.hungryAnim);
		}
		if (this.despawningStartFX != null)
		{
			this.modifiedValues.Add("despawningStartFX", this.despawningStartFX);
		}
		if (this.despawningOngoingFX != null)
		{
			this.modifiedValues.Add("despawningOngoingFX", this.despawningOngoingFX);
		}
		if (CrittersAnim.IsModified(this.despawningAnim))
		{
			this.modifiedValues.Add("despawningAnim", this.despawningAnim);
		}
		if (this.spawningStartFX != null)
		{
			this.modifiedValues.Add("spawningStartFX", this.spawningStartFX);
		}
		if (this.spawningOngoingFX != null)
		{
			this.modifiedValues.Add("spawningOngoingFX", this.spawningOngoingFX);
		}
		if (CrittersAnim.IsModified(this.spawningAnim))
		{
			this.modifiedValues.Add("spawningAnim", this.spawningAnim);
		}
		if (this.capturedStartFX != null)
		{
			this.modifiedValues.Add("capturedStartFX", this.capturedStartFX);
		}
		if (this.capturedOngoingFX != null)
		{
			this.modifiedValues.Add("capturedOngoingFX", this.capturedOngoingFX);
		}
		if (CrittersAnim.IsModified(this.capturedAnim))
		{
			this.modifiedValues.Add("capturedAnim", this.capturedAnim);
		}
		if (this.stunnedStartFX != null)
		{
			this.modifiedValues.Add("stunnedStartFX", this.stunnedStartFX);
		}
		if (this.stunnedOngoingFX != null)
		{
			this.modifiedValues.Add("stunnedOngoingFX", this.stunnedOngoingFX);
		}
		if (CrittersAnim.IsModified(this.stunnedAnim))
		{
			this.modifiedValues.Add("stunnedAnim", this.stunnedAnim);
		}
		if (this.grabbedStruggleHaptics != null)
		{
			this.modifiedValues.Add("grabbedStruggleHaptics", this.grabbedStruggleHaptics);
		}
		if (this.grabbedStruggleHapticsStrength != 0f)
		{
			this.modifiedValues.Add("grabbedStruggleHapticsStrength", this.grabbedStruggleHapticsStrength);
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00011BC6 File Offset: 0x0000FDC6
	public bool IsValueModified(string valueName)
	{
		return this.modifiedValues.ContainsKey(valueName);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00011BD4 File Offset: 0x0000FDD4
	public T GetParentValue<T>(string valueName)
	{
		if (this.parent != null)
		{
			return this.parent.GetTemplateValue<T>(valueName);
		}
		return default(T);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00011C08 File Offset: 0x0000FE08
	public T GetTemplateValue<T>(string valueName)
	{
		object obj;
		if (this.modifiedValues.TryGetValue(valueName, out obj))
		{
			return (T)((object)obj);
		}
		if (this.parent != null)
		{
			return this.parent.GetTemplateValue<T>(valueName);
		}
		return default(T);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00011C50 File Offset: 0x0000FE50
	public void ApplyToCritter(CrittersPawn critter)
	{
		this.ApplyBehaviour(critter);
		this.ApplyBehaviourFX(critter);
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00011C60 File Offset: 0x0000FE60
	private void ApplyBehaviour(CrittersPawn critter)
	{
		critter.maxJumpVel = this.GetTemplateValue<float>("maxJumpVel");
		critter.jumpCooldown = this.GetTemplateValue<float>("jumpCooldown");
		critter.scaredJumpCooldown = this.GetTemplateValue<float>("scaredJumpCooldown");
		critter.jumpVariabilityTime = this.GetTemplateValue<float>("jumpVariabilityTime");
		critter.visionConeAngle = this.GetTemplateValue<float>("visionConeAngle");
		critter.sensoryRange = this.GetTemplateValue<float>("sensoryRange");
		critter.maxHunger = this.GetTemplateValue<float>("maxHunger");
		critter.hungryThreshold = this.GetTemplateValue<float>("hungryThreshold");
		critter.satiatedThreshold = this.GetTemplateValue<float>("satiatedThreshold");
		critter.hungerLostPerSecond = this.GetTemplateValue<float>("hungerLostPerSecond");
		critter.hungerGainedPerSecond = this.GetTemplateValue<float>("hungerGainedPerSecond");
		critter.maxFear = this.GetTemplateValue<float>("maxFear");
		critter.scaredThreshold = this.GetTemplateValue<float>("scaredThreshold");
		critter.calmThreshold = this.GetTemplateValue<float>("calmThreshold");
		critter.fearLostPerSecond = this.GetTemplateValue<float>("fearLostPerSecond");
		critter.maxAttraction = this.GetTemplateValue<float>("maxAttraction");
		critter.attractedThreshold = this.GetTemplateValue<float>("attractedThreshold");
		critter.unattractedThreshold = this.GetTemplateValue<float>("unattractedThreshold");
		critter.attractionLostPerSecond = this.GetTemplateValue<float>("attractionLostPerSecond");
		critter.maxSleepiness = this.GetTemplateValue<float>("maxSleepiness");
		critter.tiredThreshold = this.GetTemplateValue<float>("tiredThreshold");
		critter.awakeThreshold = this.GetTemplateValue<float>("awakeThreshold");
		critter.sleepinessGainedPerSecond = this.GetTemplateValue<float>("sleepinessGainedPerSecond");
		critter.sleepinessLostPerSecond = this.GetTemplateValue<float>("sleepinessLostPerSecond");
		critter.maxStruggle = this.GetTemplateValue<float>("maxStruggle");
		critter.escapeThreshold = this.GetTemplateValue<float>("escapeThreshold");
		critter.catchableThreshold = this.GetTemplateValue<float>("catchableThreshold");
		critter.struggleGainedPerSecond = this.GetTemplateValue<float>("struggleGainedPerSecond");
		critter.struggleLostPerSecond = this.GetTemplateValue<float>("struggleLostPerSecond");
		critter.lifeTime = (double)this.GetTemplateValue<float>("lifeTime");
		critter.attractedToList = this.GetTemplateValue<List<crittersAttractorStruct>>("attractedToList");
		critter.afraidOfList = this.GetTemplateValue<List<crittersAttractorStruct>>("afraidOfList");
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00011E90 File Offset: 0x00010090
	private void ApplyBehaviourFX(CrittersPawn critter)
	{
		critter.StartStateFX.Clear();
		critter.OngoingStateFX.Clear();
		critter.stateAnim.Clear();
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<GameObject>("eatingStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<GameObject>("eatingOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Eating, this.GetTemplateValue<CrittersAnim>("eatingAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<GameObject>("fearStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<GameObject>("fearOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Running, this.GetTemplateValue<CrittersAnim>("fearAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<GameObject>("attractionStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<GameObject>("attractionOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.AttractedTo, this.GetTemplateValue<CrittersAnim>("attractionAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<GameObject>("sleepStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<GameObject>("sleepOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Sleeping, this.GetTemplateValue<CrittersAnim>("sleepAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<GameObject>("grabbedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<GameObject>("grabbedOngoingFX"));
		critter.OnReleasedFX = this.GetTemplateValue<GameObject>("grabbedStopFX");
		critter.stateAnim.Add(CrittersPawn.CreatureState.Grabbed, this.GetTemplateValue<CrittersAnim>("grabbedAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<GameObject>("hungryStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<GameObject>("hungryOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.SeekingFood, this.GetTemplateValue<CrittersAnim>("hungryAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<GameObject>("despawningStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<GameObject>("despawningOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Despawning, this.GetTemplateValue<CrittersAnim>("despawningAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<GameObject>("spawningStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<GameObject>("spawningOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Spawning, this.GetTemplateValue<CrittersAnim>("spawningAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<GameObject>("capturedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<GameObject>("capturedOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Captured, this.GetTemplateValue<CrittersAnim>("capturedAnim"));
		critter.StartStateFX.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<GameObject>("stunnedStartFX"));
		critter.OngoingStateFX.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<GameObject>("stunnedOngoingFX"));
		critter.stateAnim.Add(CrittersPawn.CreatureState.Stunned, this.GetTemplateValue<CrittersAnim>("stunnedAnim"));
		critter.grabbedHaptics = this.GetTemplateValue<AudioClip>("grabbedStruggleHaptics");
		critter.grabbedHapticsStrength = this.GetTemplateValue<float>("grabbedStruggleHapticsStrength");
	}

	// Token: 0x0400033B RID: 827
	public CritterTemplate parent;

	// Token: 0x0400033C RID: 828
	[Space]
	[Header("Description")]
	public string temperament = "UNKNOWN";

	// Token: 0x0400033D RID: 829
	[Space]
	[Header("Behaviour")]
	[CritterTemplateParameter]
	public float maxJumpVel;

	// Token: 0x0400033E RID: 830
	[CritterTemplateParameter]
	public float jumpCooldown;

	// Token: 0x0400033F RID: 831
	[CritterTemplateParameter]
	public float scaredJumpCooldown;

	// Token: 0x04000340 RID: 832
	[CritterTemplateParameter]
	public float jumpVariabilityTime;

	// Token: 0x04000341 RID: 833
	[Space]
	[CritterTemplateParameter]
	public float visionConeAngle;

	// Token: 0x04000342 RID: 834
	[FormerlySerializedAs("visionConeHeight")]
	[CritterTemplateParameter]
	public float sensoryRange;

	// Token: 0x04000343 RID: 835
	[Space]
	[CritterTemplateParameter]
	public float maxHunger;

	// Token: 0x04000344 RID: 836
	[CritterTemplateParameter]
	public float hungryThreshold;

	// Token: 0x04000345 RID: 837
	[CritterTemplateParameter]
	public float satiatedThreshold;

	// Token: 0x04000346 RID: 838
	[CritterTemplateParameter]
	public float hungerLostPerSecond;

	// Token: 0x04000347 RID: 839
	[CritterTemplateParameter]
	public float hungerGainedPerSecond;

	// Token: 0x04000348 RID: 840
	[Space]
	[CritterTemplateParameter]
	public float maxFear;

	// Token: 0x04000349 RID: 841
	[CritterTemplateParameter]
	public float scaredThreshold;

	// Token: 0x0400034A RID: 842
	[CritterTemplateParameter]
	public float calmThreshold;

	// Token: 0x0400034B RID: 843
	[CritterTemplateParameter]
	public float fearLostPerSecond;

	// Token: 0x0400034C RID: 844
	[Space]
	[CritterTemplateParameter]
	public float maxAttraction;

	// Token: 0x0400034D RID: 845
	[CritterTemplateParameter]
	public float attractedThreshold;

	// Token: 0x0400034E RID: 846
	[CritterTemplateParameter]
	public float unattractedThreshold;

	// Token: 0x0400034F RID: 847
	[CritterTemplateParameter]
	public float attractionLostPerSecond;

	// Token: 0x04000350 RID: 848
	[Space]
	[CritterTemplateParameter]
	public float maxSleepiness;

	// Token: 0x04000351 RID: 849
	[CritterTemplateParameter]
	public float tiredThreshold;

	// Token: 0x04000352 RID: 850
	[CritterTemplateParameter]
	public float awakeThreshold;

	// Token: 0x04000353 RID: 851
	[CritterTemplateParameter]
	public float sleepinessGainedPerSecond;

	// Token: 0x04000354 RID: 852
	[CritterTemplateParameter]
	public float sleepinessLostPerSecond;

	// Token: 0x04000355 RID: 853
	[Space]
	[CritterTemplateParameter]
	public float struggleGainedPerSecond;

	// Token: 0x04000356 RID: 854
	[CritterTemplateParameter]
	public float maxStruggle;

	// Token: 0x04000357 RID: 855
	[CritterTemplateParameter]
	public float escapeThreshold;

	// Token: 0x04000358 RID: 856
	[CritterTemplateParameter]
	public float catchableThreshold;

	// Token: 0x04000359 RID: 857
	[CritterTemplateParameter]
	public float struggleLostPerSecond;

	// Token: 0x0400035A RID: 858
	[Space]
	[CritterTemplateParameter]
	public float lifeTime;

	// Token: 0x0400035B RID: 859
	[Space]
	public List<crittersAttractorStruct> attractedToList;

	// Token: 0x0400035C RID: 860
	public List<crittersAttractorStruct> afraidOfList;

	// Token: 0x0400035D RID: 861
	[Space]
	[Header("Visual")]
	[CritterTemplateParameter]
	public float minSize;

	// Token: 0x0400035E RID: 862
	[CritterTemplateParameter]
	public float maxSize;

	// Token: 0x0400035F RID: 863
	[CritterTemplateParameter]
	public float hatChance;

	// Token: 0x04000360 RID: 864
	public GameObject[] hats;

	// Token: 0x04000361 RID: 865
	[Space]
	[Header("Behaviour FX")]
	public GameObject eatingStartFX;

	// Token: 0x04000362 RID: 866
	public GameObject eatingOngoingFX;

	// Token: 0x04000363 RID: 867
	public CrittersAnim eatingAnim;

	// Token: 0x04000364 RID: 868
	public GameObject fearStartFX;

	// Token: 0x04000365 RID: 869
	public GameObject fearOngoingFX;

	// Token: 0x04000366 RID: 870
	public CrittersAnim fearAnim;

	// Token: 0x04000367 RID: 871
	public GameObject attractionStartFX;

	// Token: 0x04000368 RID: 872
	public GameObject attractionOngoingFX;

	// Token: 0x04000369 RID: 873
	public CrittersAnim attractionAnim;

	// Token: 0x0400036A RID: 874
	public GameObject sleepStartFX;

	// Token: 0x0400036B RID: 875
	public GameObject sleepOngoingFX;

	// Token: 0x0400036C RID: 876
	public CrittersAnim sleepAnim;

	// Token: 0x0400036D RID: 877
	public GameObject grabbedStartFX;

	// Token: 0x0400036E RID: 878
	public GameObject grabbedOngoingFX;

	// Token: 0x0400036F RID: 879
	public GameObject grabbedStopFX;

	// Token: 0x04000370 RID: 880
	public CrittersAnim grabbedAnim;

	// Token: 0x04000371 RID: 881
	public GameObject hungryStartFX;

	// Token: 0x04000372 RID: 882
	public GameObject hungryOngoingFX;

	// Token: 0x04000373 RID: 883
	public CrittersAnim hungryAnim;

	// Token: 0x04000374 RID: 884
	public GameObject spawningStartFX;

	// Token: 0x04000375 RID: 885
	public GameObject spawningOngoingFX;

	// Token: 0x04000376 RID: 886
	public CrittersAnim spawningAnim;

	// Token: 0x04000377 RID: 887
	public GameObject despawningStartFX;

	// Token: 0x04000378 RID: 888
	public GameObject despawningOngoingFX;

	// Token: 0x04000379 RID: 889
	public CrittersAnim despawningAnim;

	// Token: 0x0400037A RID: 890
	public GameObject capturedStartFX;

	// Token: 0x0400037B RID: 891
	public GameObject capturedOngoingFX;

	// Token: 0x0400037C RID: 892
	public CrittersAnim capturedAnim;

	// Token: 0x0400037D RID: 893
	public GameObject stunnedStartFX;

	// Token: 0x0400037E RID: 894
	public GameObject stunnedOngoingFX;

	// Token: 0x0400037F RID: 895
	public CrittersAnim stunnedAnim;

	// Token: 0x04000380 RID: 896
	public AudioClip grabbedStruggleHaptics;

	// Token: 0x04000381 RID: 897
	public float grabbedStruggleHapticsStrength;

	// Token: 0x04000382 RID: 898
	private Dictionary<string, object> modifiedValues = new Dictionary<string, object>();
}
