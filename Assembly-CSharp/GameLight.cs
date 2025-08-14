using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class GameLight : MonoBehaviour
{
	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06002408 RID: 9224 RVA: 0x000C1326 File Offset: 0x000BF526
	// (set) Token: 0x06002409 RID: 9225 RVA: 0x000C132E File Offset: 0x000BF52E
	public float InitialIntensity { get; private set; }

	// Token: 0x0600240A RID: 9226 RVA: 0x000C1337 File Offset: 0x000BF537
	private void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		}
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000C1355 File Offset: 0x000BF555
	private void Start()
	{
		this.InitialIntensity = this.light.intensity;
		this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		this.initialized = true;
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000C1383 File Offset: 0x000BF583
	private void OnDisable()
	{
		GameLightingManager.instance.RemoveGameLight(this);
	}

	// Token: 0x04002D79 RID: 11641
	public Light light;

	// Token: 0x04002D7A RID: 11642
	public bool negativeLight;

	// Token: 0x04002D7B RID: 11643
	public int lightId;

	// Token: 0x04002D7C RID: 11644
	private bool initialized;
}
