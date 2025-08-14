using System;
using UnityEngine;

// Token: 0x02000665 RID: 1637
public class GRSconce : MonoBehaviour
{
	// Token: 0x06002820 RID: 10272 RVA: 0x000D8838 File Offset: 0x000D6A38
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange += this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged += this.OnStateChange;
		}
		this.state = GRSconce.State.Off;
		this.StopLight();
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000D889C File Offset: 0x000D6A9C
	private bool IsAuthority()
	{
		return this.gameEntity.IsAuthority();
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000D88AC File Offset: 0x000D6AAC
	private void SetState(GRSconce.State newState)
	{
		this.state = newState;
		GRSconce.State state = this.state;
		if (state != GRSconce.State.Off)
		{
			if (state == GRSconce.State.On)
			{
				this.StartLight();
			}
		}
		else
		{
			this.StopLight();
		}
		if (this.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000D8900 File Offset: 0x000D6B00
	private void StartLight()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.volume = this.lightOnSoundVolume;
		this.audioSource.clip = this.lightOnSound;
		this.audioSource.Play();
		this.meshRenderer.material = this.onMaterial;
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000D895C File Offset: 0x000D6B5C
	private void StopLight()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000D8980 File Offset: 0x000D6B80
	private void OnEnergyChange(GRTool tool, int energy, GameEntityId chargingEntityId)
	{
		if (this.IsAuthority() && this.state == GRSconce.State.Off && tool.IsEnergyFull())
		{
			this.SetState(GRSconce.State.On);
		}
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000D89A4 File Offset: 0x000D6BA4
	private void OnStateChange(long prevState, long nextState)
	{
		if (!this.IsAuthority())
		{
			GRSconce.State state = (GRSconce.State)nextState;
			this.SetState(state);
		}
	}

	// Token: 0x04003393 RID: 13203
	public GameEntity gameEntity;

	// Token: 0x04003394 RID: 13204
	public GameLight gameLight;

	// Token: 0x04003395 RID: 13205
	public GRTool tool;

	// Token: 0x04003396 RID: 13206
	public MeshRenderer meshRenderer;

	// Token: 0x04003397 RID: 13207
	public Material offMaterial;

	// Token: 0x04003398 RID: 13208
	public Material onMaterial;

	// Token: 0x04003399 RID: 13209
	public AudioSource audioSource;

	// Token: 0x0400339A RID: 13210
	public AudioClip lightOnSound;

	// Token: 0x0400339B RID: 13211
	public float lightOnSoundVolume;

	// Token: 0x0400339C RID: 13212
	private GRSconce.State state;

	// Token: 0x02000666 RID: 1638
	private enum State
	{
		// Token: 0x0400339E RID: 13214
		Off,
		// Token: 0x0400339F RID: 13215
		On
	}
}
