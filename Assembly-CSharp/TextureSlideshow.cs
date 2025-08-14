using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class TextureSlideshow : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x00004ECC File Offset: 0x000030CC
	private void Awake()
	{
		this._renderer = base.GetComponent<Renderer>();
		this._renderer.material.mainTexture = this.textures[0];
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004EF2 File Offset: 0x000030F2
	private void OnEnable()
	{
		base.StartCoroutine(this.runSlideshow());
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00004F01 File Offset: 0x00003101
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00004F09 File Offset: 0x00003109
	private IEnumerator runSlideshow()
	{
		yield return new WaitForSecondsRealtime(this.prePause);
		int i = 0;
		for (;;)
		{
			yield return new WaitForSecondsRealtime(Random.Range(this.minMaxPause.x, this.minMaxPause.y));
			this._renderer.material.mainTexture = this.textures[i];
			i = (i + 1) % this.textures.Length;
		}
		yield break;
	}

	// Token: 0x040000B9 RID: 185
	private Renderer _renderer;

	// Token: 0x040000BA RID: 186
	[SerializeField]
	private Texture[] textures;

	// Token: 0x040000BB RID: 187
	[SerializeField]
	private Vector2 minMaxPause;

	// Token: 0x040000BC RID: 188
	[SerializeField]
	private float prePause = 1f;
}
