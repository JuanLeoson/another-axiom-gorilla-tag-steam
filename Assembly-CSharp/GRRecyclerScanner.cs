using System;
using TMPro;
using UnityEngine;

// Token: 0x0200065A RID: 1626
public class GRRecyclerScanner : MonoBehaviour
{
	// Token: 0x060027C8 RID: 10184 RVA: 0x000D6786 File Offset: 0x000D4986
	private void Awake()
	{
		this.toolText.text = "";
		this.ratesText.text = "";
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Update()
	{
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000D67A8 File Offset: 0x000D49A8
	public void ScanItem(GRRecycler.GRToolType toolType)
	{
		int num = 0;
		switch (toolType)
		{
		case GRRecycler.GRToolType.Club:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.Club);
			this.toolText.text = "Baton";
			break;
		case GRRecycler.GRToolType.Collector:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.Collector);
			this.toolText.text = "Collector";
			break;
		case GRRecycler.GRToolType.Flash:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.Flash);
			this.toolText.text = "Flash";
			break;
		case GRRecycler.GRToolType.Lantern:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.Lantern);
			this.toolText.text = "Lantern";
			break;
		case GRRecycler.GRToolType.Revive:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.Revive);
			this.toolText.text = "Revive";
			break;
		case GRRecycler.GRToolType.ShieldGun:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.ShieldGun);
			this.toolText.text = "Shield";
			break;
		case GRRecycler.GRToolType.DirectionalShield:
			num = this.recycler.GetRecycleValue(GRRecycler.GRToolType.DirectionalShield);
			this.toolText.text = "Deflector";
			break;
		}
		this.ratesText.text = (num.ToString("D2") ?? "");
		this.audioSource.volume = this.recyclerBarcodeAudioVolume;
		this.audioSource.PlayOneShot(this.recyclerBarcodeAudio);
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000D6900 File Offset: 0x000D4B00
	private void OnTriggerEnter(Collider other)
	{
		if (this.recycler.reactor == null)
		{
			return;
		}
		if (!this.recycler.reactor.grManager.IsAuthority())
		{
			return;
		}
		if (other.gameObject.GetComponentInParent<GRTool>() == null)
		{
			return;
		}
		GRRecycler.GRToolType toolType = GRRecycler.GRToolType.None;
		if (other.gameObject.GetComponentInParent<GRToolClub>() != null)
		{
			toolType = GRRecycler.GRToolType.Club;
		}
		else if (other.gameObject.GetComponentInParent<GRToolCollector>() != null)
		{
			toolType = GRRecycler.GRToolType.Collector;
		}
		else if (other.gameObject.GetComponentInParent<GRToolFlash>() != null)
		{
			toolType = GRRecycler.GRToolType.Flash;
		}
		else if (other.gameObject.GetComponentInParent<GRToolLantern>() != null)
		{
			toolType = GRRecycler.GRToolType.Lantern;
		}
		else if (other.gameObject.GetComponentInParent<GRToolRevive>() != null)
		{
			toolType = GRRecycler.GRToolType.Revive;
		}
		else if (other.gameObject.GetComponentInParent<GRToolShieldGun>() != null)
		{
			toolType = GRRecycler.GRToolType.ShieldGun;
		}
		else if (other.gameObject.GetComponentInParent<GRToolDirectionalShield>() != null)
		{
			toolType = GRRecycler.GRToolType.DirectionalShield;
		}
		this.recycler.reactor.grManager.RequestRecycleScanItem(toolType);
	}

	// Token: 0x04003340 RID: 13120
	public GRRecycler recycler;

	// Token: 0x04003341 RID: 13121
	[SerializeField]
	private TextMeshPro toolText;

	// Token: 0x04003342 RID: 13122
	[SerializeField]
	private TextMeshPro ratesText;

	// Token: 0x04003343 RID: 13123
	public AudioSource audioSource;

	// Token: 0x04003344 RID: 13124
	public AudioClip recyclerBarcodeAudio;

	// Token: 0x04003345 RID: 13125
	public float recyclerBarcodeAudioVolume = 0.5f;
}
