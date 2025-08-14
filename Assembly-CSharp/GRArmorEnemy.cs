using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000600 RID: 1536
public class GRArmorEnemy : MonoBehaviour
{
	// Token: 0x060025C5 RID: 9669 RVA: 0x000CA838 File Offset: 0x000C8A38
	private void Awake()
	{
		this.SetHp(0);
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000CA841 File Offset: 0x000C8A41
	public void SetHp(int hp)
	{
		this.hp = hp;
		this.RefreshArmor();
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000CA850 File Offset: 0x000C8A50
	private void RefreshArmor()
	{
		bool flag = this.hp > 0;
		GRArmorEnemy.Hide(this.renderers, !flag);
		if (flag && this.armorStateMaterials.Count > 0 && this.armorStateMaterials.Count == this.armorStateThresholds.Length)
		{
			Material material = this.armorStateMaterials[0];
			int num = 0;
			while (num < this.armorStateMaterials.Count && this.hp <= this.armorStateThresholds[num])
			{
				material = this.armorStateMaterials[num];
				if (this.hp == this.armorStateThresholds[num])
				{
					break;
				}
				num++;
			}
			if (material != this.renderers[0].material)
			{
				this.renderers[0].material = material;
				this.SetArmorColor(this.GetArmorColor());
			}
		}
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000CA92C File Offset: 0x000C8B2C
	public void SetArmorColor(Color newColor)
	{
		if (this.renderers != null && this.renderers.Count > 0)
		{
			this.renderers[0].material.SetColor("_BaseColor", newColor);
		}
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000CA960 File Offset: 0x000C8B60
	public Color GetArmorColor()
	{
		Color result = Color.white;
		if (this.renderers.Count > 0)
		{
			result = this.renderers[0].material.GetColor("_BaseColor");
		}
		return result;
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000CA9A0 File Offset: 0x000C8BA0
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000CA9E1 File Offset: 0x000C8BE1
	public void PlayHitFx(Vector3 position)
	{
		this.PlayFx(this.fxHit, position);
		this.PlaySound(this.hitSound, this.hitSoundVolume, position);
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000CAA03 File Offset: 0x000C8C03
	public void PlayBlockFx(Vector3 position)
	{
		this.PlayFx(this.fxBlock, position);
		this.PlaySound(this.blockSound, this.blockSoundVolume, position);
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000CAA25 File Offset: 0x000C8C25
	public void PlayDestroyFx(Vector3 position)
	{
		this.PlayFx(this.fxDestroy, position);
		this.PlaySound(this.destroySound, this.destroySoundVolume, position);
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000CAA47 File Offset: 0x000C8C47
	private void PlayFx(GameObject fx, Vector3 position)
	{
		if (fx == null)
		{
			return;
		}
		fx.SetActive(false);
		fx.SetActive(true);
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000CAA61 File Offset: 0x000C8C61
	private void PlaySound(AudioClip clip, float volume, Vector3 position)
	{
		this.audioSource.clip = clip;
		this.audioSource.volume = volume;
		this.audioSource.Play();
	}

	// Token: 0x04002FD8 RID: 12248
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x04002FD9 RID: 12249
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002FDA RID: 12250
	[SerializeField]
	private GameObject fxHit;

	// Token: 0x04002FDB RID: 12251
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x04002FDC RID: 12252
	[SerializeField]
	private float hitSoundVolume;

	// Token: 0x04002FDD RID: 12253
	[SerializeField]
	private GameObject fxBlock;

	// Token: 0x04002FDE RID: 12254
	[SerializeField]
	private AudioClip blockSound;

	// Token: 0x04002FDF RID: 12255
	[SerializeField]
	private float blockSoundVolume;

	// Token: 0x04002FE0 RID: 12256
	[SerializeField]
	private GameObject fxDestroy;

	// Token: 0x04002FE1 RID: 12257
	[SerializeField]
	private AudioClip destroySound;

	// Token: 0x04002FE2 RID: 12258
	[SerializeField]
	private float destroySoundVolume;

	// Token: 0x04002FE3 RID: 12259
	[SerializeField]
	private List<Material> armorStateMaterials;

	// Token: 0x04002FE4 RID: 12260
	[SerializeField]
	private int[] armorStateThresholds;

	// Token: 0x04002FE5 RID: 12261
	private int hp;
}
