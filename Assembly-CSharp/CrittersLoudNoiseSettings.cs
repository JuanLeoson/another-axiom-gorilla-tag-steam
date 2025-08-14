using System;

// Token: 0x02000059 RID: 89
public class CrittersLoudNoiseSettings : CrittersActorSettings
{
	// Token: 0x060001B9 RID: 441 RVA: 0x0000AAB4 File Offset: 0x00008CB4
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)this.parentActor;
		crittersLoudNoise.soundVolume = this._soundVolume;
		crittersLoudNoise.soundDuration = this._soundDuration;
		crittersLoudNoise.soundEnabled = this._soundEnabled;
		crittersLoudNoise.disableWhenSoundDisabled = this._disableWhenSoundDisabled;
		crittersLoudNoise.volumeFearAttractionMultiplier = this._volumeFearAttractionMultiplier;
	}

	// Token: 0x04000200 RID: 512
	public float _soundVolume;

	// Token: 0x04000201 RID: 513
	public float _soundDuration;

	// Token: 0x04000202 RID: 514
	public bool _soundEnabled;

	// Token: 0x04000203 RID: 515
	public bool _disableWhenSoundDisabled;

	// Token: 0x04000204 RID: 516
	public float _volumeFearAttractionMultiplier = 1f;
}
